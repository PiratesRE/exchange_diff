using System;
using System.Security;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Win32;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal static class Globals
	{
		internal static object ReadRegKey(string parameterRegistryKeyPath, string nameOfRegKey)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(parameterRegistryKeyPath, false))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue(nameOfRegKey);
						if (value != null)
						{
							return value;
						}
					}
				}
			}
			catch (SecurityException)
			{
			}
			catch (ObjectDisposedException)
			{
			}
			return null;
		}

		public const string ElcAssistantName = "ElcAssistant";

		internal const string ElcConfigurationXSOClass = "ELC";

		internal const string ElcTagConfigurationXSOClass = "MRM";

		internal const string MRMFolderVerifyerClass = "MRMFolder";

		internal const string AutoTagSettingConfigurationXSOClass = "MRM.AutoTag.Setting";

		internal const string AutoTagModelConfigurationXSOClass = "MRM.AutoTag.Model";

		internal const string ElcConfigurationStoreClass = "IPM.Configuration.ELC";

		internal const string MrmConfigurationStoreClass = "IPM.Configuration.MRM";

		internal const string AutoTagModelConfigurationStoreClass = "IPM.Configuration.MRM.AutoTag.Model";

		internal const string AutoTagSettingConfigurationStoreClass = "IPM.Configuration.MRM.AutoTag.Setting";

		internal const UserConfigurationTypes ElcConfigurationTypes = UserConfigurationTypes.Stream | UserConfigurationTypes.XML | UserConfigurationTypes.Dictionary;

		internal const int QueryResultBatchSize = 100;

		internal static readonly string UnlimitedHoldDuration = "Unlimited";

		internal static readonly int HRESULT_FROM_WIN32_DS_OPERATIONS_ERROR = -2147016672;

		internal static readonly string BlankComment = " ";

		internal static readonly string ElcRootFolderClass = "IPF.Note.OutlookHomepage";
	}
}
