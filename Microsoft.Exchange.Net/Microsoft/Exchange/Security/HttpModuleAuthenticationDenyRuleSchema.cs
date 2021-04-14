using System;

namespace Microsoft.Exchange.Security
{
	internal static class HttpModuleAuthenticationDenyRuleSchema
	{
		internal const string DenyRulePortsSection = "Ports";

		internal const string DenyRuleIPRangesSection = "IPRanges";

		internal const string Operator = "Operator";

		internal const string HttpModuleAuthenticationDenyRuleName = "Name";

		internal const string HttpModuleAuthenticationDenyRuleSection = "Rule";

		internal const string HttpModuleAuthenticationDenyRuleExecute = "Execute";

		internal const string HttpModuleAuthenticationDenyRuleDescription = "Description";

		internal const string DenyRuleAuthSchemesSection = "AuthSchemes";

		internal const string DenyRuleUserPatternsSection = "UserPatterns";

		internal const string DenyRuleCookiePatternsSection = "CookiePatterns";

		internal const string DenyRulePortValue = "Value";

		internal const string DenyRuleIPRangeValue = "Value";

		internal const string DenyRuleAuthSchemeValue = "Value";

		internal const string DenyRuleUserPatternsValue = "Value";

		internal const string DenyRuleCookiePatternsValue = "Value";
	}
}
