using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PropertyErrorException : DocumentLibraryException
	{
		internal PropertyErrorException(PropertyError propertyError) : this(propertyError, null)
		{
		}

		internal PropertyErrorException(PropertyError propertyError, Exception innerException) : base(propertyError.PropertyErrorDescription, innerException)
		{
			this.propertyError = propertyError;
		}

		public PropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyError.PropertyDefinition;
			}
		}

		public PropertyErrorCode PropertyErrorCode
		{
			get
			{
				return this.propertyError.PropertyErrorCode;
			}
		}

		internal static PropertyErrorException GetExceptionFromError(PropertyError propertyError)
		{
			switch (propertyError.PropertyErrorCode)
			{
			case PropertyErrorCode.NotFound:
				return new PropertyErrorNotFoundException(propertyError);
			case PropertyErrorCode.NotSupported:
				return new PropertyErrorNotSupportedException(propertyError);
			case PropertyErrorCode.CorruptData:
				return new PropertyErrorCorruptDataException(propertyError);
			}
			return new PropertyErrorException(propertyError);
		}

		private readonly PropertyError propertyError;
	}
}
