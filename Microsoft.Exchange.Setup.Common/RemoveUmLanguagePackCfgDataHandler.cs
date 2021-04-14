using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Win32;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RemoveUmLanguagePackCfgDataHandler : ConfigurationDataHandler
	{
		public RemoveUmLanguagePackCfgDataHandler(ISetupContext context, MonadConnection connection, CultureInfo culture) : base(context, "", "remove-umlanguagepack", connection)
		{
			this.Culture = culture;
			string umLanguagePackNameForCultureInfo = UmLanguagePackConfigurationInfo.GetUmLanguagePackNameForCultureInfo(this.Culture);
			InstallableUnitConfigurationInfo installableUnitConfigurationInfo = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(umLanguagePackNameForCultureInfo);
			if (installableUnitConfigurationInfo == null)
			{
				installableUnitConfigurationInfo = new UmLanguagePackConfigurationInfo(this.Culture);
				InstallableUnitConfigurationInfoManager.AddInstallableUnit(umLanguagePackNameForCultureInfo, installableUnitConfigurationInfo);
			}
			base.InstallableUnitName = installableUnitConfigurationInfo.Name;
			this.productCode = this.GetProductGuidForCulture("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\LanguagePacks\\", this.Culture, false);
			this.teleProductCode = this.GetProductGuidForCulture("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\TeleLanguagePacks\\", this.Culture, false);
			this.transProductCode = this.GetProductGuidForCulture("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\TransLanguagePacks\\", this.Culture, true);
			this.ttsProductCode = this.GetProductGuidForCulture("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole\\TtsLanguagePacks\\", this.Culture, false);
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			SetupLogger.Log(Strings.RemoveUmLanguagePackLogFilePath(this.LogFilePath));
			base.Parameters.AddWithValue("logfilepath", this.LogFilePath);
			base.Parameters.AddWithValue("propertyvalues", "ESE=1");
			base.Parameters.AddWithValue("ProductCode", this.ProductCode);
			base.Parameters.AddWithValue("TeleProductCode", this.TeleProductCode);
			base.Parameters.AddWithValue("TransProductCode", this.TransProductCode);
			base.Parameters.AddWithValue("TtsProductCode", this.TtsProductCode);
			base.Parameters.AddWithValue("Language", this.Culture);
			SetupLogger.TraceExit();
		}

		public Guid GetProductGuidForCulture(string keyPath, CultureInfo culture, bool onlyIfInstalled)
		{
			Guid result = Guid.Empty;
			string text = (string)Registry.GetValue(keyPath, culture.ToString(), null);
			if (!string.IsNullOrEmpty(text))
			{
				Guid guid = new Guid(text);
				if (!onlyIfInstalled || MsiUtility.IsInstalled(guid))
				{
					result = guid;
				}
			}
			return result;
		}

		public override ValidationError[] ValidateConfiguration()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>(base.ValidateConfiguration());
			if (this.ProductCode == Guid.Empty)
			{
				list.Add(new SetupValidationError(Strings.UmLanguagePackNotFoundForCulture(this.Culture.ToString())));
			}
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
		}

		public bool WatsonEnabled
		{
			get
			{
				return this.watsonEnabled;
			}
			set
			{
				this.watsonEnabled = value;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
			private set
			{
				this.culture = value;
			}
		}

		public string LogFilePath
		{
			get
			{
				string path = "remove-" + RemoveUmLanguagePackCfgDataHandler.msiFilePrefix + this.Culture.ToString() + RemoveUmLanguagePackCfgDataHandler.logExtension;
				return Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, path);
			}
		}

		public Guid ProductCode
		{
			get
			{
				return this.productCode;
			}
			set
			{
				this.productCode = value;
			}
		}

		public Guid TeleProductCode
		{
			get
			{
				return this.teleProductCode;
			}
			set
			{
				this.teleProductCode = value;
			}
		}

		public Guid TransProductCode
		{
			get
			{
				return this.transProductCode;
			}
			set
			{
				this.transProductCode = value;
			}
		}

		public Guid TtsProductCode
		{
			get
			{
				return this.ttsProductCode;
			}
			set
			{
				this.ttsProductCode = value;
			}
		}

		private bool watsonEnabled;

		private CultureInfo culture;

		private static string msiFilePrefix = "UMLanguagePack.";

		private static string logExtension = ".msilog";

		private Guid productCode;

		private Guid teleProductCode;

		private Guid transProductCode;

		private Guid ttsProductCode;
	}
}
