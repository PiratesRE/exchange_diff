using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ToDoSearchValidation : SearchFolderValidation
	{
		internal ToDoSearchValidation() : base(new IValidator[0])
		{
		}

		internal static SearchFolderCriteria GetToDoSearchCriteria(DefaultFolderContext context)
		{
			QueryFilter queryFilter = new OrFilter(new QueryFilter[]
			{
				new TextFilter(InternalSchema.ItemClass, "IPM.Task", MatchOptions.FullString, MatchFlags.IgnoreCase),
				new TextFilter(InternalSchema.ItemClass, "IPM.Task.", MatchOptions.Prefix, MatchFlags.IgnoreCase)
			});
			QueryFilter queryFilter2 = new NotFilter(new AndFilter(new QueryFilter[]
			{
				new AndFilter(new QueryFilter[]
				{
					new ExistsFilter(InternalSchema.TaskType),
					new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.TaskType, TaskDelegateState.Owned)
				}),
				new AndFilter(new QueryFilter[]
				{
					new ExistsFilter(InternalSchema.TaskAccepted),
					new ComparisonFilter(ComparisonOperator.NotEqual, InternalSchema.TaskAccepted, true)
				})
			}));
			QueryFilter queryFilter3 = new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(InternalSchema.ItemColor),
				new ComparisonFilter(ComparisonOperator.NotEqual, InternalSchema.ItemColor, 0)
			});
			QueryFilter queryFilter4 = new AndFilter(new QueryFilter[]
			{
				new ExistsFilter(InternalSchema.MapiToDoItemFlag),
				new BitMaskFilter(InternalSchema.MapiToDoItemFlag, 1UL, true)
			});
			OrFilter orFilter = new OrFilter(new QueryFilter[]
			{
				new AndFilter(new QueryFilter[]
				{
					new ExistsFilter(InternalSchema.MapiFlagStatus),
					new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.MapiFlagStatus, 1),
					new OrFilter(new QueryFilter[]
					{
						new NotFilter(new ExistsFilter(InternalSchema.ItemColor)),
						new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.ItemColor, 0)
					})
				}),
				new AndFilter(new QueryFilter[]
				{
					new ExistsFilter(InternalSchema.TaskStatus),
					new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.TaskStatus, 2)
				})
			});
			return new SearchFolderCriteria(new AndFilter(new QueryFilter[]
			{
				SearchFolderValidation.GetSearchExclusionFoldersFilter(context, null, SearchFolderValidation.ExcludeFromRemindersSearchFolder),
				new OrFilter(new QueryFilter[]
				{
					new AndFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter2
					}),
					queryFilter3,
					queryFilter4,
					orFilter
				})
			}), new StoreId[]
			{
				context[DefaultFolderType.Root]
			})
			{
				DeepTraversal = true
			};
		}

		internal static bool MatchedToDoSearchCriteriaApproxly(SearchFolderCriteria criteria)
		{
			string text = criteria.SearchQuery.ToString();
			return text != null && (text.IndexOf("IPM.Task") > 0 && text.IndexOf("IPM.Task.") > 0) && text.IndexOf(InternalSchema.ItemColor.Name) > 0;
		}

		protected override void SetPropertiesInternal(DefaultFolderContext context, Folder folder)
		{
			base.SetPropertiesInternal(context, folder);
			SearchFolderCriteria searchFolderCriteria = null;
			SearchFolder searchFolder = (SearchFolder)folder;
			searchFolder[InternalSchema.ContainerClass] = "IPF.Task";
			int num = 786432;
			searchFolder[InternalSchema.ExtendedFolderToDoVersion] = num;
			searchFolder.Save();
			searchFolder.Load(null);
			try
			{
				searchFolderCriteria = searchFolder.GetSearchCriteria();
			}
			catch (ObjectNotInitializedException arg)
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug<string, ObjectNotInitializedException>((long)this.GetHashCode(), "ToDoSearchValidation::SetPropertiesInternal. Failed to get search criteria on the folder with name {0} due to {1}. It can be normal though for a search folder without search criteria applied yet.", searchFolder.DisplayName, arg);
			}
			if (searchFolderCriteria == null || !ToDoSearchValidation.MatchedToDoSearchCriteriaApproxly(searchFolderCriteria))
			{
				ExTraceGlobals.DefaultFoldersTracer.TraceDebug<string, SearchFolderCriteria, SearchFolderCriteria>((long)this.GetHashCode(), "ToDoSearchValidation::SetPropertiesInternal. Apply ToDo search criteria on the folder. DisplayName = {0}, currentCriteria = {1}, newCriteria = {2}.", searchFolder.DisplayName, searchFolderCriteria, ToDoSearchValidation.GetToDoSearchCriteria(context));
				searchFolder.ApplyContinuousSearch(ToDoSearchValidation.GetToDoSearchCriteria(context));
			}
		}
	}
}
