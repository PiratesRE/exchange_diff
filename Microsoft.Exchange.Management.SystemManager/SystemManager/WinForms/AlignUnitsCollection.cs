using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class AlignUnitsCollection
	{
		public int[] RowDeltaValue { get; set; }

		private bool GetControlVisible(Control control)
		{
			ISite site = control.Site;
			if (site != null && site.DesignMode)
			{
				IDesignerHost designerHost = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if (designerHost != null)
				{
					try
					{
						ControlDesigner target = designerHost.GetDesigner(control) as ControlDesigner;
						BindingFlags invokeAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty;
						object obj = typeof(ControlDesigner).InvokeMember("ShadowProperties", invokeAttr, null, target, null);
						return (bool)obj.GetType().InvokeMember("Item", invokeAttr, null, obj, new object[]
						{
							"Visible"
						});
					}
					catch (Exception)
					{
						return control.Visible;
					}
				}
			}
			return control.Visible;
		}

		private IList<int> GetInvisibleRowsWithControl(TableLayoutPanel panel)
		{
			List<int> list = new List<int>();
			foreach (object obj in panel.Controls)
			{
				Control control = (Control)obj;
				int row = panel.GetRow(control);
				if (row >= 0 && !list.Contains(row))
				{
					list.Add(row);
				}
			}
			foreach (object obj2 in panel.Controls)
			{
				Control control2 = (Control)obj2;
				if (this.GetControlVisible(control2))
				{
					list.Remove(panel.GetRow(control2));
				}
			}
			return list;
		}

		private IDictionary<int, int> GetRowsMapping(TableLayoutPanel panel)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			IList<int> invisibleRowsWithControl = this.GetInvisibleRowsWithControl(panel);
			int num = 0;
			for (int i = 0; i < panel.RowCount; i++)
			{
				if (invisibleRowsWithControl.Contains(i))
				{
					num++;
				}
				else
				{
					dictionary[i] = i - num;
				}
			}
			return dictionary;
		}

		private AlignUnitsCollection(TableLayoutPanel panel)
		{
			IDictionary<int, int> rowsMapping = this.GetRowsMapping(panel);
			this.unitsCollection = new AlignUnit[rowsMapping.Count, panel.ColumnCount];
			this.RowDeltaValue = new int[rowsMapping.Count];
			foreach (object obj in panel.Controls)
			{
				Control control = (Control)obj;
				int row = panel.GetRow(control);
				int column = panel.GetColumn(control);
				if (this.GetControlVisible(control) && row >= 0 && column >= 0)
				{
					int num = rowsMapping[row];
					int num2 = column;
					AlignUnit alignUnit = new AlignUnit(control, panel.GetRowSpan(control), panel.GetColumnSpan(control), num, num2);
					for (int i = 0; i < alignUnit.ColumnSpan; i++)
					{
						this.unitsCollection[num, num2 + i] = alignUnit;
					}
					this.unitsList.Add(alignUnit);
					if (!this.rowUnitsDictionary.ContainsKey(num))
					{
						this.rowUnitsDictionary[num] = new List<AlignUnit>();
					}
					this.rowUnitsDictionary[num].Add(alignUnit);
				}
			}
			for (int j = 0; j < this.RowCount; j++)
			{
				if (!this.rowUnitsDictionary.ContainsKey(j))
				{
					this.rowUnitsDictionary[j] = new List<AlignUnit>();
				}
				else
				{
					this.rowUnitsDictionary[j].Sort();
				}
				this.RowDeltaValue[j] = AlignSettings.DefaultVertical;
			}
			this.UpdateCompareMargin();
		}

		public static AlignUnitsCollection GetAlignUnitsCollectionFromTLP(TableLayoutPanel tlp)
		{
			return new AlignUnitsCollection(tlp);
		}

		private void UpdateCompareMargin()
		{
			foreach (AlignUnit alignUnit in this.Units)
			{
				AlignMappingEntry mappingEntry = AlignSettings.GetMappingEntry(alignUnit.Control.GetType());
				if (mappingEntry != AlignMappingEntry.Empty)
				{
					alignUnit.CompareMargin = mappingEntry.CompareMargin;
					alignUnit.InlinedMargin = mappingEntry.InlinedMargin;
					if (alignUnit.Control.Height < mappingEntry.DefaultHeight && mappingEntry.DefaultHeight > 0)
					{
						int num = mappingEntry.DefaultHeight - alignUnit.Control.Height;
						if (num >= alignUnit.CompareMargin.Vertical)
						{
							alignUnit.CompareMargin = Padding.Empty;
						}
						else
						{
							int num2 = num * alignUnit.CompareMargin.Top / alignUnit.CompareMargin.Vertical;
							alignUnit.CompareMargin -= new Padding(0, num2, 0, num - num2);
						}
					}
				}
				else
				{
					alignUnit.CompareMargin = Padding.Empty;
					alignUnit.InlinedMargin = Padding.Empty;
				}
			}
		}

		public int RowCount
		{
			get
			{
				return this.unitsCollection.GetLength(0);
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.unitsCollection.GetLength(1);
			}
		}

		public AlignUnit GetUnitFromPosition(int row, int column)
		{
			if (row >= 0 && row < this.RowCount && column >= 0 && column < this.ColumnCount)
			{
				return this.unitsCollection[row, column];
			}
			return null;
		}

		public AlignUnit GetMaxUnitInRow(int row)
		{
			AlignUnit alignUnit = null;
			foreach (AlignUnit alignUnit2 in this.GetAlignUnitsInRow(row))
			{
				if (alignUnit == null || alignUnit2.CompareMargin.Vertical > alignUnit.CompareMargin.Vertical)
				{
					alignUnit = alignUnit2;
				}
			}
			return alignUnit;
		}

		public AlignUnit GetMinUnitInRow(int row)
		{
			AlignUnit alignUnit = null;
			foreach (AlignUnit alignUnit2 in this.GetAlignUnitsInRow(row))
			{
				if (alignUnit == null || alignUnit2.CompareMargin.Vertical < alignUnit.CompareMargin.Vertical)
				{
					alignUnit = alignUnit2;
				}
			}
			return alignUnit;
		}

		public IList<AlignUnit> GetAlignUnitsInRow(int row)
		{
			if (!this.rowUnitsDictionary.ContainsKey(row))
			{
				this.rowUnitsDictionary[row] = new List<AlignUnit>();
			}
			return this.rowUnitsDictionary[row];
		}

		public IList<AlignUnit> Units
		{
			get
			{
				return this.unitsList;
			}
		}

		public AlignUnit GetOffsetUnit(AlignUnit unit, int row, int col)
		{
			if (row > 0)
			{
				row += unit.RowSpan - 1;
			}
			if (col > 0)
			{
				col += unit.ColumnSpan - 1;
			}
			return this.GetUnitFromPosition(unit.Row + row, unit.Column + col);
		}

		public IList<AlignUnit> GetOffsetUnits(AlignUnit unit, int row, int col)
		{
			List<AlignUnit> list = new List<AlignUnit>();
			if (row > 0)
			{
				row += unit.RowSpan - 1;
			}
			if (col > 0)
			{
				col += unit.ColumnSpan - 1;
			}
			for (int i = 0; i < unit.ColumnSpan; i++)
			{
				AlignUnit unitFromPosition = this.GetUnitFromPosition(unit.Row + row, unit.Column + col + i);
				if (unitFromPosition != null && !list.Contains(unitFromPosition))
				{
					list.Add(unitFromPosition);
				}
			}
			return list;
		}

		private AlignUnit[,] unitsCollection;

		private IDictionary<int, List<AlignUnit>> rowUnitsDictionary = new Dictionary<int, List<AlignUnit>>();

		private IList<AlignUnit> unitsList = new List<AlignUnit>();
	}
}
