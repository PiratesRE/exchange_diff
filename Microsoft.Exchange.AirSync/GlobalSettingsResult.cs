using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.Exchange.AirSync
{
	public class GlobalSettingsResult
	{
		public static GlobalSettingsResult Create(string propertyNameFilter, bool returnOnlySettingsThatAreNotDefault)
		{
			GlobalSettingsResult globalSettingsResult = new GlobalSettingsResult();
			List<GlobalSettingsResultItem> list = new List<GlobalSettingsResultItem>();
			Regex regex = null;
			if (propertyNameFilter != null)
			{
				string pattern = string.Format("^{0}$", Regex.Escape(propertyNameFilter).Replace("\\*", ".*").Replace("\\?", "."));
				regex = new Regex(pattern);
			}
			IList<GlobalSettingsPropertyDefinition> allProperties = GlobalSettingsSchema.AllProperties;
			foreach (GlobalSettingsPropertyDefinition globalSettingsPropertyDefinition in allProperties)
			{
				if (regex == null || regex.Match(globalSettingsPropertyDefinition.Name).Success)
				{
					object setting = GlobalSettings.GetSetting(globalSettingsPropertyDefinition);
					string valueString = GlobalSettingsResult.GetValueString(setting);
					string valueString2 = GlobalSettingsResult.GetValueString(globalSettingsPropertyDefinition.DefaultValue);
					if (!returnOnlySettingsThatAreNotDefault || valueString != valueString2)
					{
						list.Add(new GlobalSettingsResultItem(globalSettingsPropertyDefinition.Name, globalSettingsPropertyDefinition.Type.Name, GlobalSettingsResult.GetValueString(setting), GlobalSettingsResult.GetValueString(globalSettingsPropertyDefinition.DefaultValue)));
					}
				}
			}
			globalSettingsResult.Entries = list.ToArray();
			return globalSettingsResult;
		}

		private static string GetValueString(object value)
		{
			if (value == null)
			{
				return "$null";
			}
			IList<string> list = value as IList<string>;
			if (list != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (string value2 in list)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					else
					{
						flag = false;
					}
					stringBuilder.Append(value2);
				}
				return stringBuilder.ToString();
			}
			return value.ToString();
		}

		[XmlArrayItem("AppSetting")]
		[XmlArray("AppSettings")]
		public GlobalSettingsResultItem[] Entries { get; set; }
	}
}
