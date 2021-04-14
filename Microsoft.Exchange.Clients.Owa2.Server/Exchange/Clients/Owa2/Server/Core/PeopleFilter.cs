using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class PeopleFilter
	{
		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public BaseFolderId FolderId { get; set; }

		[DataMember]
		public BaseFolderId ParentFolderId { get; set; }

		[DataMember]
		public int TotalCount { get; set; }

		[DataMember]
		internal int SortGroupPriority { get; set; }

		[DataMember]
		internal bool IsReadOnly { get; set; }
	}
}
