using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal interface IDeliveryItem
	{
		bool HasMessage { get; }

		MessageItem Message { get; }

		StoreSession Session { get; }

		StoreId DeliverToFolder { get; set; }

		MailboxSession MailboxSession { get; }

		PublicFolderSession PublicFolderSession { get; }

		DisposeTracker DisposeTracker { get; }

		void Deliver(ProxyAddress recipientProxyAddress);

		void SetProperty(PropertyDefinition property, object value);

		void DeleteProperty(PropertyDefinition property);

		Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode);

		void CreatePublicFolderMessage(MailRecipient recipient, DeliverableItem item);

		void CreateSession(MailRecipient recipient, OpenTransportSessionFlags deliveryFlags, DeliverableItem item, ICollection<CultureInfo> recipientLanguages);

		void CreateMailboxMessage(bool leaveReceivedTime);

		void LoadMailboxMessage(string internetMessageId);

		void DisposeMessageAndSession();

		void Dispose();
	}
}
