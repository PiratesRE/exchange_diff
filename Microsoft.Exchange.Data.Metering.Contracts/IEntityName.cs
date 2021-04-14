using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface IEntityName<TEntityType> : IEquatable<IEntityName<TEntityType>> where TEntityType : struct, IConvertible
	{
		TEntityType Type { get; }

		string Value { get; }
	}
}
