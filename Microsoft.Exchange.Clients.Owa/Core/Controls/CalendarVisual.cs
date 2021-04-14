using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public abstract class CalendarVisual
	{
		public CalendarVisual(int dataIndex)
		{
			if (dataIndex < 0)
			{
				throw new ArgumentOutOfRangeException("dataIndex");
			}
			this.dataIndex = dataIndex;
			this.rect = new Rect();
			this.adjustedRect = new Rect();
		}

		public Rect Rect
		{
			get
			{
				return this.rect;
			}
		}

		public Rect AdjustedRect
		{
			get
			{
				return this.adjustedRect;
			}
		}

		public int DataIndex
		{
			get
			{
				return this.dataIndex;
			}
		}

		private Rect adjustedRect;

		private Rect rect;

		private int dataIndex;
	}
}
