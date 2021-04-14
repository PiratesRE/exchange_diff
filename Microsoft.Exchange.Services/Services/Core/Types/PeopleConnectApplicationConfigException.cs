using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PeopleConnectApplicationConfigException : ServicePermanentException
	{
		public PeopleConnectApplicationConfigException(Exception innerException) : base((CoreResources.IDs)2869245557U, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
