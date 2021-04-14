using System;
using Microsoft.Exchange.Transport.Sync.Common.SendAsDefaults;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class SendAddressDefaultSetting
	{
		public SendAddressDefaultSetting(UserContext userContext) : this(userContext.UserOptions.SendAddressDefault, userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString())
		{
		}

		protected SendAddressDefaultSetting(string settingValue, string userEmailAddress)
		{
			this.settingValue = (settingValue ?? string.Empty);
			this.isUserEmailAddress = this.settingValue.Equals(userEmailAddress);
		}

		public bool IsUserEmailAddress
		{
			get
			{
				return this.isUserEmailAddress;
			}
		}

		public bool TryGetSubscriptionSendAddressId(out Guid subscriptionSendAddressId)
		{
			SendAsDefaultsManager sendAsDefaultsManager = new SendAsDefaultsManager();
			return sendAsDefaultsManager.TryParseSubscriptionSendAddressId(this.settingValue, out subscriptionSendAddressId);
		}

		private string settingValue;

		private bool isUserEmailAddress;
	}
}
