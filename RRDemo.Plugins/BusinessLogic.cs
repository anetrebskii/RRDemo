using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RRDemo.Plugins
{
    internal class BusinessLogic
    {
        private IOrganizationService _orgService;
        public BusinessLogic(IOrganizationService orgService)
        {
            _orgService = orgService;
        }

        public IEnumerable<DateRange> GetTimeEntriesToCreate(DateTime start, DateTime end, Guid resourceId)
        {
            var query = new QueryExpression(Constants.TIME_ENTRY)
            {
                ColumnSet = new ColumnSet(new[] { Constants.START, Constants.END, Constants.DATE, Constants.RESOURCE, Constants.DURATION }),
                Criteria = new FilterExpression
                {
                    Conditions = {
                            new ConditionExpression(Constants.DATE,ConditionOperator.Between, new[]{ start.Date, end.Date }),
                            new ConditionExpression(Constants.RESOURCE, ConditionOperator.Equal, resourceId)
                        }
                }
            };

            var existingRanges = _orgService.RetrieveMultiple(query).Entities.Select(x => new DateRange(x));

            var days = end.Date.Subtract(start.Date).Days + 1;

            return Enumerable.Range(0, days)
                 .Select(x =>
                    new DateRange(start.Date.AddDays(x), start.Date.AddDays(x + 1).AddMinutes(-1), start.AddDays(x).Date, resourceId)
                 )
                 .Except(existingRanges);
        }

        public bool TryValidateContext(IPluginExecutionContext context, out Entity timeEntry, out DateTime start, out DateTime end, out Guid resourceId)
        {
            start = end = new DateTime();
            resourceId = Guid.Empty;
            EntityReference resource;

            timeEntry = new Entity();

            if (context.Depth > 1)
                return false;

            timeEntry = context.InputParameters[Constants.TARGET] as Entity;

            if (timeEntry == null)
                return false;

            if (!timeEntry.TryGetAttributeValue(Constants.START, out start))
                return false;

            if (!timeEntry.TryGetAttributeValue(Constants.END, out end))
                return false;

            if (!timeEntry.TryGetAttributeValue(Constants.RESOURCE, out resource))
                return false;

            resourceId = resource.Id;

            return true;
        }

        public void Execute(Entity timeEntry, DateTime start, DateTime end, Guid resourceId)
        {
            CreateTimeEntries(timeEntry, GetTimeEntriesToCreate(start, end, resourceId));
        }

        private void CreateTimeEntries(Entity timeEntry, IEnumerable<DateRange> records)
        {
            var currentRecord = records.First();
            timeEntry[Constants.START] = currentRecord.Start;
            timeEntry[Constants.END] = currentRecord.End;
            timeEntry[Constants.DATE] = currentRecord.Date;
            timeEntry[Constants.DURATION] = currentRecord.Duration;

            foreach (var record in records.Skip(1))
                _orgService.Create(record.Entity);
        }
    }
}