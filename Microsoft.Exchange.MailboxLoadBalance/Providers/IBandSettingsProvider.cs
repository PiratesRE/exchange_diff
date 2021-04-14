using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;

namespace Microsoft.Exchange.MailboxLoadBalance.Providers
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IBandSettingsProvider : IDisposable
	{
		IEnumerable<Band> GetBandSettings();

		PersistedBandDefinition PersistBand(Band band, bool enabled);

		IEnumerable<PersistedBandDefinition> GetPersistedBands();

		PersistedBandDefinition DisableBand(Band band);

		PersistedBandDefinition EnableBand(Band band);

		PersistedBandDefinition RemoveBand(Band band);
	}
}
