using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class FreeBusyResponse
	{
		public ResponseMessage ResponseMessage
		{
			get
			{
				return this.responseMessageField;
			}
			set
			{
				this.responseMessageField = value;
			}
		}

		public FreeBusyView FreeBusyView
		{
			get
			{
				return this.freeBusyViewField;
			}
			set
			{
				this.freeBusyViewField = value;
			}
		}

		private FreeBusyView freeBusyViewField;

		private ResponseMessage responseMessageField;
	}
}
