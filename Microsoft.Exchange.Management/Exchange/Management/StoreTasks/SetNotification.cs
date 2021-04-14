using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Set", "Notification", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetNotification : SetTenantADTaskBase<EwsStoreObjectIdParameter, AsyncOperationNotification, AsyncOperationNotification>
	{
		[Parameter(Mandatory = true, ParameterSetName = "Settings")]
		public AsyncOperationType ProcessType
		{
			get
			{
				return (AsyncOperationType)base.Fields["ProcessType"];
			}
			set
			{
				base.Fields["ProcessType"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Identity")]
		[Parameter(Mandatory = true, ParameterSetName = "Settings")]
		public MultiValuedProperty<SmtpAddress> NotificationEmails
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)base.Fields["NotificationEmails"];
			}
			set
			{
				base.Fields["NotificationEmails"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			return new AsyncOperationNotificationDataProvider(base.CurrentOrganizationId);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetNotification(this.Identity.ToString());
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.ParameterSetName == "Settings")
			{
				string id;
				if (AsyncOperationNotificationDataProvider.SettingsObjectIdentityMap.TryGetValue(this.ProcessType, out id))
				{
					this.Identity = new EwsStoreObjectIdParameter(id);
				}
				else
				{
					base.WriteError(new ArgumentException(Strings.ErrorInvalidAsyncNotificationProcessType(this.ProcessType.ToString())), ErrorCategory.InvalidArgument, this.ProcessType);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			AsyncOperationNotification asyncOperationNotification = (AsyncOperationNotification)dataObject;
			MultiValuedProperty<ADRecipientOrAddress> multiValuedProperty = new MultiValuedProperty<ADRecipientOrAddress>();
			if (this.NotificationEmails != null)
			{
				foreach (SmtpAddress smtpAddress in this.NotificationEmails)
				{
					multiValuedProperty.Add(new ADRecipientOrAddress(new Participant(string.Empty, smtpAddress.ToString(), "SMTP")));
				}
			}
			asyncOperationNotification.NotificationEmails = multiValuedProperty;
			if (!AsyncOperationNotification.IsSettingsObjectId(asyncOperationNotification.AlternativeId) && !asyncOperationNotification.IsNotificationEmailFromTaskSent)
			{
				((AsyncOperationNotificationDataProvider)base.DataSession).SendNotificationEmail(asyncOperationNotification, true, null, true);
				asyncOperationNotification.IsNotificationEmailFromTaskSent = true;
			}
			base.StampChangesOn(dataObject);
			TaskLogger.LogExit();
		}

		private OrganizationId ResolveCurrentOrganization()
		{
			OrganizationId result;
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 173, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\StoreTasks\\AsyncOperationNotification\\SetNotification.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				result = adorganizationalUnit.OrganizationId;
			}
			else
			{
				result = (base.CurrentOrganizationId ?? base.ExecutingUserOrganizationId);
			}
			return result;
		}

		private const string NotificationEmailParameter = "NotificationEmails";

		private const string OrganizationParameter = "Organization";

		private const string ProcessTypeParameter = "ProcessType";

		private const string SettingsParameterSet = "Settings";
	}
}
