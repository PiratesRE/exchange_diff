using System;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration
{
	public interface IClientAccessRulesAuthorizer
	{
		ClientAccessProtocol Protocol { get; }

		void SafeAppendGenericInfo(HttpContext httpContext, string key, string value);

		void ResponseToError(HttpContext httpContext);

		OrganizationId GetUserOrganization();
	}
}
