using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class RpcUtility
	{
		internal static LocalizedString MapRpcErrorCodeToMessage(int errorCode, string serverName)
		{
			int num = errorCode;
			if (num == 1753)
			{
				return Strings.ElcExpirationServiceUnavailable(serverName, errorCode.ToString());
			}
			return Strings.ElcServiceCallFailed(serverName, errorCode.ToString());
		}
	}
}
