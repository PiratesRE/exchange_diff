using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LanguagePackInstallInfo : InstallableUnitConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "LanguagePacks";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.CopyLanguagePacksDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				if (this.totalSize < 0.0m && InstallableUnitConfigurationInfo.SetupContext != null && InstallableUnitConfigurationInfo.SetupContext.LanguagesToInstall != null)
				{
					this.totalSize = 0.0m;
					foreach (KeyValuePair<string, long> keyValuePair in InstallableUnitConfigurationInfo.SetupContext.LanguagesToInstall)
					{
						this.totalSize += keyValuePair.Value;
					}
					this.totalSize /= 1048576m;
				}
				return this.totalSize;
			}
		}

		private const int megaByte = 1048576;

		private decimal totalSize = -1.0m;
	}
}
