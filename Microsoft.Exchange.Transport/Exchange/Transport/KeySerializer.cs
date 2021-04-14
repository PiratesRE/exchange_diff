using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport
{
	internal static class KeySerializer
	{
		static KeySerializer()
		{
			KeySerializer.stringToKnownKeyMap = new Dictionary<string, KeySerializer.KnownKeyInformation>(KeySerializer.knownKeys.Length, StringComparer.OrdinalIgnoreCase);
			KeySerializer.shortToKnownKeyMap = new Dictionary<short, KeySerializer.KnownKeyInformation>(KeySerializer.knownKeys.Length);
			foreach (KeySerializer.KnownKeyInformation valueToAdd in KeySerializer.knownKeys)
			{
				TransportHelpers.AttemptAddToDictionary<string, KeySerializer.KnownKeyInformation>(KeySerializer.stringToKnownKeyMap, valueToAdd.Key, valueToAdd, null);
				TransportHelpers.AttemptAddToDictionary<short, KeySerializer.KnownKeyInformation>(KeySerializer.shortToKnownKeyMap, valueToAdd.UniqueMappedShortValue, valueToAdd, null);
			}
		}

		public static TypedValue Serialize(string originalKey)
		{
			if (string.IsNullOrEmpty(originalKey))
			{
				throw new ArgumentException("Empty key is not supported!", "originalKey");
			}
			KeySerializer.KnownKeyInformation knownKeyInformation;
			TypedValue result;
			if (KeySerializer.stringToKnownKeyMap.TryGetValue(originalKey, out knownKeyInformation))
			{
				result.Type = StreamPropertyType.Int16;
				result.Value = knownKeyInformation.UniqueMappedShortValue;
			}
			else
			{
				result.Type = StreamPropertyType.String;
				result.Value = KeySerializer.unknownKeyStringPool.Intern(originalKey);
			}
			return result;
		}

		public static string Deserialize(TypedValue key)
		{
			if (key.Type == StreamPropertyType.String)
			{
				string text = (string)key.Value;
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "KeySerializer.DeSerialize(): Unknown key '{0}'.  If not an agent key then it should be added to known keys in KeySerializer.cs", text);
				return KeySerializer.unknownKeyStringPool.Intern(text);
			}
			if (key.Type != StreamPropertyType.Int16)
			{
				throw new FormatException(string.Format(CultureInfo.InvariantCulture, "Corrupted extended property stream: invalid property type '{0}' for key.", new object[]
				{
					key.Type.ToString()
				}));
			}
			short num = (short)key.Value;
			KeySerializer.KnownKeyInformation knownKeyInformation;
			if (KeySerializer.shortToKnownKeyMap.TryGetValue(num, out knownKeyInformation))
			{
				return knownKeyInformation.Key;
			}
			ExTraceGlobals.GeneralTracer.TraceDebug<short>(0L, "KeySerializer.DeSerialize(): Previously known key that was mapped to {0} is not a known key anymore.", num);
			return null;
		}

		private static readonly KeySerializer.KnownKeyInformation[] knownKeys = new KeySerializer.KnownKeyInformation[]
		{
			new KeySerializer.KnownKeyInformation(0, "Microsoft.Exchange.ContentIdentifier"),
			new KeySerializer.KnownKeyInformation(1, "Microsoft.Exchange.Edge.RuleCollectionHistory"),
			new KeySerializer.KnownKeyInformation(2, "Microsoft.Exchange.Journaling.External"),
			new KeySerializer.KnownKeyInformation(3, "Microsoft.Exchange.Journaling.Internal"),
			new KeySerializer.KnownKeyInformation(4, "Microsoft.Exchange.Journaling.OriginalRecipientInfo"),
			new KeySerializer.KnownKeyInformation(5, "Microsoft.Exchange.Journaling.ProcessedOnRouted"),
			new KeySerializer.KnownKeyInformation(6, "Microsoft.Exchange.Journaling.ProcessedOnSubmitted"),
			new KeySerializer.KnownKeyInformation(7, "Microsoft.Exchange.Legacy.PassThru"),
			new KeySerializer.KnownKeyInformation(8, "Microsoft.Exchange.MapiDisplayName"),
			new KeySerializer.KnownKeyInformation(9, "Microsoft.Exchange.Transport.ClientRequestedInternetEncoding"),
			new KeySerializer.KnownKeyInformation(10, "Microsoft.Exchange.Transport.ClientRequestedSendRichInfo"),
			new KeySerializer.KnownKeyInformation(11, "Microsoft.Exchange.Transport.DirectoryData.Database"),
			new KeySerializer.KnownKeyInformation(12, "Microsoft.Exchange.Transport.DirectoryData.DeliverToMailboxAndForward"),
			new KeySerializer.KnownKeyInformation(13, "Microsoft.Exchange.Transport.DirectoryData.EmailAddresses"),
			new KeySerializer.KnownKeyInformation(14, "Microsoft.Exchange.Transport.DirectoryData.ExchangeGuid"),
			new KeySerializer.KnownKeyInformation(15, "Microsoft.Exchange.Transport.DirectoryData.ExternalEmailAddress"),
			new KeySerializer.KnownKeyInformation(16, "Microsoft.Exchange.Transport.DirectoryData.ForwardingAddress"),
			new KeySerializer.KnownKeyInformation(17, "Microsoft.Exchange.Transport.DirectoryData.InternetEncoding"),
			new KeySerializer.KnownKeyInformation(18, "Microsoft.Exchange.Transport.DirectoryData.LegacyExchangeDN"),
			new KeySerializer.KnownKeyInformation(19, "Microsoft.Exchange.Transport.DirectoryData.ManagedBy"),
			new KeySerializer.KnownKeyInformation(20, "Microsoft.Exchange.Transport.DirectoryData.ObjectGuid"),
			new KeySerializer.KnownKeyInformation(21, "Microsoft.Exchange.Transport.DirectoryData.RecipientType"),
			new KeySerializer.KnownKeyInformation(22, "Microsoft.Exchange.Transport.DirectoryData.RequireAllSendersAreAuthenticated"),
			new KeySerializer.KnownKeyInformation(23, "Microsoft.Exchange.Transport.DirectoryData.SendDeliveryReportsTo"),
			new KeySerializer.KnownKeyInformation(24, "Microsoft.Exchange.Transport.DirectoryData.Sender.DistinguishedName"),
			new KeySerializer.KnownKeyInformation(25, "Microsoft.Exchange.Transport.DirectoryData.Sender.ExternalOofOptions"),
			new KeySerializer.KnownKeyInformation(26, "Microsoft.Exchange.Transport.DirectoryData.Sender.Id"),
			new KeySerializer.KnownKeyInformation(27, "Microsoft.Exchange.Transport.DirectoryData.Sender.RecipientLimits"),
			new KeySerializer.KnownKeyInformation(28, "Microsoft.Exchange.Transport.DirectoryData.SendOofMessageToOriginator"),
			new KeySerializer.KnownKeyInformation(29, "Microsoft.Exchange.Transport.DirectoryData.ServerName"),
			new KeySerializer.KnownKeyInformation(30, "Microsoft.Exchange.Transport.DirectoryData.UseMapiRichTextFormat"),
			new KeySerializer.KnownKeyInformation(31, "Microsoft.Exchange.Transport.ElcJournalReport"),
			new KeySerializer.KnownKeyInformation(32, "Microsoft.Exchange.Transport.History"),
			new KeySerializer.KnownKeyInformation(33, "Microsoft.Exchange.Transport.Legacy.AlreadyForwarded"),
			new KeySerializer.KnownKeyInformation(34, "Microsoft.Exchange.Transport.MessageTemplate"),
			new KeySerializer.KnownKeyInformation(35, "Microsoft.Exchange.Transport.Processed"),
			new KeySerializer.KnownKeyInformation(36, "Microsoft.Exchange.Transport.RecipientP2Type"),
			new KeySerializer.KnownKeyInformation(37, "Microsoft.Exchange.Transport.ResentMapiMessage"),
			new KeySerializer.KnownKeyInformation(38, "Microsoft.Exchange.Transport.Resolved"),
			new KeySerializer.KnownKeyInformation(39, "Microsoft.Exchange.Transport.RoutingTimeStamp"),
			new KeySerializer.KnownKeyInformation(40, "Microsoft.Exchange.Transport.RuleCollectionHistory"),
			new KeySerializer.KnownKeyInformation(41, "Microsoft.Exchange.Transport.Sender.Resolved"),
			new KeySerializer.KnownKeyInformation(42, "Microsoft.Exchange.Transport.ServerLegacyDN"),
			new KeySerializer.KnownKeyInformation(43, "TransportRuleCollectionHistory"),
			new KeySerializer.KnownKeyInformation(44, "Microsoft.Exchange.Transport.Orar"),
			new KeySerializer.KnownKeyInformation(46, "Microsoft.Exchange.Transport.DirectoryData.PrimarySmtpAddress"),
			new KeySerializer.KnownKeyInformation(47, "Microsoft.Exchange.Transport.DirectoryData.DisplayName"),
			new KeySerializer.KnownKeyInformation(48, "Microsoft.Exchange.Transport.ResolverLogLevel"),
			new KeySerializer.KnownKeyInformation(49, "Microsoft.Exchange.Transport.DirectoryData.MaxReceiveSize"),
			new KeySerializer.KnownKeyInformation(50, "Microsoft.Exchange.Transport.DirectoryData.Sender.MaxSendSize"),
			new KeySerializer.KnownKeyInformation(51, "Microsoft.Exchange.Transport.RoutingOverride"),
			new KeySerializer.KnownKeyInformation(52, "Microsoft.Exchange.Transport.MailItemTracing"),
			new KeySerializer.KnownKeyInformation(53, "Microsoft.Exchange.IsAuthenticated"),
			new KeySerializer.KnownKeyInformation(54, "Microsoft.Exchange.JournalRecipientList"),
			new KeySerializer.KnownKeyInformation(55, "Microsoft.Exchange.SmtpMuaSubmission"),
			new KeySerializer.KnownKeyInformation(56, "Microsoft.Exchange.Transport.AgentDeferCount"),
			new KeySerializer.KnownKeyInformation(57, "Microsoft.Exchange.Transport.ResentMapiP2ToRecipients"),
			new KeySerializer.KnownKeyInformation(58, "Microsoft.Exchange.Transport.ResentMapiP2CcRecipients"),
			new KeySerializer.KnownKeyInformation(59, "ExSysMessage"),
			new KeySerializer.KnownKeyInformation(60, "Microsoft.Exchange.Transport.OpenDomainRoutingDisabled"),
			new KeySerializer.KnownKeyInformation(61, "Microsoft.Exchange.Transport.DirectoryData.AcceptMessagesOnlyFrom"),
			new KeySerializer.KnownKeyInformation(62, "Microsoft.Exchange.Transport.DirectoryData.AcceptMessagesOnlyFromDLMembers"),
			new KeySerializer.KnownKeyInformation(63, "Microsoft.Exchange.Transport.DirectoryData.RejectMessagesFrom"),
			new KeySerializer.KnownKeyInformation(64, "Microsoft.Exchange.Transport.DirectoryData.RejectMessagesFromDLMembers"),
			new KeySerializer.KnownKeyInformation(65, "Microsoft.Exchange.Transport.DirectoryData.BypassModerationFrom"),
			new KeySerializer.KnownKeyInformation(66, "Microsoft.Exchange.Transport.DirectoryData.BypassModerationFromDLMembers"),
			new KeySerializer.KnownKeyInformation(67, "Microsoft.Exchange.Transport.DirectoryData.ModerationEnabled"),
			new KeySerializer.KnownKeyInformation(68, "Microsoft.Exchange.Transport.DirectoryData.ModeratedBy"),
			new KeySerializer.KnownKeyInformation(69, "Microsoft.Exchange.Transport.DirectoryData.ArbitrationMailbox"),
			new KeySerializer.KnownKeyInformation(70, "Microsoft.Exchange.Transport.DirectoryData.SendModerationNotifications"),
			new KeySerializer.KnownKeyInformation(71, "Microsoft.Exchange.Transport.DirectoryData.BypassNestedModerationEnabled"),
			new KeySerializer.KnownKeyInformation(72, "Microsoft.Exchange.Transport.DirectoryData.HomeMtaServerId"),
			new KeySerializer.KnownKeyInformation(73, "Microsoft.Exchange.Transport.Categorizer.CategorizerComponent.MessageLatencyHeaderStamped"),
			new KeySerializer.KnownKeyInformation(74, "Microsoft.Exchange.Transport.DirectoryData.ForwardingSmtpAddress"),
			new KeySerializer.KnownKeyInformation(75, "Microsoft.Exchange.Transport.TransportMailItem.DeliveryPriority"),
			new KeySerializer.KnownKeyInformation(76, "Microsoft.Exchange.Transport.TransportMailItem.PrioritizationReason"),
			new KeySerializer.KnownKeyInformation(77, "Microsoft.Exchange.Transport.InboundTrustEnabled"),
			new KeySerializer.KnownKeyInformation(78, "Microsoft.Exchange.Transport.MailRecipient.TlsDomain"),
			new KeySerializer.KnownKeyInformation(79, "Microsoft.Exchange.Transport.MailRecipient.OriginatorOrganization"),
			new KeySerializer.KnownKeyInformation(80, "Microsoft.Exchange.Transport.Agent.OpenDomainRouting.MailFlowPartnerSettings.InternalMailContentType"),
			new KeySerializer.KnownKeyInformation(81, "Microsoft.Exchange.Transport.RecipientDiagnosticInfo"),
			new KeySerializer.KnownKeyInformation(82, "Microsoft.Exchange.Transport.GeneratedByMailboxRule"),
			new KeySerializer.KnownKeyInformation(83, "Microsoft.Exchange.Transport.AddedByTransportRule"),
			new KeySerializer.KnownKeyInformation(84, "Microsoft.Exchange.Transport.ModeratedByTransportRule"),
			new KeySerializer.KnownKeyInformation(85, "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmitted"),
			new KeySerializer.KnownKeyInformation(86, "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.ProcessedOnSubmittedForJournalNdr"),
			new KeySerializer.KnownKeyInformation(87, "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.SenderIsRecipient"),
			new KeySerializer.KnownKeyInformation(88, "Microsoft.Exchange.Transport.DirectoryData.Id"),
			new KeySerializer.KnownKeyInformation(89, "Microsoft.Exchange.Transport.MailRecipient.NetworkMessageId"),
			new KeySerializer.KnownKeyInformation(90, "Microsoft.Exchange.MessagingPolicies.UnJournalAgent.MessageOriginalDate"),
			new KeySerializer.KnownKeyInformation(91, "Microsoft.Exchange.Transport.MailRecipient.RequiredTlsAuthLevel"),
			new KeySerializer.KnownKeyInformation(92, "Microsoft.Exchange.Extensibility.Internal.TransportConstant.RoutedByTransportRule.Removed"),
			new KeySerializer.KnownKeyInformation(93, "Microsoft.Exchange.DsnGenerator.DsnSource"),
			new KeySerializer.KnownKeyInformation(94, "Microsoft.Exchange.Transport.TransportMailItem.ExternalOrganizationId"),
			new KeySerializer.KnownKeyInformation(95, "Microsoft.Exchange.Transport.TransportMailItem.SystemProbeId"),
			new KeySerializer.KnownKeyInformation(96, "Microsoft.Exchange.Transport.MailboxTransport.RetryOnDuplicateDelivery "),
			new KeySerializer.KnownKeyInformation(97, "Microsoft.Exchange.Transport.TransportMailItem.InboundProxySequenceNumber"),
			new KeySerializer.KnownKeyInformation(98, "Microsoft.Exchange.Transport.MailboxTransport.InternalMessageId"),
			new KeySerializer.KnownKeyInformation(99, "Microsoft.Exchange.Transport.SmtpInSessionId"),
			new KeySerializer.KnownKeyInformation(100, SmtpMessageContextBlob.ProcessTransportRoleKey),
			new KeySerializer.KnownKeyInformation(101, "Microsoft.Exchange.Transport.IsOneOffRecipient"),
			new KeySerializer.KnownKeyInformation(102, "Microsoft.Exchange.Transport.IsRemoteRecipient"),
			new KeySerializer.KnownKeyInformation(103, "Microsoft.Exchange.Transport.MailRecipient.AddressBookPolicy"),
			new KeySerializer.KnownKeyInformation(104, "Microsoft.Exchange.Transport.MailRecipient.OutboundIPPool"),
			new KeySerializer.KnownKeyInformation(105, "Microsoft.Exchange.Transport.Categorizer.CategorizerComponent.ResubmitCount"),
			new KeySerializer.KnownKeyInformation(106, "Microsoft.Exchange.Transport.DirectoryData.RecipientTypeDetailsRaw")
		};

		private static Dictionary<string, KeySerializer.KnownKeyInformation> stringToKnownKeyMap;

		private static Dictionary<short, KeySerializer.KnownKeyInformation> shortToKnownKeyMap;

		private static StringPool unknownKeyStringPool = new StringPool(StringComparer.OrdinalIgnoreCase);

		internal struct KnownKeyInformation
		{
			internal KnownKeyInformation(short uniqueMappedShortValue, string key)
			{
				this.uniqueMappedShortValue = uniqueMappedShortValue;
				this.stringKey = key;
			}

			public short UniqueMappedShortValue
			{
				get
				{
					return this.uniqueMappedShortValue;
				}
			}

			public string Key
			{
				get
				{
					return this.stringKey;
				}
			}

			private readonly short uniqueMappedShortValue;

			private readonly string stringKey;
		}
	}
}
