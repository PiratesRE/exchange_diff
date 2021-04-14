using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADMiniClientAccessServerOrArrayWrapper : ADObjectWrapperBase, IADMiniClientAccessServerOrArray, IADObjectCommon
	{
		private ADMiniClientAccessServerOrArrayWrapper(MiniClientAccessServerOrArray caServerOrArray) : base(caServerOrArray)
		{
			this.Fqdn = (string)caServerOrArray[MiniClientAccessServerOrArraySchema.Fqdn];
			this.ExchangeLegacyDN = (string)caServerOrArray[MiniClientAccessServerOrArraySchema.ExchangeLegacyDN];
			this.ServerSite = (ADObjectId)caServerOrArray[MiniClientAccessServerOrArraySchema.Site];
		}

		public static ADMiniClientAccessServerOrArrayWrapper CreateWrapper(MiniClientAccessServerOrArray caServerOrArray)
		{
			if (caServerOrArray == null)
			{
				return null;
			}
			return new ADMiniClientAccessServerOrArrayWrapper(caServerOrArray);
		}

		public string Fqdn { get; private set; }

		public string ExchangeLegacyDN { get; private set; }

		public ADObjectId ServerSite { get; private set; }

		public static readonly ADPropertyDefinition[] PropertiesNeededForCas = new ADPropertyDefinition[]
		{
			MiniClientAccessServerOrArraySchema.Fqdn,
			MiniClientAccessServerOrArraySchema.Site,
			MiniClientAccessServerOrArraySchema.ExchangeLegacyDN
		};
	}
}
