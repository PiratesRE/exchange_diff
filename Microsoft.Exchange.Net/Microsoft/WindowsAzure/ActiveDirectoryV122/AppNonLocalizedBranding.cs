using System;
using System.CodeDom.Compiler;
using System.Data.Services.Common;

namespace Microsoft.WindowsAzure.ActiveDirectoryV122
{
	[DataServiceKey("locale")]
	public class AppNonLocalizedBranding
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public static AppNonLocalizedBranding CreateAppNonLocalizedBranding(string locale)
		{
			return new AppNonLocalizedBranding
			{
				locale = locale
			};
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string heroBackgroundColor
		{
			get
			{
				return this._heroBackgroundColor;
			}
			set
			{
				this._heroBackgroundColor = value;
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
		public string preloadUrl
		{
			get
			{
				return this._preloadUrl;
			}
			set
			{
				this._preloadUrl = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _heroBackgroundColor;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _locale;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _preloadUrl;
	}
}
