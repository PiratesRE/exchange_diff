using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.FreeBusy
{
	internal static class QueryFreeBusyItemBySubject
	{
		public static object[][] Query(Folder folder, string subject)
		{
			QueryFreeBusyItemBySubject.Tracer.TraceDebug<string, string>(0L, "Searching for item with subject '{0}' in folder '{1}'", subject, folder.DisplayName);
			QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Subject, subject);
			List<object[]> list = new List<object[]>(1);
			using (QueryResult queryResult = QueryFreeBusyItemBySubject.CreateQueryResult(folder))
			{
				DateTime minValue = DateTime.MinValue;
				queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter);
				object[][] rows = queryResult.GetRows(10);
				if (rows != null)
				{
					foreach (object[] array2 in rows)
					{
						if (!(array2[0] is VersionedId))
						{
							QueryFreeBusyItemBySubject.Tracer.TraceDebug(0L, "Query returned row without id.");
						}
						else
						{
							string y = array2[2] as string;
							if (string.IsNullOrEmpty(subject))
							{
								QueryFreeBusyItemBySubject.Tracer.TraceDebug(0L, "Query returned row without subject.");
							}
							else
							{
								if (!StringComparer.InvariantCultureIgnoreCase.Equals(subject, y))
								{
									break;
								}
								list.Add(array2);
							}
						}
					}
				}
			}
			QueryFreeBusyItemBySubject.Tracer.TraceDebug<int>(0L, "Search resulted in {0} items.", list.Count);
			return list.ToArray();
		}

		private static QueryResult CreateQueryResult(Folder folder)
		{
			return folder.ItemQuery(ItemQueryType.None, null, QueryFreeBusyItemBySubject.sortBy, QueryFreeBusyItemBySubject.properties);
		}

		internal const int IdIndex = 0;

		internal const int LastModifiedTimeIndex = 1;

		internal const int SubjectIndex = 2;

		private const int RowBatch = 10;

		private static readonly PropertyDefinition[] properties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.LastModifiedTime,
			ItemSchema.Subject
		};

		private static readonly SortBy[] sortBy = new SortBy[]
		{
			new SortBy(ItemSchema.Subject, SortOrder.Ascending)
		};

		private static readonly Trace Tracer = ExTraceGlobals.FreeBusyTracer;
	}
}
