using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	[ConfigurationCollection(typeof(DenyRuleAuthScheme), AddItemName = "Value")]
	public class DenyRuleAuthSchemeCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new DenyRuleAuthScheme();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((DenyRuleAuthScheme)element).Value;
		}

		[ConfigurationProperty("Operator", IsRequired = true)]
		public DenyRuleOperator OperatorType
		{
			get
			{
				return (DenyRuleOperator)base["Operator"];
			}
			set
			{
				base["Operator"] = value;
			}
		}

		internal bool TryLoad()
		{
			foreach (object obj in this)
			{
				DenyRuleAuthScheme denyRuleAuthScheme = (DenyRuleAuthScheme)obj;
				if (!denyRuleAuthScheme.TryLoad())
				{
					return false;
				}
				if (!this.DenyRuleAuthSchemeSet.Contains(denyRuleAuthScheme.AuthenticationType))
				{
					this.DenyRuleAuthSchemeSet.Add(denyRuleAuthScheme.AuthenticationType);
				}
			}
			return true;
		}

		internal bool Evaluate(HttpContext httpContext)
		{
			DenyRuleAuthenticationType item;
			if (!Enum.TryParse<DenyRuleAuthenticationType>(httpContext.User.Identity.AuthenticationType, true, out item))
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[DenyRuleAuthScheme.Execute] Authentication type {0} is not supported", httpContext.User.Identity.AuthenticationType);
				return this.OperatorType != DenyRuleOperator.In;
			}
			if (this.OperatorType == DenyRuleOperator.In)
			{
				return this.DenyRuleAuthSchemeSet.Contains(item);
			}
			return !this.DenyRuleAuthSchemeSet.Contains(item);
		}

		private HashSet<DenyRuleAuthenticationType> DenyRuleAuthSchemeSet = new HashSet<DenyRuleAuthenticationType>();
	}
}
