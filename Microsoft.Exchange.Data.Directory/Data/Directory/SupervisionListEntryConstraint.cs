using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class SupervisionListEntryConstraint : PropertyDefinitionConstraint
	{
		public SupervisionListEntryConstraint(bool oneOffEntry)
		{
			this.oneOffEntry = oneOffEntry;
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			ADObjectIdWithString adobjectIdWithString = value as ADObjectIdWithString;
			if (adobjectIdWithString == null)
			{
				return null;
			}
			string stringValue = adobjectIdWithString.StringValue;
			if (stringValue.Equals(string.Empty) || stringValue.IndexOf(SupervisionListEntryConstraint.Delimiter) == 0 || stringValue.LastIndexOf(SupervisionListEntryConstraint.Delimiter) == stringValue.Length - 1)
			{
				return new PropertyConstraintViolationError(DirectoryStrings.ConstraintViolationSupervisionListEntryStringPartIsInvalid, propertyDefinition, value, this);
			}
			if (this.oneOffEntry)
			{
				int num = stringValue.LastIndexOf(SupervisionListEntryConstraint.Delimiter);
				string address = stringValue.Substring(num + 1, stringValue.Length - (num + 1));
				if (num == -1 || !SmtpAddress.IsValidSmtpAddress(address))
				{
					return new PropertyConstraintViolationError(DirectoryStrings.ConstraintViolationOneOffSupervisionListEntryStringPartIsInvalid, propertyDefinition, value, this);
				}
			}
			return null;
		}

		public static readonly char Delimiter = ',';

		public static readonly char[] SupervisionTagInvalidChars = new char[]
		{
			',',
			'@'
		};

		private readonly bool oneOffEntry;
	}
}
