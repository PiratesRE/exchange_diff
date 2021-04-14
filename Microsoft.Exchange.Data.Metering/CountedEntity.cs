using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering
{
	internal class CountedEntity<TEntityType> : ICountedEntity<TEntityType>, IEquatable<ICountedEntity<TEntityType>> where TEntityType : struct, IConvertible
	{
		public CountedEntity(IEntityName<TEntityType> groupName, IEntityName<TEntityType> name)
		{
			ArgumentValidator.ThrowIfNull("groupName", groupName);
			ArgumentValidator.ThrowIfNull("name", name);
			this.GroupName = groupName;
			this.Name = name;
		}

		public IEntityName<TEntityType> GroupName { get; private set; }

		public IEntityName<TEntityType> Name { get; private set; }

		public bool Equals(ICountedEntity<TEntityType> other)
		{
			return other != null && this.GroupName.Equals(other.GroupName) && this.Name.Equals(other.Name);
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj is ICountedEntity<TEntityType> && this.Equals(obj as ICountedEntity<TEntityType>)));
		}

		public override int GetHashCode()
		{
			return this.GroupName.GetHashCode() * 397 ^ this.Name.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("Group:{0};Name:{1}", this.GroupName, this.Name);
		}
	}
}
