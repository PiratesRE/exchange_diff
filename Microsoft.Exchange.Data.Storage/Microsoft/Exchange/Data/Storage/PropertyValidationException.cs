using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class PropertyValidationException : CorruptDataException
	{
		internal PropertyValidationException(string firstPropertyError, PropertyDefinition firstFailedProperty, PropertyValidationError[] errors) : base(ServerStrings.ExPropertyValidationFailed(firstPropertyError, (firstFailedProperty == null) ? null : firstFailedProperty.ToString()))
		{
			this.propertyValidationErrors = errors;
		}

		protected PropertyValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.propertyValidationErrors = (PropertyValidationError[])info.GetValue("propertyValidationErrors", typeof(PropertyValidationError[]));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("propertyValidationErrors", this.propertyValidationErrors);
		}

		public PropertyValidationError[] PropertyValidationErrors
		{
			get
			{
				return this.propertyValidationErrors;
			}
		}

		private const string PropertyValidationErrorsLabel = "propertyValidationErrors";

		private readonly PropertyValidationError[] propertyValidationErrors;
	}
}
