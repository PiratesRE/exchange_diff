using System;
using System.Runtime.InteropServices;
using Microsoft.Mce.Interop.Api;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class RulePackageLoader : IRulePackageLoader
	{
		internal void SetRulePackage(string packageKey, string rulePackage)
		{
			this.packageKey = packageKey;
			this.rulePackage = rulePackage;
		}

		public void GetRulePackages(uint rulePackageRequestDetailsSize, RULE_PACKAGE_REQUEST_DETAILS[] rulePackageRequestDetails)
		{
			if (rulePackageRequestDetailsSize != 1U || rulePackageRequestDetails[0].RulePackageID != this.packageKey)
			{
				throw new COMException("Unable to find the rule package", -2147220985);
			}
			rulePackageRequestDetails[0].RulePackage = this.rulePackage;
		}

		public void GetUpdatedRulePackageInfo(uint rulePackageTimestampDetailsSize, RULE_PACKAGE_TIMESTAMP_DETAILS[] rulePackageTimestampDetails)
		{
			throw new COMException("Rule package updating not supported", -2147467263);
		}

		private string packageKey;

		private string rulePackage;
	}
}
