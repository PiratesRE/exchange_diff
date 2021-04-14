using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	internal sealed class RefinersResultRow
	{
		private RefinersResultRow(string entryName, long entryCount, double sum, double min, double max)
		{
			this.entryName = entryName;
			this.entryCount = entryCount;
			this.sum = sum;
			this.min = min;
			this.max = max;
		}

		internal string EntryName
		{
			get
			{
				return this.entryName;
			}
		}

		internal long EntryCount
		{
			get
			{
				return this.entryCount;
			}
		}

		internal double Sum
		{
			get
			{
				return this.sum;
			}
		}

		internal double Min
		{
			get
			{
				return this.min;
			}
		}

		internal double Max
		{
			get
			{
				return this.max;
			}
		}

		internal static RefinersResultRow NewRow(string entryName, long entryCount)
		{
			return RefinersResultRow.NewRow(entryName, entryCount, 0.0, 0.0, 0.0);
		}

		internal static RefinersResultRow NewRow(string entryName, long entryCount, double sum, double min, double max)
		{
			Globals.AssertRetail(!string.IsNullOrEmpty(entryName), "Invalid Refiner Entry Name");
			Globals.AssertRetail(entryCount >= 0L, "Invalid Refiner Entry Count");
			return new RefinersResultRow(entryName, entryCount, sum, min, max);
		}

		private readonly string entryName;

		private readonly long entryCount;

		private readonly double sum;

		private readonly double max;

		private readonly double min;
	}
}
