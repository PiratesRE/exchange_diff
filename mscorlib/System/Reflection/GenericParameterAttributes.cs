using System;

namespace System.Reflection
{
	[Flags]
	[__DynamicallyInvokable]
	public enum GenericParameterAttributes
	{
		[__DynamicallyInvokable]
		None = 0,
		[__DynamicallyInvokable]
		VarianceMask = 3,
		[__DynamicallyInvokable]
		Covariant = 1,
		[__DynamicallyInvokable]
		Contravariant = 2,
		[__DynamicallyInvokable]
		SpecialConstraintMask = 28,
		[__DynamicallyInvokable]
		ReferenceTypeConstraint = 4,
		[__DynamicallyInvokable]
		NotNullableValueTypeConstraint = 8,
		[__DynamicallyInvokable]
		DefaultConstructorConstraint = 16
	}
}
