using System;
using System.Collections;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "WebServicesVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewWebServicesVirtualDirectory : NewExchangeServiceVirtualDirectory<ADWebServicesVirtualDirectory>
	{
		[Parameter]
		public Uri InternalNLBBypassUrl
		{
			get
			{
				return this.DataObject.InternalNLBBypassUrl;
			}
			set
			{
				this.DataObject.InternalNLBBypassUrl = value;
			}
		}

		[Parameter]
		public GzipLevel GzipLevel
		{
			get
			{
				return this.DataObject.GzipLevel;
			}
			set
			{
				this.DataObject.GzipLevel = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string AppPoolIdForManagement
		{
			get
			{
				return (string)base.Fields["AppPoolIdForManagement"];
			}
			set
			{
				base.Fields["AppPoolIdForManagement"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		[Parameter(Mandatory = false)]
		public bool WSSecurityAuthentication
		{
			get
			{
				return base.Fields["WSSecurityAuthentication"] != null && (bool)base.Fields["WSSecurityAuthentication"];
			}
			set
			{
				base.Fields["WSSecurityAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OAuthAuthentication
		{
			get
			{
				return base.Fields["OAuthAuthentication"] != null && (bool)base.Fields["OAuthAuthentication"];
			}
			set
			{
				base.Fields["OAuthAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MRSProxyEnabled
		{
			get
			{
				return this.DataObject.MRSProxyEnabled;
			}
			set
			{
				this.DataObject.MRSProxyEnabled = value;
			}
		}

		protected override string VirtualDirectoryName
		{
			get
			{
				return "EWS";
			}
		}

		protected override string VirtualDirectoryPath
		{
			get
			{
				if (base.Role != VirtualDirectoryRole.ClientAccess)
				{
					return "ClientAccess\\exchweb\\EWS";
				}
				return "FrontEnd\\HttpProxy\\EWS";
			}
		}

		protected override string DefaultApplicationPoolId
		{
			get
			{
				return "MSExchangeServicesAppPool";
			}
		}

		protected override Uri DefaultInternalUrl
		{
			get
			{
				return new Uri(string.Format("https://{0}/EWS/Exchange.asmx", ComputerInformation.DnsFullyQualifiedDomainName));
			}
		}

		protected override void SetDefaultAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory)
		{
			virtualDirectory.WindowsAuthentication = new bool?(true);
			virtualDirectory.WSSecurityAuthentication = new bool?(true);
			virtualDirectory.OAuthAuthentication = new bool?(base.Role == VirtualDirectoryRole.ClientAccess);
			virtualDirectory.BasicAuthentication = new bool?(false);
			virtualDirectory.DigestAuthentication = new bool?(false);
			virtualDirectory.LiveIdBasicAuthentication = new bool?(false);
			virtualDirectory.LiveIdNegotiateAuthentication = new bool?(false);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewWebServicesVirtualDirectory(base.Server.ToString());
			}
		}

		protected override ListDictionary ChildVirtualDirectories
		{
			get
			{
				ListDictionary listDictionary = new ListDictionary();
				if (base.Role == VirtualDirectoryRole.Mailbox && Directory.Exists(System.IO.Path.Combine(base.Path, "bin")))
				{
					listDictionary.Add("bin", new ArrayList
					{
						new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.NoAccess),
						new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.NoneSet),
						new MetabaseProperty("AppPoolId", base.AppPoolId)
					});
				}
				return listDictionary;
			}
		}

		protected override void InternalValidate()
		{
			if (!base.Fields.IsModified("Path"))
			{
				base.Path = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, this.VirtualDirectoryPath);
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			base.InternalValidateBasicLiveIdBasic();
			if (string.Empty.Equals(this.AppPoolIdForManagement))
			{
				base.WriteError(new ArgumentException(Strings.ErrorAppPoolIdCannotBeEmpty, "AppPoolIdForManagement"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				string path = System.IO.Path.Combine(base.Path, "Nego2");
				if (!Directory.Exists(path))
				{
					base.WriteError(new ArgumentException(Strings.ErrorDirectoryManagementWebServiceNotFound(path), "Path"), ErrorCategory.InvalidArgument, this.DataObject.Identity);
				}
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (base.Role == VirtualDirectoryRole.Mailbox && this.InternalNLBBypassUrl == null)
			{
				this.InternalNLBBypassUrl = new Uri(string.Format("https://{0}:444/EWS/Exchange.asmx", ComputerInformation.DnsFullyQualifiedDomainName));
			}
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
		}

		protected override bool InternalProcessStartWork()
		{
			this.DataObject.GzipLevel = GzipLevel.High;
			return true;
		}

		protected override void InternalProcessMetabase()
		{
			base.InternalProcessMetabase();
			if (Environment.OSVersion.Version.Major < 6)
			{
				return;
			}
			string path = System.IO.Path.Combine(Environment.GetEnvironmentVariable("windir"), "system32");
			string text = System.IO.Path.Combine(path, "inetsrv");
			string text2 = System.IO.Path.Combine(text, "appcmd.exe");
			if (!File.Exists(text2))
			{
				base.WriteError(new InvalidOperationException(Strings.StampExistingResponsePassThroughOnVirtualDirectoryFailure(this.DataObject.Server.Name, this.DataObject.MetabasePath, 0, "appcmd.exe doesn't exist in the given path " + text2)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			string text3 = "set config \"" + base.WebSiteName + "/EWS/\" -section:system.webServer/httpErrors -existingResponse:PassThrough -commit:apphost";
			TaskLogger.Trace("Stamping the virtual directory {0} with existingResponse = PassThrough", new object[]
			{
				this.DataObject.MetabasePath
			});
			TaskLogger.Trace("Invoking appcmd with command line: {0}{1}", new object[]
			{
				text2,
				text3
			});
			string text4;
			string text5;
			int num = ProcessRunner.Run(text2, text3, -1, text, out text4, out text5);
			TaskLogger.Trace("The return value from appcmd: {0}", new object[]
			{
				num
			});
			TaskLogger.Trace("appcmd output: {0}", new object[]
			{
				text4
			});
			TaskLogger.Trace("appcmd errors: {0}", new object[]
			{
				text5
			});
			TaskLogger.Trace("Finished running appcmd command", new object[0]);
			if (num != 0)
			{
				base.WriteError(new InvalidOperationException(Strings.StampExistingResponsePassThroughOnVirtualDirectoryFailure(this.DataObject.Server.Name, this.DataObject.MetabasePath, num, text5)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			this.UpdateCompressionSettings();
		}

		private void UpdateCompressionSettings()
		{
			if (this.GzipLevel == GzipLevel.Error)
			{
				base.WriteError(new TaskException(Strings.GzipCannotBeSetToError), ErrorCategory.NotSpecified, null);
				return;
			}
			if (this.GzipLevel == GzipLevel.Low)
			{
				this.WriteWarning(Strings.GzipLowDoesNotUseDynamicCompression);
			}
			string metabasePath = this.DataObject.MetabasePath;
			Gzip.SetIisGzipLevel(IisUtility.WebSiteFromMetabasePath(metabasePath), GzipLevel.High);
			Gzip.SetVirtualDirectoryGzipLevel(metabasePath, this.DataObject.GzipLevel);
			if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
			{
				try
				{
					Gzip.SetIisGzipMimeTypes();
				}
				catch (Exception ex)
				{
					TaskLogger.Trace("Exception occurred in SetIisGzipMimeTypes(): {0}", new object[]
					{
						ex.Message
					});
					this.WriteWarning(Strings.SetIISGzipMimeTypesFailure);
					throw;
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.Force)
			{
				foreach (ADPropertyDefinition adpropertyDefinition in NewWebServicesVirtualDirectory.HostProperties)
				{
					if (!NewWebServicesVirtualDirectory.IsValidHost(this.DataObject, adpropertyDefinition) && !base.ShouldContinue(Strings.ConfirmationMessageHostCannotBeResolved(adpropertyDefinition.Name)))
					{
						TaskLogger.LogExit();
						return;
					}
				}
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessComplete()
		{
			base.InternalProcessComplete();
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				DirectoryEntry directoryEntry = IisUtility.CreateWebDirObject(this.DataObject.MetabasePath, null, "Nego2");
				IisUtility.SetProperty(directoryEntry, "AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script, true);
				directoryEntry.CommitChanges();
				string nego2Path = string.Format("{0}/{1}", this.DataObject.MetabasePath, "Nego2");
				ExchangeServiceVDirHelper.SetSplitVirtualDirectoryAuthenticationMethods(this.DataObject, nego2Path, new Task.TaskErrorLoggingDelegate(base.WriteError), this.MetabaseSetPropertiesFailureMessage);
			}
			else if (base.Role == VirtualDirectoryRole.ClientAccess)
			{
				ExchangeServiceVDirHelper.CheckAndUpdateLocalhostWebBindingsIfNecessary(this.DataObject);
			}
			ExchangeServiceVDirHelper.EwsAutodiscMWA.OnNewManageWCFEndpoints(this, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol.Ews, this.DataObject.BasicAuthentication, this.DataObject.WindowsAuthentication, this.DataObject.WSSecurityAuthentication ?? false, this.DataObject.OAuthAuthentication ?? false, this.DataObject, base.Role);
		}

		protected override IConfigurable PrepareDataObject()
		{
			if (!base.Fields.Contains("ExtendedProtectionTokenChecking"))
			{
				base.Fields["ExtendedProtectionTokenChecking"] = ExtendedProtectionTokenCheckingMode.None;
			}
			if (!base.Fields.Contains("ExtendedProtectionSPNList"))
			{
				base.Fields["ExtendedProtectionSPNList"] = null;
			}
			if (!base.Fields.Contains("ExtendedProtectionFlags"))
			{
				base.Fields["ExtendedProtectionFlags"] = ExtendedProtectionFlag.None;
			}
			ADWebServicesVirtualDirectory adwebServicesVirtualDirectory = (ADWebServicesVirtualDirectory)base.PrepareDataObject();
			if (base.Fields["WSSecurityAuthentication"] != null)
			{
				adwebServicesVirtualDirectory.WSSecurityAuthentication = new bool?(this.WSSecurityAuthentication);
			}
			if (base.Fields["OAuthAuthentication"] != null)
			{
				adwebServicesVirtualDirectory.OAuthAuthentication = new bool?(this.OAuthAuthentication);
			}
			return adwebServicesVirtualDirectory;
		}

		protected override void WriteResultMetabaseFixup(ExchangeVirtualDirectory dataObject)
		{
			ADWebServicesVirtualDirectory adwebServicesVirtualDirectory = (ADWebServicesVirtualDirectory)dataObject;
			adwebServicesVirtualDirectory.CertificateAuthentication = new bool?(true);
			adwebServicesVirtualDirectory.ResetChangeTracking();
		}

		internal static bool IsValidHost(ADWebServicesVirtualDirectory dataObject, ADPropertyDefinition property)
		{
			if (dataObject.IsChanged(property))
			{
				Uri uri = dataObject.propertyBag[property] as Uri;
				try
				{
					if (uri != null)
					{
						Dns.GetHostEntry(uri.DnsSafeHost);
					}
				}
				catch (SocketException)
				{
					return false;
				}
				catch (ArgumentOutOfRangeException)
				{
					return false;
				}
				return true;
			}
			return true;
		}

		private const string EWSVDirName = "EWS";

		private const string EWSVDirPath = "ClientAccess\\exchweb\\EWS";

		private const string EWSCafeVDirPath = "FrontEnd\\HttpProxy\\EWS";

		private const string EWSDefaultAppPoolId = "MSExchangeServicesAppPool";

		private const string BinFolderName = "bin";

		internal static ADPropertyDefinition[] HostProperties = new ADPropertyDefinition[]
		{
			ADWebServicesVirtualDirectorySchema.InternalNLBBypassUrl,
			ADVirtualDirectorySchema.InternalUrl,
			ADVirtualDirectorySchema.ExternalUrl
		};
	}
}
