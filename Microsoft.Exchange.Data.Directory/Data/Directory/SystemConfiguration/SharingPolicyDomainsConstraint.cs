using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	internal sealed class SharingPolicyDomainsConstraint : CollectionPropertyDefinitionConstraint
	{
		public override PropertyConstraintViolationError Validate(IEnumerable value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value == null)
			{
				return null;
			}
			HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			StringBuilder stringBuilder = new StringBuilder(128);
			foreach (object obj in value)
			{
				SharingPolicyDomain sharingPolicyDomain = (SharingPolicyDomain)obj;
				if (hashSet.Contains(sharingPolicyDomain.Domain))
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(sharingPolicyDomain.Domain);
				}
				else
				{
					hashSet.Add(sharingPolicyDomain.Domain);
				}
			}
			if (stringBuilder.Length > 0)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.SharingPolicyDuplicateDomain(stringBuilder.ToString()), propertyDefinition, value, this);
			}
			return null;
		}

		public static readonly PropertyDefinitionConstraint[] Constrains = new PropertyDefinitionConstraint[]
		{
			new SharingPolicyDomainsConstraint()
		};
	}
}
