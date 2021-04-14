using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InvalidSchemaVersionForMailboxVersionException : ServicePermanentException
	{
		public InvalidSchemaVersionForMailboxVersionException() : base(CoreResources.IDs.ErrorInvalidSchemaVersionForMailboxVersion)
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
