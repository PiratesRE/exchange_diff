using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class FreeBusyView
	{
		[XmlElement]
		[IgnoreDataMember]
		public FreeBusyViewType FreeBusyViewType
		{
			get
			{
				return this.view;
			}
			set
			{
				this.view = value;
			}
		}

		[DataMember(Name = "FreeBusyViewType")]
		[XmlIgnore]
		public string FreeBusyViewTypeString
		{
			get
			{
				return EnumUtilities.ToString<FreeBusyViewType>(this.FreeBusyViewType);
			}
			set
			{
				this.FreeBusyViewType = EnumUtilities.Parse<FreeBusyViewType>(value);
			}
		}

		[DataMember]
		[XmlElement(IsNullable = false)]
		public string MergedFreeBusy
		{
			get
			{
				return this.mergedFreeBusy;
			}
			set
			{
				if (value != null)
				{
					this.mergedFreeBusy = value;
				}
			}
		}

		[XmlArray(IsNullable = false)]
		[DataMember]
		[XmlArrayItem(Type = typeof(CalendarEvent), IsNullable = false)]
		public CalendarEvent[] CalendarEventArray
		{
			get
			{
				return this.calendarEventArray;
			}
			set
			{
				if (value != null)
				{
					this.calendarEventArray = value;
				}
			}
		}

		[DataMember]
		[XmlElement(IsNullable = false)]
		public WorkingHours WorkingHours
		{
			get
			{
				return this.workingHours;
			}
			set
			{
				if (value != null)
				{
					this.workingHours = value;
				}
			}
		}

		internal static FreeBusyView CreateFrom(FreeBusyQueryResult result)
		{
			if (result == null)
			{
				return null;
			}
			return new FreeBusyView
			{
				MergedFreeBusy = result.MergedFreeBusy,
				FreeBusyViewType = result.View,
				calendarEventArray = result.CalendarEventArray,
				workingHours = result.WorkingHours
			};
		}

		private FreeBusyView()
		{
		}

		private FreeBusyViewType view;

		private string mergedFreeBusy;

		private CalendarEvent[] calendarEventArray;

		private WorkingHours workingHours;
	}
}
