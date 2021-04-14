using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Hygiene.Data.DataProvider;

namespace Microsoft.Exchange.Hygiene.Data.AsyncQueue
{
	internal class AsyncQueueSession : HygieneSession
	{
		public AsyncQueueSession()
		{
			this.dataProviderDirectory = ConfigDataProviderFactory.Default.Create(DatabaseType.Directory);
			this.dataProviderMtrt = ConfigDataProviderFactory.Default.Create(DatabaseType.Mtrt);
		}

		public int PartitionCount
		{
			get
			{
				return this.GetAllPhysicalPartitions().Length;
			}
		}

		public void AddNewRequest(AsyncQueueRequest request, DateTime? scheduleTime = null)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (scheduleTime != null)
			{
				request[AsyncQueueRequestSchema.ScheduleTimeProperty] = scheduleTime.Value;
			}
			this.Save(this.dataProviderDirectory, request);
		}

		public void UpdateRequestStatus(Guid requestId, Guid organizationalUnitRoot, AsyncQueueStatus currentStatus, AsyncQueueStatus newStatus)
		{
			AsyncQueueRequestStatusUpdate asyncQueueRequestStatusUpdate = new AsyncQueueRequestStatusUpdate(organizationalUnitRoot, requestId);
			asyncQueueRequestStatusUpdate.CurrentStatus = currentStatus;
			asyncQueueRequestStatusUpdate.Status = newStatus;
			this.Save(this.dataProviderDirectory, asyncQueueRequestStatusUpdate);
		}

		public void UpdateStepStatus(string processInstanceName, Guid requestId, Guid organizationalUnitRoot, Guid stepId, AsyncQueueStatus currentStatus, AsyncQueueStatus newStatus, out AsyncQueueRequestStatusInfo requestStatusInfo, TimeSpan? retryWaitInterval = null, string cookie = null)
		{
			requestStatusInfo = null;
			AsyncQueueSession.ValidateProcessInstanceName(processInstanceName);
			AsyncQueueRequestStatusUpdate asyncQueueRequestStatusUpdate = new AsyncQueueRequestStatusUpdate(organizationalUnitRoot, requestId, stepId);
			asyncQueueRequestStatusUpdate.ProcessInstanceName = processInstanceName;
			asyncQueueRequestStatusUpdate.CurrentStatus = currentStatus;
			asyncQueueRequestStatusUpdate.Status = newStatus;
			asyncQueueRequestStatusUpdate.Cookie = cookie;
			if (retryWaitInterval != null)
			{
				asyncQueueRequestStatusUpdate.RetryInterval = (int)retryWaitInterval.Value.TotalSeconds;
			}
			asyncQueueRequestStatusUpdate[AsyncQueueRequestStatusUpdateSchema.RequestCompleteProperty] = false;
			asyncQueueRequestStatusUpdate[AsyncQueueRequestStatusUpdateSchema.RequestStatusProperty] = AsyncQueueStatus.None;
			asyncQueueRequestStatusUpdate[AsyncQueueRequestStatusUpdateSchema.RequestStartDatetimeProperty] = null;
			asyncQueueRequestStatusUpdate[AsyncQueueRequestStatusUpdateSchema.RequestEndDatetimeProperty] = null;
			this.Save(this.dataProviderDirectory, asyncQueueRequestStatusUpdate);
			requestStatusInfo = new AsyncQueueRequestStatusInfo();
			requestStatusInfo.Status = asyncQueueRequestStatusUpdate.RequestStatus;
			requestStatusInfo.StartDatetime = asyncQueueRequestStatusUpdate.RequestStartDatetime;
			requestStatusInfo.EndDatetime = asyncQueueRequestStatusUpdate.RequestEndDatetime;
		}

