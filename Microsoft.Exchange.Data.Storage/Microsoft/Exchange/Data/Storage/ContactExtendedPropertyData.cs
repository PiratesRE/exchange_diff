using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContactExtendedPropertyData : IEquatable<ContactExtendedPropertyData>
	{
		public ContactExtendedPropertyData(PropertyDefinition definition, object rawValue)
		{
			this.definition = definition;
			this.rawValue = rawValue;
		}

		public PropertyDefinition PropertyDefinition
		{
			get
			{
				return this.definition;
			}
		}

		public object RawValue
		{
			get
			{
				return this.rawValue;
			}
		}

		public bool Equals(ContactExtendedPropertyData other)
		{
			return other != null && other.definition == this.definition && other.rawValue.ToString().Equals(this.rawValue.ToString());
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as ContactExtendedPropertyData);
		}

		public override int GetHashCode()
		{
			return this.definition.GetHashCode() ^ this.rawValue.GetHashCode();
		}

		private readonly PropertyDefinition definition;

		private readonly object rawValue;
	}
}
