using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.DxStore.Common;

namespace Microsoft.Exchange.DxStore.HA
{
	public class DataStoreMergedContainer
	{
		public DataStoreMergedContainer(DiffReportVerboseMode diffReportVerboseMode)
		{
			this.Entries = new SortedDictionary<string, DataStoreMergedContainer.KeyEntry>(StringComparer.OrdinalIgnoreCase);
			this.DiffReportVerboseMode = diffReportVerboseMode;
		}

		public bool IsAnalysisComplete { get; private set; }

		public DiffReportVerboseMode DiffReportVerboseMode { get; set; }

		public SortedDictionary<string, DataStoreMergedContainer.KeyEntry> Entries { get; set; }

		public DataStoreDiffReport Report { get; set; }

		public DataStoreMergedContainer.KeyEntry AddOrUpdateKey(string fullKeyName, bool isClusdb)
		{
			DataStoreMergedContainer.KeyEntry keyEntry = null;
			if (!this.Entries.TryGetValue(fullKeyName, out keyEntry) || keyEntry == null)
			{
				keyEntry = new DataStoreMergedContainer.KeyEntry(fullKeyName);
				this.Entries.Add(fullKeyName, keyEntry);
			}
			keyEntry.Update(isClusdb);
			return keyEntry;
		}

		public void Analyze()
		{
			if (!this.IsAnalysisComplete)
			{
				this.IsAnalysisComplete = true;
				foreach (DataStoreMergedContainer.KeyEntry keyEntry in this.Entries.Values)
				{
					keyEntry.Analyze();
				}
				this.Report = DataStoreDiffReport.Create(this.Entries.Values, this.DiffReportVerboseMode);
			}
		}

		public class EntryBase
		{
			public EntryBase(string name)
			{
				this.Name = name;
			}

			public string Name { get; set; }

			public bool IsPresentInClusdb { get; set; }

			public bool IsPresentInDxStore { get; set; }

			public bool IsPresentOnlyInClusdb
			{
				get
				{
					return this.IsPresentInClusdb && !this.IsPresentInDxStore;
				}
			}

			public bool IsPresentOnlyInDxStore
			{
				get
				{
					return this.IsPresentInDxStore && !this.IsPresentInClusdb;
				}
			}

			public bool IsPresentOnBoth
			{
				get
				{
					return this.IsPresentInDxStore && this.IsPresentInClusdb;
				}
			}
		}

		public class PropertyEntry : DataStoreMergedContainer.EntryBase
		{
			public PropertyEntry(string propertyName) : base(propertyName)
			{
			}

			public string ClusdbValue { get; set; }

			public string ClusdbValueKind { get; set; }

			public string DxStoreValue { get; set; }

			public string DxStoreValueKind { get; set; }

			public bool IsValueMatches
			{
				get
				{
					if (this.isValueMatches == null)
					{
						if (base.IsPresentOnBoth && Utils.IsEqual(this.ClusdbValue, this.DxStoreValue, StringComparison.OrdinalIgnoreCase) && Utils.IsEqual(this.ClusdbValueKind, this.DxStoreValueKind, StringComparison.OrdinalIgnoreCase))
						{
							this.isValueMatches = new bool?(true);
						}
						else
						{
							this.isValueMatches = new bool?(false);
						}
					}
					return this.isValueMatches.Value;
				}
			}

			public void Update(string propertyValue, string propertyKind, bool isClusdb)
			{
				string text = propertyValue.Replace("\r\n", "\n");
				if (isClusdb)
				{
					base.IsPresentInClusdb = true;
					this.ClusdbValue = text;
					this.ClusdbValueKind = propertyKind;
					return;
				}
				base.IsPresentInDxStore = true;
				this.DxStoreValue = text;
				this.DxStoreValueKind = propertyKind;
			}

			private bool? isValueMatches;
		}

