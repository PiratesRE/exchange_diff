using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class UriSchemeConstraint : PropertyDefinitionConstraint
	{
		public UriSchemeConstraint(IEnumerable<string> expectedUriSchemes)
		{
			this.expectedUriSchemes = expectedUriSchemes;
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			Uri uri = value as Uri;
			if (uri != null && !this.IsExpectedScheme(uri.Scheme))
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationInvalidUriScheme(uri, this.MakeCommaSeparatedListOfAllowedSchemes()), propertyDefinition, value, this);
			}
			return null;
		}

		private string MakeCommaSeparatedListOfAllowedSchemes()
		{
			return string.Join(", ", this.expectedUriSchemes.ToArray<string>());
		}

		private bool IsExpectedScheme(string scheme)
		{
			foreach (string x in this.expectedUriSchemes)
			{
				if (StringComparer.OrdinalIgnoreCase.Compare(x, scheme) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private readonly IEnumerable<string> expectedUriSchemes;
	}
}
