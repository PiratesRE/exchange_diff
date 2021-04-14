using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class ObjectTypeChangedException : ServicePermanentException
	{
		public ObjectTypeChangedException() : base((CoreResources.IDs)4261845811U)
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
