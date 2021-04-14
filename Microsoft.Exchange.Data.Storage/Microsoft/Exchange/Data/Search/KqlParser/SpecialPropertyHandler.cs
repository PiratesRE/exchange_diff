using System;
using System.Collections.Generic;
using Microsoft.Ceres.InteractionEngine.Processing.BuiltIn.Parsing.Kql;
using Microsoft.Ceres.NlpBase.RichTypes.QueryTree;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search.KqlParser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SpecialPropertyHandler : ISpecialPropertyHandler
	{
		public SpecialPropertyHandler(LocalizedKeywordMapping keywordMapping, List<ParserErrorInfo> errors)
		{
			this.keywordMapping = keywordMapping;
			this.errors = errors;
		}

		public TreeNode CheckForSpecialProperty(string name, string value, PropertyOperator op, bool excluded)
		{
			PropertyKeyword propertyKeyword;
			KindKeyword kindKeyword;
			if (this.errors != null && this.keywordMapping.TryGetPropertyKeyword(name, out propertyKeyword) && propertyKeyword == PropertyKeyword.Kind && !this.keywordMapping.TryGetKindKeyword(value, out kindKeyword))
			{
				this.errors.Add(new ParserErrorInfo(ParserErrorCode.InvalidKindFormat));
			}
			return null;
		}

		private readonly LocalizedKeywordMapping keywordMapping;

		private List<ParserErrorInfo> errors;
	}
}
