using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation;

namespace Microsoft.Exchange.Diagnostics
{
	internal abstract class RequestDetailsLoggerBase<T> : DisposeTrackableBase where T : RequestDetailsLoggerBase<T>, new()
	{
		public static T Current
		{
			get
			{
				return RequestDetailsLoggerBase<T>.GetCurrent(HttpContext.Current);
			}
			private set
			{
				RequestDetailsLoggerBase<T>.SetCurrent(HttpContext.Current, value);
			}
		}

		public static T GetCurrent(HttpContext httpContext)
		{
			if (httpContext != null && httpContext.Items != null)
			{
				return (T)((object)httpContext.Items[RequestDetailsLoggerBase<T>.ContextItemKey]);
			}
			if (RequestDetailsLoggerBase<T>.AdditionalLoggerGetterForGetCurrent != null)
			{
				return RequestDetailsLoggerBase<T>.AdditionalLoggerGetterForGetCurrent();
			}
			return default(T);
		}

		public static void SetCurrent(HttpContext httpContext, T logger)
		{
			if (httpContext != null && httpContext.Items != null)
			{
				httpContext.Items[RequestDetailsLoggerBase<T>.ContextItemKey] = logger;
				return;
			}
			if (RequestDetailsLoggerBase<T>.AdditionalLoggerSetterForSetCurrent != null)
			{
				RequestDetailsLoggerBase<T>.AdditionalLoggerSetterForSetCurrent(logger);
			}
		}

		public static bool IsInitialized { get; private set; }

		public bool SkipLogging { get; set; }

		public bool EndActivityContext { get; set; }

		public Guid ActivityId
		{
			get
			{
				if (this.ActivityScope != null)
				{
					return this.ActivityScope.ActivityId;
				}
				return Guid.Empty;
			}
		}

		public IActivityScope ActivityScope { get; private set; }

		public object SyncRoot
		{
			get
			{
				return this.syncRoot;
			}
		}

		protected static int? ErrorMessageLengthThreshold { get; set; }

		protected static bool ProcessExceptionMessage { get; set; }

		protected static Func<T> AdditionalLoggerGetterForGetCurrent { get; set; }

		protected static Action<T> AdditionalLoggerSetterForSetCurrent { get; set; }

		internal static bool IsDebugBuild
		{
			get
			{
				return false;
			}
		}

		internal static RequestLoggerConfig RequestLoggerConfig { get; private set; }

		protected internal static Log Log { get; private set; }

		private protected static LogSchema LogSchema { protected get; private set; }

		private protected LogRowFormatter Row { protected get; private set; }

		protected Dictionary<Enum, long> Latencies
		{
			get
			{
				if (this.latencies == null)
				{
					lock (this.latenciesLock)
					{
						if (this.latencies == null)
						{
							this.latencies = new Dictionary<Enum, long>(RequestDetailsLoggerBase<T>.RequestLoggerConfig.DefaultLatencyDictionarySize);
						}
					}
				}
				return this.latencies;
			}
		}

		public static RequestLoggerConfig GetConfig()
		{
			RequestLoggerConfig requestLoggerConfig;
			using (T t = Activator.CreateInstance<T>())
			{
				requestLoggerConfig = t.GetRequestLoggerConfig();
			}
			return requestLoggerConfig;
		}

		public static T InitializeRequestLogger()
		{
			return RequestDetailsLoggerBase<T>.InitializeRequestLogger(null);
		}

		public static T InitializeRequestLogger(IActivityScope activityScope)
		{
			T t = RequestDetailsLoggerBase<T>.Current;
			if (t == null)
			{
				lock (RequestDetailsLoggerBase<T>.staticSyncRoot)
				{
					t = RequestDetailsLoggerBase<T>.Current;
					if (t == null)
					{
						t = Activator.CreateInstance<T>();
						t.EndActivityContext = true;
						t.httpContext = HttpContext.Current;
						RequestDetailsLoggerBase<T>.Current = t;
					}
				}
			}
			t.InitializeRequest(activityScope);
			return t;
		}

		public static string[] GetColumnNames()
		{
			RequestLoggerConfig config = RequestDetailsLoggerBase<T>.GetConfig();
			string[] array = new string[config.Columns.Count];
			int num = 0;
			foreach (KeyValuePair<string, Enum> keyValuePair in config.Columns)
			{
				array[num++] = keyValuePair.Key;
			}
			return array;
		}

