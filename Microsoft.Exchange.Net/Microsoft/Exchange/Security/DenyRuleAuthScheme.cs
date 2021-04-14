using System;
using System.Configuration;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	internal class DenyRuleAuthScheme : ConfigurationElement
	{
		internal DenyRuleAuthenticationType AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
		}

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
			bool result = true;
			if (!Enum.TryParse<DenyRuleAuthenticationType>(this.Value, true, out this.authenticationType))
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[DenyRuleAuthScheme.TryLoad] Authentication type {0} is not supported", this.Value);
				ExEventLog eventLogger = RulesBasedHttpModule.EventLogger;
				ExEventLog.EventTuple tuple_RulesBasedHttpModule_InvalidRuleConfigured = CommonEventLogConstants.Tuple_RulesBasedHttpModule_InvalidRuleConfigured;
				string value = this.Value;
				object[] array = new object[2];
				array[0] = "Auth Scheme";
				eventLogger.LogEvent(tuple_RulesBasedHttpModule_InvalidRuleConfigured, value, array);
				return false;
			}
			return result;
		}

		private DenyRuleAuthenticationType authenticationType;
	}
}
