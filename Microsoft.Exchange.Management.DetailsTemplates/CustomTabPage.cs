using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[Designer(typeof(CustomTabPage.CustomTabDesigner))]
	internal sealed class CustomTabPage : TabPage
	{
		[DefaultValue("")]
		public override string Text
		{
			get
			{
				return this.detailsTemplatePage.Text;
			}
			set
			{
				this.detailsTemplatePage.Text = (value ?? string.Empty);
				base.Text = (value ?? string.Empty);
			}
		}

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			if (!drgevent.Data.GetDataPresent(typeof(CustomTabPage)))
			{
				base.OnDragEnter(drgevent);
			}
		}

		public int HelpContext
		{
			get
			{
				return this.detailsTemplatePage.HelpContext;
			}
			set
			{
				this.detailsTemplatePage.HelpContext = value;
			}
		}

		[Browsable(false)]
		public Page DetailsTemplateTab
		{
			get
			{
				return this.detailsTemplatePage;
			}
			set
			{
				this.detailsTemplatePage = value;
				base.Text = this.detailsTemplatePage.Text;
			}
		}

		public const string TabPageIndex = "TabPageIndex";

		private Page detailsTemplatePage = new Page();

		private sealed class CustomTabDesigner : ScrollableControlDesigner
		{
			public override SelectionRules SelectionRules
			{
				get
				{
					return this.selectionRules;
				}
			}

			public override void InitializeNewComponent(IDictionary defaultValues)
			{
				base.InitializeNewComponent(defaultValues);
				this.Control.Text = string.Empty;
				int num = 0;
				if (defaultValues != null && defaultValues["TabPageIndex"] != null)
				{
					num = (int)defaultValues["TabPageIndex"];
				}
				TabControl tabControl = (this.Control == null) ? null : (this.Control.Parent as TabControl);
				if (tabControl != null)
				{
					if (num < 0)
					{
						num = 0;
					}
					else if (num >= tabControl.TabCount)
					{
						num = tabControl.TabCount - 1;
					}
					TabPage tabPage = this.Control as TabPage;
					if (num != tabControl.TabPages.IndexOf(tabPage))
					{
						tabControl.TabPages.Remove(tabPage);
						tabControl.TabPages.Insert(num, tabPage);
					}
					tabControl.SelectedTab = tabPage;
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
			}

			private SelectionRules selectionRules;
		}
	}
}
