using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class InstallableUnitConfigurationInfo
	{
		public static SetupContext SetupContext
		{
			get
			{
				return InstallableUnitConfigurationInfo.setupContext;
			}
			set
			{
				InstallableUnitConfigurationInfo.setupContext = value;
			}
		}

		public abstract string Name { get; }

		public abstract LocalizedString DisplayName { get; }

		public abstract decimal Size { get; }

		private static SetupContext setupContext;
	}
}
