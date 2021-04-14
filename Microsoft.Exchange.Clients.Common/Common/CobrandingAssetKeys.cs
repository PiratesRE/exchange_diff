using System;

namespace Microsoft.Exchange.Clients.Common
{
	internal static class CobrandingAssetKeys
	{
		public static string GetAssetKeyString(CobrandingAssetKey assetKey)
		{
			return CobrandingAssetKeys.CobrandingAssetKeyMap[(int)assetKey];
		}

		private static readonly string[] CobrandingAssetKeyMap = new string[]
		{
			"Exchange.Identity.OrganizationName",
			"Exchange.Identity.LogoR3.Path",
			"Exchange.Identity.LogoR3.AltText",
			"Exchange.Navigation.Signout.URL",
			"Exchange.Theme.EnableCustomTheme",
			"Exchange.Theme.Application.HoverColor",
			"Exchange.Theme.Content.SignOutColor",
			"Exchange.Theme.BrandBarR3.TextColor",
			"Exchange.Theme.Content.PrimaryLinkColor",
			"Exchange.Theme.Application.SelectedBorderColor",
			"Exchange.Theme.ActiveText.Hex",
			"Exchange.Theme.BrandBarR3.Path",
			"Exchange.Theme.BrandBarBackgroundImageR3.Path"
		};
	}
}
