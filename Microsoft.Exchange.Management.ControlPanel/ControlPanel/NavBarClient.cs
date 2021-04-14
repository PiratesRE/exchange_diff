using System;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.Online.BOX.Shell;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NavBarClient : NavBarClientBase
	{
		public NavBarClient(bool showAdminFeature, bool fallbackMode, bool forceReload) : base(showAdminFeature, fallbackMode, forceReload)
		{
		}

		protected override NavBarInfoRequest CreateRequest()
		{
			return new NavBarInfoRequest
			{
				UserPuid = this.userPuid,
				UserPrincipalName = this.userPrincipalName,
				CultureName = this.cultureName,
				WorkloadId = (RbacPrincipal.Current.IsInRole("FFO") ? 7 : 1),
				CurrentMainLinkID = 5,
				TrackingGuid = Guid.NewGuid().ToString()
			};
		}

		protected override NavBarPack TryGetNavBarPackFromCache()
		{
			NavBarPack navBarPack = NavBarCache.Get(this.userPuid, this.userPrincipalName, this.cultureName);
			if (navBarPack != null)
			{
				navBarPack.IsFresh = false;
			}
			return navBarPack;
		}

		protected override void CallShellService(ShellServiceClient client, NavBarInfoRequest request)
		{
			this.navbarInfo = client.GetNavBarInfo(request);
		}

		protected override NavBarPack GetMockNavBarPack()
		{
			return MockNavBar.GetMockNavBarPack(this.userPuid, this.userPrincipalName, this.cultureName, RtlUtil.IsRtl, this.isGallatin, false);
		}

		protected override NavBarPack EndGetNavBarPack()
		{
			if (!base.LogException() && this.navbarInfo != null)
			{
				NavBarPack navBarPackFromInfo = NavBarClientBase.GetNavBarPackFromInfo(this.navbarInfo, this.cultureName);
				ExTraceGlobals.WebServiceTracer.TraceInformation(0, 0L, string.Format("NavBarData acquired. SessionId: {0}, CorrelationId: {1}", navBarPackFromInfo.NavBarData.SessionID, navBarPackFromInfo.NavBarData.CorrelationID));
				navBarPackFromInfo.IsGeminiShellEnabled = false;
				navBarPackFromInfo.IsFresh = true;
				NavBarCache.Set(this.userPuid, this.userPrincipalName, navBarPackFromInfo);
				return navBarPackFromInfo;
			}
			return null;
		}

		private NavBarInfo navbarInfo;
	}
}
