using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Search.Mdb;

namespace Microsoft.Exchange.Search.Engine
{
	internal class CatalogItemStatistics
	{
		internal CatalogItemStatistics(List<MdbInfo> mdbInfoList)
		{
			foreach (MdbInfo mdbInfo in mdbInfoList)
			{
				if (mdbInfo.MountedOnLocalServer)
				{
					this.ActiveItems += mdbInfo.NumberOfItems;
					if (mdbInfo.IsInstantSearchEnabled)
					{
						this.ActiveItemsInstantSearchOn += mdbInfo.NumberOfItems;
					}
					else
					{
						this.ActiveItemsInstantSearchOff += mdbInfo.NumberOfItems;
					}
					if (mdbInfo.IsRefinersEnabled)
					{
						this.ActiveItemsRefinersOn += mdbInfo.NumberOfItems;
					}
					else
					{
						this.ActiveItemsRefinersOff += mdbInfo.NumberOfItems;
					}
				}
				else
				{
					this.PassiveItems += mdbInfo.NumberOfItems;
					if (mdbInfo.IsCatalogSuspended)
					{
						this.PassiveItemsCatalogSuspendedOn += mdbInfo.NumberOfItems;
					}
					else
					{
						this.PassiveItemsCatalogSuspendedOff += mdbInfo.NumberOfItems;
					}
				}
			}
		}

		public long ActiveItems { get; private set; }

		public long PassiveItems { get; private set; }

		public long ActiveItemsInstantSearchOn { get; private set; }

		public long ActiveItemsInstantSearchOff { get; private set; }

		public long ActiveItemsRefinersOn { get; private set; }

		public long ActiveItemsRefinersOff { get; private set; }

		public long PassiveItemsCatalogSuspendedOn { get; private set; }

		public long PassiveItemsCatalogSuspendedOff { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"CatalogItemStatistics =  ActiveItems:",
				this.ActiveItems,
				", PassiveItems:",
				this.PassiveItems,
				", ActiveItemsInstantSearchOn:",
				this.ActiveItemsInstantSearchOn,
				", ActiveItemsInstantSearchOff:",
				this.ActiveItemsInstantSearchOff,
				", ActiveItemsRefinersOn:",
				this.ActiveItemsRefinersOn,
				", ActiveItemsRefinersOff:",
				this.ActiveItemsRefinersOff,
				", PassiveItemsCatalogSuspendedOn:",
				this.PassiveItemsCatalogSuspendedOn,
				", PassiveItemsCatalogSuspendedOff:",
				this.PassiveItemsCatalogSuspendedOff
			});
		}

		internal static string GenerateFeatureStateLoggingInfo(List<MdbInfo> mdbInfoList)
		{
			if (mdbInfoList.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (MdbInfo mdbInfo in mdbInfoList)
			{
				stringBuilder.AppendFormat("{0},{1},{2},{3},{4},{5};", new object[]
				{
					mdbInfo.Name,
					mdbInfo.NumberOfItems,
					mdbInfo.MountedOnLocalServer ? "1" : "0",
					mdbInfo.IsInstantSearchEnabled ? "1" : "0",
					mdbInfo.IsRefinersEnabled ? "1" : "0",
					mdbInfo.IsCatalogSuspended ? "1" : "0"
				});
			}
			return stringBuilder.ToString();
		}

		private const string CatalogItemStatisticsLoggingFormat = "{0},{1},{2},{3},{4},{5};";
	}
}
