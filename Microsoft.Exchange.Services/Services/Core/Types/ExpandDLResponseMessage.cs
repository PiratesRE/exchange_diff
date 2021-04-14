using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("ExpandDLResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ExpandDLResponseMessage : ResponseMessage
	{
		public ExpandDLResponseMessage()
		{
		}

		internal ExpandDLResponseMessage(ServiceResultCode code, ServiceError error, XmlNode dlExpansionSet) : base(code, error)
		{
			this.dlExpansionSet = dlExpansionSet;
		}

		[XmlAnyElement]
		public XmlNode DLExpansionSet
		{
			get
			{
				return this.dlExpansionSet;
			}
			set
			{
				this.dlExpansionSet = value;
			}
		}

		private XmlNode dlExpansionSet;
	}
}
