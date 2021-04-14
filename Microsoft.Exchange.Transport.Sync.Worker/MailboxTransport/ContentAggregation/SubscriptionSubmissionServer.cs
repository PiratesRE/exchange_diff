using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Threading;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.SubscriptionSubmission;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Submission;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionSubmissionServer : SubscriptionSubmissionRpcServer
	{
		public override byte[] SubscriptionSubmit(int version, byte[] inputBlob)
		{
			bool flag = false;
			AggregationWorkItem aggregationWorkItem = null;
			byte[] result;
			try
			{
				if (this.stopped)
				{
					this.syncLogSession.LogInformation((TSLID)503UL, SubscriptionSubmissionServer.diag, "Server is stopped so we will not submit.", new object[0]);
					result = SubscriptionSubmissionServer.CreateResponsePropertyCollection(SubscriptionSubmissionResult.EdgeTransportStopped);
				}
				else if (!AggregationConfiguration.Instance.IsEnabled)
				{
					this.syncLogSession.LogInformation((TSLID)504UL, SubscriptionSubmissionServer.diag, "TransportSync is disabled so we will not submit.", new object[0]);
					result = SubscriptionSubmissionServer.CreateResponsePropertyCollection(SubscriptionSubmissionResult.TransportSyncDisabled);
				}
				else
				{
					this.syncLogSession.LogDebugging((TSLID)505UL, SubscriptionSubmissionServer.diag, "Submitting Subscription.", new object[0]);
					MdbefPropertyCollection inputArgs = MdbefPropertyCollection.Create(inputBlob, 0, inputBlob.Length);
					AggregationSubscription aggregationSubscription = null;
					bool flag2 = false;
					Guid guid;
					if (!this.TryGetInputValue<Guid>(inputArgs, (SubscriptionSubmissionPropTag)2684747848U, out guid))
					{
						flag2 = true;
					}
					Guid guid2;
					if (!this.TryGetInputValue<Guid>(inputArgs, (SubscriptionSubmissionPropTag)2684551240U, out guid2))
					{
						flag2 = true;
					}
					string text;
					if (!this.TryGetInputValue<string>(inputArgs, (SubscriptionSubmissionPropTag)2684420127U, out text))
					{
						flag2 = true;
					}
					short num;
					if (!this.TryGetInputValue<short>(inputArgs, (SubscriptionSubmissionPropTag)2684354562U, out num))
					{
						flag2 = true;
					}
					byte[] byteArray;
					if (!this.TryGetInputValue<byte[]>(inputArgs, (SubscriptionSubmissionPropTag)2684485890U, out byteArray))
					{
						flag2 = true;
					}
					bool flag3;
					if (!this.TryGetInputValue<bool>(inputArgs, (SubscriptionSubmissionPropTag)2684616715U, out flag3))
					{
						flag2 = true;
					}
					short num2;
					if (!this.TryGetInputValue<short>(inputArgs, (SubscriptionSubmissionPropTag)2684944386U, out num2))
					{
						flag2 = true;
					}
					bool flag4;
					if (!this.TryGetInputValue<bool>(inputArgs, (SubscriptionSubmissionPropTag)2685009931U, out flag4))
					{
						flag2 = true;
					}
					Guid databaseGuid;
					if (!this.TryGetInputValue<Guid>(inputArgs, (SubscriptionSubmissionPropTag)2684682312U, out databaseGuid))
					{
						flag2 = true;
					}
					string text2;
					if (!this.TryGetInputValue<string>(inputArgs, (SubscriptionSubmissionPropTag)2684813343U, out text2))
					{
						flag2 = true;
					}
					Guid guid3;
					if (!this.TryGetInputValue<Guid>(inputArgs, (SubscriptionSubmissionPropTag)2684878920U, out guid3))
					{
						flag2 = true;
					}
					short num3;
					if (!this.TryGetInputValue<short>(inputArgs, (SubscriptionSubmissionPropTag)2685337602U, out num3))
					{
						flag2 = true;
					}
					byte[] serializedData;
					if (!this.TryGetInputValue<byte[]>(inputArgs, (SubscriptionSubmissionPropTag)2685075714U, out serializedData))
					{
						flag2 = true;
					}
					bool flag5;
					if (!this.TryGetInputValue<bool>(inputArgs, (SubscriptionSubmissionPropTag)2685141003U, out flag5))
					{
						flag2 = true;
					}
					Guid guid4;
					if (!this.TryGetInputValue<Guid>(inputArgs, (SubscriptionSubmissionPropTag)2685272136U, out guid4))
					{
						flag2 = true;
					}
					string text3;
					this.TryGetInputValue<string>(inputArgs, (SubscriptionSubmissionPropTag)2685206559U, out text3);
					if (flag2)
					{
						throw new InvalidOperationException("Failed to retrieve arguments from the subscription submission call");
					}
					AggregationSubscriptionType aggregationSubscriptionType = (AggregationSubscriptionType)num;
					StoreObjectId storeObjectId = StoreObjectId.Deserialize(byteArray);
					AggregationType aggregationType = (AggregationType)num2;
					SyncPhase syncPhase = (SyncPhase)num3;
					try
					{
						SerializedSubscription serializedSubscription = SerializedSubscription.FromBytes(serializedData);
						if (serializedSubscription.Version == AggregationSubscription.CurrentSupportedVersion)
						{
							aggregationSubscription = serializedSubscription.DeserializeSubscription();
						}
					}
					catch (SerializationException ex)
					{
						string message = string.Format(CultureInfo.InvariantCulture, "Exception encountered while deserializing the subscription from the dispatching information: {0}", new object[]
						{
							ex
						});
						this.syncLogSession.ReportWatson(message, ex);
					}
					this.syncLogSession.LogInformation((TSLID)506UL, SubscriptionSubmissionServer.diag, "AllRequiredPropsLoaded::userMbxGuid:{0},subsId:{1},legacyDN:{2},subsType:{3},subsMsgId:{4},recoverySync:{5},aggType:{6},initialSync:{7},mbxServer:{8},tenantGuid:{9},subscription:{10},isSyncNow:{11},mbxServerSyncWatermark:{12},mbxServerGuid:{13},syncPhase:{14}", new object[]
					{
						guid,
						guid2,
						text,
						aggregationSubscriptionType,
						storeObjectId.ToBase64String(),
						flag3,
						aggregationType,
						flag4,
						text2,
						guid3,
						aggregationSubscription,
						flag5,
						text3 ?? "<null>",
						guid4,
						syncPhase
					});
					AggregationSubscriptionType aggregationSubscriptionType2 = aggregationSubscriptionType;
					if (aggregationSubscriptionType2 <= AggregationSubscriptionType.IMAP)
					{
						switch (aggregationSubscriptionType2)
						{
						case AggregationSubscriptionType.Pop:
						case AggregationSubscriptionType.DeltaSyncMail:
							break;
						case (AggregationSubscriptionType)3:
							goto IL_38B;
						default:
							if (aggregationSubscriptionType2 != AggregationSubscriptionType.IMAP)
							{
								goto IL_38B;
							}
							break;
						}
					}
					else if (aggregationSubscriptionType2 != AggregationSubscriptionType.Facebook && aggregationSubscriptionType2 != AggregationSubscriptionType.LinkedIn)
					{
						goto IL_38B;
					}
					aggregationWorkItem = new AggregationWorkItem(AggregationConfiguration.Instance.SyncLog, text, storeObjectId, aggregationSubscriptionType, guid2, flag3, databaseGuid, guid, guid3, aggregationType, flag4, text2, flag5, aggregationSubscription, text3, guid4, syncPhase);
					SubscriptionSubmissionResult subscriptionSubmissionResult = this.SubmitWorkItem(aggregationWorkItem);
					flag = (SubscriptionSubmissionResult.Success == subscriptionSubmissionResult);
					this.syncLogSession.LogVerbose((TSLID)1336UL, SubscriptionSubmissionServer.diag, guid2, guid, "Subscription submision returning: '{0}'", new object[]
					{
						subscriptionSubmissionResult
					});
					return SubscriptionSubmissionServer.CreateResponsePropertyCollection(subscriptionSubmissionResult);
					IL_38B:
					throw new InvalidOperationException("Unknown subscription type: " + aggregationSubscriptionType);
				}
			}
			catch (Exception exception)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(exception);
				result = SubscriptionSubmissionServer.CreateResponsePropertyCollection(SubscriptionSubmissionResult.UnknownRetryableError);
			}
			finally
			{
				if (!flag && aggregationWorkItem != null)
				{
					this.syncLogSession.LogInformation((TSLID)527UL, SubscriptionSubmissionServer.diag, aggregationWorkItem.SubscriptionId, aggregationWorkItem.UserMailboxGuid, "Disposing work item due to failure in scheduling: " + aggregationWorkItem, new object[0]);
					aggregationWorkItem.Dispose();
					aggregationWorkItem = null;
				}
			}
			return result;
		}

		internal void Start()
		{
			this.syncLogSession = AggregationConfiguration.Instance.SyncLog.OpenGlobalSession();
			this.syncLogSession.LogInformation((TSLID)525UL, SubscriptionSubmissionServer.diag, "Starting Subscription Submission Server.", new object[0]);
		}

		internal void Stop()
		{
			this.syncLogSession.LogInformation((TSLID)526UL, SubscriptionSubmissionServer.diag, "Stopping Subscription Submission Server.", new object[0]);
			if (this.stopped)
			{
				return;
			}
			RpcServerBase.StopServer(SubscriptionSubmissionRpcServer.RpcIntfHandle);
			this.stopped = true;
		}

		private static byte[] CreateResponsePropertyCollection(SubscriptionSubmissionResult errorCode)
		{
			return new MdbefPropertyCollection
			{
				{
					2835349507U,
					(int)errorCode
				}
			}.GetBytes();
		}

		private bool TryGetInputValue<T>(MdbefPropertyCollection inputArgs, SubscriptionSubmissionPropTag inArg, out T outputValue)
		{
			string format;
			if (RpcHelper.TryGetProperty<T>(inputArgs, (uint)inArg, out outputValue, out format))
			{
				this.syncLogSession.LogDebugging((TSLID)508UL, SubscriptionSubmissionServer.diag, "Read {0}: {1}", new object[]
				{
					inArg,
					outputValue ?? "<<Null>>"
				});
				return true;
			}
			this.syncLogSession.LogError((TSLID)509UL, SubscriptionSubmissionServer.diag, format, new object[0]);
			return false;
		}

		private SubscriptionSubmissionResult SubmitWorkItem(AggregationWorkItem workItem)
		{
			SubscriptionSubmissionResult result;
			try
			{
				if (Interlocked.Increment(ref this.submittingThreads) > Components.Configuration.LocalServer.MaxConcurrentMailboxSubmissions / 2)
				{
					result = SubscriptionSubmissionResult.MaxConcurrentMailboxSubmissions;
				}
				else
				{
					SubscriptionSubmissionResult subscriptionSubmissionResult = AggregationComponent.SubmitWorkItem(workItem);
					result = subscriptionSubmissionResult;
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.submittingThreads);
			}
			return result;
		}

		private static readonly Trace diag = ExTraceGlobals.SubscriptionSubmissionServerTracer;

		private bool stopped;

		private volatile int submittingThreads;

		private GlobalSyncLogSession syncLogSession;
	}
}
