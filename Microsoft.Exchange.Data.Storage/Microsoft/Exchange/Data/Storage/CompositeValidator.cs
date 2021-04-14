using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CompositeValidator : IValidator
	{
		internal CompositeValidator(params IValidator[] validators)
		{
			this.validators = validators;
		}

		public bool Validate(DefaultFolderContext context, PropertyBag propertyBag)
		{
			foreach (IValidator validator in this.validators)
			{
				if (!validator.Validate(context, propertyBag))
				{
					return false;
				}
			}
			return true;
		}

		public void SetProperties(DefaultFolderContext context, Folder folder)
		{
			foreach (IValidator validator in this.validators)
			{
				validator.SetProperties(context, folder);
			}
		}

		private readonly IValidator[] validators;
	}
}
