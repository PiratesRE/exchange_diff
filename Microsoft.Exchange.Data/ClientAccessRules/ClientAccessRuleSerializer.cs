using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	internal class ClientAccessRuleSerializer : RuleSerializer
	{
		public static ClientAccessRuleSerializer Instance
		{
			get
			{
				return ClientAccessRuleSerializer.instance;
			}
		}

		protected override void SaveRuleAttributes(XmlWriter xmlWriter, Rule rule)
		{
			ClientAccessRule clientAccessRule = rule as ClientAccessRule;
			if (clientAccessRule != null && clientAccessRule.DatacenterAdminsOnly)
			{
				xmlWriter.WriteAttributeString(ClientAccessRuleSerializer.DatacenterAdminsOnlyAttributeName, clientAccessRule.DatacenterAdminsOnly.ToString());
			}
			base.SaveRuleAttributes(xmlWriter, rule);
		}

		protected override void SaveValue(XmlWriter xmlWriter, Value value)
		{
			if (value != null)
			{
				if (value.ParsedValue is IEnumerable<string>)
				{
					ClientAccessRuleSerializer.SaveValue<string>(xmlWriter, (IEnumerable<string>)value.ParsedValue, (string v) => v.ToString());
					return;
				}
				if (value.ParsedValue is IEnumerable<IPRange>)
				{
					ClientAccessRuleSerializer.SaveValue<IPRange>(xmlWriter, (IEnumerable<IPRange>)value.ParsedValue, (IPRange v) => v.ToString());
					return;
				}
				if (value.ParsedValue is IEnumerable<IntRange>)
				{
					ClientAccessRuleSerializer.SaveValue<IntRange>(xmlWriter, (IEnumerable<IntRange>)value.ParsedValue, (IntRange v) => v.ToString());
					return;
				}
				if (value.ParsedValue is IEnumerable<ClientAccessProtocol>)
				{
					ClientAccessRuleSerializer.SaveValue<ClientAccessProtocol>(xmlWriter, (IEnumerable<ClientAccessProtocol>)value.ParsedValue, delegate(ClientAccessProtocol v)
					{
						int num = (int)v;
						return num.ToString();
					});
					return;
				}
				if (value.ParsedValue is IEnumerable<Regex>)
				{
					ClientAccessRuleSerializer.SaveValue<Regex>(xmlWriter, (IEnumerable<Regex>)value.ParsedValue, new Func<Regex, string>(ClientAccessRulesUsernamePatternProperty.GetDisplayValue));
					return;
				}
				if (value.ParsedValue is IEnumerable<ClientAccessAuthenticationMethod>)
				{
					ClientAccessRuleSerializer.SaveValue<ClientAccessAuthenticationMethod>(xmlWriter, (IEnumerable<ClientAccessAuthenticationMethod>)value.ParsedValue, delegate(ClientAccessAuthenticationMethod v)
					{
						int num = (int)v;
						return num.ToString();
					});
					return;
				}
			}
			base.SaveValue(xmlWriter, value);
		}

		private static void SaveValue<T>(XmlWriter xmlWriter, IEnumerable<T> values, Func<T, string> converter)
		{
			foreach (T arg in values)
			{
				xmlWriter.WriteStartElement("value");
				xmlWriter.WriteValue(converter(arg));
				xmlWriter.WriteEndElement();
			}
		}

		private static readonly ClientAccessRuleSerializer instance = new ClientAccessRuleSerializer();

		private static readonly string DatacenterAdminsOnlyAttributeName = "DatacenterAdminsOnly";
	}
}
