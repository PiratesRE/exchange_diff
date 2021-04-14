using System;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PeopleIKnow.Agent;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PeopleIKnowAgent : StoreDriverDeliveryAgent
	{
		public PeopleIKnowAgent(bool isAgentEnabled)
		{
			if (isAgentEnabled)
			{
				base.OnPromotedMessage += this.OnPromotedMessageHandler;
			}
		}

		public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			int hashCode = this.GetHashCode();
			PeopleIKnowAgent.tracer.TraceDebug((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: entering");
			if (args == null || !(args is StoreDriverDeliveryEventArgsImpl))
			{
				PeopleIKnowAgent.tracer.TraceError((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: args is null or not a StoreDriverDeliveryEventArgsImpl; exiting");
				return;
			}
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			MailboxSession mailboxSession = storeDriverDeliveryEventArgsImpl.MailboxSession;
			if (mailboxSession == null)
			{
				PeopleIKnowAgent.tracer.TraceError((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: MailboxSession is null; exiting");
				return;
			}
			string text = null;
			MessageItem messageItem = null;
			if (storeDriverDeliveryEventArgsImpl.MailItem != null && storeDriverDeliveryEventArgsImpl.MailItem.Message != null && storeDriverDeliveryEventArgsImpl.MailItem.Message.Sender != null && !string.IsNullOrEmpty(storeDriverDeliveryEventArgsImpl.MailItem.Message.Sender.SmtpAddress) && storeDriverDeliveryEventArgsImpl.ReplayItem != null)
			{
				text = storeDriverDeliveryEventArgsImpl.MailItem.Message.Sender.SmtpAddress;
				messageItem = storeDriverDeliveryEventArgsImpl.ReplayItem;
				PeopleIKnowAgent.tracer.TraceDebug<string>((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: senderSmtpAddress is {0}", text);
				string text2 = mailboxSession.MdbGuid.ToString();
				PeopleIKnowAgent.tracer.TraceDebug<string>((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: mdbGuid is {0}", text2);
				PeopleIKnowAgent.PeopleIKnowAgentPerfLogging peopleIKnowAgentPerfLogging = new PeopleIKnowAgent.PeopleIKnowAgentPerfLogging();
				try
				{
					using (new StopwatchPerformanceTracker("OnPromotedMessageHandler", peopleIKnowAgentPerfLogging))
					{
						using (new CpuPerformanceTracker("OnPromotedMessageHandler", peopleIKnowAgentPerfLogging))
						{
							using (new StorePerformanceTracker("OnPromotedMessageHandler", peopleIKnowAgentPerfLogging))
							{
								using (Folder folder = Folder.Bind(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox), new PropertyDefinition[]
								{
									FolderSchema.PeopleIKnowEmailAddressCollection,
									FolderSchema.PeopleIKnowEmailAddressRelevanceScoreCollection
								}))
								{
									if (folder == null)
									{
										PeopleIKnowAgent.tracer.TraceDebug((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: inbox  folder could not be found");
									}
									else
									{
										byte[] array = (folder.TryGetProperty(FolderSchema.PeopleIKnowEmailAddressRelevanceScoreCollection) as byte[]) ?? (folder.TryGetProperty(FolderSchema.PeopleIKnowEmailAddressCollection) as byte[]);
										if (array == null || array.Length == 0)
										{
											PeopleIKnowAgent.tracer.TraceDebug((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: The property PeopleIKnowAddressList did not exist on the folder");
										}
										else
										{
											bool flag = false;
											PeopleIKnowEmailAddressCollection peopleIKnowEmailAddressCollection = PeopleIKnowEmailAddressCollection.CreateFromByteArray(array, PeopleIKnowAgent.tracer, hashCode);
											if (peopleIKnowEmailAddressCollection != null)
											{
												PeopleIKnowMetadata peopleIKnowMetadata;
												flag = peopleIKnowEmailAddressCollection.Contains(text, out peopleIKnowMetadata);
												if (flag && peopleIKnowMetadata != null)
												{
													messageItem[MessageItemSchema.SenderRelevanceScore] = peopleIKnowMetadata.RelevanceScore;
												}
											}
											PeopleIKnowAgent.tracer.TraceDebug<string, string>((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: The senderSmtpAddress {0} {1} contained in the list of people I know", text, flag ? "is" : "is not");
											messageItem.IsFromFavoriteSender = flag;
										}
									}
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (ex is OutOfMemoryException || ex is StackOverflowException || ex is ThreadAbortException)
					{
						throw;
					}
					PeopleIKnowAgent.tracer.TraceError<Exception>((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler encountered an exception: {0}", ex);
					StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_PeopleIKnowAgentException, ex.Message, new object[]
					{
						ex
					});
				}
				finally
				{
					PeopleIKnowAgent.tracer.TraceDebug<TimeSpan>((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: StopwatchTime = {0}", peopleIKnowAgentPerfLogging.StopwatchTime);
					PeopleIKnowAgent.tracer.TraceDebug<TimeSpan>((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: CpuTime = {0}", peopleIKnowAgentPerfLogging.CpuTime);
					PeopleIKnowAgent.tracer.TraceDebug<uint>((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: StoreRPCs = {0}", peopleIKnowAgentPerfLogging.StoreRPCs);
					PeopleIKnow.GetInstance(text2).AverageStopWatchTime.IncrementBy(peopleIKnowAgentPerfLogging.StopwatchTime.Ticks);
					PeopleIKnow.GetInstance(text2).AverageStopWatchTimeBase.Increment();
					PeopleIKnow.GetInstance(text2).AverageCpuTime.IncrementBy(peopleIKnowAgentPerfLogging.CpuTime.Ticks);
					PeopleIKnow.GetInstance(text2).AverageCpuTimeBase.Increment();
					PeopleIKnow.GetInstance(text2).AverageStoreRPCs.IncrementBy((long)((ulong)peopleIKnowAgentPerfLogging.StoreRPCs));
					PeopleIKnow.GetInstance(text2).AverageStoreRPCsBase.Increment();
					PeopleIKnowAgent.tracer.TraceDebug((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: exiting");
				}
				return;
			}
			PeopleIKnowAgent.tracer.TraceError<string, string>((long)hashCode, "PeopleIKnowAgent.OnPromotedMessageHandler: senderSmtpAddress {0} null, replayItem {1} null; exiting", string.IsNullOrEmpty(text) ? "is" : "is not", (messageItem == null) ? "is" : "is not");
		}

		private const string OnPromotedMessageHandlerMarker = "OnPromotedMessageHandler";

		private static readonly Trace tracer = ExTraceGlobals.GeneralTracer;

		private class PeopleIKnowAgentPerfLogging : IPerformanceDataLogger
		{
			public TimeSpan StopwatchTime
			{
				get
				{
					return this.stopwatchTime;
				}
			}

			public TimeSpan CpuTime
			{
				get
				{
					return this.cpuTime;
				}
			}

			public uint StoreRPCs
			{
				get
				{
					return this.storeRPCs;
				}
			}

			public void Log(string marker, string counter, TimeSpan dataPoint)
			{
				if (counter.Equals("ElapsedTime"))
				{
					this.stopwatchTime = dataPoint;
					return;
				}
				if (counter.Equals("CpuTime"))
				{
					this.cpuTime = dataPoint;
				}
			}

			public void Log(string marker, string counter, uint dataPoint)
			{
				if (counter.Equals("StoreRpcCount"))
				{
					this.storeRPCs = dataPoint;
				}
			}

			public void Log(string marker, string counter, string dataPoint)
			{
				throw new NotImplementedException();
			}

			private TimeSpan stopwatchTime;

			private TimeSpan cpuTime;

			private uint storeRPCs;
		}
	}
}
