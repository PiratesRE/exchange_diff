using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[XmlType("GetUserAvailabilityRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetUserAvailabilityRequest : BaseAvailabilityRequest
	{
		[DataMember]
		[XmlElement(IsNullable = false, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
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

		[DataMember]
		[XmlArray(IsNullable = false)]
		[XmlArrayItem(ElementName = "MailboxData", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public MailboxData[] MailboxDataArray
		{
			get
			{
				return this.mailboxDataArray;
			}
			set
			{
				this.mailboxDataArray = value;
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[DataMember]
		public FreeBusyViewOptions FreeBusyViewOptions
		{
			get
			{
				return this.freeBusyViewOptions;
			}
			set
			{
				this.freeBusyViewOptions = value;
			}
		}

		[DataMember]
		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public SuggestionsViewOptions SuggestionsViewOptions
		{
			get
			{
				return this.suggestionsViewOptions;
			}
			set
			{
				this.suggestionsViewOptions = value;
			}
		}

		[XmlIgnore]
		internal bool DefaultFreeBusyAccessOnly
		{
			get
			{
				return this.defaultFreeBusyAccessOnly;
			}
			set
			{
				this.defaultFreeBusyAccessOnly = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetUserAvailability(callContext, this);
		}

		private SerializableTimeZone timeZone;

		private MailboxData[] mailboxDataArray;

		private FreeBusyViewOptions freeBusyViewOptions;

		private SuggestionsViewOptions suggestionsViewOptions;

		private bool defaultFreeBusyAccessOnly;
	}
}
