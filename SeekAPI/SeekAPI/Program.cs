using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvcAngular.Generator;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace SeekAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // --generate-angular -o \"D:\VSTS\Repos\Machine Learning Lecture\Projects\Seek\SeekUI\src\app\services\mvc-api\" -a \"SeekAPI;JobModel\"
            //
            //
            //
            //args = new string[] {
            //    @"--generate-angular",
            //    "-o",
            //    @"D:\VSTS\Repos\Machine Learning Lecture\Projects\Seek\SeekUI\src\app\services\mvc-api\",
            //    "-a",
            //    "SeekAPI;JobModel"
            //};

            //Console.WriteLine($"Arguments: {string.Join(" ", args)}");
            if (AngularGenerator.ShouldRunMvc(args))
            {
                BuildWebHost(args).Run();
            }
        }

        public static void LoadDynamic()
        {
            
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }

    
}
