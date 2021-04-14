using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class RegistryObject : ConfigurableObject
	{
		public RegistryObject() : this(null)
		{
		}

		public RegistryObject(RegistryObjectId identity) : base(new SimpleProviderPropertyBag())
		{
			if (identity != null)
			{
				this.propertyBag[SimpleProviderObjectSchema.Identity] = identity;
			}
		}

		internal abstract RegistryObjectSchema RegistrySchema { get; }

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return this.RegistrySchema;
			}
		}

		internal void AddValidationError(ValidationError error)
		{
			if (this.validationErrors == null)
			{
				this.validationErrors = new List<ValidationError>();
			}
			this.validationErrors.Add(error);
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.validationErrors != null)
			{
				errors.AddRange(this.validationErrors);
			}
			base.ValidateRead(errors);
		}

		private List<ValidationError> validationErrors;
	}
}
