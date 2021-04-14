using System;
using System.CodeDom.Compiler;
using System.Data.Services.Client;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV122
{
	[DataServiceKey("locale")]
	public class AppLocalizedBranding
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static AppLocalizedBranding CreateAppLocalizedBranding(DataServiceStreamLink appBannerLogo, DataServiceStreamLink heroIllustration, string locale)
		{
			return new AppLocalizedBranding
			{
				appBannerLogo = appBannerLogo,
				heroIllustration = heroIllustration,
				locale = locale
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DataServiceStreamLink appBannerLogo
		{
			get
			{
				return this._appBannerLogo;
			}
			set
			{
				this._appBannerLogo = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string appBannerLogoUrl
		{
			get
			{
				return this._appBannerLogoUrl;
			}
			set
			{
				this._appBannerLogoUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string displayName
		{
			get
			{
				return this._displayName;
			}
			set
			{
				this._displayName = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DataServiceStreamLink heroIllustration
		{
			get
			{
				return this._heroIllustration;
			}
			set
			{
				this._heroIllustration = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string heroIllustrationUrl
		{
			get
			{
				return this._heroIllustrationUrl;
			}
			set
			{
				this._heroIllustrationUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string locale
		{
			get
			{
				return this._locale;
			}
			set
			{
				this._locale = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DataServiceStreamLink _appBannerLogo;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _appBannerLogoUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _displayName;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DataServiceStreamLink _heroIllustration;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _heroIllustrationUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _locale;
	}
}
