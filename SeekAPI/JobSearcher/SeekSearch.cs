using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;
using JobModel.Entities;
using JobModel.AutoFac;
using ArangoDB.Client;
using System.Web;
using Serilog;
using HtmlAgilityPack;
using System.Threading;

namespace JobSearcher
{
    public class SeekSearch
    {
        private readonly ArangoConnection _arangoConnection;
        private readonly SearchOptions _searchOptions;
        private readonly ILogger _logger;

        private Regex jobLink = new Regex(@"https:\/\/www\.seek\.com\.au\/job\/(\d+)");

        public SeekSearch(ArangoConnection arangoConnection, SearchOptions searchOptions, ILogger logger) // ChromeDriver chromeDriver, 
        {
            _arangoConnection = arangoConnection;
            _searchOptions = searchOptions;
            _logger = logger;
        }

        public async Task SearchAllAnalysis()
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            using (var client = _arangoConnection.CreateClient())
            {
                var jobs = await client.Query<JobAnalysis>().ToListAsync();
                foreach(var job in jobs)
                {
                    
                    bool success = false;
                    int retry = 0;
                    while(!success && retry < _searchOptions.MaxRetry)
                    {
                        try
                        {
                            retry++;
                            await Search(job, client, timeStamp);
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, $"search {job.Title}.  {retry} of {_searchOptions.MaxRetry} attempts.");
                            Thread.Sleep(100);
                        }
                    }
                }
            }
        }

        public async Task Search(JobAnalysis jobAnalysis, IArangoDatabase client, string timeStamp)
        {
            var urls = SearchJob(jobAnalysis);

            Dictionary<string, Job> jobs = new Dictionary<string, Job>();

            JobAnalysisEntry jobAnalysisEntry = new JobAnalysisEntry()
            {
                _key = $"{jobAnalysis._key}__{timeStamp}",
                AnalysisTime = DateTime.Now,
                KeywordStatistics = new Dictionary<string, int>(),
                TotalJobs = urls.Count,
            };

            int urlIndex = 0;
            // download jobs and add edge to entry
            foreach (var url in urls)
            {
                urlIndex++;
                _logger.Information($"Download Job Url ({urlIndex} of {urls.Count}): {url}");
                await DownloadJob(url, jobAnalysisEntry, client, jobs);
            }

            AnalyzeJobs(jobAnalysis, jobAnalysisEntry, jobs);

            client.UpsertIgnoreNull(jobAnalysisEntry);

            client.UpsertEdge<EntryOf, JobAnalysisEntry, JobAnalysis>(jobAnalysisEntry, jobAnalysis);

            // add edges to jobs;

            foreach(var job in jobs.Values)
            {
                client.UpsertEdge<JobAnalysisOf, JobAnalysisEntry, Job>(jobAnalysisEntry, job);
            }
        }

        public List<string> SearchJob(JobAnalysis jobAnalysis)
        {
            _logger.Information($"Search Job: {jobAnalysis.Title}");

            List<string> urls = new List<string>();

            string keyWords = jobAnalysis.Title;

            HtmlDocument document = null;

            Func<bool> hasNext = new Func<bool>(() =>
                document.DocumentNode.DescendantsAndSelf().Any(n =>
                    n.Name.ToLower() == "a" && n.GetAttributeValue("data-automation", null) == "page-next"
                )
            );

            int index = 1;

            do
            {
                // get total number of jobs
                _logger.Information($"  Search Job: {jobAnalysis.Title} - Page {index}");

                List<HtmlNode> urlNodes = null;
                do
                {
                    int chromeRetry = 0;
                    var url = $"https://www.seek.com.au/{keyWords.Replace(" ", "-")}-jobs?page={index}";
                    try
                    {
                        chromeRetry++;

                        document = WebExtensions.LoadPage(url);
                        urlNodes = document.DocumentNode.DescendantsAndSelf()
                            .Where(n =>
                                n.Name.ToLower() == "a" &&
                                n.GetAttributeValue("data-automation", "") == "jobTitle").ToList();

                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"failed to get job list elements from url {url}. {chromeRetry} of {_searchOptions.MaxRetry} attempts.");
                    }
                } while (urlNodes == null);



                foreach (var urlNode in urlNodes)
                {
                    var href = urlNode.GetAttributeValue("href", null);
                    if(href != null)
                        urls.Add($@"https://www.seek.com.au{href}");
                }
           
                index++;

            } while (hasNext());

            return urls;
        }

        public async Task DownloadJob(string url, JobAnalysisEntry jobAnalysisEntry, IArangoDatabase client, Dictionary<string, Job> jobs)
        {
            

            var match = jobLink.Match(url);
            var key = match.Groups[1].Value;

            // if job has been saved in database, don't do it again.
            bool exists = false;
            {
                bool success = false;
                int retry = 0;
                while (!success && retry < _searchOptions.MaxRetry)
                {
                    try
                    {
                        retry++;
                        var jobsFound = await client.Query<Job>().Filter(j => j._key == key).ToListAsync();
                        var first = jobsFound.FirstOrDefault();
                        if (first != null && first.Description != null && first.Description != "")
                        {
                            jobs.Add(first._key, first);
                            exists = true;
                        }
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"failed to access job({key}) from arango. {retry} of {_searchOptions.MaxRetry} attempts.");
                        Thread.Sleep(100);
                    }
                }
            }

            if (!exists)
            {
                bool success = false;
                int retry = 0;

                HtmlDocument document = null;

                while (!success && retry < _searchOptions.MaxRetry)
                {
                    try
                    {
                        retry++;

                        HtmlNode jobDescription = null;
                        HtmlNode jobTitle = null;
                        HtmlNode workType = null;
                        HtmlNode publishDate = null;
                        HtmlNode location = null;
                        HtmlNode salary = null;
                        HtmlNode city = null;
                        int chromeRetry = 0;

                        do
                        {
                            try
                            {
                                chromeRetry++;

                                document = WebExtensions.LoadPage(url);

                                HtmlNode jobBox = document.DocumentNode.DescendantsAndSelf()
                                    .Where(n => n.Name.ToLower() == "div" && n.GetAttributeValue("data-automation", null) == "jobDescription")
                                    .FirstOrDefault();
                                jobDescription = jobBox.DescendantsAndSelf()
                                    .Where(n => n.Name.ToLower() == "div" && n.HasClass("templatetext")).FirstOrDefault();

                                jobTitle = jobBox.DescendantsAndSelf()
                                    .Where(n => n.HasClass("jobtitle")).FirstOrDefault();

                                HtmlNode infoHeader = document.DocumentNode.DescendantsAndSelf()
                                    .Where(n => n.Name.ToLower() == "section" && n.GetAttributeValue("aria-labelledby", null) == "jobInfoHeader")
                                    .FirstOrDefault();
                                 
                                publishDate = infoHeader.DescendantsAndSelf()
                                    .Where(n => n.Name.ToLower() == "dd" && n.GetAttributeValue("data-automation", null) == "job-detail-date").FirstOrDefault();

                                workType = infoHeader.DescendantsAndSelf()
                                    .Where(n => n.Name.ToLower() == "dd" && n.GetAttributeValue("data-automation", null) == "job-detail-work-type").FirstOrDefault();

                                HtmlNode dataList = infoHeader.DescendantsAndSelf()
                                    .Where(n => n.Name.ToLower() == "dl").FirstOrDefault();

                                var dataTitles = infoHeader.DescendantsAndSelf()
                                    .Where(n => n.Name.ToLower() == "dt").ToList();

                                var dataDetails = infoHeader.DescendantsAndSelf()
                                    .Where(n => n.Name.ToLower() == "dd").ToList();


                                for (int i = 0; i < Math.Min(dataTitles.Count, dataDetails.Count); i++)
                                {
                                    string innerText = dataTitles[i].InnerText;
                                    if (innerText.Contains("Location"))
                                    {
                                        location = dataDetails[i];
                                        city = location.DescendantsAndSelf().Where(n => n.Name.ToLower() == "strong").FirstOrDefault();
                                    }
                                    else if (innerText.Contains("Salary")) {
                                        salary = dataDetails[i];
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex, $"failed to get jobDescription element from url {url}. {chromeRetry} of {_searchOptions.MaxRetry} attempts.");
                            }
                        } while (jobDescription == null && chromeRetry < _searchOptions.MaxRetry);

                        Job job = new Job()
                        {
                            _key = key,
                            Description = jobDescription.InnerText.HtmlDecode(),
                            TimeStamp = DateTime.Now
                        };

                        if(jobTitle != null)
                        {
                            job.Title = jobTitle.InnerText.HtmlDecode();
                        }

                        if(location != null)
                        {
                            job.Location = location.InnerText.HtmlDecode();
                            if (city != null)
                            {
                                job.City = city.InnerText.HtmlDecode();
                            }
                        }

                        if(salary != null)
                        {
                            job.RawSalary = salary.InnerText.HtmlDecode();
                        }

                        if(workType != null)
                        {
                            job.WorkType = workType.InnerText.HtmlDecode();
                        }

                        if(publishDate != null)
                        {
                            job.RawPublishDate = publishDate.InnerText.HtmlDecode();
                            DateTime publish;
                            if(DateTime.TryParse(job.RawPublishDate, out publish))
                            {
                                job.PublishDate = publish;
                            }
                        }

                        if (jobs.ContainsKey(job._key))
                        {
                            jobs[job._key] = job;
                        }
                        else
                        {
                            jobs.Add(job._key, job);
                        }

                        // save to arango db

                        client.UpsertIgnoreNull(job);

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"failed to get job details from url {url}. {retry} of {_searchOptions.MaxRetry} attempts.");
                        Thread.Sleep(100);
                    }
                }
            }
        }

        public void AnalyzeJobs(JobAnalysis jobAnalysis, JobAnalysisEntry jobAnalysisEntry, Dictionary<string, Job> jobs)
        {
            _logger.Information($"Analyzing Jobs for {jobAnalysis.Title} Entry: {jobAnalysisEntry._key}");

            if (jobAnalysis.Keywords != null)
            {
                HashSet<string> words = new HashSet<string>(new StringComparer());

                foreach (string keyword in jobAnalysis.Keywords)
                {
                    words.Add(keyword);
                }

                jobAnalysisEntry.KeywordStatistics = new Dictionary<string, int>();

                foreach (string keyword in words)
                {
                    words.Add(keyword);
                    jobAnalysisEntry.KeywordStatistics.Add(keyword, 
                        jobs.Values.Count(j =>
                        Regex.IsMatch(j.Description, $@"(^|\W){keyword}(\W|$)", RegexOptions.IgnoreCase)
                        ));
                }
            }

            // city 
            jobAnalysisEntry.CityStatistics = new Dictionary<string, int>();
            foreach(var job in jobs.Values)
            {
                if (jobAnalysisEntry.CityStatistics.ContainsKey(job.City))
                {
                    jobAnalysisEntry.CityStatistics[job.City] += 1;
                }
                else
                {
                    jobAnalysisEntry.CityStatistics.Add(job.City, 1);
                }
            }

            // worktype
            jobAnalysisEntry.WorkTypeStatistics = new Dictionary<string, int>();
            foreach (var job in jobs.Values)
            {
                if (jobAnalysisEntry.WorkTypeStatistics.ContainsKey(job.WorkType))
                {
                    jobAnalysisEntry.WorkTypeStatistics[job.WorkType] += 1;
                }
                else
                {
                    jobAnalysisEntry.WorkTypeStatistics.Add(job.WorkType, 1);
                }
            }
        }
    }

    class StringComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Compare(x, y, true) == 0;
        }

        public int GetHashCode(string obj)
        {
            return obj.ToLower().GetHashCode();
        }
    }

    static class WebExtensions
    {
        public  static HtmlDocument LoadPage(string url)
        {
            using (WebClient client = new WebClient())
            {
                string html = client.DownloadString(new Uri(url));
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                return htmlDocument;
            }
        }

        public static string HtmlDecode(this string value)
        {
            return WebUtility.HtmlDecode(value);
        }
    }
}
