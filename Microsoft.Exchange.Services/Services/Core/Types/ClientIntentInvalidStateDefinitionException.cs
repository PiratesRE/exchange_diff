using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ClientIntentInvalidStateDefinitionException : ServicePermanentException
	{
		public ClientIntentInvalidStateDefinitionException() : base((CoreResources.IDs)3510335548U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010;
			}
		}
	}
}
