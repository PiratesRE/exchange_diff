using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class PropertyConversionError : PropertyValidationError
	{
		public PropertyConversionError(LocalizedString description, PropertyDefinition propertyDefinition, object invalidData, Exception exception) : base(description, propertyDefinition, invalidData)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			this.exception = exception;
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
		}

		public bool Equals(PropertyConversionError other)
		{
			return other != null && object.Equals(this.Exception, other.Exception) && base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as PropertyConversionError);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.Exception.GetHashCode();
		}

		private Exception exception;
	}
}