		public static void SafeSetLogger(RequestDetailsLoggerBase<T> requestDetailsLogger, Enum key, object value)
		{
			RequestDetailsLoggerBase<T>.SafeLogOperation<Enum, object>(requestDetailsLogger, key, value, delegate(RequestDetailsLoggerBase<T> logger, Enum k, object v)
			{
				logger.Set(k, v);
			});
		}

		public static void SafeAppendDetailedExchangePrincipalLatency(RequestDetailsLoggerBase<T> requestDetailsLogger, string key, string value)
		{
			RequestDetailsLoggerBase<T>.SafeAppendColumn(requestDetailsLogger, ServiceLatencyMetadata.DetailedExchangePrincipalLatency, key, value);
		}

		public static void SafeAppendColumn(RequestDetailsLoggerBase<T> requestDetailsLogger, Enum columnName, string key, string value)
		{
			RequestDetailsLoggerBase<T>.SafeLogOperation<string, string>(requestDetailsLogger, key, value, delegate(RequestDetailsLoggerBase<T> logger, string k, string v)
			{
				logger.SafeAppend(columnName, key, value);
			});
		}

		protected static void SafeAppendBackEndGenericInfo(RequestDetailsLoggerBase<T> requestDetailsLogger, string key, string value)
		{
			RequestDetailsLoggerBase<T>.SafeAppendColumn(requestDetailsLogger, ServiceCommonMetadata.BackEndGenericInfo, key, value);
		}

		public static void SafeAppendAuthenticationError(RequestDetailsLoggerBase<T> requestDetailsLogger, string key, string value)
		{
			RequestDetailsLoggerBase<T>.SafeAppendColumn(requestDetailsLogger, ServiceCommonMetadata.AuthenticationErrors, key, value);
		}

		public static void SafeAppendGenericInfo(RequestDetailsLoggerBase<T> requestDetailsLogger, string key, object value)
		{
			if (value != null && RequestDetailsLoggerBase<T>.RequestLoggerConfig != null)
			{
				RequestDetailsLoggerBase<T>.SafeAppendColumn(requestDetailsLogger, RequestDetailsLoggerBase<T>.RequestLoggerConfig.GenericInfoColumn, key, value.ToString());
			}
		}

		public static void SafeAppendGenericError(RequestDetailsLoggerBase<T> requestDetailsLogger, string key, string value)
		{
			RequestDetailsLoggerBase<T>.SafeAppendColumn(requestDetailsLogger, ServiceCommonMetadata.GenericErrors, key, value);
		}

