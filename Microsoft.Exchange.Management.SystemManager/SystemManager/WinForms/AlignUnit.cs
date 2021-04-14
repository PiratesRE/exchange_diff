using System;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class AlignUnit : IComparable<AlignUnit>
	{
		public int ColumnSpan { get; set; }

		public int RowSpan { get; set; }

		public Control Control { get; set; }

		public Padding ResultMargin { get; set; }

		public Padding CompareMargin { get; set; }

		public Padding InlinedMargin { get; set; }

		public int Row { get; set; }

		public int Column { get; set; }

		public AlignUnit(Control ctrl, int rowSpan, int columnSpan, int row, int column)
		{
			this.ColumnSpan = columnSpan;
			this.RowSpan = rowSpan;
			this.Row = row;
			this.Column = column;
			this.Control = ctrl;
		}

		public int CompareTo(AlignUnit unit)
		{
			if (this.Row == unit.Row)
			{
				return this.Column.CompareTo(unit.Column);
			}
			return this.Row.CompareTo(unit.Row);
		}
	}
}
