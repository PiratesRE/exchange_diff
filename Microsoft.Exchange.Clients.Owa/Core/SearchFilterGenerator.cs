using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Search;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class SearchFilterGenerator : IFilterGenerator
	{
		private SearchFilterGenerator(QueryFilter advancedQueryFilter, CultureInfo userCultureInfo, IPolicyTagProvider policyTagProvider)
		{
			this.advancedQueryFilter = advancedQueryFilter;
			this.userCultureInfo = userCultureInfo;
			this.policyTagProvider = policyTagProvider;
		}

		public static AqsParser.ParseOption GetAqsParseOption(Folder folder, bool isContentIndexingEnabled)
		{
			AqsParser.ParseOption parseOption = AqsParser.ParseOption.SuppressError;
			if (!isContentIndexingEnabled)
			{
				parseOption |= AqsParser.ParseOption.ContentIndexingDisabled;
			}
			bool flag = Array.Exists<string>(SearchFilterGenerator.prefixAllowedFolderList, (string item) => string.Equals(item, folder.ClassName, StringComparison.OrdinalIgnoreCase));
			flag |= (folder is SearchFolder || folder.Session is PublicFolderSession);
			if (Globals.DisablePrefixSearch && !flag)
			{
				parseOption |= AqsParser.ParseOption.DisablePrefixMatch;
			}
			return parseOption;
		}

		public static QueryFilter Execute(string searchString, bool isContentIndexingEnabled, CultureInfo userCultureInfo, IPolicyTagProvider policyTagProvider, Folder folder, SearchScope searchScope, QueryFilter advancedQueryFilter)
		{
			SearchFilterGenerator searchFilterGenerator = new SearchFilterGenerator(advancedQueryFilter, userCultureInfo, policyTagProvider);
			return searchFilterGenerator.Execute(searchString, isContentIndexingEnabled, folder, searchScope);
		}

		public QueryFilter Execute(string searchString, bool isContentIndexingEnabled, Folder folder, SearchScope searchScope)
		{
			this.searchScope = searchScope;
			this.folderClass = folder.ClassName;
			if (searchString != null)
			{
				this.queryFilter = AqsParser.ParseAndBuildQuery(searchString, SearchFilterGenerator.GetAqsParseOption(folder, isContentIndexingEnabled), this.userCultureInfo, RescopedAll.Default, null, this.policyTagProvider);
			}
			if (this.advancedQueryFilter != null)
			{
				if (this.queryFilter == null)
				{
					this.queryFilter = this.advancedQueryFilter;
				}
				else
				{
					this.queryFilter = new AndFilter(new QueryFilter[]
					{
						this.queryFilter,
						this.advancedQueryFilter
					});
				}
			}
			if (this.queryFilter == null)
			{
				return null;
			}
			this.AddItemTypeFilter();
			return this.queryFilter;
		}

		private void AddItemTypeFilter()
		{
			if (this.searchScope == SearchScope.SelectedFolder)
			{
				return;
			}
			string[] array = null;
			if (ObjectClass.IsContactsFolder(this.folderClass))
			{
				array = SearchFilterGenerator.contactModuleIncludeList;
			}
			else if (ObjectClass.IsTaskFolder(this.folderClass))
			{
				array = SearchFilterGenerator.taskModuleIncludeList;
			}
			if (array != null)
			{
				QueryFilter[] array2 = new QueryFilter[array.Length * 2];
				for (int i = 0; i < array.Length; i++)
				{
					array2[i * 2] = new TextFilter(StoreObjectSchema.ItemClass, array[i], MatchOptions.FullString, MatchFlags.IgnoreCase);
					array2[i * 2 + 1] = new TextFilter(StoreObjectSchema.ItemClass, array[i] + ".", MatchOptions.Prefix, MatchFlags.IgnoreCase);
				}
				this.queryFilter = new AndFilter(new QueryFilter[]
				{
					this.queryFilter,
					new OrFilter(array2)
				});
			}
		}

		private static string[] contactModuleIncludeList = new string[]
		{
			"IPM.Contact",
			"IPM.DistList"
		};

		private static string[] taskModuleIncludeList = new string[]
		{
			"IPM.Task",
			"IPM.TaskRequest"
		};

		private static string[] prefixAllowedFolderList = new string[]
		{
			"IPF.Contact"
		};

		private SearchScope searchScope;

		private string folderClass;

		private QueryFilter advancedQueryFilter;

		private CultureInfo userCultureInfo;

		private QueryFilter queryFilter;

		private IPolicyTagProvider policyTagProvider;
	}
}
