using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public sealed class GetFederatedDirectoryGroupResponse : ResponseMessage
	{
		public FederatedDirectoryIdentityDetailsType[] Members { get; set; }

		public FederatedDirectoryIdentityDetailsType[] Owners { get; set; }
	}
}
