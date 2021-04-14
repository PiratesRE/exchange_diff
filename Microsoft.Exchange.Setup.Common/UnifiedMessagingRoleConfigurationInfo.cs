using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UnifiedMessagingRoleConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "UnifiedMessagingRole";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.UnifiedMessagingRoleDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				decimal num = RequiredDiskSpaceStatistics.UnifiedMessagingRole + UmLanguagePackConfigurationInfo.GetUmLanguagePackSizeForCultureInfo(UnifiedMessagingRoleConfigurationInfo.ExchangeCultureForEnUs);
				foreach (CultureInfo umlang in this.SelectedCultures)
				{
					num += UmLanguagePackConfigurationInfo.GetUmLanguagePackSizeForCultureInfo(umlang);
				}
				return num;
			}
		}

		public List<CultureInfo> SelectedCultures
		{
			get
			{
				return InstallableUnitConfigurationInfo.SetupContext.SelectedCultures;
			}
			set
			{
				InstallableUnitConfigurationInfo.SetupContext.SelectedCultures = value;
			}
		}

		private static CultureInfo ExchangeCultureForEnUs = CultureInfo.CreateSpecificCulture("en-us");
	}
}
