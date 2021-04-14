using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Contract
{
	internal static class DataConstants
	{
		public const string Namespace = "http://schemas.microsoft.com/informationprotection/computefabric";

		public const string ComplianceEndpointNetTcpAddressFormat = "net.tcp://{0}/complianceservice";

		public const string ComplianceEndpointHttpsAddressFormat = "https://{0}/complianceservice";

		public const int MaxReceivedMessageSize = 524288;

		public const int MaxBufferPoolSize = 1048576;
	}
}
