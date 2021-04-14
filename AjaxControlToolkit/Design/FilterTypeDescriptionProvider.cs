using System;
using System.ComponentModel;

namespace AjaxControlToolkit.Design
{
	internal class FilterTypeDescriptionProvider<T> : TypeDescriptionProvider, ICustomTypeDescriptor
	{
		public FilterTypeDescriptionProvider(T target) : base(TypeDescriptor.GetProvider(target))
		{
			this.target = target;
			this.baseProvider = TypeDescriptor.GetProvider(target);
		}

		protected T Target
		{
			get
			{
				return this.target;
			}
		}

		private ICustomTypeDescriptor BaseDescriptor
		{
			get
			{
				if (this.baseDescriptor == null)
				{
					if (this.FilterExtendedProperties)
					{
						this.baseDescriptor = this.baseProvider.GetExtendedTypeDescriptor(this.Target);
					}
					else
					{
						this.baseDescriptor = this.baseProvider.GetTypeDescriptor(this.Target);
					}
				}
				return this.baseDescriptor;
			}
		}

		protected bool FilterExtendedProperties
		{
			get
			{
				return this.extended;
			}
			set
			{
				this.extended = value;
			}
		}

		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			if (this.FilterExtendedProperties || instance != this.Target)
			{
				return this.baseProvider.GetTypeDescriptor(objectType, instance);
			}
			return this;
		}

		public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
		{
			if (this.FilterExtendedProperties && instance == this.Target)
			{
				return this;
			}
			return this.baseProvider.GetExtendedTypeDescriptor(instance);
		}

		protected virtual PropertyDescriptor ProcessProperty(PropertyDescriptor baseProp)
		{
			return baseProp;
		}

		public void Dispose()
		{
			this.target = default(T);
			this.baseDescriptor = null;
			this.baseProvider = null;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return this.FilterProperties(this.BaseDescriptor.GetProperties());
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = this.BaseDescriptor.GetProperties(attributes);
			return this.FilterProperties(properties);
		}

		private PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection props)
		{
			PropertyDescriptor[] array = new PropertyDescriptor[props.Count];
			props.CopyTo(array, 0);
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyDescriptor propertyDescriptor = this.ProcessProperty(array[i]);
				if (propertyDescriptor != array[i])
				{
					flag = true;
					array[i] = propertyDescriptor;
				}
			}
			if (flag)
			{
				props = new PropertyDescriptorCollection(array);
			}
			return props;
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return this.BaseDescriptor.GetAttributes();
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return this.BaseDescriptor.GetClassName();
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return this.BaseDescriptor.GetComponentName();
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return this.BaseDescriptor.GetConverter();
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return this.BaseDescriptor.GetDefaultEvent();
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return this.BaseDescriptor.GetDefaultProperty();
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return this.BaseDescriptor.GetEditor(editorBaseType);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return this.BaseDescriptor.GetEvents(attributes);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return this.BaseDescriptor.GetEvents();
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this.BaseDescriptor.GetPropertyOwner(pd);
		}

		private T target;

		private ICustomTypeDescriptor baseDescriptor;

		private TypeDescriptionProvider baseProvider;

		private bool extended;
	}
}
