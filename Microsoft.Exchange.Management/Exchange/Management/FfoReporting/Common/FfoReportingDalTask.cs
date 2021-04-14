using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Data;
using Microsoft.Exchange.Management.FfoReporting.Providers;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	public abstract class FfoReportingDalTask<TOutputObject> : FfoReportingTask<TOutputObject>, IPageableTask where TOutputObject : new()
	{
		public FfoReportingDalTask()
		{
			this.Page = 1;
			this.PageSize = 1000;
		}

		public FfoReportingDalTask(string dalTypeName) : this()
		{
			this.DalObjectTypeName = dalTypeName;
		}

		public virtual string DataSessionTypeName
		{
			get
			{
				return "Microsoft.Exchange.Hygiene.Data.MessageTrace.MessageTraceSession";
			}
		}

		public virtual string DataSessionMethodName
		{
			get
			{
				return "FindReportObject";
			}
		}

		public abstract string DalMonitorEventName { get; }

		[CmdletValidator("ValidateIntRange", new object[]
		{
			1,
			1000
		}, ErrorMessage = Strings.IDs.InvalidPage)]
		[Parameter(Mandatory = false)]
		[QueryParameter("PageQueryDefinition", new string[]
		{

		})]
		public int Page { get; set; }

		[QueryParameter("PageSizeQueryDefinition", new string[]
		{

		})]
		[CmdletValidator("ValidateIntRange", new object[]
		{
			1,
			5000
		}, ErrorMessage = Strings.IDs.InvalidPageSize)]
		[Parameter(Mandatory = false)]
		public int PageSize { get; set; }

		protected string DalObjectTypeName { get; set; }

		protected override IReadOnlyList<TOutputObject> AggregateOutput()
		{
			IEnumerable dalRecords = this.GetDalRecords(new FfoReportingDalTask<TOutputObject>.DalRetrievalDelegate(ServiceLocator.Current.GetService<IDalProvider>().GetSingleDataPage), null);
			List<IDataProcessor> list = new List<IDataProcessor>();
			int startIndex = (this.Page - 1) * this.PageSize;
			list.Add(ConversionProcessor.CreatePageable<TOutputObject>(this, startIndex));
			if (base.NeedSuppressingPiiData)
			{
				list.Add(RedactionProcessor.Create<TOutputObject>());
			}
			return DataProcessorDriver.Process<TOutputObject>(dalRecords, list);
		}

		protected IEnumerable GetDalRecords(FfoReportingDalTask<TOutputObject>.DalRetrievalDelegate getDataPageMethod, QueryFilter filter = null)
		{
			IEnumerable result;
			try
			{
				IEnumerable enumerable = getDataPageMethod(this.DataSessionTypeName, this.DalObjectTypeName, this.DataSessionMethodName, filter ?? this.BuildQueryFilter());
				result = enumerable;
			}
			catch (DalRetrievalException ex)
			{
				base.Diagnostics.SetHealthRed(this.DalMonitorEventName, string.Format("Error retrieving data from the DAL: {0}", ex.ToString()), ex);
				throw;
			}
			return result;
		}

		protected QueryFilter BuildQueryFilter()
		{
			List<ComparisonFilter> list = new List<ComparisonFilter>();
			Guid externalDirectoryOrganizationId = ServiceLocator.Current.GetService<IAuthenticationProvider>().GetExternalDirectoryOrganizationId(base.CurrentOrganizationId);
			list.Add(new ComparisonFilter(ComparisonOperator.Equal, Schema.Utilities.GetSchemaPropertyDefinition("OrganizationQueryDefinition"), externalDirectoryOrganizationId));
			foreach (Tuple<PropertyInfo, QueryParameter> tuple in Schema.Utilities.GetProperties<QueryParameter>(base.GetType()))
			{
				PropertyInfo item = tuple.Item1;
				QueryParameter item2 = tuple.Item2;
				if (item != null && item2 != null)
				{
					item2.AddFilter(list, item.GetValue(this));
				}
			}
			AndFilter result = new AndFilter(list.ToArray());
			base.Diagnostics.Checkpoint("BuildQueryFilter");
			return result;
		}

		protected delegate IEnumerable DalRetrievalDelegate(string dataSessionTypeName, string dalObjectTypeName, string dataSessionMethodName, QueryFilter filter);
	}
}
