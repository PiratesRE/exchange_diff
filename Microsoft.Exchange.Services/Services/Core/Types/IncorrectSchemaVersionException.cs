using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class IncorrectSchemaVersionException : ServicePermanentException
	{
		public IncorrectSchemaVersionException() : base((CoreResources.IDs)3510999536U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
