using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "SearchMailboxesResponseMessage", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "SearchMailboxesResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SearchMailboxesResponseMessage : ResponseMessage
	{
		[XmlElement("SearchMailboxesResult")]
		[DataMember(Name = "SearchMailboxesResult", IsRequired = false)]
		public SearchMailboxesResult SearchMailboxesResult
		{
			get
			{
				return this.searchMailboxesResult;
			}
			set
			{
				this.searchMailboxesResult = value;
			}
		}

		public SearchMailboxesResponseMessage()
		{
		}

		internal SearchMailboxesResponseMessage(ServiceResultCode code, ServiceError error, SearchMailboxesResult results) : base(code, error)
		{
			this.searchMailboxesResult = results;
		}

		private SearchMailboxesResult searchMailboxesResult;
	}
}
