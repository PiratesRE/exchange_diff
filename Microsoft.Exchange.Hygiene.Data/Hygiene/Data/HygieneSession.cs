using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class HygieneSession
	{
		public static SqlPropertyDefinition[] FindPropertyDefinition(DatabaseType databaseType, string entityName = null, string propertyName = null, int? entityId = null, int? propertyId = null)
		{
			IConfigDataProvider configDataProvider = ConfigDataProviderFactory.Default.Create(databaseType);
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, SqlPropertyDefinition.EntityNameProp, entityName),
				new ComparisonFilter(ComparisonOperator.Equal, SqlPropertyDefinition.PropertyNameProp, propertyName),
				new ComparisonFilter(ComparisonOperator.Equal, SqlPropertyDefinition.EntityIdProp, entityId),
				new ComparisonFilter(ComparisonOperator.Equal, SqlPropertyDefinition.PropertyIdProp, propertyId)
			});
			return configDataProvider.Find<SqlPropertyDefinition>(filter, null, true, null).Cast<SqlPropertyDefinition>().ToArray<SqlPropertyDefinition>();
		}

		public static void SavePropertyDefinition(DatabaseType databaseType, SqlPropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			IConfigDataProvider configDataProvider = ConfigDataProviderFactory.Default.Create(databaseType);
			configDataProvider.Save(propertyDefinition);
		}

		public static void DeletePropertyDefinition(DatabaseType databaseType, SqlPropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			IConfigDataProvider configDataProvider = ConfigDataProviderFactory.Default.Create(databaseType);
			configDataProvider.Delete(propertyDefinition);
		}

		internal static void ValidateDBVersion(DatabaseType databaseType)
		{
			IConfigDataProvider configDataProvider = ConfigDataProviderFactory.Default.Create(databaseType);
			configDataProvider.Find<SqlDBVersion>(null, null, false, null).Cast<SqlDBVersion>().ToArray<SqlDBVersion>();
		}
	}
}
