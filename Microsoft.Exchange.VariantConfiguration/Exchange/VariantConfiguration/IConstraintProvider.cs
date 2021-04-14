using System;

namespace Microsoft.Exchange.VariantConfiguration
{
	public interface IConstraintProvider
	{
		ConstraintCollection Constraints { get; }

		string RampId { get; }

		string RotationId { get; }
	}
}
