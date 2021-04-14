using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class ClassificationRuleCollectionVersionValidator : IClassificationRuleCollectionValidator
	{
		private static void ValidateOobRulePackVersionGreaterThanOrEqual(Version rulePackVersion, Version existingVersion)
		{
			ExAssert.RetailAssert(rulePackVersion != null && existingVersion != null, "Both new and existing rule package version must be specified for version validation purpose");
			if (rulePackVersion < existingVersion)
			{
				LocalizedString message = Strings.ClassificationRuleCollectionVersionViolation(rulePackVersion.ToString(), existingVersion.ToString());
				throw new ClassificationRuleCollectionVersionValidationException(message);
			}
		}

		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			if (context.ExistingRulePackDataObject == null)
			{
				return;
			}
			Version version = null;
			try
			{
				version = context.GetExistingRulePackVersion();
			}
			catch (XmlException)
			{
				if (context.IsPayloadOobRuleCollection)
				{
					throw;
				}
			}
			if (null == version)
			{
				return;
			}
			Version rulePackVersion = XmlProcessingUtils.GetRulePackVersion(rulePackXDocument);
			ClassificationRuleCollectionVersionValidator.ValidateOobRulePackVersionGreaterThanOrEqual(rulePackVersion, version);
		}
	}
}
