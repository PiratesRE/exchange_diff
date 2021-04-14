using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Hygiene.Deployment.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Security
{
	public class IpsecConnectionSecurityRulesProbe : ProbeWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.logger = new ProbeWorkItemLogger(this, false, true);
			this.logger.LogMessage("IpsecConnectionSecurityRulesProbe started");
			List<Dictionary<string, string>> firewallConnectionSecurityRules = NetHelpers.GetFirewallConnectionSecurityRules();
			this.logger.LogMessage("ruleCount:" + firewallConnectionSecurityRules.Count);
			bool flag = bool.Parse(ProbeHelper.GetExtensionAttribute(this.logger, this, "VerifyRequestInRequestOut"));
			if (!flag)
			{
				this.logger.LogMessage("Skipping RequestInRequestOut");
				return;
			}
			this.logger.LogMessage("Verifying RequestInRequestOut.");
			bool flag2 = false;
			foreach (Dictionary<string, string> dictionary in firewallConnectionSecurityRules)
			{
				this.logger.LogMessage("ruleName:" + (dictionary.ContainsKey("Rule Name") ? dictionary["Rule Name"] : "(null)"));
				if (this.AssertRuleProperty(dictionary, "Endpoint1", "Any") && this.AssertRuleProperty(dictionary, "Endpoint2", "Any") && this.AssertRuleProperty(dictionary, "Action", "RequestInRequestOut") && this.AssertRuleProperty(dictionary, "Auth1", "ComputerKerb") && this.AssertRuleProperty(dictionary, "Auth2", "UserKerb") && this.AssertRuleProperty(dictionary, "MainModeSecMethods", "ECDHP384-AES256-SHA384"))
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				this.logger.LogMessage("requestInRequestOutSucceeded is true");
				return;
			}
			throw new Exception("Did not find a rule that matched for RequestInRequestOut");
		}

		private bool AssertRuleProperty(Dictionary<string, string> rule, string propertyName, string expectedValue)
		{
			return rule.ContainsKey(propertyName) && string.Equals(rule[propertyName], expectedValue);
		}

		private IHygieneLogger logger = new NullHygieneLogger();
	}
}
