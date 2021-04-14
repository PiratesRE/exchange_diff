using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Security
{
	internal class LiveIdPropertySet
	{
		private string RpsToken { get; set; }

		private LiveIdPropertySet(HttpContext httpContext)
		{
			string text = httpContext.Request.Headers["msExchOrganizationContext"];
			if (!string.IsNullOrEmpty(text))
			{
				this.RequestType = RequestType.EcpByoidAdmin;
				this.TargetTenant = text;
				return;
			}
			string text2 = httpContext.Request.Headers["msExchTargetTenant"];
			if (string.IsNullOrEmpty(text2))
			{
				this.RequestType = RequestType.Regular;
				return;
			}
			this.TargetTenant = text2;
			if (httpContext.Items["msExchNoResolveId"] != null)
			{
				this.RequestType = RequestType.EcpDelegatedAdminTargetForest;
				return;
			}
			this.RequestType = RequestType.EcpDelegatedAdmin;
		}

		internal static LiveIdPropertySet Current
		{
			get
			{
				HttpContext httpContext = HttpContext.Current;
				LiveIdPropertySet liveIdPropertySet = (LiveIdPropertySet)httpContext.Items["LiveIdPropertySet"];
				if (liveIdPropertySet == null)
				{
					liveIdPropertySet = new LiveIdPropertySet(httpContext);
					httpContext.Items["LiveIdPropertySet"] = liveIdPropertySet;
				}
				return liveIdPropertySet;
			}
		}

		internal static LiveIdPropertySet GetLiveIdPropertySet(HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			LiveIdPropertySet liveIdPropertySet = (LiveIdPropertySet)httpContext.Items["LiveIdPropertySet"];
			if (liveIdPropertySet == null)
			{
				liveIdPropertySet = new LiveIdPropertySet(httpContext);
				httpContext.Items["LiveIdPropertySet"] = liveIdPropertySet;
			}
			return liveIdPropertySet;
		}

		internal uint RpsTicketType { get; set; }

		internal RequestType RequestType { get; private set; }

		internal string TargetTenant { get; private set; }

		internal string PUID { get; private set; }

		internal string OrgIdPUID { get; private set; }

		internal string CID { get; private set; }

		internal string MemberName { get; private set; }

		internal bool HasAcceptedAccruals { get; private set; }

		internal string RpsRespHeaders { get; private set; }

		internal string SiteName { get; private set; }

		internal uint LoginAttributes { get; private set; }

		internal uint IssueInstant { get; private set; }

		internal IIdentity Identity { get; set; }

		internal OrganizationProperties OrganizationProperties { get; set; }

		internal void SetLiveIdProperties(string puid, string orgIdPuid, string cid, string memberName, bool hasAcceptedAccruals, uint loginAttributes, uint rpsTicketType, string respHeaders, string siteName, uint issueInstant)
		{
			this.PUID = puid;
			this.OrgIdPUID = orgIdPuid;
			this.CID = cid;
			this.MemberName = memberName;
			this.HasAcceptedAccruals = hasAcceptedAccruals;
			this.LoginAttributes = loginAttributes;
			this.RpsTicketType = rpsTicketType;
			this.RpsRespHeaders = respHeaders;
			this.SiteName = siteName;
			this.IssueInstant = issueInstant;
		}

		private const string Key = "LiveIdPropertySet";
	}
}
