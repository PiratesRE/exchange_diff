using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Format;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal struct CssPropertyInstruction
	{
		public CssPropertyInstruction(PropertyId propertyId, PropertyValueParsingMethod parsingMethod, MultiPropertyParsingMethod multiPropertyParsingMethod)
		{
			this.propertyId = propertyId;
			this.parsingMethod = parsingMethod;
			this.multiPropertyParsingMethod = multiPropertyParsingMethod;
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

		public MultiPropertyParsingMethod MultiPropertyParsingMethod
		{
			get
			{
				return this.multiPropertyParsingMethod;
			}
		}

		private PropertyId propertyId;

		private PropertyValueParsingMethod parsingMethod;

		private MultiPropertyParsingMethod multiPropertyParsingMethod;
	}
}
