using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.Configuration
{
	internal sealed class AppConfigSession : HygieneSession
	{
		private AppConfigSession()
		{
			this.dataProvider = ConfigDataProviderFactory.Default.Create(DatabaseType.Spam);
		}

		public static AppConfigSession Default
		{
			get
			{
				return AppConfigSession.defaultInstance;
			}
		}

		private IConfigDataProvider DataProvider
		{
			get
			{
				return this.dataProvider;
			}
		}

		public IEnumerable<AppConfigParameter> Find(AppConfigVersion version, params string[] names)
		{
			ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.Equal, AppConfigSchema.ParamVersionProp, version.ToInt64());
			IConfigurable[] source;
			if (names == null || names.Length == 0)
			{
				source = this.DataProvider.Find<AppConfigSchema.AppConfigByVersion>(comparisonFilter, null, false, null);
			}
			else
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				for (int i = 0; i < names.Length; i++)
				{
					string text = names[i];
					if (text == null)
					{
						throw new ArgumentNullException("names", string.Format("Parameter name at index {0} is null.", i));
					}
					if (text.Length > 255)
					{
						throw new ArgumentOutOfRangeException("names", string.Format("Parameter name at index {0} is too long.", i));
					}
					if (!hashSet.Add(text))
					{
						throw new ArgumentException(string.Format("Duplicate parameter name is found at index {0}.", i), "names");
					}
				}
				DataTable dataTable = new DataTable();
				dataTable.TableName = "AppConfigNamesTableType";
				dataTable.Columns.Add("nvc_ParamNames", typeof(string));
				for (int j = 0; j < names.Length; j++)
				{
					dataTable.Rows.Add(new object[]
					{
						names[j]
					});
				}
				ComparisonFilter comparisonFilter2 = new ComparisonFilter(ComparisonOperator.Equal, AppConfigSchema.ParamNamesTableProp, dataTable);
				source = this.DataProvider.Find<AppConfigSchema.AppConfigByName>(new AndFilter(new QueryFilter[]
				{
					comparisonFilter,
					comparisonFilter2
				}), null, false, null);
			}
			return source.Cast<AppConfigParameter>();
		}

		public AppConfigCollection FindValues(AppConfigVersion version, params string[] names)
		{
			IEnumerable<AppConfigParameter> enumerable = this.Find(version, names);
			AppConfigCollection appConfigCollection = new AppConfigCollection();
			foreach (AppConfigParameter appConfigParameter in enumerable)
			{
				appConfigCollection.Add(appConfigParameter.Name, appConfigParameter.Value);
			}
			return appConfigCollection;
		}

		public AppConfigCollection FindValuesByDescription(string[] descriptions, ref string pageCookie)
		{
			if (descriptions.Length == 0)
			{
				throw new ArgumentNullException("descriptions");
			}
			MultiValuedProperty<string> propertyValue = new MultiValuedProperty<string>(descriptions);
			ComparisonFilter baseQueryFilter = new ComparisonFilter(ComparisonOperator.Equal, AppConfigSchema.DescriptionQueryProp, propertyValue);
			List<AppConfigSchema.AppConfigByDescription> list = new List<AppConfigSchema.AppConfigByDescription>();
			AppConfigCollection appConfigCollection = new AppConfigCollection();
			bool flag = false;
			while (!flag)
			{
				QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(baseQueryFilter, pageCookie);
				IEnumerable<AppConfigSchema.AppConfigByDescription> collection = this.DataProvider.FindPaged<AppConfigSchema.AppConfigByDescription>(pagingQueryFilter, null, false, null, 1000);
				list.AddRange(collection);
				pageCookie = PagingHelper.GetProcessedCookie(pagingQueryFilter, out flag);
			}
			if (!list.Any<AppConfigSchema.AppConfigByDescription>())
			{
				return appConfigCollection;
			}
			foreach (AppConfigParameter appConfigParameter in list)
			{
				appConfigCollection.Add(appConfigParameter.Name, appConfigParameter.Value);
			}
			return appConfigCollection;
		}

		public void Delete(IEnumerable<AppConfigParameter> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			int num = AppConfigSession.Inspect<AppConfigParameter>(items, AppConfigSession.AppConfigParameterEqualityComparer.Default);
			if (num == 0)
			{
				throw new ArgumentOutOfRangeException("items");
			}
			if (num < 0)
			{
				throw new ArgumentException("Duplicate parameters specified.", "items");
			}
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (AppConfigParameter appConfigParameter in items)
			{
				Guid identity = AppConfigSession.GenerateParameterIdentity();
				batchPropertyTable.AddPropertyValue(identity, AppConfigSchema.ParamVersionProp, appConfigParameter.Version.ToInt64());
				batchPropertyTable.AddPropertyValue(identity, AppConfigSchema.ParamNameProp, appConfigParameter.Name);
			}
			AppConfigSchema.AppConfigNameVersions appConfigNameVersions = new AppConfigSchema.AppConfigNameVersions(batchPropertyTable);
			AuditHelper.ApplyAuditProperties(appConfigNameVersions, default(Guid), null);
			this.DataProvider.Delete(appConfigNameVersions);
		}

		public void Save(IEnumerable<AppConfigParameter> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			int num = AppConfigSession.Inspect<AppConfigParameter>(items, AppConfigSession.AppConfigParameterEqualityComparer.Default);
			if (num == 0)
			{
				throw new ArgumentOutOfRangeException("items");
			}
			if (num < 0)
			{
				throw new ArgumentException("Duplicate parameters specified.", "items");
			}
			BatchPropertyTable batchPropertyTable = new BatchPropertyTable();
			foreach (AppConfigParameter appConfigParameter in items)
			{
				Guid identity = AppConfigSession.GenerateParameterIdentity();
				batchPropertyTable.AddPropertyValue(identity, AppConfigSchema.ParamVersionProp, appConfigParameter.Version.ToInt64());
				batchPropertyTable.AddPropertyValue(identity, AppConfigSchema.ParamNameProp, appConfigParameter.Name);
				batchPropertyTable.AddPropertyValue(identity, AppConfigSchema.ParamValueProp, appConfigParameter.Value);
				batchPropertyTable.AddPropertyValue(identity, AppConfigSchema.DescriptionProp, appConfigParameter.Description);
			}
			AppConfigSchema.AppConfigItems appConfigItems = new AppConfigSchema.AppConfigItems(batchPropertyTable);
			AuditHelper.ApplyAuditProperties(appConfigItems, default(Guid), null);
			this.DataProvider.Save(appConfigItems);
		}

		private static int Inspect<T>(IEnumerable<T> items, IEqualityComparer<T> comparer)
		{
			HashSet<T> hashSet = new HashSet<T>(comparer);
			foreach (T item in items)
			{
				if (!hashSet.Add(item))
				{
					return -1;
				}
			}
			return hashSet.Count;
		}

		private static Guid GenerateParameterIdentity()
		{
			return Guid.NewGuid();
		}

		private static readonly AppConfigSession defaultInstance = new AppConfigSession();

		private readonly IConfigDataProvider dataProvider;

		private sealed class AppConfigParameterEqualityComparer : IEqualityComparer<AppConfigParameter>
		{
			private AppConfigParameterEqualityComparer()
			{
			}

			public bool Equals(AppConfigParameter x, AppConfigParameter y)
			{
				return x.Version.Equals(y.Version) && StringComparer.OrdinalIgnoreCase.Equals(x.Name, y.Name);
			}

			public int GetHashCode(AppConfigParameter obj)
			{
				return obj.Version.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
			}

			public static readonly AppConfigSession.AppConfigParameterEqualityComparer Default = new AppConfigSession.AppConfigParameterEqualityComparer();
		}
	}
}
