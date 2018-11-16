using System;
using Xunit;
using JobModel.AutoFac;
using Autofac;
using System.Linq;
using Microsoft.Extensions.Configuration;
using JobModel.Entities;
using ArangoDB.Client;

namespace DataManagement
{
    public class Management
    {
        [Fact(DisplayName = @"Clean Jobs Data")]
        public void CleanJobsData()
        {
            AutoFacContainer autoFacContainer = new AutoFacContainer();
            var container = autoFacContainer.ContainerBuilder.Build();
            var arangoConnection = container.Resolve<ArangoConnection>();

            using(var client = arangoConnection.CreateClient())
            {
                client.Query<Job>().Remove().Execute();
                client.Query<JobAnalysisOf>().Remove().Execute();
                client.Query<JobAnalysisEntry>().Remove().Execute();
                client.Query<EntryOf>().Remove().Execute();
            }
        }

        [Fact(DisplayName = "Create Collections")]
        public void CreateCollections()
        {

            AutoFacContainer autoFacContainer = new AutoFacContainer();
            var container = autoFacContainer.ContainerBuilder.Build();
            var arangoConnection = container.Resolve<ArangoConnection>();

            using (var client = arangoConnection.CreateClient())
            {
                client.CreateCollection(nameof(AnalysisOf), type: CollectionType.Edge);
                client.CreateCollection(nameof(EntryOf), type: CollectionType.Edge);
                client.CreateCollection(nameof(JobAnalysisOf), type: CollectionType.Edge);
                client.CreateCollection(nameof(SubCategoryOf), type: CollectionType.Edge);
                client.CreateCollection(nameof(Job), type: CollectionType.Document);
                client.CreateCollection(nameof(JobAnalysis), type: CollectionType.Document);
                client.CreateCollection(nameof(JobCategory), type: CollectionType.Document);
                client.CreateCollection(nameof(JobAnalysisEntry), type: CollectionType.Document);
            }
        }
    }
}
