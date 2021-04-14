using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class Toolbox : ExchangeUserControl, IToolboxService
	{
		static Toolbox()
		{
			foreach (KeyValuePair<string, Type[]> keyValuePair in Toolbox.toolboxControls)
			{
				ToolboxItem value = new ToolboxItem(keyValuePair.Value[1]);
				Toolbox.toolboxItemDictionary.Add(keyValuePair.Key, value);
				ToolboxBitmapAttribute toolboxBitmapAttribute = TypeDescriptor.GetAttributes(keyValuePair.Value[0])[typeof(ToolboxBitmapAttribute)] as ToolboxBitmapAttribute;
				Bitmap bitmap = (Bitmap)toolboxBitmapAttribute.GetImage(keyValuePair.Value[0]);
				Icon icon = Icon.FromHandle(bitmap.GetHicon());
				Toolbox.iconLibrary.Icons.Add(keyValuePair.Key, icon);
			}
		}

		public Toolbox(DetailsTemplatesSurface designSurface)
		{
			this.designSurface = designSurface;
		}

		internal void Initialize()
		{
			DetailsTemplateTypeService detailsTemplateTypeService = (DetailsTemplateTypeService)this.GetService(typeof(DetailsTemplateTypeService));
			if (detailsTemplateTypeService != null)
			{
				this.dataSource = new DataTable();
				this.dataSource.Columns.Add(Toolbox.ToolNameColumn, typeof(string));
				if (!detailsTemplateTypeService.TemplateType.Equals("Mailbox Agent"))
				{
					foreach (KeyValuePair<string, Type[]> keyValuePair in Toolbox.toolboxControls)
					{
						if (!detailsTemplateTypeService.TemplateType.Equals("Search Dialog") || !Toolbox.forbiddenSearchDialogTools.Contains(keyValuePair.Key))
						{
							DataRow dataRow = this.dataSource.NewRow();
							dataRow[Toolbox.ToolNameColumn] = keyValuePair.Key;
							this.dataSource.Rows.Add(dataRow);
						}
					}
				}
				this.toolList = new DataListView();
				this.toolList.AutoGenerateColumns = false;
				this.toolList.AvailableColumns.Add(Toolbox.ToolNameColumn, Strings.ToolNameColumnName, true);
				this.toolList.IconLibrary = Toolbox.iconLibrary;
				this.toolList.ImagePropertyName = Toolbox.ToolNameColumn;
				this.toolList.IdentityProperty = Toolbox.ToolNameColumn;
				this.toolList.MultiSelect = false;
				this.toolList.ShowSelectionPropertiesCommand = null;
				this.toolList.SelectionNameProperty = Toolbox.ToolNameColumn;
				this.toolList.DataSource = this.dataSource.DefaultView;
				this.toolList.Dock = DockStyle.Fill;
				base.Controls.Add(this.toolList);
				this.toolList.MouseDown += this.toolList_MouseDown;
				this.toolList.MouseUp += this.toolList_MouseUp;
				this.toolList.SelectionChanged += this.toolList_SelectionChanged;
			}
		}

		private void StartDragEvent(string toolName)
		{
			if (toolName != null)
			{
				ToolboxItem toolboxItem = Toolbox.toolboxItemDictionary[toolName];
				if (this.mouseClicks == 1)
				{
					DataObject data = this.SerializeToolboxItem(toolboxItem) as DataObject;
					base.DoDragDrop(data, DragDropEffects.Copy);
					DetailsTemplatesSurface.SortControls(this.designSurface.TemplateTab.SelectedTab, false);
					this.mouseClicks = 0;
					return;
				}
				if (this.mouseClicks == 2)
				{
					IDesignerHost designerHost = (IDesignerHost)this.designSurface.GetService(typeof(IDesignerHost));
					ISelectionService selectionService = (designerHost == null) ? null : (designerHost.GetService(typeof(ISelectionService)) as ISelectionService);
					if (selectionService != null)
					{
						DesignerTransaction designerTransaction = null;
						try
						{
							designerTransaction = designerHost.CreateTransaction(toolboxItem.TypeName + this.designSurface.TemplateTab.Site.Name);
							Hashtable hashtable = new Hashtable();
							hashtable["Parent"] = this.designSurface.TemplateTab.SelectedTab;
							ICollection collection = toolboxItem.CreateComponents(designerHost, hashtable);
							if (collection != null && collection.Count > 0)
							{
								selectionService.SetSelectedComponents(collection, SelectionTypes.Replace);
							}
						}
						finally
						{
							if (designerTransaction != null)
							{
								designerTransaction.Commit();
							}
						}
						DetailsTemplatesSurface.SortControls(this.designSurface.TemplateTab.SelectedTab, false);
					}
					this.mouseClicks = 0;
				}
			}
		}

		private void toolList_SelectionChanged(object sender, EventArgs e)
		{
			this.StartDragEvent(this.toolList.SelectedIdentity as string);
		}

		private void toolList_MouseUp(object sender, MouseEventArgs e)
		{
			this.mouseClicks = 0;
		}

		private void toolList_MouseDown(object sender, MouseEventArgs e)
		{
			this.mouseClicks = e.Clicks;
			if (this.toolList.SelectedIndices.Count == 1 && this.toolList.GetItemRect(this.toolList.SelectedIndices[0]).Contains(e.Location))
			{
				this.StartDragEvent(this.toolList.SelectedIdentity as string);
			}
		}

		public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
		{
			return null;
		}

		public ToolboxItem GetSelectedToolboxItem()
		{
			return this.GetSelectedToolboxItem(null);
		}

		public ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost host)
		{
			return (ToolboxItem)((DataObject)serializedObject).GetData(typeof(ToolboxItem));
		}

		public ToolboxItem DeserializeToolboxItem(object serializedObject)
		{
			return this.DeserializeToolboxItem(serializedObject, null);
		}

		public object SerializeToolboxItem(ToolboxItem toolboxItem)
		{
			return new DataObject(toolboxItem);
		}

		public void SelectedToolboxItemUsed()
		{
		}

		public void AddToolboxItem(ToolboxItem toolboxItem, string category)
		{
		}

		public void AddToolboxItem(ToolboxItem toolboxItem)
		{
		}

		public bool IsToolboxItem(object serializedObject, IDesignerHost host)
		{
			return false;
		}

		public bool IsToolboxItem(object serializedObject)
		{
			return false;
		}

		public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
		{
		}

		public CategoryNameCollection CategoryNames
		{
			get
			{
				return null;
			}
		}

		public string SelectedCategory
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		void IToolboxService.Refresh()
		{
		}

		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host)
		{
		}

		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host)
		{
		}

		public bool IsSupported(object serializedObject, ICollection filterAttributes)
		{
			return false;
		}

		public bool IsSupported(object serializedObject, IDesignerHost host)
		{
			return false;
		}

		public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
		{
			return null;
		}

		public ToolboxItemCollection GetToolboxItems(string category)
		{
			return null;
		}

		public ToolboxItemCollection GetToolboxItems(IDesignerHost host)
		{
			return null;
		}

		public ToolboxItemCollection GetToolboxItems()
		{
			return null;
		}

		public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
		{
		}

		public void AddCreator(ToolboxItemCreatorCallback creator, string format)
		{
		}

		public bool SetCursor()
		{
			return false;
		}

		public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
		{
		}

		public void RemoveToolboxItem(ToolboxItem toolboxItem)
		{
		}

		public void RemoveCreator(string format, IDesignerHost host)
		{
		}

		public void RemoveCreator(string format)
		{
		}

		private DataListView toolList;

		private DetailsTemplatesSurface designSurface;

		private DataTable dataSource;

		private int mouseClicks;

		private static string ToolNameColumn = "ToolName";

		internal static string CheckboxTool = Strings.Checkbox;

		internal static string EditTool = Strings.Edit;

		internal static string GroupboxTool = Strings.Groupbox;

		internal static string LabelTool = Strings.Label;

		internal static string ListboxTool = Strings.Listbox;

		internal static string MVDropdownTool = Strings.MultiValuedDD;

		internal static string MVListboxTool = Strings.MultiValuedLB;

		private static List<string> forbiddenSearchDialogTools = new List<string>(new string[]
		{
			Toolbox.ListboxTool,
			Toolbox.CheckboxTool,
			Toolbox.MVDropdownTool,
			Toolbox.MVListboxTool
		});

		private static KeyValuePair<string, Type[]>[] toolboxControls = new KeyValuePair<string, Type[]>[]
		{
			new KeyValuePair<string, Type[]>(Toolbox.CheckboxTool, new Type[]
			{
				typeof(CheckBox),
				typeof(CustomCheckBox)
			}),
			new KeyValuePair<string, Type[]>(Toolbox.EditTool, new Type[]
			{
				typeof(TextBox),
				typeof(CustomTextBox)
			}),
			new KeyValuePair<string, Type[]>(Toolbox.GroupboxTool, new Type[]
			{
				typeof(GroupBox),
				typeof(CustomGroupBox)
			}),
			new KeyValuePair<string, Type[]>(Toolbox.LabelTool, new Type[]
			{
				typeof(Label),
				typeof(CustomLabel)
			}),
			new KeyValuePair<string, Type[]>(Toolbox.ListboxTool, new Type[]
			{
				typeof(ListBox),
				typeof(CustomListBox)
			}),
			new KeyValuePair<string, Type[]>(Toolbox.MVDropdownTool, new Type[]
			{
				typeof(ComboBox),
				typeof(CustomComboBox)
			}),
			new KeyValuePair<string, Type[]>(Toolbox.MVListboxTool, new Type[]
			{
				typeof(ListBox),
				typeof(CustomMultiValuedListBox)
			})
		};

		private static Dictionary<string, ToolboxItem> toolboxItemDictionary = new Dictionary<string, ToolboxItem>();

		private static IconLibrary iconLibrary = new IconLibrary();
	}
}
