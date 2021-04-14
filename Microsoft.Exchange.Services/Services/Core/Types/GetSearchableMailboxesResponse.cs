using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetSearchableMailboxesResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class GetSearchableMailboxesResponse : ResponseMessage
	{
		public GetSearchableMailboxesResponse()
		{
		}

		internal GetSearchableMailboxesResponse(ServiceResultCode code, ServiceError error, SearchableMailbox[] mailboxes, FailedSearchMailbox[] failedMailboxes) : base(code, error)
		{
			if (mailboxes != null && mailboxes.Length > 0)
			{
				this.searchableMailboxes.AddRange(mailboxes);
			}
			if (failedMailboxes != null && failedMailboxes.Length > 0)
			{
				this.FailedMailboxes = failedMailboxes;
			}
		}

		[DataMember(Name = "SearchableMailboxes", IsRequired = false)]
		[XmlArray]
		[XmlArrayItem("SearchableMailbox", Type = typeof(SearchableMailbox), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public SearchableMailbox[] SearchableMailboxes
		{
			get
			{
				return this.searchableMailboxes.ToArray();
			}
			set
			{
				this.searchableMailboxes.Clear();
				if (value != null && value.Length > 0)
				{
					this.searchableMailboxes.AddRange(value);
				}
			}
		}

		[XmlArray]
		[DataMember(Name = "FailedMailboxes", EmitDefaultValue = false, IsRequired = false)]
		[XmlArrayItem(ElementName = "FailedMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(FailedSearchMailbox))]
		public FailedSearchMailbox[] FailedMailboxes { get; set; }

		private List<SearchableMailbox> searchableMailboxes = new List<SearchableMailbox>();
	}
}
