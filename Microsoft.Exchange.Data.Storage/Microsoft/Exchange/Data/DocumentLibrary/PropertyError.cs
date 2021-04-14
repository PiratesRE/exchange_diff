using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyError
	{
		public PropertyError(PropertyDefinition propertyDefinition, PropertyErrorCode error, string errorDescription)
		{
			this.propertyDefinition = propertyDefinition;
			this.error = error;
			this.errorDescription = errorDescription;
		}

		public PropertyError(PropertyDefinition propertyDefinition, PropertyErrorCode error) : this(propertyDefinition, error, string.Empty)
		{
		}

		public PropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
		}

		public PropertyErrorCode PropertyErrorCode
		{
			get
			{
				return this.error;
			}
		}

		public string PropertyErrorDescription
		{
			get
			{
				return this.errorDescription;
			}
		}

		public override string ToString()
		{
			return string.Format("Property = {0}, PropertyErrorCode = {1}, PropertyErrorCode = {2}", (this.PropertyDefinition == null) ? "<null>" : this.PropertyDefinition.ToString(), this.PropertyErrorCode, this.PropertyErrorDescription);
		}

		private const string StringValue = "Property = {0}, PropertyErrorCode = {1}, PropertyErrorCode = {2}";

		private readonly PropertyDefinition propertyDefinition;

		private readonly PropertyErrorCode error;

		private readonly string errorDescription;
	}
}
