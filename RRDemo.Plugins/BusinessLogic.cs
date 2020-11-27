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

        public IEnumerable<DateRange> GetTimeEntriesToCreate(DateTime start, DateTime end)
        {
            var query = new QueryExpression(Constants.TIME_ENTRY)
            {
                ColumnSet = new ColumnSet(new[] { Constants.START, Constants.END, Constants.DATE }),
                Criteria = new FilterExpression
                {
                    Conditions = {
                            new ConditionExpression(Constants.DATE,ConditionOperator.Between, new[]{ start.Date, end.Date })
                        }
                }
            };

            var existingRanges = _orgService.RetrieveMultiple(query).Entities.Select(x => new DateRange(x));

            var days = end.Date.Subtract(start.Date).Days + 1;

            return Enumerable.Range(0, days)
                 .Select(x =>
                    new DateRange(x == 0 ? start : start.Date.AddDays(x), x == days - 1 ? end : start.Date.AddDays(x + 1), start.AddDays(x).Date)
                 )
                 .Except(existingRanges);
        }

        public bool TryValidateContext(IPluginExecutionContext context, out Entity timeEntry, out DateTime start, out DateTime end)
        {
            start = end = new DateTime();
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

            return true;
        }

        public void Execute(Entity timeEntry, DateTime start, DateTime end)
        {
            CreateTimeEntries(timeEntry, start, end, GetTimeEntriesToCreate(start, end));
        }

        private void CreateTimeEntries(Entity timeEntry, DateTime start, DateTime end, IEnumerable<DateRange> records)
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
