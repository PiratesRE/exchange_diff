using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;

namespace AjaxControlToolkit.Design
{
	internal class ExtenderPropertiesProxy : ICustomTypeDescriptor
	{
		private object Target
		{
			get
			{
				return this.target;
			}
		}

		public ExtenderPropertiesProxy(object target, params string[] propsToHide)
		{
			this.target = target;
			this.propsToHide = propsToHide;
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(this.Target);
			if (this.propsToHide != null && this.propsToHide.Length > 0)
			{
				List<PropertyDescriptor> list = new List<PropertyDescriptor>();
				for (int i = 0; i < propertyDescriptorCollection.Count; i++)
				{
					PropertyDescriptor prop = propertyDescriptorCollection[i];
					ExtenderControlPropertyAttribute extenderControlPropertyAttribute = (ExtenderControlPropertyAttribute)prop.Attributes[typeof(ExtenderControlPropertyAttribute)];
					if (extenderControlPropertyAttribute != null)
					{
						ExtenderVisiblePropertyAttribute extenderVisiblePropertyAttribute = (ExtenderVisiblePropertyAttribute)prop.Attributes[typeof(ExtenderVisiblePropertyAttribute)];
						if (extenderVisiblePropertyAttribute != null && extenderVisiblePropertyAttribute.Value)
						{
							int num = Array.FindIndex<string>(this.propsToHide, (string s) => s == prop.Name);
							if (num == -1)
							{
								IDReferencePropertyAttribute idreferencePropertyAttribute = (IDReferencePropertyAttribute)prop.Attributes[typeof(IDReferencePropertyAttribute)];
								Attribute attribute = prop.Attributes[typeof(TypeConverterAttribute)];
								if (idreferencePropertyAttribute != null && !idreferencePropertyAttribute.IsDefaultAttribute())
								{
									Type type = typeof(TypedControlIDConverter<Control>).GetGenericTypeDefinition();
									type = type.MakeGenericType(new Type[]
									{
										idreferencePropertyAttribute.ReferencedControlType
									});
									attribute = new TypeConverterAttribute(type);
								}
								prop = TypeDescriptor.CreateProperty(prop.ComponentType, prop, new Attribute[]
								{
									BrowsableAttribute.Yes,
									attribute
								});
								list.Add(prop);
							}
						}
					}
				}
				propertyDescriptorCollection = new PropertyDescriptorCollection(list.ToArray());
			}
			return propertyDescriptorCollection;
		}

		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this.Target);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this.Target);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this.Target);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this.Target);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this.Target);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this.Target);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this.Target, editorBaseType);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this.Target, attributes);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this.Target);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return TypeDescriptor.GetProperties(this.Target);
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this.Target;
		}

		private object target;

		private string[] propsToHide;
	}
}
