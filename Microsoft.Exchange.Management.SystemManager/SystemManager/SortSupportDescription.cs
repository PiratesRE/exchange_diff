using System;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class SortSupportDescription
	{
		public SortSupportDescription(string columnName, SortMode sortMode, IComparer comparer, ICustomFormatter customFormatter, IFormatProvider formatProvider, string formatString, string defaultEmptyText)
		{
			if (string.IsNullOrEmpty(columnName))
			{
				throw new ArgumentException();
			}
			this.ColumnName = columnName;
			this.SortedColumnReferenceNumberProperty = string.Format("ColumnName_{0}_SortedColumn_ReferenceNumber", this.ColumnName);
			this.SortMode = sortMode;
			this.Comparer = comparer;
			this.CustomFormatter = customFormatter;
			this.FormatProvider = formatProvider;
			this.FormatString = formatString;
			this.DefaultEmptyText = defaultEmptyText;
			this.Comparer = null;
			this.SortedColumnName = string.Empty;
		}

		public string ColumnName { get; private set; }

		public SortMode SortMode { get; private set; }

		public IComparer Comparer { get; private set; }

		public ICustomFormatter CustomFormatter { get; private set; }

		public IFormatProvider FormatProvider { get; private set; }

		public string FormatString { get; private set; }

		public string DefaultEmptyText { get; private set; }

		public string SortedColumnName { get; private set; }

		public DataTableLoaderView DataTableLoaderView
		{
			get
			{
				return this.dataTableLoaderView;
			}
			internal set
			{
				if (this.DataTableLoaderView != value)
				{
					if (this.DataTableLoaderView != null)
					{
						this.DetachDataTableLoaderView();
					}
					this.dataTableLoaderView = value;
					if (this.DataTableLoaderView != null)
					{
						this.AttachDataTableLoaderView();
					}
				}
			}
		}

		public DataTableLoader DataTableLoader
		{
			get
			{
				return this.DataTableLoaderView.DataTableLoader;
			}
		}

		public IObjectComparer DefaultObjectComparer
		{
			get
			{
				return this.DataTableLoaderView.DefaultObjectComparer;
			}
		}

		public ITextComparer DefaultTextComparer
		{
			get
			{
				return this.DataTableLoaderView.DefaultTextComparer;
			}
		}

		private void AttachDataTableLoaderView()
		{
			if (this.SortMode == SortMode.NotSupported)
			{
				return;
			}
			this.AttachSortedColumn();
			this.DataTableLoader.Table.Columns.CollectionChanged += this.DataColumns_CollectionChanged;
		}

		private void DetachDataTableLoaderView()
		{
			if (this.SortMode == SortMode.NotSupported)
			{
				return;
			}
			this.DataTableLoader.Table.Columns.CollectionChanged -= this.DataColumns_CollectionChanged;
			this.DetachSortedColumn();
		}

		private void AttachSortedColumn()
		{
			if (this.DataTableLoader.Table.Columns.Contains(this.ColumnName))
			{
				if (this.SortMode == SortMode.NotSpecified)
				{
					this.SortMode = this.DefaultObjectComparer.GetSortMode(this.DataTableLoader.Table.Columns[this.ColumnName].DataType);
				}
				if (this.SortMode == SortMode.Standard)
				{
					this.SortedColumnName = this.ColumnName;
					return;
				}
				if (this.SortMode == SortMode.DelegateColumn)
				{
					string text = this.DataTableLoader.Table.Columns[this.ColumnName].ExtendedProperties["DelegateColumnName"] as string;
					if (this.DefaultObjectComparer.GetSortMode(this.DataTableLoader.Table.Columns[text].DataType) == SortMode.Standard)
					{
						this.SortedColumnName = text;
						return;
					}
					this.SortedColumnName = this.ColumnName;
					return;
				}
				else
				{
					if (this.SortMode == SortMode.Text)
					{
						this.Comparer = new SupportTextComparerAdapter(this.DefaultTextComparer, this.CustomFormatter, this.FormatProvider, this.FormatString, this.DefaultEmptyText);
					}
					if (this.SortMode == SortMode.Custom)
					{
						this.Comparer = new SupportTextComparerAdapter(this.DefaultObjectComparer, this.CustomFormatter, this.FormatProvider, this.FormatString, this.DefaultEmptyText);
					}
					this.SortedColumnName = this.GetSortSupportDescriptionNameWithPostfix();
					if (!this.DataTableLoader.Table.Columns.Contains(this.SortedColumnName))
					{
						DataColumn dataColumn = new DataColumn(this.SortedColumnName);
						dataColumn.DataType = typeof(IComparable);
						dataColumn.ExtendedProperties[this.SortedColumnReferenceNumberProperty] = 0;
						dataColumn.ExtendedProperties["ColumnValueCalculator"] = new ColumnValueCalculator(this.ColumnName, this.SortedColumnName, new ComparableTypeConverter(this.Comparer));
						this.DataTableLoader.Table.Columns.Add(dataColumn);
					}
					this.DataTableLoader.Table.Columns[this.SortedColumnName].ExtendedProperties[this.SortedColumnReferenceNumberProperty] = (int)this.DataTableLoader.Table.Columns[this.SortedColumnName].ExtendedProperties[this.SortedColumnReferenceNumberProperty] + 1;
				}
			}
		}

		private void DetachSortedColumn()
		{
			if (this.SortedColumnName != this.ColumnName && this.DataTableLoader.Table.Columns.Contains(this.SortedColumnName))
			{
				DataColumn dataColumn = this.DataTableLoader.Table.Columns[this.SortedColumnName];
				dataColumn.ExtendedProperties[this.SortedColumnReferenceNumberProperty] = (int)dataColumn.ExtendedProperties[this.SortedColumnReferenceNumberProperty] - 1;
				if ((int)dataColumn.ExtendedProperties[this.SortedColumnReferenceNumberProperty] == 0)
				{
					this.DataTableLoader.Table.Columns.Remove(this.SortedColumnName);
				}
				this.Comparer = null;
				this.SortedColumnName = string.Empty;
			}
		}

		private void DataColumns_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Add)
			{
				DataColumn dataColumn = e.Element as DataColumn;
				if (string.Compare(dataColumn.ColumnName, this.ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.AttachSortedColumn();
					return;
				}
			}
			else if (e.Action == CollectionChangeAction.Remove)
			{
				DataColumn dataColumn2 = e.Element as DataColumn;
				if (string.Compare(dataColumn2.ColumnName, this.ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.DetachSortedColumn();
				}
			}
		}

		private string GetSortSupportDescriptionNameWithPostfix()
		{
			return string.Format("ColumnName_{0}_TableName_{1}_SortedColumnName_Number_{2}", this.ColumnName, this.DataTableLoader.Table.TableName, 0);
		}

		private DataTableLoaderView dataTableLoaderView;

		private readonly string SortedColumnReferenceNumberProperty;
	}
}
