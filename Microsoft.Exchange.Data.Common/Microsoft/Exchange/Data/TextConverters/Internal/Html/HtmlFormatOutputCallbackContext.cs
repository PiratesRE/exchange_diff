using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal class HtmlFormatOutputCallbackContext : HtmlTagContext
	{
		public HtmlFormatOutputCallbackContext(HtmlFormatOutput formatOutput)
		{
			this.formatOutput = formatOutput;
		}

		public new void InitializeTag(bool isEndTag, HtmlNameIndex tagNameIndex, bool tagDropped)
		{
			base.InitializeTag(isEndTag, tagNameIndex, tagDropped);
			this.countAttributes = 0;
		}

		public void InitializeFragment(bool isEmptyElementTag)
		{
			base.InitializeFragment(isEmptyElementTag, this.countAttributes, (this.countAttributes == 0) ? HtmlFormatOutputCallbackContext.CompleteTagWithoutAttributesParts : HtmlFormatOutputCallbackContext.CompleteTagWithAttributesParts);
		}

		internal void Reset()
		{
			this.countAttributes = 0;
		}

		internal void AddAttribute(HtmlNameIndex nameIndex, string value)
		{
			this.attributes[this.countAttributes].NameIndex = nameIndex;
			this.attributes[this.countAttributes].Value = value;
			this.attributes[this.countAttributes].ReadIndex = 0;
			this.countAttributes++;
		}

		internal override string GetTagNameImpl()
		{
			return HtmlNameData.Names[(int)base.TagNameIndex].Name;
		}

		internal override HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex)
		{
			return HtmlNameData.Names[(int)this.attributes[attributeIndex].NameIndex].PublicAttributeId;
		}

		internal override HtmlAttributeParts GetAttributePartsImpl(int attributeIndex)
		{
			return HtmlFormatOutputCallbackContext.CompleteAttributeParts;
		}

		internal override string GetAttributeNameImpl(int attributeIndex)
		{
			return HtmlNameData.Names[(int)this.attributes[attributeIndex].NameIndex].Name;
		}

		internal override string GetAttributeValueImpl(int attributeIndex)
		{
			return this.attributes[attributeIndex].Value;
		}

		internal override int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count)
		{
			int num = Math.Min(count, this.attributes[attributeIndex].Value.Length - this.attributes[attributeIndex].ReadIndex);
			if (num != 0)
			{
				this.attributes[attributeIndex].Value.CopyTo(this.attributes[attributeIndex].ReadIndex, buffer, offset, num);
				HtmlFormatOutputCallbackContext.AttributeDescriptor[] array = this.attributes;
				array[attributeIndex].ReadIndex = array[attributeIndex].ReadIndex + num;
			}
			return num;
		}

		internal override void WriteTagImpl(bool copyTagAttributes)
		{
			this.formatOutput.Writer.WriteTagBegin(base.TagNameIndex, null, base.IsEndTag, false, false);
			if (copyTagAttributes)
			{
				for (int i = 0; i < this.countAttributes; i++)
				{
					this.WriteAttributeImpl(i, true, true);
				}
			}
		}

		internal override void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue)
		{
			if (writeName)
			{
				this.formatOutput.Writer.WriteAttributeName(this.attributes[attributeIndex].NameIndex);
			}
			if (writeValue)
			{
				this.formatOutput.Writer.WriteAttributeValue(this.attributes[attributeIndex].Value);
			}
		}

		private const int MaxCallbackAttributes = 10;

		private static readonly HtmlAttributeParts CompleteAttributeParts = new HtmlAttributeParts(HtmlToken.AttrPartMajor.Complete, HtmlToken.AttrPartMinor.CompleteNameWithCompleteValue);

		private static readonly HtmlTagParts CompleteTagWithAttributesParts = new HtmlTagParts(HtmlToken.TagPartMajor.Complete, HtmlToken.TagPartMinor.CompleteNameWithAttributes);

		private static readonly HtmlTagParts CompleteTagWithoutAttributesParts = new HtmlTagParts(HtmlToken.TagPartMajor.Complete, HtmlToken.TagPartMinor.CompleteName);

		private HtmlFormatOutput formatOutput;

		private int countAttributes;

		private HtmlFormatOutputCallbackContext.AttributeDescriptor[] attributes = new HtmlFormatOutputCallbackContext.AttributeDescriptor[10];

		private struct AttributeDescriptor
		{
			public HtmlNameIndex NameIndex;

			public string Value;

			public int ReadIndex;
		}
	}
}
