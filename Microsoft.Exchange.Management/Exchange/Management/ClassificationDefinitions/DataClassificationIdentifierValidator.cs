using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class DataClassificationIdentifierValidator : IClassificationRuleCollectionValidator
	{
		private static void ValidateRuleIdentifiersUniqueness(IList<string> ruleIdentifiersToValidate)
		{
			if (DataClassificationIdentifierValidator.isRuleIdComparerCaseSensitive)
			{
				return;
			}
			ExAssert.RetailAssert(ruleIdentifiersToValidate != null, "Classification rule identifiers to be checked for case insensitive uniqueness must not be null");
			if (ruleIdentifiersToValidate.Count == 0)
			{
				return;
			}
			HashSet<string> uniqueRuleIdsSet = new HashSet<string>(ClassificationDefinitionConstants.RuleIdComparer);
			List<string> list = (from ruleId in ruleIdentifiersToValidate
			where !uniqueRuleIdsSet.Add(ruleId)
			select ruleId).ToList<string>();
			if (list.Count > 0)
			{
				LocalizedString message = Strings.ClassificationRuleCollectionNonUniqueRuleIdViolation(string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, list));
				throw ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionIdentifierValidationException, List<string>>(new ClassificationRuleCollectionIdentifierValidationException(message), list);
			}
		}

		private static void ValidateUpdatedOobRulePackIsSuperSet(IEnumerable<string> existingOobRuleIds, IEnumerable<string> updatedOobRuleIds)
		{
			if (existingOobRuleIds == null)
			{
				return;
			}
			List<string> list = existingOobRuleIds.Except(updatedOobRuleIds, ClassificationDefinitionConstants.RuleIdComparer).ToList<string>();
			if (list.Count > 0)
			{
				LocalizedString message = Strings.ClassificationRuleCollectionOobRulesRemoved(string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, list));
				throw ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionIdentifierValidationException, List<string>>(new ClassificationRuleCollectionIdentifierValidationException(message), list);
			}
		}

		private static void ValidateNoRuleIdentifiersConflicts(Dictionary<string, HashSet<string>> existingClassificationIdentifiers, IList<string> ruleIdentifiersToValidate)
		{
			ExAssert.RetailAssert(ruleIdentifiersToValidate != null, "Classification rule identifiers to be checked for conflicts with existing classification rule identifiers must not be null");
			if (existingClassificationIdentifiers == null || existingClassificationIdentifiers.Count == 0 || ruleIdentifiersToValidate.Count == 0)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (HashSet<string> @object in existingClassificationIdentifiers.Values)
			{
				list.AddRange(ruleIdentifiersToValidate.Where(new Func<string, bool>(@object.Contains)));
			}
			if (list.Count > 0)
			{
				LocalizedString message = Strings.ClassificationRuleCollectionExistingRuleIdViolation(string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, list));
				throw ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionIdentifierValidationException, List<string>>(new ClassificationRuleCollectionIdentifierValidationException(message), list);
			}
		}

		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			IList<string> allRuleIds = XmlProcessingUtils.GetAllRuleIds(rulePackXDocument);
			DataClassificationIdentifierValidator.ValidateRuleIdentifiersUniqueness(allRuleIds);
			if (context.OperationType != ClassificationRuleCollectionOperationType.Import && context.IsPayloadOobRuleCollection && context.ExistingRulePackDataObject != null)
			{
				DataClassificationIdentifierValidator.ValidateUpdatedOobRulePackIsSuperSet(context.GetRuleIdentifiersFromExistingRulePack(), allRuleIds);
			}
			string rulePackId = XmlProcessingUtils.GetRulePackId(rulePackXDocument);
			Dictionary<string, HashSet<string>> allExistingClassificationIdentifiers = context.GetAllExistingClassificationIdentifiers(delegate(TransportRule rulePackDataObject)
			{
				string name = rulePackDataObject.Name;
				if (string.IsNullOrEmpty(name))
				{
					name = rulePackDataObject.Id.Name;
				}
				return !ClassificationDefinitionConstants.RuleCollectionIdComparer.Equals(rulePackId, name);
			});
			DataClassificationIdentifierValidator.ValidateNoRuleIdentifiersConflicts(allExistingClassificationIdentifiers, allRuleIds);
		}

		private static readonly bool isRuleIdComparerCaseSensitive = !ClassificationDefinitionConstants.RuleIdComparer.Equals("A", "a");
	}
}
