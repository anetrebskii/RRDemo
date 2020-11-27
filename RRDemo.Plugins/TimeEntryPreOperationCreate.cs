using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RRDemo.Plugins
{
    public class TimeEntryPreOperationCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var localContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var orgService = ((IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory))).CreateOrganizationService(localContext.UserId);

            DateTime start;
            DateTime end;
            Entity timeEntry;
            var logic = new BusinessLogic(orgService);

            if (logic.TryValidateContext(localContext, out timeEntry, out start, out end))
                logic.Execute(timeEntry, start, end);
        }
    }
}
