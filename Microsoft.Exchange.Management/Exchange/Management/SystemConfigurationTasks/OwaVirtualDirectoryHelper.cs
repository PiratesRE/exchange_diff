using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.IisTasks;
using Microsoft.Exchange.Management.Metabase;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class OwaVirtualDirectoryHelper : WebAppVirtualDirectoryHelper
	{
		private OwaVirtualDirectoryHelper()
		{
		}

		internal static string OwaPath
		{
			get
			{
				if (string.IsNullOrEmpty(OwaVirtualDirectoryHelper.owaPath))
				{
					OwaVirtualDirectoryHelper.owaPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess\\owa");
				}
				return OwaVirtualDirectoryHelper.owaPath;
			}
		}

		internal static string OwaCafePath
		{
			get
			{
				if (string.IsNullOrEmpty(OwaVirtualDirectoryHelper.cafePath))
				{
					OwaVirtualDirectoryHelper.cafePath = Path.Combine(ConfigurationContext.Setup.InstallPath, "FrontEnd\\HttpProxy\\owa");
				}
				return OwaVirtualDirectoryHelper.cafePath;
			}
		}

		internal static string OwaVersionDllPath
		{
			get
			{
				if (string.IsNullOrEmpty(OwaVirtualDirectoryHelper.owaVersionDllPath))
				{
					OwaVirtualDirectoryHelper.owaVersionDllPath = Path.Combine(OwaVirtualDirectoryHelper.OwaPath, "Bin\\Microsoft.Exchange.Clients.Owa.dll");
				}
				return OwaVirtualDirectoryHelper.owaVersionDllPath;
			}
		}

		internal static void EnableIsapiFilter(ADOwaVirtualDirectory adOwaVirtualDirectory, bool forCafe)
		{
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(adOwaVirtualDirectory.MetabasePath))
			{
				if (forCafe)
				{
					OwaIsapiFilter.InstallForCafe(directoryEntry);
				}
				else
				{
					OwaIsapiFilter.Install(directoryEntry);
				}
			}
		}

		internal static void DisableIsapiFilter(ADOwaVirtualDirectory adOwaVirtualDirectory)
		{
			DirectoryEntry directoryEntry;
			DirectoryEntry virtualDirectory = directoryEntry = IisUtility.CreateIISDirectoryEntry(adOwaVirtualDirectory.MetabasePath);
			try
			{
				OwaIsapiFilter.UninstallIfLastVdir(virtualDirectory);
			}
			finally
			{
				if (directoryEntry != null)
				{
					((IDisposable)directoryEntry).Dispose();
				}
			}
		}

		public static void CopyDavVdirsToMetabase(string domainController, string exchangeServerName, string metabaseServerName)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(domainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 154, "CopyDavVdirsToMetabase", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\VirtualDirectoryTasks\\OWAVirtualDirectoryHelper.cs");
			Server server = topologyConfigurationSession.FindServerByName(exchangeServerName);
			IConfigDataProvider configDataProvider = topologyConfigurationSession;
			IEnumerable<ADOwaVirtualDirectory> enumerable = configDataProvider.FindPaged<ADOwaVirtualDirectory>(null, server.Id, true, null, 0);
			OwaVirtualDirectoryHelper.SetWebSvcExtRestrictionList(metabaseServerName);
			foreach (ADOwaVirtualDirectory adowaVirtualDirectory in enumerable)
			{
				if (!adowaVirtualDirectory.IsExchange2007OrLater)
				{
					string[] array = adowaVirtualDirectory.MetabasePath.Split(new char[]
					{
						'/'
					});
					if (array.Length == 7)
					{
						array[2] = metabaseServerName;
						MultiValuedProperty<AuthenticationMethod> internalAuthenticationMethods = adowaVirtualDirectory.InternalAuthenticationMethods;
						adowaVirtualDirectory.WindowsAuthentication = true;
						string appPoolRootPath = IisUtility.GetAppPoolRootPath(metabaseServerName);
						string text = "MSExchangeOWAAppPool";
						if (!IisUtility.Exists(appPoolRootPath, text, "IIsApplicationPool"))
						{
							using (DirectoryEntry directoryEntry = IisUtility.CreateApplicationPool(metabaseServerName, text))
							{
								IisUtility.SetProperty(directoryEntry, "AppPoolIdentityType", 0, true);
								directoryEntry.CommitChanges();
							}
						}
						if (!IisUtility.Exists(string.Join("/", array)))
						{
							DirectoryEntry directoryEntry2 = IisUtility.CreateWebDirObject(string.Join("/", array, 0, 6), adowaVirtualDirectory.FolderPathname, array[6]);
							ArrayList arrayList = new ArrayList();
							arrayList.Add(new MetabaseProperty("LogonMethod", MetabasePropertyTypes.LogonMethod.ClearTextLogon));
							arrayList.Add(new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Write | MetabasePropertyTypes.AccessFlags.Source | MetabasePropertyTypes.AccessFlags.Script));
							arrayList.Add(new MetabaseProperty("DirBrowseFlags", (MetabasePropertyTypes.DirBrowseFlags)3221225534U));
							arrayList.Add(new MetabaseProperty("ScriptMaps", OwaVirtualDirectoryHelper.GetDavScriptMaps(), true));
							if (adowaVirtualDirectory.VirtualDirectoryType == VirtualDirectoryTypes.Exchweb)
							{
								arrayList.Add(new MetabaseProperty("HttpExpires", "D, 0x278d00"));
							}
							if (adowaVirtualDirectory.DefaultDomain.Length > 0)
							{
								arrayList.Add(new MetabaseProperty("DefaultLogonDomain", adowaVirtualDirectory.DefaultDomain, true));
							}
							OwaIsapiFilter.DisableFba(directoryEntry2);
							uint num = 0U;
							using (MultiValuedProperty<AuthenticationMethod>.Enumerator enumerator2 = adowaVirtualDirectory.InternalAuthenticationMethods.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									switch (enumerator2.Current)
									{
									case AuthenticationMethod.Basic:
										num |= 2U;
										break;
									case AuthenticationMethod.Digest:
										num |= 16U;
										break;
									case AuthenticationMethod.Ntlm:
										num |= 4U;
										break;
									case AuthenticationMethod.Fba:
										OwaIsapiFilter.EnableFba(directoryEntry2);
										break;
									}
								}
							}
							arrayList.Add(new MetabaseProperty("AuthFlags", num, true));
							IisUtility.SetProperties(directoryEntry2, arrayList);
							IisUtility.AssignApplicationPool(directoryEntry2, text);
						}
					}
				}
			}
		}

		internal static bool IsDataCenterCafe(VirtualDirectoryRole role)
		{
			return role == VirtualDirectoryRole.ClientAccess && Datacenter.IsMicrosoftHostedOnly(true);
		}

		internal static void SetWebSvcExtRestrictionList(string metabaseServerName)
		{
			using (IsapiExtensionList isapiExtensionList = new IsapiExtensionList(metabaseServerName))
			{
				string groupID = "MSEXCHANGE";
				string description = "Microsoft Exchange Server";
				string physicalPath;
				if (RoleManager.GetRoleByName("MailboxRole").IsInstalled || RoleManager.GetRoleByName("MailboxRole").IsPartiallyInstalled)
				{
					physicalPath = Path.Combine(ConfigurationContext.Setup.BinPath, "davex.dll");
				}
				else
				{
					physicalPath = Path.Combine(ConfigurationContext.Setup.BinPath, "exprox.dll");
				}
				isapiExtensionList.Add(true, physicalPath, false, groupID, description);
				isapiExtensionList.CommitChanges();
			}
		}

		internal static string GetDavScriptMaps()
		{
			Role roleByName = RoleManager.GetRoleByName("MailboxRole");
			ConfigurationStatus configurationStatus = new ConfigurationStatus("MailboxRole");
			RolesUtility.GetConfiguringStatus(ref configurationStatus);
			string path;
			if (roleByName.IsInstalled || (roleByName.IsPartiallyInstalled && configurationStatus.Action != InstallationModes.Uninstall))
			{
				path = "davex.dll";
			}
			else
			{
				path = "exprox.dll";
			}
			string str = Path.Combine(ConfigurationContext.Setup.BinPath, path);
			return "*," + str + ",1";
		}

		internal static void AddVersionVDir(ListDictionary childVDirs)
		{
			ArrayList versionVDirProperties = OwaVirtualDirectoryHelper.GetVersionVDirProperties();
			string owaAssemblyVersion = OwaVirtualDirectoryHelper.GetOwaAssemblyVersion();
			childVDirs.Add(owaAssemblyVersion, versionVDirProperties);
		}

		internal static void CreateLegacyVDirs(string metabasePath, bool deleteObjectIfExists)
		{
			string webSiteRoot = IisUtility.GetWebSiteRoot(metabasePath);
			IList legacyVirtualDirectories = OwaVirtualDirectoryHelper.GetLegacyVirtualDirectories();
			if (legacyVirtualDirectories != null)
			{
				ArrayList arrayList = new ArrayList();
				arrayList.Add(new MetabaseProperty("HttpRedirect", "/owa"));
				string localPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess\\owa");
				OwaVirtualDirectoryHelper.CreatedLegacyVDirs.Clear();
				foreach (object obj in legacyVirtualDirectories)
				{
					string text = (string)obj;
					if (deleteObjectIfExists && IisUtility.WebDirObjectExists(webSiteRoot, text))
					{
						IisUtility.DeleteWebDirObject(webSiteRoot, text);
					}
					CreateVirtualDirectory createVirtualDirectory = new CreateVirtualDirectory();
					createVirtualDirectory.Name = text;
					createVirtualDirectory.Parent = webSiteRoot;
					createVirtualDirectory.LocalPath = localPath;
					createVirtualDirectory.CustomizedVDirProperties = arrayList;
					createVirtualDirectory.Initialize();
					createVirtualDirectory.Execute();
					OwaVirtualDirectoryHelper.CreatedLegacyVDirs.Add(text);
				}
			}
		}

		internal static string GetOwaAssemblyVersion()
		{
			string text = OwaVirtualDirectoryHelper.OwaVersionDllPath;
			string result;
			try
			{
				FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(text);
				string text2 = string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					versionInfo.FileMajorPart,
					versionInfo.FileMinorPart,
					versionInfo.FileBuildPart,
					versionInfo.FilePrivatePart
				});
				result = text2;
			}
			catch (FileNotFoundException innerException)
			{
				throw new GetOwaVersionException(text, innerException);
			}
			return result;
		}

		internal static ArrayList GetVersionVDirProperties()
		{
			return new ArrayList
			{
				new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Anonymous),
				new MetabaseProperty("LogonMethod", MetabasePropertyTypes.LogonMethod.ClearTextLogon),
				new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read),
				new MetabaseProperty("HttpExpires", "D, 0x278d00")
			};
		}

		internal static IList GetLegacyVirtualDirectories()
		{
			return new List<string>
			{
				"Exchange",
				"Exchweb",
				"Public"
			};
		}

		private static List<MetabaseProperty> GetOwaCalendarVDirProperties(VirtualDirectoryRole role)
		{
			List<MetabaseProperty> list = new List<MetabaseProperty>();
			list.Add(new MetabaseProperty("Path", (role == VirtualDirectoryRole.Mailbox) ? OwaVirtualDirectoryHelper.OwaPath : OwaVirtualDirectoryHelper.OwaCafePath));
			list.Add(new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Anonymous));
			list.Add(new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script));
			list.Add(new MetabaseProperty("AppIsolated", MetabasePropertyTypes.AppIsolated.Pooled));
			if (!OwaVirtualDirectoryHelper.IsDataCenterCafe(role))
			{
				list.Add(new MetabaseProperty("AccessSSLFlags", MetabasePropertyTypes.AccessSSLFlags.None));
			}
			return list;
		}

		internal static void CreateOwaCalendarVDir(string metabasePath, VirtualDirectoryRole role)
		{
			MetabasePropertyTypes.AppPoolIdentityType appPoolIdentityType = MetabasePropertyTypes.AppPoolIdentityType.LocalSystem;
			if (IisUtility.WebDirObjectExists(metabasePath, "Calendar"))
			{
				string hostName = IisUtility.GetHostName(metabasePath);
				string appPoolRootPath = IisUtility.GetAppPoolRootPath(hostName);
				using (DirectoryEntry directoryEntry = IisUtility.FindWebObject(appPoolRootPath, "MSExchangeOWACalendarAppPool", "IIsApplicationPool"))
				{
					IisUtility.SetProperty(directoryEntry, "AppPoolIdentityType", appPoolIdentityType, true);
					directoryEntry.CommitChanges();
				}
				return;
			}
			CreateVirtualDirectory createVirtualDirectory = new CreateVirtualDirectory();
			createVirtualDirectory.Name = "Calendar";
			createVirtualDirectory.Parent = metabasePath;
			createVirtualDirectory.LocalPath = ((role == VirtualDirectoryRole.Mailbox) ? OwaVirtualDirectoryHelper.OwaPath : OwaVirtualDirectoryHelper.OwaCafePath);
			createVirtualDirectory.CustomizedVDirProperties = OwaVirtualDirectoryHelper.GetOwaCalendarVDirProperties(role);
			createVirtualDirectory.ApplicationPool = "MSExchangeOWACalendarAppPool";
			createVirtualDirectory.AppPoolIdentityType = appPoolIdentityType;
			createVirtualDirectory.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
			createVirtualDirectory.AppPoolQueueLength = 10;
			createVirtualDirectory.Initialize();
			createVirtualDirectory.Execute();
		}

		private static List<MetabaseProperty> GetOwaIntegratedVDirProperties(VirtualDirectoryRole role)
		{
			List<MetabaseProperty> list = new List<MetabaseProperty>();
			list.Add(new MetabaseProperty("Path", OwaVirtualDirectoryHelper.OwaCafePath));
			list.Add(new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Ntlm));
			list.Add(new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script));
			list.Add(new MetabaseProperty("AppIsolated", MetabasePropertyTypes.AppIsolated.Pooled));
			if (!OwaVirtualDirectoryHelper.IsDataCenterCafe(role))
			{
				list.Add(new MetabaseProperty("AccessSSLFlags", MetabasePropertyTypes.AccessSSLFlags.AccessSSL | MetabasePropertyTypes.AccessSSLFlags.AccessSSL128));
			}
			return list;
		}

		internal static void CreateOwaIntegratedVDir(string metabasePath, VirtualDirectoryRole role)
		{
			MetabasePropertyTypes.AppPoolIdentityType appPoolIdentityType = MetabasePropertyTypes.AppPoolIdentityType.LocalSystem;
			if (IisUtility.WebDirObjectExists(metabasePath, "Integrated"))
			{
				string hostName = IisUtility.GetHostName(metabasePath);
				string appPoolRootPath = IisUtility.GetAppPoolRootPath(hostName);
				using (DirectoryEntry directoryEntry = IisUtility.FindWebObject(appPoolRootPath, "MSExchangeOWAAppPool", "IIsApplicationPool"))
				{
					IisUtility.SetProperty(directoryEntry, "AppPoolIdentityType", appPoolIdentityType, true);
					directoryEntry.CommitChanges();
				}
				return;
			}
			CreateVirtualDirectory createVirtualDirectory = new CreateVirtualDirectory();
			createVirtualDirectory.Name = "Integrated";
			createVirtualDirectory.Parent = metabasePath;
			createVirtualDirectory.LocalPath = OwaVirtualDirectoryHelper.OwaCafePath;
			createVirtualDirectory.CustomizedVDirProperties = OwaVirtualDirectoryHelper.GetOwaIntegratedVDirProperties(role);
			createVirtualDirectory.ApplicationPool = "MSExchangeOWAAppPool";
			createVirtualDirectory.AppPoolIdentityType = appPoolIdentityType;
			createVirtualDirectory.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
			createVirtualDirectory.AppPoolQueueLength = 10;
			createVirtualDirectory.Initialize();
			createVirtualDirectory.Execute();
		}

		private static List<MetabaseProperty> GetOmaVDirProperties(VirtualDirectoryRole role)
		{
			List<MetabaseProperty> list = new List<MetabaseProperty>();
			list.Add(new MetabaseProperty("Path", OwaVirtualDirectoryHelper.OwaCafePath));
			list.Add(new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Basic));
			list.Add(new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script));
			list.Add(new MetabaseProperty("AppIsolated", MetabasePropertyTypes.AppIsolated.Pooled));
			if (!OwaVirtualDirectoryHelper.IsDataCenterCafe(role))
			{
				list.Add(new MetabaseProperty("AccessSSLFlags", MetabasePropertyTypes.AccessSSLFlags.AccessSSL | MetabasePropertyTypes.AccessSSLFlags.AccessSSL128));
			}
			return list;
		}

		internal static void CreateOmaVDir(string metabasePath, VirtualDirectoryRole role)
		{
			MetabasePropertyTypes.AppPoolIdentityType appPoolIdentityType = MetabasePropertyTypes.AppPoolIdentityType.LocalSystem;
			if (IisUtility.WebDirObjectExists(metabasePath, "oma"))
			{
				string hostName = IisUtility.GetHostName(metabasePath);
				string appPoolRootPath = IisUtility.GetAppPoolRootPath(hostName);
				using (DirectoryEntry directoryEntry = IisUtility.FindWebObject(appPoolRootPath, "MSExchangeOWAAppPool", "IIsApplicationPool"))
				{
					IisUtility.SetProperty(directoryEntry, "AppPoolIdentityType", appPoolIdentityType, true);
					directoryEntry.CommitChanges();
				}
				return;
			}
			CreateVirtualDirectory createVirtualDirectory = new CreateVirtualDirectory();
			createVirtualDirectory.Name = "oma";
			createVirtualDirectory.Parent = metabasePath;
			createVirtualDirectory.LocalPath = OwaVirtualDirectoryHelper.OwaCafePath;
			createVirtualDirectory.CustomizedVDirProperties = OwaVirtualDirectoryHelper.GetOmaVDirProperties(role);
			createVirtualDirectory.ApplicationPool = "MSExchangeOWAAppPool";
			createVirtualDirectory.AppPoolIdentityType = appPoolIdentityType;
			createVirtualDirectory.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
			createVirtualDirectory.AppPoolQueueLength = 10;
			createVirtualDirectory.Initialize();
			createVirtualDirectory.Execute();
		}

		internal const string LocalPath = "ClientAccess\\owa";

		internal const string CafePath = "FrontEnd\\HttpProxy\\owa";

		internal const string CafeBinPath = "FrontEnd\\HttpProxy\\bin";

		internal const string DefaultApplicationPool = "MSExchangeOWAAppPool";

		internal const string OwaCalendarApplicationPool = "MSExchangeOWACalendarAppPool";

		private const string defaultExchangeName = "Exchange";

		private const string defaultPublicName = "Public";

		private const string defaultExchwebName = "Exchweb";

		private const string defaultExadminName = "Exadmin";

		internal const string defaultE12Name = "owa";

		internal const string CalendarVdirName = "Calendar";

		internal const string IntegratedVdirName = "Integrated";

		internal const string OmaVdirName = "oma";

		private static string owaPath;

		private static string cafePath;

		private static string owaVersionDllPath;

		internal static List<string> CreatedLegacyVDirs = new List<string>();
	}
}
