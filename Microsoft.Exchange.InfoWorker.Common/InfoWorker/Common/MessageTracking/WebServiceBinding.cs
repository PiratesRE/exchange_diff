using System;
using System.Net;
using System.Net.Sockets;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class WebServiceBinding : IWebServiceBinding
	{
		public string TargetInfoForDisplay
		{
			get
			{
				return this.clientProxy.TargetInfoForDisplay;
			}
		}

		public WebServiceBinding(IClientProxy clientProxy, DirectoryContext directoryContext, WebServiceTrackingAuthority trackingAuthority)
		{
			this.clientProxy = clientProxy;
			this.directoryContext = directoryContext;
			this.diagnosticsContext = directoryContext.DiagnosticsContext;
			this.errors = directoryContext.Errors;
			this.trackingAuthority = trackingAuthority;
		}

		public InternalGetMessageTrackingReportResponse GetMessageTrackingReport(string messageTrackingReportId, ReportTemplate reportTemplate, SmtpAddress[] recipientFilter, SearchScope scope, bool returnQueueEvents, TrackingEventBudget eventBudget)
		{
			GetMessageTrackingReportRequestType getMessageTrackingReportRequestType = new GetMessageTrackingReportRequestType();
			getMessageTrackingReportRequestType.MessageTrackingReportId = messageTrackingReportId;
			if (reportTemplate == ReportTemplate.Summary)
			{
				getMessageTrackingReportRequestType.ReportTemplate = MessageTrackingReportTemplateType.Summary;
			}
			else
			{
				if (reportTemplate != ReportTemplate.RecipientPath)
				{
					throw new ArgumentException("ReportTemplate must be RecipientPath or Summary", "reportTemplate");
				}
				getMessageTrackingReportRequestType.ReportTemplate = MessageTrackingReportTemplateType.RecipientPath;
			}
			if (recipientFilter != null && recipientFilter.Length > 0)
			{
				getMessageTrackingReportRequestType.RecipientFilter = new EmailAddressType();
				getMessageTrackingReportRequestType.RecipientFilter.EmailAddress = recipientFilter[0].ToString();
			}
			getMessageTrackingReportRequestType.ReturnQueueEvents = returnQueueEvents;
			getMessageTrackingReportRequestType.ReturnQueueEventsSpecified = true;
			if (this.diagnosticsContext.Enabled)
			{
				getMessageTrackingReportRequestType.DiagnosticsLevel = Names<DiagnosticsLevel>.Map[(int)this.diagnosticsContext.DiagnosticsLevel];
			}
			TimeSpan clientTimeout;
			TimeSpan value;
			this.directoryContext.TrackingBudget.GetTimeBudgetRemainingForWSCall(this.trackingAuthority.TrackingAuthorityKind, out clientTimeout, out value);
			TrackingExtendedProperties trackingExtendedProperties = new TrackingExtendedProperties(false, false, new TimeSpan?(value), reportTemplate == ReportTemplate.Summary, false);
			getMessageTrackingReportRequestType.Properties = trackingExtendedProperties.ToTrackingPropertyArray();
			getMessageTrackingReportRequestType.Scope = WebServiceBinding.GetWebServiceScope(scope);
			Exception ex = null;
			InternalGetMessageTrackingReportResponse internalGetMessageTrackingReportResponse = null;
			this.WriteStartEvent(false, messageTrackingReportId, null);
			internalGetMessageTrackingReportResponse = this.TryCallWebServiceMethod<InternalGetMessageTrackingReportResponse, GetMessageTrackingReportRequestType>(delegate(GetMessageTrackingReportRequestType req)
			{
				if (clientTimeout == TimeSpan.Zero)
				{
					throw new TimeoutExpiredException("Not enough time remaining");
				}
				return this.clientProxy.GetMessageTrackingReport(new GetMessageTrackingReportRequestTypeWrapper(req), clientTimeout);
			}, getMessageTrackingReportRequestType, out ex);
			if (internalGetMessageTrackingReportResponse != null)
			{
				if (internalGetMessageTrackingReportResponse.Response.ResponseClass != ResponseClassType.Success)
				{
					TrackingError trackingErrorFromWebResponseError = this.GetTrackingErrorFromWebResponseError(internalGetMessageTrackingReportResponse.Response.ResponseCode, this.trackingAuthority.Domain, internalGetMessageTrackingReportResponse.Response.MessageText);
					this.errors.Errors.Add(trackingErrorFromWebResponseError);
				}
				this.errors.ReadErrorsFromWSMessage(internalGetMessageTrackingReportResponse.Response.Diagnostics, internalGetMessageTrackingReportResponse.Response.Errors);
				this.diagnosticsContext.MergeEvents(internalGetMessageTrackingReportResponse.Response.Diagnostics);
			}
			int num = 0;
			if (internalGetMessageTrackingReportResponse != null && internalGetMessageTrackingReportResponse.Response.MessageTrackingReport != null && internalGetMessageTrackingReportResponse.RecipientTrackingEvents != null)
			{
				num = internalGetMessageTrackingReportResponse.RecipientTrackingEvents.Count;
				if (this.diagnosticsContext.VerboseDiagnostics)
				{
					foreach (RecipientTrackingEvent recipientTrackingEvent in internalGetMessageTrackingReportResponse.RecipientTrackingEvents)
					{
						SmtpAddress recipientAddress = recipientTrackingEvent.RecipientAddress;
						string text = recipientTrackingEvent.RecipientAddress.ToString();
						string value2 = text;
						if (!string.IsNullOrEmpty(recipientTrackingEvent.UniquePathId))
						{
							value2 = string.Format("[{0}]{1}", recipientTrackingEvent.UniquePathId, text);
						}
						this.diagnosticsContext.AddProperty(DiagnosticProperty.Data1, value2);
						this.diagnosticsContext.WriteEvent();
					}
				}
			}
			eventBudget.IncrementBy((uint)num);
			this.WriteEndEvent(ex, num);
			if (ex != null)
			{
				throw ex;
			}
			return internalGetMessageTrackingReportResponse;
		}

		public FindMessageTrackingReportResponseMessageType FindMessageTrackingReport(string domain, SmtpAddress? senderAddress, SmtpAddress? recipientAddress, string serverHint, SmtpAddress? federatedDeliveryMailbox, SearchScope scope, string messageId, string subject, bool expandTree, bool searchAsRecip, bool searchForModerationResult, DateTime start, DateTime end, TrackingEventBudget eventBudget)
		{
			FindMessageTrackingReportRequestType findMessageTrackingReportRequestType = new FindMessageTrackingReportRequestType();
			findMessageTrackingReportRequestType.StartDateTime = start;
			findMessageTrackingReportRequestType.StartDateTimeSpecified = true;
			findMessageTrackingReportRequestType.EndDateTime = end;
			findMessageTrackingReportRequestType.EndDateTimeSpecified = true;
			findMessageTrackingReportRequestType.MessageId = messageId;
			findMessageTrackingReportRequestType.Subject = subject;
			findMessageTrackingReportRequestType.Domain = domain;
			findMessageTrackingReportRequestType.Scope = WebServiceBinding.GetWebServiceScope(scope);
			findMessageTrackingReportRequestType.ServerHint = serverHint;
			TimeSpan clientTimeout;
			TimeSpan value;
			this.directoryContext.TrackingBudget.GetTimeBudgetRemainingForWSCall(this.trackingAuthority.TrackingAuthorityKind, out clientTimeout, out value);
			TrackingExtendedProperties trackingExtendedProperties = new TrackingExtendedProperties(expandTree, searchAsRecip, new TimeSpan?(value), false, searchForModerationResult);
			findMessageTrackingReportRequestType.Properties = trackingExtendedProperties.ToTrackingPropertyArray();
			if (this.diagnosticsContext.Enabled)
			{
				findMessageTrackingReportRequestType.DiagnosticsLevel = Names<DiagnosticsLevel>.Map[(int)this.diagnosticsContext.DiagnosticsLevel];
			}
			if (federatedDeliveryMailbox != null)
			{
				findMessageTrackingReportRequestType.FederatedDeliveryMailbox = new EmailAddressType();
				findMessageTrackingReportRequestType.FederatedDeliveryMailbox.EmailAddress = federatedDeliveryMailbox.Value.ToString();
			}
			if (senderAddress != null)
			{
				findMessageTrackingReportRequestType.Sender = new EmailAddressType();
				findMessageTrackingReportRequestType.Sender.EmailAddress = senderAddress.Value.ToString();
			}
			if (recipientAddress != null)
			{
				findMessageTrackingReportRequestType.Recipient = new EmailAddressType();
				findMessageTrackingReportRequestType.Recipient.EmailAddress = recipientAddress.Value.ToString();
			}
			Exception ex = null;
			this.WriteStartEvent(true, messageId, serverHint);
			FindMessageTrackingReportResponseMessageType findMessageTrackingReportResponseMessageType = this.TryCallWebServiceMethod<FindMessageTrackingReportResponseMessageType, FindMessageTrackingReportRequestType>(delegate(FindMessageTrackingReportRequestType req)
			{
				if (clientTimeout == TimeSpan.Zero)
				{
					throw new TimeoutExpiredException("Not enough time remaining");
				}
				return this.clientProxy.FindMessageTrackingReport(new FindMessageTrackingReportRequestTypeWrapper(req), clientTimeout);
			}, findMessageTrackingReportRequestType, out ex);
			int count = 0;
			if (findMessageTrackingReportResponseMessageType != null)
			{
				if (findMessageTrackingReportResponseMessageType.ResponseClass != ResponseClassType.Success)
				{
					TrackingError trackingErrorFromWebResponseError = this.GetTrackingErrorFromWebResponseError(findMessageTrackingReportResponseMessageType.ResponseCode, this.trackingAuthority.Domain, findMessageTrackingReportResponseMessageType.MessageText);
					this.errors.Errors.Add(trackingErrorFromWebResponseError);
				}
				this.errors.ReadErrorsFromWSMessage(findMessageTrackingReportResponseMessageType.Diagnostics, findMessageTrackingReportResponseMessageType.Errors);
				this.diagnosticsContext.MergeEvents(findMessageTrackingReportResponseMessageType.Diagnostics);
				if (findMessageTrackingReportResponseMessageType.MessageTrackingSearchResults != null)
				{
					count = findMessageTrackingReportResponseMessageType.MessageTrackingSearchResults.Length;
					eventBudget.IncrementBy(10U);
					if (this.diagnosticsContext.VerboseDiagnostics)
					{
						foreach (FindMessageTrackingSearchResultType findMessageTrackingSearchResultType in findMessageTrackingReportResponseMessageType.MessageTrackingSearchResults)
						{
							this.diagnosticsContext.AddProperty(DiagnosticProperty.Data1, findMessageTrackingSearchResultType.MessageTrackingReportId);
							this.diagnosticsContext.WriteEvent();
						}
					}
				}
			}
			this.WriteEndEvent(ex, count);
			if (ex != null)
			{
				throw ex;
			}
			return findMessageTrackingReportResponseMessageType;
		}

		internal static bool IsPageNotFoundWebException(WebException e)
		{
			HttpWebResponse httpWebResponse = e.Response as HttpWebResponse;
			return httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.NotFound;
		}

		private static string GetWebServiceScope(SearchScope scope)
		{
			if (scope != SearchScope.Organization && scope != SearchScope.Site && scope != SearchScope.Forest)
			{
				throw new ArgumentException("Web service search scope can only be Organization, Forest or Site", "scope");
			}
			return Names<SearchScope>.Map[(int)scope];
		}

		private ResponseType TryCallWebServiceMethod<ResponseType, RequestType>(WebServiceBinding.WebMethodCallDelegate<ResponseType, RequestType> methodCall, RequestType request, out Exception exception)
		{
			exception = null;
			Exception ex = null;
			Exception ex2 = null;
			TrackingError trackingError = null;
			bool flag = false;
			try
			{
				this.directoryContext.Yield();
				flag = true;
				return methodCall(request);
			}
			catch (InvalidOperationException ex3)
			{
				WebException ex4 = ex3 as WebException;
				if (ex4 != null)
				{
					trackingError = this.GetTrackingErrorForWebException(ex4, false);
					ex = ex3;
				}
				else
				{
					trackingError = new TrackingError(ErrorCode.UnexpectedErrorTransient, this.TargetInfoForDisplay, string.Format("WS call to {0} failed with InvalidOperationException", this.TargetInfoForDisplay), ex3.ToString());
					ex2 = ex3;
				}
			}
			catch (SoapException ex5)
			{
				trackingError = new TrackingError(ErrorCode.UnexpectedErrorPermanent, this.TargetInfoForDisplay, string.Format("WS call to {0} failed with SoapException", this.TargetInfoForDisplay), ex5.ToString());
				ex2 = ex5;
			}
			catch (SocketException ex6)
			{
				trackingError = new TrackingError(ErrorCode.Connectivity, this.TargetInfoForDisplay, string.Empty, ex6.ToString());
				ex = ex6;
			}
			catch (InvalidParameterException ex7)
			{
				trackingError = new TrackingError(ErrorCode.UnexpectedErrorPermanent, this.TargetInfoForDisplay, string.Format("WS call to {0} failed with InvalidParameterException", this.TargetInfoForDisplay), ex7.ToString());
				ex2 = ex7;
			}
			catch (SoapFaultException ex8)
			{
				trackingError = new TrackingError(ErrorCode.UnexpectedErrorPermanent, this.TargetInfoForDisplay, string.Format("WS call to {0} failed with SoapFaultException", this.TargetInfoForDisplay), ex8.ToString());
				ex2 = ex8;
			}
			catch (TimeoutExpiredException ex9)
			{
				trackingError = new TrackingError(ErrorCode.TimeBudgetExceeded, this.TargetInfoForDisplay, string.Empty, ex9.ToString());
				ex = ex9;
			}
			catch (ClientDisconnectedException ex10)
			{
				trackingError = new TrackingError(ErrorCode.Connectivity, this.TargetInfoForDisplay, string.Empty, ex10.ToString());
				ex = ex10;
			}
			catch (AvailabilityException ex11)
			{
				trackingError = this.GetTrackingErrorForAvailabilityException(ex11);
				ex = ex11;
			}
			catch (AuthzException ex12)
			{
				trackingError = new TrackingError(ErrorCode.CrossForestAuthentication, this.TargetInfoForDisplay, string.Empty, ex12.ToString());
				ex = ex12;
			}
			catch (OverBudgetException ex13)
			{
				trackingError = new TrackingError(ErrorCode.BudgetExceeded, this.TargetInfoForDisplay, string.Empty, ex13.ToString());
				ex = ex13;
			}
			catch (InvalidFederatedOrganizationIdException ex14)
			{
				trackingError = new TrackingError(ErrorCode.CrossPremiseAuthentication, this.TargetInfoForDisplay, string.Empty, ex14.ToString());
				ex2 = ex14;
			}
			catch (UserWithoutFederatedProxyAddressException ex15)
			{
				trackingError = new TrackingError(ErrorCode.CrossPremiseMisconfiguration, this.TargetInfoForDisplay, string.Empty, ex15.ToString());
				ex2 = ex15;
			}
			finally
			{
				if (flag)
				{
					this.directoryContext.Acquire();
				}
			}
			this.errors.Errors.Add(trackingError);
			if (ex != null)
			{
				exception = new TrackingTransientException(trackingError, ex, true);
			}
			else
			{
				exception = new TrackingTransientException(trackingError, ex2, true);
			}
			TraceWrapper.SearchLibraryTracer.TraceError<Exception>(this.GetHashCode(), "Exception in web-service call: {0}", (ex == null) ? ex2 : ex);
			return default(ResponseType);
		}

		private void WriteStartEvent(bool findCall, string messageId, string serverHint)
		{
			this.operation = this.GetOperation(findCall);
			this.diagnosticsContext.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[(int)this.operation]);
			this.diagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[0]);
			this.diagnosticsContext.AddProperty(DiagnosticProperty.Svr, this.clientProxy.TargetInfoForLogging);
			this.diagnosticsContext.AddProperty(DiagnosticProperty.Mid, messageId);
			if (!string.IsNullOrEmpty(serverHint))
			{
				this.diagnosticsContext.AddProperty(DiagnosticProperty.Data1, serverHint);
			}
			this.diagnosticsContext.WriteEvent();
		}

		private void WriteEndEvent(Exception exception, int count)
		{
			this.diagnosticsContext.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[(int)this.operation]);
			this.diagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[1]);
			this.diagnosticsContext.AddProperty(DiagnosticProperty.Cnt, count);
			if (exception != null)
			{
				this.diagnosticsContext.AddProperty(DiagnosticProperty.Err, exception.InnerException.GetType().Name);
				string value = Parse.RemoveControlChars(exception.InnerException.Message);
				if (!string.IsNullOrEmpty(value))
				{
					this.diagnosticsContext.AddProperty(DiagnosticProperty.Data1, value);
				}
			}
			this.diagnosticsContext.WriteEvent();
		}

		private Operations GetOperation(bool findCall)
		{
			if (this.trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteTrustedOrg)
			{
				if (findCall)
				{
					return Operations.XPremiseFind;
				}
				return Operations.XPremiseGet;
			}
			else if (this.trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteSiteInCurrentOrg)
			{
				if (findCall)
				{
					return Operations.XSiteFind;
				}
				return Operations.XSiteGet;
			}
			else
			{
				if (this.trackingAuthority.TrackingAuthorityKind != TrackingAuthorityKind.RemoteForest)
				{
					throw new InvalidOperationException("Tracking-Authority needs to be enabled for diagnostics");
				}
				if (findCall)
				{
					return Operations.XForestFind;
				}
				return Operations.XForestGet;
			}
		}

		private TrackingError GetTrackingErrorFromWebResponseError(ResponseCodeType responseCode, string target, string messageText)
		{
			if (responseCode == ResponseCodeType.NoError)
			{
				throw new ArgumentException("ResponseCodeType.NoError cannot be mapped because it is not an error.");
			}
			TraceWrapper.SearchLibraryTracer.TraceDebug<string, string>(this.GetHashCode(), "Mapping error code {0} from web response.  MessageText: {1}", Names<ResponseCodeType>.Map[(int)responseCode], messageText);
			if (responseCode != ResponseCodeType.ErrorAccessDenied)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<string, string>(this.GetHashCode(), "Mapping error code {0} from web response to generic failure. MessageText: {1}", Names<ResponseCodeType>.Map[(int)responseCode], messageText);
				return new TrackingError(ErrorCode.UnexpectedErrorTransient, string.Empty, string.Format("WebException connecting to {0}", target), messageText);
			}
			if (this.trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteTrustedOrg)
			{
				return new TrackingError(ErrorCode.CrossPremiseMisconfiguration, target, string.Empty, messageText);
			}
			if (this.trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteForest)
			{
				return new TrackingError(ErrorCode.CrossForestMisconfiguration, target, string.Empty, messageText);
			}
			return new TrackingError(ErrorCode.UnexpectedErrorTransient, target, string.Empty, messageText);
		}

		private TrackingError GetTrackingErrorForAvailabilityException(AvailabilityException ex)
		{
			if (ex.InnerException != null)
			{
				WebException ex2 = ex.InnerException as WebException;
				if (ex2 != null)
				{
					return this.GetTrackingErrorForWebException(ex2, true);
				}
				if (ex.InnerException is SoapException)
				{
					return new TrackingError(ErrorCode.UnexpectedErrorTransient, this.TargetInfoForDisplay, string.Format("SoapException when connecting to {0}", this.TargetInfoForDisplay), ex.ToString());
				}
			}
			if (ex.ErrorCode == 5042 || ex.ErrorCode == 5027)
			{
				return new TrackingError(ErrorCode.UnexpectedErrorTransient, this.TargetInfoForDisplay, string.Format("Client disconnected by {0}", this.TargetInfoForDisplay), ex.ToString());
			}
			if (ex.ErrorCode == 5025)
			{
				return new TrackingError(ErrorCode.CrossForestAuthentication, this.TargetInfoForDisplay, string.Empty, ex.ToString());
			}
			if (this.trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteForest)
			{
				return new TrackingError(ErrorCode.CrossForestMisconfiguration, this.TargetInfoForDisplay, string.Empty, ex.ToString());
			}
			if (this.trackingAuthority.TrackingAuthorityKind == TrackingAuthorityKind.RemoteTrustedOrg)
			{
				return new TrackingError(ErrorCode.CrossPremiseMisconfiguration, this.TargetInfoForDisplay, string.Empty, ex.ToString());
			}
			return new TrackingError(ErrorCode.UnexpectedErrorTransient, this.TargetInfoForDisplay, string.Format("AvailabilityErrorCode: {0} connecting to {1}", ex.ErrorCode, this.TargetInfoForDisplay), ex.ToString());
		}

		private TrackingError GetTrackingErrorForWebException(WebException ex, bool isAvailabilityException)
		{
			if (isAvailabilityException && WebServiceBinding.IsPageNotFoundWebException(ex))
			{
				return new TrackingError(ErrorCode.LegacySender, this.TargetInfoForDisplay, string.Empty, ex.ToString());
			}
			if (ex.Status == WebExceptionStatus.Timeout)
			{
				return new TrackingError(ErrorCode.TimeBudgetExceeded, this.TargetInfoForDisplay, string.Empty, ex.ToString());
			}
			if (ex.Status == WebExceptionStatus.ConnectFailure || ex.Status == WebExceptionStatus.ConnectionClosed)
			{
				return new TrackingError(ErrorCode.Connectivity, this.TargetInfoForDisplay, string.Empty, ex.ToString());
			}
			if (!isAvailabilityException)
			{
				return new TrackingError(ErrorCode.Connectivity, this.TargetInfoForDisplay, string.Empty, ex.ToString());
			}
			return new TrackingError(ErrorCode.UnexpectedErrorTransient, this.TargetInfoForDisplay, string.Format("WebException when connecting to {0}", this.TargetInfoForDisplay), ex.ToString());
		}

		private IClientProxy clientProxy;

		protected DirectoryContext directoryContext;

		protected DiagnosticsContext diagnosticsContext;

		protected WebServiceTrackingAuthority trackingAuthority;

		protected TrackingErrorCollection errors;

		private Operations operation;

		internal delegate ResponseType WebMethodCallDelegate<ResponseType, RequestType>(RequestType requestMessage);
	}
}
