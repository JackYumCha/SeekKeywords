using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Xunit;
using JobModel.Entities;

namespace DataManagement
{
    public class SalaryAnalysis
    {

        [Fact(DisplayName = "Parse Salary")]
        public void ParseSalary()
        {
            string salaryFile = $@"{AppContext.BaseDirectory}\rawsalary.json";

            string json = File.ReadAllText(salaryFile);

            List<string> rawSalaries = JsonConvert.DeserializeObject<List<string>>(json);

            // we need to set up a set of rules to parse the json strings

            Regex rgxDigits = new Regex(@"\d+");

            rawSalaries = rawSalaries.Where(raw => rgxDigits.IsMatch(raw)).ToList();

            Func<string, Regex, AnnualSalary> convertExact = (string value, Regex rule) =>
            {
                Match match = rule.Match(value);
                if (match.Success)
                {
                    bool isDaily = match.Groups.Skip(2).Any(g => g.Value.ToLower() == "day");
                    bool hasK = false;
                    double price = match.Groups[1].Value.ToDouble(out hasK);
                    if (hasK)
                    {
                        price *= 1000d;
                    }
                    if (isDaily)
                    {
                        price *= 250d;
                    }
                    return new AnnualSalary()
                    {
                        Exact = price
                    };
                }
                return null;
            };

            Func<string, Regex, AnnualSalary> convertFromTo = (string value, Regex rule) =>
            {
                Match match = rule.Match(value);
                if (match.Success)
                {
                    bool hasKFrom = false, hasKTo = false;

                    bool isDaily = match.Groups.Skip(2).Any(g => g.Value.ToLower() == "day");

                    double from = match.Groups[1].Value.ToDouble(out hasKFrom);
                    double to = match.Groups[2].Value.ToDouble(out hasKTo);

                    bool hasK = hasKFrom || hasKTo;

                    if (hasK)
                    {
                        from *= 1000d;
                        to *= 1000d;
                    }
                    if (isDaily)
                    {
                        from *= 250d;
                        to *= 250d;
                    }
                    return new AnnualSalary()
                    {
                        From = from,
                        To = to
                    };
                }
                return null;
            };

            List<Regex> rulesExact = new List<Regex>()
            {
                new Regex(@"\$?([\d,\.]+k?)\s*incl\w*\s*super", RegexOptions.IgnoreCase),
                new Regex(@"\$?([\d,\.]+k?)\s*package", RegexOptions.IgnoreCase),
                new Regex(@"\$?([\d,\.]+k?)\s*(Base|)\s*(plus|\+)\s*super", RegexOptions.IgnoreCase),
                new Regex(@"\$?([\d,\.]+k?)\s*(per|a|\/)\s*(day)", RegexOptions.IgnoreCase),
                new Regex(@"\$?([\d,\.]+k?)p.a", RegexOptions.IgnoreCase),
                new Regex(@"\$([\d,\.]+k?)", RegexOptions.IgnoreCase),
            };

            List<Regex> rulesFromTo = new List<Regex>()
            {
                new Regex(@"\$?([\d,\.]+k?)\s*-\s*\$?([\d,\.]+k?)\s*base", RegexOptions.IgnoreCase),
                new Regex(@"\$?([\d,\.]+k?)\s*-\s*\$?([\d,\.]+k?)\s*p.a.", RegexOptions.IgnoreCase),
                new Regex(@"\$?([\d,\.]+k?)\s*-\s*\$?([\d,\.]+k?)\s*pkg", RegexOptions.IgnoreCase),
                new Regex(@"\$?([\d,\.]+k?)\s*-\s*\$?([\d,\.]+k?)\s*(per|\/)\s*(day)", RegexOptions.IgnoreCase),
                new Regex(@"\$([\d,\.]+k?)\s*-\s*\$?([\d,\.]+k?)", RegexOptions.IgnoreCase),
            };

            List<AnnualSalary> annualSalaries = new List<AnnualSalary>();

            List<string> succeeded = new List<string>();

            foreach (var rule in rulesFromTo)
            {
                List<string> failed = new List<string>();

                foreach(var value in rawSalaries)
                {
                    var salary = convertFromTo(value, rule);
                    if(salary == null)
                    {
                        failed.Add(value);
                    }
                    else
                    {
                        annualSalaries.Add(salary);
                        succeeded.Add(value);
                    }
                }
                rawSalaries = failed;
            }

            foreach (var rule in rulesExact)
            {
                List<string> failed = new List<string>();

                foreach (var value in rawSalaries)
                {
                    var salary = convertExact(value, rule);
                    if (salary == null)
                    {
                        failed.Add(value);
                    }
                    else
                    {
                        annualSalaries.Add(salary);
                        succeeded.Add(value);
                    }
                }
                rawSalaries = failed;
            }

            Debugger.Break();
        }
    }

    public static class NumberConvert
    {
        public static double ToDouble(this string value, out bool hasK)
        {
            // 150 150k 200.3 100,000.24

            Regex regexK = new Regex("k$", RegexOptions.IgnoreCase);

            hasK = regexK.IsMatch(value);

            value = regexK.Replace(value.Replace(",", "").Replace(" ", ""), "");

            double result = double.NaN;

            if(double.TryParse(value, out result))
            {
                return result;
            }
            throw new Exception($"failed to convert string to double: {value}");
        }
    }


}
