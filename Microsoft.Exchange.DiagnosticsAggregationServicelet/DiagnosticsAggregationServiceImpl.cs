using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Globalization;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.DiagnosticsAggregation;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Servicelets.DiagnosticsAggregation.Messages;
using Microsoft.Exchange.Transport.DiagnosticsAggregationService;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = false)]
	internal class DiagnosticsAggregationServiceImpl : IDiagnosticsAggregationService
	{
		public DiagnosticsAggregationServiceImpl()
		{
			this.localQueuesDataProvider = DiagnosticsAggregationServicelet.GetLocalQueuesDataProvider();
			this.groupQueuesDataProvider = DiagnosticsAggregationServicelet.GetGroupQueuesDataProvider();
			this.log = DiagnosticsAggregationServicelet.Log;
		}

		public DiagnosticsAggregationServiceImpl(ILocalQueuesDataProvider localQueuesDataProvider, IGroupQueuesDataProvider groupQueuesDataProvider, DiagnosticsAggregationLog log)
		{
			this.localQueuesDataProvider = localQueuesDataProvider;
			this.groupQueuesDataProvider = groupQueuesDataProvider;
			this.log = log;
		}

		public LocalViewResponse GetLocalView(LocalViewRequest request)
		{
			LocalViewResponse response = null;
			this.ServiceRequest(delegate
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceFunction<string, string, string>(0L, "GetLocalView called. ClientMachineName={0}; ClientProcessName={1}; ClientProcessId={2}", (request.ClientInformation == null) ? string.Empty : request.ClientInformation.ClientMachineName, (request.ClientInformation == null) ? string.Empty : request.ClientInformation.ClientProcessName, (request.ClientInformation == null) ? string.Empty : request.ClientInformation.ClientProcessId.ToString());
				this.log.LogOperationFromClient(DiagnosticsAggregationEvent.LocalViewRequestReceived, request.ClientInformation, null, "");
				DiagnosticsAggregationServiceImpl.VerifyParameterIsNotNull(request, "request");
				DiagnosticsAggregationServiceImpl.VerifyParameterIsNotNullOrEmpty(request.RequestType, "request.RequestType");
				RequestType requestType;
				bool flag = Enum.TryParse<RequestType>(request.RequestType, out requestType);
				if (!flag || requestType != RequestType.Queues)
				{
					throw DiagnosticsAggregationServiceImpl.NewUnsupportedParameterFault(request.RequestType, "request.RequestType");
				}
				ServerQueuesSnapshot localServerQueues = this.localQueuesDataProvider.GetLocalServerQueues();
				if (localServerQueues.IsEmpty())
				{
					throw DiagnosticsAggregationServiceImpl.NewFault(ErrorCode.LocalQueueDataNotAvailable, localServerQueues.LastError);
				}
				string message;
				if (this.LocalQueueDataTooOld(localServerQueues.TimeStampOfQueues, out message))
				{
					throw DiagnosticsAggregationServiceImpl.NewFault(ErrorCode.LocalQueueDataTooOld, message);
				}
				response = new LocalViewResponse(localServerQueues.GetServerSnapshotStatus());
				response.QueueLocalViewResponse = new QueueLocalViewResponse(new List<LocalQueueInfo>(localServerQueues.Queues), localServerQueues.TimeStampOfQueues);
				stopwatch.Stop();
				this.log.LogOperationFromClient(DiagnosticsAggregationEvent.LocalViewResponseSent, request.ClientInformation, new TimeSpan?(stopwatch.Elapsed), "");
			}, "GetLocalView", request.ClientInformation);
			return response;
		}

		public AggregatedViewResponse GetAggregatedView(AggregatedViewRequest request)
		{
			ExTraceGlobals.DiagnosticsAggregationTracer.TraceFunction<string, string, string>(0L, "GetAggregatedView called. ClientMachineName={0}; ClientProcessName={1}; ClientProcessId={2}", (request.ClientInformation == null) ? string.Empty : request.ClientInformation.ClientMachineName, (request.ClientInformation == null) ? string.Empty : request.ClientInformation.ClientProcessName, (request.ClientInformation == null) ? string.Empty : request.ClientInformation.ClientProcessId.ToString());
			AggregatedViewResponse response = null;
			this.ServiceRequest(delegate
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				this.log.LogOperationFromClient(DiagnosticsAggregationEvent.AggregatedViewRequestReceived, request.ClientInformation, null, "");
				DiagnosticsAggregationServiceImpl.VerifyParameterIsNotNull(request, "request");
				DiagnosticsAggregationServiceImpl.VerifyParameterIsNotNullOrEmpty(request.RequestType, "request.RequestType");
				RequestType requestType;
				bool flag = Enum.TryParse<RequestType>(request.RequestType, out requestType);
				if (!flag || requestType != RequestType.Queues)
				{
					throw DiagnosticsAggregationServiceImpl.NewUnsupportedParameterFault(request.RequestType, "request.RequestType");
				}
				DiagnosticsAggregationServiceImpl.VerifyParameterIsNotNull(request.QueueAggregatedViewRequest, "request.QueueAggregatedViewRequest");
				IQueueFilter filter;
				if (!QueueFilter.TryParse(request.QueueAggregatedViewRequest.QueueFilter, out filter))
				{
					throw DiagnosticsAggregationServiceImpl.NewInvalidParameterFault(request.QueueAggregatedViewRequest.QueueFilter, "request.QueueAggregatedViewRequest.QueueFilter");
				}
				DiagnosticsAggregationServiceImpl.VerifyParameterIsNotNullOrEmpty(request.QueueAggregatedViewRequest.GroupByKey, "request.QueueAggregatedViewRequest.GroupByKey");
				QueueDigestGroupBy groupByKey;
				if (!Enum.TryParse<QueueDigestGroupBy>(request.QueueAggregatedViewRequest.GroupByKey, out groupByKey))
				{
					throw DiagnosticsAggregationServiceImpl.NewUnsupportedParameterFault(request.QueueAggregatedViewRequest.GroupByKey, "request.QueueAggregatedViewRequest.GroupByKey");
				}
				DiagnosticsAggregationServiceImpl.VerifyParameterIsNotNullOrEmpty(request.QueueAggregatedViewRequest.DetailsLevel, "request.QueueAggregatedViewRequest.DetailsLevel");
				DetailsLevel detailsLevel;
				if (!Enum.TryParse<DetailsLevel>(request.QueueAggregatedViewRequest.DetailsLevel, out detailsLevel))
				{
					throw DiagnosticsAggregationServiceImpl.NewUnsupportedParameterFault(request.QueueAggregatedViewRequest.DetailsLevel, "request.QueueAggregatedViewRequest.DetailsLevel");
				}
				QueueAggregator queueAggregator = new QueueAggregator(groupByKey, detailsLevel, filter, new TimeSpan?(this.GetTimeSpanForQueueDataBeingCurrent()));
				bool flag2 = request.ServersToInclude != null && request.ServersToInclude.Count > 0;
				HashSet<string> hashSet = flag2 ? new HashSet<string>(request.ServersToInclude, StringComparer.InvariantCultureIgnoreCase) : new HashSet<string>();
				IDictionary<ADObjectId, ServerQueuesSnapshot> currentGroupServerToQueuesMap = this.groupQueuesDataProvider.GetCurrentGroupServerToQueuesMap();
				currentGroupServerToQueuesMap.Add(this.localQueuesDataProvider.GetLocalServerId(), this.localQueuesDataProvider.GetLocalServerQueues());
				List<ServerSnapshotStatus> list = new List<ServerSnapshotStatus>();
				foreach (KeyValuePair<ADObjectId, ServerQueuesSnapshot> keyValuePair in currentGroupServerToQueuesMap)
				{
					ADObjectId key = keyValuePair.Key;
					ServerQueuesSnapshot value = keyValuePair.Value;
					if (!flag2 || hashSet.Contains(key.ToString()))
					{
						string message;
						if (value.IsEmpty())
						{
							value.SetAsFailed(DiagnosticsAggregationServiceImpl.NewFault(ErrorCode.LocalQueueDataNotAvailable, value.LastError).Detail.ToString());
						}
						else if (this.LocalQueueDataTooOld(value.TimeStampOfQueues, out message))
						{
							value.SetAsFailed(DiagnosticsAggregationServiceImpl.NewFault(ErrorCode.LocalQueueDataTooOld, message).Detail.ToString());
						}
						else
						{
							queueAggregator.AddLocalQueues(value.Queues, value.TimeStampOfQueues);
						}
						list.Add(value.GetServerSnapshotStatus());
					}
				}
				response = new AggregatedViewResponse(list);
				response.QueueAggregatedViewResponse = new QueueAggregatedViewResponse(queueAggregator.GetResultSortedByMessageCount(request.ResultSize));
				stopwatch.Stop();
				this.log.LogOperationFromClient(DiagnosticsAggregationEvent.AggregatedViewResponseSent, request.ClientInformation, new TimeSpan?(stopwatch.Elapsed), "");
			}, "GetAggregatedView", request.ClientInformation);
			return response;
		}

		protected virtual TimeSpan GetTimeSpanForQueueDataBeingCurrent()
		{
			return DiagnosticsAggregationServicelet.Config.TimeSpanForQueueDataBeingCurrent;
		}

		protected virtual bool LocalQueueDataTooOld(DateTime timeStampOfQueues, out string errorMessage)
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpanForQueueDataBeingCurrent = DiagnosticsAggregationServicelet.Config.TimeSpanForQueueDataBeingCurrent;
			TimeSpan timeSpanForQueueDataBeingStale = DiagnosticsAggregationServicelet.Config.TimeSpanForQueueDataBeingStale;
			bool result;
			if (timeStampOfQueues < utcNow - (timeSpanForQueueDataBeingCurrent + timeSpanForQueueDataBeingStale))
			{
				result = true;
				errorMessage = string.Format("timestamp of queues: {0}, current time utc: {1}, TimeSpanForQueueDataBeingCurrent: {2}, TimeSpanForQueueDataBeingStale: {3}", new object[]
				{
					timeStampOfQueues,
					utcNow,
					timeSpanForQueueDataBeingCurrent,
					timeSpanForQueueDataBeingStale
				});
			}
			else
			{
				result = false;
				errorMessage = string.Empty;
			}
			return result;
		}

		protected virtual void HandleUnHandledException(string operationName, ClientInformation clientInfo, Exception e)
		{
			ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<string, Exception>(0L, "Operation {0} encountered exception {1}", operationName, e);
			DiagnosticsAggregationServicelet.EventLog.LogEvent(MSExchangeDiagnosticsAggregationEventLogConstants.Tuple_DiagnosticsAggregationServiceUnexpectedException, null, new object[]
			{
				operationName,
				e.ToString()
			});
			DiagnosticsAggregationEvent evt = (operationName == "GetLocalView") ? DiagnosticsAggregationEvent.LocalViewRequestReceivedFailed : DiagnosticsAggregationEvent.AggregatedViewRequestReceivedFailed;
			this.log.LogOperationFromClient(evt, clientInfo, null, e.ToString());
			ExWatson.SendReportAndCrashOnAnotherThread(e);
		}

		protected virtual void CheckClientAuthorization()
		{
			ServiceSecurityContext serviceSecurityContext = ServiceSecurityContext.Current;
			bool flag = false;
			if (serviceSecurityContext == null)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError((long)this.GetHashCode(), "ServiceSecurityContext is null");
			}
			else if (serviceSecurityContext.WindowsIdentity == null)
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError((long)this.GetHashCode(), "ServiceSecurityContext WindowsIdentity is null");
			}
			else if (!this.HasReadAccessInAd(serviceSecurityContext))
			{
				ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<string>((long)this.GetHashCode(), "User {0} does not have Read access in AD", ServiceSecurityContext.Current.WindowsIdentity.Name);
			}
			else
			{
				flag = true;
			}
			if (!flag)
			{
				string empty = string.Empty;
				if (serviceSecurityContext != null && serviceSecurityContext.WindowsIdentity != null)
				{
					string name = serviceSecurityContext.WindowsIdentity.Name;
				}
				throw DiagnosticsAggregationServiceImpl.NewFault(ErrorCode.AccessDenied, "Access is Denied");
			}
		}

		protected virtual void LogFaultException(string operationName, ClientInformation clientInfo, FaultException<DiagnosticsAggregationFault> faultException)
		{
			ExTraceGlobals.DiagnosticsAggregationTracer.TraceError<string, FaultException<DiagnosticsAggregationFault>>(0L, "Operation {0} encountered exception {1}", operationName, faultException);
			DiagnosticsAggregationEvent evt = (operationName == "GetLocalView") ? DiagnosticsAggregationEvent.LocalViewRequestReceivedFailed : DiagnosticsAggregationEvent.AggregatedViewRequestReceivedFailed;
			this.log.LogOperationFromClient(evt, clientInfo, null, faultException.Detail.ToString());
		}

		private static void VerifyParameterIsNotNull(object parameterValue, string parameterName)
		{
			if (parameterValue == null)
			{
				throw DiagnosticsAggregationServiceImpl.NewInvalidParameterFault(parameterValue, parameterName);
			}
		}

		private static void VerifyParameterIsNotNullOrEmpty(string parameterValue, string parameterName)
		{
			if (string.IsNullOrEmpty(parameterValue))
			{
				throw DiagnosticsAggregationServiceImpl.NewInvalidParameterFault(parameterValue, parameterName);
			}
		}

		private static FaultException<DiagnosticsAggregationFault> NewUnsupportedParameterFault(object parameterValue, string parameterName)
		{
			string message = string.Format(CultureInfo.InvariantCulture, "parameter [{0}] has an unsupported value [{1}]", new object[]
			{
				parameterName,
				parameterValue
			});
			return DiagnosticsAggregationServiceImpl.NewFault(ErrorCode.UnsupportedParameter, message);
		}

		private static FaultException<DiagnosticsAggregationFault> NewInvalidParameterFault(object parameterValue, string parameterName)
		{
			if (parameterValue == null)
			{
				parameterValue = "<null>";
			}
			else if (object.Equals(parameterValue, string.Empty))
			{
				parameterValue = "<empty_string>";
			}
			string message = string.Format(CultureInfo.InvariantCulture, "parameter [{0}] has an invalid value [{1}]", new object[]
			{
				parameterName,
				parameterValue
			});
			return DiagnosticsAggregationServiceImpl.NewFault(ErrorCode.InvalidParameter, message);
		}

		private static FaultException<DiagnosticsAggregationFault> NewFault(ErrorCode errorCode, string message)
		{
			return new FaultException<DiagnosticsAggregationFault>(new DiagnosticsAggregationFault(errorCode, message));
		}

		private void ServiceRequest(DiagnosticsAggregationServiceImpl.ProcessRequestDelegate serviceCall, string operationName, ClientInformation clientInfo)
		{
			try
			{
				this.CheckClientAuthorization();
				serviceCall();
			}
			catch (FaultException<DiagnosticsAggregationFault> faultException)
			{
				this.LogFaultException(operationName, clientInfo, faultException);
				throw;
			}
			catch (Exception e)
			{
				this.HandleUnHandledException(operationName, clientInfo, e);
				throw;
			}
		}

		private bool HasReadAccessInAd(ServiceSecurityContext context)
		{
			SecurityIdentifier user = context.WindowsIdentity.User;
			bool result;
			using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(context.WindowsIdentity))
			{
				AccessMask accessMask = (AccessMask)131220;
				try
				{
					AccessMask grantedAccess = (AccessMask)clientSecurityContext.GetGrantedAccess(this.GetSecurityDescriptorToCheckAgainst(), user, accessMask);
					if ((grantedAccess & accessMask) == AccessMask.Open)
					{
						this.TraceAndLogError(ExTraceGlobals.DiagnosticsAggregationTracer, "Access check failed for {0}. Response={1}", new object[]
						{
							context.WindowsIdentity.Name,
							grantedAccess
						});
						result = false;
					}
					else
					{
						result = true;
					}
				}
				catch (ADTransientException ex)
				{
					this.TraceAndLogError(ExTraceGlobals.DiagnosticsAggregationTracer, "AD Transient Exception. Details {0}", new object[]
					{
						ex
					});
					result = false;
				}
				catch (AuthzException ex2)
				{
					this.TraceAndLogError(ExTraceGlobals.DiagnosticsAggregationTracer, "Authorization check failed. Details {0}", new object[]
					{
						ex2
					});
					result = false;
				}
			}
			return result;
		}

		private SecurityDescriptor GetSecurityDescriptorToCheckAgainst()
		{
			if (DiagnosticsAggregationServiceImpl.transportServerSecurity == null)
			{
				Server localServer = DiagnosticsAggregationServicelet.LocalServer;
				RawSecurityDescriptor rawSecurityDescriptor = localServer.ReadSecurityDescriptor();
				if (rawSecurityDescriptor != null)
				{
					try
					{
						ActiveDirectorySecurity activeDirectorySecurity = TransportADUtils.SetupActiveDirectorySecurity(rawSecurityDescriptor);
						DiagnosticsAggregationServiceImpl.transportServerSecurity = new SecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm());
						return DiagnosticsAggregationServiceImpl.transportServerSecurity;
					}
					catch (OverflowException ex)
					{
						this.TraceAndLogError(ExTraceGlobals.DiagnosticsAggregationTracer, "Encountered exception while setting up Authorization setttings. Details {0}", new object[]
						{
							ex
						});
					}
				}
				DiagnosticsAggregationServiceImpl.transportServerSecurity = SecurityDescriptor.FromRawSecurityDescriptor(rawSecurityDescriptor);
			}
			return DiagnosticsAggregationServiceImpl.transportServerSecurity;
		}

		private void TraceAndLogError(Microsoft.Exchange.Diagnostics.Trace tracer, string format, params object[] parameters)
		{
			tracer.TraceError((long)this.GetHashCode(), format, parameters);
			this.log.Log(DiagnosticsAggregationEvent.ServiceletError, format, parameters);
		}

		private const string GetLocalViewOperationName = "GetLocalView";

		private const string GetAggregatedViewOperationName = "GetAggregatedView";

		private const bool IncludeExceptionDetailInFaults = false;

		private static SecurityDescriptor transportServerSecurity;

		private ILocalQueuesDataProvider localQueuesDataProvider;

		private IGroupQueuesDataProvider groupQueuesDataProvider;

		private DiagnosticsAggregationLog log;

		protected delegate void ProcessRequestDelegate();
	}
}
