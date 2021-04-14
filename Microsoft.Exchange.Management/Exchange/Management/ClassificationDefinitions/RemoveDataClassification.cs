using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Cmdlet("Remove", "DataClassification", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveDataClassification : RemoveSystemConfigurationObjectTask<DataClassificationIdParameter, TransportRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveDataClassification(this.Identity.ToString());
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			TaskLogger.LogEnter();
			this.implementation = new DataClassificationCmdletsImplementation(this);
			TransportRule transportRule = this.implementation.Initialize(base.DataSession, this.Identity, base.OptionalIdentityData);
			if (TaskHelper.ShouldUnderscopeDataSessionToOrganization((IDirectorySession)base.DataSession, transportRule))
			{
				base.UnderscopeDataSession(transportRule.OrganizationId);
				base.CurrentOrganizationId = transportRule.OrganizationId;
			}
			TaskLogger.LogExit();
			return transportRule;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = ClassificationDefinitionConstants.ClassificationDefinitionsRdn;
			}
			base.InternalValidate();
			string name = ((DataClassificationObjectId)this.implementation.DataClassificationPresentationObject.Identity).Name;
			ILookup<string, Rule> dataClassificationsInUse = DlpUtils.GetDataClassificationsInUse(base.DataSession, new string[]
			{
				name
			}, ClassificationDefinitionConstants.RuleIdComparer);
			if (dataClassificationsInUse.Contains(name))
			{
				List<string> list = (from transportRule in dataClassificationsInUse[name]
				select transportRule.Name).ToList<string>();
				if (list.Count > 0)
				{
					base.WriteError(new DataClassificationInUseException(this.implementation.DataClassificationPresentationObject.Name, string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, list)), ErrorCategory.InvalidOperation, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			ValidationContext validationContext = new ValidationContext(ClassificationRuleCollectionOperationType.Update, base.CurrentOrganizationId, false, true, (IConfigurationSession)base.DataSession, base.DataObject, null, null);
			if (this.implementation.Delete(validationContext))
			{
				base.InternalProcessRecord();
				return;
			}
			base.DataSession.Save(base.DataObject);
		}

		private DataClassificationCmdletsImplementation implementation;
	}
}
