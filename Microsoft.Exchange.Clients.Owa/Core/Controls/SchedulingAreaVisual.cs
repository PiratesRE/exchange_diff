using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public sealed class SchedulingAreaVisual : CalendarVisual
	{
		public SchedulingAreaVisual(int dataIndex) : base(dataIndex)
		{
		}

		public int Column
		{
			get
			{
				return this.column;
			}
			set
			{
				this.column = value;
			}
		}

		private int column;
	}
}
