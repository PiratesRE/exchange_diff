using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Connect;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SerializedSubscription
	{
		private SerializedSubscription(AggregationSubscriptionType subscriptionType, long version, byte[] serializedData)
		{
			this.subscriptionType = subscriptionType;
			this.version = version;
			this.serializedData = serializedData;
		}

		public long Version
		{
			get
			{
				return this.version;
			}
		}

		public static SerializedSubscription FromSubscription(AggregationSubscription subscription)
		{
			SerializedSubscription result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					subscription.Serialize(binaryWriter);
				}
				SerializedSubscription serializedSubscription = new SerializedSubscription(subscription.SubscriptionType, subscription.Version, memoryStream.ToArray());
				result = serializedSubscription;
			}
			return result;
		}

		public static SerializedSubscription FromReader(BinaryReader reader)
		{
			int count = reader.ReadInt32();
			byte[] array = reader.ReadBytes(count);
			return SerializedSubscription.FromBytes(array);
		}

		public static SerializedSubscription FromBytes(byte[] serializedData)
		{
			long num;
			AggregationSubscriptionType aggregationSubscriptionType;
			using (MemoryStream memoryStream = new MemoryStream(serializedData))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					AggregationSubscription.DeserializeVersionAndSubscriptionType(binaryReader, out num, out aggregationSubscriptionType);
				}
			}
			return new SerializedSubscription(aggregationSubscriptionType, num, serializedData);
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(this.serializedData.Length);
			writer.Write(this.serializedData);
		}

		public byte[] GetBytes()
		{
			return this.serializedData;
		}

		public bool TryDeserializeSubscription(out AggregationSubscription subscription)
		{
			bool result;
			try
			{
				subscription = this.DeserializeSubscription();
				result = true;
			}
			catch (SerializationException ex)
			{
				CommonLoggingHelper.SyncLogSession.LogError((TSLID)155UL, string.Format(CultureInfo.InvariantCulture, "Exception encountered while deserializing the subscription: {0}", new object[]
				{
					ex
				}), new object[0]);
				subscription = null;
				result = false;
			}
			return result;
		}

		public AggregationSubscription DeserializeSubscription()
		{
			if (this.version != AggregationSubscription.CurrentSupportedVersion)
			{
				throw new SerializationException("Serialized subscription is an unsupported version");
			}
			AggregationSubscription aggregationSubscription;
			if (this.subscriptionType == AggregationSubscriptionType.Pop)
			{
				aggregationSubscription = new PopAggregationSubscription();
			}
			else if (this.subscriptionType == AggregationSubscriptionType.IMAP)
			{
				aggregationSubscription = new IMAPAggregationSubscription();
			}
			else if (this.subscriptionType == AggregationSubscriptionType.DeltaSyncMail)
			{
				aggregationSubscription = new DeltaSyncAggregationSubscription();
			}
			else if (this.subscriptionType == AggregationSubscriptionType.Facebook)
			{
				aggregationSubscription = new ConnectSubscription();
			}
			else
			{
				if (this.subscriptionType != AggregationSubscriptionType.LinkedIn)
				{
					throw new InvalidOperationException("unsupported subscription type");
				}
				aggregationSubscription = new ConnectSubscription();
			}
			using (MemoryStream memoryStream = new MemoryStream(this.serializedData))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					aggregationSubscription.Deserialize(binaryReader);
				}
			}
			return aggregationSubscription;
		}

		private readonly AggregationSubscriptionType subscriptionType;

		private readonly long version;

		private readonly byte[] serializedData;
	}
}
