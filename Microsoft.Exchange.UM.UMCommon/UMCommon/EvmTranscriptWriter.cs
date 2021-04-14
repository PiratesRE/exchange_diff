using System;
using System.Globalization;
using System.Xml;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class EvmTranscriptWriter
	{
		protected EvmTranscriptWriter(CultureInfo language)
		{
			this.Language = language;
		}

		private protected CultureInfo Language { protected get; private set; }

		public void WriteTranscript(XmlNode asrData)
		{
			bool flag = false;
			XmlElement xmlElement = null;
			XmlElement xmlElement2 = null;
			if (asrData.NodeType == XmlNodeType.Element && string.Equals(asrData.LocalName, "ASR", StringComparison.OrdinalIgnoreCase))
			{
				foreach (object obj in asrData.ChildNodes)
				{
					XmlElement xmlElement3 = (XmlElement)obj;
					if (string.Equals(xmlElement3.LocalName, "Information", StringComparison.OrdinalIgnoreCase))
					{
						xmlElement = xmlElement3;
					}
					else if (string.Equals(xmlElement3.LocalName, "ErrorInformation", StringComparison.OrdinalIgnoreCase))
					{
						xmlElement2 = xmlElement3;
					}
					else
					{
						this.Write(xmlElement3);
						flag = true;
					}
				}
			}
			if (flag && AppConfig.Instance.Service.EnableTranscriptionWhitespace)
			{
				this.WriteEndOfParagraph();
			}
			if (xmlElement != null)
			{
				this.WriteInformation(xmlElement);
			}
			if (xmlElement2 != null)
			{
				this.WriteInformation(xmlElement2);
			}
		}

		public void WriteErrorInformationOnly(XmlNode asrData)
		{
			if (asrData.NodeType == XmlNodeType.Element && string.Equals(asrData.LocalName, "ASR", StringComparison.OrdinalIgnoreCase))
			{
				foreach (object obj in asrData.ChildNodes)
				{
					XmlElement xmlElement = (XmlElement)obj;
					if (string.Equals(xmlElement.LocalName, "ErrorInformation", StringComparison.OrdinalIgnoreCase))
					{
						this.WriteInformation(xmlElement);
						break;
					}
				}
			}
		}

		protected static bool IgnoreLeadingOrTrailingCharInTelAnchor(char c)
		{
			return '.' == c || ',' == c || ';' == c || ':' == c || char.IsWhiteSpace(c);
		}

		protected void Write(XmlElement element)
		{
			if (string.Equals(element.LocalName, "Feature", StringComparison.OrdinalIgnoreCase))
			{
				string value = element.Attributes["class"].Value;
				if (string.Equals(value, "PhoneNumber", StringComparison.OrdinalIgnoreCase))
				{
					this.WritePhoneNumber(element);
					return;
				}
				this.WriteGenericFeature(element);
				return;
			}
			else
			{
				if (string.Equals(element.LocalName, "Text", StringComparison.OrdinalIgnoreCase))
				{
					this.WriteGenericTextElement(element);
					return;
				}
				if (string.Equals(element.LocalName, "Break", StringComparison.OrdinalIgnoreCase))
				{
					this.WriteBreakElement(element);
					return;
				}
				throw new ArgumentException(element.LocalName);
			}
		}

		protected abstract void WriteEndOfParagraph();

		protected abstract void WriteInformation(XmlElement element);

		protected abstract void WritePhoneNumber(XmlElement element);

		protected abstract void WriteGenericFeature(XmlElement element);

		protected abstract void WriteGenericTextElement(XmlElement element);

		protected abstract void WriteBreakElement(XmlElement element);
	}
}
