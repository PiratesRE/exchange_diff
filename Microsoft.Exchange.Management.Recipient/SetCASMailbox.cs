using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.AirSync;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("set", "CASMailbox", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetCASMailbox : SetCASMailboxBase<MailboxIdParameter, CASMailbox>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter ResetAutoBlockedDevices
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResetAutoBlockedDevices"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ResetAutoBlockedDevices"] = value;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (MailboxTaskHelper.ExcludeMailboxPlan(adrecipient, false))
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ErrorCategory.InvalidData, this.Identity);
			}
			return adrecipient;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return CASMailbox.FromDataObject((ADUser)dataObject);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			CASMailbox casmailbox = (CASMailbox)this.GetDynamicParameters();
			if (casmailbox.EwsAllowListSpecified && casmailbox.EwsBlockListSpecified)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorEwsAllowListAndEwsBlockListSpecified), ErrorCategory.InvalidArgument, null);
			}
			if (casmailbox.IsModified(CASMailboxSchema.EwsApplicationAccessPolicy))
			{
				if (!casmailbox.EwsAllowListSpecified && !casmailbox.EwsBlockListSpecified)
				{
					casmailbox.EwsExceptions = null;
				}
				if (casmailbox.EwsApplicationAccessPolicy == EwsApplicationAccessPolicy.EnforceAllowList && casmailbox.EwsBlockListSpecified)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorEwsEnforceAllowListAndEwsBlockListSpecified), ErrorCategory.InvalidArgument, null);
				}
				if (casmailbox.EwsApplicationAccessPolicy == EwsApplicationAccessPolicy.EnforceBlockList && casmailbox.EwsAllowListSpecified)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorEwsEnforceBlockListAndEwsAllowListSpecified), ErrorCategory.InvalidArgument, null);
				}
			}
			else
			{
				if (casmailbox.EwsAllowListSpecified)
				{
					casmailbox.EwsApplicationAccessPolicy = new EwsApplicationAccessPolicy?(EwsApplicationAccessPolicy.EnforceAllowList);
				}
				if (casmailbox.EwsBlockListSpecified)
				{
					casmailbox.EwsApplicationAccessPolicy = new EwsApplicationAccessPolicy?(EwsApplicationAccessPolicy.EnforceBlockList);
				}
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurable configurable = base.PrepareDataObject();
			CASMailbox casmailbox = (CASMailbox)this.GetDynamicParameters();
			if (casmailbox.ActiveSyncDebugLoggingSpecified || this.ResetAutoBlockedDevices)
			{
				ADUser user = (ADUser)configurable;
				CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, user, false, this.ConfirmationMessage, delegate(PropertyBag parameters)
				{
					if (parameters.Contains("Confirm"))
					{
						parameters.Remove("Confirm");
					}
				});
			}
			return configurable;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			CASMailbox casmailbox = (CASMailbox)this.GetDynamicParameters();
			if (casmailbox.ActiveSyncDebugLoggingSpecified || this.ResetAutoBlockedDevices)
			{
				ADUser dataObject = this.DataObject;
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(dataObject, RemotingOptions.AllowCrossSite);
				if (exchangePrincipal == null)
				{
					base.WriteVerbose(Strings.ExchangePrincipalNotFoundException(dataObject.ToString()));
					TaskLogger.LogExit();
					return;
				}
				try
				{
					using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=Set-CasMailbox"))
					{
						if (casmailbox.ActiveSyncDebugLoggingSpecified)
						{
							SyncStateStorage.UpdateMailboxLoggingEnabled(mailboxSession, casmailbox.ActiveSyncDebugLogging, null);
						}
						if (this.ResetAutoBlockedDevices)
						{
							List<Exception> list = DeviceBehavior.ResetAutoBlockedDevices(mailboxSession);
							foreach (Exception ex in list)
							{
								base.WriteVerbose(Strings.ResetAutoBlockedDevicesException(ex.ToString()));
							}
						}
					}
				}
				catch (LocalizedException exception)
				{
					base.WriteError(exception, (ErrorCategory)1001, this.Identity);
				}
			}
			TaskLogger.LogExit();
		}

		protected override bool IsObjectStateChanged()
		{
			CASMailbox casmailbox = (CASMailbox)this.GetDynamicParameters();
			return casmailbox.ActiveSyncDebugLoggingSpecified || base.IsObjectStateChanged();
		}
	}
}
