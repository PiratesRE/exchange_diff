using System;
using System.Web;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration
{
	public class RemotePowershellClientAccessRulesAuthorizer : IClientAccessRulesAuthorizer
	{
		public void SafeAppendGenericInfo(HttpContext httpContext, string key, string value)
		{
			HttpLogger.SafeAppendGenericInfo(key, value);
		}

		public void ResponseToError(HttpContext httpContext)
		{
			httpContext.Response.StatusCode = 400;
			httpContext.Response.Write(string.Format("[FailureCategory={0}] ", FailureCategory.ClientAccessRules) + Environment.NewLine + Strings.ErrorRemotePowershellConnectionBlocked + Environment.NewLine);
		}

		public OrganizationId GetUserOrganization()
		{
			UserToken userToken = HttpContext.Current.CurrentUserToken();
			if (userToken != null)
			{
				return userToken.Organization;
			}
			return null;
		}

		public ClientAccessProtocol Protocol
		{
			get
			{
				return ClientAccessProtocol.RemotePowerShell;
			}
		}
	}
}
