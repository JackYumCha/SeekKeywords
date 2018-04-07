using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Events;

namespace JobModel.AutoFac
{
    public class SerilogOptions
    {
        public LogEventLevel RollingFileLogEventLevel { get; set; }
        public LogEventLevel ConsoleLogEventLevel { get; set; }
    }
}
