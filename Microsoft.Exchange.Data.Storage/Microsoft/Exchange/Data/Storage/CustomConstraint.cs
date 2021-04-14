using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CustomConstraint : StoreObjectConstraint
	{
		internal CustomConstraint(string constraintDescription, PropertyDefinition[] relevantProperties, IsObjectValidDelegate validationDelegate, bool objectIsValidIfDelegateIsTrue) : base(relevantProperties)
		{
			this.constraintDescription = constraintDescription;
			this.validationDelegate = validationDelegate;
			this.propertyDefinition = relevantProperties[0];
			this.objectIsValidIfDelegateIsTrue = objectIsValidIfDelegateIsTrue;
		}

		internal CustomConstraint(CustomConstraintDelegateEnum delegateEnum, bool objectIsValidIfDelegateIsTrue) : this(delegateEnum.ToString(), CustomConstraint.delegateDictionary[delegateEnum].RelevantProperties, CustomConstraint.delegateDictionary[delegateEnum].IsObjectValidDelegate, objectIsValidIfDelegateIsTrue)
		{
		}

		internal override StoreObjectValidationError Validate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			if (this.validationDelegate(context, validatablePropertyBag) == this.objectIsValidIfDelegateIsTrue)
			{
				return null;
			}
			return new StoreObjectValidationError(context, this.propertyDefinition, validatablePropertyBag.TryGetProperty(this.propertyDefinition), this);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!this.objectIsValidIfDelegateIsTrue)
			{
				stringBuilder.Append("NOT");
			}
			stringBuilder.AppendFormat("({0})", this.constraintDescription);
			return stringBuilder.ToString();
		}

		private static Dictionary<CustomConstraintDelegateEnum, CustomConstraint.CustomConstraintAttributes> CreateDelegateDictionary()
		{
			return new Dictionary<CustomConstraintDelegateEnum, CustomConstraint.CustomConstraintAttributes>
			{
				{
					CustomConstraintDelegateEnum.IsNotConfigurationFolder,
					new CustomConstraint.CustomConstraintAttributes(new IsObjectValidDelegate(Folder.IsNotConfigurationFolder), new PropertyDefinition[]
					{
						FolderSchema.Id
					})
				},
				{
					CustomConstraintDelegateEnum.IsStartDateDefined,
					new CustomConstraint.CustomConstraintAttributes(new IsObjectValidDelegate(Task.IsStartDateDefined), new PropertyDefinition[]
					{
						InternalSchema.StartDate
					})
				},
				{
					CustomConstraintDelegateEnum.DoesFolderHaveFixedDisplayName,
					new CustomConstraint.CustomConstraintAttributes(new IsObjectValidDelegate(Folder.DoesFolderHaveFixedDisplayName), new PropertyDefinition[]
					{
						FolderSchema.Id
					})
				}
			};
		}

		private readonly IsObjectValidDelegate validationDelegate;

		private readonly PropertyDefinition propertyDefinition;

		private readonly string constraintDescription;

		private readonly bool objectIsValidIfDelegateIsTrue;

		private static readonly Dictionary<CustomConstraintDelegateEnum, CustomConstraint.CustomConstraintAttributes> delegateDictionary = CustomConstraint.CreateDelegateDictionary();

		private struct CustomConstraintAttributes
		{
			public CustomConstraintAttributes(IsObjectValidDelegate isObjectValidDelegate, PropertyDefinition[] relevantProperties)
			{
				this.IsObjectValidDelegate = isObjectValidDelegate;
				this.RelevantProperties = relevantProperties;
			}

			public readonly IsObjectValidDelegate IsObjectValidDelegate;

			public readonly PropertyDefinition[] RelevantProperties;
		}
	}
}
