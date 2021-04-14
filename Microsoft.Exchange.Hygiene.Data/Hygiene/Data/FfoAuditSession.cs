using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data
{
	[Serializable]
	internal class FfoAuditSession : IFfoAuditSession
	{
		public DatabaseType DatabaseType { get; private set; }

		public FfoAuditSession(DatabaseType databaseType) : this(ConfigDataProviderFactory.Default.Create(databaseType))
		{
			if (!FfoAuditSession.IsDatabaseTypeSupported(databaseType))
			{
				IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
				string format = "The session FfoAuditSession doesn't support the database {0}. The following types are supported: {1}.";
				object[] array = new object[2];
				array[0] = databaseType;
				array[1] = string.Join(", ", from dt in FfoAuditSession.supportedDatabases
				select dt.ToString());
				throw new ArgumentException(string.Format(invariantCulture, format, array), "databaseType");
			}
			this.DatabaseType = databaseType;
		}

		internal FfoAuditSession(IConfigDataProvider dataProvider)
		{
			this.dataProvider = dataProvider;
		}

		public IEnumerable<AuditProperty> FindAuditPropertiesByInstance(Guid partitionId, Guid instanceId, string entityName)
		{
			if (instanceId == Guid.Empty)
			{
				throw new ArgumentNullException("instanceId");
			}
			if (string.IsNullOrEmpty(entityName))
			{
				throw new ArgumentNullException("entityName");
			}
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, AuditProperty.EntityNameProp, entityName),
				new ComparisonFilter(ComparisonOperator.Equal, AuditProperty.InstanceIdProp, instanceId),
				new ComparisonFilter(ComparisonOperator.Equal, AuditProperty.PartitionIdProp, partitionId)
			});
			return this.dataProvider.Find<AuditProperty>(filter, null, true, null).Cast<AuditProperty>();
		}

		public IEnumerable<AuditProperty> FindAuditPropertiesByAuditId(Guid partitionId, Guid auditId)
		{
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, AuditProperty.AuditIdProp, auditId),
				new ComparisonFilter(ComparisonOperator.Equal, AuditProperty.PartitionIdProp, partitionId)
			});
			return this.dataProvider.Find<AuditProperty>(filter, null, true, null).Cast<AuditProperty>();
		}

		public IEnumerable<AuditHistoryResult> FindAuditHistory(string entityName, Guid? entityInstanceId, Guid partitionId, DateTime startTime, DateTime? endTime)
		{
			if (string.IsNullOrWhiteSpace(entityName) && (entityInstanceId == null || entityInstanceId == Guid.Empty))
			{
				throw new ArgumentException("You must provide a value to either the entityName or the entityInstanceId.", "entityName");
			}
			if (partitionId == Guid.Empty)
			{
				throw new ArgumentException("You must provide a non-empty partitionId.", "partitionId");
			}
			if (endTime != null && startTime > endTime)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The endTime (endTime = {0}) cannot be less than the startTime (startTime = {1}).", new object[]
				{
					endTime.Value,
					startTime
				}), "endTime");
			}
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.PartitionIdParameterDefinition, partitionId),
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.EntityNameParameterDefinition, entityName),
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.EntityInstanceIdParameterDefinition, entityInstanceId),
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.StartTimeParameterDefinition, startTime),
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.EndTimeParameterDefinition, endTime)
			});
			IConfigurable[] array = this.dataProvider.Find<AuditHistoryResult>(filter, null, true, null);
			return (array != null) ? array.Cast<AuditHistoryResult>() : Enumerable.Empty<AuditHistoryResult>();
		}

		public IEnumerable<AuditHistoryResult> SearchAuditHistory(string entityName, string searchString, Guid? entityInstanceId, Guid partitionId, DateTime startTime, DateTime? endTime)
		{
			if (string.IsNullOrWhiteSpace(entityName))
			{
				throw new ArgumentException("You must provide a value to entityName.", "entityName");
			}
			if (string.IsNullOrWhiteSpace(searchString))
			{
				throw new ArgumentException("You must provide a non-empty pattern to search.", "searchString");
			}
			if (endTime != null && startTime > endTime)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The endTime (endTime = {0}) cannot be less than the startTime (startTime = {1}).", new object[]
				{
					endTime.Value,
					startTime
				}), "endTime");
			}
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.PartitionIdParameterDefinition, partitionId),
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.EntityNameParameterDefinition, entityName),
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.EntityInstanceIdParameterDefinition, entityInstanceId),
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.PropertyValueStringDefinition, string.Format("%{0}%", searchString)),
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.StartTimeParameterDefinition, startTime),
				new ComparisonFilter(ComparisonOperator.Equal, AuditHistoryResult.EndTimeParameterDefinition, endTime)
			});
			IConfigurable[] array = this.dataProvider.Find<AuditHistoryResult>(filter, null, true, null);
			return (array != null) ? array.Cast<AuditHistoryResult>() : Enumerable.Empty<AuditHistoryResult>();
		}

		public void SetEntityData(Guid partitionId, string tableName, string columnName, string condition, string newValue)
		{
			if (partitionId == Guid.Empty)
			{
				throw new ArgumentException("You must provide a non-empty partitionId.", "partitionId");
			}
			if (string.IsNullOrWhiteSpace(tableName))
			{
				throw new ArgumentNullException("tableName");
			}
			if (string.IsNullOrWhiteSpace(columnName))
			{
				throw new ArgumentNullException("columnName");
			}
			if (string.IsNullOrWhiteSpace(condition))
			{
				throw new ArgumentNullException("condition");
			}
			this.dataProvider.Save(new SetEntityDataRequest
			{
				PartitionId = partitionId,
				TableName = tableName,
				ColumnName = columnName,
				Condition = condition,
				NewValue = newValue
			});
		}

		private static bool IsDatabaseTypeSupported(DatabaseType databaseType)
		{
			return FfoAuditSession.supportedDatabases.Contains(databaseType);
		}

		private static readonly HashSet<DatabaseType> supportedDatabases = new HashSet<DatabaseType>
		{
			DatabaseType.BackgroundJobBackend,
			DatabaseType.Directory,
			DatabaseType.Domain,
			DatabaseType.Kes,
			DatabaseType.Mtrt,
			DatabaseType.Spam
		};

		private IConfigDataProvider dataProvider;
	}
}
