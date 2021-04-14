using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public enum ThemeFileId
	{
		[ThemeFileInfo]
		None,
		[ThemeFileInfo("owafont.css", ThemeFileInfoFlags.Resource)]
		OwaFontCss,
		[ThemeFileInfo("logon.css", ThemeFileInfoFlags.Resource)]
		LogonCss,
		[ThemeFileInfo("errorFE.css", ThemeFileInfoFlags.Resource)]
		ErrorFECss,
		[ThemeFileInfo("icon_settings.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		OwaSettings,
		[ThemeFileInfo("olk_logo_white.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		OutlookLogoWhite,
		[ThemeFileInfo("olk_logo_white_cropped.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		OutlookLogoWhiteCropped,
		[ThemeFileInfo("olk_logo_white_small.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		OutlookLogoWhiteSmall,
		[ThemeFileInfo("owa_text_blue.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		OwaHeaderTextBlue,
		[ThemeFileInfo("bg_gradient.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		BackgroundGradient,
		[ThemeFileInfo("bg_gradient_login.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		BackgroundGradientLogin,
		[ThemeFileInfo("Sign_in_arrow.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		SignInArrow,
		[ThemeFileInfo("Sign_in_arrow_rtl.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		SignInArrowRtl,
		[ThemeFileInfo("warn.png")]
		Error,
		[ThemeFileInfo("lgntopl.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonTopLeft,
		[ThemeFileInfo("lgntopm.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonTopMiddle,
		[ThemeFileInfo("lgntopr.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonTopRight,
		[ThemeFileInfo("lgnbotl.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonBottomLeft,
		[ThemeFileInfo("lgnbotm.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonBottomMiddle,
		[ThemeFileInfo("lgnbotr.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonBottomRight,
		[ThemeFileInfo("lgnexlogo.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonExchangeLogo,
		[ThemeFileInfo("lgnleft.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonLeft,
		[ThemeFileInfo("lgnright.gif", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		LogonRight,
		[ThemeFileInfo("favicon.ico", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		FavoriteIcon,
		[ThemeFileInfo("favicon_office.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		FaviconOffice,
		[ThemeFileInfo("icp.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		ICPNum,
		[ThemeFileInfo("office365_cn.png", ThemeFileInfoFlags.LooseImage | ThemeFileInfoFlags.Resource)]
		Office365CnLogo,
		[ThemeFileInfo]
		Count
	}
}
