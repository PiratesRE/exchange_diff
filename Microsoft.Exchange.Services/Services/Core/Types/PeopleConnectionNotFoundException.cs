using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PeopleConnectionNotFoundException : ServicePermanentException
	{
		public PeopleConnectionNotFoundException() : base(CoreResources.IDs.ErrorPeopleConnectionNotFound)
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
