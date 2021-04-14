using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "CalendarView", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class CalendarPageView : BasePagingType
	{
		[XmlAttribute]
		[IgnoreDataMember]
		public DateTime StartDate { get; set; }

		[DataMember(Name = "StartDate", IsRequired = true)]
		[XmlIgnore]
		public string StartDateString
		{
			get
			{
				return this.StartDate.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				this.StartDate = DateTime.Parse(value);
			}
		}

		[IgnoreDataMember]
		[XmlAttribute]
		public DateTime EndDate { get; set; }

		[XmlIgnore]
		[DataMember(Name = "EndDate", IsRequired = true)]
		public string EndDateString
		{
			get
			{
				return this.EndDate.ToString(CultureInfo.InvariantCulture);
			}
			set
			{
				this.EndDate = DateTime.Parse(value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public ExDateTime StartDateEx
		{
			get
			{
				if (!this.startExDateTimeInitialized)
				{
					try
					{
						if (this.StartDate.Kind == DateTimeKind.Local)
						{
							this.startExDateTime = new ExDateTime(EWSSettings.RequestTimeZone, this.StartDate.ToUniversalTime());
						}
						else
						{
							this.startExDateTime = new ExDateTime(EWSSettings.RequestTimeZone, this.StartDate);
						}
						this.startExDateTimeInitialized = true;
					}
					catch (ArgumentOutOfRangeException innerException)
					{
						throw new ValueOutOfRangeException(innerException, CalendarPageView.messageXmlValuesNames, CalendarPageView.messageXmlValuesStartDate);
					}
				}
				return this.startExDateTime;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public ExDateTime EndDateEx
		{
			get
			{
				if (!this.endExDateTimeInitialized)
				{
					try
					{
						if (this.EndDate.Kind == DateTimeKind.Local)
						{
							this.endExDateTime = new ExDateTime(EWSSettings.RequestTimeZone, this.EndDate.ToUniversalTime());
						}
						else
						{
							this.endExDateTime = new ExDateTime(EWSSettings.RequestTimeZone, this.EndDate);
						}
						this.endExDateTimeInitialized = true;
					}
					catch (ArgumentOutOfRangeException innerException)
					{
						throw new ValueOutOfRangeException(innerException, CalendarPageView.messageXmlValuesNames, CalendarPageView.messageXmlValuesEndDate);
					}
				}
				return this.endExDateTime;
			}
		}

		internal BasePageResult ApplyPostQueryPaging(object[][] view)
		{
			return new BasePageResult(new NormalQueryView(view, base.MaxRows));
		}

		internal void Validate(CalendarFolder calendarFolder)
		{
			if (calendarFolder == null)
			{
				throw new CalendarExceptionFolderIsInvalidForCalendarView();
			}
			ExDateTime t = this.StartDateEx;
			ExDateTime exDateTime = this.EndDateEx;
			if (t > exDateTime)
			{
				throw new CalendarExceptionEndDateIsEarlierThanStartDate();
			}
			ExDateTime other = ExDateTime.MaxValue.AddYears(-2);
			if (t.CompareTo(other) < 0)
			{
				t = t.AddYears(2);
			}
			else
			{
				exDateTime = exDateTime.AddYears(-2);
			}
			if (t.CompareTo(exDateTime) < 0)
			{
				throw new CalendarExceptionViewRangeTooBig();
			}
		}

		private const int MaxCalendarViewRangeInYears = 2;

		private static string[] messageXmlValuesNames = new string[]
		{
			"Element",
			"Attribute"
		};

		private static string[] messageXmlValuesStartDate = new string[]
		{
			"CalendarView",
			"StartDate"
		};

		private static string[] messageXmlValuesEndDate = new string[]
		{
			"CalendarView",
			"EndDate"
		};

		private ExDateTime startExDateTime;

		private ExDateTime endExDateTime;

		private bool startExDateTimeInitialized;

		private bool endExDateTimeInitialized;
	}
}
