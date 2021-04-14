using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ColumnAdjuster
	{
		internal ColumnAdjuster(PropertyDefinition property)
		{
			this.property = property;
		}

		internal int Index
		{
			get
			{
				return this.index;
			}
		}

		internal static PropertyDefinition[] Adjust(PropertyDefinition[] columns, IList<ColumnAdjuster> columnsToAdjust)
		{
			int num = 0;
			for (int i = 0; i < columnsToAdjust.Count; i++)
			{
				ColumnAdjuster columnAdjuster = columnsToAdjust[i];
				int num2 = Array.IndexOf<PropertyDefinition>(columns, columnAdjuster.property);
				if (num2 == -1)
				{
					num++;
				}
				else
				{
					columnAdjuster.index = num2;
				}
			}
			if (num > 0)
			{
				PropertyDefinition[] array = new PropertyDefinition[columns.Length + num];
				Array.Copy(columns, array, columns.Length);
				int num3 = columns.Length;
				for (int j = 0; j < columnsToAdjust.Count; j++)
				{
					ColumnAdjuster columnAdjuster2 = columnsToAdjust[j];
					if (columnAdjuster2.index == -1)
					{
						array[num3] = columnAdjuster2.property;
						columnAdjuster2.index = num3;
						num3++;
					}
				}
				return array;
			}
			return columns;
		}

		private PropertyDefinition property;

		private int index = -1;
	}
}
