using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class GrammarRuleRef : GrammarItemBase
	{
		internal GrammarRuleRef(CustomGrammarBase grammar, bool hoistVariables) : base(1f)
		{
			this.grammar = grammar;
			this.hoistVariables = hoistVariables;
		}

		public override bool IsEmpty
		{
			get
			{
				return this.grammar.IsEmpty;
			}
		}

		public override bool Equals(GrammarItemBase otherItemBase)
		{
			GrammarRuleRef grammarRuleRef = otherItemBase as GrammarRuleRef;
			return grammarRuleRef != null && string.Equals(grammarRuleRef.grammar.FileName, this.grammar.FileName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(grammarRuleRef.grammar.Rule, this.grammar.Rule, StringComparison.InvariantCultureIgnoreCase);
		}

		protected override string GetInnerItem()
		{
			if (!this.hoistVariables)
			{
				return string.Format(CultureInfo.InvariantCulture, "\r\n                <ruleref uri=\"{0}#{1}\"/>", new object[]
				{
					this.grammar.FileName,
					this.grammar.Rule
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "\r\n                <ruleref uri=\"{0}#{1}\"/>\r\n                <tag>out=rules.latest();</tag>", new object[]
			{
				this.grammar.FileName,
				this.grammar.Rule
			});
		}

		protected override int InternalGetHashCode()
		{
			return this.grammar.FileName.GetHashCode() ^ this.grammar.Rule.GetHashCode();
		}

		private readonly CustomGrammarBase grammar;

		private readonly bool hoistVariables;
	}
}
