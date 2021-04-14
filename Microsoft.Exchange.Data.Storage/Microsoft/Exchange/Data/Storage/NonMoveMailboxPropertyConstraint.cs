using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NonMoveMailboxPropertyConstraint : PropertyDefinitionConstraint
	{
		internal NonMoveMailboxPropertyConstraint(PropertyDefinitionConstraint constraint)
		{
			ArgumentValidator.ThrowIfNull("constraint", constraint);
			this.constraint = constraint;
		}

		public PropertyDefinitionConstraint Constraint
		{
			get
			{
				return this.constraint;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			return this.constraint.Validate(value, propertyDefinition, propertyBag);
		}

		public override PropertyConstraintViolationError Validate(ExchangeOperationContext context, object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			ArgumentValidator.ThrowIfNull("propertyDefinition", propertyDefinition);
			if (context == null || !context.IsMoveUser)
			{
				return this.Validate(value, propertyDefinition, propertyBag);
			}
			NonMoveMailboxPropertyConstraint.Tracer.TraceDebug<PropertyDefinition>((long)this.GetHashCode(), "Skipping validation of property '{0}' during move mailbox stage", propertyDefinition);
			return null;
		}

		private static readonly Trace Tracer = ExTraceGlobals.StorageTracer;

		private PropertyDefinitionConstraint constraint;
	}
}
