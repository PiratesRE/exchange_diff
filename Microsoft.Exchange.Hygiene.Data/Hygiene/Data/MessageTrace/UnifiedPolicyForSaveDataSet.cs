using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class UnifiedPolicyForSaveDataSet : ConfigurablePropertyBag
	{
		static UnifiedPolicyForSaveDataSet()
		{
			foreach (TvpInfo tvpInfo in UnifiedPolicyForSaveDataSet.tvpPrototypeList)
			{
				UnifiedPolicyForSaveDataSet.mapTableToTvpColumnInfo.Add(tvpInfo.TableName, tvpInfo.Columns);
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return this.identity;
			}
		}

		public object PhysicalPartionId
		{
			get
			{
				return this[UnifiedPolicyCommonSchema.PhysicalInstanceKeyProp];
			}
			set
			{
				this[UnifiedPolicyCommonSchema.PhysicalInstanceKeyProp] = value;
			}
		}

		public object FssCopyId
		{
			get
			{
				return this[UnifiedPolicyCommonSchema.FssCopyIdProp];
			}
			set
			{
				this[UnifiedPolicyCommonSchema.FssCopyIdProp] = value;
			}
		}

		public static UnifiedPolicyForSaveDataSet CreateDataSet(object partitionId, IEnumerable<UnifiedPolicyTrace> policyTraceList, int? fssCopyId = null)
		{
			if (partitionId == null)
			{
				throw new ArgumentNullException("partitionId");
			}
			UnifiedPolicyForSaveDataSet unifiedPolicyForSaveDataSet = UnifiedPolicyForSaveDataSet.CreateSkeletonUnifiedPolicyForSaveDataSetObject();
			unifiedPolicyForSaveDataSet.PhysicalPartionId = (int)partitionId;
			if (fssCopyId != null)
			{
				unifiedPolicyForSaveDataSet.FssCopyId = fssCopyId;
			}
			foreach (UnifiedPolicyTrace unifiedPolicyTrace in policyTraceList)
			{
				UnifiedPolicyForSaveDataSet.SerializeObjectToDataTable<UnifiedPolicyTrace>(unifiedPolicyTrace, UnifiedPolicyDataSetSchema.UnifiedPolicyObjectTableProperty, ref unifiedPolicyForSaveDataSet);
				foreach (UnifiedPolicyRule unifiedPolicyRule in unifiedPolicyTrace.Rules)
				{
					UnifiedPolicyForSaveDataSet.SetCommonProperties(unifiedPolicyTrace, unifiedPolicyRule);
					UnifiedPolicyForSaveDataSet.SerializeObjectToDataTable<UnifiedPolicyRule>(unifiedPolicyRule, UnifiedPolicyDataSetSchema.UnifiedPolicyRuleTableProperty, ref unifiedPolicyForSaveDataSet);
					foreach (UnifiedPolicyRuleAction unifiedPolicyRuleAction in unifiedPolicyRule.Actions)
					{
						UnifiedPolicyForSaveDataSet.SetCommonProperties(unifiedPolicyTrace, unifiedPolicyRuleAction);
						UnifiedPolicyForSaveDataSet.SerializeObjectToDataTable<UnifiedPolicyRuleAction>(unifiedPolicyRuleAction, UnifiedPolicyDataSetSchema.UnifiedPolicyRuleActionTableProperty, ref unifiedPolicyForSaveDataSet);
					}
					foreach (UnifiedPolicyRuleClassification unifiedPolicyRuleClassification in unifiedPolicyRule.Classifications)
					{
						UnifiedPolicyForSaveDataSet.SetCommonProperties(unifiedPolicyTrace, unifiedPolicyRuleClassification);
						UnifiedPolicyForSaveDataSet.SerializeObjectToDataTable<UnifiedPolicyRuleClassification>(unifiedPolicyRuleClassification, UnifiedPolicyDataSetSchema.UnifiedPolicyRuleClassificationTableProperty, ref unifiedPolicyForSaveDataSet);
					}
				}
			}
			return unifiedPolicyForSaveDataSet;
		}

		public override Type GetSchemaType()
		{
			return typeof(UnifiedPolicyDataSetSchema);
		}

		public int GetDatasize()
		{
			int num = 0;
			foreach (HygienePropertyDefinition propertyDefinition in UnifiedPolicyForSaveDataSet.tvpDataTables)
			{
				DataTable dataTable = this[propertyDefinition] as DataTable;
				if (dataTable != null)
				{
					num += dataTable.Rows.Count;
				}
			}
			return num;
		}

		private static void SetCommonProperties(UnifiedPolicyTrace trace, ConfigurablePropertyBag dataObject)
		{
			dataObject[UnifiedPolicyCommonSchema.OrganizationalUnitRootProperty] = trace.OrganizationalUnitRoot;
			dataObject[UnifiedPolicyCommonSchema.ObjectIdProperty] = trace.ObjectId;
			dataObject[UnifiedPolicyCommonSchema.DataSourceProperty] = trace.DataSource;
			dataObject[UnifiedPolicyCommonSchema.HashBucketProperty] = trace[UnifiedPolicyCommonSchema.HashBucketProperty];
		}

		private static TvpInfo CreateTvpInfoPrototype(HygienePropertyDefinition tableName, HygienePropertyDefinition[] columnDefinitions)
		{
			HygienePropertyDefinition[] array = new HygienePropertyDefinition[columnDefinitions.Length];
			DataTable dataTable = new DataTable();
			DataColumnCollection columns = dataTable.Columns;
			dataTable.TableName = tableName.Name;
			foreach (HygienePropertyDefinition hygienePropertyDefinition in columnDefinitions)
			{
				if (!hygienePropertyDefinition.IsCalculated)
				{
					DataColumn dataColumn = columns.Add(hygienePropertyDefinition.Name, (hygienePropertyDefinition.Type == typeof(byte[])) ? hygienePropertyDefinition.Type : DalHelper.ConvertToStoreType(hygienePropertyDefinition));
					array[dataColumn.Ordinal] = hygienePropertyDefinition;
				}
			}
			dataTable.BeginLoadData();
			return new TvpInfo(tableName, dataTable, array);
		}

		private static UnifiedPolicyForSaveDataSet CreateSkeletonUnifiedPolicyForSaveDataSetObject()
		{
			UnifiedPolicyForSaveDataSet unifiedPolicyForSaveDataSet = new UnifiedPolicyForSaveDataSet();
			foreach (TvpInfo tvpInfo in UnifiedPolicyForSaveDataSet.tvpPrototypeList)
			{
				unifiedPolicyForSaveDataSet[tvpInfo.TableName] = tvpInfo.Tvp.Clone();
			}
			return unifiedPolicyForSaveDataSet;
		}

		private static void SerializeObjectToDataTable<T>(T source, HygienePropertyDefinition tableDefinition, ref UnifiedPolicyForSaveDataSet saveDataSet) where T : ConfigurablePropertyBag
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			DataTable dataTable = saveDataSet[tableDefinition] as DataTable;
			if (dataTable == null)
			{
				throw new ArgumentNullException("table");
			}
			HygienePropertyDefinition[] columns = UnifiedPolicyForSaveDataSet.mapTableToTvpColumnInfo[tableDefinition];
			DataRow row = dataTable.NewRow();
			UnifiedPolicyForSaveDataSet.PopulateRow(row, columns, source);
			dataTable.Rows.Add(row);
		}

		private static void PopulateRow(DataRow row, HygienePropertyDefinition[] columns, ConfigurablePropertyBag dataSource)
		{
			if (columns == null)
			{
				throw new ArgumentNullException("columns");
			}
			for (int i = 0; i < columns.Length; i++)
			{
				HygienePropertyDefinition hygienePropertyDefinition = columns[i];
				if (hygienePropertyDefinition != null && !hygienePropertyDefinition.IsCalculated)
				{
					object obj = dataSource[hygienePropertyDefinition];
					if (obj != hygienePropertyDefinition.DefaultValue)
					{
						row[i] = obj;
					}
				}
			}
		}

		private static TvpInfo[] tvpPrototypeList = new TvpInfo[]
		{
			UnifiedPolicyForSaveDataSet.CreateTvpInfoPrototype(UnifiedPolicyDataSetSchema.UnifiedPolicyObjectTableProperty, UnifiedPolicyTrace.Properties),
			UnifiedPolicyForSaveDataSet.CreateTvpInfoPrototype(UnifiedPolicyDataSetSchema.UnifiedPolicyRuleTableProperty, UnifiedPolicyRule.Properties),
			UnifiedPolicyForSaveDataSet.CreateTvpInfoPrototype(UnifiedPolicyDataSetSchema.UnifiedPolicyRuleActionTableProperty, UnifiedPolicyRuleAction.Properties),
			UnifiedPolicyForSaveDataSet.CreateTvpInfoPrototype(UnifiedPolicyDataSetSchema.UnifiedPolicyRuleClassificationTableProperty, UnifiedPolicyRuleClassification.Properties)
		};

		private static HygienePropertyDefinition[] tvpDataTables = new HygienePropertyDefinition[]
		{
			UnifiedPolicyDataSetSchema.UnifiedPolicyObjectTableProperty,
			UnifiedPolicyDataSetSchema.UnifiedPolicyRuleTableProperty,
			UnifiedPolicyDataSetSchema.UnifiedPolicyRuleActionTableProperty,
			UnifiedPolicyDataSetSchema.UnifiedPolicyRuleClassificationTableProperty
		};

		private static Dictionary<HygienePropertyDefinition, HygienePropertyDefinition[]> mapTableToTvpColumnInfo = new Dictionary<HygienePropertyDefinition, HygienePropertyDefinition[]>();

		private ObjectId identity = new ConfigObjectId(CombGuidGenerator.NewGuid().ToString());
	}
}
