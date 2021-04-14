using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	public class WstDataProviderSettings
	{
		public static WstDataProviderSettings Default
		{
			get
			{
				return WstDataProviderSettings.defaultInstance;
			}
		}

		public bool IgnoreCorruptQueryResults { get; set; }

		private static WstDataProviderSettings defaultInstance = new WstDataProviderSettings();
	}
}
