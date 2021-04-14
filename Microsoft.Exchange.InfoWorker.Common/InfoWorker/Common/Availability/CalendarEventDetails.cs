using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarEventDetails
	{
		[DataMember]
		public string ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		[XmlElement(IsNullable = false)]
		[DataMember]
		public string Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		[XmlElement(IsNullable = false)]
		[DataMember]
		public string Location
		{
			get
			{
				return this.location;
			}
			set
			{
				this.location = value;
			}
		}

		[XmlElement(IsNullable = false)]
		[DataMember]
		public bool IsMeeting
		{
			get
			{
				return this.isMeeting;
			}
			set
			{
				this.isMeeting = value;
			}
		}

		[XmlElement(IsNullable = false)]
		[DataMember]
		public bool IsRecurring
		{
			get
			{
				return this.isRecurring;
			}
			set
			{
				this.isRecurring = value;
			}
		}

		[XmlElement(IsNullable = false)]
		[DataMember]
		public bool IsException
		{
			get
			{
				return this.isException;
			}
			set
			{
				this.isException = value;
			}
		}

		[XmlElement(IsNullable = false)]
		[DataMember]
		public bool IsReminderSet
		{
			get
			{
				return this.isReminderSet;
			}
			set
			{
				this.isReminderSet = value;
			}
		}

		[DataMember]
		public bool IsPrivate
		{
			get
			{
				return this.isPrivate;
			}
			set
			{
				this.isPrivate = value;
			}
		}

		private string id;

		private string subject;

		private string location;

		private bool isMeeting;

		private bool isRecurring;

		private bool isException;

		private bool isReminderSet;

		private bool isPrivate;
	}
}
