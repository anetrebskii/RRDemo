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

        public DateRange(DateTime start, DateTime end, DateTime date) : this(new Entity(Constants.TIME_ENTRY))
        {
            Start = start;
            End = end;
            Date = date;
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
            {
                return _timeentry.GetAttributeValue<DateTime>(Constants.START);
            }
        }
        public DateTime End
        {
            private set
            {
                _timeentry[Constants.END] = value;
            }
            get
            {
                return _timeentry.GetAttributeValue<DateTime>(Constants.END);
            }
        }
        public DateTime Date
        {
            private set
            {
                _timeentry[Constants.DATE] = value;
            }
            get
            {
                return _timeentry.GetAttributeValue<DateTime>(Constants.DATE);
            }
        }
        public int Duration
        {
            private set
            {
                _timeentry[Constants.DURATION] = value;
            }
            get
            {
                return _timeentry.GetAttributeValue<int>(Constants.DURATION);
            }
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
