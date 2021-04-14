using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Cmdlet("Remove", "RMSTrustedPublishingDomain", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveRmsTrustedPublishingDomain : RemoveSystemConfigurationObjectTask<RmsTrustedPublishingDomainIdParameter, RMSTrustedPublishingDomain>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveRMSTPD(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			bool flag = false;
			ADPagedReader<RMSTrustedPublishingDomain> adpagedReader = this.ConfigurationSession.FindPaged<RMSTrustedPublishingDomain>(base.DataObject.Id.Parent, QueryScope.OneLevel, null, null, 0);
			foreach (RMSTrustedPublishingDomain rmstrustedPublishingDomain in adpagedReader)
			{
				if (!rmstrustedPublishingDomain.Default)
				{
					flag = true;
					break;
				}
			}
			if (base.DataObject.Default && flag)
			{
				base.WriteError(new CannotRemoveDefaultRmsTpdWithoutSettingAnotherDefaultException(), (ErrorCategory)1000, this.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = false;
			IRMConfiguration irmconfiguration = IRMConfiguration.Read((IConfigurationSession)base.DataSession, out flag);
			if (irmconfiguration == null)
			{
				base.WriteError(new FailedToAccessIrmConfigurationException(), (ErrorCategory)1002, this.Identity);
			}
			if (base.DataObject.Default)
			{
				if (!this.Force && !base.ShouldContinue(Strings.RemoveDefaultTPD(this.Identity.ToString())))
				{
					TaskLogger.LogExit();
					return;
				}
				irmconfiguration.InternalLicensingEnabled = false;
				irmconfiguration.SharedServerBoxRacIdentity = null;
				irmconfiguration.PublishingLocation = null;
				irmconfiguration.ServiceLocation = null;
				irmconfiguration.LicensingLocation = null;
			}
			else
			{
				if (irmconfiguration.LicensingLocation.Contains(base.DataObject.IntranetLicensingUrl))
				{
					irmconfiguration.LicensingLocation.Remove(base.DataObject.IntranetLicensingUrl);
				}
				if (irmconfiguration.LicensingLocation.Contains(base.DataObject.ExtranetLicensingUrl))
				{
					irmconfiguration.LicensingLocation.Remove(base.DataObject.ExtranetLicensingUrl);
				}
			}
			base.DataSession.Save(irmconfiguration);
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || RmsUtil.IsKnownException(exception);
		}
	}
}
