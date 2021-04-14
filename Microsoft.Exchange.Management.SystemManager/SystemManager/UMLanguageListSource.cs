using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class UMLanguageListSource : ObjectListSource
	{
		public UMLanguageListSource(UMLanguage[] languages) : base(languages)
		{
		}

		protected override string GetValueText(object objectValue)
		{
			string result = string.Empty;
			UMLanguage umlanguage = objectValue as UMLanguage;
			if (umlanguage != null)
			{
				result = umlanguage.DisplayName;
			}
			return result;
		}
	}
}
