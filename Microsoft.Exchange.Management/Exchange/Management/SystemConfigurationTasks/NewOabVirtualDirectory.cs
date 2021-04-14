using System;
using System.Collections;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using System.Runtime.InteropServices;
using IISOle;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "OabVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewOabVirtualDirectory : NewExchangeVirtualDirectory<ADOabVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOabVirtualDirectory(base.WebSiteName.ToString(), base.Server.ToString());
			}
		}

		public NewOabVirtualDirectory()
		{
			this.Name = "OAB";
			base.AppPoolId = "MSExchangeOABAppPool";
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
			this.RequireSSL = true;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(IISNotInstalledException).IsInstanceOfType(exception);
		}

		internal new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		internal new string ApplicationRoot
		{
			get
			{
				return base.ApplicationRoot;
			}
			set
			{
				base.ApplicationRoot = value;
			}
		}

		internal new MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return base.InternalAuthenticationMethods;
			}
			set
			{
				base.InternalAuthenticationMethods = value;
			}
		}

		internal new MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			get
			{
				return base.ExternalAuthenticationMethods;
			}
			set
			{
				base.ExternalAuthenticationMethods = value;
			}
		}

		[Parameter]
		public int PollInterval
		{
			get
			{
				return this.DataObject.PollInterval;
			}
			set
			{
				this.DataObject.PollInterval = value;
			}
		}

		[Parameter]
		public bool RequireSSL
		{
			get
			{
				return (bool)base.Fields["RequireSSL"];
			}
			set
			{
				base.Fields["RequireSSL"] = value;
			}
		}

		[Parameter]
		public SwitchParameter Recovery
		{
			get
			{
				return (SwitchParameter)(base.Fields["Recovery"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Recovery"] = value;
			}
		}

		internal override MetabasePropertyTypes.AppPoolIdentityType AppPoolIdentityType
		{
			get
			{
				return MetabasePropertyTypes.AppPoolIdentityType.LocalSystem;
			}
		}

		protected override ArrayList CustomizedVDirProperties
		{
			get
			{
				ArrayList customizedVDirProperties = base.CustomizedVDirProperties;
				customizedVDirProperties.Add(new MetabaseProperty("MimeMap", new MimeMapClass
				{
					Extension = ".lzx",
					MimeType = "application/octet-stream"
				}));
				return customizedVDirProperties;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADOabVirtualDirectory adoabVirtualDirectory = (ADOabVirtualDirectory)base.PrepareDataObject();
			adoabVirtualDirectory.RequireSSL = this.RequireSSL;
			this.serverFQDN = base.OwningServer.Fqdn;
			string text;
			if (string.IsNullOrEmpty(base.WebSiteName))
			{
				if (base.Role == VirtualDirectoryRole.ClientAccess)
				{
					text = IisUtility.GetWebSiteName(IisUtility.CreateAbsolutePath(IisUtility.AbsolutePathType.WebSiteRoot, this.serverFQDN, "/W3SVC/1/ROOT", null));
				}
				else
				{
					text = "Exchange Back End";
				}
			}
			else
			{
				text = base.WebSiteName;
			}
			ADOabVirtualDirectory[] array = ((IConfigurationSession)base.DataSession).Find<ADOabVirtualDirectory>(base.OwningServer.Id, QueryScope.SubTree, null, null, 0);
			if (array != null && array.Length != 0)
			{
				ADOabVirtualDirectory[] array2 = array;
				int i = 0;
				while (i < array2.Length)
				{
					ADOabVirtualDirectory adoabVirtualDirectory2 = array2[i];
					string webSiteRootPath = null;
					string text2 = null;
					try
					{
						IisUtility.ParseApplicationRootPath(adoabVirtualDirectory2.MetabasePath, ref webSiteRootPath, ref text2);
					}
					catch (IisUtilityInvalidApplicationRootPathException ex)
					{
						base.WriteWarning(ex.Message);
						goto IL_24E;
					}
					goto IL_D6;
					IL_24E:
					i++;
					continue;
					IL_D6:
					string webSiteName = IisUtility.GetWebSiteName(webSiteRootPath);
					if (string.Compare(webSiteName, text, false) == 0)
					{
						try
						{
							if (DirectoryEntry.Exists(adoabVirtualDirectory2.MetabasePath))
							{
								if (!this.Recovery)
								{
									base.WriteError(new InvalidOperationException(Strings.ErrorOabVirtualDirectoryAlreadyExists(adoabVirtualDirectory2.Identity.ToString(), text, this.serverFQDN)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
								}
								else
								{
									base.WriteError(new InvalidOperationException(Strings.ErrorOabVirtualDirectoryAlreadyExistsWithRecovery(adoabVirtualDirectory2.Identity.ToString(), text, this.serverFQDN)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
								}
							}
							else if (!this.Recovery)
							{
								base.WriteError(new InvalidOperationException(Strings.ErrorOabVirtualDirectoryADObjectAlreadyExists(adoabVirtualDirectory2.Identity.ToString(), text, this.serverFQDN)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
							}
							else
							{
								array[0].CopyChangesFrom(adoabVirtualDirectory);
								adoabVirtualDirectory = adoabVirtualDirectory2;
								adoabVirtualDirectory.SetId(new ADObjectId(adoabVirtualDirectory.Server.DistinguishedName).GetDescendantId("Protocols", "HTTP", new string[]
								{
									string.Format("{0} ({1})", this.Name, base.WebSiteName)
								}));
								adoabVirtualDirectory.MetabasePath = string.Format("IIS://{0}{1}/{2}", this.serverFQDN, IisUtility.FindWebSiteRootPath(base.WebSiteName, this.serverFQDN), this.Name);
							}
						}
						catch (COMException exception)
						{
							base.WriteError(exception, ErrorCategory.ReadError, null);
						}
						goto IL_24E;
					}
					goto IL_24E;
				}
			}
			if (new VirtualDirectoryExistsCondition(this.serverFQDN, text, this.Name).Verify())
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorOabVirtualDirectoryIISObjectAlreadyExists(string.Format("{0}\\{1}", text, this.Name), text, this.serverFQDN)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			this.isExistingFolder = true;
			this.isLocalServer = this.IsLocalServer();
			if (string.IsNullOrEmpty(base.Path))
			{
				base.Path = System.IO.Path.Combine(base.OwningServer.InstallPath.PathName, (base.Role == VirtualDirectoryRole.ClientAccess) ? "FrontEnd\\HttpProxy\\OAB" : "ClientAccess\\OAB");
			}
			else
			{
				LocalLongFullPath localLongFullPath = null;
				try
				{
					localLongFullPath = LocalLongFullPath.Parse(base.Path);
				}
				catch (ArgumentException ex2)
				{
					base.WriteError(new ArgumentException(new LocalizedString(ex2.Message.ToString()), "Path"), ErrorCategory.InvalidArgument, base.OwningServer.Identity);
					return null;
				}
				base.Path = localLongFullPath.PathName;
			}
			if (base.Role == VirtualDirectoryRole.ClientAccess && adoabVirtualDirectory.InternalUrl == null)
			{
				if (this.isLocalServer)
				{
					adoabVirtualDirectory.InternalUrl = new Uri(string.Format("http://{0}/{1}", ComputerInformation.DnsFullyQualifiedDomainName, "OAB"));
				}
				else
				{
					base.WriteError(new ArgumentException(Strings.ErrorMissingInternalUrlInRemoteScenario, "InternalUrl"), ErrorCategory.InvalidArgument, base.OwningServer.Identity);
				}
			}
			TaskLogger.LogExit();
			return adoabVirtualDirectory;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			try
			{
				if (!new PathOnFixedDriveCondition(this.serverFQDN, base.Path).Verify())
				{
					base.WriteError(new ArgumentException(Strings.ErrorOabVirtualDirctoryPathNotOnFixedDrive(base.Path), "Path"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				}
				if (!new VirtualDirectoryPathExistsCondition(this.serverFQDN, base.Path).Verify())
				{
					if (this.isLocalServer)
					{
						try
						{
							Directory.CreateDirectory(base.Path);
							goto IL_FA;
						}
						catch (UnauthorizedAccessException ex)
						{
							this.isExistingFolder = false;
							TaskLogger.Trace("The creation of directoy '{0}' failed, returned error: {1}.", new object[]
							{
								base.Path,
								ex.Message.ToString()
							});
							goto IL_FA;
						}
						catch (IOException ex2)
						{
							this.isExistingFolder = false;
							TaskLogger.Trace("The creation of directoy '{0}' failed, returned error: {1}.", new object[]
							{
								base.Path,
								ex2.Message.ToString()
							});
							goto IL_FA;
						}
					}
					this.isExistingFolder = false;
				}
				IL_FA:;
			}
			catch (WmiException ex3)
			{
				base.WriteError(new InvalidOperationException(new LocalizedString(ex3.Message)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		private bool IsLocalServer()
		{
			string localComputerFqdn = NativeHelpers.GetLocalComputerFqdn(false);
			int num = localComputerFqdn.IndexOf('.');
			string value = (num == -1) ? localComputerFqdn : localComputerFqdn.Substring(0, num);
			return base.OwningServer.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase);
		}

		internal static void UpdateMetabase(ADOabVirtualDirectory virtualDirectory, bool updateAuthenticationMethod, Task.TaskErrorLoggingDelegate handler)
		{
			try
			{
				DirectoryEntry directoryEntry2;
				DirectoryEntry directoryEntry = directoryEntry2 = IisUtility.CreateIISDirectoryEntry(virtualDirectory.MetabasePath);
				try
				{
					ArrayList arrayList = new ArrayList();
					int num = (int)(IisUtility.GetIisPropertyValue("AccessSSLFlags", IisUtility.GetProperties(directoryEntry)) ?? 0);
					if (virtualDirectory.RequireSSL)
					{
						num |= 8;
					}
					else
					{
						num &= -9;
						num &= -257;
						num &= -65;
					}
					arrayList.Add(new MetabaseProperty("AccessSSLFlags", num, true));
					if (updateAuthenticationMethod)
					{
						uint num2 = (uint)((int)(IisUtility.GetIisPropertyValue("AuthFlags", IisUtility.GetProperties(directoryEntry)) ?? 0));
						num2 |= 4U;
						num2 &= 4294967294U;
						arrayList.Add(new MetabaseProperty("AuthFlags", num2, true));
						MultiValuedProperty<AuthenticationMethod> multiValuedProperty = new MultiValuedProperty<AuthenticationMethod>();
						multiValuedProperty.Add(AuthenticationMethod.WindowsIntegrated);
						if (IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Basic))
						{
							multiValuedProperty.Add(AuthenticationMethod.Basic);
						}
						if (IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Digest))
						{
							multiValuedProperty.Add(AuthenticationMethod.Digest);
						}
						if (IisUtility.CheckForAuthenticationMethod(directoryEntry, AuthenticationMethodFlags.Fba))
						{
							multiValuedProperty.Add(AuthenticationMethod.Fba);
						}
						virtualDirectory.ExternalAuthenticationMethods = (virtualDirectory.InternalAuthenticationMethods = multiValuedProperty);
					}
					IisUtility.SetProperties(directoryEntry, arrayList);
					directoryEntry.CommitChanges();
					IisUtility.CommitMetabaseChanges((virtualDirectory.Server == null) ? null : virtualDirectory.Server.ToString());
				}
				finally
				{
					if (directoryEntry2 != null)
					{
						((IDisposable)directoryEntry2).Dispose();
					}
				}
			}
			catch (COMException exception)
			{
				handler(exception, ErrorCategory.InvalidOperation, virtualDirectory.Identity);
			}
		}

		protected override void InternalProcessMetabase()
		{
			TaskLogger.LogEnter();
			base.InternalProcessMetabase();
			NewOabVirtualDirectory.UpdateMetabase(this.DataObject, true, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessComplete()
		{
			TaskLogger.LogEnter();
			base.InternalProcessComplete();
			if (!this.isExistingFolder)
			{
				if (this.isLocalServer)
				{
					this.WriteWarning(Strings.FaildToCreateFolder(base.Path));
				}
				else
				{
					this.WriteWarning(Strings.FolderNotExistsOnRemoteServer(base.Path, this.serverFQDN));
				}
			}
			TaskLogger.LogExit();
		}

		protected override bool FailOnVirtualDirectoryADObjectAlreadyExists()
		{
			return this.Recovery == false;
		}

		protected override void WriteResultMetabaseFixup(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			base.WriteResultMetabaseFixup(dataObject);
			((ADOabVirtualDirectory)dataObject).RequireSSL = this.RequireSSL;
			dataObject.ResetChangeTracking();
			TaskLogger.LogExit();
		}

		private const string oabVirtualDirectoryName = "OAB";

		private const string oabVirtualDirectoryPath = "FrontEnd\\HttpProxy\\OAB";

		private const string oabBackEndVirtualDirectoryPath = "ClientAccess\\OAB";

		private const string defaultApplicationPool = "MSExchangeOABAppPool";

		private bool isExistingFolder;

		private bool isLocalServer;

		private string serverFQDN;
	}
}
