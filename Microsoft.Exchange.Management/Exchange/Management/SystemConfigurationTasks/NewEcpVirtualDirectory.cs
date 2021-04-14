using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "EcpVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewEcpVirtualDirectory : NewWebAppVirtualDirectory<ADEcpVirtualDirectory>
	{
		public NewEcpVirtualDirectory()
		{
			this.Name = "ecp";
			base.AppPoolId = "MSExchangeECPAppPool";
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			private set
			{
				base.Name = value;
			}
		}

		public new string ApplicationRoot
		{
			get
			{
				return base.ApplicationRoot;
			}
			private set
			{
				base.ApplicationRoot = value;
			}
		}

		internal new MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				return base.ExternalAuthenticationMethods;
			}
			private set
			{
				base.ExternalAuthenticationMethods = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewEcpVirtualDirectory(base.WebSiteName, base.Server.ToString());
			}
		}

		protected override ArrayList CustomizedVDirProperties
		{
			get
			{
				return new ArrayList
				{
					new MetabaseProperty("DefaultDoc", "default.aspx"),
					new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script),
					new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Basic),
					new MetabaseProperty("AppFriendlyName", this.Name),
					new MetabaseProperty("AppRoot", base.AppRootValue),
					new MetabaseProperty("AppIsolated", MetabasePropertyTypes.AppIsolated.Pooled)
				};
			}
		}

		protected override ListDictionary ChildVirtualDirectories
		{
			get
			{
				ListDictionary listDictionary = new ListDictionary();
				if (base.Role == VirtualDirectoryRole.Mailbox)
				{
					this.AddThemesDirectory(listDictionary, "App_Themes");
					this.AddThemesDirectory(listDictionary, NewEcpVirtualDirectory.GetEcpAssemblyVersion());
					this.AddCertAuthDirectory(listDictionary, "ReportingWebService");
				}
				return listDictionary;
			}
		}

		protected override bool VerifyRoleConsistency()
		{
			if (base.Role == VirtualDirectoryRole.ClientAccess && !base.OwningServer.IsCafeServer)
			{
				base.WriteError(new ArgumentException("Argument: -Role ClientAccess"), ErrorCategory.InvalidArgument, null);
				return false;
			}
			if (base.Role == VirtualDirectoryRole.Mailbox && !base.OwningServer.IsCafeServer && !base.OwningServer.IsClientAccessServer && !base.OwningServer.IsFfoWebServiceRole && !base.OwningServer.IsOSPRole)
			{
				base.WriteError(new ArgumentException("Argument: -Role Mailbox"), ErrorCategory.InvalidArgument, null);
				return false;
			}
			return true;
		}

		protected override bool ShouldCreateVirtualDirectory()
		{
			base.ShouldCreateVirtualDirectory();
			return this.VerifyRoleConsistency();
		}

		private void AddThemesDirectory(ListDictionary dirs, string path)
		{
			if (Directory.Exists(System.IO.Path.Combine(base.Path, path)))
			{
				dirs.Add(path, new ArrayList
				{
					new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Anonymous),
					new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read)
				});
			}
		}

		private void AddCertAuthDirectory(ListDictionary dirs, string path)
		{
			if (Directory.Exists(System.IO.Path.Combine(base.Path, path)))
			{
				dirs.Add(path, new ArrayList
				{
					new MetabaseProperty("AccessSSLFlags", MetabasePropertyTypes.AccessSSLFlags.AccessSSL | MetabasePropertyTypes.AccessSSLFlags.AccessSSLNegotiateCert | MetabasePropertyTypes.AccessSSLFlags.AccessSSLRequireCert | MetabasePropertyTypes.AccessSSLFlags.AccessSSL128),
					new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script)
				});
			}
		}

		private static string GetEcpAssemblyVersion()
		{
			string text = NewEcpVirtualDirectory.EcpVersionDllPath;
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
				throw new GetEcpVersionException(text, innerException);
			}
			return result;
		}

		private static string EcpVersionDllPath
		{
			get
			{
				if (string.IsNullOrEmpty(NewEcpVirtualDirectory.ecpVersionDllPath))
				{
					NewEcpVirtualDirectory.ecpVersionDllPath = System.IO.Path.Combine(NewEcpVirtualDirectory.EcpPath, "Bin\\Microsoft.Exchange.Management.ControlPanel.dll");
				}
				return NewEcpVirtualDirectory.ecpVersionDllPath;
			}
		}

		private static string EcpPath
		{
			get
			{
				if (string.IsNullOrEmpty(NewEcpVirtualDirectory.ecpPath))
				{
					NewEcpVirtualDirectory.ecpPath = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess\\ecp");
				}
				return NewEcpVirtualDirectory.ecpPath;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			if (!base.Fields.IsModified("Path"))
			{
				base.Path = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, (base.Role == VirtualDirectoryRole.ClientAccess) ? "FrontEnd\\HttpProxy\\ecp" : "ClientAccess\\ecp");
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (!new VirtualDirectoryPathExistsCondition(base.OwningServer.Fqdn, base.Path).Verify())
			{
				base.WriteError(new ArgumentException(Strings.ErrorPathNotExistsOnServer(base.Path, base.OwningServer.Name), "Path"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override bool InternalProcessStartWork()
		{
			this.SetDefaults();
			return true;
		}

		protected override void InternalProcessMetabase()
		{
			ADOwaVirtualDirectory adowaVirtualDirectory = WebAppVirtualDirectoryHelper.FindWebAppVirtualDirectoryInSameWebSite<ADOwaVirtualDirectory>(this.DataObject, base.DataSession);
			if (adowaVirtualDirectory != null && !string.IsNullOrEmpty(adowaVirtualDirectory.DefaultDomain))
			{
				this.DataObject.DefaultDomain = adowaVirtualDirectory.DefaultDomain;
			}
			WebAppVirtualDirectoryHelper.UpdateMetabase(this.DataObject, this.DataObject.MetabasePath, true);
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.ReportingWebService.Enabled)
			{
				string physicalPath;
				List<MetabaseProperty> metabaseProperties;
				if (base.Role == VirtualDirectoryRole.ClientAccess)
				{
					physicalPath = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, "FrontEnd\\HttpProxy\\ReportingWebService");
					metabaseProperties = this.CreateFrontEndVdirProperies();
				}
				else
				{
					physicalPath = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess\\ReportingWebService");
					metabaseProperties = this.CreateBackEndVdirProperies();
				}
				this.CreateReportingWebServiceVDir(this.DataObject.MetabasePath, physicalPath, metabaseProperties);
				if (base.Role == VirtualDirectoryRole.ClientAccess)
				{
					string parent = string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
					{
						this.DataObject.MetabasePath,
						"ReportingWebService"
					});
					this.CreatePartnerVDir(parent);
				}
			}
		}

		protected override void WriteResultMetabaseFixup(ExchangeVirtualDirectory targetDataObject)
		{
			base.WriteResultMetabaseFixup(targetDataObject);
			if (WebAppVirtualDirectoryHelper.FindWebAppVirtualDirectoryInSameWebSite<ADOwaVirtualDirectory>((ExchangeWebAppVirtualDirectory)targetDataObject, base.DataSession) == null)
			{
				this.WriteWarning(Strings.CreateOwaForEcpWarning);
			}
		}

		private void SetDefaults()
		{
			this.DataObject.GzipLevel = GzipLevel.High;
			this.DataObject.FormsAuthentication = (base.Role == VirtualDirectoryRole.ClientAccess);
			this.DataObject.BasicAuthentication = (base.Role == VirtualDirectoryRole.ClientAccess);
			this.DataObject.WindowsAuthentication = (base.Role == VirtualDirectoryRole.Mailbox);
			this.DataObject.LiveIdAuthentication = false;
			this.DataObject.DigestAuthentication = false;
			this.DataObject.ExternalAuthenticationMethods = new MultiValuedProperty<AuthenticationMethod>(AuthenticationMethod.Fba);
		}

		private void CreateReportingWebServiceVDir(string metabasePath, string physicalPath, List<MetabaseProperty> metabaseProperties)
		{
			TaskLogger.LogEnter();
			CreateVirtualDirectory createVirtualDirectory = new CreateVirtualDirectory();
			createVirtualDirectory.Name = "ReportingWebService";
			createVirtualDirectory.Parent = metabasePath;
			createVirtualDirectory.LocalPath = physicalPath;
			createVirtualDirectory.CustomizedVDirProperties = metabaseProperties;
			createVirtualDirectory.ApplicationPool = "MSExchangeReportingWebServiceAppPool";
			createVirtualDirectory.AppPoolIdentityType = MetabasePropertyTypes.AppPoolIdentityType.LocalSystem;
			createVirtualDirectory.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
			createVirtualDirectory.Initialize();
			createVirtualDirectory.Execute();
			TaskLogger.LogExit();
		}

		private void CreatePartnerVDir(string parent)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add(new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.NoneSet));
			arrayList.Add(new MetabaseProperty("AccessSSLFlags", MetabasePropertyTypes.AccessSSLFlags.AccessSSL | MetabasePropertyTypes.AccessSSLFlags.AccessSSLNegotiateCert | MetabasePropertyTypes.AccessSSLFlags.AccessSSLRequireCert | MetabasePropertyTypes.AccessSSLFlags.AccessSSL128));
			arrayList.Add(new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script));
			CreateVirtualDirectory createVirtualDirectory = new CreateVirtualDirectory();
			createVirtualDirectory.Name = "partner";
			createVirtualDirectory.Parent = parent;
			createVirtualDirectory.CustomizedVDirProperties = arrayList;
			createVirtualDirectory.LocalPath = null;
			createVirtualDirectory.Initialize();
			createVirtualDirectory.Execute();
		}

		private List<MetabaseProperty> CreateFrontEndVdirProperies()
		{
			return new List<MetabaseProperty>
			{
				new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.NoneSet),
				new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script),
				new MetabaseProperty("AppIsolated", MetabasePropertyTypes.AppIsolated.Pooled)
			};
		}

		private List<MetabaseProperty> CreateBackEndVdirProperies()
		{
			List<MetabaseProperty> list = new List<MetabaseProperty>();
			list.Add(new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Anonymous | MetabasePropertyTypes.AuthFlags.Ntlm));
			string value = string.Format("{0},{1}", "Negotiate", "NTLM");
			list.Add(new MetabaseProperty("NTAuthenticationProviders", value));
			list.Add(new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script));
			list.Add(new MetabaseProperty("AppIsolated", MetabasePropertyTypes.AppIsolated.Pooled));
			return list;
		}

		private const string DefaultVDirName = "ecp";

		private const string LocalPath = "ClientAccess\\ecp";

		private const string CafePath = "FrontEnd\\HttpProxy\\ecp";

		private const string DefaultApplicationPool = "MSExchangeECPAppPool";

		private const string AppThemesPath = "App_Themes";

		private const string ReportingWebServicePath = "ReportingWebService";

		private const string ReportingWebServiceVDirName = "ReportingWebService";

		private const string ReportingWebServiceVDirPath = "ClientAccess\\ReportingWebService";

		private const string ReportingWebServiceVDirFrontEndPath = "FrontEnd\\HttpProxy\\ReportingWebService";

		private const string ReportingWebServiceApplicationPool = "MSExchangeReportingWebServiceAppPool";

		private static string ecpPath;

		private static string ecpVersionDllPath;
	}
}
