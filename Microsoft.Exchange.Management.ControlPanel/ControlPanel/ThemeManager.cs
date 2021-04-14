using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ThemeManager
	{
		public static string GetDefaultThemeName(FeatureSet featureSet)
		{
			switch (featureSet)
			{
			case FeatureSet.Options:
				return "options";
			}
			return "default";
		}

		private const string AdminDefaultTheme = "default";

		private const string OptionsDefaultTheme = "options";
	}
}
