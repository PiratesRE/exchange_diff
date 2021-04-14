using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Cmdlet("Set", "RMSTrustedPublishingDomain", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetRmsTrustedPublishingDomain : SetSystemConfigurationObjectTask<RmsTrustedPublishingDomainIdParameter, RMSTrustedPublishingDomain>
	{
		[Parameter(Mandatory = false)]
		public Uri IntranetLicensingUrl
		{
			get
			{
				return (Uri)base.Fields["IntranetLicensingUrl"];
			}
			set
			{
				base.Fields["IntranetLicensingUrl"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri ExtranetLicensingUrl
		{
			get
			{
				return (Uri)base.Fields["ExtranetLicensingUrl"];
			}
			set
			{
				base.Fields["ExtranetLicensingUrl"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri IntranetCertificationUrl
		{
			get
			{
				return (Uri)base.Fields["IntranetCertificationUrl"];
			}
			set
			{
				base.Fields["IntranetCertificationUrl"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri ExtranetCertificationUrl
		{
			get
			{
				return (Uri)base.Fields["ExtranetCertificationUrl"];
			}
			set
			{
				base.Fields["ExtranetCertificationUrl"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Default
		{
			get
			{
				return (SwitchParameter)(base.Fields["Default"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Default"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetRMSTPD(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			bool flag = false;
			if (base.Fields.IsModified("IntranetCertificationUrl") && !RMUtil.IsWellFormedRmServiceUrl(this.IntranetCertificationUrl))
			{
				base.WriteError(new RmsUrlIsInvalidException(this.IntranetCertificationUrl), (ErrorCategory)1000, this.IntranetCertificationUrl);
			}
			if (base.Fields.IsModified("ExtranetCertificationUrl") && !RMUtil.IsWellFormedRmServiceUrl(this.ExtranetCertificationUrl))
			{
				base.WriteError(new RmsUrlIsInvalidException(this.ExtranetCertificationUrl), (ErrorCategory)1000, this.ExtranetCertificationUrl);
			}
			Uri intranetLicensingUrl;
			if (base.Fields.IsModified("IntranetLicensingUrl"))
			{
				if (!RMUtil.IsWellFormedRmServiceUrl(this.IntranetLicensingUrl))
				{
					base.WriteError(new RmsUrlIsInvalidException(this.IntranetLicensingUrl), (ErrorCategory)1000, this.IntranetLicensingUrl);
				}
				intranetLicensingUrl = this.IntranetLicensingUrl;
				flag = true;
			}
			else
			{
				intranetLicensingUrl = this.DataObject.IntranetLicensingUrl;
			}
			Uri extranetLicensingUrl;
			if (base.Fields.IsModified("ExtranetLicensingUrl"))
			{
				if (!RMUtil.IsWellFormedRmServiceUrl(this.ExtranetLicensingUrl))
				{
					base.WriteError(new RmsUrlIsInvalidException(this.ExtranetLicensingUrl), (ErrorCategory)1000, this.ExtranetLicensingUrl);
				}
				extranetLicensingUrl = this.ExtranetLicensingUrl;
				flag = true;
			}
			else
			{
				extranetLicensingUrl = this.DataObject.ExtranetLicensingUrl;
			}
			if (flag)
			{
				foreach (string encodedTemplate in this.DataObject.RMSTemplates)
				{
					RmsTemplateType rmsTemplateType;
					string templateXrml = RMUtil.DecompressTemplate(encodedTemplate, out rmsTemplateType);
					this.ValidateTemplate(templateXrml, intranetLicensingUrl, extranetLicensingUrl);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			((IConfigurationSession)base.DataSession).SessionSettings.IsSharedConfigChecked = true;
			bool flag = false;
			IRMConfiguration irmconfiguration = IRMConfiguration.Read((IConfigurationSession)base.DataSession, out flag);
			RMSTrustedPublishingDomain rmstrustedPublishingDomain = null;
			if (irmconfiguration == null)
			{
				base.WriteError(new FailedToAccessIrmConfigurationException(), (ErrorCategory)1002, this.Identity);
			}
			if (this.IntranetCertificationUrl != null)
			{
				this.DataObject.IntranetCertificationUrl = RMUtil.ConvertUriToCertificateLocationDistributionPoint(this.IntranetCertificationUrl);
			}
			if (this.ExtranetCertificationUrl != null)
			{
				this.DataObject.ExtranetCertificationUrl = RMUtil.ConvertUriToCertificateLocationDistributionPoint(this.ExtranetCertificationUrl);
			}
			if (this.IntranetLicensingUrl != null)
			{
				if (irmconfiguration.LicensingLocation.Contains(this.DataObject.IntranetLicensingUrl))
				{
					irmconfiguration.LicensingLocation.Remove(this.DataObject.IntranetLicensingUrl);
				}
				Uri uri = RMUtil.ConvertUriToLicenseLocationDistributionPoint(this.IntranetLicensingUrl);
				if (!irmconfiguration.LicensingLocation.Contains(uri))
				{
					irmconfiguration.LicensingLocation.Add(uri);
				}
				this.DataObject.IntranetLicensingUrl = uri;
			}
			if (this.ExtranetLicensingUrl != null)
			{
				if (irmconfiguration.LicensingLocation.Contains(this.DataObject.ExtranetLicensingUrl))
				{
					irmconfiguration.LicensingLocation.Remove(this.DataObject.ExtranetLicensingUrl);
				}
				Uri uri2 = RMUtil.ConvertUriToLicenseLocationDistributionPoint(this.ExtranetLicensingUrl);
				if (!irmconfiguration.LicensingLocation.Contains(uri2))
				{
					irmconfiguration.LicensingLocation.Add(uri2);
				}
				this.DataObject.ExtranetLicensingUrl = uri2;
			}
			if (this.Default && !this.DataObject.Default)
			{
				this.DataObject.Default = true;
				try
				{
					ImportRmsTrustedPublishingDomain.ChangeDefaultTPDAndUpdateIrmConfigData((IConfigurationSession)base.DataSession, irmconfiguration, this.DataObject, out rmstrustedPublishingDomain);
					irmconfiguration.ServerCertificatesVersion++;
				}
				catch (RightsManagementServerException ex)
				{
					base.WriteError(new FailedToGenerateSharedKeyException(ex), (ErrorCategory)1000, this.Identity);
				}
			}
			if (rmstrustedPublishingDomain != null)
			{
				this.WriteWarning(Strings.WarningChangeDefaultTPD(rmstrustedPublishingDomain.Name, this.DataObject.Name));
				base.DataSession.Save(rmstrustedPublishingDomain);
			}
			base.DataSession.Save(irmconfiguration);
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || RmsUtil.IsKnownException(exception) || typeof(InvalidRpmsgFormatException).IsInstanceOfType(exception) || typeof(FormatException).IsInstanceOfType(exception);
		}

		private void ValidateTemplate(string templateXrml, Uri intranetLicensingUrl, Uri extranetLicensingUrl)
		{
			Uri uri = null;
			Uri uri2 = null;
			Guid templateGuid;
			DrmClientUtils.ParseTemplate(templateXrml, out uri, out uri2, out templateGuid);
			if (uri != null && Uri.Compare(uri, intranetLicensingUrl, UriComponents.SchemeAndServer, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) != 0 && Uri.Compare(uri, extranetLicensingUrl, UriComponents.SchemeAndServer, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) != 0)
			{
				base.WriteError(new FailedToMatchTemplateDistributionPointToLicensingUriException(templateGuid, uri, intranetLicensingUrl), (ErrorCategory)1000, intranetLicensingUrl);
			}
			if (uri2 != null && Uri.Compare(uri2, intranetLicensingUrl, UriComponents.SchemeAndServer, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) != 0 && Uri.Compare(uri2, extranetLicensingUrl, UriComponents.SchemeAndServer, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) != 0)
			{
				base.WriteError(new FailedToMatchTemplateDistributionPointToLicensingUriException(templateGuid, uri2, extranetLicensingUrl), (ErrorCategory)1000, extranetLicensingUrl);
			}
		}
	}
}
