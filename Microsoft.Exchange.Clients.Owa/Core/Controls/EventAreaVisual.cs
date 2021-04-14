using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class EventAreaVisual : CalendarVisual
	{
		public EventAreaVisual(int dataIndex) : base(dataIndex)
		{
		}

		public bool LeftBreak
		{
			get
			{
				return this.leftBreak;
			}
			set
			{
				this.leftBreak = value;
			}
		}

		public bool RightBreak
		{
			get
			{
				return this.rightBreak;
			}
			set
			{
				this.rightBreak = value;
			}
		}

		public void SetInnerBreak(int position)
		{
			this.innerBreaks |= 1UL << position;
		}

		public ulong InnerBreaks
		{
			get
			{
				return this.innerBreaks;
			}
		}

		private bool leftBreak;

		private bool rightBreak;

		private ulong innerBreaks;
	}
}
