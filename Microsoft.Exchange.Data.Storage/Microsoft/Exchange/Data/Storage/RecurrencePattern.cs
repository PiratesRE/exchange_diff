using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RecurrencePattern : IEquatable<RecurrencePattern>
	{
		public override string ToString()
		{
			return this.When();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is RecurrencePattern)
			{
				return this.Equals((RecurrencePattern)obj);
			}
			return base.Equals(obj);
		}

		public bool Equals(RecurrencePattern value)
		{
			return this.Equals(value, false);
		}

		public abstract bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth);

		internal abstract LocalizedString When();

		internal RecurrenceObjectType RecurrenceObjectType
		{
			get
			{
				return this.recurrenceObjectType;
			}
			set
			{
				this.recurrenceObjectType = value;
			}
		}

		internal virtual RecurrenceType MapiRecurrenceType
		{
			get
			{
				return RecurrenceType.None;
			}
		}

		private RecurrenceObjectType recurrenceObjectType;
	}
}
