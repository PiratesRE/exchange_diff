using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class AutoProvisionOverride
	{
		internal static bool TryGetOverrides(string domain, ConnectionType type, out string[] overrideHosts, out bool isTrustedForSendAs)
		{
			isTrustedForSendAs = false;
			AutoProvisionOverride.LoadOverridesIfNecessary();
			bool result;
			lock (AutoProvisionOverride.overrideSyncObj)
			{
				Dictionary<string, List<string>> dictionary;
				switch (type)
				{
				case ConnectionType.Imap:
					dictionary = AutoProvisionOverride.imapOverrides;
					break;
				case ConnectionType.Pop:
					dictionary = AutoProvisionOverride.popOverrides;
					break;
				default:
					overrideHosts = null;
					return false;
				}
				if (dictionary.ContainsKey(domain))
				{
					overrideHosts = dictionary[domain].ToArray();
					isTrustedForSendAs = AutoProvisionOverride.sendAsTrustedOverrideDomains.Contains(domain);
					result = true;
				}
				else
				{
					overrideHosts = null;
					result = false;
				}
			}
			return result;
		}

		private static void LoadOverridesIfNecessary()
		{
		}

		private static void ParseOverride(XmlTextReader reader)
		{
			string attribute = reader.GetAttribute(AutoProvisionOverride.Domain);
			string attribute2 = reader.GetAttribute(AutoProvisionOverride.TrustedBySendAs);
			if (attribute2 != null && attribute2.Equals("true"))
			{
				AutoProvisionOverride.sendAsTrustedOverrideDomains.Add(attribute);
			}
			while (reader.Read())
			{
				if (reader.IsStartElement(AutoProvisionOverride.POP))
				{
					AutoProvisionOverride.ParsePOP(reader, attribute);
				}
				else if (reader.IsStartElement(AutoProvisionOverride.IMAP))
				{
					AutoProvisionOverride.ParseIMAP(reader, attribute);
				}
				if (AutoProvisionOverride.IsEndElement(reader, AutoProvisionOverride.Override))
				{
					return;
				}
			}
		}

		private static void ParsePOP(XmlTextReader reader, string domainName)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Text)
				{
					AutoProvisionOverride.AddOverride(domainName, AutoProvisionOverride.popOverrides, reader.Value);
				}
				if (AutoProvisionOverride.IsEndElement(reader, AutoProvisionOverride.POP))
				{
					return;
				}
			}
		}

		private static void ParseIMAP(XmlTextReader reader, string domainName)
		{
			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.Text)
				{
					AutoProvisionOverride.AddOverride(domainName, AutoProvisionOverride.imapOverrides, reader.Value);
				}
				if (AutoProvisionOverride.IsEndElement(reader, AutoProvisionOverride.IMAP))
				{
					return;
				}
			}
		}

		private static bool IsEndElement(XmlTextReader reader, string elementName)
		{
			return reader.NodeType == XmlNodeType.EndElement && reader.Name == elementName;
		}

		private static void AddOverride(string domainName, Dictionary<string, List<string>> domainToHostMap, string hostName)
		{
			List<string> list;
			if (domainToHostMap.TryGetValue(domainName, out list))
			{
				list.Add(hostName);
				return;
			}
			list = new List<string>();
			list.Add(hostName);
			domainToHostMap[domainName] = list;
		}

		private static void ClearCacheData()
		{
			AutoProvisionOverride.popOverrides.Clear();
			AutoProvisionOverride.imapOverrides.Clear();
			AutoProvisionOverride.sendAsTrustedOverrideDomains.Clear();
		}

		private const string OverrideXml = "AutoProvisionOverride.xml";

		internal static readonly string Override = "Override";

		internal static readonly string Domain = "Domain";

		internal static readonly string IMAP = "IMAP";

		internal static readonly string POP = "POP";

		internal static readonly string TrustedBySendAs = "TrustedBySendAs";

		internal static readonly string AutoProvisionOverrides = "AutoProvisionOverrides";

		private static readonly Trace diag = ExTraceGlobals.SubscriptionTaskTracer;

		private static readonly object overrideSyncObj = new object();

		private static readonly string AutoProvisionOverrideXML = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AutoProvisionOverride.xml");

		private static Dictionary<string, List<string>> popOverrides = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, List<string>> imapOverrides = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

		private static HashSet<string> sendAsTrustedOverrideDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static DateTime overrideRefreshTime = DateTime.UtcNow;
	}
}
