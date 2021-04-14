using System;
using System.Configuration;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security
{
	internal class DenyRuleIPRange : ConfigurationElement
	{
		internal long StartIPAddress
		{
			get
			{
				return (long)((ulong)this.startIPAddress);
			}
		}

		internal long EndIPAddress
		{
			get
			{
				return (long)((ulong)this.endIPAddress);
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
			if (string.IsNullOrEmpty(this.Value))
			{
				return false;
			}
			string[] array = this.Value.Split(new char[]
			{
				'/'
			});
			IPAddress ipaddress = null;
			if (array.Length != 2)
			{
				if (!IPAddress.TryParse(this.Value, out ipaddress))
				{
					Trace rulesBasedHttpModuleTracer = ExTraceGlobals.RulesBasedHttpModuleTracer;
					long id = (long)this.GetHashCode();
					string formatString = "[DenyRuleIPRange.TryLoad] Invalid IP Address notation {0}: {1}";
					object[] array2 = new object[2];
					array2[0] = this.Value;
					rulesBasedHttpModuleTracer.TraceError(id, formatString, array2);
					ExEventLog eventLogger = RulesBasedHttpModule.EventLogger;
					ExEventLog.EventTuple tuple_RulesBasedHttpModule_InvalidRuleConfigured = CommonEventLogConstants.Tuple_RulesBasedHttpModule_InvalidRuleConfigured;
					string value = this.Value;
					object[] array3 = new object[2];
					array3[0] = "IP Range";
					eventLogger.LogEvent(tuple_RulesBasedHttpModule_InvalidRuleConfigured, value, array3);
					return false;
				}
				uint num = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ipaddress.GetAddressBytes(), 0));
				this.startIPAddress = num;
				this.endIPAddress = num;
				return true;
			}
			else
			{
				if (!IPAddress.TryParse(array[0], out ipaddress))
				{
					ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[DenyRuleIPRange.TryLoad] Invalid IP Address notation {0}", this.Value);
					ExEventLog eventLogger2 = RulesBasedHttpModule.EventLogger;
					ExEventLog.EventTuple tuple_RulesBasedHttpModule_InvalidRuleConfigured2 = CommonEventLogConstants.Tuple_RulesBasedHttpModule_InvalidRuleConfigured;
					string value2 = this.Value;
					object[] array4 = new object[2];
					array4[0] = "IP Range";
					eventLogger2.LogEvent(tuple_RulesBasedHttpModule_InvalidRuleConfigured2, value2, array4);
					return false;
				}
				int num2;
				if (!int.TryParse(array[1], out num2))
				{
					ExTraceGlobals.RulesBasedHttpModuleTracer.TraceError<string>((long)this.GetHashCode(), "[DenyRuleIPRange.TryLoad] Invalid Network Address Bit notation {0}", this.Value);
					ExEventLog eventLogger3 = RulesBasedHttpModule.EventLogger;
					ExEventLog.EventTuple tuple_RulesBasedHttpModule_InvalidRuleConfigured3 = CommonEventLogConstants.Tuple_RulesBasedHttpModule_InvalidRuleConfigured;
					string value3 = this.Value;
					object[] array5 = new object[2];
					array5[0] = "IP Range";
					eventLogger3.LogEvent(tuple_RulesBasedHttpModule_InvalidRuleConfigured3, value3, array5);
					return false;
				}
				uint num = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(ipaddress.GetAddressBytes(), 0));
				uint num3 = uint.MaxValue;
				if (num2 == 0)
				{
					num3 = 0U;
				}
				else if (num2 < 32)
				{
					num3 <<= 32 - num2;
				}
				this.startIPAddress = (num & num3);
				this.endIPAddress = (num | ~num3);
				return true;
			}
		}

		private uint startIPAddress;

		private uint endIPAddress;
	}
}
