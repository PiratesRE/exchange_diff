using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExceededMaxSubscriptionLimitException : ServicePermanentException
	{
		public ExceededMaxSubscriptionLimitException() : base(CoreResources.IDs.ErrorExceededSubscriptionCount)
		{
			base.ConstantValues.Add("PolicyLimit", CallContext.Current.Budget.ThrottlingPolicy.EwsMaxSubscriptions.Value.ToString());
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}

		public static void Throw()
		{
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
			{
				throw new ExceededMaxSubscriptionLimitException();
			}
			throw new ServerBusyException();
		}

		private const string PolicyLimitKey = "PolicyLimit";
	}
}