		public class KeyEntry : DataStoreMergedContainer.EntryBase
		{
			public KeyEntry(string fullKeyName) : base(fullKeyName)
			{
				this.Properties = new SortedDictionary<string, DataStoreMergedContainer.PropertyEntry>(StringComparer.OrdinalIgnoreCase);
			}

			public int PropertyMatchCount { get; set; }

			public int PropertiesOnlyInClusdbCount { get; set; }

			public int PropertiesOnlyInDxStoreCount { get; set; }

			public int PropertyDifferentCount { get; set; }

			public bool IsPropertiesMatch
			{
				get
				{
					return this.PropertyMatchCount == this.Properties.Count;
				}
			}

			public SortedDictionary<string, DataStoreMergedContainer.PropertyEntry> Properties { get; set; }

			public void Update(bool isClusdb)
			{
				if (isClusdb)
				{
					base.IsPresentInClusdb = true;
					return;
				}
				base.IsPresentInDxStore = true;
			}

			public DataStoreMergedContainer.PropertyEntry AddOrUpdateProperty(string propertyName, string propertyValue, string kind, bool isClusdb)
			{
				DataStoreMergedContainer.PropertyEntry propertyEntry = null;
				if (!this.Properties.TryGetValue(propertyName, out propertyEntry) || propertyEntry == null)
				{
					propertyEntry = new DataStoreMergedContainer.PropertyEntry(propertyName);
					this.Properties.Add(propertyName, propertyEntry);
				}
				propertyEntry.Update(propertyValue, kind, isClusdb);
				return propertyEntry;
			}

			public void Analyze()
			{
				if (this.isAnalyzed)
				{
					return;
				}
				foreach (DataStoreMergedContainer.PropertyEntry propertyEntry in this.Properties.Values)
				{
					if (propertyEntry.IsPresentOnlyInClusdb)
					{
						this.PropertiesOnlyInClusdbCount++;
					}
					else if (propertyEntry.IsPresentOnlyInDxStore)
					{
						this.PropertiesOnlyInDxStoreCount++;
					}
					else if (propertyEntry.IsValueMatches)
					{
						this.PropertyMatchCount++;
					}
					else
					{
						this.PropertyDifferentCount++;
					}
				}
				this.isAnalyzed = true;
			}

			public void DumpStats(StringBuilder sb)
			{
				sb.AppendFormat("\nKey:'{0}' Properties - Total: {1} Matching: {2} Different: {3} ClusdbOnly: {4} DxStoreOnly: {5}", new object[]
				{
					base.Name,
					this.Properties.Count,
					this.PropertyMatchCount,
					this.PropertyDifferentCount,
					this.PropertiesOnlyInClusdbCount,
					this.PropertiesOnlyInDxStoreCount
				});
			}

			public void DumpProperties(StringBuilder sb, bool isIncludeValues)
			{
				foreach (DataStoreMergedContainer.PropertyEntry propertyEntry in this.Properties.Values)
				{
					string arg;
					if (propertyEntry.IsPresentOnlyInClusdb)
					{
						arg = "only in clusdb";
					}
					else if (propertyEntry.IsPresentOnlyInClusdb)
					{
						arg = "only in dxstore";
					}
					else
					{
						arg = (propertyEntry.IsValueMatches ? "matches" : "different");
					}
					sb.AppendFormat("\n   {0} : <{1}>", propertyEntry.Name, arg);
					if (isIncludeValues)
					{
						if (propertyEntry.IsPresentInClusdb)
						{
							sb.AppendFormat("\n   [CLS] => ({0}):{1}", propertyEntry.ClusdbValueKind, propertyEntry.ClusdbValue);
						}
						if (propertyEntry.IsPresentInDxStore)
						{
							sb.AppendFormat("\n   [DXS] => ({0}):{1}", propertyEntry.DxStoreValueKind, propertyEntry.DxStoreValue);
						}
					}
				}
			}

			private bool isAnalyzed;
		}
	}
}
