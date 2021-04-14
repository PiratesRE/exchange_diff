using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class OrganizationName : IOrganizationName
	{
		public OrganizationName(string name)
		{
			this.Initialize(name);
		}

		public OrganizationName(AdName adName)
		{
			if (null == adName)
			{
				throw new ArgumentNullException("adName");
			}
			this.Initialize(adName.UnescapedName);
		}

		protected static char[] ConstrainedCharacters
		{
			get
			{
				return OrganizationName.constrainedCharacters;
			}
		}

		protected virtual ADPropertyDefinition ADPropertyDefinition
		{
			get
			{
				return OrganizationName.OrgName;
			}
		}

		private void Initialize(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new FormatException(Strings.SpecifyExchangeOrganizationName);
			}
			PropertyValidationError propertyValidationError = this.ADPropertyDefinition.ValidateValue(name, false);
			if (propertyValidationError != null)
			{
				throw new FormatException(propertyValidationError.Description);
			}
			this.adName = new AdName("CN", name);
		}

		public string UnescapedName
		{
			get
			{
				return this.adName.UnescapedName;
			}
		}

		public string EscapedName
		{
			get
			{
				return this.adName.EscapedName;
			}
		}

		public override string ToString()
		{
			return this.adName.ToString();
		}

		private static readonly char[] constrainedCharacters = new char[]
		{
			'~',
			'`',
			'!',
			'@',
			'#',
			'$',
			'%',
			'^',
			'&',
			'*',
			'(',
			')',
			'_',
			'+',
			'=',
			'{',
			'}',
			'[',
			']',
			'|',
			'\\',
			':',
			';',
			'"',
			'\'',
			'<',
			'>',
			',',
			'.',
			'?',
			'/'
		};

		private static readonly ADPropertyDefinition OrgName = new ADPropertyDefinition("Organization Name", ExchangeObjectVersion.Exchange2003, typeof(string), "Organization Name", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new ADObjectNameStringLengthConstraint(1, 64),
			new ContainingNonWhitespaceConstraint(),
			new ADObjectNameCharacterConstraint(OrganizationName.ConstrainedCharacters)
		}, null, null);

		private AdName adName;
	}
}
