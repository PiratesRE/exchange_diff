using System;
using System.Collections;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "OwaVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewOwaVirtualDirectory : NewWebAppVirtualDirectory<ADOwaVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOwaVirtualDirectory(base.WebSiteName.ToString(), base.Server.ToString());
			}
		}

		public NewOwaVirtualDirectory()
		{
			base.Fields["Name"] = string.Empty;
			base.AppPoolId = "MSExchangeOWAAppPool";
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
		}

		internal new string Name
		{
			get
			{
				return base.Name;
			}
		}

		private string DomainName
		{
			get
			{
				return null;
			}
		}

		private MultiValuedProperty<string> WebReadyFileTypes
		{
			get
			{
				if (this.webReadyFileTypes == null)
				{
					this.webReadyFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultWebReadyFileTypes);
				}
				return this.webReadyFileTypes;
			}
		}

		private MultiValuedProperty<string> WebReadyMimeTypes
		{
			get
			{
				if (this.webReadyMimeTypes == null)
				{
					this.webReadyMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultWebReadyMimeTypes);
				}
				return this.webReadyMimeTypes;
			}
		}

		private MultiValuedProperty<string> AllowedFileTypes
		{
			get
			{
				if (this.allowedFileTypes == null)
				{
					this.allowedFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultAllowedFileTypes);
				}
				return this.allowedFileTypes;
			}
		}

		private MultiValuedProperty<string> AllowedMimeTypes
		{
			get
			{
				if (this.allowedMimeTypes == null)
				{
					this.allowedMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultAllowedMimeTypes);
				}
				return this.allowedMimeTypes;
			}
		}

		private MultiValuedProperty<string> ForceSaveFileTypes
		{
			get
			{
				if (this.forceSaveFileTypes == null)
				{
					this.forceSaveFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultForceSaveFileTypes);
				}
				return this.forceSaveFileTypes;
			}
		}

		private MultiValuedProperty<string> ForceSaveMimeTypes
		{
			get
			{
				if (this.forceSaveMimeTypes == null)
				{
					this.forceSaveMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultForceSaveMimeTypes);
				}
				return this.forceSaveMimeTypes;
			}
		}

		private MultiValuedProperty<string> BlockedFileTypes
		{
			get
			{
				if (this.blockedFileTypes == null)
				{
					this.blockedFileTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultBlockedFileTypes);
				}
				return this.blockedFileTypes;
			}
		}

		private MultiValuedProperty<string> BlockedMimeTypes
		{
			get
			{
				if (this.blockedMimeTypes == null)
				{
					this.blockedMimeTypes = new MultiValuedProperty<string>(OwaMailboxPolicySchema.DefaultBlockedMimeTypes);
				}
				return this.blockedMimeTypes;
			}
		}

		protected override bool FailOnVirtualDirectoryAlreadyExists()
		{
			return true;
		}

		protected override ArrayList CustomizedVDirProperties
		{
			get
			{
				return new ArrayList
				{
					new MetabaseProperty("DefaultDoc", "default.aspx"),
					new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script),
					new MetabaseProperty("CacheControlCustom", "public"),
					new MetabaseProperty("HttpExpires", "D, 0x278d00"),
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
				this.AddAuthVDir(listDictionary);
				if (base.Role == VirtualDirectoryRole.Mailbox)
				{
					base.AddBinVDir(listDictionary);
					this.AddSMimeVDir(listDictionary);
					OwaVirtualDirectoryHelper.AddVersionVDir(listDictionary);
				}
				return listDictionary;
			}
		}

		protected override void DeleteFromMetabase()
		{
			base.DeleteFromMetabase();
			string webSiteRoot = IisUtility.GetWebSiteRoot(this.DataObject.MetabasePath);
			IList createdLegacyVDirs = OwaVirtualDirectoryHelper.CreatedLegacyVDirs;
			if (createdLegacyVDirs != null)
			{
				foreach (object obj in createdLegacyVDirs)
				{
					string name = (string)obj;
					if (IisUtility.WebDirObjectExists(webSiteRoot, name))
					{
						IisUtility.DeleteWebDirObject(webSiteRoot, name);
					}
				}
				OwaVirtualDirectoryHelper.CreatedLegacyVDirs.Clear();
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
				base.Path = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, (base.Role == VirtualDirectoryRole.ClientAccess) ? "FrontEnd\\HttpProxy\\owa" : "ClientAccess\\owa");
			}
			if (this.Name != string.Empty && !this.Name.Equals("owa"))
			{
				this.WriteWarning(Strings.OwaNameParameterIgnored);
			}
			base.Fields["Name"] = "owa";
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (!new VirtualDirectoryPathExistsCondition(base.OwningServer.Fqdn, base.Path).Verify())
			{
				base.WriteError(new ArgumentException(Strings.ErrorPathNotExistsOnServer(base.Path, base.OwningServer.Name), "Path"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				return;
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
			OwaVirtualDirectoryHelper.CreateOwaCalendarVDir(this.DataObject.MetabasePath, base.Role);
			if (base.Role == VirtualDirectoryRole.Mailbox)
			{
				OwaVirtualDirectoryHelper.CreateLegacyVDirs(this.DataObject.MetabasePath, false);
				try
				{
					OwaVirtualDirectoryHelper.EnableIsapiFilter(this.DataObject, false);
					goto IL_EB;
				}
				catch (Exception ex)
				{
					TaskLogger.Trace("Exception occurred in EnableIsapiFilter(): {0}", new object[]
					{
						ex.Message
					});
					this.WriteWarning(Strings.OwaMetabaseIsapiInstallFailure);
					throw;
				}
			}
			if (!Datacenter.IsMultiTenancyEnabled())
			{
				OwaVirtualDirectoryHelper.CreateOwaIntegratedVDir(this.DataObject.MetabasePath, base.Role);
				this.DataObject.IntegratedFeaturesEnabled = new bool?(true);
			}
			OwaVirtualDirectoryHelper.CreateOmaVDir(this.DataObject.MetabasePath, base.Role);
			try
			{
				OwaVirtualDirectoryHelper.EnableIsapiFilter(this.DataObject, true);
			}
			catch (Exception ex2)
			{
				TaskLogger.Trace("Exception occurred in EnableIsapiFilterForCafe(): {0}", new object[]
				{
					ex2.Message
				});
				this.WriteWarning(Strings.OwaMetabaseIsapiInstallFailure);
				throw;
			}
			try
			{
				IL_EB:
				WebAppVirtualDirectoryHelper.UpdateMetabase(this.DataObject, this.DataObject.MetabasePath, base.Role == VirtualDirectoryRole.Mailbox);
			}
			catch (Exception ex3)
			{
				TaskLogger.Trace("Exception occurred in UpdateMetabase(): {0}", new object[]
				{
					ex3.Message
				});
				this.WriteWarning(Strings.OwaMetabaseGetPropertiesFailure);
				throw;
			}
			if (base.Role == VirtualDirectoryRole.Mailbox && Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
			{
				try
				{
					Gzip.SetIisGzipMimeTypes();
				}
				catch (Exception ex4)
				{
					TaskLogger.Trace("Exception occurred in SetIisGzipMimeTypes(): {0}", new object[]
					{
						ex4.Message
					});
					this.WriteWarning(Strings.SetIISGzipMimeTypesFailure);
					throw;
				}
			}
		}

		protected override void InternalProcessComplete()
		{
			this.SetAttachmentPolicy();
			if (base.Role == VirtualDirectoryRole.Mailbox)
			{
				ExchangeServiceVDirHelper.EwsAutodiscMWA.OnNewManageWCFEndpoints(this, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol.OwaEws, new bool?(this.DataObject.BasicAuthentication), new bool?(this.DataObject.WindowsAuthentication), false, false, this.DataObject, base.Role);
			}
		}

		protected override void WriteResultMetabaseFixup(ExchangeVirtualDirectory targetDataObject)
		{
			base.WriteResultMetabaseFixup(targetDataObject);
			if (WebAppVirtualDirectoryHelper.FindWebAppVirtualDirectoryInSameWebSite<ADEcpVirtualDirectory>((ExchangeWebAppVirtualDirectory)targetDataObject, base.DataSession) == null)
			{
				this.WriteWarning(Strings.CreateEcpForOwaWarning);
			}
		}

		private void SetAttachmentPolicy()
		{
			this.DataObject.WebReadyFileTypes = this.WebReadyFileTypes;
			this.DataObject.WebReadyMimeTypes = this.WebReadyMimeTypes;
			this.DataObject.AllowedFileTypes = this.AllowedFileTypes;
			this.DataObject.AllowedMimeTypes = this.AllowedMimeTypes;
			this.DataObject.ForceSaveFileTypes = this.ForceSaveFileTypes;
			this.DataObject.ForceSaveMimeTypes = this.ForceSaveMimeTypes;
			this.DataObject.BlockedFileTypes = this.BlockedFileTypes;
			this.DataObject.BlockedMimeTypes = this.BlockedMimeTypes;
			base.DataSession.Save(this.DataObject);
		}

		private void SetDefaults()
		{
			this.DataObject.DefaultClientLanguage = new int?((int)OwaMailboxPolicySchema.DefaultClientLanguage.DefaultValue);
			this.DataObject.OutboundCharset = new OutboundCharsetOptions?((OutboundCharsetOptions)OwaMailboxPolicySchema.OutboundCharset.DefaultValue);
			this.DataObject.OwaVersion = (OwaVersions)ADOwaVirtualDirectorySchema.OwaVersion.DefaultValue;
			this.DataObject.LogonFormat = LogonFormats.FullDomain;
			this.DataObject.ClientAuthCleanupLevel = ClientAuthCleanupLevels.High;
			this.DataObject.LogonAndErrorLanguage = (int)OwaMailboxPolicySchema.LogonAndErrorLanguage.DefaultValue;
			this.DataObject.RemoteDocumentsActionForUnknownServers = new RemoteDocumentsActions?(RemoteDocumentsActions.Block);
			this.DataObject.ActionForUnknownFileAndMIMETypes = new AttachmentBlockingActions?(AttachmentBlockingActions.Allow);
			this.DataObject.FilterWebBeaconsAndHtmlForms = new WebBeaconFilterLevels?(WebBeaconFilterLevels.UserFilterChoice);
			this.DataObject.NotificationInterval = new int?(120);
			this.DataObject.UserContextTimeout = new int?(60);
			this.DataObject.RedirectToOptimalOWAServer = new bool?(true);
			this.DataObject.UseGB18030 = new bool?(Convert.ToBoolean((int)OwaMailboxPolicySchema.UseGB18030.DefaultValue));
			this.DataObject.UseISO885915 = new bool?(Convert.ToBoolean((int)OwaMailboxPolicySchema.UseISO885915.DefaultValue));
			this.DataObject.InstantMessagingType = new InstantMessagingTypeOptions?((InstantMessagingTypeOptions)OwaMailboxPolicySchema.InstantMessagingType.DefaultValue);
			this.DataObject[ADOwaVirtualDirectorySchema.ADMailboxFolderSet] = (int)OwaMailboxPolicySchema.ADMailboxFolderSet.DefaultValue;
			this.DataObject[ADOwaVirtualDirectorySchema.ADMailboxFolderSet2] = (int)OwaMailboxPolicySchema.ADMailboxFolderSet2.DefaultValue;
			this.DataObject[ADOwaVirtualDirectorySchema.FileAccessControlOnPublicComputers] = (int)OwaMailboxPolicySchema.FileAccessControlOnPublicComputers.DefaultValue;
			this.DataObject[ADOwaVirtualDirectorySchema.FileAccessControlOnPrivateComputers] = (int)OwaMailboxPolicySchema.FileAccessControlOnPrivateComputers.DefaultValue;
			this.DataObject.DefaultDomain = "";
			this.DataObject.GzipLevel = GzipLevel.High;
			this.DataObject.FormsAuthentication = (base.Role == VirtualDirectoryRole.ClientAccess);
			this.DataObject.BasicAuthentication = (base.Role == VirtualDirectoryRole.ClientAccess);
			this.DataObject.DigestAuthentication = false;
			this.DataObject.WindowsAuthentication = (base.Role == VirtualDirectoryRole.Mailbox);
			this.DataObject.LiveIdAuthentication = false;
			this.DataObject.ExternalAuthenticationMethods = new MultiValuedProperty<AuthenticationMethod>(AuthenticationMethod.Fba);
		}

		private void AddAuthVDir(ListDictionary childVDirs)
		{
			TaskLogger.LogEnter();
			childVDirs.Add("auth", new ArrayList
			{
				new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Execute | MetabasePropertyTypes.AccessFlags.Script),
				new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Anonymous),
				new MetabaseProperty("LogonMethod", MetabasePropertyTypes.LogonMethod.ClearTextLogon),
				new MetabaseProperty("HttpExpires", "D, 0x278d00")
			});
			TaskLogger.LogExit();
		}

		private void AddSMimeVDir(ListDictionary childVDirs)
		{
			base.AddChildVDir(childVDirs, "smime", new MetabaseProperty[]
			{
				new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Read)
			});
		}

		private string LDAPPrefix()
		{
			return NewOwaVirtualDirectory.LDAPPrefix(base.DomainController);
		}

		private static string LDAPPrefix(string DomainController)
		{
			if (!string.IsNullOrEmpty(DomainController))
			{
				return "LDAP://" + DomainController + "/";
			}
			return "LDAP://";
		}

		private string GetFullyQualifiedDomainName()
		{
			return NewOwaVirtualDirectory.GetFullyQualifiedDomainName(base.DomainController);
		}

		internal static string GetFullyQualifiedDomainName(string DomainController)
		{
			TaskLogger.LogEnter();
			DirectoryEntry directoryEntry = null;
			DirectoryEntry directoryEntry2 = null;
			DirectorySearcher directorySearcher = null;
			SearchResultCollection searchResultCollection = null;
			int num;
			try
			{
				string path = NewOwaVirtualDirectory.LDAPPrefix(DomainController) + "RootDSE";
				directoryEntry = new DirectoryEntry(path);
				directoryEntry2 = new DirectoryEntry(NewOwaVirtualDirectory.LDAPPrefix(DomainController) + directoryEntry.Properties["configurationNamingContext"].Value);
				directorySearcher = new DirectorySearcher(directoryEntry2);
				directorySearcher.Filter = "(&(objectClass=msExchRecipientPolicy)(msExchPolicyOrder=2147483647))";
				directorySearcher.PropertiesToLoad.Add("gatewayProxy");
				directorySearcher.SearchScope = SearchScope.Subtree;
				searchResultCollection = directorySearcher.FindAll();
				foreach (object obj in searchResultCollection)
				{
					SearchResult searchResult = (SearchResult)obj;
					ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["gatewayProxy"];
					foreach (object obj2 in resultPropertyValueCollection)
					{
						string text = obj2.ToString();
						if (text.StartsWith("SMTP:"))
						{
							num = text.IndexOf('@');
							if (num >= 0)
							{
								TaskLogger.LogExit();
								return text.Substring(num + 1);
							}
						}
					}
				}
			}
			catch (COMException ex)
			{
				throw new IISGeneralCOMException(ex.Message, ex.ErrorCode, ex);
			}
			finally
			{
				if (searchResultCollection != null)
				{
					searchResultCollection.Dispose();
				}
				if (directorySearcher != null)
				{
					directorySearcher.Dispose();
				}
				if (directoryEntry2 != null)
				{
					directoryEntry2.Dispose();
				}
				if (directoryEntry != null)
				{
					directoryEntry.Dispose();
				}
			}
			TaskLogger.LogExit();
			string hostName = Dns.GetHostName();
			IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
			string hostName2 = hostEntry.HostName;
			num = hostName2.IndexOf('.');
			return (num >= 0 && num < hostName2.Length - 1) ? hostName2.Substring(num + 1) : hostName2;
		}

		private const string defaultE12Name = "owa";

		private const string backendStorageUncPath = "\\\\.\\BackOfficeStorage\\";

		private const string LdapProtocol = "LDAP://";

		private MultiValuedProperty<string> webReadyFileTypes;

		private MultiValuedProperty<string> webReadyMimeTypes;

		private MultiValuedProperty<string> allowedFileTypes;

		private MultiValuedProperty<string> allowedMimeTypes;

		private MultiValuedProperty<string> forceSaveFileTypes;

		private MultiValuedProperty<string> forceSaveMimeTypes;

		private MultiValuedProperty<string> blockedFileTypes;

		private MultiValuedProperty<string> blockedMimeTypes;
	}
}
