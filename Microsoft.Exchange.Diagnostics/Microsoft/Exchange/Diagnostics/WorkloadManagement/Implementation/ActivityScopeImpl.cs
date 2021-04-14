using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation
{
	internal sealed class ActivityScopeImpl : IActivityScope, IDisposable
	{
		internal static int? MaxAppendableColumnLength { get; set; }

		private ActivityScopeImpl(Guid localId)
		{
			this.LocalId = localId;
			this.ActivityId = Guid.NewGuid();
			int? initialMetadataCapacity = ActivityContext.InitialMetadataCapacity;
			if (initialMetadataCapacity != null)
			{
				this.metadata = new ConcurrentDictionary<Enum, object>(Environment.ProcessorCount * 4, initialMetadataCapacity.Value);
			}
			else
			{
				this.metadata = new ConcurrentDictionary<Enum, object>();
			}
			this.statistics = new ConcurrentDictionary<OperationKey, OperationStatistics>();
			this.status = 0;
			this.activityType = ActivityType.Request;
			this.startTime = DateTime.UtcNow;
			this.endTime = null;
			this.stopwatch = Stopwatch.StartNew();
		}

		public Guid ActivityId
		{
			get
			{
				return this.activityId;
			}
			private set
			{
				DebugContext.SetActivityId(value);
				this.activityId = value;
			}
		}

		public Guid LocalId { get; private set; }

		public ActivityContextStatus Status
		{
			get
			{
				return (ActivityContextStatus)this.status;
			}
		}

		public ActivityType ActivityType
		{
			get
			{
				return this.activityType;
			}
			internal set
			{
				this.activityType = value;
			}
		}

		public DateTime StartTime
		{
			get
			{
				return this.startTime;
			}
		}

		public DateTime? EndTime
		{
			get
			{
				return this.endTime;
			}
		}

		public double TotalMilliseconds
		{
			get
			{
				return (double)this.stopwatch.ElapsedMilliseconds;
			}
		}

		public object UserState { get; set; }

		public string UserId
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.UserId);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.UserId, value);
			}
		}

		public string Puid
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.Puid);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.Puid, value);
			}
		}

		public string UserEmail
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.UserEmail);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.UserEmail, value);
			}
		}

		public string AuthenticationType
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.AuthenticationType);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.AuthenticationType, value);
			}
		}

		public string AuthenticationToken
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.AuthenticationToken);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.AuthenticationToken, value);
			}
		}

		public string TenantId
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.TenantId);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.TenantId, value);
			}
		}

		public string TenantType
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.TenantType);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.TenantType, value);
			}
		}

		public string Component
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.Component);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.Component, value);
			}
		}

		public string ComponentInstance
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.ComponentInstance);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.ComponentInstance, value);
			}
		}

		public string Feature
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.Feature);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.Feature, value);
			}
		}

		public string Protocol
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.Protocol);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.Protocol, value);
			}
		}

		public string ClientInfo
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.ClientInfo);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.ClientInfo, value);
			}
		}

		public string ClientRequestId
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.ClientRequestId);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.ClientRequestId, value);
			}
		}

		public string ReturnClientRequestId
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.ReturnClientRequestId);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.ReturnClientRequestId, value);
			}
		}

		public string Action
		{
			get
			{
				return (string)this.GetStandardProperty(ActivityStandardMetadata.Action);
			}
			set
			{
				this.SetStandardProperty(ActivityStandardMetadata.Action, value);
			}
		}

		public IEnumerable<KeyValuePair<Enum, object>> Metadata
		{
			get
			{
				return this.metadata;
			}
		}

		public IEnumerable<KeyValuePair<OperationKey, OperationStatistics>> Statistics
		{
			get
			{
				return this.statistics;
			}
		}

		public ActivityContextState Suspend()
		{
			Func<string, LocalizedString> func = (string debugInfo) => DiagnosticsResources.ExceptionMustStartBeforeSuspend(debugInfo);
			if (this.status == 2)
			{
				try
				{
					throw new ActivityContextException(func(DebugContext.GetDebugInfo()));
				}
				catch (ActivityContextException)
				{
				}
				return new ActivityContextState(this, this.metadata);
			}
			this.TransitionToFinalStatus(ActivityContextStatus.ActivitySuspended, ActivityContext.OnSuspendEventArgs);
			return new ActivityContextState(this, this.metadata);
		}

		public void End()
		{
			this.TransitionToFinalStatus(ActivityContextStatus.ActivityEnded, ActivityContext.OnEndEventArgs);
		}

		public bool AddOperation(ActivityOperationType activityOperationType, string instance, float value = 0f, int count = 1)
		{
			if (ExTraceGlobals.ActivityContextTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.ActivityContextTracer.TraceDebug((long)this.LocalId.GetHashCode(), "ActivityScopeImpl.AddOperation - activityOperationType = {0}, instance = {1}, value = {2}. Key = {3}", new object[]
				{
					activityOperationType,
					instance ?? "<null>",
					value,
					this.LocalId
				});
			}
			if (!this.VerifyIsStarted())
			{
				return false;
			}
			OperationStatistics operationStatistics = this.GetOperationStatistics(activityOperationType, instance);
			if (this.Status == ActivityContextStatus.ActivityStarted)
			{
				operationStatistics.AddCall(value, count);
				return true;
			}
			return false;
		}

		public void SetProperty(Enum property, string value)
		{
			this.SetPropertyObject(property, value);
		}

		public bool AppendToProperty(Enum property, string value)
		{
			if (!this.IsRegisteredProperty(property))
			{
				return false;
			}
			if (string.IsNullOrEmpty(value))
			{
				return true;
			}
			object obj;
			StringBuilder stringBuilder;
			if (this.metadata.TryGetValue(property, out obj))
			{
				stringBuilder = new StringBuilder((string)obj);
			}
			else
			{
				stringBuilder = new StringBuilder();
			}
			if (ActivityScopeImpl.MaxAppendableColumnLength != null)
			{
				if (stringBuilder.Length > ActivityScopeImpl.MaxAppendableColumnLength)
				{
					return true;
				}
				if (stringBuilder.Length + value.Length > ActivityScopeImpl.MaxAppendableColumnLength.Value)
				{
					stringBuilder.Append(value.Substring(0, ActivityScopeImpl.MaxAppendableColumnLength.Value - stringBuilder.Length));
					stringBuilder.Append("...");
				}
				else
				{
					stringBuilder.Append(value);
				}
			}
			else
			{
				stringBuilder.Append(value);
			}
			this.metadata[property] = stringBuilder.ToString();
			return true;
		}

		public string GetProperty(Enum property)
		{
			object propertyObject = this.GetPropertyObject(property);
			if (propertyObject == null)
			{
				return null;
			}
			return LogRowFormatter.Format(propertyObject);
		}

		public List<KeyValuePair<string, object>> GetFormattableMetadata()
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			foreach (KeyValuePair<Enum, object> keyValuePair in this.Metadata)
			{
				string key = ActivityContext.LookupEnumName(keyValuePair.Key);
				list.Add(new KeyValuePair<string, object>(key, keyValuePair.Value));
			}
			string text = ActivityContext.LookupEnumName(WlmMetaData.TimeOnServer);
			if (this.Status != ActivityContextStatus.ActivityStarted && text != null)
			{
				list.Add(new KeyValuePair<string, object>(text, this.TotalMilliseconds));
			}
			return list;
		}

		public IEnumerable<KeyValuePair<string, object>> GetFormattableMetadata(IEnumerable<Enum> properties)
		{
			foreach (Enum property in properties)
			{
				if (this.metadata.ContainsKey(property))
				{
					string key = ActivityContext.LookupEnumName(property);
					yield return new KeyValuePair<string, object>(key, this.metadata[property]);
				}
			}
			yield break;
		}

		public List<KeyValuePair<string, object>> GetFormattableStatistics()
		{
			return ActivityScopeImpl.GetFormattableStatistics(this.statistics);
		}

		public void UpdateFromMessage(OperationContext wcfOperationContext)
		{
			ActivityContextState activityContextState = ActivityContextState.DeserializeFrom(wcfOperationContext);
			if (activityContextState != null)
			{
				this.UpdateFromState(activityContextState);
			}
		}

		public void UpdateFromMessage(HttpRequestMessageProperty wcfMessage)
		{
			ActivityContextState activityContextState = ActivityContextState.DeserializeFrom(wcfMessage);
			if (activityContextState != null)
			{
				this.UpdateFromState(activityContextState);
			}
		}

		public void UpdateFromMessage(HttpRequest httpRequest)
		{
			this.UpdateFromMessage(new HttpRequestWrapper(httpRequest));
		}

		public void UpdateFromMessage(HttpRequestBase httpRequestBase)
		{
			ActivityContextState activityContextState = ActivityContextState.DeserializeFrom(httpRequestBase);
			if (activityContextState != null)
			{
				this.UpdateFromState(activityContextState);
			}
		}

		public void SerializeTo(HttpRequestMessageProperty wcfMessage)
		{
			if (wcfMessage == null)
			{
				throw new ArgumentNullException("wcfMessage");
			}
			string serializedScope = this.GetSerializedScope();
			if (wcfMessage.Headers["X-MSExchangeActivityCtx"] == null)
			{
				wcfMessage.Headers.Add("X-MSExchangeActivityCtx", serializedScope);
				return;
			}
			wcfMessage.Headers["X-MSExchangeActivityCtx"] = serializedScope;
		}

		public void SerializeTo(OperationContext wcfOperationContext)
		{
			if (wcfOperationContext == null)
			{
				throw new ArgumentNullException("wcfOperationContext");
			}
			string serializedScope = this.GetSerializedScope();
			MessageHeader header = MessageHeader.CreateHeader("X-MSExchangeActivityCtx", string.Empty, serializedScope);
			wcfOperationContext.OutgoingMessageHeaders.Add(header);
		}

		public void SerializeTo(HttpWebRequest httpWebRequest)
		{
			if (httpWebRequest == null)
			{
				throw new ArgumentNullException("httpWebRequest");
			}
			string serializedScope = this.GetSerializedScope();
			if (httpWebRequest.Headers["X-MSExchangeActivityCtx"] == null)
			{
				httpWebRequest.Headers.Add("X-MSExchangeActivityCtx", serializedScope);
				return;
			}
			httpWebRequest.Headers["X-MSExchangeActivityCtx"] = serializedScope;
		}

		public void SerializeTo(HttpResponse httpResponse)
		{
			if (httpResponse == null)
			{
				throw new ArgumentNullException("httpResponse");
			}
			httpResponse.Headers["request-id"] = this.activityId.ToString();
			if (!string.IsNullOrWhiteSpace(this.ClientRequestId) && string.Equals("true", this.ReturnClientRequestId, StringComparison.OrdinalIgnoreCase))
			{
				httpResponse.Headers["client-request-id"] = this.ClientRequestId;
			}
		}

		public void SerializeMinimalTo(HttpWebRequest httpWebRequest)
		{
			if (httpWebRequest == null)
			{
				throw new ArgumentNullException("httpWebRequest");
			}
			string minimalSerializedScope = this.GetMinimalSerializedScope();
			if (httpWebRequest.Headers["X-MSExchangeActivityCtx"] == null)
			{
				httpWebRequest.Headers.Add("X-MSExchangeActivityCtx", minimalSerializedScope);
				return;
			}
			httpWebRequest.Headers["X-MSExchangeActivityCtx"] = minimalSerializedScope;
		}

		public void SerializeMinimalTo(HttpRequestBase httpRequest)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			string minimalSerializedScope = this.GetMinimalSerializedScope();
			if (httpRequest.Headers["X-MSExchangeActivityCtx"] == null)
			{
				httpRequest.Headers.Add("X-MSExchangeActivityCtx", minimalSerializedScope);
				return;
			}
			httpRequest.Headers["X-MSExchangeActivityCtx"] = minimalSerializedScope;
		}

		public AggregatedOperationStatistics TakeStatisticsSnapshot(AggregatedOperationType type)
		{
			switch (type)
			{
			case AggregatedOperationType.ADCalls:
				return this.TakeADStatisticsSnapshot();
			case AggregatedOperationType.StoreRPCs:
				return this.TakeStoreRpcStatisticsSnapshot();
			case AggregatedOperationType.ADObjToExchObjLatency:
				return this.TakeADObjToExchObjStatisticsSnapshot();
			default:
				throw new NotSupportedException("Unknown type: " + type);
			}
		}

		public void Dispose()
		{
			ActivityScopeImpl activityScopeImpl;
			try
			{
				ActivityScopeImpl.scopeCacheLock.EnterWriteLock();
				if (ActivityScopeImpl.scopeCache.TryGetValue(this.LocalId, out activityScopeImpl))
				{
					ActivityScopeImpl.scopeCache.Remove(this.LocalId);
				}
			}
			finally
			{
				try
				{
					ActivityScopeImpl.scopeCacheLock.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			ExTraceGlobals.ActivityContextTracer.TraceDebug<bool, Guid>((long)this.LocalId.GetHashCode(), "ActivityScopeImpl.Remove - TryRemove removed an object - {0}. Key = {1}", activityScopeImpl != null, this.LocalId);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Id");
			stringBuilder.Append(ActivityContextState.SerializedKeyValueDelimiter[0]);
			stringBuilder.Append(this.ActivityId.ToString("D"));
			foreach (KeyValuePair<Enum, object> keyValuePair in this.Metadata)
			{
				if (keyValuePair.Value != null)
				{
					stringBuilder.Append(ActivityContextState.SerializedElementDelimiter[0]);
					stringBuilder.Append(ActivityContext.LookupEnumName(keyValuePair.Key));
					stringBuilder.Append(ActivityContextState.SerializedKeyValueDelimiter[0]);
					stringBuilder.Append(LogRowFormatter.Format(keyValuePair.Value));
				}
			}
			return stringBuilder.ToString();
		}

		internal static ActivityScope AddActivityScope(ActivityContextState activityContextState)
		{
			Guid guid = Guid.NewGuid();
			ActivityScope result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				try
				{
					ActivityScopeImpl.scopeCacheLock.EnterWriteLock();
					ActivityScopeImpl activityScopeImpl = new ActivityScopeImpl(guid);
					disposeGuard.Add<ActivityScopeImpl>(activityScopeImpl);
					ActivityScopeImpl.scopeCache.Add(guid, activityScopeImpl);
					ActivityScope activityScope = new ActivityScope(activityScopeImpl);
					disposeGuard.Add<ActivityScope>(activityScope);
					activityScopeImpl.UpdateFromState(activityContextState);
					SingleContext.Singleton.LocalId = new Guid?(guid);
					SingleContext.Singleton.SetId();
					disposeGuard.Success();
					result = activityScope;
				}
				finally
				{
					try
					{
						ActivityScopeImpl.scopeCacheLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			return result;
		}

		internal static IActivityScope GetActivityScope(Guid key)
		{
			IActivityScope activityScope = null;
			ActivityScopeImpl activityScopeImpl = null;
			try
			{
				ActivityScopeImpl.scopeCacheLock.EnterReadLock();
				if (ActivityScopeImpl.scopeCache.TryGetValue(key, out activityScopeImpl))
				{
					activityScope = activityScopeImpl;
				}
			}
			finally
			{
				try
				{
					ActivityScopeImpl.scopeCacheLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			if (activityScope != null)
			{
				return activityScope;
			}
			return null;
		}

		internal static ActivityScopeImpl GetScopeImpl(Guid key)
		{
			ActivityScopeImpl activityScopeImpl;
			ActivityScopeImpl.scopeCache.TryGetValue(key, out activityScopeImpl);
			ExTraceGlobals.ActivityContextTracer.TraceDebug<bool>((long)key.GetHashCode(), "ActivityScopeImpl.GetScopeImpl - TryGetValue found ActivityScopeImpl object - {0}", activityScopeImpl != null);
			return activityScopeImpl;
		}

		internal static IEnumerable<KeyValuePair<Guid, ActivityScopeImpl>> GetScopeCacheForUnitTesting()
		{
			return ActivityScopeImpl.scopeCache;
		}

		internal static List<KeyValuePair<string, object>> GetFormattableStatistics(IEnumerable<KeyValuePair<OperationKey, OperationStatistics>> statistics)
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			foreach (KeyValuePair<OperationKey, OperationStatistics> keyValuePair in statistics)
			{
				keyValuePair.Value.AppendStatistics(keyValuePair.Key, list);
			}
			return list;
		}

		internal void RemoveInstrInstances()
		{
			foreach (KeyValuePair<OperationKey, OperationStatistics> keyValuePair in this.statistics)
			{
				if (object.ReferenceEquals(keyValuePair.Key.Instance, "INSTR"))
				{
					OperationStatistics operationStatistics;
					this.statistics.TryRemove(keyValuePair.Key, out operationStatistics);
				}
			}
		}

		internal void SetPropertyObject(Enum property, object value)
		{
			Enum key;
			if (!ActivityContext.TryGetPreboxedEnum(property, out key))
			{
				return;
			}
			if (value != null)
			{
				this.metadata[key] = value;
				return;
			}
			this.metadata.TryRemove(property, out value);
		}

		internal object GetPropertyObject(Enum property)
		{
			object result = null;
			if (property.GetType() == typeof(ActivityReadonlyMetadata))
			{
				switch ((ActivityReadonlyMetadata)property)
				{
				case ActivityReadonlyMetadata.ActivityId:
					result = this.ActivityId;
					break;
				case ActivityReadonlyMetadata.StartTime:
					result = this.StartTime;
					break;
				case ActivityReadonlyMetadata.EndTime:
					result = this.EndTime;
					break;
				case ActivityReadonlyMetadata.TotalMilliseconds:
					result = this.TotalMilliseconds;
					break;
				}
			}
			else
			{
				if (!this.IsRegisteredProperty(property))
				{
					return null;
				}
				this.metadata.TryGetValue(property, out result);
			}
			return result;
		}

		internal string GetSerializedScope()
		{
			return string.Concat(new object[]
			{
				"V",
				ActivityContextState.SerializedKeyValueDelimiter[0],
				ActivityContextState.SerializationVersion,
				ActivityContextState.SerializedElementDelimiter[0],
				"Id",
				ActivityContextState.SerializedKeyValueDelimiter[0],
				this.ActivityId.ToString("D"),
				ActivityContextState.SerializedElementDelimiter[0],
				"C",
				ActivityContextState.SerializedKeyValueDelimiter[0],
				HttpUtility.UrlEncode(this.Component),
				ActivityContextState.SerializedElementDelimiter[0],
				"P",
				ActivityContextState.SerializedKeyValueDelimiter[0],
				this.GetSerializedPayload()
			});
		}

		internal string GetMinimalSerializedScope()
		{
			return ActivityContextState.GetMinimalSerializedScope(this.ActivityId);
		}

		internal void UpdateFromState(ActivityContextState state)
		{
			if (state != null)
			{
				if (state.ActivityId != null)
				{
					this.ActivityId = state.ActivityId.Value;
					this.ActivityType = state.ActivityType;
				}
				if (state.Properties != null)
				{
					foreach (KeyValuePair<Enum, object> keyValuePair in state.Properties)
					{
						this.SetPropertyObject(keyValuePair.Key, keyValuePair.Value);
					}
				}
				DebugContext.UpdateFrom(this);
			}
		}

		private bool IsRegisteredProperty(Enum key)
		{
			return null != ActivityContext.LookupEnumName(key);
		}

		private bool VerifyIsStarted()
		{
			return this.Status == ActivityContextStatus.ActivityStarted;
		}

		private void SetStandardProperty(ActivityStandardMetadata property, object value)
		{
			this.SetPropertyObject(property, value);
		}

		private object GetStandardProperty(ActivityStandardMetadata property)
		{
			return this.GetPropertyObject(property);
		}

		private OperationStatistics GetOperationStatistics(ActivityOperationType activityOperationType, string instance)
		{
			OperationKey key = new OperationKey(activityOperationType, instance);
			Func<OperationKey, OperationStatistics> valueFactory = (OperationKey operationKey) => OperationStatistics.GetInstance(operationKey);
			return this.statistics.GetOrAdd(key, valueFactory);
		}

		private void TransitionToFinalStatus(ActivityContextStatus status, ActivityEventArgs eventArgs)
		{
			bool flag = false;
			try
			{
				int num;
				try
				{
					this.spinLock.Enter(ref flag);
					num = this.status;
					this.status = (int)status;
				}
				finally
				{
					if (flag)
					{
						this.spinLock.Exit();
					}
				}
				if (num == 0)
				{
					this.stopwatch.Stop();
					this.endTime = new DateTime?(DateTime.UtcNow);
					ExTraceGlobals.ActivityContextTracer.TraceDebug<Guid, ActivityContextStatus>((long)this.LocalId.GetHashCode(), "ActivityScopeImpl.TransitionToFinalStatus key {0}, Status={1}.", this.LocalId, status);
					ActivityContext.RaiseEvent(this, eventArgs);
				}
			}
			finally
			{
				this.Dispose();
				ActivityContext.ClearThreadScope();
			}
		}

		private string GetSerializedPayload()
		{
			byte[] inArray = null;
			string text = string.Empty;
			MemoryStream memoryStream = null;
			BinaryWriter binaryWriter = null;
			int num = 0;
			string result;
			try
			{
				foreach (Enum @enum in ActivityContextState.PayloadAllowedMetadata)
				{
					string value = ActivityContext.LookupEnumName(@enum);
					string property = this.GetProperty(@enum);
					if (!string.IsNullOrEmpty(property))
					{
						if (memoryStream == null)
						{
							memoryStream = new MemoryStream();
							binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8);
						}
						binaryWriter.Write(value);
						binaryWriter.Write(property);
						if (memoryStream.Position >= 1024L)
						{
							ExTraceGlobals.ActivityContextTracer.TraceDebug(0L, "Reached MaximumPayloadBinaryLength limit, some metadata will not be serialized into payload.");
							break;
						}
						num = (int)memoryStream.Position;
					}
				}
				if (memoryStream != null)
				{
					binaryWriter.Flush();
					memoryStream.Flush();
					inArray = memoryStream.GetBuffer();
				}
				if (num > 0)
				{
					text = Convert.ToBase64String(inArray, 0, num, Base64FormattingOptions.None);
				}
				result = text;
			}
			finally
			{
				if (binaryWriter != null)
				{
					binaryWriter.Dispose();
				}
				if (memoryStream != null)
				{
					memoryStream.Dispose();
				}
			}
			return result;
		}

		private AggregatedOperationStatistics TakeADStatisticsSnapshot()
		{
			AggregatedOperationStatistics result = new AggregatedOperationStatistics
			{
				Type = AggregatedOperationType.ADCalls
			};
			foreach (KeyValuePair<OperationKey, OperationStatistics> keyValuePair in this.statistics)
			{
				switch (keyValuePair.Key.ActivityOperationType)
				{
				case ActivityOperationType.ADRead:
				case ActivityOperationType.ADSearch:
				case ActivityOperationType.ADWrite:
				{
					AverageOperationStatistics averageOperationStatistics = (AverageOperationStatistics)keyValuePair.Value;
					result.Count += (long)averageOperationStatistics.Count;
					result.TotalMilliseconds += (double)((float)averageOperationStatistics.Count * averageOperationStatistics.CumulativeAverage);
					break;
				}
				}
			}
			return result;
		}

		private AggregatedOperationStatistics TakeStoreRpcStatisticsSnapshot()
		{
			AggregatedOperationStatistics result = new AggregatedOperationStatistics
			{
				Type = AggregatedOperationType.StoreRPCs
			};
			foreach (KeyValuePair<OperationKey, OperationStatistics> keyValuePair in this.statistics)
			{
				switch (keyValuePair.Key.ActivityOperationType)
				{
				case ActivityOperationType.RpcCount:
				{
					CountOperationStatistics countOperationStatistics = (CountOperationStatistics)keyValuePair.Value;
					result.Count += (long)countOperationStatistics.Count;
					break;
				}
				case ActivityOperationType.RpcLatency:
				{
					TotalOperationStatistics totalOperationStatistics = (TotalOperationStatistics)keyValuePair.Value;
					result.TotalMilliseconds += totalOperationStatistics.Total;
					break;
				}
				}
			}
			return result;
		}

		private AggregatedOperationStatistics TakeADObjToExchObjStatisticsSnapshot()
		{
			AggregatedOperationStatistics result = new AggregatedOperationStatistics
			{
				Type = AggregatedOperationType.ADObjToExchObjLatency
			};
			foreach (KeyValuePair<OperationKey, OperationStatistics> keyValuePair in this.statistics)
			{
				ActivityOperationType activityOperationType = keyValuePair.Key.ActivityOperationType;
				if (activityOperationType == ActivityOperationType.ADObjToExchObjLatency)
				{
					AverageOperationStatistics averageOperationStatistics = (AverageOperationStatistics)keyValuePair.Value;
					result.Count += (long)averageOperationStatistics.Count;
					result.TotalMilliseconds += (double)((float)averageOperationStatistics.Count * averageOperationStatistics.CumulativeAverage);
				}
			}
			return result;
		}

		private static readonly Dictionary<Guid, ActivityScopeImpl> scopeCache = new Dictionary<Guid, ActivityScopeImpl>();

		private static readonly ReaderWriterLockSlim scopeCacheLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		private readonly DateTime startTime;

		private readonly ConcurrentDictionary<OperationKey, OperationStatistics> statistics;

		private readonly ConcurrentDictionary<Enum, object> metadata;

		private DateTime? endTime;

		private readonly Stopwatch stopwatch;

		private Guid activityId;

		private int status;

		private ActivityType activityType;

		private SpinLock spinLock = new SpinLock(DebugContext.TestOnlyIsDebugBuild);
	}
}
