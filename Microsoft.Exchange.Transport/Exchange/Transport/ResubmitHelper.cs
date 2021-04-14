using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.Transport.RemoteDelivery;

namespace Microsoft.Exchange.Transport
{
	internal abstract class ResubmitHelper
	{
		protected ResubmitHelper(ResubmitReason resubmitReason, MessageTrackingSource messageTrackingSource, string sourceContext, NextHopSolutionKey scopingSolutionKey, Trace traceComponent)
		{
			if (traceComponent == null)
			{
				throw new ArgumentNullException("traceComponent");
			}
			this.resubmitReason = resubmitReason;
			this.messageTrackingSource = messageTrackingSource;
			this.sourceContext = (sourceContext ?? string.Empty);
			this.scopingSolutionKey = scopingSolutionKey;
			this.traceComponent = traceComponent;
			this.serverVersion = Components.Configuration.LocalServer.TransportServer.AdminDisplayVersion;
		}

		public ResubmitReason ResubmitReason
		{
			get
			{
				return this.resubmitReason;
			}
		}

		protected Trace TraceComponent
		{
			get
			{
				return this.traceComponent;
			}
		}

		protected MessageTrackingSource MessageTrackingSource
		{
			get
			{
				return this.messageTrackingSource;
			}
		}

		protected string SourceContext
		{
			get
			{
				return this.sourceContext;
			}
		}

		protected bool IsScopedToSolution
		{
			get
			{
				return !this.scopingSolutionKey.Equals(NextHopSolutionKey.Empty);
			}
		}

		protected abstract string GetComponentNameForReceivedHeader();

		protected NextHopSolution GetScopingSolution(TransportMailItem mailItem)
		{
			if (!this.IsScopedToSolution)
			{
				return null;
			}
			NextHopSolution result;
			if (!mailItem.NextHopSolutions.TryGetValue(this.scopingSolutionKey, out result))
			{
				throw new InvalidOperationException("Scoped resubmit did not find a solution for key: " + this.scopingSolutionKey);
			}
			return result;
		}

		public void Resubmit(IEnumerable<TransportMailItem> mailItems)
		{
			if (mailItems == null)
			{
				throw new ArgumentNullException("mailItems");
			}
			foreach (TransportMailItem mailItem in mailItems)
			{
				this.Resubmit(mailItem, null);
			}
		}

		public virtual void Resubmit(TransportMailItem mailItem, Action<TransportMailItem> processClonedItemDelegate = null)
		{
			if (mailItem == null)
			{
				throw new ArgumentNullException("mailItem");
			}
			TransportMailItem transportMailItem;
			lock (mailItem)
			{
				NextHopSolution scopingSolution = this.GetScopingSolution(mailItem);
				IEnumerable<MailRecipient> recipients;
				int count;
				if (scopingSolution == null)
				{
					recipients = mailItem.Recipients;
					count = mailItem.Recipients.Count;
				}
				else
				{
					recipients = scopingSolution.Recipients;
					count = scopingSolution.Recipients.Count;
				}
				int num = 0;
				int num2 = 0;
				transportMailItem = null;
				List<MailRecipient> list = null;
				foreach (MailRecipient mailRecipient in recipients.ToArray<MailRecipient>())
				{
					if (!this.IsDeleted(mailRecipient))
					{
						ResubmitHelper.RecipientAction recipientAction = this.DetermineRecipientAction(mailRecipient);
						if (recipientAction == ResubmitHelper.RecipientAction.Copy || recipientAction == ResubmitHelper.RecipientAction.Move)
						{
							if (transportMailItem == null)
							{
								transportMailItem = this.CloneWithoutRecipients(mailItem);
								if (processClonedItemDelegate != null)
								{
									processClonedItemDelegate(transportMailItem);
								}
							}
							if (recipientAction == ResubmitHelper.RecipientAction.Copy)
							{
								num++;
								this.ProcessRecipient(mailRecipient.CopyTo(transportMailItem));
							}
							else
							{
								if (list == null)
								{
									list = new List<MailRecipient>(count);
								}
								list.Add(mailRecipient);
							}
						}
					}
				}
				int num3;
				if (list != null)
				{
					foreach (MailRecipient mailRecipient2 in list)
					{
						mailRecipient2.MoveTo(transportMailItem);
						this.ProcessRecipient(mailRecipient2);
					}
					num3 = list.Count;
					list.Clear();
					list = null;
				}
				else
				{
					num3 = 0;
				}
				if (transportMailItem == null)
				{
					this.traceComponent.TraceDebug<long, ResubmitReason>((long)this.GetHashCode(), "Mail Item '{0}' not resubmitted since no recipients were chosen for the clone.  Resubmit Reason is '{1}'", mailItem.RecordId, this.resubmitReason);
				}
				else
				{
					this.traceComponent.TraceDebug((long)this.GetHashCode(), "Mail Item '{0}' resubmitted as mail item '{1}'.  '{2}' recipients were copied, '{3}' recipients were moved, '{4}' recipients were reused and '{5}' recipients were ignored.  Resubmit Reason is '{6}'", new object[]
					{
						mailItem.RecordId,
						transportMailItem.RecordId,
						num,
						num3,
						num2,
						count - (num + num3 + num2),
						this.resubmitReason
					});
					string componentNameForReceivedHeader = this.GetComponentNameForReceivedHeader();
					if (!string.IsNullOrEmpty(componentNameForReceivedHeader))
					{
						DateHeader dateHeader = new DateHeader("Date", DateTime.UtcNow);
						string value = dateHeader.Value;
						string localServerTcpInfo = ResubmitHelper.GetLocalServerTcpInfo(transportMailItem);
						ReceivedHeader newChild = new ReceivedHeader(SmtpReceiveServer.ServerName, localServerTcpInfo, SmtpReceiveServer.ServerName, localServerTcpInfo, null, componentNameForReceivedHeader, this.serverVersion.ToString(), null, value);
						transportMailItem.MimeDocument.RootPart.Headers.PrependChild(newChild);
						SystemProbe.TracePass<string>(transportMailItem, "Transport", "Message resubmitted for component '{0}'", componentNameForReceivedHeader);
					}
					else
					{
						SystemProbe.TracePass<string>(transportMailItem, "Transport", "Message resubmitted for reason '{0}'", this.resubmitReason.ToString());
					}
					transportMailItem.CommitLazy();
					MessageTrackingLog.TrackResubmit(this.messageTrackingSource, transportMailItem, mailItem, this.sourceContext);
				}
			}
			if (transportMailItem != null)
			{
				this.TrackLatency(transportMailItem);
				transportMailItem.RouteForHighAvailability = true;
				Components.CategorizerComponent.EnqueueSubmittedMessage(transportMailItem);
				this.UpdatePerformanceCounters();
			}
		}

