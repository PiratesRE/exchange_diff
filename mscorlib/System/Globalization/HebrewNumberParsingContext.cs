using System;

namespace System.Globalization
{
	internal struct HebrewNumberParsingContext
	{
		public HebrewNumberParsingContext(int result)
		{
			this.state = HebrewNumber.HS.Start;
			this.result = result;
		}

		internal HebrewNumber.HS state;

		internal int result;
	}
}
