using System;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class SendAddressAvailableQueryProcessor : EcpCmdletQueryProcessor
	{
		internal override bool? IsInRoleCmdlet(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			SendAddress sendAddress = new SendAddress();
			PowerShellResults<SendAddressRow> list = sendAddress.GetList(null, null);
			if (list.Succeeded)
			{
				return new bool?(list.Output.Length > 2);
			}
			base.LogCmdletError(list, "SendAddressAvailable");
			return new bool?(false);
		}

		internal const string RoleName = "SendAddressAvailable";
	}
}
