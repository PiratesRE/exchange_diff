using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.UM;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess
{
	internal class XSOUMCallDataRecordAccessor : DisposableBase, IUMCallDataRecordStorage, IDisposeTrackable, IDisposable
	{
		public XSOUMCallDataRecordAccessor(ExchangePrincipal mailboxPrincipal)
		{
			ValidateArgument.NotNull(mailboxPrincipal, "mailboxPrincipal");
			this.mailboxPrincipal = mailboxPrincipal;
			this.tracer = new DiagnosticHelper(this, ExTraceGlobals.XsoTracer);
			this.disposeMailboxSession = true;
			this.ExecuteXSOOperation(delegate
			{
				this.Initialize(this.CreateMailboxSession("Client=UM;Action=Manage-CDRMessages"));
			});
		}

		public XSOUMCallDataRecordAccessor(MailboxSession mailboxSession)
		{
			ValidateArgument.NotNull(mailboxSession, "mailboxSession");
			this.tracer = new DiagnosticHelper(this, ExTraceGlobals.XsoTracer);
			this.Initialize(mailboxSession);
		}

		public void CreateUMCallDataRecord(CDRData cdrData)
		{
			ValidateArgument.NotNull(cdrData, "cdrData");
			this.tracer.Trace("XSOUMCallDataRecordAccessor : CreateUMCallDataRecord, saving cdrData {0}", new object[]
			{
				cdrData.CallIdentity
			});
			this.ExecuteXSOOperation(delegate
			{
				using (Folder folder = UMStagingFolder.OpenOrCreateUMReportingFolder(this.mailboxSession))
				{
					MessageItem messageItem2;
					MessageItem messageItem = messageItem2 = XsoUtil.CreateTemporaryMessage(this.mailboxSession, folder, 90);
					try
					{
						if (!this.TrySetExtendedProperty())
						{
							this.tracer.Trace("Unable to set the extended prop on UMReporting Folder. Not saving the CDR", new object[0]);
							return;
						}
						this.SetMessageProperties(messageItem, cdrData);
						messageItem.Save(SaveMode.NoConflictResolution);
					}
					finally
					{
						if (messageItem2 != null)
						{
							((IDisposable)messageItem2).Dispose();
						}
					}
				}
				this.tracer.Trace("XSOUMCallDataRecordAccessor : CreateUMCallDataRecord, Successfully saved cdrData {0}", new object[]
				{
					cdrData.CallIdentity
				});
			});
		}

		public CDRData[] GetUMCallDataRecordsForUser(string userLegacyExchangeDN)
		{
			ValidateArgument.NotNullOrEmpty(userLegacyExchangeDN, "userLegacyExchangeDN");
			this.tracer.Trace("XSOUMCallDataRecordAccessor : GetUMCallDataRecordsForUser, for user {0}", new object[]
			{
				userLegacyExchangeDN
			});
			CDRData[] cdrs = null;
			this.ExecuteXSOOperation(delegate
			{
				SearchState searchState;
				cdrs = UMReportUtil.PerformCDRSearchUsingCI(this.mailboxSession, userLegacyExchangeDN, out searchState);
			});
			if (cdrs == null)
			{
				cdrs = new CDRData[0];
			}
			this.tracer.Trace("XSOUMCallDataRecordAccessor : GetUMCallDataRecordsForUser, for user {0}. Found {1} CDRData records", new object[]
			{
				userLegacyExchangeDN,
				cdrs.Length
			});
			return cdrs;
		}

		public CDRData[] GetUMCallDataRecords(ExDateTime startDateTime, ExDateTime endDateTime, int offset, int numberOfRecordsToRead)
		{
			if (ExDateTime.Equals(startDateTime, ExDateTime.MinValue))
			{
				throw new ArgumentException("Start time is not valid");
			}
			if (ExDateTime.Equals(endDateTime, ExDateTime.MinValue))
			{
				throw new ArgumentException("End time is not valid");
			}
			if (offset < 0)
			{
				throw new ArgumentException("Offset should be greater than or equal to 0");
			}
			if (numberOfRecordsToRead < 0)
			{
				throw new ArgumentException("NumberOfRecords should be greater than or equal to 0");
			}
			if (numberOfRecordsToRead > 500000)
			{
				numberOfRecordsToRead = 500000;
			}
			this.tracer.Trace("XSOUMCallDataRecordAccessor : GetUMCallDataRecords with start date {0}, end date {1}, offset {2}, number of records {3}", new object[]
			{
				startDateTime,
				endDateTime,
				offset,
				numberOfRecordsToRead
			});
			CDRData[] cdrs = null;
			this.ExecuteXSOOperation(delegate
			{
				cdrs = UMReportUtil.ReadCDRs(this.mailboxSession, startDateTime, endDateTime, offset, numberOfRecordsToRead);
			});
			if (cdrs == null)
			{
				cdrs = new CDRData[0];
			}
			this.tracer.Trace("XSOUMCallDataRecordAccessor : GetUMCallDataRecords with start date {0}, end date {1}. Found {2} CDRData records", new object[]
			{
				startDateTime,
				endDateTime,
				cdrs.Length
			});
			return cdrs;
		}

		public UMReportRawCounters[] GetUMCallSummary(Guid dialPlanGuid, Guid gatewayGuid, GroupBy groupby)
		{
			this.tracer.Trace("XSOUMCallDataRecordAccessor : GetUMCallSummary with dial plan {0}, Gateway {1} Request.", new object[]
			{
				dialPlanGuid,
				gatewayGuid
			});
			UMReportRawCounters[] counters = null;
			this.ExecuteXSOOperation(delegate
			{
				counters = UMReportUtil.QueryUMReport(this.mailboxSession, dialPlanGuid, gatewayGuid, groupby);
			});
			if (counters == null)
			{
				counters = new UMReportRawCounters[0];
			}
			this.tracer.Trace("XSOUMCallDataRecordAccessor : GetUMCallSummary with dial plan {0}, Gateway {1}. Found {2} UMReportRawCounters records", new object[]
			{
				dialPlanGuid,
				gatewayGuid,
				counters.Length
			});
			return counters;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.tracer.Trace("XSOCallDataRecordStorage : InternalDispose", new object[0]);
				if (this.mailboxSession != null && this.disposeMailboxSession)
				{
					this.mailboxSession.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XSOUMCallDataRecordAccessor>(this);
		}

		private MailboxSession CreateMailboxSession(string clientString)
		{
			ValidateArgument.NotNullOrEmpty(clientString, "clientString");
			return MailboxSessionEstablisher.OpenAsAdmin(this.mailboxPrincipal, CultureInfo.InvariantCulture, clientString);
		}

		private bool TrySetExtendedProperty()
		{
			bool result = false;
			try
			{
				UMReportUtil.SetMailboxExtendedProperty(this.mailboxSession, true);
				result = true;
			}
			catch (FolderSaveException ex)
			{
				this.tracer.Trace("Cannot set the extended prop on the mailbox. Details {0}", new object[]
				{
					ex
				});
				if (ex.InnerException != null && ex.InnerException is PropertyErrorException)
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CannotSetExtendedProp, null, new object[]
					{
						this.mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress,
						CommonUtil.ToEventLogString(ex)
					});
				}
			}
			return result;
		}

		private void SetMessageProperties(MessageItem messageToSubmit, CDRData cdrData)
		{
			messageToSubmit.ClassName = "IPM.Note.Microsoft.CDR.UM";
			messageToSubmit[MessageItemSchema.XCDRDataCallStartTime] = new ExDateTime(ExTimeZone.UtcTimeZone, cdrData.CallStartTime);
			messageToSubmit.From = new Participant(cdrData.CallerLegacyExchangeDN, string.Empty, "EX");
			messageToSubmit.Recipients.Add(new Participant(cdrData.CalleeLegacyExchangeDN, string.Empty, "EX"));
			messageToSubmit[MessageItemSchema.XCDRDataCallType] = cdrData.CallType;
			messageToSubmit[MessageItemSchema.XCDRDataCallIdentity] = cdrData.CallIdentity;
			messageToSubmit[MessageItemSchema.XCDRDataParentCallIdentity] = cdrData.ParentCallIdentity;
			messageToSubmit[MessageItemSchema.XCDRDataUMServerName] = cdrData.UMServerName;
			messageToSubmit[MessageItemSchema.XCDRDataDialPlanGuid] = cdrData.DialPlanGuid;
			messageToSubmit[MessageItemSchema.XCDRDataDialPlanName] = cdrData.DialPlanName;
			messageToSubmit[MessageItemSchema.XCDRDataCallDuration] = cdrData.CallDuration;
			messageToSubmit[MessageItemSchema.XCDRDataIPGatewayAddress] = cdrData.IPGatewayAddress;
			messageToSubmit[MessageItemSchema.XCDRDataIPGatewayName] = cdrData.IPGatewayName;
			messageToSubmit[MessageItemSchema.XCDRDataGatewayGuid] = cdrData.GatewayGuid;
			messageToSubmit[MessageItemSchema.XCDRDataCalledPhoneNumber] = cdrData.CalledPhoneNumber;
			messageToSubmit[MessageItemSchema.XCDRDataCallerPhoneNumber] = cdrData.CallerPhoneNumber;
			messageToSubmit[MessageItemSchema.XCDRDataOfferResult] = cdrData.OfferResult;
			messageToSubmit[MessageItemSchema.XCDRDataDropCallReason] = cdrData.DropCallReason;
			messageToSubmit[MessageItemSchema.XCDRDataReasonForCall] = cdrData.ReasonForCall;
			messageToSubmit[MessageItemSchema.XCDRDataTransferredNumber] = cdrData.TransferredNumber;
			messageToSubmit[MessageItemSchema.XCDRDataDialedString] = cdrData.DialedString;
			messageToSubmit[MessageItemSchema.XCDRDataCallerMailboxAlias] = cdrData.CallerMailboxAlias;
			messageToSubmit[MessageItemSchema.XCDRDataCalleeMailboxAlias] = cdrData.CalleeMailboxAlias;
			messageToSubmit[MessageItemSchema.XCDRDataAutoAttendantName] = cdrData.AutoAttendantName;
			messageToSubmit[MessageItemSchema.XCDRDataAudioCodec] = cdrData.AudioQualityMetrics.AudioCodec;
			messageToSubmit[MessageItemSchema.XCDRDataBurstDensity] = cdrData.AudioQualityMetrics.BurstDensity;
			messageToSubmit[MessageItemSchema.XCDRDataBurstDuration] = cdrData.AudioQualityMetrics.BurstDuration;
			messageToSubmit[MessageItemSchema.XCDRDataJitter] = cdrData.AudioQualityMetrics.Jitter;
			messageToSubmit[MessageItemSchema.XCDRDataNMOS] = cdrData.AudioQualityMetrics.NMOS;
			messageToSubmit[MessageItemSchema.XCDRDataNMOSDegradation] = cdrData.AudioQualityMetrics.NMOSDegradation;
			messageToSubmit[MessageItemSchema.XCDRDataNMOSDegradationJitter] = cdrData.AudioQualityMetrics.NMOSDegradationJitter;
			messageToSubmit[MessageItemSchema.XCDRDataNMOSDegradationPacketLoss] = cdrData.AudioQualityMetrics.NMOSDegradationPacketLoss;
			messageToSubmit[MessageItemSchema.XCDRDataPacketLoss] = cdrData.AudioQualityMetrics.PacketLoss;
			messageToSubmit[MessageItemSchema.XCDRDataRoundTrip] = cdrData.AudioQualityMetrics.RoundTrip;
		}

		private void Initialize(MailboxSession session)
		{
			ExAssert.RetailAssert(session != null, "MailboxSession cannot be null");
			this.mailboxSession = session;
			this.tracer.Trace("XSOUMCallDataRecordAccessor called from WebServices : {1}", new object[]
			{
				!this.disposeMailboxSession
			});
		}

		private void ExecuteXSOOperation(Action function)
		{
			try
			{
				function();
			}
			catch (Exception ex)
			{
				if (this.mailboxPrincipal != null)
				{
					XsoUtil.LogMailboxConnectionFailureException(ex, this.mailboxPrincipal);
				}
				CallIdTracer.TraceError(ExTraceGlobals.UMReportsTracer, this, ex.ToString(), new object[0]);
				throw;
			}
		}

		private const int MessageRetentionPeriod = 90;

		private readonly bool disposeMailboxSession;

		private ExchangePrincipal mailboxPrincipal;

		private MailboxSession mailboxSession;

		private DiagnosticHelper tracer;
	}
}
