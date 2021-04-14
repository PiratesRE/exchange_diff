using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Nspi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class NspiPropertyDefinition : ADPropertyDefinition
	{
		public PropTag PropTag
		{
			get
			{
				return this.propTag;
			}
		}

		public bool MemberOfGlobalCatalog
		{
			get
			{
				return this.memberOfGlobalCatalog;
			}
		}

		public NspiPropertyDefinition(PropTag propTag, Type type, string ldapDisplayName, ADPropertyDefinitionFlags flags, object defaultValue, bool memberOfGlobalCatalog) : base(propTag.ToString(), ExchangeObjectVersion.Exchange2003, type, ldapDisplayName, flags, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null)
		{
			this.propTag = propTag;
			this.memberOfGlobalCatalog = memberOfGlobalCatalog;
		}

		private readonly PropTag propTag;

		private readonly bool memberOfGlobalCatalog;
	}
}
