using Microsoft.Xrm.Sdk;
using System;

namespace RRDemo.Plugins
{
    internal class DateRange : IEquatable<DateRange>
    {
        private Entity _timeentry;

        public DateRange(Entity entity)
        {
            _timeentry = entity;
        }

        public DateRange(DateTime start, DateTime end, DateTime date, Guid resourceId) : this(new Entity(Constants.TIME_ENTRY))
        {
            Start = start;
            End = end;
            Date = date;
            ResourceId = resourceId;
            Duration = (int)End.Subtract(Start).TotalMinutes;
        }

        public bool Equals(DateRange other)
        {
            return this.Date == other.Date;
        }

        public override int GetHashCode()
        {
            return (int)(Date - new DateTime(1900, 1, 1)).TotalDays;
        }

        public DateTime Start
        {
            private set
            {
                _timeentry[Constants.START] = value;
            }
            get
            => _timeentry.GetAttributeValue<DateTime>(Constants.START);
        }
        public DateTime End
        {
            private set
            {
                _timeentry[Constants.END] = value;
            }
            get
            => _timeentry.GetAttributeValue<DateTime>(Constants.END);
        }
        public DateTime Date
        {
            private set
            {
                _timeentry[Constants.DATE] = value;
            }
            get => _timeentry.GetAttributeValue<DateTime>(Constants.DATE);
        }
        public int Duration
        {
            private set
            {
                _timeentry[Constants.DURATION] = value;
            }
            get => _timeentry.GetAttributeValue<int>(Constants.DURATION);
        }

        public Guid ResourceId
        {
            private set
            {
                _timeentry[Constants.RESOURCE] = new EntityReference(Constants.BOOKABLE_RESOURCE, value);
            }
            get => _timeentry.GetAttributeValue<EntityReference>(Constants.RESOURCE).Id;
        }

        public Entity Entity
        {
            get
            {
                return _timeentry;
            }
        }
    }
}
