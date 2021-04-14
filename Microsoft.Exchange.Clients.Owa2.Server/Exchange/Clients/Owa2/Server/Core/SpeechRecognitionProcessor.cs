using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SpeechRecognitionProcessor
	{
		public SpeechRecognitionProcessor(HttpContext httpContext)
		{
			this.speechScenario = SpeechRecognitionProcessor.CreateSpeechRecognitionScenario(httpContext, out this.budget);
			this.HttpContext = httpContext;
			this.parameters = this.speechScenario.Parameters;
			this.userContext = this.speechScenario.UserContext;
			this.maxAudioSize = AppConfigLoader.GetConfigIntValue("SpeechRecognitionMaxAudioSize", 0, int.MaxValue, 32500);
		}

		public HttpContext HttpContext { get; private set; }

		private string RequestId
		{
			get
			{
				if (this.parameters == null)
				{
					return string.Empty;
				}
				return this.parameters.RequestId.ToString();
			}
		}

		private string UserObjectGuid
		{
			get
			{
				if (this.parameters == null)
				{
					return string.Empty;
				}
				return this.parameters.UserObjectGuid.ToString();
			}
		}

		private string TenantGuid
		{
			get
			{
				if (this.parameters == null)
				{
					return string.Empty;
				}
				return this.parameters.TenantGuid.ToString();
			}
		}

		public IAsyncResult BeginRecognition(AsyncCallback callback, object context)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionProcessor.BeginRecognition");
			this.CollectAndLogStatisticsInformation(SpeechLoggerProcessType.BeginRequest, -1);
			this.clientCallback = callback;
			this.asyncResult = new SpeechRecognitionAsyncResult(new AsyncCallback(this.OnRequestCompleted), context);
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.DoRecognition));
			return this.asyncResult;
		}

		private static SpeechRecognitionScenarioBase CreateSpeechRecognitionScenario(HttpContext httpContext, out IStandardBudget budget)
		{
			ValidateArgument.NotNull(httpContext, "httpContext is null");
			budget = null;
			Exception ex = null;
			SpeechRecognitionScenarioBase result = null;
			try
			{
				Guid guid = Guid.NewGuid();
				string text;
				MobileSpeechRecoRequestType mobileSpeechRecoRequestType;
				CultureInfo cultureInfo;
				ExTimeZone exTimeZone;
				SpeechRecognitionProcessor.GetQueryStringParameters(httpContext.Request, out text, out mobileSpeechRecoRequestType, out cultureInfo, out exTimeZone);
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug(0L, "SpeechRecognitionProcessor.CreateSpeechRecognitionProcessor - requestId='{0}', tag='{1}', requestType='{2}', culture='{3}', timeZone='{4}'", new object[]
				{
					guid,
					text,
					mobileSpeechRecoRequestType,
					cultureInfo,
					exTimeZone
				});
				Guid guid2;
				Guid guid3;
				OrganizationId organizationId;
				UserContext userContext;
				SpeechRecognitionProcessor.GetUserIdentity(httpContext, out guid2, out guid3, out organizationId, out userContext);
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Guid, Guid, OrganizationId>(0L, "SpeechRecognitionProcessor.CreateSpeechRecognitionProcessor - userObjectGuid='{0}', tenantGuid='{1}', orgId='{2}'", guid2, guid3, organizationId);
				RequestParameters requestParameters = new RequestParameters(guid, text, mobileSpeechRecoRequestType, cultureInfo, exTimeZone, guid2, guid3, organizationId);
				switch (mobileSpeechRecoRequestType)
				{
				case MobileSpeechRecoRequestType.FindInGAL:
				case MobileSpeechRecoRequestType.FindInPersonalContacts:
				case MobileSpeechRecoRequestType.StaticGrammarsCombined:
					throw new ArgumentOutOfRangeException("operation", mobileSpeechRecoRequestType, "Invalid parameter");
				case MobileSpeechRecoRequestType.FindPeople:
					result = new FindPeopleSpeechRecognitionScenario(requestParameters, userContext);
					break;
				case MobileSpeechRecoRequestType.CombinedScenarios:
					result = new CombinedSpeechRecognitionScenario(requestParameters, userContext);
					break;
				case MobileSpeechRecoRequestType.DaySearch:
				case MobileSpeechRecoRequestType.AppointmentCreation:
					result = new SingleSpeechRecognitionScenario(requestParameters, userContext);
					break;
				default:
					ExAssert.RetailAssert(false, "Invalid request type '{0}'", new object[]
					{
						mobileSpeechRecoRequestType
					});
					break;
				}
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechRecoRequestParams, null, new object[]
				{
					guid,
					text,
					mobileSpeechRecoRequestType,
					cultureInfo,
					exTimeZone,
					guid2,
					guid3,
					organizationId
				});
				string text2 = null;
				HttpRequest request = httpContext.Request;
				if (request.QueryString != null)
				{
					text2 = request.QueryString.ToString();
				}
				if (request.Headers != null && !string.IsNullOrEmpty(request.Headers["X-OWA-CorrelationId"]))
				{
					text2 = text2 + "." + request.Headers["X-OWA-CorrelationId"];
				}
				SpeechRecognitionProcessor.InitializeThrottlingBudget(userContext, text2, out budget);
			}
			catch (OverBudgetException ex2)
			{
				ex = ex2;
			}
			catch (ArgumentOutOfRangeException ex3)
			{
				ex = ex3;
			}
			catch (Exception ex4)
			{
				ex = ex4;
				ExWatson.SendReport(ex4, ReportOptions.None, null);
			}
			finally
			{
				if (ex != null)
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Exception>(0L, "SpeechRecognitionProcessor.CreateSpeechRecognitionProcessor - Exception='{0}'", ex);
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_InvalidSpeechRecoRequest, null, new object[]
					{
						CommonUtil.ToEventLogString(ex)
					});
					SpeechRecognitionProcessor.SpeechHttpStatus status = SpeechRecognitionProcessor.MapInvalidRequestToHttpStatus(ex);
					result = new InvalidRequestSpeechRecognitionScenario(status);
				}
			}
			return result;
		}

		private static void InitializeThrottlingBudget(UserContext userContext, string description, out IStandardBudget budget)
		{
			ValidateArgument.NotNull(userContext, "userContext is null");
			budget = null;
			try
			{
				string callerInfo = string.IsNullOrEmpty(description) ? "SpeechRecognitionProcessor.InitializeThrottlingBudget" : description;
				budget = StandardBudget.Acquire(userContext.ExchangePrincipal.Sid, BudgetType.OwaVoice, userContext.ExchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings());
				budget.CheckOverBudget();
				budget.StartConnection(callerInfo);
				budget.StartLocal(callerInfo, default(TimeSpan));
			}
			catch (Exception ex)
			{
				if (budget != null)
				{
					budget.Dispose();
					budget = null;
				}
				throw ex;
			}
		}

		private static void GetQueryStringParameters(HttpRequest request, out string tag, out MobileSpeechRecoRequestType requestType, out CultureInfo culture, out ExTimeZone timeZone)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug(0L, "Entering SpeechRecognitionProcessor.GetQueryStringParameters");
			NameValueCollection queryString = request.QueryString;
			string text = queryString["tag"];
			string text2 = queryString["operation"];
			string text3 = queryString["culture"];
			string text4 = queryString["timezone"];
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentOutOfRangeException("tag", "Parameter was not specified");
			}
			tag = text;
			if (string.IsNullOrEmpty(text4))
			{
				throw new ArgumentOutOfRangeException("timezone", "Parameter was not specified");
			}
			if (!ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(text4, out timeZone))
			{
				throw new ArgumentOutOfRangeException("timezone", text4, "Invalid parameter");
			}
			if (text2 == null)
			{
				throw new ArgumentOutOfRangeException("operation", "Parameter was not specified");
			}
			if (!EnumValidator.TryParse<MobileSpeechRecoRequestType>(text2, EnumParseOptions.IgnoreCase, out requestType))
			{
				throw new ArgumentOutOfRangeException("operation", text2, "Invalid parameter");
			}
			if (text3 == null)
			{
				throw new ArgumentOutOfRangeException("culture", "Parameter was not specified");
			}
			try
			{
				culture = new CultureInfo(text3);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentOutOfRangeException("culture", text3, ex.Message);
			}
		}

		private static void GetUserIdentity(HttpContext httpContext, out Guid userObjectGuid, out Guid tenantGuid, out OrganizationId orgId, out UserContext userContext)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug(0L, "Entering SpeechRecognitionProcessor.GetUserIdentity");
			userContext = UserContextManager.GetUserContext(httpContext);
			userObjectGuid = userContext.ExchangePrincipal.ObjectId.ObjectGuid;
			orgId = userContext.ExchangePrincipal.MailboxInfo.OrganizationId;
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(orgId);
			tenantGuid = iadsystemConfigurationLookup.GetExternalDirectoryOrganizationId();
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Guid, Guid, OrganizationId>(0L, "SpeechRecognitionProcessor.GetUserIdentity - userObjectGuid='{0}', tenantGuid='{1}', orgId='{2}'", userObjectGuid, tenantGuid, orgId);
		}

		private static SpeechRecognitionProcessor.SpeechHttpStatus MapInvalidRequestToHttpStatus(Exception ex)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<Exception>(0L, "SpeechRecognitionProcessor.MapInvalidRequestToHttpStatus - ex='{0}'", ex);
			if (ex is ArgumentOutOfRangeException)
			{
				return SpeechRecognitionProcessor.SpeechHttpStatus.BadRequest;
			}
			if (ex is OverBudgetException)
			{
				return SpeechRecognitionProcessor.SpeechHttpStatus.MaxRequestsExceeded;
			}
			return SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError;
		}

		private void DoRecognition(object state)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionProcessor.DoRecognition");
			try
			{
				this.speechScenario.StartRecoRequestAsync(new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedDelegate(this.OnRecognizeCompleted));
				this.CollectAndLogStatisticsInformation(SpeechLoggerProcessType.BeginReadAudio, -1);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.ReadAudioBytesAsync));
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "SpeechRecognitionProcessor.DoRecognition - Called scenario.StartRecoRequestAsync");
			}
			catch (Exception e)
			{
				this.HandleUnexpectedException(e);
			}
		}

		private void ReadAudioBytesAsync(object state)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionProcessor.ReadAudioBytesAsync");
			SpeechRecognitionProcessor.SpeechStreamBuffer speechStreamBuffer = new SpeechRecognitionProcessor.SpeechStreamBuffer();
			try
			{
				this.audioMemoryStream = new MemoryStream(16250);
				speechStreamBuffer.AudioStream = this.HttpContext.Request.GetBufferlessInputStream();
				speechStreamBuffer.AudioBuffer = new byte[400];
				speechStreamBuffer.AudioStream.BeginRead(speechStreamBuffer.AudioBuffer, 0, 400, new AsyncCallback(this.ReadAudioChunkCompleted), speechStreamBuffer);
			}
			catch (HttpException e)
			{
				this.SignalRecognizeWithEmptyAudioAndBailOut(speechStreamBuffer, e, SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError, true);
			}
			catch (Exception e2)
			{
				this.SignalRecognizeWithEmptyAudioAndBailOut(speechStreamBuffer, e2, SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError, false);
			}
		}

		private void ReadAudioChunkCompleted(IAsyncResult asyncResult)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionProcessor.BeginReadAudioBytes");
			SpeechRecognitionProcessor.SpeechStreamBuffer speechStreamBuffer = null;
			try
			{
				speechStreamBuffer = (asyncResult.AsyncState as SpeechRecognitionProcessor.SpeechStreamBuffer);
				int num = speechStreamBuffer.AudioStream.EndRead(asyncResult);
				if (num == 0)
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "SpeechRecognitionProcessor.BeginReadAudioBytes - End of stream");
					this.SignalRecognizeAsync();
					speechStreamBuffer.Dispose();
				}
				else
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "SpeechRecognitionProcessor.BeginReadAudioBytes - Read chunk");
					this.audioMemoryStream.Write(speechStreamBuffer.AudioBuffer, 0, num);
					if (this.audioMemoryStream.Length > (long)this.maxAudioSize)
					{
						ExTraceGlobals.SpeechRecognitionTracer.TraceError<int>((long)this.GetHashCode(), "SpeechRecognitionProcessor.BeginReadAudioBytes - Max audio size ({0}) exceeded", this.maxAudioSize);
						this.SignalRecognizeWithEmptyAudioAndBailOut(speechStreamBuffer, new ArgumentException("Max audio size exceeded"), SpeechRecognitionProcessor.SpeechHttpStatus.BadRequest, true);
					}
					else
					{
						speechStreamBuffer.AudioStream.BeginRead(speechStreamBuffer.AudioBuffer, 0, 400, new AsyncCallback(this.ReadAudioChunkCompleted), speechStreamBuffer);
					}
				}
			}
			catch (HttpException e)
			{
				this.SignalRecognizeWithEmptyAudioAndBailOut(speechStreamBuffer, e, SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError, true);
			}
			catch (Exception e2)
			{
				this.SignalRecognizeWithEmptyAudioAndBailOut(speechStreamBuffer, e2, SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError, false);
			}
		}

		private void SignalRecognizeWithEmptyAudioAndBailOut(SpeechRecognitionProcessor.SpeechStreamBuffer speechStreamBuffer, Exception e, SpeechRecognitionProcessor.SpeechHttpStatus status, bool expectedException)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceError<string, int, string>((long)this.GetHashCode(), "SignalRecognizeWithEmptyAudioAndBailOut - exception='{0}' Status code='{1}', Status message='{2}'", e.Message, status.StatusCode, status.StatusDescription);
			if (this.audioMemoryStream != null)
			{
				this.audioMemoryStream.Close();
			}
			this.audioMemoryStream = new MemoryStream();
			this.SignalRecognizeAsync();
			if (speechStreamBuffer != null)
			{
				speechStreamBuffer.Dispose();
			}
			if (expectedException)
			{
				this.HandleException(e, status);
				return;
			}
			this.HandleUnexpectedException(e);
		}

		private void SignalRecognizeAsync()
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionProcessor.SignalRecognizeAsync");
			this.CollectAndLogStatisticsInformation(SpeechLoggerProcessType.EndReadAudio, this.audioMemoryStream.ToArray().Length);
			this.speechScenario.SetAudio(this.audioMemoryStream.ToArray());
			this.audioMemoryStream.Close();
			this.audioMemoryStream = null;
		}

		private void OnRecognizeCompleted(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionProcessor.OnRecognizeCompleted");
			if (args.HttpStatus == SpeechRecognitionProcessor.SpeechHttpStatus.Success)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.HandleRecoResults), args);
				return;
			}
			this.CompleteRequest(args);
		}

		private void HandleRecoResults(object state)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionProcessor.HandleRecoResults");
			try
			{
				SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs speechProcessorAsyncCompletedArgs = (SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs)state;
				string responseText;
				SpeechRecognitionProcessor.SpeechHttpStatus httpStatus;
				SpeechRecognitionResultHandler.HandleRecoResult(speechProcessorAsyncCompletedArgs.ResponseText, this.parameters, this.HttpContext, this.userContext, out responseText, out httpStatus);
				SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args = new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(responseText, httpStatus);
				this.CompleteRequest(args);
			}
			catch (ArgumentException e)
			{
				this.HandleException(e, SpeechRecognitionProcessor.SpeechHttpStatus.BadRequest);
			}
			catch (Exception e2)
			{
				this.HandleUnexpectedException(e2);
			}
		}

		private void CompleteRequest(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args)
		{
			if (this.IsSpeechRequestNotCompleted())
			{
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int, string>((long)this.GetHashCode(), "SpeechRecognitionProcessor - Status code='{0}', Status message='{1}'", args.HttpStatus.StatusCode, args.HttpStatus.StatusDescription);
				this.CollectAndLogStatisticsInformation(SpeechLoggerProcessType.RequestCompleted, -1);
				this.asyncResult.StatusCode = args.HttpStatus.StatusCode;
				this.asyncResult.StatusDescription = args.HttpStatus.StatusDescription;
				this.asyncResult.ResponseText = args.ResponseText;
				this.asyncResult.ThrottlingDelay = -1.0;
				this.asyncResult.ThrottlingNotEnforcedReason = string.Empty;
				if (this.budget != null)
				{
					try
					{
						this.budget.EndLocal();
						DelayEnforcementResults delayEnforcementResults = ResourceLoadDelayInfo.EnforceDelay(this.budget, new WorkloadSettings(WorkloadType.OwaVoice, false), null, TimeSpan.MaxValue, null);
						if (delayEnforcementResults != null && delayEnforcementResults.DelayInfo != null)
						{
							ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "SpeechRecognitionProcessor - Request id={0}, Delayed amount={1}s, Capped delay={2}s, Delay Required={3}, NotEnforcedReason={4}", new object[]
							{
								this.RequestId,
								delayEnforcementResults.DelayedAmount.TotalSeconds,
								delayEnforcementResults.DelayInfo.Delay.TotalSeconds,
								delayEnforcementResults.DelayInfo.Required,
								delayEnforcementResults.NotEnforcedReason
							});
							this.asyncResult.ThrottlingDelay = delayEnforcementResults.DelayedAmount.TotalSeconds;
							this.asyncResult.ThrottlingNotEnforcedReason = delayEnforcementResults.NotEnforcedReason;
						}
						this.budget.EndConnection();
					}
					finally
					{
						this.budget.Dispose();
						this.budget = null;
					}
				}
				this.asyncResult.IsCompleted = true;
				return;
			}
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "SpeechRecognitionProcessor.CompleteRequest: speech request already completed, ignoring this request.");
		}

		private bool IsSpeechRequestNotCompleted()
		{
			int num = Interlocked.CompareExchange(ref this.speechRequestCompleted, 1, 0);
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "SpeechRecognitionProcessor.SpeechRequestNotCompleted value of speechRequestCompleted:{0}", num);
			return num == 0;
		}

		private void OnRequestCompleted(IAsyncResult asyncResult)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionProcessor.OnRequestCompleted");
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.InvokeClientCallback), asyncResult);
		}

		private void InvokeClientCallback(object state)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug((long)this.GetHashCode(), "Entering SpeechRecognitionProcessor.InvokeClientCallback");
			if (this.clientCallback != null)
			{
				SpeechRecognitionAsyncResult speechRecognitionAsyncResult = state as SpeechRecognitionAsyncResult;
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechRecoRequestCompleted, null, new object[]
				{
					this.RequestId,
					speechRecognitionAsyncResult.StatusCode,
					CommonUtil.ToEventLogString(speechRecognitionAsyncResult.StatusDescription),
					CommonUtil.ToEventLogString(speechRecognitionAsyncResult.ResponseText),
					speechRecognitionAsyncResult.ThrottlingDelay,
					CommonUtil.ToEventLogString(speechRecognitionAsyncResult.ThrottlingNotEnforcedReason)
				});
				this.clientCallback(state as IAsyncResult);
			}
		}

		private void HandleUnexpectedException(Exception e)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceError<Exception>((long)this.GetHashCode(), "SpeechRecognitionProcessor - HandleUnexpectedException='{0}'", e);
			ExWatson.SendReport(e, ReportOptions.None, null);
			this.HandleException(e, SpeechRecognitionProcessor.SpeechHttpStatus.InternalServerError);
		}

		private void HandleException(Exception e, SpeechRecognitionProcessor.SpeechHttpStatus status)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceError<Exception, int, string>((long)this.GetHashCode(), "SpeechRecognitionProcessor - Exception='{0}', Status Code='{1}', Status Description='{2}'", e, status.StatusCode, status.StatusDescription);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechRecoRequestFailed, null, new object[]
			{
				this.RequestId,
				this.UserObjectGuid,
				this.TenantGuid,
				CommonUtil.ToEventLogString(e)
			});
			this.CompleteRequest(new SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs(string.Empty, status));
		}

		private void CollectAndLogStatisticsInformation(SpeechLoggerProcessType processType, int audioLength)
		{
			if (this.parameters != null)
			{
				SpeechProcessorStatisticsLogger.SpeechProcessorStatisticsLogRow row = this.CollectStatisticsLog(processType, audioLength);
				SpeechRecognitionHandler.SpeechProcessorStatisticsLogger.Append(row);
			}
		}

		private SpeechProcessorStatisticsLogger.SpeechProcessorStatisticsLogRow CollectStatisticsLog(SpeechLoggerProcessType processType, int audioLength)
		{
			return new SpeechProcessorStatisticsLogger.SpeechProcessorStatisticsLogRow
			{
				RequestId = this.parameters.RequestId,
				Culture = this.parameters.Culture,
				Tag = this.parameters.Tag,
				TenantGuid = new Guid?(this.parameters.TenantGuid),
				UserObjectGuid = new Guid?(this.parameters.UserObjectGuid),
				TimeZone = this.parameters.TimeZone.ToString(),
				StartTime = ExDateTime.UtcNow,
				ProcessType = new SpeechLoggerProcessType?(processType),
				AudioLength = audioLength
			};
		}

		private const string TagParameterName = "tag";

		private const string RequestTypeParameterName = "operation";

		private const string CultureParameterName = "culture";

		private const string TimeZoneParameterName = "timezone";

		private const string SpeechRecognitionMaxAudioSize = "SpeechRecognitionMaxAudioSize";

		private const int AudioChunkReadSize = 400;

		private const int DefaultMaxAudioSize = 32500;

		private const int TypicalAudioSize = 16250;

		private readonly int maxAudioSize = 32500;

		private AsyncCallback clientCallback;

		private SpeechRecognitionAsyncResult asyncResult;

		private MemoryStream audioMemoryStream;

		private int speechRequestCompleted;

		private IStandardBudget budget;

		private SpeechRecognitionScenarioBase speechScenario;

		private RequestParameters parameters;

		private UserContext userContext;

		internal delegate void SpeechProcessorAsyncCompletedDelegate(SpeechRecognitionProcessor.SpeechProcessorAsyncCompletedArgs args);

		internal delegate void SpeechProcessorResultsCompletedDelegate(SpeechRecognition helper);

		internal class SpeechProcessorAsyncCompletedArgs
		{
			public SpeechProcessorAsyncCompletedArgs(string responseText, SpeechRecognitionProcessor.SpeechHttpStatus httpStatus)
			{
				this.ResponseText = responseText;
				this.HttpStatus = httpStatus;
			}

			public string ResponseText { get; private set; }

			public SpeechRecognitionProcessor.SpeechHttpStatus HttpStatus { get; private set; }
		}

		internal class SpeechHttpStatus
		{
			private SpeechHttpStatus(int statusCode, string statusDescription)
			{
				this.StatusCode = statusCode;
				this.StatusDescription = statusDescription;
			}

			public int StatusCode { get; private set; }

			public string StatusDescription { get; private set; }

			public static readonly SpeechRecognitionProcessor.SpeechHttpStatus Success = new SpeechRecognitionProcessor.SpeechHttpStatus(200, null);

			public static readonly SpeechRecognitionProcessor.SpeechHttpStatus NoSpeechDetected = new SpeechRecognitionProcessor.SpeechHttpStatus(451, "No Speech Detected");

			public static readonly SpeechRecognitionProcessor.SpeechHttpStatus MaxRequestsExceeded = new SpeechRecognitionProcessor.SpeechHttpStatus(452, "Maximum Requests Exceeded");

			public static readonly SpeechRecognitionProcessor.SpeechHttpStatus NoContactWithEmailAddress = new SpeechRecognitionProcessor.SpeechHttpStatus(453, "No User With Email Address");

			public static readonly SpeechRecognitionProcessor.SpeechHttpStatus BadRequest = new SpeechRecognitionProcessor.SpeechHttpStatus(400, null);

			public static readonly SpeechRecognitionProcessor.SpeechHttpStatus ServiceUnavailable = new SpeechRecognitionProcessor.SpeechHttpStatus(503, null);

			public static readonly SpeechRecognitionProcessor.SpeechHttpStatus InternalServerError = new SpeechRecognitionProcessor.SpeechHttpStatus(500, null);
		}

		private class SpeechStreamBuffer : DisposableBase
		{
			public byte[] AudioBuffer { get; set; }

			public Stream AudioStream { get; set; }

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<SpeechRecognitionProcessor.SpeechStreamBuffer>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing && this.AudioStream != null)
				{
					this.AudioStream.Close();
					this.AudioStream = null;
				}
			}
		}
	}
}
