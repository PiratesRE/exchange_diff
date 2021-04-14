using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class UriKindConstraint : PropertyDefinitionConstraint
	{
		public UriKindConstraint(UriKind expectedUriKind)
		{
			this.expectedUriKind = expectedUriKind;
		}

		public UriKind ExpectedUriKind
		{
			get
			{
				return this.expectedUriKind;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			Uri uri = value as Uri;
			if (uri != null && !this.IsUriOfExpectedKind(uri))
			{
				return new PropertyConstraintViolationError(DataStrings.ConstraintViolationInvalidUriKind(uri, this.expectedUriKind), propertyDefinition, value, this);
			}
			return null;
		}

		private bool IsUriOfExpectedKind(Uri uri)
		{
			if (this.expectedUriKind == UriKind.Absolute)
			{
				return uri.IsAbsoluteUri;
			}
			return this.expectedUriKind != UriKind.Relative || !uri.IsAbsoluteUri;
		}

		private readonly UriKind expectedUriKind;
	}
}
