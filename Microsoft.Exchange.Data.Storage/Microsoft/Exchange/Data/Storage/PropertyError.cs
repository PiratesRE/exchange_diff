using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PropertyError
	{
		public PropertyError(PropertyDefinition propertyDefinition, PropertyErrorCode error, string errorDescription)
		{
			EnumValidator.ThrowIfInvalid<PropertyErrorCode>(error, "error");
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

		public static bool IsPropertyNotFound(object propertyValue)
		{
			PropertyError propertyError = propertyValue as PropertyError;
			return propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.NotFound;
		}

		public static bool IsPropertyValueTooBig(object propertyValue)
		{
			PropertyError propertyError = propertyValue as PropertyError;
			return propertyError != null && (propertyError.PropertyErrorCode == PropertyErrorCode.NotEnoughMemory || propertyError.PropertyErrorCode == PropertyErrorCode.RequireStreamed);
		}

		public static bool IsPropertyError(object propertyValue)
		{
			return propertyValue is PropertyError;
		}

		public static Exception ToException(params PropertyError[] propertyErrors)
		{
			return PropertyError.ToException(LocalizedString.Empty, propertyErrors);
		}

		public static StoragePermanentException ToException(LocalizedString message, params PropertyError[] propertyErrors)
		{
			foreach (PropertyError propertyError in propertyErrors)
			{
				if (propertyError.PropertyErrorCode == PropertyErrorCode.NotEnoughMemory && propertyError.PropertyDefinition is StorePropertyDefinition && (((StorePropertyDefinition)propertyError.PropertyDefinition).PropertyFlags & PropertyFlags.Streamable) == PropertyFlags.None)
				{
					throw new PropertyTooBigException(propertyError.PropertyDefinition);
				}
			}
			if (!(message != LocalizedString.Empty))
			{
				return PropertyErrorException.FromPropertyErrorsInternal(propertyErrors);
			}
			return PropertyErrorException.FromPropertyErrorsInternal(message, propertyErrors);
		}

		public LocalizedString ToLocalizedString()
		{
			return ServerStrings.PropertyErrorString((this.PropertyDefinition == null) ? "<null>" : this.PropertyDefinition.ToString(), this.PropertyErrorCode, this.PropertyErrorDescription);
		}

		public override string ToString()
		{
			return this.ToLocalizedString().ToString();
		}

		public override bool Equals(object obj)
		{
			PropertyError propertyError = obj as PropertyError;
			return propertyError != null && this.error == propertyError.error && this.propertyDefinition.Equals(propertyError.propertyDefinition);
		}

		public override int GetHashCode()
		{
			return (int)(this.error ^ (PropertyErrorCode)this.propertyDefinition.GetHashCode());
		}

		private readonly PropertyDefinition propertyDefinition;

		private readonly PropertyErrorCode error;

		private readonly string errorDescription;
	}
}
