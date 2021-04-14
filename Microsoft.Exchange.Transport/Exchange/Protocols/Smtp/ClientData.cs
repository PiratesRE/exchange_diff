using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class ClientData
	{
		public int Count { get; internal set; }

		public bool Discredited
		{
			get
			{
				return this.discredited;
			}
		}

		public void MarkBad()
		{
			this.discredited = true;
		}

		public void MarkGood()
		{
			this.discredited = false;
		}

		private bool discredited;
	}
}
