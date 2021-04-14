using System;
using System.Net;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class MapiSubmissionItem : SubmissionItem
	{
		public MapiSubmissionItem(MapiSubmissionInfo mapiSubmissionInfo, MailItemSubmitter context, IStoreDriverTracer storeDriverTracer) : base("mapi", context, mapiSubmissionInfo, storeDriverTracer)
		{
			if (mapiSubmissionInfo == null)
			{
				throw new ArgumentNullException("mapiSubmissionInfo");
			}
			this.mapiSubmissionInfo = mapiSubmissionInfo;
		}

		public override bool Done
		{
			get
			{
				return this.done;
			}
		}

		public override string SourceServerFqdn
		{
			get
			{
				return this.mapiSubmissionInfo.MailboxFqdn;
			}
		}

		public override IPAddress SourceServerNetworkAddress
		{
			get
			{
				return this.mapiSubmissionInfo.NetworkAddress;
			}
		}

		public override DateTime OriginalCreateTime
		{
			get
			{
				return this.mapiSubmissionInfo.OriginalCreateTime;
			}
		}

		public override Exception DoneWithMessage()
		{
			this.done = true;
			return StoreProvider.CallDoneWithMessageWithRetry(base.Session, base.Item, 6, base.Context);
		}

		public override uint LoadFromStore()
		{
			uint result = 0U;
			ExTraceGlobals.FaultInjectionTracer.TraceTest(2615553341U);
			try
			{
				this.OpenStore();
				base.StoreDriverTracer.StoreDriverSubmissionTracer.TracePass(base.StoreDriverTracer.MessageProbeActivityId, 0L, "Bind to message item");
				StoreId storeId = StoreObjectId.FromProviderSpecificId(this.mapiSubmissionInfo.EntryId);
				ExDateTime dt = (base.Context == null) ? default(ExDateTime) : ExDateTime.UtcNow;
				try
				{
					base.Item = StoreProvider.GetMessageItem(base.Session, storeId, StoreObjectSchema.ContentConversionProperties);
				}
				finally
				{
					if (base.Context != null)
					{
						TimeSpan additionalLatency = ExDateTime.UtcNow - dt;
						base.Context.AddRpcLatency(additionalLatency, "Bind message");
					}
				}
				if (!base.IsSubmitMessage)
				{
					base.StoreDriverTracer.StoreDriverSubmissionTracer.TracePass(base.StoreDriverTracer.MessageProbeActivityId, 0L, "Notification recieved for a message that wasn't submitted");
					base.Dispose();
					result = 9U;
				}
			}
			catch (ObjectNotFoundException)
			{
				base.StoreDriverTracer.StoreDriverSubmissionTracer.TracePass(base.StoreDriverTracer.MessageProbeActivityId, 0L, "Message was deleted prior to MailSubmitted event processing completed");
				result = 6U;
			}
			catch (VirusMessageDeletedException)
			{
				base.StoreDriverTracer.StoreDriverSubmissionTracer.TracePass(base.StoreDriverTracer.MessageProbeActivityId, 0L, "Item was marked for deletion by a virus scanner.");
				result = 8U;
			}
			return result;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					base.DisposeMessageItem();
					if (base.Session != null)
					{
						if (base.Context != null)
						{
							LatencyTracker.BeginTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionStoreDisposeSession, base.Context.LatencyTracker);
						}
						try
						{
							base.DisposeStoreSession();
						}
						finally
						{
							if (base.Context != null)
							{
								TimeSpan additionalLatency = LatencyTracker.EndTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionStoreDisposeSession, base.Context.LatencyTracker);
								base.Context.AddRpcLatency(additionalLatency, "Session dispose");
							}
						}
					}
					if (base.Context != null)
					{
						MapiSubmissionInfo mapiSubmissionInfo = (MapiSubmissionInfo)base.Info;
						base.StoreDriverTracer.MapiStoreDriverSubmissionTracer.TracePass(base.StoreDriverTracer.MessageProbeActivityId, 0L, "Event {0}, Mailbox {1}, Mdb {2}, RPC latency {4}", new object[]
						{
							mapiSubmissionInfo.EventCounter,
							mapiSubmissionInfo.MailboxGuid,
							mapiSubmissionInfo.MdbGuid,
							base.Context.RpcLatency
						});
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void OpenStore()
		{
			if (this.mapiSubmissionInfo.IsPublicFolder)
			{
				base.StoreDriverTracer.StoreDriverSubmissionTracer.TracePfdPass<int, string>(base.StoreDriverTracer.MessageProbeActivityId, 0L, "PFD ESD {0} Opening public store on {1}", 29595, this.mapiSubmissionInfo.MailboxServerDN);
				try
				{
					if (base.Context != null)
					{
						LatencyTracker.BeginTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionStoreOpenSession, base.Context.LatencyTracker);
					}
					base.Session = StoreProvider.OpenStore(this.mapiSubmissionInfo.GetOrganizationId(), this.mapiSubmissionInfo.MailboxGuid);
					return;
				}
				finally
				{
					if (base.Context != null)
					{
						TimeSpan additionalLatency = LatencyTracker.EndTrackLatency(LatencyComponent.MailboxTransportSubmissionStoreDriverSubmissionStoreOpenSession, base.Context.LatencyTracker);
						base.Context.AddRpcLatency(additionalLatency, "Open session");
					}
				}
			}
			base.StoreDriverTracer.StoreDriverSubmissionTracer.TracePfdPass(base.StoreDriverTracer.MessageProbeActivityId, 0L, "PFD ESD {0} Opening mailbox {1} on {2},{3}", new object[]
			{
				17307,
				this.mapiSubmissionInfo.MailboxGuid,
				this.mapiSubmissionInfo.MdbGuid,
				this.mapiSubmissionInfo.MailboxFqdn
			});
			ExDateTime dt = (base.Context == null) ? default(ExDateTime) : ExDateTime.UtcNow;
			try
			{
				base.Session = StoreProvider.OpenStore(this.mapiSubmissionInfo.GetOrganizationId(), "DummyName", this.mapiSubmissionInfo.MailboxFqdn, this.mapiSubmissionInfo.MailboxServerDN, this.mapiSubmissionInfo.MailboxGuid, this.mapiSubmissionInfo.MdbGuid, this.mapiSubmissionInfo.GetSenderLocales(), this.mapiSubmissionInfo.GetAggregatedMailboxGuids());
			}
			finally
			{
				if (base.Context != null)
				{
					TimeSpan additionalLatency2 = ExDateTime.UtcNow - dt;
					base.Context.AddRpcLatency(additionalLatency2, "Open session");
				}
			}
			base.SetSessionTimeZone();
		}

		private const string DummyDisplayName = "DummyName";

		private const int MaxRetry = 6;

		private static readonly Trace diag = ExTraceGlobals.StoreDriverSubmissionTracer;

		private MapiSubmissionInfo mapiSubmissionInfo;

		private bool done;
	}
}
