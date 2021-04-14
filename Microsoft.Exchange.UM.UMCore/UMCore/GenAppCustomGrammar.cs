using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class GenAppCustomGrammar : CustomGrammarBase
	{
		internal GenAppCustomGrammar(CultureInfo transctiptionLanguage, List<CustomGrammarBase> genAppGrammars) : base(transctiptionLanguage)
		{
			this.genAppGrammars = new List<CustomGrammarBase>(genAppGrammars.Count);
			foreach (CustomGrammarBase item in genAppGrammars)
			{
				this.genAppGrammars.Add(item);
			}
		}

		internal override string FileName
		{
			get
			{
				return "ExtGenAppRule.grxml";
			}
		}

		internal override string Rule
		{
			get
			{
				return "ExtGenAppRule";
			}
		}

		internal override void WriteCustomGrammar(string customGrammarDir)
		{
			base.WriteCustomGrammar(customGrammarDir);
			foreach (CustomGrammarBase customGrammarBase in this.genAppGrammars)
			{
				customGrammarBase.WriteCustomGrammar(customGrammarDir);
			}
		}

		protected override List<GrammarItemBase> GetItems()
		{
			List<GrammarItemBase> list = new List<GrammarItemBase>(this.genAppGrammars.Count);
			foreach (CustomGrammarBase grammar in this.genAppGrammars)
			{
				list.Add(new GrammarRuleRef(grammar, true));
			}
			return list;
		}

		private readonly List<CustomGrammarBase> genAppGrammars;
	}
}
