using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetGroupInfoRequest
	{
		[DataMember]
		public ItemId ItemId { get; set; }

		[DataMember]
		public string AdObjectId { get; set; }

		[DataMember]
		public EmailAddressWrapper EmailAddress { get; set; }

		[DataMember]
		public IndexedPageView Paging { get; set; }

		[DataMember]
		public GetGroupResultSet ResultSet { get; set; }

		[DataMember]
		public TargetFolderId ParentFolderId { get; set; }
	}
}
