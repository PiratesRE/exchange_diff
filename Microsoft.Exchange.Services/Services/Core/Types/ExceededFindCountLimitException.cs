using System;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExceededFindCountLimitException : ServicePermanentException
	{
		public ExceededFindCountLimitException() : base((CoreResources.IDs)2226715912U)
		{
			int findCountLimit = Global.FindCountLimit;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(3913690429U, ref findCountLimit);
			base.ConstantValues.Add("PolicyLimit", findCountLimit.ToString());
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
				throw new ExceededFindCountLimitException();
			}
			throw new ServerBusyException();
		}

		private const string PolicyLimitKey = "PolicyLimit";
	}
}
