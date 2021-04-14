using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Availability;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetUserAvailability : AvailabilityCommandBase<Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityRequest, Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse>
	{
		internal override bool SupportsExternalUsers
		{
			get
			{
				return true;
			}
		}

		internal override Offer ExpectedOffer
		{
			get
			{
				return Offer.SharingCalendarFreeBusy;
			}
		}

		internal override TimeSpan? MaxExecutionTime
		{
			get
			{
				return new TimeSpan?(GetUserAvailability.WlmExecutionTime);
			}
		}

		public GetUserAvailability(CallContext callContext, Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityRequest request) : base(callContext, request)
		{
			OwsLogRegistry.Register(GetUserAvailability.GetUserAvailabilityActionName, typeof(AvailabilityServiceMetadata), new Type[0]);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return base.Result.Value;
		}

		internal override ServiceResult<Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse> Execute()
		{
			AvailabilityCommandBase<Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityRequest, Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse>.CalendarViewTracer.TraceDebug((long)this.GetHashCode(), "Entered WebMethod GetUserAvailability()");
			this.freeBusyViewOptions = base.Request.FreeBusyViewOptions;
			this.mailboxDataArray = base.Request.MailboxDataArray;
			this.suggestionsViewOptions = base.Request.SuggestionsViewOptions;
			if (EWSSettings.RequestTimeZone != null && EWSSettings.RequestTimeZone != ExTimeZone.UtcTimeZone)
			{
				this.requestTimeZone = EWSSettings.RequestTimeZone;
			}
			else if (base.Request.TimeZone != null)
			{
				try
				{
					this.requestTimeZone = base.Request.TimeZone.TimeZone;
				}
				catch (InvalidTimeZoneException ex)
				{
					AvailabilityCommandBase<Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityRequest, Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse>.CalendarViewTracer.TraceError<InvalidTimeZoneException>((long)this.GetHashCode(), "The timezone specified by client is invalid. Exception: {0}", ex);
					throw FaultExceptionUtilities.CreateAvailabilityFault(new InvalidParameterException(CoreResources.descInvalidTimeZone, ex), FaultParty.Sender);
				}
			}
			this.defaultFreeBusyAccessOnly = base.Request.DefaultFreeBusyAccessOnly;
			return new ServiceResult<Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse>(this.GetUserAvailabilityFromRequest());
		}

		private Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse GetUserAvailabilityFromRequest()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse result;
			try
			{
				AvailabilityCommandBase<Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityRequest, Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse>.CheckRequestStreamSize(CallContext.Current.HttpContext.Request);
				FaultInjection.GenerateFault((FaultInjection.LIDs)3309710653U);
				result = this.InternalGetUserAvailability();
			}
			catch (InvalidParameterException exception)
			{
				throw FaultExceptionUtilities.CreateAvailabilityFault(exception, FaultParty.Sender);
			}
			catch (ClientDisconnectedException)
			{
				ExceptionHandler<XmlNode>.HandleClientDisconnect();
				result = null;
			}
			catch (AvailabilityException exception2)
			{
				throw FaultExceptionUtilities.CreateAvailabilityFault(exception2, FaultParty.Sender);
			}
			catch (AuthzException innerException)
			{
				throw FaultExceptionUtilities.CreateAvailabilityFault(new InvalidAuthorizationContextException(innerException), FaultParty.Sender);
			}
			catch (OverBudgetException innerException2)
			{
				throw FaultExceptionUtilities.CreateAvailabilityFault(new ServerBusyException(innerException2), FaultParty.Sender);
			}
			finally
			{
				stopwatch.Stop();
				base.CallContext.ProtocolLog.Set(AvailabilityServiceMetadata.TimeInAs, stopwatch.ElapsedMilliseconds);
				if (RequestDetailsLogger.Current != null && RequestDetailsLogger.Current.ShouldSendDebugResponseHeaders())
				{
					string asLogPrefix = "AS.";
					List<KeyValuePair<string, object>> data = RequestDetailsLogger.Current.ActivityScope.GetFormattableMetadata().FindAll((KeyValuePair<string, object> x) => x.Key.Contains(asLogPrefix));
					string value = LogRowFormatter.FormatCollection(data);
					RequestDetailsLogger.Current.PushDebugInfoToResponseHeaders("ASGenericInfo", value);
				}
				AvailabilityCommandBase<Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityRequest, Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse>.CalendarViewTracer.TraceDebug((long)this.GetHashCode(), "Leaving GetUserAvailability()");
			}
			return result;
		}

		private Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse InternalGetUserAvailability()
		{
			Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse result = null;
			ClientContext clientContext = null;
			ExternalCallContext externalCallContext = null;
			Stopwatch stopwatch = Stopwatch.StartNew();
			AvailabilityCommandBase<Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityRequest, Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse>.CalendarViewTracer.TraceDebug((long)this.GetHashCode(), "Entered InternalGetUserAvailability()");
			string text = base.CallContext.MessageId;
			if (text == null)
			{
				text = AvailabilityQuery.CreateNewMessageId();
			}
			try
			{
				base.CallerBudget.EndLocal();
				externalCallContext = (base.CallContext as ExternalCallContext);
				if (externalCallContext != null)
				{
					clientContext = ClientContext.Create(externalCallContext.EmailAddress, externalCallContext.ExternalId, externalCallContext.WSSecurityHeader, externalCallContext.SharingSecurityHeader, externalCallContext.Budget, this.requestTimeZone, EWSSettings.ClientCulture, text);
					base.CallContext.ProtocolLog.Set(AvailabilityServiceMetadata.ExtC, stopwatch.ElapsedMilliseconds);
				}
				else
				{
					clientContext = ClientContext.Create(base.CallContext.EffectiveCaller.ClientSecurityContext, base.CallContext.ADRecipientSessionContext.OrganizationId, base.CallContext.Budget, this.requestTimeZone, EWSSettings.ClientCulture, text);
					base.CallContext.ProtocolLog.Set(AvailabilityServiceMetadata.IntC, stopwatch.ElapsedMilliseconds);
				}
			}
			finally
			{
				base.CallerBudget.StartLocal("GetUserAvailability.InternalGetUserAvailability", default(TimeSpan));
			}
			clientContext.RequestSchemaVersion = (Microsoft.Exchange.InfoWorker.Common.Availability.Proxy.ExchangeVersionType)ExchangeVersion.Current.Version;
			string text2 = RequestDetailsLogger.Current.ActivityScope.ClientRequestId;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = RequestDetailsLogger.Current.ActivityScope.ActivityId.ToString("N");
			}
			clientContext.RequestId = text2;
			base.CallContext.ProtocolLog.Set(AvailabilityServiceMetadata.PASQ1, stopwatch.ElapsedMilliseconds);
			try
			{
				AvailabilityQuery availabilityQuery = null;
				try
				{
					base.CallerBudget.EndLocal();
					availabilityQuery = new AvailabilityQuery();
				}
				finally
				{
					base.CallerBudget.StartLocal("GetUserAvailability.InternalGetUserAvailability", default(TimeSpan));
				}
				base.CallContext.ProtocolLog.Set(AvailabilityServiceMetadata.PASQ2, stopwatch.ElapsedMilliseconds);
				availabilityQuery.PreExecutionTimeTaken = RequestDetailsLogger.Current.ActivityScope.TotalMilliseconds;
				base.CallContext.ProtocolLog.Set(AvailabilityServiceMetadata.PreexecutionLatency, availabilityQuery.PreExecutionTimeTaken);
				availabilityQuery.ClientContext = clientContext;
				availabilityQuery.DefaultFreeBusyOnly = this.defaultFreeBusyAccessOnly;
				availabilityQuery.MailboxArray = this.mailboxDataArray;
				availabilityQuery.IsCrossForestRequest = (base.CallContext.AvailabilityProxyRequestType != null && base.CallContext.AvailabilityProxyRequestType.Value == ProxyRequestType.CrossForest);
				availabilityQuery.DesiredFreeBusyView = this.freeBusyViewOptions;
				availabilityQuery.DesiredSuggestionsView = this.suggestionsViewOptions;
				availabilityQuery.HttpResponse = CallContext.Current.HttpContext.Response;
				stopwatch.Stop();
				base.CallContext.ProtocolLog.Set(AvailabilityServiceMetadata.PASQT, stopwatch.ElapsedMilliseconds);
				try
				{
					AvailabilityQueryResult queryResult = availabilityQuery.Execute();
					result = Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse.CreateFrom(queryResult);
					if (externalCallContext == null)
					{
						List<ClientStatistics> clientRequestStatistics = base.CallContext.ClientRequestStatistics;
						if (clientRequestStatistics != null)
						{
							foreach (ClientStatistics clientStatistics in clientRequestStatistics)
							{
								ClientStatisticsReporter.ReportRequest(clientStatistics.MessageId, clientStatistics.RequestTime, clientStatistics.ResponseTime, clientStatistics.ResponseSize, clientStatistics.HttpResponseCode, clientStatistics.ErrorCode);
							}
						}
					}
				}
				finally
				{
					if (availabilityQuery.RequestLogger.ErrorData != null)
					{
						foreach (string value in availabilityQuery.RequestLogger.ErrorData)
						{
							RequestDetailsLogger.Current.AppendGenericError("ASError", value);
						}
					}
					RequestDetailsLogger.Current.Set(AvailabilityServiceMetadata.RequestStatistics, availabilityQuery.RequestLogger.LogData.ToString());
				}
			}
			finally
			{
				if (clientContext != null)
				{
					clientContext.Dispose();
				}
			}
			AvailabilityCommandBase<Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityRequest, Microsoft.Exchange.InfoWorker.Availability.GetUserAvailabilityResponse>.CalendarViewTracer.TraceDebug((long)this.GetHashCode(), "Leaving InternalGetUserAvailability()");
			return result;
		}

		private static readonly string GetUserAvailabilityActionName = typeof(GetUserAvailability).Name;

		private static readonly TimeSpan WlmExecutionTime = TimeSpan.FromSeconds(8.0);

		private FreeBusyViewOptions freeBusyViewOptions;

		private MailboxData[] mailboxDataArray;

		private SuggestionsViewOptions suggestionsViewOptions;

		private ExTimeZone requestTimeZone;

		private bool defaultFreeBusyAccessOnly;
	}
}
