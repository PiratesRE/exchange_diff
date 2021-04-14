using System;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DefaultAlignRule : IAlignRule
	{
		public void Apply(AlignUnitsCollection units)
		{
			Padding padding = new Padding(0);
			for (int i = 0; i < units.RowCount; i++)
			{
				AlignUnit maxUnitInRow = units.GetMaxUnitInRow(i);
				if (maxUnitInRow == null)
				{
					padding = new Padding(0);
				}
				else
				{
					AlignUnit minUnitInRow = units.GetMinUnitInRow(i);
					Padding padding2 = maxUnitInRow.CompareMargin - minUnitInRow.CompareMargin;
					padding2 = ((padding2.Vertical > maxUnitInRow.InlinedMargin.Vertical) ? padding2 : maxUnitInRow.InlinedMargin);
					foreach (AlignUnit alignUnit in units.GetAlignUnitsInRow(i))
					{
						alignUnit.ResultMargin = maxUnitInRow.CompareMargin - alignUnit.CompareMargin;
					}
					units.RowDeltaValue[i] = units.RowDeltaValue[i] - padding.Bottom - padding2.Top;
					if (units.RowDeltaValue[i] < DefaultAlignRule.MinimalVertical)
					{
						units.RowDeltaValue[i] = DefaultAlignRule.MinimalVertical;
					}
					padding = padding2;
				}
			}
			if (units.RowCount > 0)
			{
				units.RowDeltaValue[0] = 0;
			}
		}

		public static int MinimalVertical = 3;
	}
}
