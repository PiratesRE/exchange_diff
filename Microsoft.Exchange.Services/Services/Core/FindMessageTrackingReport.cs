using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class FindMessageTrackingReport : BaseMessageTrackingReport<FindMessageTrackingReportRequest, FindMessageTrackingSearchResultType[]>
	{
		protected override string Domain
		{
			get
			{
				return base.Request.Domain;
			}
		}

		protected override string DiagnosticsLevel
		{
			get
			{
				return base.Request.DiagnosticsLevel;
			}
		}

		public FindMessageTrackingReport(CallContext callContext, FindMessageTrackingReportRequest request) : base(callContext, request)
		{
		}

		protected override void PrepareRequest()
		{
			TrackingConverter.Convert(base.Request, ExchangeVersion.Current, out this.extendedProperties);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			FindMessageTrackingReportResponseMessage findMessageTrackingReportResponseMessage = new FindMessageTrackingReportResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value, base.GetDiagnosticsToTransmit(), this.forestSearchExecuted ? Names<SearchScope>.Map[1] : Names<SearchScope>.Map[0], null, null);
			TrackingConverter.Convert(findMessageTrackingReportResponseMessage, this.errors, ExchangeVersion.Current);
			return findMessageTrackingReportResponseMessage;
		}

		internal override ServiceResult<FindMessageTrackingSearchResultType[]> Execute()
		{
			IEnumerable<MessageTrackingSearchResult> internalResults = null;
			SearchMessageTrackingReportImpl searchMessageTrackingReportImpl = null;
			bool flag = false;
			bool flag2 = false;
			try
			{
				this.ValidateRequest();
				base.InitializeRequest();
				TrackingEventBudget.AcquireThread();
				flag2 = true;
				TrackedUser trackedUser = null;
				if (base.Request.Sender != null)
				{
					trackedUser = TrackedUser.Create(base.Request.Sender.EmailAddress, this.directoryContext.TenantGalSession);
					if (trackedUser == null)
					{
						ExTraceGlobals.WebServiceTracer.TraceError<string>((long)this.GetHashCode(), "Sender {0} found in AD, but the ADRecipient object was invalid. Rejecting EWS request", base.Request.Sender.EmailAddress);
						throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
					}
				}
				TrackedUser[] recipients = null;
				if (base.Request.Recipient != null)
				{
					TrackedUser trackedUser2 = TrackedUser.Create(base.Request.Recipient.EmailAddress, this.directoryContext.TenantGalSession);
					if (trackedUser2 == null)
					{
						ExTraceGlobals.WebServiceTracer.TraceError<string>((long)this.GetHashCode(), "Recipient {0} found in AD, but the ADRecipient object was invalid. Rejecting EWS request", base.Request.Recipient.EmailAddress);
						throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
					}
					recipients = new TrackedUser[]
					{
						trackedUser2
					};
				}
				SearchScope searchScope;
				EnumValidator<SearchScope>.TryParse(base.Request.Scope, EnumParseOptions.Default, out searchScope);
				if (searchScope != SearchScope.Organization && searchScope != SearchScope.Forest && searchScope != SearchScope.Site)
				{
					throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
				}
				SmtpAddress? smtpAddress = null;
				if (base.Request.FederatedDeliveryMailbox != null && !string.IsNullOrEmpty(base.Request.FederatedDeliveryMailbox.EmailAddress))
				{
					smtpAddress = new SmtpAddress?(new SmtpAddress(base.Request.FederatedDeliveryMailbox.EmailAddress));
					if (!smtpAddress.Value.IsValidAddress)
					{
						throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
					}
				}
				if (this.extendedProperties.SearchForModerationResult && (trackedUser == null || !trackedUser.IsArbitrationMailbox || string.IsNullOrEmpty(base.Request.MessageId)))
				{
					ExTraceGlobals.WebServiceTracer.TraceError<TrackedUser, string>((long)this.GetHashCode(), "Sender must be an arbitration mailbox and message id is present to search for moderation result. sender={0}, messageId={1}", trackedUser, base.Request.MessageId);
					throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
				}
				Server localServer = ServerCache.Instance.GetLocalServer();
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[8]);
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[0]);
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Svr, localServer.Fqdn);
				this.directoryContext.DiagnosticsContext.WriteEvent();
				searchMessageTrackingReportImpl = new SearchMessageTrackingReportImpl(this.directoryContext, searchScope, trackedUser, null, base.Request.ServerHint, recipients, null, base.Request.Subject, base.Request.MessageId, Unlimited<uint>.UnlimitedValue, this.extendedProperties.ExpandTree, this.extendedProperties.SearchAsRecip, !(ExchangeVersion.Current == ExchangeVersion.Exchange2010), this.extendedProperties.SearchForModerationResult);
				internalResults = searchMessageTrackingReportImpl.Execute();
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[8]);
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[1]);
				this.directoryContext.DiagnosticsContext.WriteEvent();
			}
			catch (TransientException ex)
			{
				flag = true;
				base.AddError(new TrackingError(ErrorCode.UnexpectedErrorTransient, string.Empty, ex.Message, ex.ToString()));
				ExTraceGlobals.WebServiceTracer.TraceError<TransientException>((long)this.GetHashCode(), "TransientException during processing FindMessageTrackingReport: {0}", ex);
			}
			catch (DataSourceOperationException ex2)
			{
				flag = true;
				base.AddError(new TrackingError(ErrorCode.UnexpectedErrorPermanent, string.Empty, "Error from Active Directory provider", ex2.ToString()));
				ExTraceGlobals.WebServiceTracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "DataSourceOperationException during processing FindMessageTrackingReport: {0}", ex2);
			}
			catch (DataValidationException ex3)
			{
				flag = true;
				base.AddError(new TrackingError(ErrorCode.UnexpectedErrorPermanent, string.Empty, "Validation Error from Active Directory provider", ex3.ToString()));
				ExTraceGlobals.WebServiceTracer.TraceError<DataValidationException>((long)this.GetHashCode(), "DataValidationException during processing FindMessageTrackingReport: {0}", ex3);
			}
			catch (TrackingFatalException ex4)
			{
				if (!ex4.IsAlreadyLogged)
				{
					base.AddError(ex4.TrackingError);
				}
				flag = true;
				ExTraceGlobals.WebServiceTracer.TraceError<TrackingFatalException>((long)this.GetHashCode(), "TrackingFatalException during processing FindMessageTrackingReport: {0}", ex4);
			}
			catch (TrackingTransientException ex5)
			{
				if (!ex5.IsAlreadyLogged)
				{
					base.AddError(ex5.TrackingError);
				}
				flag = true;
				ExTraceGlobals.WebServiceTracer.TraceError<TrackingTransientException>((long)this.GetHashCode(), "TrackingTransientException during processing FindMessageTrackingReport: {0}", ex5);
			}
			finally
			{
				base.CleanupRequest();
				if (flag || (!TrackingErrorCollection.IsNullOrEmpty(this.errors) && this.errors.Errors.Count > 0))
				{
					base.LogRequestStatus(false);
					PerfCounterData.ResultCounter.AddFailure();
				}
				else
				{
					base.LogRequestStatus(true);
					PerfCounterData.ResultCounter.AddSuccess();
				}
				InfoWorkerMessageTrackingPerformanceCounters.MessageTrackingFailureRateWebService.RawValue = (long)PerfCounterData.ResultCounter.FailurePercentage;
				if (flag2)
				{
					TrackingEventBudget.ReleaseThread();
				}
			}
			if (searchMessageTrackingReportImpl != null)
			{
				this.errors = searchMessageTrackingReportImpl.Errors;
				this.forestSearchExecuted = searchMessageTrackingReportImpl.WholeForestSearchExecuted;
			}
			return this.ConvertToWSResults(internalResults);
		}

		private ServiceResult<FindMessageTrackingSearchResultType[]> ConvertToWSResults(IEnumerable<MessageTrackingSearchResult> internalResults)
		{
			List<FindMessageTrackingSearchResultType> list = new List<FindMessageTrackingSearchResultType>();
			if (internalResults != null)
			{
				foreach (MessageTrackingSearchResult messageTrackingSearchResult in internalResults)
				{
					EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
					emailAddressWrapper.EmailAddress = messageTrackingSearchResult.FromAddress.ToString();
					emailAddressWrapper.Name = messageTrackingSearchResult.FromDisplayName;
					EmailAddressWrapper[] array = new EmailAddressWrapper[messageTrackingSearchResult.RecipientAddresses.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = new EmailAddressWrapper();
						array[i].EmailAddress = messageTrackingSearchResult.RecipientAddresses[i].ToString();
					}
					FindMessageTrackingSearchResultType findMessageTrackingSearchResultType = new FindMessageTrackingSearchResultType(messageTrackingSearchResult.Subject, emailAddressWrapper, array, messageTrackingSearchResult.SubmittedDateTime, messageTrackingSearchResult.MessageTrackingReportId.ToString(), messageTrackingSearchResult.PreviousHopServer, messageTrackingSearchResult.FirstHopServer, null);
					TrackingConverter.Convert(findMessageTrackingSearchResultType, ExchangeVersion.Current);
					list.Add(findMessageTrackingSearchResultType);
				}
			}
			return new ServiceResult<FindMessageTrackingSearchResultType[]>(list.ToArray());
		}

		private void ValidateRequest()
		{
			ExTraceGlobals.WebServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "FindMessageTrackingReport running in domain: {0}", base.Request.Domain);
			if (base.Request.Recipient != null)
			{
				string emailAddress = base.Request.Recipient.EmailAddress;
				if (string.IsNullOrEmpty(emailAddress) || !SmtpAddress.IsValidSmtpAddress(emailAddress))
				{
					ExTraceGlobals.WebServiceTracer.TraceError<string>((long)this.GetHashCode(), "Invalid SMTP address in request (Recipient element): {0}", emailAddress);
					throw new InvalidSmtpAddressException();
				}
				ExTraceGlobals.WebServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "Recipient: {0}", base.Request.Recipient.EmailAddress);
			}
			if (base.Request.Sender != null)
			{
				if (base.Request.Sender.EmailAddress == null || !SmtpAddress.IsValidSmtpAddress(base.Request.Sender.EmailAddress))
				{
					ExTraceGlobals.WebServiceTracer.TraceError<string>((long)this.GetHashCode(), "Invalid SMTP address in request (Sender element): {0}", base.Request.Sender.EmailAddress);
					throw new InvalidSmtpAddressException();
				}
				ExTraceGlobals.WebServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "Sender: {0}", base.Request.Sender.EmailAddress);
			}
			else if (base.Request.MessageId == null)
			{
				ExTraceGlobals.WebServiceTracer.TraceError((long)this.GetHashCode(), "Sender and MessageId elements are both null");
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			ExTraceGlobals.WebServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "Validation succeeded, Message-Id: {0}", base.Request.MessageId);
		}

		private bool forestSearchExecuted;
	}
}
