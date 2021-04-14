using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class FreeBusyVisual : CalendarVisual
	{
		public FreeBusyVisual(int dataIndex) : base(dataIndex)
		{
		}

		public BusyTypeWrapper FreeBusyIndex
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		private BusyTypeWrapper index;
	}
}
