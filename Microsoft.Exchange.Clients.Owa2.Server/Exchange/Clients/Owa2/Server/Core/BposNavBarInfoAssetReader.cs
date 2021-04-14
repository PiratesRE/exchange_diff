using System;
using System.Globalization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Online.BOX.Shell;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class BposNavBarInfoAssetReader : BposAssetReader<BposNavBarInfo>
	{
		internal BposNavBarInfoAssetReader(string userPrincipalName, CultureInfo culture) : base(userPrincipalName, culture)
		{
		}

		protected override BposNavBarInfo ExecuteRequest(ShellServiceClient client, string cultureName, string userPrincipalName, string userPuid, AuthZClientInfo effectiveCaller, string trackingGuid)
		{
			NavBarInfoRequest navBarInfoRequest = new NavBarInfoRequest
			{
				BrandId = null,
				CultureName = cultureName,
				CurrentMainLinkID = NavBarMainLinkID.Outlook,
				UserPrincipalName = userPrincipalName,
				UserPuid = userPuid,
				WorkloadId = WorkloadAuthenticationId.Exchange,
				TrackingGuid = trackingGuid
			};
			NavBarInfo navBarInfo = client.GetNavBarInfo(navBarInfoRequest);
			return this.CreateBposNavBarInfo(navBarInfo, effectiveCaller);
		}

		private BposNavBarInfo CreateBposNavBarInfo(NavBarInfo info, AuthZClientInfo effectiveCaller)
		{
			if (info == null)
			{
				return null;
			}
			try
			{
				if (base.ShouldUpdateCache(info.Version))
				{
					NavBarData navBarData = base.CreateNavBarData(info.NavBarDataJson);
					base.UpdateCachedAssets(new BposNavBarInfo(info.Version, navBarData));
				}
			}
			catch (Exception)
			{
			}
			NavBarData navBarData2 = base.CreateNavBarData(info.NavBarDataJson);
			base.UpdateAppsLinks(navBarData2, effectiveCaller);
			return new BposNavBarInfo(info.Version, navBarData2);
		}
	}
}
