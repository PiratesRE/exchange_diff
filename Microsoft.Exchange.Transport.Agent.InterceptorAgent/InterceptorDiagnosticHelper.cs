using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal class InterceptorDiagnosticHelper
	{
		internal static XElement GetDiagnosticInfo(DiagnosableParameters parameters, string factoryName, FilteredRuleCache ruleCache)
		{
			XElement xelement = new XElement(factoryName);
			bool flag = parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("showRules", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag3 = parameters.Argument.IndexOf("refresh", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag4 = (!flag2 && !flag3 && !flag) || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			if (flag)
			{
				flag2 = true;
			}
			if (flag3)
			{
				InterceptorAgentRulesCache.Instance.Reload();
				flag2 = true;
			}
			if (flag2)
			{
				InterceptorDiagnosticHelper.GetDiagnosticInfoOfRules(xelement, ruleCache.Rules);
			}
			if (flag4)
			{
				xelement.Add(new XElement("help", "Supported arguments: showRules, refresh, verbose"));
				xelement.Add(new XElement("Example", string.Format("$xml = [xml](Get-ExchangeDiagnosticInfo -server {0} -process {1} -component {2} -argument refresh)", Components.Configuration.LocalServer.TransportServer.Identity, Process.GetCurrentProcess().ProcessName, factoryName)));
			}
			return xelement;
		}

		private static void GetDiagnosticInfoOfRules(XElement root, IEnumerable<InterceptorAgentRule> rules)
		{
			ArgumentValidator.ThrowIfNull("root", root);
			if (rules == null)
			{
				root.Add(new XElement("RuleName", "none"));
				return;
			}
			foreach (InterceptorAgentRule interceptorAgentRule in rules)
			{
				XElement xelement = new XElement("Rule");
				xelement.SetAttributeValue("Name", interceptorAgentRule.Name);
				root.Add(xelement);
				xelement.Add(new XElement("CreatedBy", interceptorAgentRule.CreatedBy));
				xelement.Add(new XElement("Action", interceptorAgentRule.Action));
				xelement.Add(new XElement("Event", interceptorAgentRule.Events));
				XElement xelement2 = new XElement("Conditions");
				xelement.Add(xelement2);
				foreach (InterceptorAgentCondition content in interceptorAgentRule.Conditions)
				{
					xelement2.Add(new XElement("Condition", content));
				}
				xelement.Add(interceptorAgentRule.GetDiagnosticInfoOfPerfCounters());
			}
		}
	}
}
