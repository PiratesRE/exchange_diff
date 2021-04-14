using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class FilterValuePropertyDescriptor : PropertyDescriptor
	{
		internal FilterValuePropertyDescriptor(FilterNode owner, PropertyDescriptor pd) : base(pd)
		{
			this.owner = owner;
			this.original = pd;
		}

		public override Type ComponentType
		{
			get
			{
				return this.original.ComponentType;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return this.original.IsReadOnly;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.original.PropertyType;
			}
		}

		public Type ValuePropertyType
		{
			get
			{
				if (this.owner.PropertyDefinition != null)
				{
					return this.owner.PropertyDefinition.Type;
				}
				return this.PropertyType;
			}
		}

		public override bool CanResetValue(object component)
		{
			return this.original.CanResetValue(component);
		}

		public override object GetValue(object component)
		{
			return this.original.GetValue(component);
		}

		public override void ResetValue(object component)
		{
			this.original.ResetValue(component);
		}

		public override void SetValue(object component, object value)
		{
			this.original.SetValue(component, value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return this.original.ShouldSerializeValue(component);
		}

		private FilterNode owner;

		private PropertyDescriptor original;
	}
}
