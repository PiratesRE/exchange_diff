using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.DxStore.HA
{
	public class DataStoreDiffReport
	{
		public DataStoreDiffReport()
		{
			this.KeysOnlyInClusdb = new List<DataStoreMergedContainer.KeyEntry>();
			this.KeysOnlyInDxStore = new List<DataStoreMergedContainer.KeyEntry>();
			this.KeysInBothAndPropertiesMatch = new List<DataStoreMergedContainer.KeyEntry>();
			this.KeysInBothButPropertiesMismatch = new List<DataStoreMergedContainer.KeyEntry>();
		}

		public List<DataStoreMergedContainer.KeyEntry> KeysOnlyInClusdb { get; set; }

		public List<DataStoreMergedContainer.KeyEntry> KeysOnlyInDxStore { get; set; }

		public List<DataStoreMergedContainer.KeyEntry> KeysInBothAndPropertiesMatch { get; set; }

		public List<DataStoreMergedContainer.KeyEntry> KeysInBothButPropertiesMismatch { get; set; }

		public bool IsEverythingMatches
		{
			get
			{
				return this.TotalKeysCount == this.CountKeysInClusdbAndDxStoreAndPropertiesMatch;
			}
		}

		public int TotalKeysCount
		{
			get
			{
				return this.CountKeysOnlyInClusdb + this.CountKeysOnlyInDxStore + this.CountKeysInClusdbAndDxStoreAndPropertiesMatch + this.CountKeysInClusdbAndDxStoreButPropertiesDifferent;
			}
		}

		public int TotalPropertiesCount
		{
			get
			{
				return this.CountPropertiesOnlyInClusdb + this.CountPropertiesOnlyInDxStore + this.CountPropertiesSameInClusdbAndDxStore + this.CountPropertiesDifferentInClusdbAndDxStore;
			}
		}

		public int TotalClusdbKeysCount
		{
			get
			{
				return this.CountKeysOnlyInClusdb + this.CountKeysInClusdbAndDxStoreAndPropertiesMatch + this.CountKeysInClusdbAndDxStoreButPropertiesDifferent;
			}
		}

		public int TotalClusdbPropertiesCount
		{
			get
			{
				return this.CountPropertiesOnlyInClusdb + this.CountPropertiesSameInClusdbAndDxStore + this.CountPropertiesDifferentInClusdbAndDxStore;
			}
		}

		public int TotalDxStoreKeysCount
		{
			get
			{
				return this.CountKeysOnlyInDxStore + this.CountKeysInClusdbAndDxStoreAndPropertiesMatch + this.CountKeysInClusdbAndDxStoreButPropertiesDifferent;
			}
		}

		public int TotalDxStorePropertiesCount
		{
			get
			{
				return this.CountPropertiesOnlyInDxStore + this.CountPropertiesSameInClusdbAndDxStore + this.CountPropertiesDifferentInClusdbAndDxStore;
			}
		}

		public int CountKeysOnlyInClusdb
		{
			get
			{
				if (this.KeysOnlyInClusdb == null)
				{
					return 0;
				}
				return this.KeysOnlyInClusdb.Count;
			}
		}

		public int CountKeysOnlyInDxStore
		{
			get
			{
				if (this.KeysOnlyInDxStore == null)
				{
					return 0;
				}
				return this.KeysOnlyInDxStore.Count;
			}
		}

		public int CountKeysInClusdbAndDxStoreAndPropertiesMatch
		{
			get
			{
				if (this.KeysInBothAndPropertiesMatch == null)
				{
					return 0;
				}
				return this.KeysInBothAndPropertiesMatch.Count;
			}
		}

		public int CountKeysInClusdbAndDxStoreButPropertiesDifferent
		{
			get
			{
				if (this.KeysInBothButPropertiesMismatch == null)
				{
					return 0;
				}
				return this.KeysInBothButPropertiesMismatch.Count;
			}
		}

		public int CountPropertiesOnlyInClusdb { get; set; }

		public int CountPropertiesOnlyInDxStore { get; set; }

		public int CountPropertiesSameInClusdbAndDxStore { get; set; }

		public int CountPropertiesDifferentInClusdbAndDxStore { get; set; }

		public string VerboseReport { get; set; }

		public static DataStoreDiffReport Create(IEnumerable<DataStoreMergedContainer.KeyEntry> keyEntries, DiffReportVerboseMode diffReportVerboseMode)
		{
			DataStoreDiffReport dataStoreDiffReport = new DataStoreDiffReport();
			foreach (DataStoreMergedContainer.KeyEntry keyEntry in keyEntries)
			{
				if (keyEntry.IsPresentOnlyInClusdb)
				{
					dataStoreDiffReport.KeysOnlyInClusdb.Add(keyEntry);
				}
				else if (keyEntry.IsPresentOnlyInDxStore)
				{
					dataStoreDiffReport.KeysOnlyInDxStore.Add(keyEntry);
				}
				else if (keyEntry.IsPropertiesMatch)
				{
					dataStoreDiffReport.KeysInBothAndPropertiesMatch.Add(keyEntry);
				}
				else
				{
					dataStoreDiffReport.KeysInBothButPropertiesMismatch.Add(keyEntry);
				}
				dataStoreDiffReport.CountPropertiesOnlyInClusdb += keyEntry.PropertiesOnlyInClusdbCount;
				dataStoreDiffReport.CountPropertiesOnlyInDxStore += keyEntry.PropertiesOnlyInDxStoreCount;
				dataStoreDiffReport.CountPropertiesSameInClusdbAndDxStore += keyEntry.PropertyMatchCount;
				dataStoreDiffReport.CountPropertiesDifferentInClusdbAndDxStore += keyEntry.PropertyDifferentCount;
			}
			dataStoreDiffReport.VerboseReport = dataStoreDiffReport.GenerateVerboseReport(diffReportVerboseMode);
			return dataStoreDiffReport;
		}

		public void DumpKeys(StringBuilder sb, string title, List<DataStoreMergedContainer.KeyEntry> keyEntries, DiffReportVerboseMode diffReportVerboseMode)
		{
			if (keyEntries != null && keyEntries.Count > 0)
			{
				sb.AppendLine(title);
				sb.AppendLine(new string('=', title.Length));
				foreach (DataStoreMergedContainer.KeyEntry keyEntry in keyEntries)
				{
					keyEntry.DumpStats(sb);
					if (diffReportVerboseMode.HasFlag(DiffReportVerboseMode.ShowPropertyNames))
					{
						keyEntry.DumpProperties(sb, diffReportVerboseMode.HasFlag(DiffReportVerboseMode.ShowPropertyValues));
					}
				}
				sb.AppendLine();
			}
		}

		public string GenerateVerboseReport(DiffReportVerboseMode diffReportVerboseMode)
		{
			if (diffReportVerboseMode.HasFlag(DiffReportVerboseMode.Disabled))
			{
				return "*** Verbose reporting mode is disabled ***";
			}
			StringBuilder stringBuilder = new StringBuilder(1024);
			this.DumpKeys(stringBuilder, "Keys only present in ClusDb", this.KeysOnlyInClusdb, diffReportVerboseMode);
			this.DumpKeys(stringBuilder, "Keys only present in DxStore", this.KeysOnlyInDxStore, diffReportVerboseMode);
			this.DumpKeys(stringBuilder, "Keys present on both but properties not matching", this.KeysInBothButPropertiesMismatch, diffReportVerboseMode);
			if (diffReportVerboseMode.HasFlag(DiffReportVerboseMode.ShowMatchingKeys))
			{
				this.DumpKeys(stringBuilder, "Keys present on both and properties matching", this.KeysInBothAndPropertiesMatch, diffReportVerboseMode);
			}
			return stringBuilder.ToString();
		}

		public const int MaxCharsPerLine = 15360;
	}
}
