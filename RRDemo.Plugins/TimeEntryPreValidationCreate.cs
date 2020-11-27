using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RRDemo.Plugins
{
    public class TimeEntryPreValidationCreate : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            var localContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var orgService = ((IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory))).CreateOrganizationService(localContext.UserId);

            DateTime start;
            DateTime end;
            Entity timeEntry;
            var logic = new BusinessLogic(orgService);

            if (!logic.TryValidateContext(localContext, out timeEntry, out start, out end))
                return;

            if (start == end)
                throw new InvalidPluginExecutionException($"msdyn_start can't be equal msdyn_end");

            var leftRecords = new BusinessLogic(orgService).GetTimeEntriesToCreate(start, end);

            if (leftRecords.Count() == 0)
                throw new InvalidPluginExecutionException($"Can't create duplicate time entry");
        }
    }
}
