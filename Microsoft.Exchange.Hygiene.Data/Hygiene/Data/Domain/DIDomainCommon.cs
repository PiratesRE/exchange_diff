using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	internal class DIDomainCommon : DICommon
	{
		public string DomainKey
		{
			get
			{
				return this[DomainSchema.DomainKey] as string;
			}
		}

		public string DomainName
		{
			get
			{
				return this[DomainSchema.DomainName] as string;
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			List<PropertyDefinition> list = new List<PropertyDefinition>(base.GetPropertyDefinitions(false));
			list.AddRange(DIDomainCommon.definitions);
			return list;
		}

		private static readonly PropertyDefinition[] definitions = new PropertyDefinition[]
		{
			DomainSchema.DomainKey,
			DomainSchema.DomainName
		};
	}
}
