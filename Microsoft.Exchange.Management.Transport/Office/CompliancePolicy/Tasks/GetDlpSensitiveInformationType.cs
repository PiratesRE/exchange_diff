using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.Classification;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[OutputType(new Type[]
	{
		typeof(PsDlpSensitiveInformationType)
	})]
	[Cmdlet("Get", "DlpSensitiveInformationType")]
	public sealed class GetDlpSensitiveInformationType : Task
	{
		[Parameter(Mandatory = false, Position = 0)]
		public string Identity { get; set; }

		protected override void InternalValidate()
		{
			base.InternalValidate();
			Utils.ThrowIfNotRunInEOP();
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			IClassificationRuleStore instance = InMemoryClassificationRuleStore.GetInstance();
			string locale = CultureInfo.CurrentCulture.ToString().ToLower();
			if (this.Identity != null)
			{
				RuleDefinitionDetails ruleDefinitionDetails = GetDlpSensitiveInformationType.GetRuleDefinitionDetails(instance, this.Identity, locale);
				base.WriteObject(new PsDlpSensitiveInformationType(ruleDefinitionDetails));
				return;
			}
			RULE_PACKAGE_DETAILS[] rulePackageDetails = instance.GetRulePackageDetails(null);
			foreach (RULE_PACKAGE_DETAILS rule_PACKAGE_DETAILS in rulePackageDetails)
			{
				foreach (string identity in rule_PACKAGE_DETAILS.RuleIDs)
				{
					RuleDefinitionDetails ruleDefinitionDetails2 = GetDlpSensitiveInformationType.GetRuleDefinitionDetails(instance, identity, locale);
					base.WriteObject(new PsDlpSensitiveInformationType(ruleDefinitionDetails2));
				}
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || Utils.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(exception));
		}

		internal static RuleDefinitionDetails GetRuleDefinitionDetails(IClassificationRuleStore ruleStore, string identity, string locale)
		{
			RuleDefinitionDetails ruleDetails;
			try
			{
				ruleDetails = ruleStore.GetRuleDetails(identity, locale);
				if (ruleDetails == null || ruleDetails.LocalizableDetails == null || !ruleDetails.LocalizableDetails.Any<KeyValuePair<string, CLASSIFICATION_DEFINITION_DETAILS>>())
				{
					ruleDetails = ruleStore.GetRuleDetails(identity, "en-us");
				}
			}
			catch (ClassificationRuleStoreExceptionBase innerException)
			{
				throw new SensitiveInformationNotFoundException(identity, innerException);
			}
			return ruleDetails;
		}
	}
}
