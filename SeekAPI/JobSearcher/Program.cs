using System;
using JobModel.AutoFac;
using Autofac;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace JobSearcher
{
    public class Program
    {
        public static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            Console.WriteLine("Seek Job Keywords Analysis!");

            AutoFacContainer autoFacContainer = new AutoFacContainer();

            autoFacContainer.ContainerBuilder
                .RegisterInstance(autoFacContainer.Configuration.GetSection(nameof(SearchOptions)).Get<SearchOptions>());

            autoFacContainer.ContainerBuilder.RegisterType<SeekSearch>();

            Container = autoFacContainer.ContainerBuilder.Build();

            var search = Container.Resolve<SeekSearch>();

            search.SearchAllAnalysis().Wait();
        }

    }
}
