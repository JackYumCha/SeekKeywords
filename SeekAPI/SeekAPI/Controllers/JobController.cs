using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobModel.AutoFac;
using JobModel.Entities;
using ArangoDB.Client;
using MvcAngular;

namespace SeekAPI.Controllers
{
    [Angular, Route("[controller]/[action]")]
    public class JobController: Controller
    {
        private readonly ArangoConnection _arangoConnection;

        public JobController(ArangoConnection arangoConnection)
        {
            _arangoConnection = arangoConnection;
        }

        [HttpPost]
        public async Task<List<JobAnalysis>> ListJobAnalysis()
        {
            using (var client = _arangoConnection.CreateClient())
            {
                var results = await client.Query<JobAnalysis>().ToListAsync();
                return results.Select(r => new JobAnalysis()
                {
                    _key = r._key,
                    Title = r.Title
                }).ToList();
            }
        }

        [HttpPost]
        public async Task<JobAnalysis> GetJobAnalysis(string jobAnalysisKey)
        {
            using (var client = _arangoConnection.CreateClient())
            {
                var results = await client.Query<JobAnalysis>().Filter(ja => ja._key == jobAnalysisKey).ToListAsync();
                return results.FirstOrDefault();
            }
        }

        [HttpPost]
        public List<JobAnalysisEntry> ListJobAnalysisEntries(string jobAnalysisKey)
        {
            using (var client = _arangoConnection.CreateClient())
            {
                string id = $"{nameof(JobAnalysis)}/{jobAnalysisKey}";
                var results = client.TraverseEdgeFromOriginInBound(id, 1, 1, typeof(EntryOf));
                return results.Select(r =>
                {
                    var entry = r.Vertex.ToObject<JobAnalysisEntry>();
                    return new JobAnalysisEntry()
                    {
                        _key = entry._key,
                        AnalysisTime = entry.AnalysisTime
                    };
                })
                .OrderByDescending(entry => entry.AnalysisTime)
                .Take(100)
                .ToList();
            }
        }

        [HttpPost]
        public async Task<JobAnalysisEntry> GetJobAnalysisEntry(string jobAnalysisEntryKey)
        {
            using (var client = _arangoConnection.CreateClient())
            {
                var results = await client.Query<JobAnalysisEntry>().Filter(ja => ja._key == jobAnalysisEntryKey).ToListAsync();
                return results.FirstOrDefault();
            }
        }
    }
}
