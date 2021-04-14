using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI.Tasks;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ObjectList : ExchangeUserControl
	{
		public ObjectList()
		{
			this.InitializeComponent();
			base.Name = "ObjectList";
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			this.listview = new DataListView();
			this.listview.TabIndex = 1;
			this.listview.Dock = DockStyle.Fill;
			this.listview.AllowColumnReorder = true;
			this.listview.FullRowSelect = true;
			this.listview.Name = "listView";
			this.listview.View = View.Details;
			this.listview.ShowFilter = false;
			this.filter = new FilterControl();
			this.filter.Dock = DockStyle.Top;
			this.filter.TabIndex = 0;
			this.filter.Visible = false;
			this.filter.Name = "filter";
			base.Controls.Add(this.listview);
			base.Controls.Add(this.filter);
			base.ResumeLayout();
		}

		[DefaultValue(null)]
		public object DataSource
		{
			get
			{
				return this.datasource;
			}
			set
			{
				if (this.datasource != value)
				{
					this.datasource = value;
					this.listview.DataSource = this.datasource;
					IAdvancedBindingListView advancedBindingListView = this.datasource as IAdvancedBindingListView;
					if (advancedBindingListView != null && advancedBindingListView.SupportsFiltering)
					{
						this.filter.DataSource = advancedBindingListView;
					}
					IPagedList pagedList = this.datasource as IPagedList;
					if (pagedList != null && pagedList.SupportsPaging)
					{
						this.PagingControl.Visible = true;
						this.PagingControl.DataSource = pagedList;
					}
				}
			}
		}

		public new event EventHandler DoubleClick
		{
			add
			{
				this.listview.DoubleClick += value;
			}
			remove
			{
				this.listview.DoubleClick -= value;
			}
		}

		public event EventHandler SelectionChanged
		{
			add
			{
				this.listview.SelectionChanged += value;
			}
			remove
			{
				this.listview.SelectionChanged -= value;
			}
		}

		public ListView.SelectedIndexCollection SelectedIndices
		{
			get
			{
				return this.listview.SelectedIndices;
			}
		}

		public ListView.ListViewItemCollection Items
		{
			get
			{
				return this.listview.Items;
			}
		}

		public string FilterExpression
		{
			get
			{
				return this.filter.Expression;
			}
		}

		public event EventHandler FilterExpressionChanged
		{
			add
			{
				this.filter.ExpressionChanged += value;
			}
			remove
			{
				this.filter.ExpressionChanged -= value;
			}
		}

		public DataListView ListView
		{
			get
			{
				return this.listview;
			}
		}

		public FilterControl FilterControl
		{
			get
			{
				return this.filter;
			}
		}

		public PagingControl PagingControl
		{
			get
			{
				if (this.paging == null)
				{
					this.paging = new PagingControl();
					this.paging.Dock = DockStyle.Bottom;
					this.paging.TabIndex = 2;
					this.paging.BackColor = SystemColors.Control;
					this.paging.Visible = false;
					this.paging.Name = "paging";
					base.Controls.Add(this.paging);
				}
				return this.paging;
			}
		}

		private FilterControl filter;

		private DataListView listview;

		private PagingControl paging;

		private object datasource;
	}
}
