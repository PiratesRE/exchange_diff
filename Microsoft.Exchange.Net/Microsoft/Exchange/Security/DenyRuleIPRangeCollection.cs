using System;
using System.Configuration;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	[ConfigurationCollection(typeof(DenyRuleIPRange), AddItemName = "Value")]
	public class DenyRuleIPRangeCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new DenyRuleIPRange();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((DenyRuleIPRange)element).Value;
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
				DenyRuleIPRange denyRuleIPRange = (DenyRuleIPRange)obj;
				if (!denyRuleIPRange.TryLoad())
				{
					return false;
				}
			}
			return true;
		}

		internal bool Evaluate(HttpContext httpContext)
		{
			string userHostAddress = httpContext.Request.UserHostAddress;
			IPAddress ipaddress;
			if (!IPAddress.TryParse(userHostAddress, out ipaddress))
			{
				ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[DenyRuleIPRange.Evaluate] Invalid User Host Address {0}", httpContext.Request.UserHostAddress);
				return this.OperatorType != DenyRuleOperator.In;
			}
			long num = (long)((ulong)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ipaddress.GetAddressBytes(), 0)));
			if (this.OperatorType == DenyRuleOperator.In)
			{
				foreach (object obj in this)
				{
					DenyRuleIPRange denyRuleIPRange = (DenyRuleIPRange)obj;
					if (num <= denyRuleIPRange.EndIPAddress && num >= denyRuleIPRange.StartIPAddress)
					{
						return true;
					}
				}
				return false;
			}
			foreach (object obj2 in this)
			{
				DenyRuleIPRange denyRuleIPRange2 = (DenyRuleIPRange)obj2;
				if (num <= denyRuleIPRange2.EndIPAddress && num >= denyRuleIPRange2.StartIPAddress)
				{
					return false;
				}
			}
			return true;
		}
	}
}
