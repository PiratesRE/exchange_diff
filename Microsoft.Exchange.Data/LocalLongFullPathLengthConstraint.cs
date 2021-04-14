using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class LocalLongFullPathLengthConstraint : PropertyDefinitionConstraint
	{
		private LocalLongFullPathLengthConstraint(LocalLongFullPathLengthConstraint.LocalLongFullPathLengthValidationType validationType)
		{
			this.validationType = validationType;
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			if (value != null)
			{
				try
				{
					if (this.validationType == LocalLongFullPathLengthConstraint.LocalLongFullPathLengthValidationType.Directory)
					{
						((LocalLongFullPath)value).ValidateDirectoryPathLength();
					}
					else
					{
						((LocalLongFullPath)value).ValidateFilePathLength();
					}
				}
				catch (FormatException ex)
				{
					return new PropertyConstraintViolationError(DataStrings.ConstraintViolationPathLength(value.ToString(), ex.Message), propertyDefinition, value, this);
				}
			}
			return null;
		}

		private LocalLongFullPathLengthConstraint.LocalLongFullPathLengthValidationType validationType;

		public static LocalLongFullPathLengthConstraint LocalLongFullDirectoryPathLengthConstraint = new LocalLongFullPathLengthConstraint(LocalLongFullPathLengthConstraint.LocalLongFullPathLengthValidationType.Directory);

		public static LocalLongFullPathLengthConstraint LocalLongFullFilePathLengthConstraint = new LocalLongFullPathLengthConstraint(LocalLongFullPathLengthConstraint.LocalLongFullPathLengthValidationType.File);

		private enum LocalLongFullPathLengthValidationType
		{
			Directory,
			File
		}
	}
}
