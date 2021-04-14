using System;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class DiscoveryFlighting
	{
		public static bool IsDocIdHintFlighted(CallContext callContext)
		{
			bool result = false;
			if (callContext != null)
			{
				IRecipientSession session = null;
				ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = null;
				VariantConfigurationSnapshot variantConfigurationSnapshot = null;
				MailboxSearchHelper.PerformCommonAuthorization(callContext.IsExternalUser, out exchangeRunspaceConfiguration, out session);
				if (exchangeRunspaceConfiguration == null && callContext.EffectiveCaller != null && callContext.EffectiveCaller.ObjectSid != null)
				{
					exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(callContext.EffectiveCaller, null, false);
				}
				if (exchangeRunspaceConfiguration != null && exchangeRunspaceConfiguration.ExecutingUser != null)
				{
					ADUser user = new ADUser(session, exchangeRunspaceConfiguration.ExecutingUser.propertyBag);
					variantConfigurationSnapshot = VariantConfiguration.GetSnapshot(user.GetContext(null), null, null);
				}
				if (variantConfigurationSnapshot != null)
				{
					result = variantConfigurationSnapshot.Eac.DiscoveryDocIdHint.Enabled;
				}
			}
			return result;
		}
	}
}
