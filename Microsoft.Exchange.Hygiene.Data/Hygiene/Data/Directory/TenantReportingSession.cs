using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class TenantReportingSession : HygieneSession
	{
		public TenantReportingSession()
		{
			this.dataProviderDirectory = ConfigDataProviderFactory.Default.Create(DatabaseType.Directory);
		}

		public int PartitionCount
		{
			get
			{
				return this.GetAllPhysicalPartitions().Length;
			}
		}

		public void Save(IConfigurable obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(obj.GetType().Name);
			}
			if (obj is OnDemandQueryRequest)
			{
				OnDemandQueryRequest reportRequest = obj as OnDemandQueryRequest;
				this.Save(reportRequest);
			}
			else
			{
				if (!(obj is ReportSchedule))
				{
					throw new NotSupportedException("This object type cannot be saved through this session object");
				}
				ReportSchedule reportSchedule = obj as ReportSchedule;
				this.Save(reportSchedule);
			}
			this.dataProviderDirectory.Save(obj);
		}

		public IEnumerable<ReportSchedule> FindReportSchedules(Guid tenantId, string scheduleName = null, int pageSize = 100)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ReportScheduleSchema.TenantId, tenantId);
			if (!string.IsNullOrWhiteSpace(scheduleName))
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.Equal, ReportScheduleSchema.ScheduleName, scheduleName)
				});
				return this.dataProviderDirectory.Find<ReportSchedule>(queryFilter, null, false, null).Cast<ReportSchedule>().Cache<ReportSchedule>();
			}
			return new ConfigDataProviderPagedReader<ReportSchedule>(this.dataProviderDirectory, null, queryFilter, null, pageSize);
		}

		public IEnumerable<OnDemandQueryRequest> FindOnDemandReportRequests(Guid tenantId, Guid? requestId = null, IEnumerable<OnDemandQueryRequestStatus> requestStatuses = null, DateTime? submissionDateTimeStart = null, int pageSize = 100)
		{
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, tenantId);
			if (requestId != null)
			{
				return (from OnDemandQueryRequest r in this.dataProviderDirectory.Find<OnDemandQueryRequest>(QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.Equal, OnDemandQueryRequestSchema.RequestId, requestId.Value)
				}), null, false, null)
				where submissionDateTimeStart == null || r.SubmissionTime >= submissionDateTimeStart.Value.Subtract(TimeSpan.FromSeconds(1.0))
				select r).Cache<OnDemandQueryRequest>();
			}
			return (from r in new ConfigDataProviderPagedReader<OnDemandQueryRequest>(this.dataProviderDirectory, null, queryFilter, null, pageSize)
			where r.TenantId == tenantId && (requestStatuses == null || requestStatuses.Any((OnDemandQueryRequestStatus s) => s == r.RequestStatus)) && (submissionDateTimeStart == null || r.SubmissionTime >= submissionDateTimeStart.Value.Subtract(TimeSpan.FromSeconds(1.0)))
			select r).Cache<OnDemandQueryRequest>();
		}

		public IEnumerable<ReportSchedule> FindReportSchedules(int partitionId, ref string pageCookie, out bool complete, int pageSize = 100)
		{
			QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, this.GetPartition(partitionId)), pageCookie);
			IEnumerable<ReportSchedule> result = this.dataProviderDirectory.FindPaged<ReportSchedule>(pagingQueryFilter, null, false, null, pageSize).Cast<ReportSchedule>().Cache<ReportSchedule>();
			pageCookie = PagingHelper.GetProcessedCookie(pagingQueryFilter, out complete);
			return result;
		}

		public IEnumerable<OnDemandQueryRequest> FindOnDemandReportsForScheduling(int partitionId, IEnumerable<OnDemandQueryType> queryTypes, ref string pageCookie, out bool complete, int pageSize = 100)
		{
			QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(QueryFilter.AndTogether(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, this.GetPartition(partitionId)),
				new ComparisonFilter(ComparisonOperator.Equal, OnDemandQueryRequestSchema.Container, OnDemandQueryRequest.DefaultContainer)
			}), pageCookie);
			IEnumerable<OnDemandQueryRequest> result = (from OnDemandQueryRequest r in this.dataProviderDirectory.FindPaged<OnDemandQueryRequest>(pagingQueryFilter, null, false, null, pageSize)
			where queryTypes.Any((OnDemandQueryType t) => t == r.QueryType)
			select r).Cache<OnDemandQueryRequest>();
			pageCookie = PagingHelper.GetProcessedCookie(pagingQueryFilter, out complete);
			return result;
		}

		public IEnumerable<OnDemandQueryRequest> FindOnDemandReportsForScheduling(IEnumerable<OnDemandQueryType> queryTypes, IEnumerable<OnDemandQueryRequestStatus> requestStatuses, ref string pageCookie, out bool complete, int pageSize = 100)
		{
			complete = true;
			Dictionary<int, string> dictionary = this.FromPageCookieString(pageCookie);
			IEnumerable<OnDemandQueryRequest> enumerable = Enumerable.Empty<OnDemandQueryRequest>();
			int num = 0;
			while (num < this.PartitionCount && enumerable.Count<OnDemandQueryRequest>() < pageSize)
			{
				bool flag = false;
				string empty = string.Empty;
				QueryFilter pagingQueryFilter = PagingHelper.GetPagingQueryFilter(QueryFilter.AndTogether(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PhysicalInstanceKeyProp, this.GetPartition(num)),
					new ComparisonFilter(ComparisonOperator.Equal, OnDemandQueryRequestSchema.Container, OnDemandQueryRequest.DefaultContainer)
				}), dictionary[num]);
				enumerable = enumerable.Concat((from OnDemandQueryRequest r in this.dataProviderDirectory.FindPaged<OnDemandQueryRequest>(pagingQueryFilter, null, false, null, pageSize)
				where queryTypes.Any((OnDemandQueryType t) => t == r.QueryType)
				select r).Cache<OnDemandQueryRequest>());
				dictionary[num] = PagingHelper.GetProcessedCookie(pagingQueryFilter, out flag);
				if (!flag)
				{
					complete = false;
				}
				num++;
			}
			pageCookie = this.ToPageCookieString(dictionary);
			return enumerable;
		}

		public void SetExecutionStatus(Guid tenantId, string scheduleName, Guid executionContextId, ReportExecutionStatusType executionStatus)
		{
			ReportSchedule reportSchedule = this.GetReportSchedule(tenantId, scheduleName);
			reportSchedule.SetExecutionStatus(executionContextId, executionStatus);
			this.Save(reportSchedule);
		}

		public void SetExecutionContext(Guid tenantId, string scheduleName, Guid executionContextId)
		{
			ReportSchedule reportSchedule = this.GetReportSchedule(tenantId, scheduleName);
			reportSchedule.SetExecutionContext(executionContextId);
			this.Save(reportSchedule);
		}

		public object[] GetAllPhysicalPartitions()
		{
			return ((IPartitionedDataProvider)this.dataProviderDirectory).GetAllPhysicalPartitions();
		}

		private void Save(ReportSchedule reportSchedule)
		{
			this.dataProviderDirectory.Save(reportSchedule);
		}

		private void Save(OnDemandQueryRequest reportRequest)
		{
			if (!reportRequest.GetPropertyDefinitions(true).Any((PropertyDefinition r) => r.Name == OnDemandQueryRequestSchema.Container.Name))
			{
				if (reportRequest.GetPropertyDefinitions(true).Any((PropertyDefinition r) => r.Name == OnDemandQueryRequestSchema.RequestStatus.Name))
				{
					reportRequest[OnDemandQueryRequestSchema.Container] = reportRequest[OnDemandQueryRequestSchema.RequestStatus].ToString();
				}
				else
				{
					OnDemandQueryRequest onDemandQueryRequest = this.FindOnDemandReportRequests(reportRequest.TenantId, new Guid?(reportRequest.RequestId), null, null, 100).FirstOrDefault<OnDemandQueryRequest>();
					if (onDemandQueryRequest != null)
					{
						reportRequest[OnDemandQueryRequestSchema.Container] = onDemandQueryRequest[OnDemandQueryRequestSchema.Container];
					}
					else
					{
						reportRequest[OnDemandQueryRequestSchema.Container] = OnDemandQueryRequest.DefaultContainer;
					}
				}
			}
			this.dataProviderDirectory.Save(reportRequest);
		}

		private ReportSchedule GetReportSchedule(Guid tenantId, string scheduleName)
		{
			List<ReportSchedule> list = this.FindReportSchedules(tenantId, scheduleName, 100).ToList<ReportSchedule>();
			if (list == null || list.Count != 1)
			{
				throw new InvalidDataException(string.Format("The schedule '{0}' could not be found for tenant: {1}", scheduleName ?? string.Empty, tenantId));
			}
			return list[0];
		}

		private object GetPartition(int partitionId)
		{
			if (partitionId >= this.PartitionCount)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid Parition Id {0} passed. Valid partition values are 0 to {1}", partitionId, this.PartitionCount - 1));
			}
			return this.GetAllPhysicalPartitions()[partitionId];
		}

		private Dictionary<int, string> GetEmptyCookieDict()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>(this.PartitionCount);
			for (int i = 0; i < this.PartitionCount; i++)
			{
				dictionary[i] = null;
			}
			return dictionary;
		}

		private Dictionary<int, string> FromPageCookieString(string pageCookie)
		{
			Dictionary<int, string> emptyCookieDict = this.GetEmptyCookieDict();
			if (!string.IsNullOrWhiteSpace(pageCookie))
			{
				string[] array = pageCookie.Split(new char[]
				{
					'#',
					';'
				});
				int num = 0;
				if (array.Length == this.PartitionCount * 2)
				{
					int key;
					while (num < array.Length && int.TryParse(array[num], out key))
					{
						emptyCookieDict[key] = array[num + 1];
						num += 2;
					}
				}
			}
			return emptyCookieDict;
		}

		private string ToPageCookieString(Dictionary<int, string> cookieDict)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (cookieDict != null)
			{
				for (int i = 0; i < this.PartitionCount; i++)
				{
					if (i == 0)
					{
						stringBuilder.AppendFormat("{0}#{1}", i, cookieDict[i] ?? string.Empty);
					}
					else
					{
						stringBuilder.AppendFormat(";{0}#{1}", i, cookieDict[i] ?? string.Empty);
					}
				}
			}
			return stringBuilder.ToString();
		}

		private readonly IConfigDataProvider dataProviderDirectory;
	}
}
