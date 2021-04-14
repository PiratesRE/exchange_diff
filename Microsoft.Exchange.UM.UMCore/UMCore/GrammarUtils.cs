using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class GrammarUtils
	{
		internal static ResourceManager GrammarResourceManager
		{
			get
			{
				if (GrammarUtils.grammarResources == null)
				{
					lock (GrammarUtils.resourceLock)
					{
						if (GrammarUtils.grammarResources == null)
						{
							GrammarUtils.grammarResources = new ResourceManager("Microsoft.Exchange.UM.Grammars.Grammars.Strings", Assembly.Load("Microsoft.Exchange.UM.Grammars"));
						}
					}
				}
				return GrammarUtils.grammarResources;
			}
		}

		internal static string GetLocString(string grammarKeywordName, CultureInfo culture)
		{
			string @string = GrammarUtils.GrammarResourceManager.GetString(grammarKeywordName, culture);
			if (@string == null)
			{
				throw new MowaGrammarException(Strings.InvalidGrammarResourceId(grammarKeywordName));
			}
			return @string;
		}

		private static ResourceManager grammarResources;

		private static object resourceLock = new object();
	}
}
