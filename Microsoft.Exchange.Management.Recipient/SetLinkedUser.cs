using System;
using System.Management.Automation;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("set", "LinkedUser", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetLinkedUser : SetOrgPersonObjectTask<UserIdParameter, LinkedUser, ADUser>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetLinkedUser(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public UserIdParameter LinkedMasterAccount
		{
			get
			{
				return (UserIdParameter)base.Fields[UserSchema.LinkedMasterAccount];
			}
			set
			{
				base.Fields[UserSchema.LinkedMasterAccount] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LinkedDomainController
		{
			get
			{
				return (string)base.Fields["LinkedDomainController"];
			}
			set
			{
				base.Fields["LinkedDomainController"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PSCredential LinkedCredential
		{
			get
			{
				return (PSCredential)base.Fields["LinkedCredential"];
			}
			set
			{
				base.Fields["LinkedCredential"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.LinkedMasterAccount != null)
			{
				if (string.IsNullOrEmpty(this.LinkedDomainController))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMissLinkedDomainController), ErrorCategory.InvalidArgument, this.Identity);
				}
				try
				{
					NetworkCredential userForestCredential = (this.LinkedCredential == null) ? null : this.LinkedCredential.GetNetworkCredential();
					this.linkedUserSid = MailboxTaskHelper.GetAccountSidFromAnotherForest(this.LinkedMasterAccount, this.LinkedDomainController, userForestCredential, base.GlobalConfigSession, new MailboxTaskHelper.GetUniqueObject(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
				}
				catch (PSArgumentException exception)
				{
					TaskLogger.LogExit();
					base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, this.LinkedCredential);
				}
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADUser aduser = (ADUser)base.ResolveDataObject();
			if (aduser.RecipientTypeDetails != RecipientTypeDetails.LinkedUser)
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ErrorCategory.InvalidData, this.Identity);
			}
			if (base.Fields.IsModified(UserSchema.LinkedMasterAccount))
			{
				aduser.MasterAccountSid = this.linkedUserSid;
			}
			return aduser;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADUser aduser = (ADUser)base.PrepareDataObject();
			aduser.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.LinkedUser);
			if ("Linked" == base.ParameterSetName)
			{
				aduser.MasterAccountSid = this.linkedUserSid;
			}
			TaskLogger.LogExit();
			return aduser;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			SetADUserBase<UserIdParameter, User>.ValidateUserParameters(this.DataObject, this.ConfigurationSession, RecipientTaskHelper.CreatePartitionOrRootOrgScopedGcSession(base.DomainController, this.DataObject.Id), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError), ExchangeErrorCategory.Client, this.ShouldCheckAcceptedDomains(), base.ProvisioningCache);
			if (this.DataObject.IsModified(UserSchema.CertificateSubject))
			{
				NewLinkedUser.ValidateCertificateSubject(this.DataObject.CertificateSubject, OrganizationId.ForestWideOrgId.Equals(this.DataObject.OrganizationId) ? null : this.DataObject.OrganizationId.PartitionId, this.DataObject.Id, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return new LinkedUser((ADUser)dataObject);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		private SecurityIdentifier linkedUserSid;
	}
}
