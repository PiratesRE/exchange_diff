using System;
using System.CodeDom.Compiler;
using System.Data.Services.Client;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectory
{
	[DataServiceKey("locale")]
	public class LoginTenantBranding
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static LoginTenantBranding CreateLoginTenantBranding(DataServiceStreamLink bannerLogo, DataServiceStreamLink illustration, string locale, DataServiceStreamLink tileLogo)
		{
			return new LoginTenantBranding
			{
				bannerLogo = bannerLogo,
				illustration = illustration,
				locale = locale,
				tileLogo = tileLogo
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string backgroundColor
		{
			get
			{
				return this._backgroundColor;
			}
			set
			{
				this._backgroundColor = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DataServiceStreamLink bannerLogo
		{
			get
			{
				return this._bannerLogo;
			}
			set
			{
				this._bannerLogo = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string bannerLogoUrl
		{
			get
			{
				return this._bannerLogoUrl;
			}
			set
			{
				this._bannerLogoUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string boilerPlateText
		{
			get
			{
				return this._boilerPlateText;
			}
			set
			{
				this._boilerPlateText = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DataServiceStreamLink illustration
		{
			get
			{
				return this._illustration;
			}
			set
			{
				this._illustration = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string illustrationUrl
		{
			get
			{
				return this._illustrationUrl;
			}
			set
			{
				this._illustrationUrl = value;
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
		public string metadataUrl
		{
			get
			{
				return this._metadataUrl;
			}
			set
			{
				this._metadataUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public DataServiceStreamLink tileLogo
		{
			get
			{
				return this._tileLogo;
			}
			set
			{
				this._tileLogo = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string tileLogoUrl
		{
			get
			{
				return this._tileLogoUrl;
			}
			set
			{
				this._tileLogoUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string userIdLabel
		{
			get
			{
				return this._userIdLabel;
			}
			set
			{
				this._userIdLabel = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _backgroundColor;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DataServiceStreamLink _bannerLogo;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _bannerLogoUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _boilerPlateText;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DataServiceStreamLink _illustration;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _illustrationUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _locale;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _metadataUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private DataServiceStreamLink _tileLogo;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _tileLogoUrl;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _userIdLabel;
	}
}
