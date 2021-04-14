using System;
using System.Configuration;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	internal class HttpModuleAuthenticationDenyRule : ConfigurationElement
	{
		[ConfigurationProperty("Name", IsRequired = true)]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
		}

		[ConfigurationProperty("Description")]
		public string Description
		{
			get
			{
				return (string)base["Description"];
			}
		}

		[ConfigurationProperty("Execute")]
		public DenyRuleExecuteEvent ExecuteEventType
		{
			get
			{
				if (base["Execute"] == null)
				{
					return DenyRuleExecuteEvent.Always;
				}
				return (DenyRuleExecuteEvent)base["Execute"];
			}
		}

		[ConfigurationProperty("Ports")]
		public PortCollection Ports
		{
			get
			{
				return (PortCollection)base["Ports"];
			}
		}

		[ConfigurationProperty("IPRanges")]
		public DenyRuleIPRangeCollection IPRanges
		{
			get
			{
				return (DenyRuleIPRangeCollection)base["IPRanges"];
			}
		}

		[ConfigurationProperty("AuthSchemes")]
		public DenyRuleAuthSchemeCollection AuthSchemes
		{
			get
			{
				return (DenyRuleAuthSchemeCollection)base["AuthSchemes"];
			}
		}

		[ConfigurationProperty("UserPatterns")]
		public DenyRuleUserPatternCollection UserPatterns
		{
			get
			{
				return (DenyRuleUserPatternCollection)base["UserPatterns"];
			}
		}

		[ConfigurationProperty("CookiePatterns")]
		public DenyRuleCookiePatternCollection CookiePatterns
		{
			get
			{
				return (DenyRuleCookiePatternCollection)base["CookiePatterns"];
			}
		}

		internal bool TryLoad()
		{
			this.isValid = (this.IPRanges.TryLoad() && this.Ports.TryLoad() && this.AuthSchemes.TryLoad() && this.UserPatterns.TryLoad() && this.CookiePatterns.TryLoad());
			return this.isValid;
		}

		internal bool Evaluate(HttpContext httpContext)
		{
			if (!this.isValid)
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[HttpModuleAuthenticationDenyRule.Execute] There was an error loading the rule {0}. Skipping it.", this.Name);
				return true;
			}
			return (this.IPRanges.Count <= 0 || this.IPRanges.Evaluate(httpContext)) && (this.Ports.Count <= 0 || this.Ports.Evaluate(httpContext)) && (this.UserPatterns.Count <= 0 || this.UserPatterns.Evaluate(httpContext)) && (this.AuthSchemes.Count <= 0 || this.AuthSchemes.Evaluate(httpContext)) && (this.CookiePatterns.Count <= 0 || this.CookiePatterns.Evaluate(httpContext));
		}

		private bool isValid;
	}
}
