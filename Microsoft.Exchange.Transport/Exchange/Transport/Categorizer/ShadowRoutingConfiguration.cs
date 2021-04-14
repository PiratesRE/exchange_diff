using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ShadowRoutingConfiguration
	{
		public ShadowRoutingConfiguration(ShadowMessagePreference shadowPreference, int remoteShadowCount, int localShadowCount)
		{
			this.shadowMessagePreference = shadowPreference;
			this.remoteShadowCount = remoteShadowCount;
			this.localShadowCount = localShadowCount;
		}

		public ShadowMessagePreference ShadowMessagePreference
		{
			get
			{
				return this.shadowMessagePreference;
			}
		}

		public int RemoteShadowCount
		{
			get
			{
				return this.remoteShadowCount;
			}
		}

		public int LocalShadowCount
		{
			get
			{
				return this.localShadowCount;
			}
		}

		private readonly ShadowMessagePreference shadowMessagePreference;

		private readonly int remoteShadowCount;

		private readonly int localShadowCount;
	}
}
