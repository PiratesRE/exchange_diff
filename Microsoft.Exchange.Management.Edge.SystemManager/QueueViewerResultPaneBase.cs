using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	public abstract class QueueViewerResultPaneBase : DataListViewResultPane
	{
		public QueueViewerResultPaneBase()
		{
			this.icons = new IconLibrary();
			this.icons.Icons.Add("Filter", Icons.CreateFilter);
			this.objectList = new ObjectList();
			this.objectList.FilterControl.SupportsOrOperator = false;
			this.objectList.DataSource = null;
			this.objectList.Dock = DockStyle.Fill;
			this.objectList.ListView.IdentityProperty = "Identity";
			base.ListControl = this.objectList.ListView;
			base.FilterControl = this.objectList.FilterControl;
			this.ObjectList.FilterExpressionChanged += this.objectList_FilterExpressionChanged;
			base.Controls.Add(this.objectList);
			this.Dock = DockStyle.Fill;
		}

		private void objectList_FilterExpressionChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(this.ObjectList.FilterControl.Expression))
			{
				base.Icon = this.icons.GetIcon("Filter", -1);
				return;
			}
			base.Icon = null;
		}

		internal ObjectList ObjectList
		{
			get
			{
				return this.objectList;
			}
		}

		[DefaultValue(null)]
		internal QueueViewerDataSource Datasource
		{
			get
			{
				return this.datasource;
			}
			set
			{
				if (this.Datasource != value)
				{
					this.datasource = value;
					if (this.Datasource != null)
					{
						if (this.Datasource.GetCommandParameters.Contains("server"))
						{
							this.Datasource.GetCommandParameters.Remove("server");
						}
						if (!string.IsNullOrEmpty(this.ServerName))
						{
							this.Datasource.GetCommandParameters.AddWithValue("server", this.ServerName);
						}
						this.RefreshableDataSource = this.Datasource;
						this.SetUpDatasourceColumns();
						this.Datasource.BeginInit();
						this.Datasource.PageSize = this.pageSize;
						this.ObjectList.DataSource = this.Datasource;
						this.ObjectList.FilterControl.PersistedExpression = base.PrivateSettings.FilterExpression;
						this.Datasource.EndInit();
						base.SetRefreshWhenActivated();
					}
					this.OnDatasourceChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnDatasourceChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[QueueViewerResultPaneBase.EventDatasourceChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler DatasourceChanged
		{
			add
			{
				base.Events.AddHandler(QueueViewerResultPaneBase.EventDatasourceChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(QueueViewerResultPaneBase.EventDatasourceChanged, value);
			}
		}

		internal int PageSize
		{
			get
			{
				return this.pageSize;
			}
			set
			{
				if (value != this.pageSize)
				{
					this.pageSize = value;
					if (this.Datasource != null)
					{
						this.Datasource.PageSize = this.pageSize;
						this.RefreshDatasource();
					}
				}
			}
		}

		internal virtual string ServerName
		{
			get
			{
				return this.serverName;
			}
			set
			{
				if (value != this.serverName)
				{
					this.serverName = value;
					if (this.Datasource != null)
					{
						if (this.Datasource.GetCommandParameters.Contains("server"))
						{
							this.Datasource.GetCommandParameters.Remove("server");
						}
						if (!string.IsNullOrEmpty(this.ServerName))
						{
							this.Datasource.GetCommandParameters.AddWithValue("server", this.serverName);
						}
						this.RefreshDatasource();
					}
				}
			}
		}

		protected abstract void SetUpDatasourceColumns();

		private void RefreshDatasource()
		{
			if (this.Datasource != null && base.IsHandleCreated && !base.DesignMode)
			{
				base.SetRefreshWhenActivated();
			}
		}

		private const string FilterIconName = "Filter";

		private ObjectList objectList;

		private QueueViewerDataSource datasource;

		private IconLibrary icons;

		private static readonly object EventDatasourceChanged = new object();

		private int pageSize = 1000;

		private string serverName = string.Empty;
	}
}
