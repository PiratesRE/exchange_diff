using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Rpc;
using Microsoft.Exchange.Transport.Sync.Common.Rpc.Completion;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionCompletionDataUnpacker
	{
		internal SubscriptionCompletionDataUnpacker(GlobalSyncLogSession globalSyncLogSession)
		{
			SyncUtilities.ThrowIfArgumentNull("globalSyncLogSession", globalSyncLogSession);
			this.syncLogSession = globalSyncLogSession;
		}

		internal uint UnpackData(byte[] inputBlob, out SubscriptionCompletionData subscriptionCompletionData)
		{
			SyncUtilities.ThrowIfArgumentNull("inputBlob", inputBlob);
			MdbefPropertyCollection inputArgs = MdbefPropertyCollection.Create(inputBlob, 0, inputBlob.Length);
			subscriptionCompletionData = null;
			Guid databaseGuid;
			Guid mailboxGuid;
			Guid subscriptionGuid;
			byte[] byteArray;
			SyncPhase syncPhase;
			SubscriptionCompletionStatus subscriptionCompletionStatus;
			AggregationType aggregationType;
			if (!this.TryGetProperty<Guid>(inputArgs, (SubscriptionCompletionPropTag)2684354632U, out databaseGuid) || !this.TryGetProperty<Guid>(inputArgs, (SubscriptionCompletionPropTag)2684813384U, out mailboxGuid) || !this.TryGetProperty<Guid>(inputArgs, (SubscriptionCompletionPropTag)2685075528U, out subscriptionGuid) || !this.TryGetProperty<byte[]>(inputArgs, (SubscriptionCompletionPropTag)2684420354U, out byteArray) || !this.TryGetEnumProperty<SyncPhase>(inputArgs, (SubscriptionCompletionPropTag)2684878851U, out syncPhase) || !this.TryGetEnumProperty<SubscriptionCompletionStatus>(inputArgs, (SubscriptionCompletionPropTag)2684616707U, out subscriptionCompletionStatus) || !this.TryGetEnumProperty<AggregationType>(inputArgs, (SubscriptionCompletionPropTag)2685140995U, out aggregationType))
			{
				return 268435457U;
			}
			subscriptionCompletionData = new SubscriptionCompletionData();
			subscriptionCompletionData.DatabaseGuid = databaseGuid;
			subscriptionCompletionData.MailboxGuid = mailboxGuid;
			subscriptionCompletionData.SubscriptionGuid = subscriptionGuid;
			subscriptionCompletionData.SyncPhase = syncPhase;
			subscriptionCompletionData.AggregationType = aggregationType;
			subscriptionCompletionData.SubscriptionCompletionStatus = subscriptionCompletionStatus;
			try
			{
				subscriptionCompletionData.SubscriptionMessageID = StoreObjectId.Deserialize(byteArray);
			}
			catch (CorruptDataException)
			{
				this.syncLogSession.LogError((TSLID)420UL, "Invalid SubscriptionMessageId received.", new object[0]);
				return 268435459U;
			}
			int? num;
			if (this.TryGetProperty<int?>(inputArgs, (SubscriptionCompletionPropTag)2684551171U, out num))
			{
				subscriptionCompletionData.MoreDataToDownload = (num != null && num > 0);
			}
			string syncWatermark;
			if (this.TryGetProperty<string>(inputArgs, (SubscriptionCompletionPropTag)2685009951U, out syncWatermark))
			{
				subscriptionCompletionData.SyncWatermark = syncWatermark;
			}
			byte[] serializedData;
			if (this.TryGetProperty<byte[]>(inputArgs, (SubscriptionCompletionPropTag)2684944642U, out serializedData))
			{
				try
				{
					subscriptionCompletionData.SerializedSubscription = SerializedSubscription.FromBytes(serializedData);
				}
				catch (SerializationException exception)
				{
					this.syncLogSession.ReportWatson("Invalid Subscription received.", exception);
				}
			}
			return 0U;
		}

		private bool TryGetEnumProperty<TOutputType>(MdbefPropertyCollection inputArgs, SubscriptionCompletionPropTag property, out TOutputType outputValue) where TOutputType : struct
		{
			outputValue = default(TOutputType);
			object obj;
			if (this.TryGetProperty<object>(inputArgs, property, out obj))
			{
				outputValue = (TOutputType)((object)obj);
				return true;
			}
			return false;
		}

		private bool TryGetProperty<TOutputType>(MdbefPropertyCollection inputArgs, SubscriptionCompletionPropTag property, out TOutputType outputValue)
		{
			outputValue = default(TOutputType);
			TOutputType toutputType;
			string text;
			if (RpcHelper.TryGetProperty<TOutputType>(inputArgs, (uint)property, out toutputType, out text))
			{
				outputValue = toutputType;
				this.syncLogSession.LogDebugging((TSLID)428UL, "Extracted for property {0}, value {1}.", new object[]
				{
					property,
					outputValue
				});
				return true;
			}
			this.syncLogSession.LogDebugging((TSLID)429UL, "Could not extract property {0}.", new object[]
			{
				property
			});
			return false;
		}

		private readonly GlobalSyncLogSession syncLogSession;
	}
}
