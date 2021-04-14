using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Exchange.Compliance.Xml
{
	internal class SafeXmlFactory
	{
		public static XmlTextReader CreateSafeXmlTextReader(Stream stream)
		{
			return new XmlTextReader(stream)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(string url)
		{
			return new XmlTextReader(url)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(TextReader input)
		{
			return new XmlTextReader(input)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(Stream input, XmlNameTable nt)
		{
			return new XmlTextReader(input, nt)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(string url, Stream input)
		{
			return new XmlTextReader(url, input)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(string url, TextReader input)
		{
			return new XmlTextReader(url, input)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(string url, XmlNameTable nt)
		{
			return new XmlTextReader(url, nt)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(TextReader input, XmlNameTable nt)
		{
			return new XmlTextReader(input, nt)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			return new XmlTextReader(xmlFragment, fragType, context)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(string url, Stream input, XmlNameTable nt)
		{
			return new XmlTextReader(url, input, nt)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(string url, TextReader input, XmlNameTable nt)
		{
			return new XmlTextReader(url, input, nt)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlTextReader CreateSafeXmlTextReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
		{
			return new XmlTextReader(xmlFragment, fragType, context)
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		public static XmlReader CreateSafeXmlReader(Stream stream)
		{
			return XmlReader.Create(stream, SafeXmlFactory.defaultSettings);
		}

		public static XmlReader CreateSafeXmlReader(Stream stream, XmlReaderSettings settings)
		{
			settings.DtdProcessing = DtdProcessing.Prohibit;
			settings.XmlResolver = null;
			return XmlReader.Create(stream, settings);
		}

		public static XPathDocument CreateXPathDocument(Stream stream)
		{
			XPathDocument result;
			using (XmlReader xmlReader = XmlReader.Create(stream, SafeXmlFactory.defaultSettings))
			{
				result = SafeXmlFactory.CreateXPathDocument(xmlReader);
			}
			return result;
		}

		public static XPathDocument CreateXPathDocument(string uri)
		{
			XPathDocument result;
			using (XmlReader xmlReader = XmlReader.Create(uri, SafeXmlFactory.defaultSettings))
			{
				result = SafeXmlFactory.CreateXPathDocument(xmlReader);
			}
			return result;
		}

		public static XPathDocument CreateXPathDocument(TextReader textReader)
		{
			XPathDocument result;
			using (XmlReader xmlReader = XmlReader.Create(textReader, SafeXmlFactory.defaultSettings))
			{
				result = SafeXmlFactory.CreateXPathDocument(xmlReader);
			}
			return result;
		}

		public static XPathDocument CreateXPathDocument(XmlReader reader)
		{
			if (reader.Settings != null && reader.Settings.DtdProcessing != DtdProcessing.Prohibit)
			{
				throw new XmlDtdException();
			}
			return new XPathDocument(reader);
		}

		public static XPathDocument CreateXPathDocument(string uri, XmlSpace space)
		{
			XPathDocument result;
			using (XmlReader xmlReader = XmlReader.Create(uri, SafeXmlFactory.defaultSettings))
			{
				result = SafeXmlFactory.CreateXPathDocument(xmlReader, space);
			}
			return result;
		}

		public static XPathDocument CreateXPathDocument(XmlReader reader, XmlSpace space)
		{
			if (reader.Settings != null && reader.Settings.DtdProcessing != DtdProcessing.Prohibit)
			{
				throw new XmlDtdException();
			}
			return new XPathDocument(reader, space);
		}

		private static XmlReaderSettings defaultSettings = new XmlReaderSettings
		{
			DtdProcessing = DtdProcessing.Prohibit,
			XmlResolver = null
		};
	}
}
