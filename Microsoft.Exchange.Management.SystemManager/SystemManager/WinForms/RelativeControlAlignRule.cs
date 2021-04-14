using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class RelativeControlAlignRule : IAlignRule
	{
		public IList<RelativeControlAlignRule.Condition> Conditions
		{
			get
			{
				return this.conditionsList;
			}
		}

		public RelativeControlAlignRule(int value, IList<RelativeControlAlignRule.Condition> list)
		{
			this.deltaValue = value;
			this.conditionsList = list;
		}

		public RelativeControlAlignRule(int value) : this(value, new List<RelativeControlAlignRule.Condition>())
		{
		}

		public void Apply(AlignUnitsCollection units)
		{
			foreach (AlignUnit alignUnit in units.Units)
			{
				if (units.RowDeltaValue[alignUnit.Row] > this.deltaValue && this.Match(alignUnit, units))
				{
					units.RowDeltaValue[alignUnit.Row] = this.deltaValue;
				}
			}
		}

		private bool Match(AlignUnit unit, AlignUnitsCollection collection)
		{
			foreach (RelativeControlAlignRule.Condition condition in this.conditionsList)
			{
				AlignUnit offsetUnit = collection.GetOffsetUnit(unit, condition.OffsetRow, condition.OffSetColumn);
				if (!this.IsConditionMatch(condition, offsetUnit))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsConditionMatch(RelativeControlAlignRule.Condition condition, AlignUnit unit)
		{
			foreach (Type type in condition.ExcludedTypes)
			{
				if (this.IsTypeMatch(type, unit))
				{
					return false;
				}
			}
			foreach (Type type2 in condition.IncludedTypes)
			{
				if (this.IsTypeMatch(type2, unit))
				{
					return true;
				}
			}
			return condition.IncludedTypes.Count == 0;
		}

		private bool IsTypeMatch(Type type, AlignUnit unit)
		{
			if (!(type == null))
			{
				return unit != null && type.IsAssignableFrom(unit.Control.GetType());
			}
			return unit == null;
		}

		private IList<RelativeControlAlignRule.Condition> conditionsList;

		private int deltaValue;

		public class Condition
		{
			public int OffsetRow { get; private set; }

			public int OffSetColumn { get; private set; }

			public IList<Type> IncludedTypes { get; private set; }

			public IList<Type> ExcludedTypes { get; private set; }

			public Condition(int row, int column, IList<Type> includedTypes, IList<Type> excludedTypes)
			{
				this.OffSetColumn = column;
				this.OffsetRow = row;
				this.IncludedTypes = includedTypes;
				this.ExcludedTypes = excludedTypes;
			}

			public Condition(int row, int column, IList<Type> includedTypes) : this(row, column, includedTypes, new List<Type>())
			{
			}
		}
	}
}
