using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Office.Story.V1.ImageAnalysis
{
	[Serializable]
	internal class Bin<T>
	{
		public List<T> Items { get; set; }

		public double RangeStart { get; set; }

		public double RangeEnd { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0:G}, {1:G}]: {2}", new object[]
			{
				this.RangeStart,
				this.RangeEnd,
				this.Items.Count
			});
		}
	}
}