		private static void SafeLogOperation<TKey, TValue>(RequestDetailsLoggerBase<T> requestDetailsLogger, TKey key, TValue value, RequestDetailsLoggerBase<T>.LogOperation<TKey, TValue> logOperation)
		{
			if (requestDetailsLogger == null || requestDetailsLogger.IsDisposed || key == null || value == null)
			{
				return;
			}
			try
			{
				if (Monitor.TryEnter(requestDetailsLogger.SyncRoot))
				{
					if (!requestDetailsLogger.IsDisposed)
					{
						logOperation(requestDetailsLogger, key, value);
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(requestDetailsLogger.SyncRoot))
				{
					Monitor.Exit(requestDetailsLogger.SyncRoot);
				}
			}
		}

		public static void SafeLogRequestException(RequestDetailsLoggerBase<T> requestDetailsLogger, Exception ex, string keyPrefix)
		{
			RequestDetailsLoggerBase<T>.SafeLogOperation<Exception, string>(requestDetailsLogger, ex, keyPrefix, delegate(RequestDetailsLoggerBase<T> logger, Exception k, string v)
			{
				logger.LogExceptionToGenericError(k, v);
			});
		}

		internal static string GetCorrectServerNameFromExceptionIfNecessary(string exceptionName, Exception ex)
		{
			if (exceptionName.Equals("Microsoft.Exchange.Data.Storage.WrongServerException", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					MethodInfo method = ex.GetType().GetMethod("RightServerToString");
					if (method != null)
					{
						return method.Invoke(ex, null) as string;
					}
					ExWatson.SendReport(new MissingMethodException("Microsoft.Exchange.Data.Storage.WrongServerException", "RightServerToString"), ReportOptions.DoNotFreezeThreads, null);
				}
				catch (Exception exception)
				{
					ExWatson.SendReport(exception, ReportOptions.DoNotFreezeThreads, null);
				}
			}
			return null;
		}

		private static void SafeInitializeLogger(RequestDetailsLoggerBase<T> requestDetailsLoggerBase)
		{
			if (!RequestDetailsLoggerBase<T>.IsInitialized)
			{
				lock (RequestDetailsLoggerBase<T>.staticSyncRoot)
				{
					if (!RequestDetailsLoggerBase<T>.IsInitialized)
					{
						requestDetailsLoggerBase.InitializeLogger();
						RequestDetailsLoggerBase<T>.IsInitialized = true;
					}
				}
			}
		}

		private static void GlobalActivityLogger(object sender, ActivityEventArgs args)
		{
			IActivityScope activityScope = sender as IActivityScope;
			if (activityScope.ActivityType == ActivityType.Global && (args.ActivityEventType == ActivityEventType.EndActivity || args.ActivityEventType == ActivityEventType.SuspendActivity))
			{
				RequestDetailsLoggerBase<T> requestDetailsLoggerBase = RequestDetailsLoggerBase<T>.InitializeRequestLogger(activityScope);
				ServiceCommonMetadataPublisher.PublishServerInfo(requestDetailsLoggerBase.ActivityScope);
				requestDetailsLoggerBase.Commit();
			}
		}

		private static string FormatForCsv(string value)
		{
			if (value.Contains(","))
			{
				value = value.Replace(',', ' ');
			}
			if (value.Contains("\r\n"))
			{
				value = value.Replace("\r\n", " ");
			}
			return value;
		}

		public virtual void Commit()
		{
			this.Dispose();
		}

		public virtual string Set(Enum property, object value)
		{
			string text = null;
			try
			{
				if (this.VerifyIsDisposed())
				{
					return null;
				}
				if (value != null)
				{
					text = LogRowFormatter.Format(value);
				}
				this.ActivityScope.SetProperty(property, text);
			}
			catch (ActivityContextException exception)
			{
				ExWatson.HandleException(new UnhandledExceptionEventArgs(exception, false), ReportOptions.None);
			}
			return text;
		}

		public virtual string Get(Enum property)
		{
			string result = null;
			try
			{
				if (base.IsDisposed)
				{
					return null;
				}
				result = this.ActivityScope.GetProperty(property);
			}
			catch (ActivityContextException exception)
			{
				ExWatson.HandleException(new UnhandledExceptionEventArgs(exception, false), ReportOptions.None);
			}
			return result;
		}

		public void AppendGenericInfo(string key, object value)
		{
			RequestDetailsLoggerBase<T>.SafeAppendGenericInfo(this, key, value);
		}

		public void ExcludeLogEntry()
		{
			this.excludeLogEntry = true;
		}

		public void TrackLatency(Enum latencyMetadata, Action method)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				method();
			}
			finally
			{
				stopwatch.Stop();
				this.UpdateLatency(latencyMetadata, (double)stopwatch.ElapsedMilliseconds);
			}
		}

