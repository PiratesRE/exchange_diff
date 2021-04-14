using System;
using System.CodeDom.Compiler;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Search
{
	[GeneratedCode("microsoft.search.platform.parallax.tools.codegenerator.exe", "1.0.0.0")]
	public interface ICompletions : ISettings
	{
		int MaxCompletionTraversalCount { get; }

		string TopNExclusionCharacters { get; }

		bool FinalWordSuggestionsEnabled { get; }
	}
}
