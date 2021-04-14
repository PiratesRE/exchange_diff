using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Search;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetGroupRequestWrapper
	{
		[DataMember(Name = "itemId")]
		public ItemId ItemId { get; set; }

		[DataMember(Name = "adObjectId")]
		public string AdObjectId { get; set; }

		[DataMember(Name = "emailAddress")]
		public EmailAddressWrapper EmailAddress { get; set; }

		[DataMember(Name = "paging")]
		public IndexedPageView Paging { get; set; }

		[DataMember(Name = "resultSet")]
		public GetGroupResultSet ResultSet { get; set; }
	}
}
