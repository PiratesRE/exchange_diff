using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class PsWsClientErrorParser
	{
		static PsWsClientErrorParser()
		{
			PsWsClientErrorParser.namespaceManager.AddNamespace("meta", PsWsClientErrorParser.MetadataNamespace.NamespaceName);
		}

		public static string Parse(string input)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(input))
			{
				int num = input.IndexOf(PsWsClientErrorParser.responseBody);
				if (num != -1)
				{
					string text = input.Substring(num + PsWsClientErrorParser.responseBody.Length);
					using (XmlReader xmlReader = XmlReader.Create(new StringReader(text.Trim())))
					{
						XDocument xdocument = XDocument.Load(xmlReader);
						XElement xelement = xdocument.Root.XPathSelectElement("/meta:error/meta:innererror/meta:message", PsWsClientErrorParser.namespaceManager);
						string text2 = xelement.Value.ToString();
						int num2 = text2.IndexOf(PsWsClientErrorParser.errorSeparator);
						if (num2 != -1)
						{
							result = text2.Substring(num2 + PsWsClientErrorParser.errorSeparator.Length);
						}
					}
				}
			}
			return result;
		}

		private static readonly XNamespace MetadataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

		private static readonly XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());

		private static readonly string responseBody = "Response body:";

		private static readonly string errorSeparator = ":";
	}
}
