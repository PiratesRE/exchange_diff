using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class DataTableLoaderView : Component, IAdvancedBindingListView, IBindingListView, IBindingList, IList, ICollection, IEnumerable, ITypedList
	{
		public DataTableLoaderView()
		{
		}

		public DataTableLoaderView(DataTableLoader dataTableLoader, IObjectComparer defaultObjectComparer, ObjectToFilterableConverter defaultObjectToFilterableConverter)
		{
			if (dataTableLoader == null)
			{
				throw new ArgumentNullException("dataTableLoader");
			}
			if (dataTableLoader.Table == null)
			{
				throw new ArgumentException("dataTableLoader.Table is null");
			}
			if (defaultObjectComparer == null)
			{
				throw new ArgumentNullException("defaultObjectComparer");
			}
			if (defaultObjectToFilterableConverter == null)
			{
				throw new ArgumentNullException("defaultObjectToFilterableConverter");
			}
			this.DataTableLoader = dataTableLoader;
			this.DefaultObjectComparer = defaultObjectComparer;
			this.DefaultObjectToFilterableConverter = defaultObjectToFilterableConverter;
			this.FilterSupportDescriptions.ListChanging += this.FilterSupportDescriptions_ListChanging;
			this.FilterSupportDescriptions.ListChanged += this.FilterSupportDescriptions_ListChanged;
			this.SortSupportDescriptions.ListChanging += this.SortSupportDescriptions_ListChanging;
			this.SortSupportDescriptions.ListChanged += this.SortSupportDescriptions_ListChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.SortSupportDescriptions.Clear();
				this.SortSupportDescriptions.ListChanging -= this.SortSupportDescriptions_ListChanging;
				this.SortSupportDescriptions.ListChanged -= this.SortSupportDescriptions_ListChanged;
				this.FilterSupportDescriptions.Clear();
				this.FilterSupportDescriptions.ListChanging -= this.FilterSupportDescriptions_ListChanging;
				this.FilterSupportDescriptions.ListChanged -= this.FilterSupportDescriptions_ListChanged;
				this.DataTableLoader = null;
			}
			base.Dispose(disposing);
		}

		public DataTableLoader DataTableLoader
		{
			get
			{
				return this.dataTableLoader;
			}
			private set
			{
				if (this.DataTableLoader != value)
				{
					this.DetachSort();
					this.DetachFilter();
					if (this.DataTableLoader != null)
					{
						this.DataTableLoader.RefreshingChanged -= this.DataTableLoader_RefreshingChanged;
						this.DataView = null;
					}
					this.dataTableLoader = value;
					if (this.DataTableLoader != null)
					{
						this.DataView = new DataView(this.DataTableLoader.Table);
						this.DataTableLoader.RefreshingChanged += this.DataTableLoader_RefreshingChanged;
					}
					this.AttachSort();
					this.AttachFilter();
				}
			}
		}

		private void DataTableLoader_RefreshingChanged(object sender, EventArgs e)
		{
			this.Filtering = this.DataTableLoader.Refreshing;
		}

		public DataView DataView
		{
			get
			{
				return this.dataView;
			}
			private set
			{
				if (this.DataView != value)
				{
					if (this.DataView != null)
					{
						this.DataView.ListChanged -= this.DataView_ListChanged;
						this.DataView.Table = null;
						this.DataView.Dispose();
					}
					this.dataView = value;
					if (this.DataView != null)
					{
						this.DataView.AllowEdit = false;
						this.DataView.AllowNew = false;
						this.DataView.AllowDelete = false;
						this.DataView.ListChanged += this.DataView_ListChanged;
					}
				}
			}
		}

		public int Count
		{
			get
			{
				return this.DataView.Count;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable)this.DataView).GetEnumerator();
		}

		private void DataView_ListChanged(object sender, ListChangedEventArgs e)
		{
			this.OnListChanged(e);
		}

		public bool SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			ListChangedEventHandler listChangedEventHandler = (ListChangedEventHandler)base.Events[DataTableLoaderView.EventListChanged];
			if (listChangedEventHandler != null)
			{
				listChangedEventHandler(this, e);
			}
		}

		public event ListChangedEventHandler ListChanged
		{
			add
			{
				base.Events.AddHandler(DataTableLoaderView.EventListChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataTableLoaderView.EventListChanged, value);
			}
		}

		public virtual bool SupportsFiltering
		{
			get
			{
				return true;
			}
		}

		public ObjectToFilterableConverter DefaultObjectToFilterableConverter { get; private set; }

		public ChangeNotifyingCollection<FilterSupportDescription> FilterSupportDescriptions
		{
			get
			{
				return this.filterSupportDescriptions;
			}
		}

		public FilterSupportDescription GetFilterSupportDescription(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentException();
			}
			return this.FilterSupportDescriptions.FirstOrDefault((FilterSupportDescription arg) => string.Compare(arg.ColumnName, propertyName, StringComparison.OrdinalIgnoreCase) == 0);
		}

		public bool IsFilterSupported(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentException("propertyName");
			}
			if (!this.DataTableLoader.Table.Columns.Contains(propertyName))
			{
				return false;
			}
			FilterSupportDescription filterSupportDescription = this.FilterSupportDescriptions.FirstOrDefault((FilterSupportDescription arg) => string.Compare(arg.ColumnName, propertyName, StringComparison.OrdinalIgnoreCase) == 0);
			if (filterSupportDescription != null)
			{
				return !string.IsNullOrEmpty(filterSupportDescription.FilteredColumnName);
			}
			return typeof(IConvertible).IsAssignableFrom(this.DataTableLoader.Table.Columns[propertyName].DataType);
		}

		private void FilterSupportDescriptions_ListChanging(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				this.DetachFilter(this.FilterSupportDescriptions[e.NewIndex]);
			}
		}

		private void FilterSupportDescriptions_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				this.AttachFilter(this.FilterSupportDescriptions[e.NewIndex]);
			}
		}

		private void AttachFilter()
		{
			foreach (FilterSupportDescription filterSupportDescription in this.FilterSupportDescriptions)
			{
				this.AttachFilter(filterSupportDescription);
			}
		}

		private void DetachFilter()
		{
			foreach (FilterSupportDescription filterSupportDescription in this.FilterSupportDescriptions)
			{
				this.DetachFilter(filterSupportDescription);
			}
		}

		private void AttachFilter(FilterSupportDescription filterSupportDescription)
		{
			filterSupportDescription.DataTableLoaderView = this;
		}

		private void DetachFilter(FilterSupportDescription filterSupportDescription)
		{
			filterSupportDescription.DataTableLoaderView = this;
		}

		public virtual void ApplyFilter(QueryFilter filter)
		{
			this.QueryFilter = filter;
			this.ApplyFilter();
		}

		public void RemoveFilter()
		{
			this.additionalFilter = string.Empty;
			this.ApplyFilter(null);
		}

		public QueryFilter QueryFilter { get; private set; }

		public bool UseDataTableLoaderSideFilter
		{
			get
			{
				return this.DataTableLoader.ResultsLoaderProfile != null && this.DataTableLoader.ResultsLoaderProfile.InputTable.Columns.Contains("Filter");
			}
		}

		[DefaultValue(null)]
		public string Filter
		{
			get
			{
				return this.additionalFilter;
			}
			set
			{
				if (string.Compare(this.additionalFilter, value) != 0)
				{
					this.additionalFilter = value;
					this.ApplyFilter();
				}
			}
		}

		private void ApplyFilter()
		{
			if (this.UseDataTableLoaderSideFilter)
			{
				string text = (this.QueryFilter != null) ? this.QueryFilter.GenerateInfixString(FilterLanguage.Monad) : string.Empty;
				if (string.IsNullOrEmpty(text))
				{
					text = this.additionalFilter;
				}
				else if (!string.IsNullOrEmpty(this.additionalFilter))
				{
					text = string.Format(CultureInfo.InvariantCulture, "(({0}) -and ({1}))", new object[]
					{
						text,
						this.additionalFilter
					});
				}
				this.DataTableLoader.InputValue("Filter", text);
				this.DataTableLoader.Refresh(this.CreateProgress(this.DataTableLoader.RefreshCommandText));
			}
			else
			{
				string text2 = (this.QueryFilter != null) ? DataTableLoaderView.AdaptQueryFilterForAdo(this.GetTransformedFilter()).GenerateInfixString(FilterLanguage.Ado) : string.Empty;
				if (string.IsNullOrEmpty(text2))
				{
					text2 = this.additionalFilter;
				}
				else if (!string.IsNullOrEmpty(this.additionalFilter))
				{
					text2 = string.Format(CultureInfo.InvariantCulture, "({0} AND {1})", new object[]
					{
						text2,
						this.additionalFilter
					});
				}
				this.DataView.RowFilter = text2;
			}
			if (this.DataTableLoader.ResultsLoaderProfile != null && this.DataTableLoader.ResultsLoaderProfile.PostRefreshAction != null)
			{
				this.DataTableLoader.ResultsLoaderProfile.PostRefreshAction.FilteredDataView = this.DataView;
			}
		}

		private IProgress CreateProgress(string operationName)
		{
			IProgressProvider progressProvider = (IProgressProvider)this.GetService(typeof(IProgressProvider));
			if (progressProvider != null)
			{
				return progressProvider.CreateProgress(operationName);
			}
			return NullProgress.Value;
		}

		private QueryFilter GetTransformedFilter()
		{
			return this.QueryFilter;
		}

		public static QueryFilter AdaptQueryFilterForAdo(QueryFilter filter)
		{
			QueryFilter queryFilter = filter;
			if (filter is CompositeFilter)
			{
				CompositeFilter compositeFilter = (CompositeFilter)filter;
				List<QueryFilter> list = new List<QueryFilter>(compositeFilter.FilterCount);
				for (int i = 0; i < compositeFilter.FilterCount; i++)
				{
					list.Add(DataTableLoaderView.AdaptQueryFilterForAdo(compositeFilter.Filters[i]));
				}
				if (filter is AndFilter)
				{
					queryFilter = new AndFilter(compositeFilter.IgnoreWhenVerifyingMaxDepth, list.ToArray());
				}
				else if (filter is OrFilter)
				{
					queryFilter = new OrFilter(compositeFilter.IgnoreWhenVerifyingMaxDepth, list.ToArray());
				}
			}
			else if (filter is TextFilter)
			{
				TextFilter textFilter = filter as TextFilter;
				queryFilter = new TextFilter(textFilter.Property, DataTableLoaderView.EscapeStringForADOLike(textFilter.Text), textFilter.MatchOptions, textFilter.MatchFlags);
			}
			else if (filter is ComparisonFilter)
			{
				ComparisonFilter comparisonFilter = filter as ComparisonFilter;
				ComparisonFilter comparisonFilter2 = new ComparisonFilter(comparisonFilter.ComparisonOperator, comparisonFilter.Property, comparisonFilter.PropertyValue);
				if (comparisonFilter.ComparisonOperator == ComparisonOperator.NotEqual || comparisonFilter.ComparisonOperator == ComparisonOperator.LessThanOrEqual || comparisonFilter.ComparisonOperator == ComparisonOperator.LessThan)
				{
					queryFilter = new OrFilter(new QueryFilter[]
					{
						comparisonFilter2,
						new NotFilter(new ExistsFilter(comparisonFilter.Property))
					});
				}
				else
				{
					queryFilter = comparisonFilter2;
				}
			}
			else if (filter is NotFilter)
			{
				NotFilter notFilter = filter as NotFilter;
				QueryFilter queryFilter2 = DataTableLoaderView.AdaptQueryFilterForAdo(notFilter.Filter);
				TextFilter textFilter2 = queryFilter2 as TextFilter;
				if (textFilter2 != null)
				{
					queryFilter = new OrFilter(new QueryFilter[]
					{
						new NotFilter(queryFilter2),
						new NotFilter(new ExistsFilter(textFilter2.Property))
					});
				}
				else
				{
					queryFilter = new NotFilter(queryFilter2);
				}
			}
			else if (filter is ExistsFilter)
			{
				ExistsFilter existsFilter = filter as ExistsFilter;
				queryFilter = new ExistsFilter(existsFilter.Property);
				if (existsFilter.Property.Type == typeof(string) || existsFilter.Property.Type == typeof(MultiValuedProperty<string>))
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						new NotFilter(new TextFilter(existsFilter.Property, string.Empty, MatchOptions.ExactPhrase, MatchFlags.Default))
					});
				}
			}
			return queryFilter;
		}

		private static string EscapeStringForADOLike(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			return DataTableLoaderView.regexForLike.Replace(text, (Match str) => string.Format("[{0}]", str));
		}

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
				base.Events.AddHandler(DataTableLoaderView.EventFilteringChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataTableLoaderView.EventFilteringChanged, value);
			}
		}

		protected virtual void OnFilteringChanged(EventArgs e)
		{
			if (base.Events[DataTableLoaderView.EventFilteringChanged] != null)
			{
				((EventHandler)base.Events[DataTableLoaderView.EventFilteringChanged])(this, e);
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
			if (!this.SupportCancelFiltering)
			{
				throw new InvalidOperationException("Do not support cancel filtering");
			}
			if (this.Filtering && this.DataTableLoader.Refreshing)
			{
				this.DataTableLoader.CancelRefresh();
			}
		}

		public bool SupportsSorting
		{
			get
			{
				return true;
			}
		}

		public bool SupportsAdvancedSorting
		{
			get
			{
				return true;
			}
		}

		public IObjectComparer DefaultObjectComparer { get; private set; }

		public ITextComparer DefaultTextComparer
		{
			get
			{
				return this.DefaultObjectComparer.TextComparer;
			}
		}

		public ChangeNotifyingCollection<SortSupportDescription> SortSupportDescriptions
		{
			get
			{
				return this.sortSupportDescriptions;
			}
		}

		public SortSupportDescription GetSortSupportDescription(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentException();
			}
			return this.SortSupportDescriptions.FirstOrDefault((SortSupportDescription arg) => string.Compare(arg.ColumnName, propertyName, StringComparison.OrdinalIgnoreCase) == 0);
		}

		public bool IsSortSupported(string propertyName)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw new ArgumentException("propertyName");
			}
			if (!this.DataTableLoader.Table.Columns.Contains(propertyName))
			{
				return false;
			}
			SortSupportDescription sortSupportDescription = this.SortSupportDescriptions.FirstOrDefault((SortSupportDescription arg) => string.Compare(arg.ColumnName, propertyName, StringComparison.OrdinalIgnoreCase) == 0);
			if (sortSupportDescription != null)
			{
				return sortSupportDescription.SortMode != SortMode.NotSupported && !string.IsNullOrEmpty(sortSupportDescription.SortedColumnName);
			}
			return typeof(IComparable).IsAssignableFrom(this.DataTableLoader.Table.Columns[propertyName].DataType);
		}

		private void SortSupportDescriptions_ListChanging(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				this.DetachSort(this.SortSupportDescriptions[e.NewIndex]);
			}
		}

		private void SortSupportDescriptions_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				this.AttachSort(this.SortSupportDescriptions[e.NewIndex]);
			}
		}

		private void AttachSort()
		{
			foreach (SortSupportDescription sortSupportDescription in this.SortSupportDescriptions)
			{
				this.AttachSort(sortSupportDescription);
			}
		}

		private void DetachSort()
		{
			foreach (SortSupportDescription sortSupportDescription in this.SortSupportDescriptions)
			{
				this.DetachSort(sortSupportDescription);
			}
		}

		private void AttachSort(SortSupportDescription sortSupportDescription)
		{
			sortSupportDescription.DataTableLoaderView = this;
		}

		private void DetachSort(SortSupportDescription sortSupportDescription)
		{
			sortSupportDescription.DataTableLoaderView = null;
		}

		public bool IsSorted
		{
			get
			{
				return ((IBindingList)this.DataView).IsSorted;
			}
		}

		public virtual void ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			if (!this.DataTableLoader.Table.Columns.Contains(property.Name))
			{
				throw new ArgumentException("property");
			}
			this.DataView.Sort = this.CreateSortString(property, direction);
			this.SortDescriptions = new ListSortDescriptionCollection(new ListSortDescription[]
			{
				new ListSortDescription(property, direction)
			});
		}

		public virtual void ApplySort(ListSortDescriptionCollection sorts)
		{
			if (sorts == null)
			{
				throw new ArgumentNullException("sorts");
			}
			List<ListSortDescription> list = new List<ListSortDescription>();
			foreach (object obj in ((IEnumerable)sorts))
			{
				ListSortDescription listSortDescription = (ListSortDescription)obj;
				if (listSortDescription == null || listSortDescription.PropertyDescriptor == null || !this.DataTableLoader.Table.Columns.Contains(listSortDescription.PropertyDescriptor.Name))
				{
					throw new ArgumentException("sorts");
				}
				list.Add(new ListSortDescription(listSortDescription.PropertyDescriptor, listSortDescription.SortDirection));
			}
			this.DataView.Sort = this.CreateSortString(sorts);
			this.SortDescriptions = new ListSortDescriptionCollection(list.ToArray());
		}

		public void RemoveSort()
		{
			this.ApplySort(new ListSortDescriptionCollection());
		}

		public ListSortDescriptionCollection SortDescriptions
		{
			get
			{
				return this.sortDescriptions;
			}
			private set
			{
				this.sortDescriptions = value;
			}
		}

		public ListSortDirection SortDirection
		{
			get
			{
				return ((IBindingList)this.DataView).SortDirection;
			}
		}

		public PropertyDescriptor SortProperty
		{
			get
			{
				if (this.SortDescriptions.Count != 0)
				{
					return this.SortDescriptions[0].PropertyDescriptor;
				}
				return null;
			}
		}

		private string CreateSortString(PropertyDescriptor property, ListSortDirection direction)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			string columnName = property.Name;
			if (this.DataTableLoader.Table.Columns.Contains(columnName))
			{
				SortSupportDescription sortSupportDescription = this.SortSupportDescriptions.FirstOrDefault((SortSupportDescription arg) => string.Compare(arg.ColumnName, columnName, StringComparison.OrdinalIgnoreCase) == 0);
				if (sortSupportDescription != null && !string.IsNullOrEmpty(sortSupportDescription.SortedColumnName))
				{
					columnName = sortSupportDescription.SortedColumnName;
				}
			}
			stringBuilder.Append(columnName);
			stringBuilder.Append(']');
			if (ListSortDirection.Descending == direction)
			{
				stringBuilder.Append(" DESC");
			}
			return stringBuilder.ToString();
		}

		private string CreateSortString(ListSortDescriptionCollection sorts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (object obj in ((IEnumerable)sorts))
			{
				ListSortDescription listSortDescription = (ListSortDescription)obj;
				if (!flag)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(this.CreateSortString(listSortDescription.PropertyDescriptor, listSortDescription.SortDirection));
				if (flag)
				{
					flag = false;
				}
			}
			return stringBuilder.ToString();
		}

		public bool SupportsSearching
		{
			get
			{
				return true;
			}
		}

		public void AddIndex(PropertyDescriptor property)
		{
			((IBindingList)this.DataView).AddIndex(property);
		}

		public void RemoveIndex(PropertyDescriptor property)
		{
			((IBindingList)this.DataView).RemoveIndex(property);
		}

		public int Find(PropertyDescriptor property, object key)
		{
			return ((IBindingList)this.DataView).Find(property, key);
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return ((IBindingList)this.DataView).AllowEdit;
			}
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return ((IBindingList)this.DataView).AllowNew;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return ((IBindingList)this.DataView).AllowRemove;
			}
		}

		object IBindingList.AddNew()
		{
			return ((IBindingList)this.DataView).AddNew();
		}

		int IList.Add(object value)
		{
			return ((IList)this.DataView).Add(value);
		}

		void IList.Clear()
		{
			((IList)this.DataView).Clear();
		}

		bool IList.Contains(object value)
		{
			return ((IList)this.DataView).Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return ((IList)this.DataView).IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			((IList)this.DataView).Insert(index, value);
		}

		bool IList.IsFixedSize
		{
			get
			{
				return ((IList)this.DataView).IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return ((IList)this.DataView).IsReadOnly;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return ((IList)this.DataView)[index];
			}
			set
			{
				((IList)this.DataView)[index] = value;
			}
		}

		void IList.Remove(object value)
		{
			((IList)this.DataView).Remove(value);
		}

		void IList.RemoveAt(int index)
		{
			((IList)this.DataView).RemoveAt(index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.DataView.CopyTo(array, index);
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return ((ICollection)this.DataView).IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return ((ICollection)this.DataView).SyncRoot;
			}
		}

		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			return ((ITypedList)this.DataView).GetItemProperties(listAccessors);
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			return ((ITypedList)this.DataView).GetListName(listAccessors);
		}

		public static DataTableLoaderView Create(DataTableLoader dataTableLoader)
		{
			return new DataTableLoaderView(dataTableLoader, ObjectComparer.DefaultObjectComparer, ObjectToFilterableConverter.DefaultObjectToFilterableConverter);
		}

		internal ComparisonFilter CreateComparisonFilter(ComparisonOperator comparisonOperator, string propertyName, object propertyValue)
		{
			FilterSupportDescription filterSupportDescription = this.GetFilterSupportDescription(propertyName);
			if (filterSupportDescription == null)
			{
				filterSupportDescription = new FilterSupportDescription(propertyName);
				this.FilterSupportDescriptions.Add(filterSupportDescription);
			}
			if (!this.DataTableLoader.Table.Columns.Contains(filterSupportDescription.FilteredColumnName))
			{
				return null;
			}
			return new ComparisonFilter(comparisonOperator, new DataTableLoaderView.FilterablePropertyDefinition(filterSupportDescription.FilteredColumnName, this.DataTableLoader.Table.Columns[filterSupportDescription.FilteredColumnName].DataType), this.DefaultObjectToFilterableConverter.ToFilterable(propertyValue));
		}

		internal ExistsFilter CreateExistsFilter(string propertyName)
		{
			FilterSupportDescription filterSupportDescription = this.GetFilterSupportDescription(propertyName);
			if (filterSupportDescription == null)
			{
				filterSupportDescription = new FilterSupportDescription(propertyName);
				this.FilterSupportDescriptions.Add(filterSupportDescription);
			}
			if (!this.DataTableLoader.Table.Columns.Contains(filterSupportDescription.FilteredColumnName))
			{
				return null;
			}
			return new ExistsFilter(new DataTableLoaderView.FilterablePropertyDefinition(filterSupportDescription.FilteredColumnName, this.DataTableLoader.Table.Columns[filterSupportDescription.FilteredColumnName].DataType));
		}

		private DataTableLoader dataTableLoader;

		private DataView dataView;

		private static readonly object EventListChanged = new object();

		private ChangeNotifyingCollection<FilterSupportDescription> filterSupportDescriptions = new ChangeNotifyingCollection<FilterSupportDescription>();

		private string additionalFilter;

		private static readonly Regex regexForLike = new Regex("[\\*%\\[\\]]");

		private bool filtering;

		private static object EventFilteringChanged = new object();

		private ChangeNotifyingCollection<SortSupportDescription> sortSupportDescriptions = new ChangeNotifyingCollection<SortSupportDescription>();

		private ListSortDescriptionCollection sortDescriptions = new ListSortDescriptionCollection();

		private class FilterablePropertyDefinition : PropertyDefinition
		{
			public FilterablePropertyDefinition(string name, Type type) : base(name, type)
			{
			}
		}
	}
}
