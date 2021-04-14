using System;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	internal class DenyRuleCookiePattern : ConfigurationElement
	{
		[ConfigurationProperty("Value", IsRequired = true)]
		public string Value
		{
			get
			{
				return (string)base["Value"];
			}
			set
			{
				base["Value"] = value;
			}
		}

		protected override void DeserializeElement(XmlReader reader, bool s)
		{
			this.Value = (reader.ReadElementContentAs(typeof(string), null) as string);
		}

		internal bool TryLoad()
		{
			try
			{
				this.DenyRuleCookiePatternRegex = new Regex(this.Value, RegexOptions.IgnoreCase | RegexOptions.Compiled);
			}
			catch (ArgumentNullException ex)
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string, ArgumentNullException>((long)this.GetHashCode(), "[DenyRuleCookiePattern.TryLoad] Exception encountered while parsing regex {0}: {1}", this.Value, ex);
				RulesBasedHttpModule.EventLogger.LogEvent(CommonEventLogConstants.Tuple_RulesBasedHttpModule_InvalidRuleConfigured, this.Value, new object[]
				{
					"Cookie pattern",
					ex
				});
				return false;
			}
			catch (ArgumentException ex2)
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string, ArgumentException>((long)this.GetHashCode(), "[HttpModuleAuthenticationDenyRule.TryLoad] Exception encountered while parsing regex {0}: {1}", this.Value, ex2);
				RulesBasedHttpModule.EventLogger.LogEvent(CommonEventLogConstants.Tuple_RulesBasedHttpModule_InvalidRuleConfigured, this.Value, new object[]
				{
					"Cookie pattern",
					ex2
				});
				return false;
			}
			return true;
		}

		internal bool Evaluate(string cookieForCurrentRequest)
		{
			return this.DenyRuleCookiePatternRegex.IsMatch(cookieForCurrentRequest);
		}

		private Regex DenyRuleCookiePatternRegex;
	}
}
