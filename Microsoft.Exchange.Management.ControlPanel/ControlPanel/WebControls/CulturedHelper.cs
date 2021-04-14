using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	internal class CulturedHelper
	{
		internal static void CreateChildControls(WebControl control, string defaultPattern, int index, Dictionary<string, ITemplate> templates)
		{
			string text = defaultPattern;
			CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
			Dictionary<string, string[]> dictionary = (Dictionary<string, string[]>)ConfigurationManager.GetSection("GlobalInfos");
			if (dictionary != null)
			{
				bool flag = false;
				string[] array = null;
				dictionary.TryGetValue(currentUICulture.Name, out array);
				if (array != null && array.Length >= 2 && !string.IsNullOrEmpty(array[index]))
				{
					text = array[index];
					flag = true;
				}
				if (!flag)
				{
					array = null;
					dictionary.TryGetValue(currentUICulture.TwoLetterISOLanguageName, out array);
					if (array != null && array.Length >= 2 && !string.IsNullOrEmpty(array[index]))
					{
						text = array[index];
					}
				}
			}
			string[] array2 = text.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array2.Length; i++)
			{
				string text2 = array2[i].Trim();
				if (!string.IsNullOrEmpty(text2) && templates[text2] != null)
				{
					templates[text2].InstantiateIn(control);
				}
			}
		}
	}
}
