using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class FilterSupportDescription
	{
		public FilterSupportDescription(string columnName)
		{
			if (string.IsNullOrEmpty(columnName))
			{
				throw new ArgumentException();
			}
			this.ColumnName = columnName;
			this.FilteredColumnReferenceNumberProperty = string.Format("ColumnName_{0}_FilteredColumn_ReferenceNumber", this.ColumnName);
			this.FilteredColumnName = string.Empty;
		}

		public string ColumnName { get; private set; }

		public string FilteredColumnName { get; private set; }

		public IFilterableConverter FilterableConverter { get; private set; }

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

		public ObjectToFilterableConverter DefaultObjectToFilterableConverter
		{
			get
			{
				return this.DataTableLoaderView.DefaultObjectToFilterableConverter;
			}
		}

		private void AttachDataTableLoaderView()
		{
			this.AttachFilteredColumn();
			this.DataTableLoader.Table.Columns.CollectionChanged += this.DataColumns_CollectionChanged;
		}

		private void AttachFilteredColumn()
		{
			if (this.DataTableLoader.Table.Columns.Contains(this.ColumnName))
			{
				Type dataType = this.DataTableLoader.Table.Columns[this.ColumnName].DataType;
				if (this.DefaultObjectToFilterableConverter.ShouldUseStandardFiltering(dataType))
				{
					this.FilteredColumnName = this.ColumnName;
					this.FilterableConverter = null;
					return;
				}
				string filterSupportDescriptionNameWithPostfix = this.GetFilterSupportDescriptionNameWithPostfix();
				IFilterableConverter filterableConverter;
				if (!this.DataTableLoader.Table.Columns.Contains(filterSupportDescriptionNameWithPostfix))
				{
					DataColumn dataColumn = new DataColumn(filterSupportDescriptionNameWithPostfix);
					dataColumn.DataType = typeof(IConvertible);
					dataColumn.ExtendedProperties[this.FilteredColumnReferenceNumberProperty] = 0;
					filterableConverter = this.DefaultObjectToFilterableConverter;
					dataColumn.ExtendedProperties["ColumnValueCalculator"] = new ColumnValueCalculator(this.ColumnName, filterSupportDescriptionNameWithPostfix, new FilterSupportDescription.FilterableObjectTypeConverter(filterableConverter));
					this.DataTableLoader.Table.Columns.Add(dataColumn);
				}
				else
				{
					filterableConverter = ((this.DataTableLoader.Table.Columns[filterSupportDescriptionNameWithPostfix].ExtendedProperties["ColumnValueCalculator"] as ColumnValueCalculator).TypeConverter as FilterSupportDescription.FilterableObjectTypeConverter).FilterableConverter;
				}
				this.DataTableLoader.Table.Columns[filterSupportDescriptionNameWithPostfix].ExtendedProperties[this.FilteredColumnReferenceNumberProperty] = (int)this.DataTableLoader.Table.Columns[filterSupportDescriptionNameWithPostfix].ExtendedProperties[this.FilteredColumnReferenceNumberProperty] + 1;
				this.FilterableConverter = filterableConverter;
				this.FilteredColumnName = filterSupportDescriptionNameWithPostfix;
			}
		}

		private void DetachDataTableLoaderView()
		{
			this.DataTableLoader.Table.Columns.CollectionChanged -= this.DataColumns_CollectionChanged;
			this.DetachFilteredColumn();
		}

		private void DetachFilteredColumn()
		{
			if (this.FilteredColumnName != this.ColumnName && this.DataTableLoader.Table.Columns.Contains(this.FilteredColumnName))
			{
				DataColumn dataColumn = this.DataTableLoader.Table.Columns[this.FilteredColumnName];
				dataColumn.ExtendedProperties[this.FilteredColumnReferenceNumberProperty] = (int)dataColumn.ExtendedProperties[this.FilteredColumnReferenceNumberProperty] - 1;
				if ((int)dataColumn.ExtendedProperties[this.FilteredColumnReferenceNumberProperty] == 0)
				{
					this.DataTableLoader.Table.Columns.Remove(this.FilteredColumnName);
				}
				this.FilteredColumnName = string.Empty;
				this.FilterableConverter = null;
			}
		}

		private void DataColumns_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Add)
			{
				DataColumn dataColumn = e.Element as DataColumn;
				if (string.Compare(dataColumn.ColumnName, this.ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.AttachFilteredColumn();
					return;
				}
			}
			else if (e.Action == CollectionChangeAction.Remove)
			{
				DataColumn dataColumn2 = e.Element as DataColumn;
				if (string.Compare(dataColumn2.ColumnName, this.ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					this.DetachFilteredColumn();
				}
			}
		}

		private string GetFilterSupportDescriptionNameWithPostfix()
		{
			return string.Format("ColumnName_{0}_TableName_{1}_FilteredColumnName_Number_{2}", this.ColumnName, this.DataTableLoader.Table.TableName, 0);
		}

		private DataTableLoaderView dataTableLoaderView;

		private readonly string FilteredColumnReferenceNumberProperty;

		private class FilterableObjectTypeConverter : TypeConverter
		{
			public FilterableObjectTypeConverter(IFilterableConverter filterableConverter)
			{
				this.FilterableConverter = filterableConverter;
			}

			public IFilterableConverter FilterableConverter { get; private set; }

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return true;
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				return this.FilterableConverter.ToFilterable(value);
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				throw new NotSupportedException();
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				throw new NotSupportedException();
			}
		}
	}
}
