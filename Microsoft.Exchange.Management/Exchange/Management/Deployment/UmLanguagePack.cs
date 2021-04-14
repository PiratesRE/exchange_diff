using System;
using System.Security;
using Microsoft.Exchange.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	public class UmLanguagePack
	{
		public string Name { get; private set; }

		public Guid ProductCode
		{
			get
			{
				if (UmLanguagePackUtils.CurrentTargetPlatform == TargetPlatform.X64)
				{
					return this.X64ProductCode;
				}
				return this.X86ProductCode;
			}
		}

		public Guid X86ProductCode { get; private set; }

		public Guid X64ProductCode { get; private set; }

		public Guid TeleProductCode { get; private set; }

		public Guid TransProductCode { get; private set; }

		public Guid TtsProductCode { get; private set; }

		public UmLanguagePack(string name, Guid x86ProductCode, Guid x64ProductCode, Guid teleProductCode, Guid transProductCode, Guid ttsProductCode)
		{
			this.Name = name;
			this.X86ProductCode = x86ProductCode;
			this.X64ProductCode = x64ProductCode;
			this.TeleProductCode = teleProductCode;
			this.TransProductCode = transProductCode;
			this.TtsProductCode = ttsProductCode;
		}

		private static void PerformRegistryOperation(string languagePackType, string culture, GrayException.UserCodeDelegate function)
		{
			try
			{
				function();
			}
			catch (SecurityException innerException)
			{
				string regKeyPath = UmLanguagePack.GetRegKeyPath(true, languagePackType);
				throw new RegistryInsufficientPermissionException(regKeyPath, culture, innerException);
			}
		}

		public void AddProductCodesToRegistry()
		{
			UmLanguagePack.PerformRegistryOperation("LanguagePacks", this.Name, delegate
			{
				this.AddRegKeyValue("LanguagePacks", this.Name, this.ProductCode);
			});
			UmLanguagePack.PerformRegistryOperation("TeleLanguagePacks", this.Name, delegate
			{
				this.AddRegKeyValue("TeleLanguagePacks", this.Name, this.TeleProductCode);
			});
			UmLanguagePack.PerformRegistryOperation("TransLanguagePacks", this.Name, delegate
			{
				this.AddRegKeyValue("TransLanguagePacks", this.Name, this.TransProductCode);
			});
			UmLanguagePack.PerformRegistryOperation("TtsLanguagePacks", this.Name, delegate
			{
				this.AddRegKeyValue("TtsLanguagePacks", this.Name, this.TtsProductCode);
			});
		}

		public void RemoveProductCodesFromRegistry()
		{
			UmLanguagePack.PerformRegistryOperation("LanguagePacks", this.Name, delegate
			{
				this.DeleteRegKeyAndValue("LanguagePacks", this.Name);
			});
			UmLanguagePack.PerformRegistryOperation("TeleLanguagePacks", this.Name, delegate
			{
				this.DeleteRegKeyAndValue("TeleLanguagePacks", this.Name);
			});
			UmLanguagePack.PerformRegistryOperation("TransLanguagePacks", this.Name, delegate
			{
				this.DeleteRegKeyAndValue("TransLanguagePacks", this.Name);
			});
			UmLanguagePack.PerformRegistryOperation("TtsLanguagePacks", this.Name, delegate
			{
				this.DeleteRegKeyAndValue("TtsLanguagePacks", this.Name);
			});
		}

		private void AddRegKeyValue(string languagePackType, string culture, Guid productCode)
		{
			string regKeyPath = UmLanguagePack.GetRegKeyPath(true, languagePackType);
			Registry.SetValue(regKeyPath, culture, productCode.ToString(), RegistryValueKind.String);
		}

		private void DeleteRegKeyAndValue(string key, string value)
		{
			string regKeyPath = UmLanguagePack.GetRegKeyPath(false, key);
			bool flag;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(regKeyPath, true))
			{
				registryKey.DeleteValue(value, false);
				string[] valueNames = registryKey.GetValueNames();
				flag = (valueNames.Length > 0);
			}
			if (!flag)
			{
				using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole", true))
				{
					registryKey2.DeleteSubKey(key);
				}
			}
		}

		private static string GetRegKeyPath(bool includeHKLM, string languagePackType)
		{
			if (!includeHKLM)
			{
				return string.Format("{0}\\{1}\\", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole", languagePackType);
			}
			return string.Format("HKEY_LOCAL_MACHINE\\{0}\\{1}\\", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole", languagePackType);
		}

		private const string LanguagePacks = "LanguagePacks";

		private const string TeleLanguagePacks = "TeleLanguagePacks";

		private const string TransLanguagePacks = "TransLanguagePacks";

		private const string TtsLanguagePacks = "TtsLanguagePacks";
	}
}
