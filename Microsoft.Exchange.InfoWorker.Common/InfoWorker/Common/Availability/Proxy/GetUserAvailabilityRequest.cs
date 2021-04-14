using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetUserAvailabilityRequest : BaseRequestType
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public SerializableTimeZone TimeZone
		{
			get
			{
				return this.timeZone;
			}
			set
			{
				this.timeZone = value;
			}
		}

		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public MailboxData[] MailboxDataArray
		{
			get
			{
				return this.mailboxDataArrayField;
			}
			set
			{
				this.mailboxDataArrayField = value;
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public FreeBusyViewOptions FreeBusyViewOptions
		{
			get
			{
				return this.freeBusyViewOptionsField;
			}
			set
			{
				this.freeBusyViewOptionsField = value;
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public SuggestionsViewOptions SuggestionsViewOptions
		{
			get
			{
				return this.suggestionsViewOptionsField;
			}
			set
			{
				this.suggestionsViewOptionsField = value;
			}
		}

		private SerializableTimeZone timeZone;

		private MailboxData[] mailboxDataArrayField;

		private FreeBusyViewOptions freeBusyViewOptionsField;

		private SuggestionsViewOptions suggestionsViewOptionsField;
	}
}
