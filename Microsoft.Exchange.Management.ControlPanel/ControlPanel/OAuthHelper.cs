using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class OAuthHelper
	{
		static OAuthHelper()
		{
			OAuthHelper.parterIdToNameMap.Add(WellknownPartnerApplicationIdentifiers.CRM, "OAuthACS.CRM");
			OAuthHelper.parterIdToNameMap.Add(WellknownPartnerApplicationIdentifiers.Exchange, "OAuthACS.Exchange");
			OAuthHelper.parterIdToNameMap.Add(WellknownPartnerApplicationIdentifiers.Intune, "OAuthACS.Intune");
			OAuthHelper.parterIdToNameMap.Add(WellknownPartnerApplicationIdentifiers.Lync, "OAuthACS.Lync");
			OAuthHelper.parterIdToNameMap.Add(WellknownPartnerApplicationIdentifiers.Office365Portal, "OAuthACS.Office365Portal");
			OAuthHelper.parterIdToNameMap.Add(WellknownPartnerApplicationIdentifiers.OfficeServiceManager, "OAuthACS.OfficeServiceManager");
			OAuthHelper.parterIdToNameMap.Add(WellknownPartnerApplicationIdentifiers.SharePoint, "OAuthACS.SharePoint");
		}

		public static KeyValuePair<string, string>? GetOAuthUserConstraint(IIdentity logonUser)
		{
			SidOAuthIdentity sidOAuthIdentity = (logonUser as SidOAuthIdentity) ?? (HttpContext.Current.Items["LogonUserIdentity"] as SidOAuthIdentity);
			OAuthIdentity oauthIdentity = (sidOAuthIdentity != null) ? sidOAuthIdentity.OAuthIdentity : (logonUser as OAuthIdentity);
			if (oauthIdentity != null)
			{
				string value = null;
				if (oauthIdentity.OAuthApplication == null || oauthIdentity.OAuthApplication.PartnerApplication == null || !OAuthHelper.parterIdToNameMap.TryGetValue(oauthIdentity.OAuthApplication.PartnerApplication.ApplicationIdentifier, out value))
				{
					value = "OAuthACS.UnknownPartner";
				}
				return new KeyValuePair<string, string>?(new KeyValuePair<string, string>("AuthMethod", value));
			}
			return null;
		}

		private static string GetUniqueStringFromService(string servicePath, string schema, string workflow)
		{
			StringBuilder stringBuilder = new StringBuilder(servicePath);
			bool flag = !string.IsNullOrEmpty(schema);
			if (flag)
			{
				stringBuilder.Append('?');
				stringBuilder.Append("schema");
				stringBuilder.Append('=');
				stringBuilder.Append(schema);
			}
			bool flag2 = !string.IsNullOrEmpty(workflow);
			if (flag2)
			{
				stringBuilder.Append(flag ? '&' : '?');
				stringBuilder.Append("workflow");
				stringBuilder.Append('=');
				stringBuilder.Append(workflow);
			}
			return stringBuilder.ToString();
		}

		public static bool IsWebRequestAllowed(HttpContext context)
		{
			if (!context.IsAcsOAuthRequest())
			{
				return true;
			}
			VariantConfigurationSnapshot snapshotForCurrentUser = EacFlightUtility.GetSnapshotForCurrentUser();
			return OAuthHelper.IsWebRequestAllowed(snapshotForCurrentUser, snapshotForCurrentUser.Eac.GetObjectsOfType<IEacWebRequest>(), context.Request);
		}

		internal static bool IsWebRequestAllowed(VariantConfigurationSnapshot snapshot, IDictionary<string, IEacWebRequest> configs, HttpRequest request)
		{
			string relativePathToAppRoot = EcpUrl.GetRelativePathToAppRoot(request.FilePath);
			string text = request.QueryString["workflow"];
			string schema = request.QueryString["schema"];
			if (text == null && request.PathInfo != null && request.PathInfo.Length > 1 && request.PathInfo[0] == '/')
			{
				text = request.PathInfo.Substring(1);
			}
			string uniqueStringFromService = OAuthHelper.GetUniqueStringFromService(relativePathToAppRoot, schema, text);
			foreach (KeyValuePair<string, IEacWebRequest> keyValuePair in configs)
			{
				if (string.Compare(uniqueStringFromService, keyValuePair.Value.Request, true) == 0)
				{
					return keyValuePair.Value.Enabled;
				}
			}
			return snapshot.Eac.UnlistedServices.Enabled;
		}

		private const string AuthMethodConstraintKey = "AuthMethod";

		private const string OAuthWithACS = "OAuthACS.";

		private const string UnknownPartner = "OAuthACS.UnknownPartner";

		private static readonly IDictionary<string, string> parterIdToNameMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	}
}
