using Microsoft.Xrm.Sdk;
using System;
using System.Linq;

namespace RRDemo.Plugins
{
    public class TimeEntryPreValidationCreate : PluginBase
    {
        public TimeEntryPreValidationCreate(string unsecure, string secure)
           : base(typeof(TimeEntryPreValidationCreate))
        {

        }

        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            DateTime start;
            DateTime end;
            Entity timeEntry;
            var logic = new BusinessLogic(localContext.OrganizationService);

            if (!logic.TryValidateContext(localContext.PluginExecutionContext, out timeEntry, out start, out end))
                return;

            if (start == end)
                throw new InvalidPluginExecutionException($"msdyn_start can't be equal msdyn_end");

            var leftRecords = new BusinessLogic(localContext.OrganizationService).GetTimeEntriesToCreate(start, end);

            if (leftRecords.Count() == 0)
                throw new InvalidPluginExecutionException($"Can't create duplicate time entry");
        }
    }
}
