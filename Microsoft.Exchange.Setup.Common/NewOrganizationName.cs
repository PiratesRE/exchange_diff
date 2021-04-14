using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class NewOrganizationName : OrganizationName
	{
		public NewOrganizationName(string name) : base(name)
		{
		}

		public NewOrganizationName(AdName adName) : base(adName)
		{
		}

		protected override ADPropertyDefinition ADPropertyDefinition
		{
			get
			{
				return NewOrganizationName.NewOrgName;
			}
		}

		public static PropertyConstraintViolationError ValidateIsValidNewOrgName(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag, PropertyDefinitionConstraint owner)
		{
			string text = value.ToString();
			if (!LegacyDN.IsValidLegacyCommonName(text))
			{
				return new PropertyConstraintViolationError(Strings.InvalidOrganizationName(text), propertyDefinition, value, owner);
			}
			return null;
		}

		private static readonly ADPropertyDefinition NewOrgName = new ADPropertyDefinition("New Organization Name", ExchangeObjectVersion.Exchange2003, typeof(string), "New Organization Name", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new ADObjectNameStringLengthConstraint(1, 64),
			new ContainingNonWhitespaceConstraint(),
			new ADObjectNameCharacterConstraint(OrganizationName.ConstrainedCharacters),
			new DelegateConstraint(new ValidationDelegate(NewOrganizationName.ValidateIsValidNewOrgName))
		}, null, null);
	}
}
