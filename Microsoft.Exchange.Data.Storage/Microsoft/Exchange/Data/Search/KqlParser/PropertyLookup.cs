using System;
using System.Collections.Generic;
using Microsoft.Ceres.InteractionEngine.Processing.BuiltIn.Parsing.Kql;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Search.KqlParser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyLookup : IPropertyLookup
	{
		public PropertyLookup(LocalizedKeywordMapping keywordMapping, ICollection<PropertyKeyword> keywords, List<ParserErrorInfo> errors)
		{
			this.keywordMapping = keywordMapping;
			this.keywords = keywords;
			this.errors = errors;
		}

		public PropertyInformation GetPropertyInformation(string name)
		{
			PropertyKeyword propertyKeyword;
			if (!this.keywordMapping.TryGetPropertyKeyword(name, out propertyKeyword) || !this.keywords.Contains(propertyKeyword))
			{
				if (this.errors != null)
				{
					this.errors.Add(new ParserErrorInfo(ParserErrorCode.InvalidPropertyKey));
				}
				return null;
			}
			if (propertyKeyword == PropertyKeyword.All)
			{
				return new PropertyInformation(name, typeof(string));
			}
			if (propertyKeyword == PropertyKeyword.IsFlagged)
			{
				return new PropertyInformation(name, typeof(bool));
			}
			Type type = PropertyLookup.GetPropertyKeywordType(propertyKeyword);
			if (type == typeof(ExDateTime))
			{
				type = typeof(DateTime);
			}
			return new PropertyInformation(name, type);
		}

		private static Type GetPropertyKeywordType(PropertyKeyword propertyKeyword)
		{
			return Globals.PropertyKeywordToDefinitionMap[propertyKeyword][0].Type;
		}

		private readonly LocalizedKeywordMapping keywordMapping;

		private readonly ICollection<PropertyKeyword> keywords;

		private List<ParserErrorInfo> errors;
	}
}
