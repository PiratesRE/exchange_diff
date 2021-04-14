using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SecureMail
{
	[Cmdlet("Set", "MessageClassification", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMessageClassification : SetSystemConfigurationObjectTask<MessageClassificationIdParameter, MessageClassification>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMessageClassification(this.Identity.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		protected override void InternalValidate()
		{
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			configurationSession.SessionSettings.IsSharedConfigChecked = true;
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = MessageClassificationIdParameter.DefaultsRoot;
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (Server.IsSubscribedGateway(base.GlobalConfigSession))
			{
				base.WriteError(new CannotRunOnSubscribedEdgeException(), ErrorCategory.InvalidOperation, null);
			}
			if (!this.DataObject.IsDefault)
			{
				this.RejectNonDefault(ADObjectSchema.Name);
				this.RejectNonDefault(ClassificationSchema.ClassificationID);
				this.RejectNonDefault(ClassificationSchema.DisplayPrecedence);
				this.RejectNonDefault(ClassificationSchema.PermissionMenuVisible);
				this.RejectNonDefault(ClassificationSchema.RetainClassificationEnabled);
			}
		}

		protected override void InternalProcessRecord()
		{
			this.DataObject.Version++;
			if (this.DataObject.IsDefault)
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ClassificationSchema.ClassificationID, this.DataObject.ClassificationID);
				IEnumerable<MessageClassification> enumerable = base.DataSession.FindPaged<MessageClassification>(filter, null, true, null, 0);
				foreach (MessageClassification messageClassification in enumerable)
				{
					if (!messageClassification.IsDefault)
					{
						messageClassification.Name = this.DataObject.Name;
						messageClassification.DisplayPrecedence = this.DataObject.DisplayPrecedence;
						messageClassification.PermissionMenuVisible = this.DataObject.PermissionMenuVisible;
						messageClassification.RetainClassificationEnabled = this.DataObject.RetainClassificationEnabled;
						base.DataSession.Save(messageClassification);
					}
				}
			}
			base.InternalProcessRecord();
		}

		private void RejectNonDefault(ProviderPropertyDefinition def)
		{
			if (this.DataObject.IsModified(def))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorCannotChangeLocaleProperty(def.Name, this.DataObject.Name)), ErrorCategory.ObjectNotFound, this.Identity);
			}
		}
	}
}
