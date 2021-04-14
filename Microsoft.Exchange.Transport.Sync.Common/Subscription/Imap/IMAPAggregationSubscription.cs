using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class IMAPAggregationSubscription : PimAggregationSubscription
	{
		public IMAPAggregationSubscription()
		{
			this.IMAPPort = 143;
			base.SubscriptionProtocolName = "IMAP";
			base.SubscriptionProtocolVersion = 1;
			base.SubscriptionType = AggregationSubscriptionType.IMAP;
		}

		public override string IncomingServerName
		{
			get
			{
				return this.imapServer;
			}
		}

		public override int IncomingServerPort
		{
			get
			{
				return this.imapPort;
			}
		}

		public override string AuthenticationType
		{
			get
			{
				return this.IMAPAuthentication.ToString();
			}
		}

		public override string EncryptionType
		{
			get
			{
				return this.IMAPSecurity.ToString();
			}
		}

		public Fqdn IMAPServer
		{
			get
			{
				return this.imapServer;
			}
			set
			{
				this.imapServer = value;
			}
		}

		public int IMAPPort
		{
			get
			{
				return this.imapPort;
			}
			set
			{
				this.imapPort = value;
			}
		}

		public string IMAPLogOnName
		{
			get
			{
				return this.imapLogOnName;
			}
			set
			{
				this.imapLogOnName = value;
			}
		}

		public IMAPSecurityMechanism IMAPSecurity
		{
			get
			{
				return (IMAPSecurityMechanism)(this.flags & IMAPAggregationFlags.SecurityMask);
			}
			set
			{
				this.flags &= ~(IMAPAggregationFlags.UseSsl | IMAPAggregationFlags.UseTls);
				this.flags |= (IMAPAggregationFlags)value;
			}
		}

		public IMAPAuthenticationMechanism IMAPAuthentication
		{
			get
			{
				return (IMAPAuthenticationMechanism)(this.flags & IMAPAggregationFlags.UseNtlmAuth);
			}
			set
			{
				this.flags &= ~IMAPAggregationFlags.UseNtlmAuth;
				this.flags |= (IMAPAggregationFlags)value;
			}
		}

		public IMAPAggregationFlags Flags
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
				return this.IMAPLogOnName;
			}
		}

		public override string VerifiedIncomingServer
		{
			get
			{
				return this.IMAPServer;
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
				return FolderSupport.FullHierarchy;
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
				return SyncQuirks.EnumerateItemChangeAsDeleteAndAdd | SyncQuirks.OnlyDeleteFoldersIfNoSubFolders | SyncQuirks.AllowDirectCloudFolderUpdates;
			}
		}

		public string ImapPathPrefix
		{
			get
			{
				return this.imapPathPrefix;
			}
			set
			{
				this.imapPathPrefix = value;
			}
		}

		public override PimSubscriptionProxy CreateSubscriptionProxy()
		{
			return new IMAPSubscriptionProxy(this);
		}

		protected override void SetPropertiesToMessageObject(MessageItem message)
		{
			base.SetPropertiesToMessageObject(message);
			message[StoreObjectSchema.ItemClass] = "IPM.Aggregation.IMAP";
			message[MessageItemSchema.SharingProviderGuid] = IMAPAggregationSubscription.IMAPProviderGuid;
			message[MessageItemSchema.SharingRemotePath] = this.IMAPServer.ToString() + ":" + this.IMAPPort.ToString(CultureInfo.InvariantCulture);
			message[AggregationSubscriptionMessageSchema.SharingRemoteUser] = this.IMAPLogOnName;
			message[MessageItemSchema.SharingDetail] = (int)(this.IMAPSecurity | (IMAPSecurityMechanism)this.IMAPAuthentication);
			if (string.IsNullOrEmpty(this.imapPathPrefix))
			{
				message.Delete(AggregationSubscriptionMessageSchema.SharingImapPathPrefix);
				return;
			}
			message[AggregationSubscriptionMessageSchema.SharingImapPathPrefix] = this.imapPathPrefix;
		}

		protected override void LoadProperties(MessageItem message)
		{
			base.LoadProperties(message);
			base.GetFqdnProperty(message, MessageItemSchema.SharingRemotePath, out this.imapServer, out this.imapPort);
			base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingRemoteUser, false, new uint?(0U), new uint?(256U), out this.imapLogOnName);
			base.GetEnumProperty<IMAPAggregationFlags>(message, MessageItemSchema.SharingDetail, null, out this.flags);
			if (base.Version >= 6L)
			{
				base.GetStringProperty(message, AggregationSubscriptionMessageSchema.SharingImapPathPrefix, true, true, new uint?(0U), new uint?(256U), out this.imapPathPrefix);
				return;
			}
			this.imapPathPrefix = null;
		}

		protected override void Serialize(AggregationSubscription.SubscriptionSerializer subscriptionSerializer)
		{
			subscriptionSerializer.SerializeImapSubscription(this);
		}

		protected override void Deserialize(AggregationSubscription.SubscriptionDeserializer deserializer)
		{
			deserializer.DeserializeImapSubscription(this);
		}

		private const string IMAPProtocolName = "IMAP";

		private const int IMAPProtocolVersion = 1;

		private const int DefaultIMAPPort = 143;

		private static readonly Guid IMAPProviderGuid = new Guid("E59E7F14-1FDF-41d4-BDAB-DF7F95BF8F47");

		private Fqdn imapServer;

		private int imapPort;

		private string imapLogOnName;

		private IMAPAggregationFlags flags;

		private string imapPathPrefix;
	}
}
