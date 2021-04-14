using System;
using System.Globalization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Online.BOX.Shell;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class BposShellInfoAssetReader : BposAssetReader<BposShellInfo>
	{
		internal BposShellInfoAssetReader(string userPrincipalName, CultureInfo culture, BposHeaderFlight currentHeaderFlight, UserContext userContext) : base(userPrincipalName, culture)
		{
			this.currentHeaderFlight = currentHeaderFlight;
			this.userContext = userContext;
			this.isGemini = (this.currentHeaderFlight == BposHeaderFlight.E16Gemini1 || this.currentHeaderFlight == BposHeaderFlight.E16Gemini2);
		}

		protected override BposShellInfo ExecuteRequest(ShellServiceClient client, string cultureName, string userPrincipalName, string userPuid, AuthZClientInfo effectiveCaller, string trackingGuid)
		{
			ShellBaseFlight value = ShellBaseFlight.V15Parity;
			if (this.currentHeaderFlight == BposHeaderFlight.E16Gemini1)
			{
				value = ShellBaseFlight.V16;
			}
			else if (this.currentHeaderFlight == BposHeaderFlight.E16Gemini2)
			{
				value = ShellBaseFlight.V16G2;
			}
			ShellInfoRequest shellInfoRequest = new ShellInfoRequest
			{
				BrandId = null,
				CultureName = cultureName,
				CurrentMainLinkID = NavBarMainLinkID.Outlook,
				UserPrincipalName = userPrincipalName,
				UserPuid = userPuid,
				WorkloadId = WorkloadAuthenticationId.Exchange,
				TrackingGuid = trackingGuid,
				ShellBaseFlight = new ShellBaseFlight?(value),
				UserThemeId = (this.isGemini ? this.userContext.Theme.FolderName : null)
			};
			shellInfoRequest.UserThemeId = null;
			ShellInfo shellInfo = client.GetShellInfo(shellInfoRequest);
			return this.CreateBposShellInfo(shellInfo, effectiveCaller);
		}

		private BposShellInfo CreateBposShellInfo(ShellInfo info, AuthZClientInfo effectiveCaller)
		{
			if (info == null)
			{
				return null;
			}
			try
			{
				if (base.ShouldUpdateCache(info.Version))
				{
					NavBarData data = base.CreateNavBarData(info.NavBarDataJson);
					base.UpdateCachedAssets(new BposShellInfo(info.Version, data, info.SuiteServiceProxyOriginAllowedList, info.SuiteServiceProxyScriptUrl));
				}
			}
			catch (Exception)
			{
			}
			NavBarData navBarData = base.CreateNavBarData(info.NavBarDataJson);
			base.UpdateAppsLinks(navBarData, effectiveCaller);
			return new BposShellInfo(info.Version, navBarData, info.SuiteServiceProxyOriginAllowedList, info.SuiteServiceProxyScriptUrl);
		}

		private readonly BposHeaderFlight currentHeaderFlight;

		private readonly bool isGemini;

		private readonly UserContext userContext;
	}
}
