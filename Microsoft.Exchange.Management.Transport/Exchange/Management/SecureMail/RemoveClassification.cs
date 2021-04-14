using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.SecureMail
{
	[Cmdlet("remove", "MessageClassification", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveClassification : RemoveSystemConfigurationObjectTask<MessageClassificationIdParameter, MessageClassification>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (base.DataObject.IsDefault && this.GetLocalizations().Length > 1)
				{
					return Strings.ConfirmationMessageRemoveMessageClassificationExtended(this.Identity.ToString());
				}
				return Strings.ConfirmationMessageRemoveMessageClassification(this.Identity.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity,
				base.DataObject
			});
			if (base.DataObject.IsDefault)
			{
				foreach (MessageClassification messageClassification in this.GetLocalizations())
				{
					if (!messageClassification.IsDefault)
					{
						this.DeleteOne(messageClassification);
					}
				}
			}
			this.DeleteOne(base.DataObject);
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = MessageClassificationIdParameter.DefaultsRoot;
			}
			base.InternalValidate();
			if (Utils.IsMessageClassificationUsedByTransportRule((IConfigurationSession)base.DataSession, base.DataObject))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorRemoveClassificationUsedByTransportRule(base.DataObject.Identity.ToString())), ErrorCategory.InvalidOperation, base.DataObject);
			}
		}

		private MessageClassification[] GetLocalizations()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ClassificationSchema.ClassificationID, base.DataObject.ClassificationID);
			return ((IConfigurationSession)base.DataSession).FindPaged<MessageClassification>(null, QueryScope.SubTree, filter, null, 0).ReadAllPages();
		}

		private void DeleteOne(MessageClassification cl)
		{
			try
			{
				base.DataSession.Delete(cl);
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, this.Identity);
			}
		}
	}
}
