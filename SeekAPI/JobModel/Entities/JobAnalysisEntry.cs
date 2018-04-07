using System;
using System.Collections.Generic;
using System.Text;
using ArangoDB.Client;
using MvcAngular;

namespace JobModel.Entities
{
    /// <summary>
    /// this analysis should have edge connected to Job
    /// </summary>
    [AngularType]
    public class JobAnalysisEntry: VertexBase
    {
        /// <summary>
        /// The time when analysis is performed
        /// </summary>
        public DateTime AnalysisTime { get; set; }
        /// <summary>
        /// The keyword statistics Keyword -> Count
        /// </summary>
        public Dictionary<string, int> KeywordStatistics { get; set; }
        public Dictionary<string, int> WorkTypeStatistics { get; set; }
        public Dictionary<string, int> CityStatistics { get; set; }
        /// <summary>
        /// Total number of jobs found
        /// </summary>
        public int TotalJobs { get; set; }
    }
}
