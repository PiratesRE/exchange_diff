using System;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal static class TransportFacades
	{
		public static Dns Dns
		{
			get
			{
				return TransportFacades.enhancedDns;
			}
		}

		public static ICategorizerComponentFacade CategorizerComponent
		{
			get
			{
				return TransportFacades.categorizerComponent;
			}
		}

		public static bool IsVoicemail(EmailMessage message)
		{
			if (message != null && message.MapiMessageClass != null)
			{
				string mapiMessageClass = message.MapiMessageClass;
				return mapiMessageClass.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase) || mapiMessageClass.Equals("IPM.Note.rpmsg.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase) || mapiMessageClass.Equals("IPM.Note.Microsoft.Voicemail.UM", StringComparison.OrdinalIgnoreCase) || mapiMessageClass.Equals("IPM.Note.Microsoft.Voicemail.UM.CA", StringComparison.OrdinalIgnoreCase) || mapiMessageClass.Equals("IPM.Note.Microsoft.Conversation.Voice", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}

		public static IShadowRedundancyComponent ShadowRedundancyComponent
		{
			get
			{
				return TransportFacades.shadowRedundancyComponent;
			}
		}

		public static bool IsStopping
		{
			get
			{
				return TransportFacades.isStopping;
			}
		}

		public static event EventHandler ConfigChanged
		{
			add
			{
				TransportFacades.configChangedDelegate = (EventHandler)Delegate.Combine(TransportFacades.configChangedDelegate, value);
			}
			remove
			{
				TransportFacades.configChangedDelegate = (EventHandler)Delegate.Remove(TransportFacades.configChangedDelegate, value);
			}
		}

		public static ITransportMailItemFacade NewMailItem(ITransportMailItemFacade originalMailItem)
		{
			ITransportMailItemFacade transportMailItemFacade = TransportFacades.newMailItemDelegate(originalMailItem);
			IShadowRedundancyManagerFacade shadowRedundancyManager = TransportFacades.shadowRedundancyComponent.ShadowRedundancyManager;
			shadowRedundancyManager.LinkSideEffectMailItemIfNeeded(originalMailItem, transportMailItemFacade);
			return transportMailItemFacade;
		}

		public static void EnsureSecurityAttributes(ITransportMailItemFacade mailItem)
		{
			TransportFacades.ensureSecurityAttributesDelegate(mailItem);
		}

		internal static IHistoryFacade ReadHistoryFrom(ITransportMailItemFacade mailItem)
		{
			return TransportFacades.readHistoryFromMailItemByAgentDelegate(mailItem);
		}

		internal static IHistoryFacade ReadHistoryFrom(IMailRecipientFacade recipient)
		{
			return TransportFacades.readHistoryFromRecipientByAgentDelegate(recipient);
		}

		public static void TrackReceiveByAgent(ITransportMailItemFacade mailItem, string sourceContext, string connectorId, long? relatedMailItemId)
		{
			TransportFacades.trackReceiveByAgentDelegate(mailItem, sourceContext, connectorId, relatedMailItemId);
		}

		public static void TrackRecipientAddByAgent(ITransportMailItemFacade mailItem, string recipEmail, RecipientP2Type recipientType, string agentName)
		{
			TransportFacades.trackRecipientAddByAgentDelegate(mailItem, recipEmail, recipientType, agentName);
		}

		public static SmtpResponse CreateAndSubmitApprovalInitiationForTransportRules(ITransportMailItemFacade transportMailItemFacade, string originalSenderAddress, string approverAddresses, string transportRuleName)
		{
			return TransportFacades.createAndSubmitApprovalInitiationForTransportRulesDelegate(transportMailItemFacade, originalSenderAddress, approverAddresses, transportRuleName);
		}

		internal static void Initialize(Dns enhancedDns, ICategorizerComponentFacade categorizerComponent, IShadowRedundancyComponent shadowRedundancyComponent, EventHandler configChangedDelegate, NewMailItemDelegate newMailItemDelegate, EnsureSecurityAttributesDelegate ensureSecurityAttributesDelegate, TrackReceiveByAgentDelegate trackReceiveByAgentDelegate, TrackRecipientAddByAgentDelegate trackRecipientAddByAgentDelegate, ReadHistoryFromMailItemByAgentDelegate readHistoryFromMailItemByAgentDelegate, ReadHistoryFromRecipientByAgentDelegate readHistoryFromRecipientByAgentDelegate, CreateAndSubmitApprovalInitiationForTransportRulesDelegate createAndSubmitApprovalInitiationForTransportRulesDelegate)
		{
			TransportFacades.enhancedDns = enhancedDns;
			TransportFacades.categorizerComponent = categorizerComponent;
			TransportFacades.shadowRedundancyComponent = shadowRedundancyComponent;
			TransportFacades.configChangedDelegate = configChangedDelegate;
			TransportFacades.newMailItemDelegate = newMailItemDelegate;
			TransportFacades.ensureSecurityAttributesDelegate = ensureSecurityAttributesDelegate;
			TransportFacades.trackReceiveByAgentDelegate = trackReceiveByAgentDelegate;
			TransportFacades.trackRecipientAddByAgentDelegate = trackRecipientAddByAgentDelegate;
			TransportFacades.readHistoryFromMailItemByAgentDelegate = readHistoryFromMailItemByAgentDelegate;
			TransportFacades.readHistoryFromRecipientByAgentDelegate = readHistoryFromRecipientByAgentDelegate;
			TransportFacades.createAndSubmitApprovalInitiationForTransportRulesDelegate = createAndSubmitApprovalInitiationForTransportRulesDelegate;
		}

		internal static void Stop()
		{
			TransportFacades.isStopping = true;
		}

		private static Dns enhancedDns;

		private static ICategorizerComponentFacade categorizerComponent;

		private static IShadowRedundancyComponent shadowRedundancyComponent;

		private static EventHandler configChangedDelegate;

		private static NewMailItemDelegate newMailItemDelegate;

		private static EnsureSecurityAttributesDelegate ensureSecurityAttributesDelegate;

		private static TrackReceiveByAgentDelegate trackReceiveByAgentDelegate;

		private static TrackRecipientAddByAgentDelegate trackRecipientAddByAgentDelegate;

		private static ReadHistoryFromMailItemByAgentDelegate readHistoryFromMailItemByAgentDelegate;

		private static ReadHistoryFromRecipientByAgentDelegate readHistoryFromRecipientByAgentDelegate;

		private static CreateAndSubmitApprovalInitiationForTransportRulesDelegate createAndSubmitApprovalInitiationForTransportRulesDelegate;

		private static bool isStopping;
	}
}
