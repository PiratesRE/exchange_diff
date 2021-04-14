using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	internal class PowershellTransportRuleSerializer
	{
		public static byte[] Serialize(IEnumerable<Rule> powershellRules)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				XDeclaration declaration = new XDeclaration("1.0", "utf-16", "yes");
				object[] array = new object[1];
				object[] array2 = array;
				int num = 0;
				XName name = XName.Get("rules");
				object[] array3 = new object[2];
				array3[0] = new XAttribute("name", "TransportVersioned");
				array3[1] = from rule in powershellRules
				select new XElement("rule", new object[]
				{
					new XAttribute(XName.Get("name"), rule.Name),
					new XAttribute(XName.Get("id"), rule.ImmutableId),
					new XAttribute(XName.Get("format"), "cmdlet"),
					new XElement(XName.Get("version"), new object[]
					{
						new XAttribute(XName.Get("requiredMinVersion"), "15.0.3.0"),
						new XElement(XName.Get("commandBlock"), new XCData(rule.ToCmdlet()))
					})
				});
				array2[num] = new XElement(name, array3);
				XDocument xdocument = new XDocument(declaration, array);
				xdocument.Save(memoryStream);
				result = memoryStream.ToArray();
			}
			return result;
		}

		private static string GetDuplicateRuleName(IEnumerable<string> ruleNames)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string text in ruleNames)
			{
				string item = text.ToUpper();
				if (hashSet.Contains(item))
				{
					return text;
				}
				hashSet.Add(item);
			}
			return null;
		}

		internal static IEnumerable<string> ParseStream(Stream data)
		{
			IEnumerable<string> result;
			try
			{
				XDocument xdocument = XDocument.Load(data);
				IEnumerable<string> ruleNames = from rule in xdocument.Elements("rules").Elements("rule")
				select rule.Attribute("name").Value;
				string duplicateRuleName = PowershellTransportRuleSerializer.GetDuplicateRuleName(ruleNames);
				if (duplicateRuleName != null)
				{
					throw new ParserException(RulesStrings.RuleNameExists(duplicateRuleName));
				}
				IEnumerable<string> enumerable = from version in xdocument.Elements("rules").Elements("rule").Elements("version")
				where PowershellTransportRuleSerializer.IsSupportedVersion(version.Attribute("requiredMinVersion").Value)
				select version.Element("commandBlock").Value.Trim();
				result = enumerable;
			}
			catch (NullReferenceException ex)
			{
				throw new XmlException("Malformed XML: " + ex.Message);
			}
			return result;
		}

		internal static bool IsSupportedVersion(string versionString)
		{
			try
			{
				Version v = new Version(versionString);
				return v >= PowershellTransportRuleSerializer.LowestSupportedVersion && v <= PowershellTransportRuleSerializer.HighestSupportedVersion;
			}
			catch (ArgumentException)
			{
			}
			catch (FormatException)
			{
			}
			return false;
		}

		internal static Version HighestSupportedVersion = new Version("15.0.3.0");

		internal static Version LowestSupportedVersion = new Version("15.0.3.0");

		internal static class Declaration
		{
			public const string Version = "1.0";

			public const string Encoding = "utf-16";

			public const string StandAlone = "yes";
		}

		internal static class Attribute
		{
			public const string Name = "name";

			public const string Id = "id";

			public const string Format = "format";

			public const string State = "state";

			public const string RequiredMinVersion = "requiredMinVersion";
		}

		internal static class Element
		{
			public const string Rules = "rules";

			public const string Rule = "rule";

			public const string Version = "version";

			public const string CommandBlock = "commandBlock";
		}

		internal static class Value
		{
			public const string Container = "TransportVersioned";

			public const string Format = "cmdlet";

			public const string Version = "15.0.3.0";
		}
	}
}
