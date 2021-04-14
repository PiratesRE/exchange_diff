using System;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Storage.Messaging.Utah
{
	internal class MessagingGeneration : DataGeneration
	{
		public MessageTable MessageTable
		{
			get
			{
				return this.messageTable;
			}
		}

		public RecipientTable RecipientTable
		{
			get
			{
				return this.recipientTable;
			}
		}

		public new MessagingDatabase MessagingDatabase
		{
			get
			{
				return (MessagingDatabase)base.MessagingDatabase;
			}
		}

		public static int GetGenerationId(long combinedId)
		{
			return (int)((ulong)combinedId >> 32);
		}

		public static int GetRowId(long combinedId)
		{
			return (int)combinedId;
		}

		public static long CombineIds(int generationId, int rowId)
		{
			ulong num = (ulong)((ulong)((long)generationId) << 32);
			return (long)(num | (ulong)rowId);
		}

		public bool TryEnterReplay()
		{
			return this.TryEnterState(MessagingGeneration.State.Replaying);
		}

		public bool TryExitReplay()
		{
			return this.TryExitState(MessagingGeneration.State.Replaying);
		}

		public override XElement GetDiagnosticInfo(string argument)
		{
			XElement xelement = base.GetDiagnosticInfo(argument);
			xelement.Add(new XElement("State", (MessagingGeneration.State)this.state));
			xelement.Add(new XElement("AttachStatus", base.IsAttached));
			if (base.IsAttached)
			{
				xelement.Add(new XElement("AttachTime", this.attachTime));
				xelement.Add(new XElement("MessageTableName", this.MessageTable.Name));
				xelement.Add(new XElement("MessageCount", this.MessageTable.MessageCount));
				xelement.Add(new XElement("ActiveMessageCount", this.MessageTable.ActiveMessageCount));
				xelement.Add(new XElement("PendingMessageCount", this.MessageTable.PendingMessageCount));
				xelement.Add(new XElement("RecipientTableName", this.RecipientTable.Name));
				xelement.Add(new XElement("RecipientCount", this.RecipientTable.RecipientCount));
				xelement.Add(new XElement("ActiveRecipientCount", this.RecipientTable.ActiveRecipientCount));
				foreach (object obj in typeof(Destination.DestinationType).GetEnumValues())
				{
					Destination.DestinationType destinationType = (Destination.DestinationType)obj;
					XElement xelement2 = new XElement("SafetyNetRecipientCount", this.RecipientTable.GetSafetyNetRecipientCount(destinationType));
					xelement2.SetAttributeValue("DestinationType", destinationType);
					xelement.Add(xelement2);
				}
			}
			xelement.Add(new XElement("DiagnosticInfo", this.diagnosticInfo));
			return xelement;
		}

		private bool TryEnterState(MessagingGeneration.State newState)
		{
			return Interlocked.CompareExchange(ref this.state, (int)newState, 0) == 0;
		}

		private bool TryExitState(MessagingGeneration.State oldState)
		{
			return Interlocked.CompareExchange(ref this.state, 0, (int)oldState) == (int)oldState;
		}

		protected override void Attach(Transaction transaction)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.messageTable = new MessageTable(this);
			this.MessageTable.Attach(transaction.Connection.Source, transaction.Connection, "MailItemTable-" + base.Name);
			this.recipientTable = new RecipientTable(this);
			this.RecipientTable.Attach(transaction.Connection.Source, transaction.Connection, "RecipientTable-" + base.Name);
			this.attachTime = stopwatch.Elapsed;
			stopwatch.Stop();
		}

		protected override void Detach()
		{
			this.MessageTable.Detach();
			this.RecipientTable.Detach();
		}

		protected override GenerationCleanupMode CleanupInternal()
		{
			if (!this.TryEnterState(MessagingGeneration.State.Expiring))
			{
				string message = string.Format("Cannot clean up generation {0}, it is being used. State={1}", base.Name, (MessagingGeneration.State)this.state);
				this.diagnosticInfo = message;
				ExTraceGlobals.StorageTracer.TraceDebug((long)base.GenerationId, message);
				return GenerationCleanupMode.None;
			}
			DateTime dateTime = this.MessagingDatabase.GenerationManager.ReferenceClock();
			bool flag = dateTime - base.EndTime > this.MessagingDatabase.Config.MessagingGenerationExpirationAge;
			bool flag2 = this.MessageTable.PendingMessageCount > 0;
			if (!flag)
			{
				if (flag2)
				{
					string message2 = string.Format("Cannot clean up generation {0}, it has {1} pending messages", base.Name, this.MessageTable.PendingMessageCount);
					this.diagnosticInfo = message2;
					ExTraceGlobals.StorageTracer.TraceDebug((long)base.GenerationId, message2);
					this.TryExitState(MessagingGeneration.State.Expiring);
					return GenerationCleanupMode.None;
				}
				DateTime dateTime2 = dateTime - this.MessagingDatabase.Config.MessagingGenerationCleanupAge;
				int timeOffset = MailRecipientStorage.GetTimeOffset(dateTime2);
				if (timeOffset < this.lastKnownDelivery)
				{
					string message3 = string.Format("Cannot cleanup generation {0} for cutoff {1}, it has deliveries at least till {2}", base.Name, dateTime2, MailRecipientStorage.GetTimeFromOffset(this.lastKnownDelivery));
					this.diagnosticInfo = message3;
					ExTraceGlobals.StorageTracer.TraceDebug((long)base.GenerationId, message3);
					this.TryExitState(MessagingGeneration.State.Expiring);
					return GenerationCleanupMode.None;
				}
				using (DataConnection dataConnection = this.MessageTable.DataSource.DemandNewConnection())
				{
					using (dataConnection.BeginTransaction())
					{
						using (DataTableCursor dataTableCursor = this.RecipientTable.OpenCursor(dataConnection))
						{
							dataTableCursor.SetCurrentIndex("NdxRecipient_DestinationHash_DeliveryTimeOffset");
							dataTableCursor.MoveBeforeFirst();
							while (dataTableCursor.TryMoveNext(false))
							{
								int? num = this.RecipientTable.Schemas[7].Int32FromIndex(dataTableCursor);
								if (num != null && num.Value > timeOffset)
								{
									this.lastKnownDelivery = num.Value;
									string message4 = string.Format("Cannot clean up generation {0} for cutoff {1}, it has deliveries at least till {2}", base.Name, dateTime2, MailRecipientStorage.GetTimeFromOffset(this.lastKnownDelivery));
									this.diagnosticInfo = message4;
									ExTraceGlobals.StorageTracer.TraceDebug((long)base.GenerationId, message4);
									this.TryExitState(MessagingGeneration.State.Expiring);
									return GenerationCleanupMode.None;
								}
							}
						}
					}
				}
			}
			GenerationCleanupMode result;
			using (DataConnection dataConnection2 = this.MessageTable.DataSource.DemandNewConnection())
			{
				using (Transaction transaction2 = dataConnection2.BeginTransaction())
				{
					if (flag2)
					{
						this.RecipientTable.TryCleanup(transaction2);
						this.MessageTable.TryCleanup(transaction2);
						transaction2.Commit(TransactionCommitMode.Lazy);
						this.TryExitState(MessagingGeneration.State.Expiring);
						result = GenerationCleanupMode.Cleanup;
					}
					else
					{
						transaction2.Checkpoint(TransactionCommitMode.Lazy, 100);
						this.RecipientTable.TryDrop(dataConnection2);
						this.MessageTable.TryDrop(dataConnection2);
						transaction2.Commit(TransactionCommitMode.Lazy);
						this.TryExitState(MessagingGeneration.State.Expiring);
						result = GenerationCleanupMode.Drop;
					}
				}
			}
			return result;
		}

		protected override bool DropInternal()
		{
			if (!this.TryEnterState(MessagingGeneration.State.Expiring))
			{
				string message = string.Format("Cannot drop generation {0}, it is being used. State={1}", base.Name, (MessagingGeneration.State)this.state);
				this.diagnosticInfo = message;
				ExTraceGlobals.StorageTracer.TraceDebug((long)base.GenerationId, message);
				return false;
			}
			using (DataConnection dataConnection = this.MessageTable.DataSource.DemandNewConnection())
			{
				using (Transaction transaction = dataConnection.BeginTransaction())
				{
					this.RecipientTable.TryDrop(dataConnection);
					this.MessageTable.TryDrop(dataConnection);
					transaction.Commit(TransactionCommitMode.Lazy);
				}
			}
			return true;
		}

		public long CombineIds(int id)
		{
			return MessagingGeneration.CombineIds(base.GenerationId, id);
		}

		private MessageTable messageTable;

		private RecipientTable recipientTable;

		private int lastKnownDelivery = int.MinValue;

		private int state;

		private string diagnosticInfo = "newvalue";

		private TimeSpan attachTime;

		private enum State
		{
			Active,
			Expiring,
			Replaying
		}
	}
}
