using System;
using System.Globalization;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DynamicUmGrammarConfig : UMGrammarConfig
	{
		internal DynamicUmGrammarConfig(string name, string rule, string condition, ActivityManagerConfig managerConfig) : base(name, rule, condition, managerConfig)
		{
		}

		internal override UMGrammar GetGrammar(ActivityManager manager, CultureInfo culture)
		{
			SearchGrammarFile searchGrammarFile = (SearchGrammarFile)manager.ReadVariable(base.GrammarName);
			return new UMGrammar(searchGrammarFile.FilePath, base.GrammarRule, culture, searchGrammarFile.BaseUri, false);
		}

		internal override void Validate()
		{
		}
	}
}
