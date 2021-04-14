using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetUserAvailabilityResponse
	{
		[XmlArrayItem(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", IsNullable = false)]
		public FreeBusyResponse[] FreeBusyResponseArray
		{
			get
			{
				return this.freeBusyResponseArrayField;
			}
			set
			{
				this.freeBusyResponseArrayField = value;
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", IsNullable = false)]
		public SuggestionsResponse SuggestionsResponse
		{
			get
			{
				return this.suggestionsResponseField;
			}
			set
			{
				this.suggestionsResponseField = value;
			}
		}

		private FreeBusyResponse[] freeBusyResponseArrayField;

		private SuggestionsResponse suggestionsResponseField;
	}
}
