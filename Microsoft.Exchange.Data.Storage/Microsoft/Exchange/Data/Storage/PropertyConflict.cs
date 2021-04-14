using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyConflict
	{
		public PropertyConflict(PropertyDefinition propertyDefinition, object originalValue, object clientValue, object serverValue, object resolvedValue, bool resolvable)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException();
			}
			this.PropertyDefinition = propertyDefinition;
			this.OriginalValue = originalValue;
			this.ClientValue = clientValue;
			this.ServerValue = serverValue;
			this.ResolvedValue = resolvedValue;
			this.ConflictResolvable = resolvable;
		}

		public override string ToString()
		{
			return string.Format("Property = {0}, values = <{1}, {2}, {3}>, ResolvedValue = {4}, ConflictResolvable = {5}.", new object[]
			{
				this.PropertyDefinition,
				this.OriginalValue,
				this.ClientValue,
				this.ServerValue,
				this.ResolvedValue,
				this.ConflictResolvable
			});
		}

		public readonly PropertyDefinition PropertyDefinition;

		public readonly bool ConflictResolvable;

		public readonly object OriginalValue;

		public readonly object ServerValue;

		public readonly object ClientValue;

		public readonly object ResolvedValue;
	}
}
