using System;
using System.Collections.Generic;
using System.Text;
using ArangoDB.Client;
using MvcAngular;

namespace JobModel.Entities
{
    [AngularType]
    public class Job: VertexBase
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string RawSalary { get; set; }
        public string City { get; set; }
        public string Location { get; set; }
        public string WorkType { get; set; }
        public string RawPublishDate { get; set; }
        public double? MinSalary { get; set; }
        public double? MaxSalary { get; set; }
        public double? MiddleSalary { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
