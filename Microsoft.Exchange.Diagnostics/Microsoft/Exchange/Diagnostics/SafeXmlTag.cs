using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace Microsoft.Exchange.Diagnostics
{
	internal class SafeXmlTag : IDisposable
	{
		public SafeXmlTag(XmlWriter writer, string tagName)
		{
			this.writer = writer;
			this.writer.WriteStartElement(tagName);
		}

		public SafeXmlTag WithAttribute(string name, string value)
		{
			value = SafeXmlTag.SanitiseString(value);
			this.writer.WriteAttributeString(name, value);
			return this;
		}

		public SafeXmlTag WithAttribute(string nspace, string name, string value)
		{
			value = SafeXmlTag.SanitiseString(value);
			if (nspace == "xml")
			{
				this.writer.WriteAttributeString(nspace, name, "http://www.w3.org/XML/1998/namespace", value);
			}
			else
			{
				this.writer.WriteAttributeString(name, nspace, value);
			}
			return this;
		}

		public void SetContent(string content)
		{
			content = SafeXmlTag.SanitiseString(content);
			this.writer.WriteString(content);
		}

		public void Dispose()
		{
			this.writer.WriteEndElement();
			this.writer = null;
		}

		private static string SanitiseString(string content)
		{
			if (!string.IsNullOrEmpty(content))
			{
				return SafeXmlTag.SanitiseContentRegex.Replace(content, "?");
			}
			return string.Empty;
		}

		private static readonly Regex SanitiseContentRegex = new Regex("(?<![\\uD800-\\uDBFF])[\\uDC00-\\uDFFF]|[\\uD800-\\uDBFF](?![\\uDC00-\\uDFFF])|[\\x00-\\x08\\x0B\\x0C\\x0E-\\x1F\\x7F-\\x9F\\uFEFF\\uFFFE\\uFFFF]", RegexOptions.Compiled);

		private XmlWriter writer;
	}
}
