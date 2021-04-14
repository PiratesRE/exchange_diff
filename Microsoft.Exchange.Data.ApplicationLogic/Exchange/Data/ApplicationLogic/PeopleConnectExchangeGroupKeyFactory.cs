using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Dkm;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleConnectExchangeGroupKeyFactory
	{
		private PeopleConnectExchangeGroupKeyFactory()
		{
		}

		public static IExchangeGroupKey Create()
		{
			if (PeopleConnectRegistryReader.Read().DogfoodInEnterprise)
			{
				return new NullExchangeGroupKey();
			}
			return new ExchangeGroupKey(null, "Microsoft Exchange DKM");
		}
	}
}
