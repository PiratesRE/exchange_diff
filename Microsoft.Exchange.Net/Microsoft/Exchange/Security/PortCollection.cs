using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	[ConfigurationCollection(typeof(DenyRulePort), AddItemName = "Value")]
	public class PortCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new DenyRulePort();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((DenyRulePort)element).Value;
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
				DenyRulePort denyRulePort = (DenyRulePort)obj;
				if (!this.portsSet.Contains(denyRulePort.Value))
				{
					this.portsSet.Add(denyRulePort.Value);
				}
			}
			return true;
		}

		internal bool Evaluate(HttpContext httpContext)
		{
			int item;
			if (!int.TryParse(httpContext.Request.ServerVariables["Server_Port"], out item))
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[PortCollection.Execute] Invalid Server Port {0}", httpContext.Request.ServerVariables["Server_Port"]);
				return this.OperatorType != DenyRuleOperator.In;
			}
			if (this.OperatorType == DenyRuleOperator.In)
			{
				return this.portsSet.Contains(item);
			}
			return !this.portsSet.Contains(item);
		}

		private const string serverPortServerVariableName = "Server_Port";

		private HashSet<int> portsSet = new HashSet<int>();
	}
}
