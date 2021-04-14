using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceBandSettingsStorageDiagnosable : LoadBalanceDiagnosableBase<LoadBalanceBandSettingsStorageDiagnosableArguments, LoadBalanceBandSettingsStorageDiagnosableResult>
	{
		public LoadBalanceBandSettingsStorageDiagnosable(LoadBalanceAnchorContext anchorContext) : base(anchorContext.Logger)
		{
			this.anchorContext = anchorContext;
		}

		protected override LoadBalanceBandSettingsStorageDiagnosableResult ProcessDiagnostic()
		{
			LoadBalanceBandSettingsStorageDiagnosableResult loadBalanceBandSettingsStorageDiagnosableResult = new LoadBalanceBandSettingsStorageDiagnosableResult();
			LoadBalanceBandSettingsStorageDiagnosableResult result;
			using (IBandSettingsProvider bandSettingsProvider = this.anchorContext.CreateBandSettingsStorage())
			{
				if (base.Arguments.ShowPersistedBands)
				{
					loadBalanceBandSettingsStorageDiagnosableResult.PersistedBands = bandSettingsProvider.GetPersistedBands().ToArray<PersistedBandDefinition>();
				}
				if (base.Arguments.ShowActiveBands)
				{
					loadBalanceBandSettingsStorageDiagnosableResult.ActiveBands = bandSettingsProvider.GetBandSettings().ToArray<Band>();
				}
				if (base.Arguments.ProcessAction)
				{
					Band band = base.Arguments.CreateBand();
					switch (base.Arguments.Action)
					{
					case LoadBalanceBandSettingsStorageDiagnosableArguments.BandStorageActionType.Create:
						loadBalanceBandSettingsStorageDiagnosableResult.ModifiedBand = bandSettingsProvider.PersistBand(band, base.Arguments.Enabled);
						break;
					case LoadBalanceBandSettingsStorageDiagnosableArguments.BandStorageActionType.Remove:
						loadBalanceBandSettingsStorageDiagnosableResult.ModifiedBand = bandSettingsProvider.RemoveBand(band);
						break;
					case LoadBalanceBandSettingsStorageDiagnosableArguments.BandStorageActionType.Enable:
						loadBalanceBandSettingsStorageDiagnosableResult.ModifiedBand = bandSettingsProvider.EnableBand(band);
						break;
					case LoadBalanceBandSettingsStorageDiagnosableArguments.BandStorageActionType.Disable:
						loadBalanceBandSettingsStorageDiagnosableResult.ModifiedBand = bandSettingsProvider.DisableBand(band);
						break;
					}
				}
				result = loadBalanceBandSettingsStorageDiagnosableResult;
			}
			return result;
		}

		private readonly LoadBalanceAnchorContext anchorContext;
	}
}