		public TResult TrackLatency<TResult>(Enum latencyMetadata, Func<TResult> method)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			TResult result;
			try
			{
				result = method();
			}
			finally
			{
				stopwatch.Stop();
				this.UpdateLatency(latencyMetadata, (double)stopwatch.ElapsedMilliseconds);
			}
			return result;
		}

		public TResult TrackLatency<TResult>(Enum latencyMetadata, Func<TResult> method, out double latencyValue)
		{
			TResult result;
			try
			{
				result = this.TrackLatency<TResult>(latencyMetadata, method);
			}
			finally
			{
				long num = 0L;
				this.TryGetLatency(latencyMetadata, out num);
				latencyValue = (double)num;
			}
			return result;
		}

		public bool TryGetLatency(Enum latencyMetadata, out long valueInMilliseconds)
		{
			valueInMilliseconds = 0L;
			if (base.IsDisposed || latencyMetadata == null)
			{
				return false;
			}
			try
			{
				if (Monitor.TryEnter(this.SyncRoot))
				{
					if (base.IsDisposed)
					{
						return false;
					}
					lock (this.latenciesLock)
					{
						if (!this.Latencies.ContainsKey(latencyMetadata))
						{
							return false;
						}
						valueInMilliseconds = this.Latencies[latencyMetadata];
						return true;
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.SyncRoot))
				{
					Monitor.Exit(this.SyncRoot);
				}
			}
			return false;
		}

		public void UpdateLatency(Enum latencyMetadata, double latencyInMilliseconds)
		{
			if (base.IsDisposed || latencyMetadata == null)
			{
				return;
			}
			try
			{
				if (Monitor.TryEnter(this.SyncRoot))
				{
					if (!base.IsDisposed)
					{
						lock (this.latenciesLock)
						{
							if (!this.Latencies.ContainsKey(latencyMetadata))
							{
								this.Latencies.Add(latencyMetadata, 0L);
							}
							Dictionary<Enum, long> dictionary;
							(dictionary = this.Latencies)[latencyMetadata] = dictionary[latencyMetadata] + (long)latencyInMilliseconds;
						}
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.SyncRoot))
				{
					Monitor.Exit(this.SyncRoot);
				}
			}
		}

		protected virtual void InitializeLogger()
		{
			RequestDetailsLoggerBase<T>.RequestLoggerConfig = this.GetRequestLoggerConfig();
			string[] array = new string[RequestDetailsLoggerBase<T>.RequestLoggerConfig.Columns.Count];
			ReadOnlyCollection<KeyValuePair<string, Enum>> columns = RequestDetailsLoggerBase<T>.RequestLoggerConfig.Columns;
			int num = 0;
			foreach (KeyValuePair<string, Enum> keyValuePair in columns)
			{
				array[num] = keyValuePair.Key;
				RequestDetailsLoggerBase<T>.enumToIndexMap.Add(keyValuePair.Value, num);
				num++;
			}
			RequestDetailsLoggerBase<T>.LogSchema = new LogSchema("Microsoft Exchange Server", "15.00.1497.015", RequestDetailsLoggerBase<T>.RequestLoggerConfig.LogType, array);
			RequestDetailsLoggerBase<T>.Log = new Log(RequestDetailsLoggerBase<T>.RequestLoggerConfig.LogFilePrefix, new LogHeaderFormatter(RequestDetailsLoggerBase<T>.LogSchema, true), RequestDetailsLoggerBase<T>.RequestLoggerConfig.LogComponent);
			string text = ConfigurationManager.AppSettings[RequestDetailsLoggerBase<T>.RequestLoggerConfig.FolderPathAppSettingsKey];
			if (string.IsNullOrEmpty(text))
			{
				text = RequestDetailsLoggerBase<T>.RequestLoggerConfig.FallbackLogFolderPath;
			}
			RequestDetailsLoggerBase<T>.Log.Configure(text, RequestDetailsLoggerBase<T>.RequestLoggerConfig.MaxAge, RequestDetailsLoggerBase<T>.RequestLoggerConfig.MaxDirectorySize, RequestDetailsLoggerBase<T>.RequestLoggerConfig.MaxLogFileSize, true);
			ActivityContext.OnActivityEvent += RequestDetailsLoggerBase<T>.GlobalActivityLogger;
			this.SetPerLogLineSizeLimit();
		}

		protected abstract RequestLoggerConfig GetRequestLoggerConfig();

		protected virtual void PreCommitTasks()
		{
		}

		protected virtual bool IsIgnorableException(Exception ex)
		{
			return false;
		}

		protected virtual bool LogFullException(Exception ex)
		{
			return true;
		}

		protected virtual void SetPerLogLineSizeLimit()
		{
			ActivityScopeImpl.MaxAppendableColumnLength = new int?(16384);
			RequestDetailsLoggerBase<T>.ErrorMessageLengthThreshold = new int?(250);
			RequestDetailsLoggerBase<T>.ProcessExceptionMessage = true;
		}

		public virtual bool ShouldSendDebugResponseHeaders()
		{
			return false;
		}

		protected virtual string GetDebugHeaderSource()
		{
			return "BE";
		}

		private void SafeAppend(Enum property, string key, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				value = value.Replace(",", " ").Replace(";", " ").Replace("=", " ").Replace(Environment.NewLine, " ");
				this.Append(property, key, value);
			}
		}

		protected void Append(Enum property, string key, string value)
		{
			StringBuilder stringBuilder = new StringBuilder(key.Length + value.Length + 2);
			stringBuilder.Append(key);
			stringBuilder.Append("=");
			stringBuilder.Append(value);
			stringBuilder.Append(";");
			try
			{
				if (!this.VerifyIsDisposed())
				{
					this.ActivityScope.AppendToProperty(property, stringBuilder.ToString());
				}
			}
			catch (ActivityContextException exception)
			{
				ExWatson.HandleException(new UnhandledExceptionEventArgs(exception, false), ReportOptions.None);
			}
		}

		protected void LogExceptionToGenericError(Exception ex, string keyPrefix)
		{
			if (!RequestDetailsLoggerBase<T>.ProcessExceptionMessage)
			{
				this.ActivityScope.SetProperty(ServiceCommonMetadata.GenericErrors, ex.ToString());
				string fullName = ex.GetType().FullName;
				this.ActivityScope.SetProperty(ServiceCommonMetadata.ExceptionName, fullName);
				string correctServerNameFromExceptionIfNecessary = RequestDetailsLoggerBase<T>.GetCorrectServerNameFromExceptionIfNecessary(fullName, ex);
				if (!string.IsNullOrWhiteSpace(correctServerNameFromExceptionIfNecessary))
				{
					this.ActivityScope.SetProperty(ServiceCommonMetadata.CorrectBEServerToUse, correctServerNameFromExceptionIfNecessary);
				}
				return;
			}
			if (this.LogFullException(ex))
			{
				this.AppendGenericError(keyPrefix, ex.ToString());
				return;
			}
			this.AppendGenericError(keyPrefix + "_Type", ex.GetType().Name);
			this.AppendGenericError(keyPrefix + "_Message", ex.Message);
			this.AppendGenericError(keyPrefix + "_OuterStackTrace", ex.StackTrace);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RequestDetailsLoggerBase<T>>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (!this.isDisposeAlreadyCalled && disposing && this.ActivityScope != null)
			{
				lock (this.SyncRoot)
				{
					if (!this.isDisposeAlreadyCalled)
					{
						this.excludeLogEntry |= this.SkipLogging;
						try
						{
							if (this.ActivityScope.ActivityType == ActivityType.Request)
							{
								this.FetchLatencyData();
								if (!this.excludeLogEntry)
								{
									this.PreCommitTasks();
								}
							}
							if (this.EndActivityContext && this.ActivityScope.Status == ActivityContextStatus.ActivityStarted)
							{
								this.ActivityScope.End();
							}
							if (!this.excludeLogEntry)
							{
								List<KeyValuePair<string, object>> formattableMetadata = this.ActivityScope.GetFormattableMetadata();
								formattableMetadata.RemoveAll(delegate(KeyValuePair<string, object> pair)
								{
									Enum @enum = ActivityContext.LookupEnum(pair.Key);
									return object.Equals(@enum, RequestDetailsLoggerBase<T>.RequestLoggerConfig.GenericInfoColumn) || object.Equals(@enum, ActivityStandardMetadata.ReturnClientRequestId) || RequestDetailsLoggerBase<T>.enumToIndexMap.ContainsKey(@enum);
								});
								List<KeyValuePair<string, object>> collection = WorkloadManagementLogger.FormatWlmActivity(this.ActivityScope, false);
								formattableMetadata.AddRange(collection);
								foreach (KeyValuePair<Enum, int> keyValuePair in RequestDetailsLoggerBase<T>.enumToIndexMap)
								{
									string text = this.Get(keyValuePair.Key);
									if (object.Equals(RequestDetailsLoggerBase<T>.RequestLoggerConfig.GenericInfoColumn, keyValuePair.Key))
									{
										text += LogRowFormatter.FormatCollection(formattableMetadata);
									}
									if (!string.IsNullOrEmpty(text))
									{
										text = RequestDetailsLoggerBase<T>.FormatForCsv(text);
										this.Row[keyValuePair.Value] = text;
									}
								}
								RequestDetailsLoggerBase<T>.Log.Append(this.Row, -1);
								this.UploadDataToStreamInsight();
							}
						}
						finally
						{
							if (this.ActivityScope != null && this.EndActivityContext && this.ActivityScope.Status == ActivityContextStatus.ActivityStarted)
							{
								this.ActivityScope.End();
							}
							this.isDisposeAlreadyCalled = true;
						}
					}
				}
			}
		}

		protected bool TryGetLogColumnValueByEnum(Enum columnName, out string value)
		{
			value = string.Empty;
			if (columnName == null)
			{
				return false;
			}
			try
			{
				int index;
				if (RequestDetailsLoggerBase<T>.enumToIndexMap.TryGetValue(columnName, out index) && this.Row[index] != null)
				{
					value = this.Row[index].ToString();
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.EventLog.LogEvent(CommonEventLogConstants.Tuple_StreamInsightsDataUploaderGetValueFailed, columnName.ToString(), new object[]
				{
					RequestDetailsLoggerBase<T>.RequestLoggerConfig.LogComponent,
					ex.Message
				});
			}
			return false;
		}

		protected static bool TryGetLogColumnIndexByEnum(Enum columnName, out int keyIndex)
		{
			return RequestDetailsLoggerBase<T>.enumToIndexMap.TryGetValue(columnName, out keyIndex);
		}

		protected virtual void UploadDataToStreamInsight()
		{
		}

		private void InitializeRequest(IActivityScope activityScope)
		{
			RequestDetailsLoggerBase<T>.SafeInitializeLogger(this);
			lock (this.SyncRoot)
			{
				if (this.ActivityScope == null)
				{
					if (activityScope == null)
					{
						this.ActivityScope = ActivityContext.Start(null);
					}
					else
					{
						this.ActivityScope = activityScope;
					}
					this.Row = new LogRowFormatter(RequestDetailsLoggerBase<T>.LogSchema);
				}
			}
		}

		private void FetchLatencyData()
		{
			if (this.latencies != null)
			{
				foreach (KeyValuePair<Enum, long> keyValuePair in this.latencies)
				{
					this.Set(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		public void AppendGenericError(string key, string value)
		{
			RequestDetailsLoggerBase<T>.SafeAppendGenericError(this, key, value);
		}

		public void PushDebugInfoToResponseHeaders()
		{
			if (this.httpContext != null)
			{
				this.PushDebugInfoToResponseHeaders(this.httpContext);
			}
		}

		public void PushDebugInfoToResponseHeaders(HttpContext httpContext)
		{
			if (httpContext != null && this.ShouldSendDebugResponseHeaders())
			{
				RequestDetailsLoggerBase<T>.SafeLogOperation<string, string>(this, string.Empty, string.Empty, delegate(RequestDetailsLoggerBase<T> logger, string k, string v)
				{
					logger.AddToResponseHeadersIfAvailable(httpContext, ServiceCommonMetadata.LiveIdBasicError);
					logger.AddToResponseHeadersIfAvailable(httpContext, ServiceCommonMetadata.LiveIdBasicLog);
					logger.AddToResponseHeadersIfAvailable(httpContext, ServiceCommonMetadata.LiveIdNegotiateError);
					logger.AddToResponseHeadersIfAvailable(httpContext, ServiceCommonMetadata.OAuthError);
					logger.AddToResponseHeadersIfAvailable(httpContext, ServiceCommonMetadata.GenericInfo);
					logger.AddToResponseHeadersIfAvailable(httpContext, ServiceCommonMetadata.AuthenticationErrors);
					logger.AddToResponseHeadersIfAvailable(httpContext, ServiceCommonMetadata.GenericErrors);
				});
			}
		}

		public void PushDebugInfoToResponseHeaders(string headerSuffix, string value)
		{
			if (base.IsDisposed)
			{
				return;
			}
			if (this.httpContext != null && this.ShouldSendDebugResponseHeaders())
			{
				this.AddToResponseHeadersIfAvailable(this.httpContext, headerSuffix, value);
			}
		}

		private void AddToResponseHeadersIfAvailable(HttpContext httpContext, Enum property)
		{
			if (base.IsDisposed)
			{
				return;
			}
			string value = this.Get(property);
			this.AddToResponseHeadersIfAvailable(httpContext, property.ToString(), value);
		}

		private void AddToResponseHeadersIfAvailable(HttpContext httpContext, string headerSuffix, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (value.Length > 1000)
				{
					value = value.Substring(0, 1000);
				}
				value = AntiXssEncoder.HtmlEncode(value, false);
				httpContext.SetResponseHeader(this.GetDebugHeaderSource(), headerSuffix, value);
			}
		}

		private bool VerifyIsDisposed()
		{
			return base.IsDisposed;
		}

		public const string KeyValueSeparator = "=";

		public const string PairSeparator = ";";

		public const string ColumnSeparator = ",";

		public const string SingleSplace = " ";

		public const int ResponseHeaderValueLengthLimit = 1000;

		private static readonly string ContextItemKey = string.Format("{0}-Logger", typeof(T).FullName);

		private static Dictionary<Enum, int> enumToIndexMap = new Dictionary<Enum, int>();

		private static object staticSyncRoot = new object();

		protected HttpContext httpContext;

		private bool isDisposeAlreadyCalled;

		private bool excludeLogEntry;

		private Dictionary<Enum, long> latencies;

		private object latenciesLock = new object();

		private object syncRoot = new object();

		protected delegate void LogOperation<TKey, TValue>(RequestDetailsLoggerBase<T> logger, TKey key, TValue value);
	}
}
