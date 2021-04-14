using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class GetUserAvailabilityRequestType : BaseRequestType
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public SerializableTimeZone TimeZone
		{
			get
			{
				return this.timeZoneField;
			}
			set
			{
				this.timeZoneField = value;
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
		public FreeBusyViewOptionsType FreeBusyViewOptions
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
		public SuggestionsViewOptionsType SuggestionsViewOptions
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

		private SerializableTimeZone timeZoneField;

		private MailboxData[] mailboxDataArrayField;

		private FreeBusyViewOptionsType freeBusyViewOptionsField;

		private SuggestionsViewOptionsType suggestionsViewOptionsField;
	}
}
