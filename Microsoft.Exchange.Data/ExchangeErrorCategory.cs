using System;

namespace Microsoft.Exchange.Data
{
	public enum ExchangeErrorCategory
	{
		Client = 1000,
		ServerOperation,
		ServerTransient,
		Context,
		Authorization,
		LiveIdAlreadyExists,
		WindowsLiveIdAlreadyUsed,
		WLCDPasswordInvalid,
		EasiIdAlreadyExists,
		MailboxMissingServerLegacyDN,
		UpdateLegacyMailboxNotSupported,
		GlobalCatalogNotAvailable,
		ProvisioningLayerNotAvailable
	}
}
