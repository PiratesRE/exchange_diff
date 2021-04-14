using System;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	internal enum UserSettings
	{
		Mail = 1,
		Spelling,
		Calendar = 4,
		General = 8,
		RegionWithoutLanguage = 16,
		RegionAndLanguage = 48
	}
}
