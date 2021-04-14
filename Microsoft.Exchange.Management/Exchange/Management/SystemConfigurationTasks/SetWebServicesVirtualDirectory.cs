using System;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "WebServicesVirtualDirectory", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetWebServicesVirtualDirectory : SetExchangeServiceVirtualDirectory<ADWebServicesVirtualDirectory>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetWebServicesVirtualDirectory(this.Identity.ToString());
			}
		}

		[Parameter]
		public Uri InternalNLBBypassUrl
		{
			get
			{
				return (Uri)base.Fields[SetWebServicesVirtualDirectory.InternalNLBBypassUrlKey];
			}
			set
			{
				base.Fields[SetWebServicesVirtualDirectory.InternalNLBBypassUrlKey] = value;
			}
		}

		[Parameter]
		public GzipLevel GzipLevel
		{
			get
			{
				return (GzipLevel)base.Fields["GzipLevel"];
			}
			set
			{
				base.Fields["GzipLevel"] = value;
			}
		}

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
		public bool CertificateAuthentication
		{
			get
			{
				return base.Fields["CertificateAuthentication"] != null && (bool)base.Fields["CertificateAuthentication"];
			}
			set
			{
				base.Fields["CertificateAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MRSProxyEnabled
		{
			get
			{
				return (bool)base.Fields["MRSProxyEnabled"];
			}
			set
			{
				base.Fields["MRSProxyEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		protected override IConfigurable PrepareDataObject()
		{
			ADWebServicesVirtualDirectory adwebServicesVirtualDirectory = (ADWebServicesVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields.Contains(SetWebServicesVirtualDirectory.InternalNLBBypassUrlKey))
			{
				adwebServicesVirtualDirectory.InternalNLBBypassUrl = (Uri)base.Fields[SetWebServicesVirtualDirectory.InternalNLBBypassUrlKey];
			}
			adwebServicesVirtualDirectory.WSSecurityAuthentication = (bool?)base.Fields["WSSecurityAuthentication"];
			adwebServicesVirtualDirectory.OAuthAuthentication = (bool?)base.Fields["OAuthAuthentication"];
			if (base.Fields.IsModified("GzipLevel"))
			{
				if (this.GzipLevel == GzipLevel.Error)
				{
					base.WriteError(new TaskException(Strings.GzipCannotBeSetToError), ErrorCategory.NotSpecified, null);
				}
				else
				{
					if (this.GzipLevel == GzipLevel.Low)
					{
						this.WriteWarning(Strings.GzipLowDoesNotUseDynamicCompression);
					}
					adwebServicesVirtualDirectory.GzipLevel = this.GzipLevel;
					this.WriteWarning(Strings.NeedIisRestartWarning);
				}
			}
			if (base.Fields.IsModified("MRSProxyEnabled"))
			{
				adwebServicesVirtualDirectory.MRSProxyEnabled = this.MRSProxyEnabled;
			}
			return adwebServicesVirtualDirectory;
		}

		private void UpdateCompressionSettings()
		{
			if (base.Fields.IsModified("GzipLevel"))
			{
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
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			ADWebServicesVirtualDirectory adwebServicesVirtualDirectory = dataObject as ADWebServicesVirtualDirectory;
			adwebServicesVirtualDirectory.GzipLevel = Gzip.GetGzipLevel(adwebServicesVirtualDirectory.MetabasePath);
			dataObject.ResetChangeTracking();
			base.StampChangesOn(dataObject);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			base.InternalValidateBasicLiveIdBasic();
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
			if (!(this.DataObject.WindowsAuthentication ?? true))
			{
				if (base.ExchangeRunspaceConfig != null && base.ExchangeRunspaceConfig.ConfigurationSettings != null && (base.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.ECP || base.ExchangeRunspaceConfig.ConfigurationSettings.ClientApplication == ExchangeRunspaceConfigurationSettings.ExchangeApplication.OSP))
				{
					if (!this.Force && !base.ShouldContinue(new LocalizedString(string.Format("{0} {1}", Strings.WarningMessageSetWebServicesVirtualDirectoryWindowsAuthentication(this.Identity.ToString()), Strings.ConfirmationMessageWebServicesVirtualDirectoryContinue))))
					{
						TaskLogger.LogExit();
						return;
					}
				}
				else
				{
					this.WriteWarning(Strings.WarningMessageSetWebServicesVirtualDirectoryWindowsAuthentication(this.Identity.ToString()));
					if (!this.Force && !base.ShouldContinue(Strings.ConfirmationMessageWebServicesVirtualDirectoryContinue))
					{
						TaskLogger.LogExit();
						return;
					}
				}
			}
			this.DataObject.CertificateAuthentication = null;
			base.InternalProcessRecord();
			base.InternalEnableLiveIdNegotiateAuxiliaryModule();
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				string text = string.Format("{0}/{1}", this.DataObject.MetabasePath, "Nego2");
				if (!IisUtility.Exists(text))
				{
					DirectoryEntry directoryEntry = IisUtility.CreateWebDirObject(this.DataObject.MetabasePath, null, "Nego2");
					IisUtility.SetProperty(directoryEntry, "AccessFlags", MetabasePropertyTypes.AccessFlags.Read | MetabasePropertyTypes.AccessFlags.Script, true);
					directoryEntry.CommitChanges();
				}
				ExchangeServiceVDirHelper.SetSplitVirtualDirectoryAuthenticationMethods(this.DataObject, text, new Task.TaskErrorLoggingDelegate(base.WriteError), this.MetabaseSetPropertiesFailureMessage);
				ExchangeServiceVDirHelper.ForceAnonymous(text);
			}
			ExchangeServiceVDirHelper.ForceAnonymous(this.DataObject.MetabasePath);
			ExchangeServiceVDirHelper.EwsAutodiscMWA.OnSetManageWCFEndpoints(this, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol.Ews, this.WSSecurityAuthentication, this.DataObject);
			this.UpdateCompressionSettings();
			TaskLogger.LogExit();
		}

		private const string ParameterMRSProxyEnabled = "MRSProxyEnabled";

		internal const string EWSVDirNameForManagement = "Management";

		internal const string EWSVDirNameForNego2Auth = "Nego2";

		private static readonly string InternalNLBBypassUrlKey = "InternalNLBBypassUrl";
	}
}