		public IPagedReader<AsyncQueueRequest> GetAsyncQueueRequests(Guid organizationalUnitRoot, string ownerId = null, Guid? requestId = null, AsyncQueuePriority? priority = null, AsyncQueueStatus? status = null, DateTime? minCreationTime = null, DateTime? maxCreationTime = null, int pageSize = 100)
		{
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				AsyncQueueSession.NewComparisonFilter(AsyncQueueRequestSchema.OrganizationalUnitRootProperty, organizationalUnitRoot),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueRequestSchema.OwnerIdProperty, ownerId),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.PageSizeQueryProperty, pageSize)
			});
			if (priority != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueRequestSchema.PriorityProperty, (byte)priority.Value)
				});
			}
			if (minCreationTime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.MinCreationDateTimeQueryProperty, minCreationTime)
				});
			}
			if (maxCreationTime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.MaxCreationDateTimeQueryProperty, maxCreationTime)
				});
			}
			if (status != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueRequestSchema.RequestStatusProperty, status)
				});
			}
			if (requestId != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.RequestIdProperty, requestId)
				});
			}
			return new ConfigDataProviderPagedReader<AsyncQueueRequest>(this.dataProviderDirectory, null, queryFilter, null, pageSize);
		}

		public IPagedReader<AsyncQueueRequest> GetAsyncQueueRequests(string ownerId, AsyncQueuePriority? priority = null, AsyncQueueStatus? status = null, DateTime? minCreationTime = null, DateTime? maxCreationTime = null, int pageSize = 100)
		{
			if (string.IsNullOrWhiteSpace(ownerId))
			{
				throw new ArgumentNullException("ownerId");
			}
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				AsyncQueueSession.NewComparisonFilter(AsyncQueueRequestSchema.OwnerIdProperty, ownerId),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.PageSizeQueryProperty, pageSize)
			});
			if (priority != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueRequestSchema.PriorityProperty, (byte)priority.Value)
				});
			}
			if (minCreationTime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.MinCreationDateTimeQueryProperty, minCreationTime)
				});
			}
			if (maxCreationTime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.MaxCreationDateTimeQueryProperty, maxCreationTime)
				});
			}
			if (status != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueRequestSchema.RequestStatusProperty, status)
				});
			}
			List<IPagedReader<AsyncQueueRequest>> list = new List<IPagedReader<AsyncQueueRequest>>();
			foreach (object propertyValue in ((IPartitionedDataProvider)this.dataProviderDirectory).GetAllPhysicalPartitions())
			{
				list.Add(new ConfigDataProviderPagedReader<AsyncQueueRequest>(this.dataProviderDirectory, null, QueryFilter.AndTogether(new QueryFilter[]
				{
					AsyncQueueSession.NewComparisonFilter(DalHelper.PhysicalInstanceKeyProp, propertyValue),
					queryFilter
				}), null, pageSize));
			}
			return new CompositePagedReader<AsyncQueueRequest>(list.ToArray());
		}

		public IPagedReader<AsyncQueueRequest> GetAsyncQueueRequests(Guid organizationalUnitRoot, Guid dependantOnRequestId, int pageSize = 100)
		{
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				AsyncQueueSession.NewComparisonFilter(AsyncQueueRequestSchema.OrganizationalUnitRootProperty, organizationalUnitRoot),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.DependantOnRequestIdQueryProperty, dependantOnRequestId),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.PageSizeQueryProperty, pageSize)
			});
			return new ConfigDataProviderPagedReader<AsyncQueueRequest>(this.dataProviderDirectory, null, filter, null, pageSize);
		}

		public List<AsyncQueueStep> GetActiveAsyncQueueRequests(int partitionId, string processInstanceName, TimeSpan failoverWaitThreshold, AsyncQueuePriority? priority = null, bool haRequestOnly = false, int batchSize = 10)
		{
			if (partitionId >= this.PartitionCount)
			{
				throw new ArgumentOutOfRangeException(string.Format("Invalid Parition Id {0} passed. Valid partition values are 0 to {1}", partitionId, this.PartitionCount - 1));
			}
			AsyncQueueSession.ValidateProcessInstanceName(processInstanceName);
			object propertyValue = this.GetAllPhysicalPartitions()[partitionId];
			MultiValuedProperty<AsyncQueueOwnerInfo> propertyValue2 = new MultiValuedProperty<AsyncQueueOwnerInfo>();
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				AsyncQueueSession.NewComparisonFilter(DalHelper.PhysicalInstanceKeyProp, propertyValue),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.ProcessInstanceNameProperty, processInstanceName),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.OwnerListQueryProperty, propertyValue2),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.BatchSizeQueryProperty, batchSize),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.FailoverWaitInSecondsQueryProperty, failoverWaitThreshold.TotalSeconds)
			});
			if (haRequestOnly)
			{
				AsyncQueueFlags asyncQueueFlags = AsyncQueueFlags.HARequest;
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.FlagsProperty, asyncQueueFlags)
				});
			}
			if (priority != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.PriorityProperty, (byte)priority.Value)
				});
			}
			return this.dataProviderDirectory.Find<AsyncQueueStep>(queryFilter, null, true, null).Cast<AsyncQueueStep>().ToList<AsyncQueueStep>();
		}

		internal IPagedReader<AsyncQueueStatusReport> GetStatusReport(string owner = null, DateTime? minCreationTime = null, DateTime? maxCreationTime = null, int? minFetchCount = null, int pageSize = 100)
		{
			QueryFilter queryFilter = AsyncQueueSession.NewComparisonFilter(AsyncQueueStatusReportSchema.PageSizeQueryProperty, pageSize);
			if (!string.IsNullOrWhiteSpace(owner))
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueStatusReportSchema.OwnerIdQueryProperty, owner)
				});
			}
			if (minCreationTime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueStatusReportSchema.MinCreationDateTimeQueryProperty, minCreationTime)
				});
			}
			if (maxCreationTime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueStatusReportSchema.MaxCreationDateTimeQueryProperty, maxCreationTime)
				});
			}
			if (minFetchCount != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueStatusReportSchema.MinFetchCountProperty, minFetchCount)
				});
			}
			List<IPagedReader<AsyncQueueStatusReport>> list = new List<IPagedReader<AsyncQueueStatusReport>>();
			foreach (object propertyValue in ((IPartitionedDataProvider)this.dataProviderDirectory).GetAllPhysicalPartitions())
			{
				list.Add(new ConfigDataProviderPagedReader<AsyncQueueStatusReport>(this.dataProviderDirectory, null, QueryFilter.AndTogether(new QueryFilter[]
				{
					AsyncQueueSession.NewComparisonFilter(DalHelper.PhysicalInstanceKeyProp, propertyValue),
					queryFilter
				}), null, pageSize));
			}
			return new CompositePagedReader<AsyncQueueStatusReport>(list.ToArray());
		}

		public void Save(AsyncQueueLog log)
		{
			if (log == null)
			{
				throw new ArgumentNullException("log");
			}
			AsyncQueueLogBatch asyncQueueLogBatch = new AsyncQueueLogBatch(log.OrganizationalUnitRoot);
			asyncQueueLogBatch.Add(log);
			this.Save(asyncQueueLogBatch);
		}

		public void Save(AsyncQueueLogBatch logBatch)
		{
			if (logBatch == null)
			{
				throw new ArgumentNullException("logBatch");
			}
			this.Save(this.dataProviderMtrt, logBatch);
		}

		public IPagedReader<AsyncQueueLog> GetAsyncQueueLogs(Guid organizationalUnitRoot, string ownerId = null, Guid? requestId = null, Guid? requestStepId = null, Guid? stepTransactionId = null, DateTime? processStartDatetime = null, DateTime? processEndDatetime = null, int pageSize = 100)
		{
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.OrganizationalUnitRootProperty, organizationalUnitRoot),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.OwnerIdProperty, ownerId),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.PageSizeQueryProperty, pageSize)
			});
			if (requestId != null && requestId != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.RequestIdProperty, requestId.Value)
				});
			}
			if (requestStepId != null && requestStepId != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.RequestStepIdProperty, requestStepId.Value)
				});
			}
			if (stepTransactionId != null && stepTransactionId != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.StepTransactionIdProperty, stepTransactionId.Value)
				});
			}
			if (processStartDatetime != null && processStartDatetime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, AsyncQueueLogSchema.ProcessStartDatetimeProperty, processStartDatetime.Value)
				});
			}
			if (processEndDatetime != null && processEndDatetime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.LessThanOrEqual, AsyncQueueLogSchema.ProcessEndDatetimeProperty, processEndDatetime.Value)
				});
			}
			return new ConfigDataProviderPagedReader<AsyncQueueLog>(this.dataProviderMtrt, null, queryFilter, null, pageSize);
		}

		public IPagedReader<AsyncQueueLog> GetAsyncQueueLogsByOwner(string ownerId, DateTime? processStartDatetime = null, DateTime? processEndDatetime = null, int pageSize = 100)
		{
			if (string.IsNullOrWhiteSpace(ownerId))
			{
				throw new ArgumentNullException("ownerId");
			}
			QueryFilter queryFilter = QueryFilter.AndTogether(new QueryFilter[]
			{
				AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.OwnerIdProperty, ownerId),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.PageSizeQueryProperty, pageSize)
			});
			if (processStartDatetime != null && processStartDatetime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, AsyncQueueLogSchema.ProcessStartDatetimeProperty, processStartDatetime.Value)
				});
			}
			if (processEndDatetime != null && processEndDatetime != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.LessThanOrEqual, AsyncQueueLogSchema.ProcessEndDatetimeProperty, processEndDatetime.Value)
				});
			}
			List<IPagedReader<AsyncQueueLog>> list = new List<IPagedReader<AsyncQueueLog>>();
			foreach (object propertyValue in ((IPartitionedDataProvider)this.dataProviderMtrt).GetAllPhysicalPartitions())
			{
				list.Add(new ConfigDataProviderPagedReader<AsyncQueueLog>(this.dataProviderMtrt, null, QueryFilter.AndTogether(new QueryFilter[]
				{
					AsyncQueueSession.NewComparisonFilter(DalHelper.PhysicalInstanceKeyProp, propertyValue),
					queryFilter
				}), null, pageSize));
			}
			return new CompositePagedReader<AsyncQueueLog>(list.ToArray());
		}

		public IPagedReader<AsyncQueueLogProperty> GetAsyncQueueLogProperties(Guid organizationalUnitRoot, Guid stepTransactionId, string logType = null, int pageSize = 100)
		{
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.OrganizationalUnitRootProperty, organizationalUnitRoot),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.StepTransactionIdProperty, stepTransactionId),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueLogPropertySchema.LogTypeProperty, logType),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.PageSizeQueryProperty, pageSize)
			});
			return new ConfigDataProviderPagedReader<AsyncQueueLogProperty>(this.dataProviderMtrt, null, filter, null, pageSize);
		}

		public List<AsyncQueueProbeResult> GetAsyncProbeResults(int nPartitionId, int batchSize, int inprogressBatchSize, int proocessBackInSeconds, int proocessInprogressBackInSeconds)
		{
			int num = this.GetAllPhysicalPartitions().Length;
			if (nPartitionId >= 0 && nPartitionId < num)
			{
				object propertyValue = this.GetAllPhysicalPartitions()[nPartitionId];
				QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
				{
					AsyncQueueSession.NewComparisonFilter(DalHelper.PhysicalInstanceKeyProp, propertyValue),
					AsyncQueueSession.NewComparisonFilter(AsyncQueueProbeSchema.BatchSize, batchSize),
					AsyncQueueSession.NewComparisonFilter(AsyncQueueProbeSchema.InprogressBatchSize, inprogressBatchSize),
					AsyncQueueSession.NewComparisonFilter(AsyncQueueProbeSchema.ProocessBackInSeconds, proocessBackInSeconds),
					AsyncQueueSession.NewComparisonFilter(AsyncQueueProbeSchema.ProocessInprogressBackInSeconds, proocessInprogressBackInSeconds)
				});
				return this.dataProviderDirectory.Find<AsyncQueueProbeResult>(filter, null, false, null).Cast<AsyncQueueProbeResult>().ToList<AsyncQueueProbeResult>();
			}
			throw new ArgumentOutOfRangeException(string.Format("AsynQueue Maximum partition={0}", num));
		}

		public void PauseOwnerProcessing(AsyncQueueOwnerInfo owner)
		{
			if (owner == null || string.IsNullOrWhiteSpace(owner.OwnerId))
			{
				throw new ArgumentNullException("owner");
			}
			AsyncQueueOwnerInfo asyncQueueOwnerInfo = this.GetPausedOwners().FirstOrDefault((AsyncQueueOwnerInfo o) => string.Equals(o.OwnerId, owner.OwnerId, StringComparison.CurrentCultureIgnoreCase));
			if (asyncQueueOwnerInfo != null)
			{
				throw new InvalidOperationException(string.Format("The owner '{0}' is in already paused.", owner.OwnerId));
			}
			this.dataProviderDirectory.Save(owner);
		}

		public void ResumeOwnerProcessing(AsyncQueueOwnerInfo owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			if (this.GetPausedOwners().FirstOrDefault((AsyncQueueOwnerInfo o) => string.Equals(o.OwnerId, owner.OwnerId, StringComparison.CurrentCultureIgnoreCase)) == null)
			{
				throw new InvalidOperationException(string.Format("The owner '{0}' is not paused.", owner.OwnerId));
			}
			this.dataProviderDirectory.Delete(owner);
		}

		public IEnumerable<AsyncQueueOwnerInfo> GetPausedOwners()
		{
			return this.dataProviderDirectory.Find<AsyncQueueOwnerInfo>(null, null, false, null).Cast<AsyncQueueOwnerInfo>();
		}

		public IEnumerable<T> FindMigrationReport<T>(QueryFilter filter) where T : IConfigurable, new()
		{
			object obj;
			if (!DalHelper.TryFindPropertyValueByName(filter, "organizationalUnitRoot", out obj))
			{
				throw new ArgumentException("Parameter 'Organization' was not specified in the query filter");
			}
			AsyncQueueRequest asyncQueueRequest = this.GetAsyncQueueRequests((Guid)obj, "MigrateData1415", null, null, null, null, null, 1).FirstOrDefault<AsyncQueueRequest>();
			if (asyncQueueRequest == null)
			{
				return null;
			}
			QueryFilter filter2 = QueryFilter.AndTogether(new QueryFilter[]
			{
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.OrganizationalUnitRootProperty, asyncQueueRequest.OrganizationalUnitRoot),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueCommonSchema.RequestIdProperty, asyncQueueRequest.RequestId),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.ProcessStartDatetimeProperty, asyncQueueRequest.CreationTime.AddHours(-1.0)),
				AsyncQueueSession.NewComparisonFilter(AsyncQueueLogSchema.ProcessEndDatetimeProperty, asyncQueueRequest.LastModifiedTime.AddHours(1.0))
			});
			return this.dataProviderMtrt.Find<T>(filter2, null, false, null).Cast<T>();
		}

		public object[] GetAllPhysicalPartitions()
		{
			return ((IPartitionedDataProvider)this.dataProviderDirectory).GetAllPhysicalPartitions();
		}

		private static ComparisonFilter NewComparisonFilter(PropertyDefinition property, object propertyValue)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, property, propertyValue);
		}

		private static void ValidateProcessInstanceName(string processInstanceName)
		{
			if (string.IsNullOrWhiteSpace(processInstanceName))
			{
				throw new ArgumentNullException("processInstanceName");
			}
		}

		private static string CheckInputType(IConfigurable obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("AsyncQueueSession input");
			}
			string name = obj.GetType().Name;
			if (!(obj is AsyncQueueRequest) && !(obj is AsyncQueueRequestStatusUpdate) && !(obj is AsyncQueueLog) && !(obj is AsyncQueueLogBatch))
			{
				throw new InvalidObjectTypeForSessionException(HygieneDataStrings.ErrorInvalidObjectTypeForSession("AsyncQueueSession", name));
			}
			return name;
		}

		private void Save(IConfigDataProvider dataProvider, IConfigurable obj)
		{
			AsyncQueueSession.CheckInputType(obj);
			dataProvider.Save(obj);
		}

		private readonly IConfigDataProvider dataProviderDirectory;

		private readonly IConfigDataProvider dataProviderMtrt;
	}
}
