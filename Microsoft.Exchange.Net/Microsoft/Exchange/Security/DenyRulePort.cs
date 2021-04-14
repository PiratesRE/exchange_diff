using System;
using System.Configuration;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	internal class DenyRulePort : ConfigurationElement
	{
		[ConfigurationProperty("Value", IsRequired = true)]
		public int Value
		{
			get
			{
				return (int)base["Value"];
			}
			set
			{
				base["Value"] = value;
			}
		}

		protected override void DeserializeElement(XmlReader reader, bool s)
		{
			string text = reader.ReadElementContentAs(typeof(string), null) as string;
			int value;
			if (!int.TryParse(text, out value))
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[PortCollection.DeserializeElement] Invalid Server Port {0}", text);
				ExEventLog eventLogger = RulesBasedHttpModule.EventLogger;
				ExEventLog.EventTuple tuple_RulesBasedHttpModule_InvalidRuleConfigured = CommonEventLogConstants.Tuple_RulesBasedHttpModule_InvalidRuleConfigured;
				string periodicKey = text;
				object[] array = new object[2];
				array[0] = "Port";
				eventLogger.LogEvent(tuple_RulesBasedHttpModule_InvalidRuleConfigured, periodicKey, array);
			}
			this.Value = value;
		}
	}
}
