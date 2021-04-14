using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class ClassificationRuleCollectionIdentifierValidator : IClassificationRuleCollectionValidator
	{
		private static void ValidateRulePackIdentifier(bool isOobRuleCollection, bool isFingerprintsRuleCollection, string rulePackIdentifier)
		{
			ExAssert.RetailAssert(rulePackIdentifier != null, "Rule package ID must be specified for rule package ID validation.");
			bool flag = rulePackIdentifier.StartsWith("00000000-0000-0000-0001");
			if ((isFingerprintsRuleCollection && !flag) || (!isFingerprintsRuleCollection && flag))
			{
				LocalizedString message = Strings.ClassificationRuleCollectionReservedFingerprintRulePackIdViolation(rulePackIdentifier);
				throw new ClassificationRuleCollectionIdentifierValidationException(message);
			}
			if (isFingerprintsRuleCollection)
			{
				return;
			}
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				return;
			}
			bool flag2 = rulePackIdentifier.StartsWith("00000000", StringComparison.OrdinalIgnoreCase);
			if (!isOobRuleCollection && flag2)
			{
				LocalizedString message2 = Strings.ClassificationRuleCollectionReservedRulePackIdViolation(rulePackIdentifier, "00000000");
				throw new ClassificationRuleCollectionIdentifierValidationException(message2);
			}
			if (isOobRuleCollection && !flag2)
			{
				LocalizedString message3 = Strings.ClassificationRuleCollectionOobRulePackIdViolation(rulePackIdentifier, "00000000");
				throw new ClassificationRuleCollectionIdentifierValidationException(message3);
			}
		}

		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			ClassificationRuleCollectionIdentifierValidator.ValidateRulePackIdentifier(context.IsPayloadOobRuleCollection, context.IsPayloadFingerprintsRuleCollection, XmlProcessingUtils.GetRulePackId(rulePackXDocument));
		}
	}
}
