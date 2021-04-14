using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class StoreDriverDeliveryEventArgsImpl : StoreDriverDeliveryEventArgs
	{
		internal StoreDriverDeliveryEventArgsImpl(MailItemDeliver mailItemDeliver)
		{
			this.mailItemDeliver = mailItemDeliver;
		}

		public override DeliverableMailItem MailItem
		{
			get
			{
				return this.mailItemDeliver.MailItemWrapper;
			}
		}

		public override RoutingAddress RecipientAddress
		{
			get
			{
				if (this.MailRecipient == null)
				{
					return RoutingAddress.Empty;
				}
				return this.MailRecipient.Email;
			}
		}

		public override string MessageClass
		{
			get
			{
				return this.mailItemDeliver.MessageClass;
			}
		}

		internal MessageItem ReplayItem
		{
			get
			{
				return this.mailItemDeliver.ReplayItem;
			}
		}

		internal MessageItem MessageItem
		{
			get
			{
				return this.mailItemDeliver.DeliveryItem.Message;
			}
		}

		internal MailRecipient MailRecipient
		{
			get
			{
				return this.mailItemDeliver.Recipient;
			}
		}

		internal bool IsPublicFolderRecipient
		{
			get
			{
				return this.mailItemDeliver.IsPublicFolderRecipient;
			}
		}

		internal bool IsJournalReport
		{
			get
			{
				return this.mailItemDeliver.IsJournalReport;
			}
		}

		internal StoreSession StoreSession
		{
			get
			{
				return this.mailItemDeliver.DeliveryItem.Session;
			}
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				return this.mailItemDeliver.DeliveryItem.MailboxSession;
			}
		}

		internal PublicFolderSession PublicFolderSession
		{
			get
			{
				return this.mailItemDeliver.DeliveryItem.PublicFolderSession;
			}
		}

		internal ADRecipientCache<TransportMiniRecipient> ADRecipientCache
		{
			get
			{
				return (ADRecipientCache<TransportMiniRecipient>)this.MailItem.RecipientCache;
			}
		}

		internal MailItemDeliver MailItemDeliver
		{
			get
			{
				return this.mailItemDeliver;
			}
		}

		internal StoreId DeliverToFolder
		{
			get
			{
				return this.mailItemDeliver.DeliveryItem.DeliverToFolder;
			}
			set
			{
				this.mailItemDeliver.DeliveryItem.DeliverToFolder = value;
			}
		}

		internal string DeliverToFolderName
		{
			set
			{
				this.mailItemDeliver.DeliverToFolderName = value;
			}
		}

		internal object RetentionPolicyTag
		{
			get
			{
				return this.retentionPolicyTag;
			}
			set
			{
				this.retentionPolicyTag = value;
			}
		}

		internal MiniRecipient MailboxOwner
		{
			get
			{
				ProxyAddress proxyAddress = new SmtpProxyAddress((string)this.MailRecipient.Email, true);
				return this.ADRecipientCache.FindAndCacheRecipient(proxyAddress).Data;
			}
		}

		internal object RetentionPeriod
		{
			get
			{
				return this.retentionPeriod;
			}
			set
			{
				this.retentionPeriod = value;
			}
		}

		internal object RetentionFlags
		{
			get
			{
				return this.retentionFlags;
			}
			set
			{
				this.retentionFlags = value;
			}
		}

		internal object ArchiveTag
		{
			get
			{
				return this.archiveTag;
			}
			set
			{
				this.archiveTag = value;
			}
		}

		internal object ArchivePeriod
		{
			get
			{
				return this.archivePeriod;
			}
			set
			{
				this.archivePeriod = value;
			}
		}

		internal object CompactDefaultRetentionPolicy
		{
			get
			{
				return this.compactDefaultRetentionPolicy;
			}
			set
			{
				this.compactDefaultRetentionPolicy = value;
			}
		}

		internal bool ShouldSkipMoveRule
		{
			get
			{
				return this.shouldSkipMoveRule;
			}
			set
			{
				this.shouldSkipMoveRule = value;
			}
		}

		internal bool ShouldRunMailboxRulesBasedOnDeliveryFolder
		{
			get
			{
				return this.shouldRunMailboxRulesBasedOnDeliveryFolder;
			}
			set
			{
				this.shouldRunMailboxRulesBasedOnDeliveryFolder = value;
			}
		}

		internal Dictionary<PropertyDefinition, object> PropertiesForAllMessageCopies
		{
			get
			{
				return this.propertiesForAllMessageCopies;
			}
			set
			{
				this.propertiesForAllMessageCopies = value;
			}
		}

		internal Dictionary<PropertyDefinition, object> PropertiesForDelegateForward
		{
			get
			{
				return this.propertiesForDelegateForward;
			}
			set
			{
				this.propertiesForDelegateForward = value;
			}
		}

		internal bool ShouldCreateItemForDelete
		{
			get
			{
				return this.shouldCreateItemForDelete;
			}
			set
			{
				this.shouldCreateItemForDelete = value;
			}
		}

		internal SmtpResponse BounceSmtpResponse
		{
			get
			{
				return this.bounceSmtpResponse;
			}
			set
			{
				this.bounceSmtpResponse = value;
			}
		}

		internal string BounceSource
		{
			get
			{
				return this.bounceSource;
			}
			set
			{
				this.bounceSource = value;
			}
		}

		internal Dictionary<PropertyDefinition, object> SharedPropertiesBetweenAgents
		{
			get
			{
				return this.sharedPropertiesBetweenAgents;
			}
			set
			{
				this.sharedPropertiesBetweenAgents = value;
			}
		}

		public void AddDeliveryErrors(List<string> deliveryErrors)
		{
			this.mailItemDeliver.AddDeliveryErrors(deliveryErrors);
		}

		public void SetDeliveryFolder(Folder folder)
		{
			this.mailItemDeliver.SetDeliveryFolder(folder);
		}

		public override void AddAgentInfo(string agentName, string eventName, List<KeyValuePair<string, string>> data)
		{
			this.MailItemDeliver.MbxTransportMailItem.AddAgentInfo(agentName, eventName, data);
		}

		internal void ResetPerRecipientState()
		{
			this.PropertiesForAllMessageCopies = null;
			this.PropertiesForDelegateForward = null;
			this.SharedPropertiesBetweenAgents = null;
			this.ShouldSkipMoveRule = false;
		}

		private MailItemDeliver mailItemDeliver;

		private Dictionary<PropertyDefinition, object> propertiesForAllMessageCopies;

		private Dictionary<PropertyDefinition, object> propertiesForDelegateForward;

		private bool shouldSkipMoveRule;

		private bool shouldRunMailboxRulesBasedOnDeliveryFolder;

		private object retentionPolicyTag;

		private object retentionPeriod;

		private object retentionFlags;

		private object archiveTag;

		private object archivePeriod;

		private object compactDefaultRetentionPolicy;

		private bool shouldCreateItemForDelete;

		private SmtpResponse bounceSmtpResponse;

		private string bounceSource;

		private Dictionary<PropertyDefinition, object> sharedPropertiesBetweenAgents;
	}
}
