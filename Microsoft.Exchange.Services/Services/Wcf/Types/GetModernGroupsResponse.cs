using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetModernGroupsResponse
	{
		[DataMember(Name = "JoinedGroups", IsRequired = true)]
		public ModernGroupType[] JoinedGroups { get; set; }

		[DataMember(Name = "PinnedGroups", IsRequired = true)]
		public ModernGroupType[] PinnedGroups { get; set; }

		[DataMember(Name = "RecommendedGroups", IsRequired = true)]
		public ModernGroupType[] RecommendedGroups { get; set; }

		[DataMember(Name = "IsModernGroupsAddressListPresent", IsRequired = true)]
		public bool IsModernGroupsAddressListPresent { get; set; }
	}
}
