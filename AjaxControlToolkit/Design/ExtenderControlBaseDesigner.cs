using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.Design;

namespace AjaxControlToolkit.Design
{
	public class ExtenderControlBaseDesigner<T> : ExtenderControlDesigner, IExtenderProvider where T : ExtenderControlBase
	{
		static ExtenderControlBaseDesigner()
		{
			TypeDescriptor.AddAttributes(typeof(ExtenderControlBaseDesigner<T>), new Attribute[]
			{
				new ProvidePropertyAttribute("Extender", typeof(Control))
			});
		}

		protected T ExtenderControl
		{
			get
			{
				return base.Component as T;
			}
		}

		protected virtual string ExtenderPropertyName
		{
			get
			{
				IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
				string format = "{0} ({1})";
				object[] array = new object[2];
				array[0] = TypeDescriptor.GetComponentName(base.Component);
				object[] array2 = array;
				int num = 1;
				T extenderControl = this.ExtenderControl;
				array2[num] = extenderControl.GetType().Name;
				return string.Format(invariantCulture, format, array);
			}
		}

		public bool CanExtend(object extendee)
		{
			Control control = extendee as Control;
			bool flag = false;
			if (control != null)
			{
				string id = control.ID;
				T extenderControl = this.ExtenderControl;
				flag = (id == extenderControl.TargetControlID);
				if (flag && this.renameProvider == null)
				{
					this.renameProvider = new ExtenderControlBaseDesigner<T>.ExtenderPropertyRenameDescProv(this, control);
					TypeDescriptor.AddProvider(this.renameProvider, control);
				}
			}
			return flag;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.renameProvider != null)
			{
				TypeDescriptor.RemoveProvider(this.renameProvider, base.Component);
				this.renameProvider.Dispose();
				this.renameProvider = null;
			}
			base.Dispose(disposing);
		}

		[TypeConverter(typeof(ExtenderPropertiesTypeDescriptor))]
		[Category("Extenders")]
		[Browsable(true)]
		public object GetExtender(object control)
		{
			Control control2 = control as Control;
			if (control2 != null)
			{
				return new ExtenderPropertiesProxy(this.ExtenderControl, new string[]
				{
					"TargetControlID"
				});
			}
			return null;
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);
			string[] array = new string[properties.Keys.Count];
			properties.Keys.CopyTo(array, 0);
			foreach (string text in array)
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)properties[text];
				if (text == "TargetControlID")
				{
					TargetControlTypeAttribute targetControlTypeAttribute = (TargetControlTypeAttribute)TypeDescriptor.GetAttributes(this.ExtenderControl)[typeof(TargetControlTypeAttribute)];
					if (targetControlTypeAttribute != null && !targetControlTypeAttribute.IsDefaultAttribute())
					{
						Type type = typeof(TypedControlIDConverter<>).MakeGenericType(new Type[]
						{
							targetControlTypeAttribute.TargetControlType
						});
						properties[text] = TypeDescriptor.CreateProperty(propertyDescriptor.ComponentType, propertyDescriptor, new Attribute[]
						{
							new TypeConverterAttribute(type)
						});
					}
				}
				ExtenderControlPropertyAttribute extenderControlPropertyAttribute = (ExtenderControlPropertyAttribute)propertyDescriptor.Attributes[typeof(ExtenderControlPropertyAttribute)];
				if (extenderControlPropertyAttribute != null && extenderControlPropertyAttribute.IsScriptProperty)
				{
					BrowsableAttribute browsableAttribute = (BrowsableAttribute)propertyDescriptor.Attributes[typeof(BrowsableAttribute)];
					if (browsableAttribute.Browsable == BrowsableAttribute.Yes.Browsable)
					{
						properties[text] = TypeDescriptor.CreateProperty(propertyDescriptor.ComponentType, propertyDescriptor, new Attribute[]
						{
							BrowsableAttribute.No,
							ExtenderVisiblePropertyAttribute.Yes
						});
					}
				}
			}
		}

		private ExtenderControlBaseDesigner<T>.ExtenderPropertyRenameDescProv renameProvider;

		private class ExtenderPropertyRenameDescProv : FilterTypeDescriptionProvider<IComponent>
		{
			public ExtenderPropertyRenameDescProv(ExtenderControlBaseDesigner<T> owner, IComponent target) : base(target)
			{
				this.owner = owner;
				base.FilterExtendedProperties = true;
			}

			protected override PropertyDescriptor ProcessProperty(PropertyDescriptor baseProp)
			{
				if (baseProp.Name == "Extender" && baseProp.ComponentType == this.owner.GetType() && this.owner.ExtenderPropertyName != null)
				{
					return TypeDescriptor.CreateProperty(baseProp.ComponentType, baseProp, new Attribute[]
					{
						new DisplayNameAttribute(this.owner.ExtenderPropertyName)
					});
				}
				return base.ProcessProperty(baseProp);
			}

			private ExtenderControlBaseDesigner<T> owner;
		}
	}
}
