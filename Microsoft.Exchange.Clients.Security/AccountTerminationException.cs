using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Clients.Security
{
	public class AccountTerminationException : LocalizedException
	{
		public AccountTerminationException(AccountState accountState) : base(new LocalizedString(null), null)
		{
			this.AccountState = accountState;
		}

		public AccountState AccountState { get; private set; }

		public override string Message
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, Strings.AccountTerminationErrorMessage, new object[]
				{
					this.AccountState.ToString()
				});
			}
		}

		public Strings.IDs ErrorMessageStringId
		{
			get
			{
				return -4727748;
			}
		}
	}
}
