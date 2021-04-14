using System;

namespace Microsoft.Exchange.Data.Metering
{
	internal interface ICountedEntity<TEntityType> : IEquatable<ICountedEntity<TEntityType>> where TEntityType : struct, IConvertible
	{
		IEntityName<TEntityType> GroupName { get; }

		IEntityName<TEntityType> Name { get; }
	}
}
