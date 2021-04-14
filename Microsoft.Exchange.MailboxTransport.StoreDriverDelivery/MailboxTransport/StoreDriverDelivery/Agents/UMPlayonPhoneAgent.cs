using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal sealed class UMPlayonPhoneAgent : StoreDriverDeliveryAgent
	{
		public UMPlayonPhoneAgent(SmtpServer server)
		{
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
		}

		public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs e)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)e;
			if (storeDriverDeliveryEventArgsImpl.IsPublicFolderRecipient || storeDriverDeliveryEventArgsImpl.IsJournalReport)
			{
				UMPlayonPhoneAgent.Tracer.TraceError((long)this.GetHashCode(), "not supported for public folder or journal reports");
				return;
			}
			if (!storeDriverDeliveryEventArgsImpl.ReplayItem.IsRestricted || !storeDriverDeliveryEventArgsImpl.ReplayItem.ClassName.StartsWith("IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			UMMailboxPolicy ummailboxPolicy;
			if (!UMAgentUtil.TryGetUMMailboxPolicy(UMPlayonPhoneAgent.Tracer, storeDriverDeliveryEventArgsImpl.ADRecipientCache, storeDriverDeliveryEventArgsImpl.MailRecipient, out ummailboxPolicy))
			{
				return;
			}
			if (storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies == null)
			{
				storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies = new Dictionary<PropertyDefinition, object>();
			}
			if (ummailboxPolicy.RequireProtectedPlayOnPhone)
			{
				storeDriverDeliveryEventArgsImpl.PropertiesForAllMessageCopies[MessageItemSchema.RequireProtectedPlayOnPhone] = "true";
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.UMPlayonPhoneAgentTracer;
	}
}
