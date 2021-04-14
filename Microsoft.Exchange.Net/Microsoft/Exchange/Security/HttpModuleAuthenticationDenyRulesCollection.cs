using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	[ConfigurationCollection(typeof(HttpModuleAuthenticationDenyRule), AddItemName = "Rule")]
	public class HttpModuleAuthenticationDenyRulesCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new HttpModuleAuthenticationDenyRule();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((HttpModuleAuthenticationDenyRule)element).Name;
		}

		internal bool TryLoad()
		{
			if (this.isLoaded)
			{
				return this.allRulesLoadedSuccessfully;
			}
			foreach (object obj in this)
			{
				HttpModuleAuthenticationDenyRule httpModuleAuthenticationDenyRule = (HttpModuleAuthenticationDenyRule)obj;
				this.allRulesLoadedSuccessfully &= httpModuleAuthenticationDenyRule.TryLoad();
				if (httpModuleAuthenticationDenyRule.ExecuteEventType.HasFlag(DenyRuleExecuteEvent.PostAuthentication))
				{
					this.postAuthRulesCollection.Add(httpModuleAuthenticationDenyRule);
				}
				if (httpModuleAuthenticationDenyRule.ExecuteEventType.HasFlag(DenyRuleExecuteEvent.PreAuthentication))
				{
					this.preAuthRulesCollection.Add(httpModuleAuthenticationDenyRule);
				}
			}
			this.isLoaded = true;
			return this.allRulesLoadedSuccessfully;
		}

		internal bool EvaluatePreAuthRules(HttpContext httpContext)
		{
			if (!this.allRulesLoadedSuccessfully)
			{
				return true;
			}
			foreach (HttpModuleAuthenticationDenyRule httpModuleAuthenticationDenyRule in this.preAuthRulesCollection)
			{
				if (httpModuleAuthenticationDenyRule.Evaluate(httpContext))
				{
					ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[HttpModuleAuthenticationDenyRulesCollection.ExecutePostAuthRules] Access denied per rule {0}.", httpModuleAuthenticationDenyRule.Name);
					return true;
				}
			}
			return false;
		}

		internal bool EvaluatePostAuthRules(HttpContext httpContext)
		{
			if (!this.allRulesLoadedSuccessfully)
			{
				return true;
			}
			foreach (HttpModuleAuthenticationDenyRule httpModuleAuthenticationDenyRule in this.postAuthRulesCollection)
			{
				if (httpModuleAuthenticationDenyRule.Evaluate(httpContext))
				{
					ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[HttpModuleAuthenticationDenyRulesCollection.ExecutePostAuthRules] Access denied per rule {0}.", httpModuleAuthenticationDenyRule.Name);
					return true;
				}
			}
			return false;
		}

		private bool allRulesLoadedSuccessfully = true;

		private bool isLoaded;

		private List<HttpModuleAuthenticationDenyRule> preAuthRulesCollection = new List<HttpModuleAuthenticationDenyRule>();

		private List<HttpModuleAuthenticationDenyRule> postAuthRulesCollection = new List<HttpModuleAuthenticationDenyRule>();
	}
}
