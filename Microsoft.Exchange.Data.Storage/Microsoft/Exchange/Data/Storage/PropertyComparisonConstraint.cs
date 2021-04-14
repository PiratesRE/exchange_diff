using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PropertyComparisonConstraint : StoreObjectConstraint
	{
		internal PropertyComparisonConstraint(StorePropertyDefinition leftPropertyDefinition, StorePropertyDefinition rightPropertyDefinition, ComparisonOperator comparisonOperator) : base(new PropertyDefinition[]
		{
			leftPropertyDefinition,
			rightPropertyDefinition
		})
		{
			EnumValidator.AssertValid<ComparisonOperator>(comparisonOperator);
			if (comparisonOperator != ComparisonOperator.Equal && comparisonOperator != ComparisonOperator.NotEqual && !typeof(IComparable).GetTypeInfo().IsAssignableFrom(leftPropertyDefinition.Type.GetTypeInfo()))
			{
				throw new NotSupportedException(ServerStrings.ExConstraintNotSupportedForThisPropertyType);
			}
			this.leftPropertyDefinition = leftPropertyDefinition;
			this.rightPropertyDefinition = rightPropertyDefinition;
			this.comparisonOperator = comparisonOperator;
		}

		public ComparisonOperator ComparisonOperator
		{
			get
			{
				return this.comparisonOperator;
			}
		}

		public PropertyDefinition LeftPropertyDefinition
		{
			get
			{
				return this.leftPropertyDefinition;
			}
		}

		public PropertyDefinition RightPropertyDefinition
		{
			get
			{
				return this.rightPropertyDefinition;
			}
		}

		internal override StoreObjectValidationError Validate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			object obj = validatablePropertyBag.TryGetProperty(this.leftPropertyDefinition);
			object obj2 = validatablePropertyBag.TryGetProperty(this.rightPropertyDefinition);
			if (PropertyError.IsPropertyNotFound(obj) && PropertyError.IsPropertyNotFound(obj2) && (this.comparisonOperator == ComparisonOperator.Equal || this.comparisonOperator == ComparisonOperator.LessThanOrEqual || this.comparisonOperator == ComparisonOperator.GreaterThanOrEqual))
			{
				return null;
			}
			if (PropertyError.IsPropertyError(obj))
			{
				return new StoreObjectValidationError(context, this.leftPropertyDefinition, obj, this);
			}
			if (PropertyError.IsPropertyError(obj2))
			{
				return new StoreObjectValidationError(context, this.rightPropertyDefinition, obj2, this);
			}
			bool flag = false;
			switch (this.comparisonOperator)
			{
			case ComparisonOperator.Equal:
				flag = Util.ValueEquals(obj, obj2);
				break;
			case ComparisonOperator.NotEqual:
				flag = !Util.ValueEquals(obj, obj2);
				break;
			case ComparisonOperator.LessThan:
				flag = PropertyComparisonConstraint.LessThan(obj, obj2);
				break;
			case ComparisonOperator.LessThanOrEqual:
				flag = !PropertyComparisonConstraint.LessThan(obj2, obj);
				break;
			case ComparisonOperator.GreaterThan:
				flag = PropertyComparisonConstraint.LessThan(obj2, obj);
				break;
			case ComparisonOperator.GreaterThanOrEqual:
				flag = !PropertyComparisonConstraint.LessThan(obj, obj2);
				break;
			}
			if (flag)
			{
				return null;
			}
			return new StoreObjectValidationError(context, this.rightPropertyDefinition, obj2, this);
		}

		private static bool LessThan(object leftValue, object rightValue)
		{
			IComparable comparable = (IComparable)leftValue;
			return comparable.CompareTo(rightValue) < 0;
		}

		public override string ToString()
		{
			return string.Format("Property {0} must be {1} when compared to property {2}.", this.leftPropertyDefinition, this.comparisonOperator, this.rightPropertyDefinition);
		}

		private readonly ComparisonOperator comparisonOperator;

		private readonly StorePropertyDefinition leftPropertyDefinition;

		private readonly StorePropertyDefinition rightPropertyDefinition;
	}
}
