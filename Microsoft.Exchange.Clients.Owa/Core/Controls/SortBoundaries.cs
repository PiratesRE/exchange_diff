using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public class SortBoundaries
	{
		public Strings.IDs TextID
		{
			get
			{
				return this.textID;
			}
			set
			{
				this.textID = value;
			}
		}

		public Strings.IDs AscendingID
		{
			get
			{
				return this.ascendingID;
			}
			set
			{
				this.ascendingID = value;
			}
		}

		public Strings.IDs DescendingID
		{
			get
			{
				return this.descendingID;
			}
			set
			{
				this.descendingID = value;
			}
		}

		public SortBoundaries(Strings.IDs textID, Strings.IDs ascendingID, Strings.IDs descendingID)
		{
			this.textID = textID;
			this.ascendingID = ascendingID;
			this.descendingID = descendingID;
		}

		private Strings.IDs textID;

		private Strings.IDs ascendingID;

		private Strings.IDs descendingID;
	}
}
