using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.FullTextIndex;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class StoreFullTextIndexHelper
	{
		internal static IDisposable SetFullTextIndexQueryTestHook(Func<IFullTextIndexQuery> testHook)
		{
			return StoreFullTextIndexHelper.FtiQueryCreator.SetTestHook(testHook);
		}

		internal static IList<StoreFullTextIndexQuery> CollectAllFullTextQueries(DataAccessOperator.DataAccessOperatorDefinition dataAccessOperatorDefinition)
		{
			List<StoreFullTextIndexQuery> queries = null;
			if (dataAccessOperatorDefinition != null)
			{
				dataAccessOperatorDefinition.EnumerateDescendants(delegate(DataAccessOperator.DataAccessOperatorDefinition operatorDefinition)
				{
					TableFunctionOperator.TableFunctionOperatorDefinition tableFunctionOperatorDefinition = operatorDefinition as TableFunctionOperator.TableFunctionOperatorDefinition;
					if (tableFunctionOperatorDefinition != null && tableFunctionOperatorDefinition.Parameters != null && tableFunctionOperatorDefinition.Parameters.Length == 1 && tableFunctionOperatorDefinition.Parameters[0] is StoreFullTextIndexQuery)
					{
						if (queries == null)
						{
							queries = new List<StoreFullTextIndexQuery>(3);
						}
						queries.Add((StoreFullTextIndexQuery)tableFunctionOperatorDefinition.Parameters[0]);
					}
				});
			}
			return queries;
		}

		internal static bool IsFullTextIndexTableFunctionOperatorDefinition(DataAccessOperator.DataAccessOperatorDefinition dataAccessOperatorDefinition)
		{
			TableFunctionOperator.TableFunctionOperatorDefinition tableFunctionOperatorDefinition = dataAccessOperatorDefinition as TableFunctionOperator.TableFunctionOperatorDefinition;
			return tableFunctionOperatorDefinition != null && tableFunctionOperatorDefinition.Parameters != null && tableFunctionOperatorDefinition.Parameters.Length == 1 && tableFunctionOperatorDefinition.Parameters[0] is StoreFullTextIndexQuery;
		}

		internal static readonly Hookable<Func<IFullTextIndexQuery>> FtiQueryCreator = Hookable<Func<IFullTextIndexQuery>>.Create(true, () => new FullTextIndexQuery());
	}
}
