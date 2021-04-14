using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class CustomUserControl : UserControl, ICustomTypeDescriptor
	{
		public CustomUserControl()
		{
			base.Name = "CustomUserControl";
			this.VisualEffectsLinearGradientMode = LinearGradientMode.Vertical;
			if (this.EnableVisualEffects)
			{
				Extensions.EnsureDoubleBuffer(this);
			}
			base.ControlAdded += this.CustomUserControl_ControlAdded;
			Theme.UseVisualEffectsChanged += this.Theme_UseVisualEffectsChanged;
		}

		public virtual PropertyDescriptorCollection GetCustomProperties(Attribute[] attributes)
		{
			return new PropertyDescriptorCollection(null);
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			foreach (object obj in TypeDescriptor.GetProperties(this, attributes, true))
			{
				PropertyDescriptor value = (PropertyDescriptor)obj;
				propertyDescriptorCollection.Add(value);
			}
			foreach (object obj2 in this.GetCustomProperties(attributes))
			{
				PropertyDescriptor value2 = (PropertyDescriptor)obj2;
				propertyDescriptorCollection.Add(value2);
			}
			return propertyDescriptorCollection;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor propertyDescriptor)
		{
			return this;
		}

		private Color VisualEffectsLinearGradientBeginColor
		{
			get
			{
				if (this.VisualEffectsLinearGradientMode == LinearGradientMode.Horizontal)
				{
					return ControlPaint.LightLight(this.BackColor);
				}
				if (this.VisualEffectsLinearGradientMode == LinearGradientMode.Vertical)
				{
					return this.BackColor;
				}
				return Color.Empty;
			}
		}

		private Color VisualEffectsLinearGradientEndColor
		{
			get
			{
				if (this.VisualEffectsLinearGradientMode == LinearGradientMode.Horizontal)
				{
					return this.BackColor;
				}
				if (this.VisualEffectsLinearGradientMode == LinearGradientMode.Vertical)
				{
					return ControlPaint.LightLight(this.BackColor);
				}
				return Color.Empty;
			}
		}

		[DefaultValue(LinearGradientMode.Vertical)]
		public LinearGradientMode VisualEffectsLinearGradientMode
		{
			get
			{
				return this.visualEffectsLinearGradientMode;
			}
			set
			{
				if (value != LinearGradientMode.Vertical && value != LinearGradientMode.Horizontal)
				{
					throw new NotSupportedException();
				}
				if (this.visualEffectsLinearGradientMode != value)
				{
					this.visualEffectsLinearGradientMode = value;
					this.Theme_UseVisualEffectsChanged(this, EventArgs.Empty);
				}
			}
		}

		[DefaultValue(false)]
		public bool EnableVisualEffects
		{
			get
			{
				return this.enableVisualEffects;
			}
			set
			{
				if (this.enableVisualEffects != value)
				{
					this.enableVisualEffects = value;
					Extensions.SetDoubleBuffer(this, value);
					this.Theme_UseVisualEffectsChanged(this, EventArgs.Empty);
				}
			}
		}

		private void Theme_UseVisualEffectsChanged(object sender, EventArgs e)
		{
			base.Invalidate(true);
		}

		private void CustomUserControl_ControlAdded(object sender, ControlEventArgs e)
		{
			if (this.EnableVisualEffects)
			{
				Extensions.EnsureDoubleBuffer(e.Control);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Theme.UseVisualEffectsChanged -= this.Theme_UseVisualEffectsChanged;
			}
			base.Dispose(disposing);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (Theme.UseVisualEffects && this.EnableVisualEffects && base.Width != 0 && base.Height != 0)
			{
				Rectangle rect = new Rectangle(0, 0, base.Width, base.Height);
				using (LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect, this.VisualEffectsLinearGradientBeginColor, this.VisualEffectsLinearGradientEndColor, this.VisualEffectsLinearGradientMode))
				{
					if (LayoutHelper.IsRightToLeft(this) && this.VisualEffectsLinearGradientMode == LinearGradientMode.Horizontal)
					{
						linearGradientBrush.RotateTransform(180f);
					}
					e.Graphics.FillRectangle(linearGradientBrush, rect);
				}
			}
			base.OnPaint(e);
		}

		private LinearGradientMode visualEffectsLinearGradientMode;

		private bool enableVisualEffects;
	}
}
