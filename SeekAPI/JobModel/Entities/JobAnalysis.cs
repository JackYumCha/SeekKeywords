using System;
using System.Collections.Generic;
using MvcAngular;
using ArangoDB.Client;

namespace JobModel.Entities
{
    [AngularType]
    public class JobAnalysis: VertexBase
    {
        /// <summary>
        /// Title is the keyword to search
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Keywords are the words to analyze
        /// </summary>
        public List<string> Keywords { get; set; }
    }
}
