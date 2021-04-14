using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal struct HtmlTagInstruction
	{
		public HtmlTagInstruction(FormatContainerType containerType, int defaultStyle, int inheritanceMaskIndex, HtmlAttributeInstruction[] attributeInstructions)
		{
			this.containerType = containerType;
			this.defaultStyle = defaultStyle;
			this.inheritanceMaskIndex = inheritanceMaskIndex;
			this.attributeInstructions = attributeInstructions;
		}

		public FormatContainerType ContainerType
		{
			get
			{
				return this.containerType;
			}
		}

		public int DefaultStyle
		{
			get
			{
				return this.defaultStyle;
			}
		}

		public int InheritanceMaskIndex
		{
			get
			{
				return this.inheritanceMaskIndex;
			}
		}

		public HtmlAttributeInstruction[] AttributeInstructions
		{
			get
			{
				return this.attributeInstructions;
			}
		}

		private FormatContainerType containerType;

		private int defaultStyle;

		private int inheritanceMaskIndex;

		private HtmlAttributeInstruction[] attributeInstructions;
	}
}
