using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[SettingsProvider(typeof(ExchangeSettingsProvider))]
	public class DataListViewSettings : ExchangeSettings
	{
		public DataListViewSettings(IComponent owner) : base(owner)
		{
		}

		[UserScopedSetting]
		[DefaultSettingValue("")]
		public Hashtable DataListViewInfo
		{
			get
			{
				return (Hashtable)this["DataListViewInfo"];
			}
			set
			{
				this["DataListViewInfo"] = value;
			}
		}

		[DefaultSettingValue(null)]
		[UserScopedSetting]
		public byte[] FilterExpression
		{
			get
			{
				return (byte[])this["FilterExpression"];
			}
			set
			{
				this["FilterExpression"] = value;
			}
		}

		[UserScopedSetting]
		[DefaultSettingValue("25")]
		public int ResultsPerPage
		{
			get
			{
				return (int)this["ResultsPerPage"];
			}
			set
			{
				this["ResultsPerPage"] = value;
			}
		}

		public void SaveDataListViewSettings(DataListView listView)
		{
			if (listView == null)
			{
				throw new ArgumentNullException();
			}
			int count = listView.Columns.Count;
			DataListViewSettings.SerializableColumnInfo[] array = new DataListViewSettings.SerializableColumnInfo[count];
			for (int i = 0; i < count; i++)
			{
				array[listView.Columns[i].DisplayIndex] = new DataListViewSettings.SerializableColumnInfo(listView.Columns[i].Name, listView.Columns[i].Width, listView.Columns[i].DisplayIndex);
			}
			if (this.DataListViewInfo == null)
			{
				this.DataListViewInfo = new Hashtable();
			}
			this.DataListViewInfo[listView.Name] = new DataListViewSettings.SerializableDataListViewInfo(array, listView.SortDirection, listView.SortProperty, listView.IsColumnsWidthDirty);
		}

		public void LoadDataListViewSettings(DataListView listView)
		{
			if (listView == null)
			{
				throw new ArgumentNullException();
			}
			listView.BeginUpdate();
			DataListViewSettings.SerializableDataListViewInfo serializableDataListViewInfo = this.FindSuitableSetting(listView);
			if (serializableDataListViewInfo == null)
			{
				listView.SortDirection = ListSortDirection.Ascending;
			}
			else
			{
				listView.SortDirection = serializableDataListViewInfo.SortDirection;
				listView.SortProperty = serializableDataListViewInfo.SortProperty;
				listView.IsColumnsWidthDirty = serializableDataListViewInfo.IsColumnsWidthDirty;
				int length = serializableDataListViewInfo.Columns.GetLength(0);
				ArrayList arrayList = new ArrayList(length);
				int num = 0;
				for (int i = 0; i < length; i++)
				{
					if (!string.IsNullOrEmpty(serializableDataListViewInfo.Columns[i].ColumnName))
					{
						ExchangeColumnHeader exchangeColumnHeader = listView.AvailableColumns[serializableDataListViewInfo.Columns[i].ColumnName];
						exchangeColumnHeader.Visible = true;
						exchangeColumnHeader.Width = serializableDataListViewInfo.Columns[i].ColumnWidth;
						exchangeColumnHeader.DisplayIndex = num++;
						arrayList.Add(exchangeColumnHeader);
					}
				}
				if (arrayList.Count > 0)
				{
					foreach (ExchangeColumnHeader exchangeColumnHeader2 in listView.AvailableColumns)
					{
						exchangeColumnHeader2.Visible = arrayList.Contains(exchangeColumnHeader2);
					}
				}
			}
			listView.EndUpdate();
		}

		private DataListViewSettings.SerializableDataListViewInfo FindSuitableSetting(DataListView listView)
		{
			if (this.DataListViewInfo == null || this.DataListViewInfo[listView.Name] == null)
			{
				return null;
			}
			DataListViewSettings.SerializableDataListViewInfo serializableDataListViewInfo = (DataListViewSettings.SerializableDataListViewInfo)this.DataListViewInfo[listView.Name];
			if (listView.AvailableColumns[serializableDataListViewInfo.SortProperty] == null)
			{
				return null;
			}
			int length = serializableDataListViewInfo.Columns.GetLength(0);
			if (length > listView.AvailableColumns.Count)
			{
				return null;
			}
			for (int i = 0; i < length; i++)
			{
				if (!string.IsNullOrEmpty(serializableDataListViewInfo.Columns[i].ColumnName) && listView.AvailableColumns[serializableDataListViewInfo.Columns[i].ColumnName] == null)
				{
					return null;
				}
			}
			return serializableDataListViewInfo;
		}

		[Serializable]
		public class SerializableDataListViewInfo
		{
			public SerializableDataListViewInfo(DataListViewSettings.SerializableColumnInfo[] cols, ListSortDirection sortDir, string sortProp, bool columnDirty)
			{
				this.columns = cols;
				this.sortDirection = sortDir;
				this.sortProperty = sortProp;
				this.isColumnsWidthDirty = columnDirty;
			}

			public DataListViewSettings.SerializableColumnInfo[] Columns
			{
				get
				{
					return this.columns;
				}
				set
				{
					this.columns = value;
				}
			}

			public string SortProperty
			{
				get
				{
					return this.sortProperty;
				}
				set
				{
					this.sortProperty = value;
				}
			}

			public ListSortDirection SortDirection
			{
				get
				{
					return this.sortDirection;
				}
				set
				{
					this.sortDirection = value;
				}
			}

			public bool IsColumnsWidthDirty
			{
				get
				{
					return this.isColumnsWidthDirty;
				}
				set
				{
					this.isColumnsWidthDirty = value;
				}
			}

			private DataListViewSettings.SerializableColumnInfo[] columns;

			private ListSortDirection sortDirection;

			private string sortProperty;

			private bool isColumnsWidthDirty;
		}

		[Serializable]
		public struct SerializableColumnInfo
		{
			public SerializableColumnInfo(string name, int width, int index)
			{
				this.columnName = name;
				this.columnWidth = width;
				this.displayIndex = index;
			}

			public int ColumnWidth
			{
				get
				{
					return this.columnWidth;
				}
				set
				{
					this.columnWidth = value;
				}
			}

			public string ColumnName
			{
				get
				{
					return this.columnName;
				}
				set
				{
					this.columnName = value;
				}
			}

			public int DisplayIndex
			{
				get
				{
					return this.displayIndex;
				}
				set
				{
					this.displayIndex = value;
				}
			}

			private int columnWidth;

			private string columnName;

			private int displayIndex;
		}
	}
}
