using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting.Providers
{
	internal class DalProviderImpl : IDalProvider
	{
		internal DalProviderImpl() : this(new Func<string, object>(DalProviderImpl.CreateTargetObject), 5000)
		{
		}

		internal DalProviderImpl(Func<string, object> activator, int maxPageSize)
		{
			this.createTargetFunction = activator;
			this.maxPageSizeForPagedRequest = maxPageSize;
		}

		public IEnumerable GetSingleDataPage(string targetObjectTypeName, string dalObjectTypeName, string methodName, QueryFilter queryFilter)
		{
			object targetInstance = this.createTargetFunction(targetObjectTypeName);
			MethodInfo retrievalMethod = this.GetRetrievalMethod(targetInstance, dalObjectTypeName, methodName);
			return (IEnumerable)this.RetrieveDalObjects(retrievalMethod, targetInstance, queryFilter);
		}

		public IEnumerable GetAllDataPages(string targetObjectTypeName, string dalObjectTypeName, string methodName, QueryFilter queryFilter)
		{
			object targetInstance = this.createTargetFunction(targetObjectTypeName);
			MethodInfo retrievalMethod = this.GetRetrievalMethod(targetInstance, dalObjectTypeName, methodName);
			IReadOnlyList<ComparisonFilter> nonPagingFilters = this.GetNonPagingFilters(queryFilter);
			List<object> list = new List<object>();
			int num = this.maxPageSizeForPagedRequest;
			int num2 = 1;
			while (num == this.maxPageSizeForPagedRequest)
			{
				QueryFilter filter = this.BuildPagingQueryFilter(nonPagingFilters, num2);
				int count = list.Count;
				list.AddRange((IEnumerable<object>)this.RetrieveDalObjects(retrievalMethod, targetInstance, filter));
				num = list.Count - count;
				num2++;
			}
			return list;
		}

		private object RetrieveDalObjects(MethodInfo method, object targetInstance, QueryFilter filter)
		{
			object result = null;
			try
			{
				FaultInjection.FaultInjectionTracer.TraceTest(4270206269U);
				result = Schema.Utilities.Invoke(method, targetInstance, new object[]
				{
					filter
				});
			}
			catch (Exception ex)
			{
				throw new DalRetrievalException(string.Format("Failed to retrieve DAL objects: {0}", ex), ex.InnerException);
			}
			return result;
		}

		private static object CreateTargetObject(string targetInstanceTypeName)
		{
			return Activator.CreateInstance("Microsoft.Exchange.Hygiene.Data", targetInstanceTypeName).Unwrap();
		}

		private MethodInfo GetRetrievalMethod(object targetInstance, string objectTypeName, string methodName)
		{
			Type type = Type.GetType(objectTypeName);
			MethodInfo method = targetInstance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
			return method.MakeGenericMethod(new Type[]
			{
				type
			});
		}

		private IReadOnlyList<ComparisonFilter> GetNonPagingFilters(QueryFilter originalFilter)
		{
			string pagePropertyName = Schema.Utilities.GetSchemaPropertyDefinition("PageQueryDefinition").Name;
			string pageSizePropertyName = Schema.Utilities.GetSchemaPropertyDefinition("PageSizeQueryDefinition").Name;
			List<ComparisonFilter> list = new List<ComparisonFilter>();
			CompositeFilter compositeFilter = originalFilter as CompositeFilter;
			if (compositeFilter != null)
			{
				list.AddRange(from ComparisonFilter filter in compositeFilter.Filters
				where filter.Property.Name != pagePropertyName && filter.Property.Name != pageSizePropertyName
				select filter);
			}
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.Utilities.GetSchemaPropertyDefinition("PageSizeQueryDefinition"), this.maxPageSizeForPagedRequest));
			return list;
		}

		private QueryFilter BuildPagingQueryFilter(IEnumerable<ComparisonFilter> baseFilters, int page)
		{
			return new AndFilter(new List<ComparisonFilter>(baseFilters)
			{
				new ComparisonFilter(ComparisonOperator.Equal, Schema.Utilities.GetSchemaPropertyDefinition("PageQueryDefinition"), page)
			}.ToArray<QueryFilter>());
		}

		private readonly Func<string, object> createTargetFunction;

		private readonly int maxPageSizeForPagedRequest;
	}
}
