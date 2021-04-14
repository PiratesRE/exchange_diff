using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ConstLeftPaddingRule : IAlignRule
	{
		public ConstLeftPaddingRule(IList<Type> excludedTypes)
		{
			this.excludedTypes = (excludedTypes ?? new List<Type>());
		}

		public void Apply(AlignUnitsCollection collection)
		{
			foreach (AlignUnit alignUnit in collection.Units)
			{
				if (this.Match(alignUnit, collection) && alignUnit.ResultMargin.Left + alignUnit.Control.Padding.Left < this.leftPadding.Left)
				{
					alignUnit.ResultMargin += this.leftPadding;
				}
			}
		}

		private bool Match(AlignUnit unit, AlignUnitsCollection collection)
		{
			foreach (Type type in this.excludedTypes)
			{
				if (type.IsAssignableFrom(unit.Control.GetType()))
				{
					return false;
				}
			}
			return true;
		}

		private IList<Type> excludedTypes;

		private Padding leftPadding = new Padding(3, 0, 0, 0);
	}
}
