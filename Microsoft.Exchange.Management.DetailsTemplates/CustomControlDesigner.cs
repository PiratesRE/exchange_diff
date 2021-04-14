using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal sealed class CustomControlDesigner : ControlDesigner
	{
		public override void InitializeNewComponent(IDictionary defaultValues)
		{
			base.InitializeNewComponent(defaultValues);
			this.Control.Text = string.Empty;
		}

		private DetailsTemplateControl BoundControl
		{
			get
			{
				return (this.Control as IDetailsTemplateControlBound).DetailsTemplateControl;
			}
		}

		[Browsable(false)]
		public override SelectionRules SelectionRules
		{
			get
			{
				SelectionRules result = SelectionRules.Visible | SelectionRules.Locked;
				DetailsTemplateTypeService detailsTemplateTypeService = (DetailsTemplateTypeService)this.GetService(typeof(DetailsTemplateTypeService));
				if (detailsTemplateTypeService == null || !detailsTemplateTypeService.TemplateType.Equals("Mailbox Agent"))
				{
					result = base.SelectionRules;
				}
				return result;
			}
		}

		[LocDisplayName(Strings.IDs.X)]
		[LocCategory(Strings.IDs.Layout)]
		public int DialogX
		{
			get
			{
				return this.BoundControl.X;
			}
			set
			{
				this.BoundControl.X = value;
				this.Control.Location = DetailsTemplatesSurface.DialogUnitsToPixel(this.BoundControl.X, this.BoundControl.Y, (this.ParentComponent as Control).FindForm());
			}
		}

		[LocDisplayName(Strings.IDs.Y)]
		[LocCategory(Strings.IDs.Layout)]
		public int DialogY
		{
			get
			{
				return this.BoundControl.Y;
			}
			set
			{
				this.BoundControl.Y = value;
				this.Control.Location = DetailsTemplatesSurface.DialogUnitsToPixel(this.BoundControl.X, this.BoundControl.Y, (this.ParentComponent as Control).FindForm());
			}
		}

		[LocCategory(Strings.IDs.Layout)]
		[LocDisplayName(Strings.IDs.Width)]
		public int DialogWidth
		{
			get
			{
				return this.BoundControl.Width;
			}
			set
			{
				this.BoundControl.Width = value;
				this.Control.Size = new Size(DetailsTemplatesSurface.DialogUnitsToPixel(this.BoundControl.Width, this.BoundControl.Height, (this.ParentComponent as Control).FindForm()));
			}
		}

		[LocCategory(Strings.IDs.Layout)]
		[LocDisplayName(Strings.IDs.Height)]
		public int DialogHeight
		{
			get
			{
				return this.BoundControl.Height;
			}
			set
			{
				this.BoundControl.Height = value;
				this.Control.Size = new Size(DetailsTemplatesSurface.DialogUnitsToPixel(this.BoundControl.Width, this.BoundControl.Height, (this.ParentComponent as Control).FindForm()));
			}
		}

		public int TabIndex
		{
			get
			{
				return this.Control.TabIndex;
			}
			set
			{
				this.Control.TabIndex = value;
				TabPage tabPage = this.Control.Parent as TabPage;
				DetailsTemplatesSurface.SortControls(tabPage, false);
			}
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);
			Collection<DictionaryEntry> collection = new Collection<DictionaryEntry>();
			foreach (object obj in properties)
			{
				DictionaryEntry item = (DictionaryEntry)obj;
				PropertyDescriptor propertyDescriptor = item.Value as PropertyDescriptor;
				if (propertyDescriptor.ComponentType != base.Component.GetType())
				{
					collection.Add(item);
				}
			}
			foreach (DictionaryEntry dictionaryEntry in collection)
			{
				PropertyDescriptor propertyDescriptor2 = dictionaryEntry.Value as PropertyDescriptor;
				properties[dictionaryEntry.Key] = TypeDescriptor.CreateProperty(propertyDescriptor2.ComponentType, propertyDescriptor2, new Attribute[]
				{
					BrowsableAttribute.No
				});
			}
			PropertyDescriptorCollection properties2 = TypeDescriptor.GetProperties(base.GetType());
			foreach (object obj2 in properties2)
			{
				PropertyDescriptor propertyDescriptor3 = (PropertyDescriptor)obj2;
				if (propertyDescriptor3.ComponentType == base.GetType())
				{
					properties[propertyDescriptor3.Name] = propertyDescriptor3;
				}
			}
		}
	}
}
