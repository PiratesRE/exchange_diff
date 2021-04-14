using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MapiHttp;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess
{
	internal abstract class MapiProbe : ProbeWorkItem
	{
		internal NetworkCredential ServerToServerCredential { private get; set; }

		private protected IContext Context { protected get; private set; }

		private protected ITask Task { protected get; private set; }

		private protected new InterpretedResult Result { protected get; private set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.ConfigureTask();
			this.Result = this.CreateInterpretedResult();
			this.Result.RawResult = base.Result;
			TplTaskEngine.BeginExecute(this.Task, cancellationToken).ContinueWith(new Action<Task>(this.TranslateTaskResults), TaskContinuationOptions.AttachedToParent);
		}

		protected abstract ITask CreateTask();

		protected virtual bool ShouldCreateRestrictedCredentials()
		{
			return false;
		}

		protected virtual InterpretedResult CreateInterpretedResult()
		{
			return new InterpretedResult();
		}

		protected MapiProbe()
		{
			this.AddOrUpdateRootCauseToComponentMapping("UnknownIssue", FailingComponent.Momt);
			this.AddOrUpdateRootCauseToComponentMapping("HighLatency", FailingComponent.Momt);
			this.AddOrUpdateRootCauseToComponentMapping("Passive", FailingComponent.ActiveManager);
			this.AddOrUpdateRootCauseToComponentMapping("SecureChannel", FailingComponent.Directory);
			this.AddOrUpdateRootCauseToComponentMapping("Networking", FailingComponent.Networking);
			this.AddOrUpdateRootCauseToComponentMapping("AccountIssue", FailingComponent.Directory);
			this.AddOrUpdateRootCauseToComponentMapping("Unauthorized", FailingComponent.Directory);
			this.AddOrUpdateRootCauseToComponentMapping("MapiHttpVersionMismatch", FailingComponent.Momt);
			this.AddOrUpdateRootCauseToComponentMapping("StoreFailure", FailingComponent.Store);
		}

		protected virtual void ProcessTaskException(Exception ex)
		{
			WebException ex2 = ex as WebException;
			if (ex is CallCancelledException || (ex2 != null && ex2.Status == WebExceptionStatus.RequestCanceled))
			{
				this.SetRootCause("UnknownIssue");
			}
			else if (ex.GetBaseException().Message.Contains("prm[2]: Long val: 1703 (0x000006A7)"))
			{
				this.SetRootCause("SecureChannel");
			}
			else if (ex.GetBaseException() is SocketException || (ex2 != null && ex2.Status == WebExceptionStatus.NameResolutionFailure))
			{
				this.SetRootCause("Networking");
			}
			else if (MapiProbe.DidProbeFailDueToStoreIssue(ex))
			{
				this.SetRootCause("StoreFailure");
			}
			else if (this.DidProbeFailDueToAccountIssue(ex))
			{
				this.SetRootCause("AccountIssue");
			}
			else if (this.DidProbeFailDueToCredentialIssue(ex))
			{
				this.SetRootCause("Unauthorized");
			}
			throw new AggregateException(new Exception[]
			{
				ex
			});
		}

		protected virtual void ConfigureProbeTaskTimeout()
		{
			this.Context.Properties.Set(ContextPropertySchema.Timeout, TimeSpan.FromSeconds((double)base.Definition.TimeoutSeconds));
		}

		protected void SetRootCause(string rootCause)
		{
			this.Result.SetRootCause(rootCause, this.RootCauseToComponentMapping.GetValueOrDefault(rootCause, FailingComponent.Momt));
		}

		protected void AddOrUpdateRootCauseToComponentMapping(string rootCauseName, FailingComponent component)
		{
			ArgumentValidator.ThrowIfNull("rootCauseName", rootCauseName);
			this.RootCauseToComponentMapping[rootCauseName] = component;
		}

		protected bool DidProbeFailDueToInvalidRequestType(Exception ex)
		{
			InvalidRequestTypeException ex2 = ex as InvalidRequestTypeException;
			if (ex2 != null && ex2.ResponseCode == ResponseCode.InvalidRequestType)
			{
				if (ex2.MapiHttpVersion == null)
				{
					return true;
				}
				if (ex2.MapiHttpVersion < this.mapiHttpVerbsUpdateVersion)
				{
					return true;
				}
			}
			return false;
		}

		protected bool DidProbeFailDueToDatabaseMountedElsewhere(Exception ex)
		{
			WebException ex2 = ex as WebException;
			ServerUnavailableException ex3 = ex as ServerUnavailableException;
			return (ex2 != null && this.Context.Properties.GetNullableOrDefault(ContextPropertySchema.ResponseStatusCode) == (HttpStatusCode)555) || (ex3 != null && ex.GetBaseException().Message.Contains("prm[0]: Long val: 555 (0x0000022B)")) || MapiProbe.DidProbeFailDueToPassiveMDB(ex);
		}

		protected bool DidProbeFailDueToCredentialIssue(Exception ex)
		{
			WebException ex2 = ex as WebException;
			RpcException ex3 = ex as RpcException;
			if (ex2 == null && ex3 == null)
			{
				ex2 = (ex.GetBaseException() as WebException);
			}
			return (ex2 != null && this.Context.Properties.GetNullableOrDefault(ContextPropertySchema.ResponseStatusCode) == HttpStatusCode.Unauthorized) || (ex3 != null && ex.GetBaseException().Message.Contains("Status: 0x00000005"));
		}

		protected bool DidProbeFailDueToAccountIssue(Exception ex)
		{
			WebException ex2 = ex as WebException;
			if (ex2 != null)
			{
				HttpStatusCode value = this.Context.Properties.GetNullableOrDefault(ContextPropertySchema.ResponseStatusCode).Value;
				if (value == (HttpStatusCode)456 || value == (HttpStatusCode)457)
				{
					return true;
				}
			}
			return false;
		}

		protected static bool DidProbeFailDueToStoreIssue(Exception ex)
		{
			return MapiProbe.SearchExceptionString(ex, MapiProbe.KnownStoreMailboxExceptions);
		}

		protected static bool DidProbeFailDueToPassiveMDB(Exception ex)
		{
			return MapiProbe.SearchExceptionString(ex, MapiProbe.KnownStorePassiveMDBExceptions);
		}

		protected static bool SearchExceptionString(Exception exception, string[] expectedExceptionsNames)
		{
			string exceptionString = exception.ToString();
			return expectedExceptionsNames.Any((string exceptionName) => exceptionString.Contains(exceptionName));
		}

		private void ConfigureTask()
		{
			this.CreateContext();
			this.ConfigureEndpoint();
			this.ConfigureProbeTaskTimeout();
			this.ConfigureAuthentication();
			this.ConfigureEmsmdbParameters();
			this.Task = this.CreateTask();
		}

		private void CreateContext()
		{
			this.Context = Microsoft.Exchange.RpcClientAccess.Monitoring.Context.CreateRoot(new Environment(), new CompositeLogger
			{
				this.outlineLogger,
				this.briefLogger
			});
			this.Context.Properties.Set(ContextPropertySchema.AdditionalHttpHeaders, new WebHeaderCollection());
		}

		private void ConfigureAuthentication()
		{
			if (base.Definition.AccountPassword != null)
			{
				RpcProxyPort asEnum = this.GetAsEnum<RpcProxyPort>("RpcProxyPort", Microsoft.Exchange.Rpc.RpcProxyPort.Default);
				this.Context.Properties.Set(ContextPropertySchema.RpcProxyPort, asEnum);
				if (asEnum.Equals(Microsoft.Exchange.Rpc.RpcProxyPort.Backend))
				{
					this.PopulateAdditionalHttpHeaders("X-RpcHttpProxyServerTarget", base.Definition.Attributes["PersonalizedServerName"]);
				}
				HttpAuthenticationScheme targetAuthenticationScheme = this.GetAsEnum<HttpAuthenticationScheme>("RpcProxyAuthenticationType", HttpAuthenticationScheme.Basic);
				this.Context.Properties.Set(ContextPropertySchema.RpcProxyAuthenticationType, targetAuthenticationScheme);
				this.Context.Properties.Set(ContextPropertySchema.RpcAuthenticationType, this.GetAsEnum<AuthenticationService>("RpcAuthenticationType", AuthenticationService.None));
				ICredentials credentials = new NetworkCredential(base.Definition.Account, base.Definition.AccountPassword);
				if (targetAuthenticationScheme == HttpAuthenticationScheme.Basic && this.ShouldCreateRestrictedCredentials())
				{
					credentials = new RestrictedCredentials(credentials, (string requestedAuthType) => StringComparer.OrdinalIgnoreCase.Equals(requestedAuthType, targetAuthenticationScheme.ToString()));
				}
				this.Context.Properties.Set(ContextPropertySchema.Credentials, credentials);
				return;
			}
			this.Context.Properties.Set(ContextPropertySchema.Credentials, this.ServerToServerCredential);
			this.PopulateAdditionalHttpHeaders("X-CommonAccessToken", base.Definition.Account);
			this.PopulateAdditionalHttpHeaders("X-RpcHttpProxyServerTarget", base.Definition.Attributes["PersonalizedServerName"]);
			this.Context.Properties.Set(ContextPropertySchema.RpcProxyPort, Microsoft.Exchange.Rpc.RpcProxyPort.Backend);
			this.Context.Properties.Set(ContextPropertySchema.RpcProxyAuthenticationType, HttpAuthenticationScheme.Negotiate);
			this.Context.Properties.Set(ContextPropertySchema.RpcAuthenticationType, AuthenticationService.None);
		}

		private void PopulateAdditionalHttpHeaders(string headerName, string headerValue)
		{
			WebHeaderCollection webHeaderCollection = this.Context.Properties.Get(ContextPropertySchema.AdditionalHttpHeaders);
			webHeaderCollection.Add(headerName, headerValue);
		}

		private void ConfigureEmsmdbParameters()
		{
			string text = base.Definition.Attributes["AccountLegacyDN"];
			this.Context.Properties.Set(ContextPropertySchema.UserLegacyDN, text);
			this.Context.Properties.Set(ContextPropertySchema.MailboxLegacyDN, base.Definition.Attributes.GetValueOrDefault("MailboxLegacyDN", text));
		}

		private void ConfigureEndpoint()
		{
			this.Context.Properties.Set(ContextPropertySchema.RpcProxyServer, base.Definition.Endpoint);
			this.Context.Properties.Set(ContextPropertySchema.RpcServer, base.Definition.SecondaryEndpoint);
			this.Context.Properties.Set(ContextPropertySchema.WebProxyServer, "<none>");
			this.Context.Properties.Set(ContextPropertySchema.MapiHttpServer, base.Definition.Endpoint);
			this.Context.Properties.Set(ContextPropertySchema.MapiHttpPersonalizedServerName, base.Definition.SecondaryEndpoint);
		}

		private T GetAsEnum<T>(string extendedAttributeName, T defaultValue)
		{
			string value;
			if (!base.Definition.Attributes.TryGetValue(extendedAttributeName, out value))
			{
				return defaultValue;
			}
			return (T)((object)Enum.Parse(typeof(T), value));
		}

		protected string GetOspOutsideInChartUrl()
		{
			string text;
			if ((!Datacenter.IsMicrosoftHostedOnly(true) && !Datacenter.IsDatacenterDedicated(true)) || !base.Definition.Attributes.TryGetValue("SiteName", out text) || string.IsNullOrEmpty(text))
			{
				return null;
			}
			return Utils.GetOspOutsideInProbeResultLink("OutlookCTP Probe", string.Format("*.*.*.{0}", text));
		}

		protected virtual void FillOutStateAttributes()
		{
			IPropertyBag properties = this.Context.Properties;
			this.Result.RespondingHttpServer = (properties.GetOrDefault(ContextPropertySchema.RespondingHttpServer) ?? properties.GetOrDefault(ContextPropertySchema.RpcProxyServer));
			this.Result.RespondingRpcProxyServer = (properties.GetOrDefault(ContextPropertySchema.RespondingRpcProxyServer) ?? "Unknown");
			this.Result.MonitoringAccount = base.Definition.Account;
			this.Result.OutlookSessionCookie = properties.GetOrDefault(ContextPropertySchema.OutlookSessionCookieValue);
			this.Result.UserLegacyDN = properties.GetOrDefault(ContextPropertySchema.UserLegacyDN);
			this.Result.RequestUrl = properties.GetOrDefault(ContextPropertySchema.RequestUrl);
			this.Result.AuthType = properties.Get(ContextPropertySchema.RpcProxyAuthenticationType).ToString();
			this.Result.TotalLatency = this.outlineLogger.TotalLatency;
			this.Result.ActivityContext = properties.GetOrDefault(ContextPropertySchema.ActivityContext);
			this.Result.FirstFailedTaskName = this.outlineLogger.FirstFailedTaskName;
			this.Result.ExecutionOutline = this.outlineLogger.GetExecutionOutline();
			this.Result.OspUrl = this.GetOspOutsideInChartUrl();
		}

		private void TranslateTaskResults(Task parentTask)
		{
			try
			{
				this.FillOutStateAttributes();
				IPropertyBag properties = this.Context.Properties;
				Exception ex = null;
				if (this.Task.Result == TaskResult.Success && this.outlineLogger.TotalLatency > MapiProbe.UserPerceivedScenarioTimeout)
				{
					this.SetRootCause("HighLatency");
				}
				else if (this.Task.Result == TaskResult.Failed)
				{
					this.Result.VerboseLog = this.briefLogger.GetExecutionContextLog();
					this.Result.ErrorDetails = properties.GetOrDefault(ContextPropertySchema.ErrorDetails);
					if (!this.Context.Properties.TryGet(ContextPropertySchema.Exception, out ex))
					{
						ex = new Exception(this.Result.ErrorDetails ?? this.Result.VerboseLog);
					}
				}
				else
				{
					ex = null;
				}
				if (ex != null)
				{
					this.SetRootCause("UnknownIssue");
					this.ProcessTaskException(ex);
				}
			}
			finally
			{
				this.Result.OnBeforeSerialize();
			}
		}

		public const string Account = "Account";

		public const string Password = "Password";

		public const string Identity = "Identity";

		public const string Endpoint = "Endpoint";

		public const string SecondaryEndpoint = "SecondaryEndpoint";

		public const string ItemTargetExtension = "ItemTargetExtension";

		public const string TimeoutSeconds = "TimeoutSeconds";

		public const string AccountDisplayName = "AccountDisplayName";

		public const string ActAsLegacyDN = "AccountLegacyDN";

		public const string MailboxLegacyDN = "MailboxLegacyDN";

		public const string PersonalizedServerName = "PersonalizedServerName";

		public const string RpcProxyPort = "RpcProxyPort";

		public const string RpcProxyAuthenticationType = "RpcProxyAuthenticationType";

		public const string RpcAuthenticationType = "RpcAuthenticationType";

		public const string SiteName = "SiteName";

		protected const int HttpStatusCodeDatabaseMountedElsewhere = 555;

		protected const int AccountLockedOutOrTermsOfUseNotAccepted = 456;

		protected const int AccountPasswordExpired = 457;

		public static readonly TimeSpan UserPerceivedScenarioTimeout = TimeSpan.FromSeconds(30.0);

		private readonly MapiProbe.BriefLogger briefLogger = new MapiProbe.BriefLogger();

		private readonly MapiProbe.OutlineLogger outlineLogger = new MapiProbe.OutlineLogger();

		private readonly Dictionary<string, FailingComponent> RootCauseToComponentMapping = new Dictionary<string, FailingComponent>();

		private readonly MapiHttpVersion mapiHttpVerbsUpdateVersion = new MapiHttpVersion(15, 0, 732, 0);

		private static readonly string[] KnownStorePassiveMDBExceptions = new string[]
		{
			typeof(MapiExceptionIllegalCrossServerConnection).Name,
			typeof(WrongServerException).Name,
			ErrorCode.WrongServer.ToString()
		};

		private static readonly string[] KnownStoreMailboxExceptions = new string[]
		{
			typeof(MapiExceptionMailboxQuarantined).Name,
			typeof(MapiExceptionMdbOffline).Name,
			ErrorCode.MdbOffline.ToString()
		};

		private class OutlineLogger : ILogger
		{
			public string FirstFailedTaskName
			{
				get
				{
					MapiProbe.OutlineLogger.TaskExecutionRecord taskExecutionRecord = this.executionOutline.FirstOrDefault((MapiProbe.OutlineLogger.TaskExecutionRecord record) => record.Result == TaskResult.Failed);
					if (taskExecutionRecord == null)
					{
						return null;
					}
					return taskExecutionRecord.Task.TaskTitle;
				}
			}

			public TimeSpan TotalLatency
			{
				get
				{
					return this.executionOutline.Aggregate(TimeSpan.Zero, (TimeSpan accumulator, MapiProbe.OutlineLogger.TaskExecutionRecord record) => accumulator + record.Latency);
				}
			}

			public string GetExecutionOutline()
			{
				return string.Concat(from record in this.executionOutline
				select string.Format("[{0}]{1} {2} {3}; ", new object[]
				{
					(int)record.Latency.TotalMilliseconds,
					record.StartTime.ToString(),
					(record.Result == TaskResult.Success) ? LocalizedString.Empty : "[FAILED!]",
					record.Task.TaskTitle
				}));
			}

			void ILogger.TaskStarted(ITaskDescriptor task)
			{
			}

			void ILogger.TaskCompleted(ITaskDescriptor task, TaskResult result)
			{
				if (this.ShouldLogTask(task))
				{
					TimeSpan latency;
					task.Properties.TryGet(ContextPropertySchema.Latency, out latency);
					ExDateTime startTime;
					task.Properties.TryGet(ContextPropertySchema.TaskStarted, out startTime);
					this.executionOutline.Add(new MapiProbe.OutlineLogger.TaskExecutionRecord
					{
						Task = task,
						Result = result,
						Latency = latency,
						StartTime = startTime
					});
				}
			}

			protected bool ShouldLogTask(ITaskDescriptor task)
			{
				return task.DependentProperties.Contains(ContextPropertySchema.Latency);
			}

			private readonly List<MapiProbe.OutlineLogger.TaskExecutionRecord> executionOutline = new List<MapiProbe.OutlineLogger.TaskExecutionRecord>();

			private class TaskExecutionRecord
			{
				public ITaskDescriptor Task { get; set; }

				public TaskResult Result { get; set; }

				public TimeSpan Latency { get; set; }

				public ExDateTime StartTime { get; set; }
			}
		}

		private class BriefLogger : ScomAlertLogger
		{
			public string GetExecutionContextLog()
			{
				return this.taskLog.ToString();
			}

			protected override void LogTaskCaption(ITaskDescriptor task)
			{
				base.LogHierarchicalOutput(task.TaskTitle);
			}

			protected override void LogInputProperties(ITaskDescriptor task)
			{
			}

			protected override void OnLogOutput(LocalizedString output)
			{
				this.taskLog.AppendLine(output);
				base.OnLogOutput(output);
			}

			protected override bool ShouldLogTask(ITaskDescriptor task)
			{
				return task.TaskType != TaskType.Infrastructure;
			}

			public BriefLogger() : base(null)
			{
			}

			private readonly StringBuilder taskLog = new StringBuilder();
		}
	}
}
