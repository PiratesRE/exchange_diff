using System;
using System.Collections;
using System.DirectoryServices;
using System.IO;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ServicesServerTasks;
using Microsoft.Exchange.Management.Metabase;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class MobileSyncVirtualDirectoryHelper
	{
		public static void ReadFromMetabase(ADMobileVirtualDirectory vdirObject, Task task)
		{
			MetabasePropertyTypes.AccessSSLFlags accessSSLFlags = MetabasePropertyTypes.AccessSSLFlags.AccessSSL | MetabasePropertyTypes.AccessSSLFlags.AccessSSLNegotiateCert | MetabasePropertyTypes.AccessSSLFlags.AccessSSLRequireCert;
			string metabasePath = vdirObject.MetabasePath;
			if (string.IsNullOrEmpty(metabasePath))
			{
				return;
			}
			MetabaseProperty[] array = null;
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(metabasePath))
			{
				array = IisUtility.GetProperties(directoryEntry);
			}
			uint num = 0U;
			bool flag = false;
			foreach (MetabaseProperty metabaseProperty in array)
			{
				if (string.Equals(metabaseProperty.Name, "AuthFlags", StringComparison.OrdinalIgnoreCase))
				{
					object value = metabaseProperty.Value;
					if (value != null)
					{
						MetabasePropertyTypes.AuthFlags authFlags = (MetabasePropertyTypes.AuthFlags)((int)value);
						vdirObject.BasicAuthEnabled = ((authFlags & MetabasePropertyTypes.AuthFlags.Basic) == MetabasePropertyTypes.AuthFlags.Basic);
						vdirObject.WindowsAuthEnabled = ((authFlags & MetabasePropertyTypes.AuthFlags.Ntlm) == MetabasePropertyTypes.AuthFlags.Ntlm);
					}
					num += 1U;
				}
				else if (string.Equals(metabaseProperty.Name, "DoDynamicCompression", StringComparison.OrdinalIgnoreCase))
				{
					object value2 = metabaseProperty.Value;
					if (value2 != null)
					{
						vdirObject.CompressionEnabled = (bool)value2;
					}
					num += 1U;
				}
				else if (string.Equals(metabaseProperty.Name, "AccessSSLFlags", StringComparison.OrdinalIgnoreCase))
				{
					int? num2 = (int?)metabaseProperty.Value;
					if (num2 != null)
					{
						if ((num2.Value & (int)accessSSLFlags) == (int)accessSSLFlags)
						{
							vdirObject.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Required);
						}
						else if ((num2.Value & 32) == 32)
						{
							vdirObject.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Accepted);
						}
						else
						{
							vdirObject.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Ignore);
						}
						if ((num2.Value & 8) == 8)
						{
							vdirObject.WebSiteSSLEnabled = true;
						}
						else
						{
							vdirObject.WebSiteSSLEnabled = false;
						}
					}
					else
					{
						vdirObject.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Ignore);
						vdirObject.WebSiteSSLEnabled = false;
					}
					flag = true;
					num += 1U;
				}
				else if (string.Equals(metabaseProperty.Name, "AppFriendlyName", StringComparison.OrdinalIgnoreCase))
				{
					object value3 = metabaseProperty.Value;
					if (value3 != null)
					{
						vdirObject.VirtualDirectoryName = (string)value3;
					}
					num += 1U;
				}
				else if (num == 4U)
				{
					break;
				}
			}
			if (!flag)
			{
				int startIndex = metabasePath.LastIndexOf('/');
				string iisDirectoryEntryPath = metabasePath.Remove(startIndex);
				using (DirectoryEntry directoryEntry2 = IisUtility.CreateIISDirectoryEntry(iisDirectoryEntryPath))
				{
					int? num3 = (int?)directoryEntry2.Properties["AccessSSLFlags"].Value;
					if (num3 != null)
					{
						if ((num3.Value & (int)accessSSLFlags) == (int)accessSSLFlags)
						{
							vdirObject.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Required);
						}
						else if ((num3.Value & 32) == 32)
						{
							vdirObject.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Accepted);
						}
						else
						{
							vdirObject.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Ignore);
						}
						if ((num3.Value & 8) == 8)
						{
							vdirObject.WebSiteSSLEnabled = true;
						}
						else
						{
							vdirObject.WebSiteSSLEnabled = false;
						}
					}
					else
					{
						vdirObject.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Ignore);
						vdirObject.WebSiteSSLEnabled = false;
					}
				}
			}
			char[] separator = new char[]
			{
				'/'
			};
			string[] array3 = metabasePath.Split(separator);
			int num4 = array3.Length - 2;
			if (num4 <= 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(47);
			for (int j = 0; j < num4; j++)
			{
				stringBuilder.Append(array3[j]);
				if (j < num4 - 1)
				{
					stringBuilder.Append('/');
				}
			}
			using (DirectoryEntry directoryEntry3 = IisUtility.CreateIISDirectoryEntry(stringBuilder.ToString()))
			{
				array = IisUtility.GetProperties(directoryEntry3);
			}
			MetabaseProperty[] array4 = array;
			int k = 0;
			while (k < array4.Length)
			{
				MetabaseProperty metabaseProperty2 = array4[k];
				if (string.Compare(metabaseProperty2.Name, "ServerComment", true) == 0)
				{
					object value4 = metabaseProperty2.Value;
					if (value4 != null)
					{
						vdirObject.WebsiteName = (string)value4;
						break;
					}
					break;
				}
				else
				{
					k++;
				}
			}
			vdirObject.InitProxyVDirDataObject();
			ExtendedProtection.LoadFromMetabase(vdirObject, task);
		}

		public static void SetToMetabase(ADMobileVirtualDirectory adObject, Task task)
		{
			string metabasePath = adObject.MetabasePath;
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(metabasePath))
			{
				ArrayList arrayList = new ArrayList();
				int? num = (int?)directoryEntry.Properties["AccessSSLFlags"].Value;
				if (num == null)
				{
					num = new int?(0);
				}
				if (adObject.ClientCertAuth == ClientCertAuthTypes.Ignore)
				{
					int num2 = 96;
					num &= ~num2;
				}
				else if (adObject.ClientCertAuth == ClientCertAuthTypes.Required)
				{
					num |= 96;
				}
				else if (adObject.ClientCertAuth == ClientCertAuthTypes.Accepted)
				{
					num = ((num & -65) | 32);
				}
				else
				{
					int num3 = 96;
					num &= ~num3;
				}
				MetabaseProperty value = new MetabaseProperty("AccessSSLFlags", num);
				arrayList.Add(value);
				IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic, adObject.BasicAuthEnabled);
				IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.WindowsIntegrated, adObject.WindowsAuthEnabled);
				MetabaseProperty value2 = new MetabaseProperty("DoDynamicCompression", adObject.CompressionEnabled);
				arrayList.Add(value2);
				IisUtility.SetProperties(directoryEntry, arrayList);
				directoryEntry.CommitChanges();
				string vdirPath = string.Format("{0}/{1}", adObject.MetabasePath, "Proxy");
				MobileSyncVirtualDirectoryHelper.UpdateSubDirectory(adObject, vdirPath, task);
				IisUtility.CommitMetabaseChanges((adObject.Server == null) ? null : adObject.Server.ToString());
			}
		}

		internal static void InstallIsapiFilter(ADMobileVirtualDirectory vdirObject, bool forCafe)
		{
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(vdirObject.MetabasePath))
			{
				if (forCafe)
				{
					ActiveSyncIsapiFilter.InstallForCafe(directoryEntry);
				}
				else
				{
					ActiveSyncIsapiFilter.Install(directoryEntry);
				}
			}
		}

		internal static void UninstallIsapiFilter(ADMobileVirtualDirectory vdirObject)
		{
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(vdirObject.MetabasePath))
			{
				ActiveSyncIsapiFilter.Uninstall(directoryEntry);
			}
		}

		internal static void InstallProxySubDirectory(ADMobileVirtualDirectory dataObject, Task task)
		{
			dataObject.InitProxyVDirDataObject();
			MobileSyncVirtualDirectoryHelper.InstallSubDirectory(dataObject, "Proxy", task);
		}

		private static void InstallSubDirectory(ADMobileVirtualDirectory dataObject, string subDirectoryName, Task task)
		{
			bool flag = false;
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(dataObject.MetabasePath, new Task.TaskErrorLoggingReThrowDelegate(task.WriteError), dataObject.Identity))
			{
				string path = Path.Combine(directoryEntry.Properties["Path"].Value.ToString(), subDirectoryName);
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				string text = string.Format("{0}/{1}", dataObject.MetabasePath, subDirectoryName);
				if (!IisUtility.Exists(text))
				{
					IisUtility.CreateWebDirectory(directoryEntry, subDirectoryName);
					MobileSyncVirtualDirectoryHelper.UpdateSubDirectory(dataObject, text, task);
					directoryEntry.CommitChanges();
					IisUtility.CommitMetabaseChanges((dataObject.Server == null) ? null : dataObject.Server.Name);
					flag = true;
				}
				ExTraceGlobals.TaskTracer.Information(0L, string.Format("Installed SubDirectory '{0}' with result '{1}'", subDirectoryName, flag));
			}
		}

		private static void CopyProxyExtendedParameters(ADMobileVirtualDirectory adObject)
		{
			adObject.ProxyVirtualDirectoryObject.ExtendedProtectionTokenChecking = adObject.ExtendedProtectionTokenChecking;
			adObject.ProxyVirtualDirectoryObject.ExtendedProtectionFlags = adObject.ExtendedProtectionFlags;
			adObject.ProxyVirtualDirectoryObject.ExtendedProtectionSPNList = adObject.ExtendedProtectionSPNList;
		}

		private static void UpdateSubDirectory(ADMobileVirtualDirectory dataObject, string vdirPath, Task task)
		{
			if (IisUtility.Exists(vdirPath))
			{
				using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(vdirPath))
				{
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.None, false);
					IisUtility.SetAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.WindowsIntegrated, true);
					IisUtility.SetProperty(directoryEntry, "AccessFlags", MetabasePropertyTypes.AccessFlags.Script, true);
					directoryEntry.CommitChanges();
				}
			}
			if (dataObject.ProxyVirtualDirectoryObject != null)
			{
				MobileSyncVirtualDirectoryHelper.CopyProxyExtendedParameters(dataObject);
				ExtendedProtection.CommitToMetabase(dataObject.ProxyVirtualDirectoryObject, task);
			}
		}

		private const uint IisPropertiesCount = 5U;
	}
}
