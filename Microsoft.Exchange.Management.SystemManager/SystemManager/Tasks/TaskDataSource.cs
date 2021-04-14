using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementGUI.Tasks;

namespace Microsoft.Exchange.Management.SystemManager.Tasks
{
	public class TaskDataSource : DataTableLoader, IPagedList, IAdvancedBindingListView, IBindingListView, IBindingList, IList, ICollection, IEnumerable, ITypedList
	{
		public TaskDataSource()
		{
			this.defaultTable = new DataTable();
			base.Table = this.defaultTable;
			base.Table.Locale = CultureInfo.InvariantCulture;
			this.InnerList.AllowEdit = false;
			this.InnerList.AllowNew = false;
			this.InnerList.AllowDelete = false;
			this.selectCommand = new LoggableMonadCommand("Get-");
			this.selectCommand.Parameters.AddWithValue("ResultSize", 1000);
			this.selectCommand.Parameters.AddWithValue("ReturnPageInfo", true);
			base.Table.ExtendedProperties["TotalCount"] = 0;
			base.Table.ExtendedProperties["Position"] = 0;
			this.sortOrder = new ArrayList();
			this.PreparePage(TaskDataSource.PageChangeType.FirstPage, 0);
			base.RefreshArgument = this.DefaultRefreshArgument;
		}

		public TaskDataSource(string noun) : this()
		{
			this.Noun = noun;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.dataView.Dispose();
				this.defaultTable.Dispose();
			}
			base.Dispose(disposing);
		}

		protected sealed override DataTable DefaultTable
		{
			get
			{
				return this.defaultTable;
			}
		}

		protected override void OnTableChanged(EventArgs e)
		{
			if (base.Table == null)
			{
				base.Table = this.dataView.Table;
				throw new ArgumentNullException("dataTable");
			}
			this.InnerList = new DataView(base.Table);
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			base.OnTableChanged(e);
		}

		private protected DataView InnerList
		{
			protected get
			{
				return this.dataView;
			}
			private set
			{
				if (this.dataView != null)
				{
					this.dataView.ListChanged -= this.InnerList_ListChanged;
				}
				this.dataView = value;
				if (this.dataView != null)
				{
					this.dataView.ListChanged += this.InnerList_ListChanged;
				}
			}
		}

		private void InnerList_ListChanged(object sender, ListChangedEventArgs e)
		{
			this.OnListChanged(e);
		}

		public void Refresh()
		{
			if (base.IsInitialized)
			{
				base.Refresh(base.CreateProgress(base.RefreshCommandText));
			}
		}

		internal MonadParameterCollection GetCommandParameters
		{
			get
			{
				return this.selectCommand.Parameters;
			}
		}

		protected override ICloneable DefaultRefreshArgument
		{
			get
			{
				return this.selectCommand;
			}
		}

		[DefaultValue("")]
		public string Noun
		{
			get
			{
				return this.noun;
			}
			set
			{
				if (value != this.Noun)
				{
					this.noun = value;
					this.selectCommand.CommandText = "get-" + this.Noun;
				}
			}
		}

		protected override void OnRefreshStarting(CancelEventArgs e)
		{
			if (this.SupportsFiltering)
			{
				this.selectCommand.Parameters.Remove("Filter");
				if (!string.IsNullOrEmpty(this.Filter))
				{
					this.selectCommand.Parameters.AddWithValue("Filter", this.Filter);
				}
			}
			if (!this.pagingParametersChanged)
			{
				this.PreparePage(TaskDataSource.PageChangeType.ReloadPage, -1);
			}
			this.pagingParametersChanged = false;
			base.OnRefreshStarting(e);
		}

		protected override void OnRefreshingChanged(EventArgs e)
		{
			this.Filtering = base.Refreshing;
			base.OnRefreshingChanged(e);
		}

		protected override bool TryToGetPartialRefreshArgument(object[] ids, out object partialRefreshArgument)
		{
			if (1 != ids.Length)
			{
				partialRefreshArgument = null;
				return false;
			}
			MonadCommand monadCommand = this.selectCommand.Clone();
			monadCommand.Parameters.Clear();
			monadCommand.Parameters.AddWithValue("Identity", ids[0]);
			partialRefreshArgument = monadCommand;
			return true;
		}

