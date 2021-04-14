using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Cmdlet("Remove", "ClassificationRuleCollection", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveClassificationRuleCollection : RemoveSystemConfigurationObjectTask<ClassificationRuleCollectionIdParameter, TransportRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveClassificationRuleCollection(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (base.OptionalIdentityData != null)
			{
				base.OptionalIdentityData.ConfigurationContainerRdn = ClassificationDefinitionConstants.ClassificationDefinitionsRdn;
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			ExAssert.RetailAssert(base.DataObject != null, "DataObject must not be null at this point as it should have been resolved by the base class.");
			if (!ClassificationDefinitionUtils.IsAdObjectAClassificationRuleCollection(base.DataObject))
			{
				base.WriteError(new ClassificationRuleCollectionInvalidObjectException(Strings.ClassificationRuleCollectionInvalidObject), ErrorCategory.InvalidOperation, null);
			}
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled && OrganizationId.ForestWideOrgId.Equals(((IDirectorySession)base.DataSession).SessionSettings.CurrentOrganizationId) && !this.Identity.IsHierarchical)
			{
				base.WriteError(new ClassificationRuleCollectionIllegalScopeException(Strings.ClassificationRuleCollectionIllegalScopedRemoveOperation), ErrorCategory.InvalidOperation, null);
			}
			SortedSet<string> sortedSet = new SortedSet<string>();
			IList<string> allRulesInUse = this.GetAllRulesInUse(base.DataObject, sortedSet);
			if (allRulesInUse.Count > 0)
			{
				LocalizedString message = Strings.ClassificationRuleCollectionSharingViolationRemoveOperationVerbose(this.Identity.ToString(), string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, allRulesInUse), string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, sortedSet));
				ClassificationRuleCollectionSharingViolationException exception = ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionSharingViolationException, IList<string>>(new ClassificationRuleCollectionSharingViolationException(message), allRulesInUse);
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
			}
		}

		private IList<string> GetAllRulesInUse(TransportRule transportRule, ISet<string> referencingRules = null)
		{
			ExAssert.RetailAssert(((IDirectorySession)base.DataSession).SessionSettings.CurrentOrganizationId.Equals(transportRule.OrganizationId), "Remove-ClassificationRuleCollection session must be underscoped to tenant organization to check whether the collection is in-use.");
			return ClassificationRuleCollectionValidationUtils.GetDeletedRulesInUse(base.DataSession, transportRule, referencingRules, null);
		}
	}
}
