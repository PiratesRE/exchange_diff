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
	internal sealed class GetMessageTrackingReport : BaseMessageTrackingReport<GetMessageTrackingReportRequest, MessageTrackingReport>
	{
		protected override string Domain
		{
			get
			{
				return this.domain;
			}
		}

		protected override string DiagnosticsLevel
		{
			get
			{
				return base.Request.DiagnosticsLevel;
			}
		}

		public GetMessageTrackingReport(CallContext callContext, GetMessageTrackingReportRequest request) : base(callContext, request)
		{
		}

		protected override void PrepareRequest()
		{
			TrackingConverter.Convert(base.Request, ExchangeVersion.Current, out this.extendedProperties);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetMessageTrackingReportResponseMessage getMessageTrackingReportResponseMessage = new GetMessageTrackingReportResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value, base.GetDiagnosticsToTransmit(), null, null);
			TrackingConverter.Convert(getMessageTrackingReportResponseMessage, this.errors, ExchangeVersion.Current);
			return getMessageTrackingReportResponseMessage;
		}

		internal override ServiceResult<MessageTrackingReport> Execute()
		{
			GetMessageTrackingReportImpl getMessageTrackingReportImpl = null;
			MessageTrackingReport messageTrackingReport = null;
			bool flag = false;
			bool flag2 = false;
			try
			{
				this.ValidateRequest();
				base.InitializeRequest();
				TrackingEventBudget.AcquireThread();
				flag2 = true;
				SearchScope searchScope;
				EnumValidator<SearchScope>.TryParse(base.Request.Scope, EnumParseOptions.Default, out searchScope);
				if (searchScope != SearchScope.Organization && searchScope != SearchScope.Forest && searchScope != SearchScope.Site)
				{
					throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
				}
				ReportConstraints reportConstraints = new ReportConstraints();
				reportConstraints.BypassDelegateChecking = true;
				reportConstraints.DetailLevel = MessageTrackingDetailLevel.Verbose;
				reportConstraints.DoNotResolve = false;
				reportConstraints.ReturnQueueEvents = base.Request.ReturnQueueEvents;
				if (base.Request.ReportTemplate == MessageTrackingReportTemplate.RecipientPath)
				{
					reportConstraints.ReportTemplate = ReportTemplate.RecipientPath;
				}
				else
				{
					if (base.Request.ReportTemplate != MessageTrackingReportTemplate.Summary)
					{
						throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
					}
					reportConstraints.ReportTemplate = ReportTemplate.Summary;
				}
				reportConstraints.RecipientPathFilter = null;
				if (this.recipientFilter != null)
				{
					reportConstraints.RecipientPathFilter = new SmtpAddress[]
					{
						this.recipientFilter.Value
					};
				}
				reportConstraints.ResultSize = Unlimited<uint>.UnlimitedValue;
				reportConstraints.Sender = SmtpAddress.NullReversePath;
				reportConstraints.Status = null;
				reportConstraints.TrackingAsSender = true;
				Server localServer = ServerCache.Instance.GetLocalServer();
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[9]);
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[0]);
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Svr, localServer.Fqdn);
				this.directoryContext.DiagnosticsContext.WriteEvent();
				LogCache logCache = new LogCache(DateTime.MinValue, DateTime.MaxValue, this.directoryContext.TrackingBudget);
				bool flag3 = false;
				if (this.extendedProperties != null && this.extendedProperties.GetAdditionalRecords && reportConstraints.ReportTemplate == ReportTemplate.Summary && this.reportId.IsSender)
				{
					ExTraceGlobals.WebServiceTracer.TraceDebug<string>((long)this.GetHashCode(), "Getting addional records for message tracking report id: {0}", base.Request.MessageTrackingReportId);
					messageTrackingReport = this.RunGetAdditionalReports(searchScope, logCache, reportConstraints, out flag3);
					if (!flag3)
					{
						ExTraceGlobals.WebServiceTracer.TraceError<string>((long)this.GetHashCode(), "Search did not yield the result asked for by caller when getting additional records. Falling back to normal get for message tracking report id: {0}", base.Request.MessageTrackingReportId);
					}
				}
				if (!flag3)
				{
					getMessageTrackingReportImpl = new GetMessageTrackingReportImpl(this.directoryContext, searchScope, this.reportId, logCache, reportConstraints);
					MessageTrackingReport messageTrackingReport2 = getMessageTrackingReportImpl.Execute();
					if (messageTrackingReport2 != null)
					{
						messageTrackingReport2.AssignReportIdToRecipEvents();
						if (messageTrackingReport != null)
						{
							messageTrackingReport.MergeRecipientEventsFrom(messageTrackingReport2);
						}
						else
						{
							messageTrackingReport = messageTrackingReport2;
						}
					}
				}
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.Op, Names<Operations>.Map[9]);
				this.directoryContext.DiagnosticsContext.AddProperty(DiagnosticProperty.OpType, Names<OpType>.Map[1]);
				this.directoryContext.DiagnosticsContext.WriteEvent();
			}
			catch (TransientException ex)
			{
				base.AddError(new TrackingError(ErrorCode.UnexpectedErrorTransient, string.Empty, string.Format("TransientException raised", new object[0]), ex.ToString()));
				flag = true;
				ExTraceGlobals.WebServiceTracer.TraceError<TransientException>((long)this.GetHashCode(), "TransientException during processing GetMessageTrackingReport: {0}", ex);
			}
			catch (DataSourceOperationException ex2)
			{
				base.AddError(new TrackingError(ErrorCode.UnexpectedErrorPermanent, string.Empty, "Error connecting to Active Directory", ex2.ToString()));
				flag = true;
				ExTraceGlobals.WebServiceTracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "DataSourceOperationException during processing GetMessageTrackingReport: {0}", ex2);
			}
			catch (DataValidationException ex3)
			{
				base.AddError(new TrackingError(ErrorCode.UnexpectedErrorPermanent, string.Empty, "Error validating data in Active Directory", ex3.ToString()));
				flag = true;
				ExTraceGlobals.WebServiceTracer.TraceError<DataValidationException>((long)this.GetHashCode(), "DataValidationException during processing GetMessageTrackingReport: {0}", ex3);
			}
			catch (TrackingFatalException ex4)
			{
				if (!ex4.IsAlreadyLogged)
				{
					base.AddError(ex4.TrackingError);
				}
				flag = true;
				ExTraceGlobals.WebServiceTracer.TraceError<TrackingFatalException>((long)this.GetHashCode(), "TrackingFatalException during processing GetMessageTrackingReport: {0}", ex4);
			}
			catch (TrackingTransientException ex5)
			{
				if (!ex5.IsAlreadyLogged)
				{
					base.AddError(ex5.TrackingError);
				}
				flag = true;
				ExTraceGlobals.WebServiceTracer.TraceError<TrackingTransientException>((long)this.GetHashCode(), "TrackingTransientException during processing GetMessageTrackingReport: {0}", ex5);
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
			if (getMessageTrackingReportImpl != null)
			{
				this.errors = getMessageTrackingReportImpl.Errors;
			}
			MessageTrackingReport messageTrackingReport3 = new MessageTrackingReport();
			RecipientTrackingEvent[] recipientTrackingEvents = GetMessageTrackingReport.emptyRecipientEvents;
			if (messageTrackingReport != null)
			{
				if (messageTrackingReport.FromAddress != null && messageTrackingReport.FromAddress.Value.Length > 0)
				{
					messageTrackingReport3.Sender = new EmailAddressWrapper();
					messageTrackingReport3.Sender.EmailAddress = messageTrackingReport.FromAddress.Value.ToString();
					messageTrackingReport3.Sender.Name = messageTrackingReport.FromDisplayName;
				}
				messageTrackingReport3.Subject = messageTrackingReport.Subject;
				messageTrackingReport3.SubmitTime = messageTrackingReport.SubmittedDateTime;
				if (messageTrackingReport.RecipientAddresses != null && messageTrackingReport.RecipientAddresses.Length > 0)
				{
					messageTrackingReport3.OriginalRecipients = new EmailAddressWrapper[messageTrackingReport.RecipientAddresses.Length];
					for (int i = 0; i < messageTrackingReport3.OriginalRecipients.Length; i++)
					{
						messageTrackingReport3.OriginalRecipients[i] = new EmailAddressWrapper();
						messageTrackingReport3.OriginalRecipients[i].EmailAddress = messageTrackingReport.RecipientAddresses[i].ToString();
						messageTrackingReport3.OriginalRecipients[i].Name = messageTrackingReport.RecipientDisplayNames[i];
					}
				}
				List<RecipientTrackingEvent> rawSerializedEvents = messageTrackingReport.RawSerializedEvents;
				if (rawSerializedEvents != null)
				{
					recipientTrackingEvents = TrackingConverter.Convert(rawSerializedEvents, ExchangeVersion.Current);
				}
			}
			messageTrackingReport3.RecipientTrackingEvents = recipientTrackingEvents;
			return new ServiceResult<MessageTrackingReport>(messageTrackingReport3);
		}

		private MessageTrackingReport RunGetAdditionalReports(SearchScope scope, LogCache logCache, ReportConstraints constraints, out bool hasSearchResult)
		{
			hasSearchResult = false;
			SearchMessageTrackingReportImpl searchMessageTrackingReportImpl = new SearchMessageTrackingReportImpl(this.directoryContext, scope, null, null, this.reportId.Server, null, logCache, null, this.reportId.MessageId, Unlimited<uint>.UnlimitedValue, false, false, true, false);
			List<MessageTrackingSearchResult> list = searchMessageTrackingReportImpl.Execute();
			if (list == null || list.Count == 0)
			{
				return null;
			}
			MessageTrackingReport messageTrackingReport = null;
			foreach (MessageTrackingSearchResult messageTrackingSearchResult in list)
			{
				GetMessageTrackingReportImpl getMessageTrackingReportImpl = new GetMessageTrackingReportImpl(this.directoryContext, scope, messageTrackingSearchResult.MessageTrackingReportId, logCache, constraints);
				MessageTrackingReport messageTrackingReport2 = getMessageTrackingReportImpl.Execute();
				if (messageTrackingReport2 != null)
				{
					if (StringComparer.Ordinal.Equals(base.Request.MessageTrackingReportId, messageTrackingReport2.MessageTrackingReportId.ToString()))
					{
						ExTraceGlobals.WebServiceTracer.TraceDebug((long)this.GetHashCode(), "Requested Report-ID is present in RunGetAdditionalReports results");
						hasSearchResult = true;
					}
					messageTrackingReport2.AssignReportIdToRecipEvents();
					if (messageTrackingReport == null)
					{
						messageTrackingReport = messageTrackingReport2;
					}
					else
					{
						messageTrackingReport.MergeRecipientEventsFrom(messageTrackingReport2);
					}
				}
				if (this.errors != null)
				{
					this.errors.Add(getMessageTrackingReportImpl.Errors);
				}
				else
				{
					this.errors = getMessageTrackingReportImpl.Errors;
				}
			}
			return messageTrackingReport;
		}

		private void ValidateRequest()
		{
			if (!MessageTrackingReportId.TryParse(base.Request.MessageTrackingReportId, out this.reportId))
			{
				ExTraceGlobals.WebServiceTracer.TraceError<string>((long)this.GetHashCode(), "Invalid message-tracking report ID: {0}", base.Request.MessageTrackingReportId);
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			ExTraceGlobals.WebServiceTracer.TraceDebug<MessageTrackingReportId>((long)this.GetHashCode(), "MessageTrackingReportId: {0}", this.reportId);
			this.domain = this.reportId.Domain;
			if (base.Request.ReportTemplate != MessageTrackingReportTemplate.RecipientPath)
			{
				return;
			}
			if (base.Request.RecipientFilter == null)
			{
				ExTraceGlobals.WebServiceTracer.TraceError((long)this.GetHashCode(), "RecipientPath template specified without a RecipientFilter");
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			this.recipientFilter = new SmtpAddress?(new SmtpAddress(base.Request.RecipientFilter.EmailAddress));
			if (!this.recipientFilter.Value.IsValidAddress)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U);
			}
			ExTraceGlobals.WebServiceTracer.TraceDebug<SmtpAddress?>((long)this.GetHashCode(), "RecipientFilter: {0}", this.recipientFilter);
		}

		private static RecipientTrackingEvent[] emptyRecipientEvents = new RecipientTrackingEvent[0];

		private string domain;

		private MessageTrackingReportId reportId;

		private SmtpAddress? recipientFilter;
	}
}
