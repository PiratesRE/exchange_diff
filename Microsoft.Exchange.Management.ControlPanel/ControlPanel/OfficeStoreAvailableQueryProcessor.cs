using System;
using System.Configuration;
using System.Web;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class OfficeStoreAvailableQueryProcessor : RbacQuery.RbacQueryProcessor
	{
		public static bool IsOfficeStoreAvailable
		{
			get
			{
				string text = ConfigurationManager.AppSettings["OfficeStoreUnavailable"];
				string a = HttpContext.Current.Request.QueryString["appStore"];
				return !string.Equals(a, "OSX", StringComparison.OrdinalIgnoreCase) && (string.IsNullOrWhiteSpace(text) || StringComparer.OrdinalIgnoreCase.Equals("false", text));
			}
		}

		public override bool? TryIsInRole(ExchangeRunspaceConfiguration rbacConfiguration)
		{
			return new bool?(OfficeStoreAvailableQueryProcessor.IsOfficeStoreAvailable);
		}

		internal const string RoleName = "OfficeStoreAvailable";
	}
}
