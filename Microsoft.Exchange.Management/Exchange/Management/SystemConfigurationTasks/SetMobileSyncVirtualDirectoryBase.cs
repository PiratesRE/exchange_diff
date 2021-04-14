using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.AirSync;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class SetMobileSyncVirtualDirectoryBase : SetExchangeVirtualDirectory<ADMobileVirtualDirectory>
	{
		[Parameter]
		public string ActiveSyncServer
		{
			get
			{
				return (string)base.Fields["ActiveSyncServer"];
			}
			set
			{
				base.Fields["ActiveSyncServer"] = value;
			}
		}

		[Parameter]
		public bool MobileClientCertificateProvisioningEnabled
		{
			get
			{
				return (bool)base.Fields["MobileClientCertificateProvisioningEnabled"];
			}
			set
			{
				base.Fields["MobileClientCertificateProvisioningEnabled"] = value;
			}
		}

		[Parameter]
		public bool BadItemReportingEnabled
		{
			get
			{
				return (bool)base.Fields["BadItemReportingEnabled"];
			}
			set
			{
				base.Fields["BadItemReportingEnabled"] = value;
			}
		}

		[Parameter]
		public bool SendWatsonReport
		{
			get
			{
				return (bool)base.Fields["SendWatsonReport"];
			}
			set
			{
				base.Fields["SendWatsonReport"] = value;
			}
		}

		[Parameter]
		public string MobileClientCertificateAuthorityURL
		{
			get
			{
				return (string)base.Fields["MobileClientCertificateAuthorityURL"];
			}
			set
			{
				base.Fields["MobileClientCertificateAuthorityURL"] = value;
			}
		}

		[Parameter]
		public string MobileClientCertTemplateName
		{
			get
			{
				return (string)base.Fields["MobileClientCertTemplateName"];
			}
			set
			{
				base.Fields["MobileClientCertTemplateName"] = value;
			}
		}

		[Parameter]
		public ClientCertAuthTypes ClientCertAuth
		{
			get
			{
				return (ClientCertAuthTypes)base.Fields["ClientCertAuth"];
			}
			set
			{
				base.Fields["ClientCertAuth"] = value;
			}
		}

		[Parameter]
		public bool BasicAuthEnabled
		{
			get
			{
				return (bool)base.Fields["BasicAuthEnabled"];
			}
			set
			{
				base.Fields["BasicAuthEnabled"] = value;
			}
		}

		[Parameter]
		public bool WindowsAuthEnabled
		{
			get
			{
				return (bool)base.Fields["WindowsAuthEnabled"];
			}
			set
			{
				base.Fields["WindowsAuthEnabled"] = value;
			}
		}

		[Parameter]
		public bool CompressionEnabled
		{
			get
			{
				return (bool)base.Fields["CompressionEnabled"];
			}
			set
			{
				base.Fields["CompressionEnabled"] = value;
			}
		}

		[Parameter]
		public RemoteDocumentsActions? RemoteDocumentsActionForUnknownServers
		{
			get
			{
				return (RemoteDocumentsActions?)base.Fields["RemoteDocumentsActionForUnknownServers"];
			}
			set
			{
				base.Fields["RemoteDocumentsActionForUnknownServers"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> RemoteDocumentsAllowedServers
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["RemoteDocumentsAllowedServers"];
			}
			set
			{
				base.Fields["RemoteDocumentsAllowedServers"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> RemoteDocumentsBlockedServers
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["RemoteDocumentsBlockedServers"];
			}
			set
			{
				base.Fields["RemoteDocumentsBlockedServers"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<string> RemoteDocumentsInternalDomainSuffixList
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["RemoteDocumentsInternalDomainSuffixList"];
			}
			set
			{
				base.Fields["RemoteDocumentsInternalDomainSuffixList"] = value;
			}
		}

		[Parameter]
		public bool InstallIsapiFilter
		{
			get
			{
				return (bool)base.Fields["InstallIsapiFilter"];
			}
			set
			{
				base.Fields["InstallIsapiFilter"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMobileSyncVirtualDirectory(this.Identity.ToString());
			}
		}

		protected virtual bool IsInSetup
		{
			get
			{
				return false;
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			try
			{
				MobileSyncVirtualDirectoryHelper.ReadFromMetabase((ADMobileVirtualDirectory)dataObject, this);
			}
			catch (IISNotInstalledException exception)
			{
				base.WriteError(exception, ErrorCategory.ObjectNotFound, null);
			}
			catch (UnauthorizedAccessException exception2)
			{
				this.WriteError(exception2, ErrorCategory.PermissionDenied, null, false);
			}
			base.StampChangesOn(dataObject);
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADMobileVirtualDirectory admobileVirtualDirectory = (ADMobileVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields.Contains("ActiveSyncServer"))
			{
				admobileVirtualDirectory.ActiveSyncServer = (string)base.Fields["ActiveSyncServer"];
			}
			if (base.Fields.Contains("MobileClientCertificateProvisioningEnabled"))
			{
				admobileVirtualDirectory.MobileClientCertificateProvisioningEnabled = (bool)base.Fields["MobileClientCertificateProvisioningEnabled"];
			}
			if (base.Fields.Contains("BadItemReportingEnabled"))
			{
				admobileVirtualDirectory.BadItemReportingEnabled = (bool)base.Fields["BadItemReportingEnabled"];
			}
			if (base.Fields.Contains("SendWatsonReport"))
			{
				admobileVirtualDirectory.SendWatsonReport = (bool)base.Fields["SendWatsonReport"];
			}
			if (base.Fields.Contains("MobileClientCertificateAuthorityURL"))
			{
				admobileVirtualDirectory.MobileClientCertificateAuthorityURL = (string)base.Fields["MobileClientCertificateAuthorityURL"];
			}
			if (base.Fields.Contains("MobileClientCertTemplateName"))
			{
				admobileVirtualDirectory.MobileClientCertTemplateName = (string)base.Fields["MobileClientCertTemplateName"];
			}
			if (base.Fields.Contains("ClientCertAuth"))
			{
				admobileVirtualDirectory.ClientCertAuth = new ClientCertAuthTypes?((ClientCertAuthTypes)base.Fields["ClientCertAuth"]);
			}
			if (base.Fields.Contains("BasicAuthEnabled"))
			{
				admobileVirtualDirectory.BasicAuthEnabled = (bool)base.Fields["BasicAuthEnabled"];
			}
			if (base.Fields.Contains("WindowsAuthEnabled"))
			{
				admobileVirtualDirectory.WindowsAuthEnabled = (bool)base.Fields["WindowsAuthEnabled"];
			}
			if (base.Fields.Contains("CompressionEnabled"))
			{
				admobileVirtualDirectory.CompressionEnabled = (bool)base.Fields["CompressionEnabled"];
			}
			if (base.Fields.Contains("RemoteDocumentsActionForUnknownServers"))
			{
				admobileVirtualDirectory.RemoteDocumentsActionForUnknownServers = (RemoteDocumentsActions?)base.Fields["RemoteDocumentsActionForUnknownServers"];
			}
			if (base.Fields.Contains("RemoteDocumentsAllowedServers"))
			{
				admobileVirtualDirectory.RemoteDocumentsAllowedServers = (MultiValuedProperty<string>)base.Fields["RemoteDocumentsAllowedServers"];
			}
			if (base.Fields.Contains("RemoteDocumentsBlockedServers"))
			{
				admobileVirtualDirectory.RemoteDocumentsBlockedServers = (MultiValuedProperty<string>)base.Fields["RemoteDocumentsBlockedServers"];
			}
			if (base.Fields.Contains("RemoteDocumentsInternalDomainSuffixList"))
			{
				admobileVirtualDirectory.RemoteDocumentsInternalDomainSuffixList = (MultiValuedProperty<string>)base.Fields["RemoteDocumentsInternalDomainSuffixList"];
			}
			if (base.Fields.Contains("ExtendedProtectionTokenCheckingField"))
			{
				admobileVirtualDirectory.ExtendedProtectionTokenChecking = (ExtendedProtectionTokenCheckingMode)base.Fields["ExtendedProtectionTokenCheckingField"];
			}
			if (base.Fields.Contains("ExtendedProtectionSpnListField"))
			{
				admobileVirtualDirectory.ExtendedProtectionSPNList = (MultiValuedProperty<string>)base.Fields["ExtendedProtectionSpnListField"];
			}
			if (base.Fields.Contains("ExtendedProtectionFlagsField"))
			{
				ExtendedProtectionFlag flags = (ExtendedProtectionFlag)base.Fields["ExtendedProtectionFlagsField"];
				admobileVirtualDirectory.ExtendedProtectionFlags = ExchangeVirtualDirectory.ExtendedProtectionFlagsToMultiValuedProperty(flags);
			}
			return admobileVirtualDirectory;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (!this.IsInSetup && this.DataObject.ExchangeVersion.IsOlderThan(ADMobileVirtualDirectory.MinimumSupportedExchangeObjectVersion))
			{
				base.WriteError(new TaskException(Strings.ErrorSetOlderVirtualDirectory(this.DataObject.Identity.ToString(), this.DataObject.ExchangeVersion.ToString(), ADMobileVirtualDirectory.MinimumSupportedExchangeObjectVersion.ToString())), ErrorCategory.InvalidArgument, null);
			}
			string metabasePath = this.DataObject.MetabasePath;
			if (!IisUtility.Exists(metabasePath))
			{
				base.WriteError(new WebObjectNotFoundException(Strings.ErrorObjectNotFound(metabasePath)), ErrorCategory.ObjectNotFound, metabasePath);
				return;
			}
			if (ExchangeServiceVDirHelper.IsBackEndVirtualDirectory(this.DataObject) && !this.IsInSetup)
			{
				bool flag = false;
				for (int i = 0; i < this.InvalidParametersOnMbxRole.Length; i++)
				{
					if (base.Fields.Contains(this.InvalidParametersOnMbxRole[i]))
					{
						this.WriteError(new InvalidArgumentForServerRoleException(this.InvalidParametersOnMbxRole[i], DirectoryStrings.ServerRoleCafe), ErrorCategory.InvalidArgument, this.DataObject, false);
						flag = true;
					}
				}
				if (flag)
				{
					return;
				}
			}
			bool flag2 = IisUtility.SSLEnabled(metabasePath);
			if (this.DataObject.BasicAuthEnabled && !flag2)
			{
				this.WriteWarning(Strings.WarnBasicAuthInClear);
			}
			if (this.DataObject.ClientCertAuth != ClientCertAuthTypes.Ignore && this.DataObject.ClientCertAuth != ClientCertAuthTypes.Accepted && this.DataObject.ClientCertAuth != ClientCertAuthTypes.Required)
			{
				base.WriteError(new ArgumentException(Strings.InvalidCertAuthValue, "ClientCertAuth"), ErrorCategory.InvalidArgument, null);
				return;
			}
			if (this.DataObject.ClientCertAuth == ClientCertAuthTypes.Required && !flag2)
			{
				base.WriteError(new ArgumentException(Strings.CertAuthWithoutSSLError, "ClientCertAuth"), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			ADMobileVirtualDirectory dataObject = this.DataObject;
			MobileSyncVirtualDirectoryHelper.SetToMetabase(dataObject, this);
			if (base.Fields.Contains("InstallIsapiFilter") && this.InstallIsapiFilter)
			{
				try
				{
					MobileSyncVirtualDirectoryHelper.InstallIsapiFilter(this.DataObject, !ExchangeServiceVDirHelper.IsBackEndVirtualDirectory(this.DataObject));
				}
				catch (Exception ex)
				{
					TaskLogger.Trace("Exception occurred in InstallIsapiFilter(): {0}", new object[]
					{
						ex.Message
					});
					this.WriteWarning(Strings.ActiveSyncMetabaseIsapiInstallFailure);
					throw;
				}
			}
			TaskLogger.LogExit();
		}

		private const string MobileSyncServerKey = "ActiveSyncServer";

		private const string MobileClientCertificateProvisioningEnabledKey = "MobileClientCertificateProvisioningEnabled";

		private const string BadItemReportingEnabledKey = "BadItemReportingEnabled";

		private const string SendWatsonReportKey = "SendWatsonReport";

		private const string MobileClientCertificateAuthorityURLKey = "MobileClientCertificateAuthorityURL";

		private const string MobileClientCertTemplateNameKey = "MobileClientCertTemplateName";

		private const string ClientCertAuthKey = "ClientCertAuth";

		private const string BasicAuthEnabledKey = "BasicAuthEnabled";

		private const string WindowsAuthEnabledKey = "WindowsAuthEnabled";

		private const string CompressionEnabledKey = "CompressionEnabled";

		private const string RemoteDocumentsActionForUnknownServersKey = "RemoteDocumentsActionForUnknownServers";

		private const string RemoteDocumentsAllowedServersKey = "RemoteDocumentsAllowedServers";

		private const string RemoteDocumentsBlockedServersKey = "RemoteDocumentsBlockedServers";

		private const string RemoteDocumentsInternalDomainSuffixListKey = "RemoteDocumentsInternalDomainSuffixList";

		private const string ExtendedProtectionTokenCheckingField = "ExtendedProtectionTokenCheckingField";

		private const string ExtendedProtectionSpnListField = "ExtendedProtectionSpnListField";

		private const string ExtendedProtectionFlagsField = "ExtendedProtectionFlagsField";

		private const string InstallIsapiFilterKey = "InstallIsapiFilter";

		private readonly string[] InvalidParametersOnMbxRole = new string[]
		{
			SetVirtualDirectory<ADMobileVirtualDirectory>.ExternalUrlKey,
			SetVirtualDirectory<ADMobileVirtualDirectory>.InternalUrlKey,
			"BasicAuthEnabled",
			"ClientCertAuth",
			SetVirtualDirectory<ADMobileVirtualDirectory>.ExternalAuthenticationMethodsKey,
			SetVirtualDirectory<ADMobileVirtualDirectory>.InternalAuthenticationMethodsKey,
			"MobileClientCertificateAuthorityURL",
			"MobileClientCertificateProvisioningEnabled",
			"MobileClientCertTemplateName",
			"BadItemReportingEnabled",
			"RemoteDocumentsActionForUnknownServers",
			"RemoteDocumentsAllowedServers",
			"RemoteDocumentsBlockedServers",
			"RemoteDocumentsInternalDomainSuffixList",
			"SendWatsonReport"
		};
	}
}
