using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class PopAggregationSubscription : PimAggregationSubscription
	{
		public PopAggregationSubscription()
		{
			this.PopPort = 110;
			this.flags = PopAggregationFlags.LeaveOnServer;
			base.SubscriptionProtocolName = "POP";
			base.SubscriptionProtocolVersion = 196608;
			base.SubscriptionType = AggregationSubscriptionType.Pop;
		}

		public override string IncomingServerName
		{
			get
			{
				return this.popServer;
			}
		}

		public override int IncomingServerPort
		{
			get
			{
				return this.popPort;
			}
		}

		public override string AuthenticationType
		{
			get
			{
				return this.PopAuthentication.ToString();
			}
		}

		public override string EncryptionType
		{
			get
			{
				return this.PopSecurity.ToString();
			}
		}

		public Fqdn PopServer
		{
			get
			{
				return this.popServer;
			}
			set
			{
				this.popServer = value;
			}
		}

		public int PopPort
		{
			get
			{
				return this.popPort;
			}
			set
			{
				this.popPort = value;
			}
		}

		public string PopLogonName
		{
			get
			{
				return this.popLogonName;
			}
			set
			{
				this.popLogonName = value;
			}
		}

		public SecurityMechanism PopSecurity
		{
			get
			{
				return (SecurityMechanism)(this.flags & PopAggregationFlags.SecurityMask);
			}
			set
			{
				this.flags &= ~(PopAggregationFlags.UseSsl | PopAggregationFlags.UseTls);
				this.flags |= (PopAggregationFlags)value;
			}
		}

		public AuthenticationMechanism PopAuthentication
		{
			get
			{
				return (AuthenticationMechanism)(this.flags & PopAggregationFlags.AuthenticationMask);
			}
			set
			{
				this.flags &= (PopAggregationFlags)(-28673);
				this.flags |= (PopAggregationFlags)value;
			}
		}

		public bool ShouldSyncDelete
		{
			get
			{
				return false;
			}
		}

		public bool ShouldLeaveOnServer
		{
			get
			{
				return PopAggregationFlags.UseBasicAuth < (this.flags & PopAggregationFlags.LeaveOnServer);
			}
			set
			{
				if (value)
				{
					this.flags |= PopAggregationFlags.LeaveOnServer;
					return;
				}
				this.flags &= ~PopAggregationFlags.LeaveOnServer;
			}
		}

		public PopAggregationFlags Flags
		{
			get
			{
				return this.flags;
			}
			internal set
			{
				this.flags = value;
			}
		}

		public override string VerifiedUserName
		{
			get
			{
				return this.PopLogonName;
			}
		}

		public override string VerifiedIncomingServer
		{
			get
			{
				return this.PopServer;
			}
		}

		public override bool SendAsNeedsVerification
		{
			get
			{
				return true;
			}
		}

		public override FolderSupport FolderSupport
		{
			get
			{
				return FolderSupport.InboxOnly;
			}
		}

		public override ItemSupport ItemSupport
		{
			get
			{
				return ItemSupport.Email | ItemSupport.Generic;
			}
		}

		public override SyncQuirks SyncQuirks
		{
			get
			{
				return SyncQuirks.EnumerateNativeDeletesOnly;
			}
		}

		public override PimSubscriptionProxy CreateSubscriptionProxy()
		{
			return new PopSubscriptionProxy(this);
		}

		protected override void SetPropertiesToMessageObject(MessageItem message)
		{
			base.SetPropertiesToMessageObject(message);
			message[StoreObjectSchema.ItemClass] = "IPM.Aggregation.Pop";
			message[MessageItemSchema.SharingProviderGuid] = PopAggregationSubscription.PopProviderGuid;
			message[MessageItemSchema.SharingRemotePath] = this.PopServer.ToString() + ":" + this.PopPort.ToString();
			message[AggregationSubscriptionMessageSchema.SharingRemoteUser] = this.PopLogonName;
			message[MessageItemSchema.SharingDetail] = (int)this.Flags;
		}

		protected override void LoadProperties(MessageItem message)
		{
			base.LoadProperties(message);
			base.GetFqdnProperty(message, MessageItemSchema.SharingRemotePath, out this.popServer, out this.popPort);
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingRemoteUser, false, new uint?(0U), new uint?(256U), out this.popLogonName);
			base.GetEnumProperty<PopAggregationFlags>(message, MessageItemSchema.SharingDetail, null, out this.flags);
		}

		protected override void Serialize(AggregationSubscription.SubscriptionSerializer subscriptionSerializer)
		{
			subscriptionSerializer.SerializePopSubscription(this);
		}

		protected override void Deserialize(AggregationSubscription.SubscriptionDeserializer deserializer)
		{
			deserializer.DeserializePopSubscription(this);
		}

		private const string PopProtocolName = "POP";

		private const int PopProtocolVersion = 196608;

		private static readonly Guid PopProviderGuid = new Guid("1df33d70-5bf4-4bf2-844c-afe75ff140fb");

		private Fqdn popServer;

		private int popPort;

		private string popLogonName;

		private PopAggregationFlags flags;
	}
}
