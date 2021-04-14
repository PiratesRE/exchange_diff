using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class PropertyValidationError : ValidationError
	{
		public PropertyValidationError(LocalizedString description, PropertyDefinition propertyDefinition, object invalidData) : base(description, propertyDefinition)
		{
			this.invalidData = invalidData;
			this.propertyDefinition = propertyDefinition;
		}

		public object InvalidData
		{
			get
			{
				return this.invalidData;
			}
		}

		public PropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
		}

		public bool Equals(PropertyValidationError other)
		{
			return other != null && object.Equals(this.PropertyDefinition, other.PropertyDefinition) && object.Equals(this.InvalidData, other.InvalidData) && base.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as PropertyValidationError);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				this.hashCode = (base.GetHashCode() ^ ((this.PropertyDefinition == null) ? 0 : this.PropertyDefinition.GetHashCode()) ^ ((this.InvalidData == null) ? 0 : this.InvalidData.GetHashCode()));
			}
			return this.hashCode;
		}

		private object invalidData;

		private PropertyDefinition propertyDefinition;

		private int hashCode;
	}
}
