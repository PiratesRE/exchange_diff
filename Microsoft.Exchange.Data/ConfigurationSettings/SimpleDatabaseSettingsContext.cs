using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SimpleDatabaseSettingsContext : SettingsContextBase
	{
		public SimpleDatabaseSettingsContext(Guid mdbGuid, SettingsContextBase nextContext = null) : base(nextContext)
		{
			this.mdbGuid = mdbGuid;
		}

		public override Guid? DatabaseGuid
		{
			get
			{
				return new Guid?(this.mdbGuid);
			}
		}

		private readonly Guid mdbGuid;
	}
}
