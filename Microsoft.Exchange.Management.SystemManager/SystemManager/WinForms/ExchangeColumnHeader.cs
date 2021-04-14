using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeColumnHeader : ColumnHeader
	{
		[DefaultValue(150)]
		public new int Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width = ((value > 0) ? value : 150);
			}
		}

		public ExchangeColumnHeader()
		{
			this.filterOperator2MenuItemMapping = new Hashtable();
			this.contextMenu = new ContextMenu();
			MenuItem menuItem = new MenuItem();
			MenuItem menuItem2 = new MenuItem();
			MenuItem menuItem3 = new MenuItem();
			MenuItem menuItem4 = new MenuItem();
			MenuItem menuItem5 = new MenuItem();
			MenuItem menuItem6 = new MenuItem();
			MenuItem menuItem7 = new MenuItem();
			MenuItem menuItem8 = new MenuItem();
			MenuItem menuItem9 = new MenuItem();
			menuItem.Name = "containsMenuItem";
			menuItem.Text = Strings.Contains;
			menuItem.Click += this.filterTypeMenuItem_Click;
			menuItem.Checked = true;
			menuItem.Tag = 1;
			this.filterOperator2MenuItemMapping.Add(1, menuItem);
			menuItem2.Name = "doesNotContainMenuItem";
			menuItem2.Text = Strings.DoesNotContain;
			menuItem2.Click += this.filterTypeMenuItem_Click;
			menuItem2.Tag = 2;
			this.filterOperator2MenuItemMapping.Add(2, menuItem2);
			menuItem3.Name = "startsWithMenuItem";
			menuItem3.Text = Strings.StartsWith;
			menuItem3.Click += this.filterTypeMenuItem_Click;
			menuItem3.Tag = 4;
			this.filterOperator2MenuItemMapping.Add(4, menuItem3);
			menuItem4.Name = "endsWithMenuItem";
			menuItem4.Text = Strings.EndsWith;
			menuItem4.Click += this.filterTypeMenuItem_Click;
			menuItem4.Tag = 8;
			this.filterOperator2MenuItemMapping.Add(8, menuItem4);
			menuItem5.Name = "isExactlyMenuItem";
			menuItem5.Text = Strings.IsExactly;
			menuItem5.Click += this.filterTypeMenuItem_Click;
			menuItem5.Tag = 16;
			this.filterOperator2MenuItemMapping.Add(16, menuItem5);
			menuItem6.Name = "isNotMenuItem";
			menuItem6.Text = Strings.IsNot;
			menuItem6.Click += this.filterTypeMenuItem_Click;
			menuItem6.Tag = 32;
			this.filterOperator2MenuItemMapping.Add(32, menuItem6);
			menuItem7.Name = "separatorMenuItem";
			menuItem7.Text = "-";
			menuItem8.Name = "clearFilterMenuItem";
			menuItem8.Text = Strings.ClearFilter;
			menuItem8.Click += delegate(object param0, EventArgs param1)
			{
				this.ListView.ClearFilter(base.Index);
			};
			menuItem9.Name = "clearAllFiltersMenuItem";
			menuItem9.Text = Strings.ClearAllFilters;
			menuItem9.Click += delegate(object param0, EventArgs param1)
			{
				this.ListView.ClearAllFilters();
			};
			this.contextMenu.MenuItems.AddRange(new MenuItem[]
			{
				menuItem,
				menuItem2,
				menuItem3,
				menuItem4,
				menuItem5,
				menuItem6,
				menuItem7,
				menuItem8,
				menuItem9
			});
			this.contextMenu.Name = "contextMenu";
			this.selectedMenuItem = menuItem;
			this.Width = 150;
		}

		public ExchangeColumnHeader(string name, string text, int width) : this(name, text, width, true)
		{
		}

		public ExchangeColumnHeader(string name, string text) : this(name, text, 150)
		{
		}

		public ExchangeColumnHeader(string name, string text, bool isDefault) : this(name, text, -2, isDefault, string.Empty)
		{
		}

		public ExchangeColumnHeader(string name, string text, int width, bool isDefault) : this(name, text, width, isDefault, string.Empty)
		{
		}

		public ExchangeColumnHeader(string name, string text, int width, bool isDefault, string defaultEmptyText) : this(name, text, width, isDefault, defaultEmptyText, null, null, null)
		{
		}

		public ExchangeColumnHeader(string name, string text, int width, bool isDefault, string defaultEmptyText, ICustomFormatter customFormatter, string formatString, IFormatProvider formatProvider) : this()
		{
			base.Name = name;
			base.Text = text;
			this.Width = width;
			this.Default = isDefault;
			this.DefaultEmptyText = defaultEmptyText;
			this.CustomFormatter = customFormatter;
			this.FormatString = formatString;
			this.FormatProvider = formatProvider;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ContextMenu.Dispose();
			}
			base.Dispose(disposing);
		}

		public string BindingListViewFilter
		{
			get
			{
				string filterValue = this.FilterValue;
				string result = null;
				if (!string.IsNullOrEmpty(filterValue))
				{
					string text = this.EscapeForColumnName(base.Name);
					string text2 = this.EscapeForUserDefinedValue(filterValue);
					FilterOperator filterOperator = (FilterOperator)this.selectedMenuItem.Tag;
					string format;
					if (filterOperator <= 8)
					{
						switch (filterOperator)
						{
						case 1:
							format = "{0} LIKE '*{1}*'";
							goto IL_AD;
						case 2:
							format = "{0} NOT LIKE '*{1}*'";
							goto IL_AD;
						case 3:
							break;
						case 4:
							format = "{0} LIKE '{1}*'";
							goto IL_AD;
						default:
							if (filterOperator == 8)
							{
								format = "{0} LIKE '*{1}'";
								goto IL_AD;
							}
							break;
						}
					}
					else
					{
						if (filterOperator == 16)
						{
							format = "{0} = '{1}'";
							goto IL_AD;
						}
						if (filterOperator == 32)
						{
							format = "NOT ({0} = '{1}')";
							goto IL_AD;
						}
					}
					throw new ArgumentOutOfRangeException();
					IL_AD:
					result = string.Format(CultureInfo.InvariantCulture, format, new object[]
					{
						text,
						text2
					});
				}
				return result;
			}
		}

		private string EscapeForColumnName(string name)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[");
			stringBuilder.Append(name.Replace("]", "\\]"));
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private string EscapeForUserDefinedValue(string value)
		{
			StringBuilder stringBuilder = new StringBuilder(value.Length * 3);
			int i = 0;
			while (i < value.Length)
			{
				char c = value[i];
				char c2 = c;
				switch (c2)
				{
				case '%':
					goto IL_61;
				case '&':
					goto IL_7F;
				case '\'':
					stringBuilder.Append("''");
					break;
				default:
					if (c2 == '*')
					{
						goto IL_61;
					}
					switch (c2)
					{
					case '[':
					case ']':
						goto IL_61;
					case '\\':
						goto IL_7F;
					default:
						goto IL_7F;
					}
					break;
				}
				IL_8D:
				i++;
				continue;
				IL_61:
				stringBuilder.Append("[").Append(c).Append("]");
				goto IL_8D;
				IL_7F:
				stringBuilder.Append(c.ToString());
				goto IL_8D;
			}
			return stringBuilder.ToString();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public FilterOperator FilterOperator
		{
			get
			{
				return (FilterOperator)this.selectedMenuItem.Tag;
			}
			set
			{
				if (this.FilterOperator != value)
				{
					this.filterTypeMenuItem_Click((MenuItem)this.filterOperator2MenuItemMapping[value], EventArgs.Empty);
				}
			}
		}

		public new DataListView ListView
		{
			get
			{
				return (DataListView)base.ListView;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public string FilterValue
		{
			get
			{
				return this.ListView.FilterValues[base.Index];
			}
			set
			{
				this.ListView.FilterValues[base.Index] = value;
			}
		}

		public ContextMenu ContextMenu
		{
			get
			{
				return this.contextMenu;
			}
		}

		private void filterTypeMenuItem_Click(object sender, EventArgs e)
		{
			this.selectedMenuItem.Checked = false;
			this.selectedMenuItem = (MenuItem)sender;
			this.selectedMenuItem.Checked = true;
		}

		[DefaultValue(true)]
		public bool IsSortable
		{
			get
			{
				return this.isSortable;
			}
			set
			{
				this.isSortable = value;
			}
		}

		[DefaultValue(true)]
		public bool IsReorderable
		{
			get
			{
				return this.isReorderable;
			}
			set
			{
				this.isReorderable = value;
			}
		}

		[DefaultValue(false)]
		public bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				if (this.Visible != value)
				{
					this.visible = value;
					this.OnVisibleChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnVisibleChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ExchangeColumnHeader.EventVisibleChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler VisibleChanged
		{
			add
			{
				base.Events.AddHandler(ExchangeColumnHeader.EventVisibleChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(ExchangeColumnHeader.EventVisibleChanged, value);
			}
		}

		[DefaultValue(false)]
		public bool Default
		{
			get
			{
				return this.isDefault;
			}
			set
			{
				if (this.isDefault != value)
				{
					this.isDefault = value;
					if (this.isDefault)
					{
						this.Visible = true;
						this.SetDefaultDisplayIndex();
					}
				}
			}
		}

		public void SetDefaultDisplayIndex()
		{
			if (this.Default && this.ListView != null)
			{
				this.defaultDisplayIndex = base.DisplayIndex;
				return;
			}
			this.defaultDisplayIndex = -1;
		}

		public int DefaultDisplayIndex
		{
			get
			{
				return this.defaultDisplayIndex;
			}
		}

		[DefaultValue("")]
		public string DefaultEmptyText
		{
			get
			{
				return this.defaultEmptyText;
			}
			set
			{
				this.defaultEmptyText = value;
			}
		}

		[DefaultValue(null)]
		public ICustomFormatter CustomFormatter
		{
			get
			{
				return this.customFormatter;
			}
			set
			{
				this.customFormatter = value;
			}
		}

		[DefaultValue(null)]
		public IToColorFormatter ToColorFormatter { get; set; }

		[DefaultValue(null)]
		public string FormatString
		{
			get
			{
				return this.formatString;
			}
			set
			{
				this.formatString = value;
			}
		}

		[DefaultValue(null)]
		public IFormatProvider FormatProvider
		{
			get
			{
				return this.formatProvider;
			}
			set
			{
				this.formatProvider = value;
			}
		}

		private const int defaultColumnWidth = 150;

		private const bool DefaultVisible = false;

		private ContextMenu contextMenu;

		private Hashtable filterOperator2MenuItemMapping;

		private MenuItem selectedMenuItem;

		private bool isSortable = true;

		private bool isReorderable = true;

		private bool visible;

		private static readonly object EventVisibleChanged = new object();

		private bool isDefault;

		private int defaultDisplayIndex = -1;

		private string defaultEmptyText = string.Empty;

		private ICustomFormatter customFormatter;

		private string formatString;

		private IFormatProvider formatProvider;
	}
}
