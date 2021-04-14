using System;
using System.Collections;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public class ViewDescriptor
	{
		public ViewDescriptor(ColumnId defaultSortColumn, bool isFixedWidth, params ColumnId[] columns)
		{
			if (columns != null && columns.Length == 0)
			{
				throw new ArgumentException("columns can not be null or empty array");
			}
			this.defaultSortColumn = defaultSortColumn;
			this.columns = columns;
			this.isFixedWidth = isFixedWidth;
			this.columnWidth = new float[columns.Length];
			for (int i = 0; i < columns.Length; i++)
			{
				Column column = ListViewColumns.GetColumn(columns[i]);
				if (isFixedWidth || !column.IsFixedWidth)
				{
					this.width += column.Width;
				}
			}
			float num = 0f;
			Column column2 = ListViewColumns.GetColumn(columns[columns.Length - 1]);
			for (int j = 0; j < columns.Length; j++)
			{
				Column column3 = ListViewColumns.GetColumn(columns[j]);
				if (!isFixedWidth && column3.IsFixedWidth)
				{
					this.columnWidth[j] = (float)((column3.Width < 1) ? 1 : column3.Width);
				}
				else
				{
					this.columnWidth[j] = ((this.width != 0) ? ((float)column3.Width * 100f / (float)this.width) : 0f);
					float num2 = (float)Math.Round((double)this.columnWidth[j], 1);
					if (column3.Id == ColumnId.ContactIcon && num2 < this.columnWidth[j])
					{
						num2 += 0.1f;
					}
					this.columnWidth[j] = ((num2 < 1f) ? 1f : num2);
					if (j == columns.Length - 2)
					{
						if (!isFixedWidth && column2.IsFixedWidth)
						{
							this.columnWidth[j] = 100f - num;
						}
					}
					else if (j == columns.Length - 1)
					{
						this.columnWidth[j] = 100f - num;
					}
					num += this.columnWidth[j];
				}
			}
			Hashtable hashtable = new Hashtable();
			foreach (ColumnId columnId in columns)
			{
				Column column4 = ListViewColumns.GetColumn(columnId);
				for (int l = 0; l < column4.PropertyCount; l++)
				{
					if (!hashtable.ContainsKey(column4[l]))
					{
						hashtable.Add(column4[l], null);
					}
				}
			}
			this.properties = new PropertyDefinition[hashtable.Count];
			int num3 = 0;
			IDictionaryEnumerator enumerator = hashtable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				this.properties[num3++] = (PropertyDefinition)enumerator.Key;
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.columns.Length;
			}
		}

		public int PropertyCount
		{
			get
			{
				return this.properties.Length;
			}
		}

		public ColumnId DefaultSortColumn
		{
			get
			{
				return this.defaultSortColumn;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
		}

		public bool IsFixedWidth
		{
			get
			{
				return this.isFixedWidth;
			}
		}

		public float GetColumnWidth(int columnIndex)
		{
			return this.columnWidth[columnIndex];
		}

		public PropertyDefinition GetProperty(int propertyIndex)
		{
			return this.properties[propertyIndex];
		}

		public ColumnId GetColumn(int columnIndex)
		{
			return this.columns[columnIndex];
		}

		public bool ContainsColumn(ColumnId columnId)
		{
			foreach (ColumnId columnId2 in this.columns)
			{
				if (columnId2 == columnId)
				{
					return true;
				}
			}
			return false;
		}

		private ColumnId[] columns;

		private ColumnId defaultSortColumn;

		private bool isFixedWidth;

		private int width;

		private float[] columnWidth;

		private PropertyDefinition[] properties;
	}
}
