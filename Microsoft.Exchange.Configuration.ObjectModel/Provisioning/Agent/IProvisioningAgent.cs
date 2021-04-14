using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Provisioning.Agent
{
	public interface IProvisioningAgent
	{
		IEnumerable<string> GetSupportedCmdlets();

		ProvisioningHandler GetCmdletHandler(string cmdletName);
	}
}