		private void InternalSort()
		{
			StringBuilder stringBuilder = new StringBuilder();
			QueueViewerSortOrderEntry[] array = (QueueViewerSortOrderEntry[])this.SortOrder.ToArray(typeof(QueueViewerSortOrderEntry));
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString());
				if (i != array.Length - 1)
				{
					stringBuilder.Append(",");
				}
			}
			this.selectCommand.Parameters.Remove("SortOrder");
			this.selectCommand.Parameters.AddWithValue("SortOrder", array);
			this.PreparePage(TaskDataSource.PageChangeType.ReloadPage, -1);
			this.Refresh();
		}

		public bool SupportsPaging
		{
			get
			{
				return true;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CurrentItem
		{
			get
			{
				return (int)base.Table.ExtendedProperties["Position"];
			}
			set
			{
				this.PreparePage(TaskDataSource.PageChangeType.PageToIndex, value);
				this.Refresh();
			}
		}

		public int ItemsInCurrentPage
		{
			get
			{
				return base.Table.Rows.Count;
			}
		}

		public int TotalItems
		{
			get
			{
				return (int)base.Table.ExtendedProperties["TotalCount"];
			}
		}

		[DefaultValue(1000)]
		public int PageSize
		{
			get
			{
				return base.ExpectedResultSize;
			}
			set
			{
				base.ExpectedResultSize = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int DefaultExpectedResultSize
		{
			get
			{
				return 1000;
			}
		}

		protected override void OnExpectedResultSizeChanged(EventArgs e)
		{
			base.OnExpectedResultSizeChanged(e);
			this.selectCommand.Parameters["ResultSize"].Value = base.ExpectedResultSize;
			this.PreparePage(TaskDataSource.PageChangeType.ReloadPage, -1);
			this.Refresh();
		}

		public void GoToFirstPage()
		{
			this.PreparePage(TaskDataSource.PageChangeType.FirstPage, 0);
			this.Refresh();
		}

		public void GoToLastPage()
		{
			this.PreparePage(TaskDataSource.PageChangeType.LastPage, 0);
			this.Refresh();
		}

		public void GoToNextPage()
		{
			this.PreparePage(TaskDataSource.PageChangeType.NextPage, 0);
			this.Refresh();
		}

		public void GoToPreviousPage()
		{
			this.PreparePage(TaskDataSource.PageChangeType.PreviousPage, 0);
			this.Refresh();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ConfigObject Bookmark
		{
			get
			{
				return this.bookmark;
			}
			set
			{
				this.bookmark = value;
			}
		}

		private bool IncludeBookmark
		{
			get
			{
				return (bool)this.selectCommand.Parameters["IncludeBookmark"].Value;
			}
			set
			{
				this.selectCommand.Parameters["IncludeBookmark"].Value = value;
			}
		}

		private void PreparePage(TaskDataSource.PageChangeType type, int index)
		{
			if (type == TaskDataSource.PageChangeType.PageToIndex && index < 0)
			{
				throw new ArgumentException("Index must be greater or equal to 0.", "index");
			}
			switch (type)
			{
			case TaskDataSource.PageChangeType.PageToIndex:
				this.ClearPagingParameters();
				this.selectCommand.Parameters.AddWithValue("SearchForward", true);
				this.selectCommand.Parameters.AddWithValue("BookmarkObject", null);
				this.selectCommand.Parameters.AddWithValue("BookmarkIndex", index);
				this.selectCommand.Parameters.AddWithValue("IncludeBookmark", true);
				break;
			case TaskDataSource.PageChangeType.FirstPage:
				this.ClearPagingParameters();
				this.selectCommand.Parameters.AddWithValue("SearchForward", true);
				this.selectCommand.Parameters.AddWithValue("BookmarkObject", null);
				this.selectCommand.Parameters.AddWithValue("BookmarkIndex", -1);
				this.selectCommand.Parameters.AddWithValue("IncludeBookmark", false);
				break;
			case TaskDataSource.PageChangeType.LastPage:
				this.ClearPagingParameters();
				this.selectCommand.Parameters.AddWithValue("SearchForward", false);
				this.selectCommand.Parameters.AddWithValue("BookmarkObject", null);
				this.selectCommand.Parameters.AddWithValue("BookmarkIndex", -1);
				this.selectCommand.Parameters.AddWithValue("IncludeBookmark", false);
				break;
			case TaskDataSource.PageChangeType.NextPage:
				this.ClearPagingParameters();
				this.selectCommand.Parameters.AddWithValue("SearchForward", true);
				this.selectCommand.Parameters.AddWithValue("BookmarkObject", base.Table.ExtendedProperties["BookmarkNext"]);
				this.selectCommand.Parameters.AddWithValue("BookmarkIndex", -1);
				this.selectCommand.Parameters.AddWithValue("IncludeBookmark", false);
				break;
			case TaskDataSource.PageChangeType.PreviousPage:
				this.ClearPagingParameters();
				this.selectCommand.Parameters.AddWithValue("SearchForward", false);
				this.selectCommand.Parameters.AddWithValue("BookmarkObject", base.Table.ExtendedProperties["BookmarkPrevious"]);
				this.selectCommand.Parameters.AddWithValue("BookmarkIndex", -1);
				this.selectCommand.Parameters.AddWithValue("IncludeBookmark", false);
				break;
			case TaskDataSource.PageChangeType.ReloadPage:
				if (base.Table.ExtendedProperties["BookmarkPrevious"] == null)
				{
					this.PreparePage(TaskDataSource.PageChangeType.FirstPage, 0);
				}
				else
				{
					this.ClearPagingParameters();
					this.selectCommand.Parameters.AddWithValue("SearchForward", true);
					this.selectCommand.Parameters.AddWithValue("BookmarkObject", base.Table.ExtendedProperties["BookmarkPrevious"]);
					this.selectCommand.Parameters.AddWithValue("BookmarkIndex", -1);
					this.selectCommand.Parameters.AddWithValue("IncludeBookmark", true);
				}
				break;
			}
			this.pagingParametersChanged = true;
		}

		private void ClearPagingParameters()
		{
			this.selectCommand.Parameters.Remove("SearchForward");
			this.selectCommand.Parameters.Remove("BookmarkObject");
			this.selectCommand.Parameters.Remove("BookmarkIndex");
			this.selectCommand.Parameters.Remove("IncludeBookmark");
		}

		public bool IsSortSupported(string propertyName)
		{
			return true;
		}

		public bool IsFilterSupported(string propertyName)
		{
			return true;
		}

		public virtual void ApplyFilter(QueryFilter filter)
		{
			this.QueryFilter = filter;
			this.ApplyFilter();
		}

		private void ApplyFilter()
		{
			this.filter = ((this.QueryFilter != null) ? this.QueryFilter.GenerateInfixString(FilterLanguage.Monad) : string.Empty);
			this.Refresh();
		}

		public QueryFilter QueryFilter { get; private set; }

		public bool Filtering
		{
			get
			{
				return this.filtering;
			}
			private set
			{
				if (this.Filtering != value)
				{
					this.filtering = value;
					this.OnFilteringChanged(EventArgs.Empty);
				}
			}
		}

		public event EventHandler FilteringChanged
		{
			add
			{
				base.Events.AddHandler(TaskDataSource.EventFilteringChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(TaskDataSource.EventFilteringChanged, value);
			}
		}

		protected virtual void OnFilteringChanged(EventArgs e)
		{
			if (base.Events[TaskDataSource.EventFilteringChanged] != null)
			{
				((EventHandler)base.Events[TaskDataSource.EventFilteringChanged])(this, e);
			}
		}

		public bool SupportCancelFiltering
		{
			get
			{
				return true;
			}
		}

		public void CancelFiltering()
		{
			if (this.Filtering)
			{
				base.CancelRefresh();
			}
		}

		public bool SupportsFiltering
		{
			get
			{
				return true;
			}
		}

		[DefaultValue("")]
		public string Filter
		{
			get
			{
				return this.filter;
			}
			set
			{
				throw new NotSupportedException("Do not support set TaskDataSource.Filter, please set QueryFilter");
			}
		}

		public void RemoveFilter()
		{
			this.ApplyFilter(null);
		}

		public bool SupportsAdvancedSorting
		{
			get
			{
				return true;
			}
		}

		public ListSortDescriptionCollection SortDescriptions
		{
			get
			{
				return this.sortDescriptions;
			}
		}

		public void ApplySort(ListSortDescriptionCollection sorts)
		{
			if (!this.SupportsAdvancedSorting)
			{
				throw new NotSupportedException();
			}
			this.OnApplySort(sorts);
		}

		protected virtual void OnApplySort(ListSortDescriptionCollection sorts)
		{
			this.sortOrder.Clear();
			this.sortDescriptions = sorts;
			foreach (object obj in ((IEnumerable)sorts))
			{
				ListSortDescription listSortDescription = (ListSortDescription)obj;
				this.sortOrder.Add(new QueueViewerSortOrderEntry(listSortDescription.PropertyDescriptor.Name, listSortDescription.SortDirection));
			}
			this.InternalSort();
		}

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			((IBindingList)this.InnerList).AddIndex(property);
		}

		object IBindingList.AddNew()
		{
			return ((IBindingList)this.InnerList).AddNew();
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return ((IBindingList)this.InnerList).AllowEdit;
			}
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return ((IBindingList)this.InnerList).AllowNew;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return ((IBindingList)this.InnerList).AllowRemove;
			}
		}

		public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			this.sortProperty = property;
			this.sortDirection = direction;
			this.OnApplySort();
		}

		protected virtual void OnApplySort()
		{
			QueueViewerSortOrderEntry value = new QueueViewerSortOrderEntry(this.SortProperty.Name, this.SortDirection);
			this.sortOrder.Clear();
			this.sortOrder.Add(value);
			this.InternalSort();
		}

		protected ArrayList SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			return ((IBindingList)this.InnerList).Find(property, key);
		}

		bool IBindingList.IsSorted
		{
			get
			{
				return null != this.sortProperty;
			}
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			ListChangedEventHandler listChangedEventHandler = (ListChangedEventHandler)base.Events[TaskDataSource.EventListChanged];
			if (listChangedEventHandler != null)
			{
				listChangedEventHandler(this, e);
			}
		}

		public event ListChangedEventHandler ListChanged
		{
			add
			{
				base.Events.AddHandler(TaskDataSource.EventListChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(TaskDataSource.EventListChanged, value);
			}
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			((IBindingList)this.InnerList).RemoveIndex(property);
		}

		public void RemoveSort()
		{
			this.sortProperty = null;
			this.sortDirection = ListSortDirection.Ascending;
			this.OnRemoveSort();
		}

		protected virtual void OnRemoveSort()
		{
			this.sortDescriptions = null;
			this.SortOrder.Clear();
			this.InternalSort();
		}

		public ListSortDirection SortDirection
		{
			get
			{
				return this.sortDirection;
			}
		}

		public PropertyDescriptor SortProperty
		{
			get
			{
				return this.sortProperty;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return true;
			}
		}

		int IList.Add(object value)
		{
			throw new NotSupportedException();
		}

		void IList.Clear()
		{
			throw new NotSupportedException();
		}

		bool IList.Contains(object value)
		{
			return ((IList)this.InnerList).Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return ((IList)this.InnerList).IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		bool IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.InnerList[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		void IList.Remove(object value)
		{
			throw new NotSupportedException();
		}

		void IList.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.InnerList.CopyTo(array, index);
		}

		public int Count
		{
			get
			{
				return this.InnerList.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)this.InnerList).IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this.InnerList).SyncRoot;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.InnerList).GetEnumerator();
		}

		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return ((ITypedList)this.InnerList).GetItemProperties(listAccessors);
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			return ((ITypedList)this.InnerList).GetListName(listAccessors);
		}

		private const int DefaultPageSize = 1000;

		private DataTable defaultTable;

		private DataView dataView;

		private MonadCommand selectCommand;

		private string noun = "";

		private bool pagingParametersChanged;

		private ConfigObject bookmark;

		private bool filtering;

		private static object EventFilteringChanged = new object();

		private string filter = "";

		private ListSortDescriptionCollection sortDescriptions;

		private ArrayList sortOrder;

		private static readonly object EventListChanged = new object();

		private ListSortDirection sortDirection;

		private PropertyDescriptor sortProperty;

		private enum PageChangeType
		{
			PageToIndex,
			FirstPage,
			LastPage,
			NextPage,
			PreviousPage,
			ReloadPage
		}
	}
}
