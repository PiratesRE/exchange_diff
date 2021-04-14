using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal abstract class PropertyDefinition
	{
		protected PropertyDefinition(string name, Type type, PropertyFlag flags)
		{
			this.name = name;
			this.type = type;
			this.flags = flags;
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal Type Type
		{
			get
			{
				return this.type;
			}
		}

		internal PropertyFlag Flags
		{
			get
			{
				return this.flags;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as PropertyDefinition);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		public virtual bool Equals(PropertyDefinition other)
		{
			return other != null && (object.ReferenceEquals(other, this) || (StringComparer.OrdinalIgnoreCase.Equals(other.Name, this.Name) && !(other.Type != this.Type) && other.Flags == this.Flags && !(other.GetType() != base.GetType())));
		}

		public override string ToString()
		{
			return string.Format("{0} ({1})", this.Name, this.Type);
		}

		private readonly string name;

		private readonly Type type;

		private readonly PropertyFlag flags;
	}
}
