using System;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CalendarPropertyId
	{
		internal CalendarPropertyId(PropertyId propertyId, string propertyName)
		{
			this.propertyId = propertyId;
			this.propertyName = propertyName;
		}

		internal CalendarPropertyId(PropertyId propertyId) : this(propertyId, string.Empty)
		{
		}

		internal CalendarPropertyId(string propertyName) : this(PropertyId.Unknown, propertyName)
		{
		}

		internal PropertyId PropertyId
		{
			get
			{
				return this.propertyId;
			}
		}

		internal string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		internal object Key
		{
			get
			{
				if (this.propertyId == PropertyId.Unknown)
				{
					return this.propertyName;
				}
				return this.propertyId;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", this.propertyId, this.propertyName);
		}

		private readonly PropertyId propertyId;

		private readonly string propertyName;
	}
}
