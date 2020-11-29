using Microsoft.Xrm.Sdk;
using System;

namespace RRDemo.Plugins
{
    public class TimeEntryPreOperationCreate : PluginBase
    {
        public TimeEntryPreOperationCreate(string unsecure, string secure)
           : base(typeof(TimeEntryPreOperationCreate))
        {

        }

        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            DateTime start;
            DateTime end;
            Guid resourceId;
            Entity timeEntry;
            var logic = new BusinessLogic(localContext.OrganizationService);

            if (logic.TryValidateContext(localContext.PluginExecutionContext, out timeEntry, out start, out end, out resourceId))
                logic.Execute(timeEntry, start, end, resourceId);
        }
    }
}