using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal struct HtmlAttributeInstruction
	{
		public HtmlAttributeInstruction(HtmlNameIndex attributeNameId, PropertyId propertyId, PropertyValueParsingMethod parsingMethod)
		{
			this.attributeNameId = attributeNameId;
			this.propertyId = propertyId;
			this.parsingMethod = parsingMethod;
		}

		public HtmlNameIndex AttributeNameId
		{
			get
			{
				return this.attributeNameId;
			}
		}

		public PropertyId PropertyId
		{
			get
			{
				return this.propertyId;
			}
		}

		public PropertyValueParsingMethod ParsingMethod
		{
			get
			{
				return this.parsingMethod;
			}
		}

		private HtmlNameIndex attributeNameId;

		private PropertyId propertyId;

		private PropertyValueParsingMethod parsingMethod;
	}
}
