using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Web.Configuration;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.VisualBasic.Devices;
using Microsoft.Web.Administration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class NewExchangeVirtualDirectory<T> : NewVirtualDirectory<T> where T : ExchangeVirtualDirectory, new()
	{
		internal MetabasePropertyTypes.ManagedPipelineMode AppPoolManagedPipelineMode
		{
			get
			{
				return this.appPoolManagedPipelineMode;
			}
			set
			{
				this.appPoolManagedPipelineMode = value;
			}
		}

		internal bool LimitMaximumMemory
		{
			get
			{
				return (bool)(base.Fields["LimitMaximumMemory"] ?? false);
			}
			set
			{
				base.Fields["LimitMaximumMemory"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string WebSiteName
		{
			get
			{
				return (string)base.Fields["WebSiteName"];
			}
			set
			{
				base.Fields["WebSiteName"] = value;
				if (value != null && value.Equals("Exchange Back End", StringComparison.OrdinalIgnoreCase))
				{
					this.Role = VirtualDirectoryRole.Mailbox;
				}
			}
		}

		[Parameter(Mandatory = false)]
		public string Path
		{
			get
			{
				return (string)base.Fields["Path"];
			}
			set
			{
				base.Fields["Path"] = LocalLongFullPath.Parse(value).PathName;
			}
		}

		[Parameter(Mandatory = false)]
		public string AppPoolId
		{
			get
			{
				return (string)base.Fields["AppPoolId"];
			}
			set
			{
				base.Fields["AppPoolId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ApplicationRoot
		{
			get
			{
				return (string)base.Fields["ApplicationRoot"];
			}
			set
			{
				base.Fields["ApplicationRoot"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExtendedProtectionTokenCheckingMode ExtendedProtectionTokenChecking
		{
			get
			{
				return (ExtendedProtectionTokenCheckingMode)(base.Fields["ExtendedProtectionTokenChecking"] ?? ExtendedProtectionTokenCheckingMode.None);
			}
			set
			{
				base.Fields["ExtendedProtectionTokenChecking"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ExtendedProtectionFlag> ExtendedProtectionFlags
		{
			get
			{
				return ExchangeVirtualDirectory.ExtendedProtectionFlagsToMultiValuedProperty((ExtendedProtectionFlag)base.Fields["ExtendedProtectionFlags"]);
			}
			set
			{
				base.Fields["ExtendedProtectionFlags"] = (int)ExchangeVirtualDirectory.ExtendedProtectionMultiValuedPropertyToFlags(value);
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtendedProtectionSPNList
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["ExtendedProtectionSPNList"];
			}
			set
			{
				base.Fields["ExtendedProtectionSPNList"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public VirtualDirectoryRole Role
		{
			get
			{
				return this.role;
			}
			set
			{
				this.role = value;
			}
		}

		internal string GetAbsolutePath(IisUtility.AbsolutePathType pathType)
		{
			string fqdn = base.OwningServer.Fqdn;
			return IisUtility.CreateAbsolutePath(pathType, fqdn, IisUtility.FindWebSiteRootPath(this.WebSiteName, fqdn), base.Name.ToString());
		}

		protected override IConfigurable PrepareDataObject()
		{
			ExchangeVirtualDirectory exchangeVirtualDirectory = (ExchangeVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields.Contains("ExtendedProtectionTokenChecking"))
			{
				exchangeVirtualDirectory.ExtendedProtectionTokenChecking = (ExtendedProtectionTokenCheckingMode)base.Fields["ExtendedProtectionTokenChecking"];
			}
			if (base.Fields.Contains("ExtendedProtectionSPNList"))
			{
				exchangeVirtualDirectory.ExtendedProtectionSPNList = (MultiValuedProperty<string>)base.Fields["ExtendedProtectionSPNList"];
			}
			if (base.Fields.Contains("ExtendedProtectionFlags"))
			{
				ExtendedProtectionFlag flags = (ExtendedProtectionFlag)base.Fields["ExtendedProtectionFlags"];
				exchangeVirtualDirectory.ExtendedProtectionFlags = ExchangeVirtualDirectory.ExtendedProtectionFlagsToMultiValuedProperty(flags);
			}
			return exchangeVirtualDirectory;
		}

		protected override void InternalValidate()
		{
			object[] array = new object[1];
			object[] array2 = array;
			int num = 0;
			T dataObject = this.DataObject;
			array2[num] = dataObject.Identity;
			TaskLogger.LogEnter(array);
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			string fqdn = base.OwningServer.Fqdn;
			string text = base.Name.ToString();
			if (string.IsNullOrEmpty(this.WebSiteName))
			{
				if (this.Role == VirtualDirectoryRole.ClientAccess)
				{
					this.WebSiteName = IisUtility.GetWebSiteName(IisUtility.CreateAbsolutePath(IisUtility.AbsolutePathType.WebSiteRoot, fqdn, "/W3SVC/1/ROOT", null));
					this.ApplicationRoot = "/W3SVC/1/ROOT";
				}
				else
				{
					this.WebSiteName = "Exchange Back End";
				}
			}
			if (string.Empty.Equals(this.AppPoolId))
			{
				Exception exception = new ArgumentException(Strings.ErrorAppPoolIdCannotBeEmpty, "AppPoolId");
				ErrorCategory category = ErrorCategory.InvalidArgument;
				T dataObject2 = this.DataObject;
				base.WriteError(exception, category, dataObject2.Identity);
			}
			try
			{
				if (!new IisVersionValidCondition(fqdn).Verify())
				{
					Exception exception2 = new ArgumentException(Strings.ErrorIisVersionIsInvalid(fqdn), "Server");
					ErrorCategory category2 = ErrorCategory.InvalidArgument;
					T dataObject3 = this.DataObject;
					base.WriteError(exception2, category2, dataObject3.Identity);
				}
			}
			catch (IOException innerException)
			{
				Exception exception3 = new ArgumentException(Strings.ErrorCannotDetermineIisVersion(fqdn), "Server", innerException);
				ErrorCategory category3 = ErrorCategory.InvalidArgument;
				T dataObject4 = this.DataObject;
				base.WriteError(exception3, category3, dataObject4.Identity);
			}
			catch (InvalidOperationException innerException2)
			{
				Exception exception4 = new ArgumentException(Strings.ErrorCannotDetermineIisVersion(fqdn), "Server", innerException2);
				ErrorCategory category4 = ErrorCategory.InvalidArgument;
				T dataObject5 = this.DataObject;
				base.WriteError(exception4, category4, dataObject5.Identity);
			}
			try
			{
				if (!new WebSiteExistsCondition(fqdn, this.WebSiteName).Verify())
				{
					Exception exception5 = new ArgumentException(Strings.ErrorWebSiteNotExists(this.WebSiteName, fqdn), "WebSiteName");
					ErrorCategory category5 = ErrorCategory.InvalidArgument;
					T dataObject6 = this.DataObject;
					base.WriteError(exception5, category5, dataObject6.Identity);
				}
				if (string.IsNullOrEmpty(this.ApplicationRoot))
				{
					this.ApplicationRoot = IisUtility.FindWebSiteRootPath(this.WebSiteName, fqdn);
				}
			}
			catch (IisUtilityCannotDisambiguateWebSiteException innerException3)
			{
				Exception exception6 = new ArgumentException(Strings.ErrorWebsiteAmbiguousInIIS(this.WebSiteName, fqdn), "WebSiteName", innerException3);
				ErrorCategory category6 = ErrorCategory.InvalidArgument;
				T dataObject7 = this.DataObject;
				base.WriteError(exception6, category6, dataObject7.Identity);
			}
			T dataObject8 = this.DataObject;
			T dataObject9 = this.DataObject;
			dataObject8.SetId(new ADObjectId(dataObject9.Server.DistinguishedName).GetDescendantId("Protocols", "HTTP", new string[]
			{
				string.Format("{0} ({1})", base.Name, this.WebSiteName)
			}));
			if (this.FailOnVirtualDirectoryADObjectAlreadyExists())
			{
				IConfigDataProvider dataSession = base.DataSession;
				T dataObject10 = this.DataObject;
				if (dataSession.Read<T>(dataObject10.Identity) != null)
				{
					string virtualDirectoryName = text;
					T dataObject11 = this.DataObject;
					Exception exception7 = new ArgumentException(Strings.ErrorVirtualDirectoryADObjectAlreadyExists(virtualDirectoryName, dataObject11.DistinguishedName), "VirtualDirectoryName");
					ErrorCategory category7 = ErrorCategory.InvalidArgument;
					T dataObject12 = this.DataObject;
					base.WriteError(exception7, category7, dataObject12.Identity);
				}
			}
			if (this.FailOnVirtualDirectoryAlreadyExists() && new VirtualDirectoryExistsCondition(fqdn, this.WebSiteName, text).Verify())
			{
				Exception exception8 = new ArgumentException(Strings.ErrorVirtualDirectoryAlreadyExists(text, this.WebSiteName, fqdn), "VirtualDirectoryName");
				ErrorCategory category8 = ErrorCategory.InvalidArgument;
				T dataObject13 = this.DataObject;
				base.WriteError(exception8, category8, dataObject13.Identity);
			}
			T dataObject14 = this.DataObject;
			dataObject14.MetabasePath = string.Format("IIS://{0}{1}/{2}", fqdn, IisUtility.FindWebSiteRootPath(this.WebSiteName, fqdn), text);
			ExtendedProtection.Validate(this, this.DataObject);
			TaskLogger.LogExit();
		}

		protected virtual bool InternalProcessStartWork()
		{
			return true;
		}

		protected virtual bool InternalShouldCreateMetabaseObject()
		{
			return true;
		}

		protected virtual void InternalProcessMetabase()
		{
			ExtendedProtection.CommitToMetabase(this.DataObject, this);
		}

		protected virtual void InternalProcessComplete()
		{
		}

		protected virtual void WriteResultMetabaseFixup(ExchangeVirtualDirectory dataObject)
		{
		}

		protected override void InternalProcessRecord()
		{
			object[] array = new object[1];
			object[] array2 = array;
			int num = 0;
			T dataObject = this.DataObject;
			array2[num] = dataObject.Identity;
			TaskLogger.LogEnter(array);
			if (!this.InternalProcessStartWork())
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (this.InternalShouldCreateMetabaseObject())
			{
				try
				{
					flag = this.CreateToMetabase();
					this.UpdateRequestAndThreadsLimits();
					this.InternalProcessMetabase();
					flag2 = true;
				}
				catch (WebObjectAlreadyExistsException ex)
				{
					Exception exception = ex;
					ErrorCategory category = ErrorCategory.InvalidArgument;
					T dataObject2 = this.DataObject;
					base.WriteError(exception, category, dataObject2.Identity);
				}
				catch (Exception ex2)
				{
					TaskLogger.Trace("Exception occurred: {0}", new object[]
					{
						ex2.Message
					});
					T dataObject3 = this.DataObject;
					string name = dataObject3.Server.Name;
					T dataObject4 = this.DataObject;
					Exception exception2 = new InvalidOperationException(Strings.CreateVirtualDirectoryMetabaseFailure(name, dataObject4.MetabasePath), ex2);
					ErrorCategory category2 = ErrorCategory.InvalidOperation;
					T dataObject5 = this.DataObject;
					base.WriteError(exception2, category2, dataObject5.Identity);
				}
				finally
				{
					if (base.HasErrors && !flag2 && flag)
					{
						this.DeleteFromMetabase();
					}
				}
			}
			try
			{
				base.InternalProcessRecord();
				if (!base.HasErrors)
				{
					this.InternalProcessComplete();
				}
			}
			catch (Exception ex3)
			{
				TaskLogger.Trace("Exception occurred: {0}", new object[]
				{
					ex3.Message
				});
				T dataObject6 = this.DataObject;
				string name2 = dataObject6.Server.Name;
				T dataObject7 = this.DataObject;
				Exception exception3 = new InvalidOperationException(Strings.CreateVirtualDirectoryADObjectFailure(name2, dataObject7.MetabasePath), ex3);
				ErrorCategory category3 = ErrorCategory.InvalidOperation;
				T dataObject8 = this.DataObject;
				base.WriteError(exception3, category3, dataObject8.Identity);
			}
			finally
			{
				if (base.HasErrors && flag)
				{
					this.DeleteFromMetabase();
				}
			}
			T dataObject9 = this.DataObject;
			ExchangeVirtualDirectory exchangeVirtualDirectory = (ExchangeVirtualDirectory)base.GetDataObject(new VirtualDirectoryIdParameter((ADObjectId)dataObject9.Identity));
			exchangeVirtualDirectory.Path = this.Path;
			this.WriteResultMetabaseFixup(exchangeVirtualDirectory);
			base.WriteObject(exchangeVirtualDirectory);
			TaskLogger.LogExit();
		}

		protected sealed override void WriteResult()
		{
		}

		protected virtual bool FailOnVirtualDirectoryAlreadyExists()
		{
			return true;
		}

		protected virtual bool FailOnVirtualDirectoryADObjectAlreadyExists()
		{
			return true;
		}

		internal virtual MetabasePropertyTypes.AppPoolIdentityType AppPoolIdentityType
		{
			get
			{
				return MetabasePropertyTypes.AppPoolIdentityType.NetworkService;
			}
		}

		protected virtual ArrayList CustomizedVDirProperties
		{
			get
			{
				return new ArrayList(IisUtility.DefaultWebDirProperties);
			}
		}

		protected virtual ListDictionary ChildVirtualDirectories
		{
			get
			{
				return null;
			}
		}

		protected virtual bool CreateToMetabase()
		{
			bool result = false;
			T dataObject = this.DataObject;
			int num = dataObject.MetabasePath.LastIndexOf('/');
			T dataObject2 = this.DataObject;
			string text = dataObject2.MetabasePath.Substring(0, num);
			T dataObject3 = this.DataObject;
			string text2 = dataObject3.MetabasePath.Substring(num + 1);
			if (!this.FailOnVirtualDirectoryAlreadyExists())
			{
				T dataObject4 = this.DataObject;
				if (IisUtility.Exists(dataObject4.MetabasePath, "IIsWebVirtualDir"))
				{
					IisUtility.DeleteWebDirObject(text, text2);
				}
			}
			CreateVirtualDirectory createVirtualDirectory = new CreateVirtualDirectory();
			createVirtualDirectory.Name = text2;
			createVirtualDirectory.Parent = text;
			createVirtualDirectory.LocalPath = this.Path;
			createVirtualDirectory.ApplicationPool = this.AppPoolId;
			createVirtualDirectory.AppPoolIdentityType = this.AppPoolIdentityType;
			createVirtualDirectory.AppPoolManagedPipelineMode = this.AppPoolManagedPipelineMode;
			createVirtualDirectory.CustomizedVDirProperties = this.CustomizedVDirProperties;
			if (!string.IsNullOrEmpty(createVirtualDirectory.ApplicationPool) && this.LimitMaximumMemory)
			{
				ulong totalPhysicalMemory = new ComputerInfo().TotalPhysicalMemory;
				createVirtualDirectory.MaximumPrivateMemory = (long)(totalPhysicalMemory / 100UL * 80UL / 1024UL);
			}
			createVirtualDirectory.Initialize();
			createVirtualDirectory.Execute();
			result = true;
			string parent = string.Format("{0}/{1}", text, text2);
			ListDictionary childVirtualDirectories = this.ChildVirtualDirectories;
			if (childVirtualDirectories != null)
			{
				foreach (object obj in childVirtualDirectories.Keys)
				{
					string text3 = (string)obj;
					CreateVirtualDirectory createVirtualDirectory2 = new CreateVirtualDirectory();
					createVirtualDirectory2.Name = text3;
					createVirtualDirectory2.Parent = parent;
					createVirtualDirectory2.CustomizedVDirProperties = (ICollection)childVirtualDirectories[text3];
					createVirtualDirectory2.LocalPath = (string)IisUtility.GetIisPropertyValue("Path", (ICollection)childVirtualDirectories[text3]);
					createVirtualDirectory2.Initialize();
					createVirtualDirectory2.Execute();
				}
			}
			return result;
		}

		protected virtual void DeleteFromMetabase()
		{
			T dataObject = this.DataObject;
			string webSiteRoot = IisUtility.GetWebSiteRoot(dataObject.MetabasePath);
			ICollection childVirtualDirectoryNames = null;
			ListDictionary childVirtualDirectories = this.ChildVirtualDirectories;
			if (childVirtualDirectories != null)
			{
				childVirtualDirectoryNames = childVirtualDirectories.Keys;
			}
			DeleteVirtualDirectory.DeleteFromMetabase(webSiteRoot, base.Name, childVirtualDirectoryNames);
		}

		protected virtual bool VerifyRoleConsistency()
		{
			bool result = true;
			if (this.Role == VirtualDirectoryRole.ClientAccess && !base.OwningServer.IsCafeServer)
			{
				result = false;
				base.WriteError(new ArgumentException("Argument: -Role ClientAccess"), ErrorCategory.InvalidArgument, null);
			}
			if (this.Role == VirtualDirectoryRole.Mailbox)
			{
				if (!base.OwningServer.IsClientAccessServer)
				{
					result = false;
					base.WriteError(new ArgumentException("Argument: -Role Mailbox"), ErrorCategory.InvalidArgument, null);
				}
				if (!string.IsNullOrEmpty(this.WebSiteName) && !this.WebSiteName.Equals("Exchange Back End", StringComparison.OrdinalIgnoreCase))
				{
					result = false;
					base.WriteError(new ArgumentException("Argument: -WebsiteName"), ErrorCategory.InvalidArgument, null);
				}
			}
			return result;
		}

		protected override bool ShouldCreateVirtualDirectory()
		{
			return base.ShouldCreateVirtualDirectory() && this.VerifyRoleConsistency();
		}

		protected string RetrieveVDirAppRootValue(string parent, string vdirName)
		{
			TaskLogger.LogEnter();
			string result = null;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(parent))
				{
					string text = (string)directoryEntry.Properties["AppRoot"].Value;
					if (text != null)
					{
						result = text + "/" + vdirName;
					}
				}
			}
			catch (COMException innerException)
			{
				throw new TaskException(Strings.ExceptionVirtualServerNotExists, innerException);
			}
			TaskLogger.LogExit();
			return result;
		}

		protected string RetrieveRemoteInstallPath(string remoteServer)
		{
			TaskLogger.LogEnter();
			string result;
			try
			{
				using (RegistryKey registryKey = RegistryUtil.OpenRemoteBaseKey(RegistryHive.LocalMachine, remoteServer))
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
					{
						result = (string)registryKey2.GetValue("MsiInstallPath");
					}
				}
			}
			catch (IOException innerException)
			{
				base.WriteError(new NoRemoteInstallPathException(Strings.NoRemoteInstallPath(remoteServer), innerException), ErrorCategory.ReadError, this);
				return null;
			}
			TaskLogger.LogExit();
			return result;
		}

		private void UpdateRequestAndThreadsLimits()
		{
			TaskLogger.LogEnter();
			using (ServerManager serverManager = new ServerManager())
			{
				Configuration applicationHostConfiguration = serverManager.GetApplicationHostConfiguration();
				ConfigurationSection section = applicationHostConfiguration.GetSection("system.webServer/serverRuntime");
				ConfigurationAttribute attribute = section.GetAttribute("appConcurrentRequestLimit");
				if (attribute != null && attribute.Schema != null && (long)attribute.Value == (long)((ulong)((uint)attribute.Schema.DefaultValue)))
				{
					attribute.Value = 50000;
					TaskLogger.Trace("Update applicationHost.config setting appConcurrentRequestLimit to {0}.", new object[]
					{
						attribute.Value
					});
				}
				TaskLogger.Trace("Unlock system.webServer/serverRuntime for all web sites", new object[0]);
				section.OverrideMode = 2;
				serverManager.CommitChanges();
			}
			Configuration configuration = WebConfigurationManager.OpenMachineConfiguration();
			ProcessModelSection processModelSection = (ProcessModelSection)configuration.GetSection("system.web/processModel");
			bool flag = false;
			if (processModelSection.RequestQueueLimit == 5000)
			{
				processModelSection.RequestQueueLimit = 25000;
				flag = true;
				TaskLogger.Trace("Update machine.config setting requestQueueLimit to {0}.", new object[]
				{
					processModelSection.RequestQueueLimit
				});
			}
			if (processModelSection.AutoConfig)
			{
				processModelSection.AutoConfig = false;
				flag = true;
				TaskLogger.Trace("Update machine.config autoconfig to false.", new object[0]);
			}
			if (processModelSection.MinWorkerThreads < 9)
			{
				processModelSection.MinWorkerThreads = 9;
				flag = true;
				TaskLogger.Trace("Update machine.config minWorkerThreads to {0}", new object[]
				{
					processModelSection.MinWorkerThreads
				});
			}
			if (flag)
			{
				configuration.Save();
			}
			TaskLogger.LogExit();
		}

		protected const string defaultWebSiteRootPath = "/W3SVC/1/ROOT";

		private const uint maximumMemory = 80U;

		private VirtualDirectoryRole role;

		private MetabasePropertyTypes.ManagedPipelineMode appPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Classic;
	}
}
