using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class EnableMailUserBase : EnableRecipientObjectTask<UserIdParameter, ADUser>
	{
		public EnableMailUserBase()
		{
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override UserIdParameter Identity
		{
			get
			{
				return (UserIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override bool DelayProvisioning
		{
			get
			{
				return true;
			}
		}

		internal override bool SkipPrepareDataObject()
		{
			return "Archive" == base.ParameterSetName || base.SkipPrepareDataObject();
		}

		protected override void PrepareRecipientObject(ref ADUser user)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(ref user);
			if (!this.IsValidUser(user))
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorInvalidRecipientType(user.Identity.ToString(), user.RecipientType.ToString())), ErrorCategory.InvalidArgument, user.Id);
			}
			if (user.RecipientType != RecipientType.MailUser)
			{
				user.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
				List<PropertyDefinition> list = new List<PropertyDefinition>(RecipientConstants.DisableMailUserBase_PropertiesToReset);
				MailboxTaskHelper.RemovePersistentProperties(list);
				MailboxTaskHelper.ClearExchangeProperties(user, list);
				user.SetExchangeVersion(ExchangeObjectVersion.Exchange2010);
				if (this.DelayProvisioning && base.IsProvisioningLayerAvailable)
				{
					this.ProvisionDefaultValues(new ADUser(), user);
				}
			}
			TaskLogger.LogExit();
		}

		protected abstract bool IsValidUser(ADUser user);

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			base.InternalProcessRecord();
			this.WriteResult();
			TaskLogger.LogExit();
		}

		protected abstract void WriteResult();

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailUser.FromDataObject((ADUser)dataObject);
		}

		protected override void PrepareRecipientAlias(ADUser dataObject)
		{
			if (!string.IsNullOrEmpty(base.Alias))
			{
				dataObject.Alias = base.Alias;
				return;
			}
			if (string.IsNullOrEmpty(dataObject.Alias))
			{
				dataObject.Alias = RecipientTaskHelper.GenerateUniqueAlias(base.TenantGlobalCatalogSession, dataObject.OrganizationId, string.IsNullOrEmpty(dataObject.UserPrincipalName) ? dataObject.SamAccountName : RecipientTaskHelper.GetLocalPartOfUserPrincalName(dataObject.UserPrincipalName), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
		}
	}
}
