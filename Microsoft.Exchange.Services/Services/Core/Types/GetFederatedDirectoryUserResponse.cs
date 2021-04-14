using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class GetFederatedDirectoryUserResponse
	{
		public FederatedDirectoryGroupType[] Groups { get; set; }

		public string PhotoUrl { get; set; }
	}
}
