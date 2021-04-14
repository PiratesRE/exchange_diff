using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientPermission
{
	internal static class RecipientPermissionTaskHelper
	{
		internal static string GetFriendlyNameOfSecurityIdentifier(SecurityIdentifier sid, IRecipientSession session, Task.TaskErrorLoggingDelegate errorLogger, Task.TaskVerboseLoggingDelegate verboseLogger)
		{
			if (!RecipientPermissionTaskHelper.sidToName.ContainsKey(sid))
			{
				ADRecipient adrecipient = (ADRecipient)SecurityPrincipalIdParameter.GetSecurityPrincipal(session, new SecurityPrincipalIdParameter(sid), errorLogger, verboseLogger);
				if (adrecipient != null)
				{
					if (adrecipient.Id != null)
					{
						RecipientPermissionTaskHelper.sidToName[sid] = adrecipient.Id.ToString();
					}
					else
					{
						RecipientPermissionTaskHelper.sidToName[sid] = SecurityPrincipalIdParameter.GetFriendlyUserName(sid, verboseLogger);
					}
				}
			}
			return RecipientPermissionTaskHelper.sidToName[sid];
		}

		private static Dictionary<SecurityIdentifier, string> sidToName = new Dictionary<SecurityIdentifier, string>();
	}
}
