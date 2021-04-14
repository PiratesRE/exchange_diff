using System;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class TextEvmTranscriptWriter : EvmTranscriptWriter
	{
		private TextEvmTranscriptWriter(CultureInfo language) : base(language)
		{
		}

		public static EvmTranscriptWriter Create(CultureInfo language)
		{
			return new TextEvmTranscriptWriter(language);
		}

		public override string ToString()
		{
			return this.builder.ToString();
		}

		protected override void WriteEndOfParagraph()
		{
			this.builder.Append(Strings.EndOfParagraphMarker.ToString(base.Language));
		}

		protected override void WriteInformation(XmlElement element)
		{
		}

		protected override void WritePhoneNumber(XmlElement element)
		{
			this.WriteGenericFeature(element);
		}

		protected override void WriteGenericFeature(XmlElement element)
		{
			foreach (object obj in element.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				this.builder.Append(xmlNode.InnerText);
			}
		}

		protected override void WriteGenericTextElement(XmlElement element)
		{
			this.builder.Append(element.InnerText);
		}

		protected override void WriteBreakElement(XmlElement element)
		{
			this.WriteEndOfParagraph();
			this.builder.Append(" ");
		}

		private StringBuilder builder = new StringBuilder(1024);
	}
}
