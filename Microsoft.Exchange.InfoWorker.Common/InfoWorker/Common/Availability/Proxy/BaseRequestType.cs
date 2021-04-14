using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(GetUserAvailabilityRequest))]
	[Serializable]
	public class BaseRequestType
	{
		public string XmlElementName
		{
			get
			{
				return this.xmlElementNameField;
			}
			set
			{
				this.xmlElementNameField = value;
			}
		}

		private string xmlElementNameField;
	}
}
