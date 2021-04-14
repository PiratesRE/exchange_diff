using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal class HtmlToHtmlTagContext : HtmlTagContext
	{
		public HtmlToHtmlTagContext(HtmlToHtmlConverter converter)
		{
			this.converter = converter;
		}

		internal override string GetTagNameImpl()
		{
			if (base.TagNameIndex > HtmlNameIndex.Unknown)
			{
				if (!base.TagParts.Begin)
				{
					return string.Empty;
				}
				return HtmlNameData.Names[(int)base.TagNameIndex].Name;
			}
			else
			{
				if (base.TagParts.Name)
				{
					return this.converter.InternalToken.Name.GetString(int.MaxValue);
				}
				return string.Empty;
			}
		}

		internal override HtmlAttributeId GetAttributeNameIdImpl(int attributeIndex)
		{
			return this.converter.GetAttributeNameId(attributeIndex);
		}

		internal override HtmlAttributeParts GetAttributePartsImpl(int attributeIndex)
		{
			return this.converter.GetAttributeParts(attributeIndex);
		}

		internal override string GetAttributeNameImpl(int attributeIndex)
		{
			return this.converter.GetAttributeName(attributeIndex);
		}

		internal override string GetAttributeValueImpl(int attributeIndex)
		{
			return this.converter.GetAttributeValue(attributeIndex);
		}

		internal override int ReadAttributeValueImpl(int attributeIndex, char[] buffer, int offset, int count)
		{
			return this.converter.ReadAttributeValue(attributeIndex, buffer, offset, count);
		}

		internal override void WriteTagImpl(bool copyTagAttributes)
		{
			this.converter.WriteTag(copyTagAttributes);
		}

		internal override void WriteAttributeImpl(int attributeIndex, bool writeName, bool writeValue)
		{
			this.converter.WriteAttribute(attributeIndex, writeName, writeValue);
		}

		private HtmlToHtmlConverter converter;
	}
}