		protected virtual bool IsDeleted(MailRecipient recipient)
		{
			return !recipient.IsActive;
		}

		protected virtual TransportMailItem CloneWithoutRecipients(TransportMailItem mailItem)
		{
			NextHopSolution scopingSolution = this.GetScopingSolution(mailItem);
			TransportMailItem transportMailItem;
			if (Components.TransportAppConfig.QueueDatabase.CloneInOriginalGeneration)
			{
				transportMailItem = mailItem.CreateNewCopyWithoutRecipients(scopingSolution);
			}
			else
			{
				transportMailItem = mailItem.NewCloneWithoutRecipients(false, null, scopingSolution);
			}
			Resolver.ClearResolverAndTransportSettings(transportMailItem);
			transportMailItem.ShadowServerContext = null;
			transportMailItem.ShadowServerDiscardId = null;
			if (transportMailItem.PrioritizationReason == "ShadowRedundancy")
			{
				transportMailItem.Priority = DeliveryPriority.Normal;
			}
			return transportMailItem;
		}

		protected virtual ResubmitHelper.RecipientAction DetermineRecipientAction(MailRecipient recipient)
		{
			return ResubmitHelper.RecipientAction.Copy;
		}

		protected virtual void ProcessRecipient(MailRecipient recipient)
		{
			Resolver.ClearResolverProperties(recipient);
			recipient.DeliveryTime = null;
			recipient.DeliveredDestination = null;
			recipient.PrimaryServerFqdnGuid = null;
			recipient.RetryCount = 0;
			recipient.Status = Status.Ready;
		}

		protected virtual void TrackLatency(TransportMailItem mailItemToResubmit)
		{
		}

		protected virtual void UpdatePerformanceCounters()
		{
		}

		private static string GetLocalServerTcpInfo(TransportMailItem mailItem)
		{
			ReceivedHeader receivedHeader = mailItem.MimeDocument.RootPart.Headers.FindFirst(HeaderId.Received) as ReceivedHeader;
			if (receivedHeader == null)
			{
				return null;
			}
			return receivedHeader.ByTcpInfo;
		}

		private readonly Trace traceComponent;

		private readonly ResubmitReason resubmitReason;

		private readonly MessageTrackingSource messageTrackingSource;

		private readonly string sourceContext;

		private readonly Version serverVersion;

		private readonly NextHopSolutionKey scopingSolutionKey;

		protected enum RecipientAction
		{
			None,
			Copy,
			Move
		}
	}
}
