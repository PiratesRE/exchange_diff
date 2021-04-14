using System;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetHybridMailflowDatacenterIPsCommand : SyntheticCommand<object>
	{
		private GetHybridMailflowDatacenterIPsCommand() : base("Get-HybridMailflowDatacenterIPs")
		{
		}

		public GetHybridMailflowDatacenterIPsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}
	}
}
