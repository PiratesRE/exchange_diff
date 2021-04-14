using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class Validation
	{
		internal static void Validate(IValidatable validatable, ValidationContext context)
		{
			IList<StoreObjectValidationError> list = new List<StoreObjectValidationError>();
			validatable.Validate(context, list);
			if (list.Count > 0)
			{
				throw new ObjectValidationException(ServerStrings.ExCannotSaveInvalidObject(list[0]), list);
			}
		}

		internal static StoreObjectValidationError[] CreateStoreObjectValiationErrorArray(IValidatable validatable, ValidationContext context)
		{
			IList<StoreObjectValidationError> list = new List<StoreObjectValidationError>();
			validatable.Validate(context, list);
			StoreObjectValidationError[] array;
			if (list.Count > 0)
			{
				array = new StoreObjectValidationError[list.Count];
				list.CopyTo(array, 0);
			}
			else
			{
				array = Validation.EmptyValidationErrorArray;
			}
			return array;
		}

		internal static void ValidateProperties(ValidationContext context, IValidatable validatable, IValidatablePropertyBag propertyBag, IList<StoreObjectValidationError> validationErrors)
		{
			foreach (StoreObjectConstraint storeObjectConstraint in validatable.Schema.Constraints)
			{
				bool flag = validatable.ValidateAllProperties;
				if (!flag)
				{
					foreach (PropertyDefinition propertyDefinition in storeObjectConstraint.RelevantProperties)
					{
						if (propertyBag.IsPropertyDirty(propertyDefinition))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					StoreObjectValidationError storeObjectValidationError = storeObjectConstraint.Validate(context, propertyBag);
					if (storeObjectValidationError != null)
					{
						validationErrors.Add(storeObjectValidationError);
					}
				}
			}
		}

		private static readonly StoreObjectValidationError[] EmptyValidationErrorArray = Array<StoreObjectValidationError>.Empty;
	}
}
