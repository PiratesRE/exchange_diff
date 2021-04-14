using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class IndentionRule : IAlignRule
	{
		public IndentionRule(IList<Type> includedTypes)
		{
			this.includedTypes = includedTypes;
		}

		public void Apply(AlignUnitsCollection collection)
		{
			int num = collection.ColumnCount;
			for (int i = 0; i < collection.RowCount; i++)
			{
				IList<AlignUnit> alignUnitsInRow = collection.GetAlignUnitsInRow(i);
				if (alignUnitsInRow.Count > 0)
				{
					AlignUnit alignUnit = alignUnitsInRow[0];
					if (alignUnit.Column < num && this.IsTypeMatch(alignUnit))
					{
						num = alignUnit.Column;
					}
					if (alignUnit.Column > num && collection.RowDeltaValue[i] > 8)
					{
						collection.RowDeltaValue[i] = 8;
					}
				}
			}
		}

		private bool IsTypeMatch(AlignUnit unit)
		{
			foreach (Type type in this.includedTypes)
			{
				if (type.IsAssignableFrom(unit.Control.GetType()))
				{
					return true;
				}
			}
			return false;
		}

		private const int DeltaValue = 8;

		private IList<Type> includedTypes;
	}
}
