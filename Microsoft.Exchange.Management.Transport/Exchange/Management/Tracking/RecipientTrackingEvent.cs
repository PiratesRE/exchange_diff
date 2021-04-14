using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.Tracking;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.Management.Tracking
{
	[Serializable]
	public class RecipientTrackingEvent : MessageTrackingConfigurableObject
	{
		private static RecipientTrackingEvent GetIntermediateTransferredEvent(RecipientTrackingEvent originalInternalEvent)
		{
			if (originalInternalEvent.EventDescription != Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.TransferredToPartnerOrg && originalInternalEvent.EventDescription != Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.SmtpSendCrossForest)
			{
				throw new ArgumentException("GetIntermediateTransferredEvent can only be called for TransferredToPartnerOrg or SmtpSendCrossForest");
			}
			return new RecipientTrackingEvent(CoreStrings.EventTransferredIntermediate.ToString(Thread.CurrentThread.CurrentCulture), originalInternalEvent.EventDescription, originalInternalEvent.InternalMessageId, originalInternalEvent.BccRecipient, originalInternalEvent.Date, _DeliveryStatus.Transferred, null, EventType.Transferred, originalInternalEvent.RecipientAddress, originalInternalEvent.RecipientDisplayName, originalInternalEvent.Server);
		}

		public DateTime Date
		{
			get
			{
				return (DateTime)this[RecipientTrackingEventSchema.Date];
			}
		}

		public SmtpAddress RecipientAddress
		{
			get
			{
				return (SmtpAddress)this[RecipientTrackingEventSchema.RecipientAddress];
			}
			internal set
			{
				this[RecipientTrackingEventSchema.RecipientAddress] = value;
			}
		}

		public string RecipientDisplayName
		{
			get
			{
				return (string)this[RecipientTrackingEventSchema.RecipientDisplayName];
			}
			internal set
			{
				this[RecipientTrackingEventSchema.RecipientDisplayName] = value;
			}
		}

		public _DeliveryStatus Status
		{
			get
			{
				return (_DeliveryStatus)this[RecipientTrackingEventSchema.DeliveryStatus];
			}
		}

		public EventType EventType
		{
			get
			{
				return (EventType)this[RecipientTrackingEventSchema.EventTypeValue];
			}
			private set
			{
				this[RecipientTrackingEventSchema.EventTypeValue] = value;
			}
		}

		public string EventDescription
		{
			get
			{
				return (string)this[RecipientTrackingEventSchema.EventDescription];
			}
		}

		public string[] EventData
		{
			get
			{
				return (string[])this[RecipientTrackingEventSchema.EventData];
			}
			internal set
			{
				this[RecipientTrackingEventSchema.EventData] = value;
			}
		}

		public string Server
		{
			get
			{
				return (string)this[RecipientTrackingEventSchema.Server];
			}
			internal set
			{
				this[RecipientTrackingEventSchema.Server] = value;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0},{1},{2},{3}", new object[]
			{
				this.RecipientAddress,
				Names<Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription>.Map[(int)this.eventDescriptionEnum],
				this.Server,
				this.Date.ToString("o", CultureInfo.InvariantCulture)
			});
			if (this.EventData != null && this.EventData.Length > 0)
			{
				stringBuilder.Append(",");
				stringBuilder.Append("Data=");
				for (int i = 0; i < this.EventData.Length; i++)
				{
					stringBuilder.Append(this.EventData[i]);
					if (i != this.EventData.Length - 1)
					{
						stringBuilder.Append(';');
					}
				}
			}
			return stringBuilder.ToString();
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return RecipientTrackingEvent.schema;
			}
		}

		internal long InternalMessageId
		{
			get
			{
				return this.internalMessageId;
			}
		}

		internal bool BccRecipient
		{
			get
			{
				return this.bccRecipient;
			}
		}

		internal EventDescription EventDescriptionEnum
		{
			get
			{
				return this.eventDescriptionEnum;
			}
		}

		internal static RecipientTrackingEvent Create(bool isLastKnownStatus, IConfigurationSession session, MultiValuedProperty<CultureInfo> userLanguages, bool returnHelpDeskMessages, RecipientTrackingEvent internalRecipientTrackingEvent)
		{
			if (isLastKnownStatus && (internalRecipientTrackingEvent.EventDescription == Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.SmtpSendCrossForest || internalRecipientTrackingEvent.EventDescription == Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.TransferredToPartnerOrg))
			{
				return RecipientTrackingEvent.GetIntermediateTransferredEvent(internalRecipientTrackingEvent);
			}
			RecipientTrackingEvent.FormatterMethod[] array = returnHelpDeskMessages ? RecipientTrackingEvent.helpDeskFormatters : RecipientTrackingEvent.iWFormatters;
			RecipientTrackingEvent.FormatterSource source = new RecipientTrackingEvent.FormatterSource(session, userLanguages, internalRecipientTrackingEvent);
			RecipientTrackingEvent.FormatterMethod formatterMethod = array[(int)internalRecipientTrackingEvent.EventDescription];
			if (formatterMethod != null)
			{
				LocalizedString localizedString;
				try
				{
					localizedString = formatterMethod(source, internalRecipientTrackingEvent.EventData);
				}
				catch (FormatException ex)
				{
					ExTraceGlobals.SearchLibraryTracer.TraceError(0L, ex.Message);
					return null;
				}
				string eventDescription = localizedString.ToString(Thread.CurrentThread.CurrentCulture);
				RecipientTrackingEvent recipientTrackingEvent = new RecipientTrackingEvent(internalRecipientTrackingEvent, eventDescription);
				EventType eventType;
				if (RecipientTrackingEvent.descriptionToTypeMap.TryGetValue(internalRecipientTrackingEvent.EventDescription, out eventType))
				{
					recipientTrackingEvent.EventType = eventType;
				}
				else
				{
					recipientTrackingEvent.EventType = EventType.Pending;
				}
				return recipientTrackingEvent;
			}
			return null;
		}

		internal static RecipientTrackingEvent GetExplanatoryMessage(IList<RecipientTrackingEvent> events)
		{
			if (events.Count == 0)
			{
				return null;
			}
			DateTime utcNow = DateTime.UtcNow;
			RecipientTrackingEvent recipientTrackingEvent = events[0];
			RecipientTrackingEvent recipientTrackingEvent2 = events[events.Count - 1];
			if (recipientTrackingEvent2.EventDescription == Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.SmtpSendCrossSite || recipientTrackingEvent2.EventDescription == Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.SmtpSend)
			{
				if (utcNow > recipientTrackingEvent2.Date && utcNow - recipientTrackingEvent2.Date > RecipientTrackingEvent.SmtpHandshakeMaximumSkew)
				{
					return new RecipientTrackingEvent(CoreStrings.TrackingExplanationLogsDeleted.ToString(Thread.CurrentThread.CurrentCulture), Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.Pending, recipientTrackingEvent2.InternalMessageId, recipientTrackingEvent2.BccRecipient, utcNow, _DeliveryStatus.Pending, null, EventType.Pending, recipientTrackingEvent2.RecipientAddress, recipientTrackingEvent2.RecipientDisplayName, recipientTrackingEvent2.Server);
				}
			}
			else if (recipientTrackingEvent2.Status == DeliveryStatus.Pending)
			{
				if (recipientTrackingEvent2.EventDescription == Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.PendingModeration || recipientTrackingEvent2.EventDescription == Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.SmtpSendCrossForest || recipientTrackingEvent2.EventDescription == Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.TransferredToPartnerOrg)
				{
					return null;
				}
				TimeSpan t = utcNow.Subtract(recipientTrackingEvent.Date);
				if (t < RecipientTrackingEvent.MaximumDelayForNormalMessage)
				{
					if (recipientTrackingEvent2.EventDescription == Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.QueueRetry || recipientTrackingEvent2.EventDescription == Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.QueueRetryNoRetryTime)
					{
						return null;
					}
					return new RecipientTrackingEvent(CoreStrings.TrackingExplanationNormalTimeSpan.ToString(Thread.CurrentThread.CurrentCulture), Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.Pending, recipientTrackingEvent2.InternalMessageId, recipientTrackingEvent2.BccRecipient, utcNow, _DeliveryStatus.Pending, null, EventType.Pending, recipientTrackingEvent2.RecipientAddress, recipientTrackingEvent2.RecipientDisplayName, recipientTrackingEvent2.Server);
				}
				else if (t >= RecipientTrackingEvent.ExcessiveDelayTimeSpan)
				{
					return new RecipientTrackingEvent(CoreStrings.TrackingExplanationExcessiveTimeSpan.ToString(Thread.CurrentThread.CurrentCulture), Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.Pending, recipientTrackingEvent2.InternalMessageId, recipientTrackingEvent2.BccRecipient, utcNow, _DeliveryStatus.Pending, null, EventType.Pending, recipientTrackingEvent2.RecipientAddress, recipientTrackingEvent2.RecipientDisplayName, recipientTrackingEvent2.Server);
				}
			}
			return null;
		}

		internal static void FillDisplayNames(BulkRecipientLookupCache recipientCache, RecipientTrackingEvent[] recipientEvents, IRecipientSession session)
		{
			if (recipientEvents == null || recipientEvents.Length == 0)
			{
				return;
			}
			IEnumerable<string> addresses = from recipientEvent in recipientEvents
			select recipientEvent.RecipientAddress.ToString();
			IEnumerable<string> enumerable = recipientCache.Resolve(addresses, session);
			int num = 0;
			foreach (string recipientDisplayName in enumerable)
			{
				recipientEvents[num].RecipientDisplayName = recipientDisplayName;
				num++;
			}
		}

		private RecipientTrackingEvent(RecipientTrackingEvent internalRecipientTrackingEvent, string eventDescription)
		{
			this.eventDescriptionEnum = internalRecipientTrackingEvent.EventDescription;
			this.internalMessageId = internalRecipientTrackingEvent.InternalMessageId;
			this.bccRecipient = internalRecipientTrackingEvent.BccRecipient;
			this[RecipientTrackingEventSchema.EventDescription] = eventDescription;
			this[RecipientTrackingEventSchema.Date] = internalRecipientTrackingEvent.Date;
			this[RecipientTrackingEventSchema.DeliveryStatus] = (_DeliveryStatus)internalRecipientTrackingEvent.Status;
			this[RecipientTrackingEventSchema.EventData] = internalRecipientTrackingEvent.EventData;
			this[RecipientTrackingEventSchema.EventTypeValue] = internalRecipientTrackingEvent.EventType;
			this[RecipientTrackingEventSchema.RecipientAddress] = internalRecipientTrackingEvent.RecipientAddress;
			this[RecipientTrackingEventSchema.RecipientDisplayName] = internalRecipientTrackingEvent.RecipientDisplayName;
			this[RecipientTrackingEventSchema.Server] = internalRecipientTrackingEvent.Server;
		}

		private RecipientTrackingEvent(string eventDescription, EventDescription eventDescriptionEnum, long internalMessageId, bool bccRecipient, DateTime date, _DeliveryStatus deliveryStatus, string[] eventData, EventType eventType, SmtpAddress recipientAddress, string recipientDisplayName, string server)
		{
			this.eventDescriptionEnum = eventDescriptionEnum;
			this.internalMessageId = internalMessageId;
			this.bccRecipient = bccRecipient;
			this[RecipientTrackingEventSchema.EventDescription] = eventDescription;
			this[RecipientTrackingEventSchema.Date] = date;
			this[RecipientTrackingEventSchema.DeliveryStatus] = deliveryStatus;
			this[RecipientTrackingEventSchema.EventData] = eventData;
			this[RecipientTrackingEventSchema.EventTypeValue] = eventType;
			this[RecipientTrackingEventSchema.RecipientAddress] = recipientAddress;
			this[RecipientTrackingEventSchema.RecipientDisplayName] = recipientDisplayName;
			this[RecipientTrackingEventSchema.Server] = server;
		}

		private static RecipientTrackingEvent.FormatterMethod[] CreateIWFormatTable()
		{
			RecipientTrackingEvent.FormatterMethod[] array = new RecipientTrackingEvent.FormatterMethod[RecipientTrackingEvent.EventDescriptionsLength];
			array[0] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventSubmitted);
			array[1] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventSubmittedCrossSite);
			array[2] = null;
			array[3] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 2)
				{
					throw new FormatException("Expanded must have group name and group email address arguments");
				}
				return CoreStrings.EventExpanded(args[1]);
			};
			array[4] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventDelivered);
			array[5] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 1)
				{
					throw new FormatException("MovedToFolderByInboxRule must have folder name argument");
				}
				return CoreStrings.EventMovedToFolderByInboxRuleIW(args[0]);
			};
			array[6] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventRulesCc);
			array[7] = RecipientTrackingEvent.FailedGeneralDelegate();
			array[8] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventFailedModerationIW);
			array[9] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventFailedTransportRulesIW);
			array[10] = null;
			array[11] = null;
			array[12] = null;
			array[13] = null;
			array[14] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventForwarded);
			array[15] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventPending);
			array[16] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventPendingModerationIW);
			array[17] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventApprovedModerationIW);
			array[18] = null;
			array[20] = null;
			array[21] = RecipientTrackingEvent.ForeignOrgDelegate();
			array[22] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventTransferredToLegacyExchangeServer);
			array[24] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventDelayedAfterTransferToPartnerOrgIW);
			array[25] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventRead);
			array[26] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventNotRead);
			array[27] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventForwardedToDelegateAndDeleted);
			array[28] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventModerationExpired);
			return array;
		}

		private static RecipientTrackingEvent.FormatterMethod[] CreateHelpDeskFormatTable()
		{
			RecipientTrackingEvent.FormatterMethod[] array = new RecipientTrackingEvent.FormatterMethod[RecipientTrackingEvent.EventDescriptionsLength];
			array[0] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args == null || args.Length < 1)
				{
					return CoreStrings.EventSubmitted;
				}
				return CoreStrings.EventSubmittedHelpDesk(args[0]);
			};
			array[1] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args == null || args.Length < 1)
				{
					return CoreStrings.EventSubmittedCrossSite;
				}
				return CoreStrings.EventSubmittedCrossSiteHelpDesk(args[0]);
			};
			array[2] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args == null || args.Length < 2)
				{
					return CoreStrings.EventResolvedHelpDesk;
				}
				return CoreStrings.EventResolvedWithDetailsHelpDesk(args[0], args[1]);
			};
			array[3] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 2)
				{
					throw new FormatException("Expanded must have group name and group email address arguments");
				}
				return CoreStrings.EventExpanded(args[1]);
			};
			array[4] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventDelivered);
			array[5] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 1)
				{
					throw new FormatException("MovedToFolderByInboxRule must have folder name argument");
				}
				return CoreStrings.EventMovedToFolderByInboxRuleHelpDesk(args[0]);
			};
			array[6] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventRulesCc);
			array[7] = RecipientTrackingEvent.FailedGeneralDelegate();
			array[8] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventFailedModerationHelpDesk);
			array[9] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventFailedTransportRulesHelpDesk);
			array[10] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 2)
				{
					throw new FormatException("SmtpSend must have sending and receiving server names");
				}
				return CoreStrings.EventSmtpSendHelpDesk(args[0], args[1]);
			};
			array[11] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 2)
				{
					throw new FormatException("SmtpSendCrossSite must have sending and receiving server names");
				}
				return CoreStrings.EventSmtpSendHelpDesk(args[0], args[1]);
			};
			array[12] = ((RecipientTrackingEvent.FormatterSource source, string[] args) => CoreStrings.EventSmtpSendHelpDesk(args[0], args[1]));
			array[13] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 2)
				{
					throw new FormatException("SmtpReceive must have sending and receiving server names");
				}
				return CoreStrings.EventSmtpReceiveHelpDesk(args[0], args[1]);
			};
			array[14] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventForwarded);
			array[15] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventPending);
			array[16] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventPendingModerationHelpDesk);
			array[17] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventApprovedModerationHelpDesk);
			array[18] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 5)
				{
					throw new FormatException("QueueRetry must have server, inRetrySinceTime, lastAttemptTime, serverTimeZone, errorMessage arguments");
				}
				if (!string.IsNullOrEmpty(args[4]))
				{
					return CoreStrings.EventQueueRetryHelpDesk(args[0], args[1], args[2], args[3], args[4]);
				}
				return CoreStrings.EventQueueRetryNoErrorHelpDesk(args[0], args[1], args[2], args[3]);
			};
			array[19] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 2)
				{
					throw new FormatException("QueueRetryNoRetryTime must have queue name and error message");
				}
				return CoreStrings.EventQueueRetryNoRetryTimeHelpDesk(args[0], args[1]);
			};
			array[20] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventMessageDefer);
			array[21] = RecipientTrackingEvent.ForeignOrgDelegate();
			array[22] = delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args.Length < 2)
				{
					throw new FormatException("TransferredToLegacyExchangeServer must have both local exchange server name and remote legacy exchange server name");
				}
				return CoreStrings.EventTransferredToLegacyExchangeServerHelpDesk(args[0], args[1]);
			};
			array[25] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventRead);
			array[26] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventNotRead);
			array[27] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventForwardedToDelegateAndDeleted);
			array[28] = RecipientTrackingEvent.SimpleDelegate(CoreStrings.EventModerationExpired);
			return array;
		}

		private static Dictionary<EventDescription, EventType> CreateDescriptionToTypeMap()
		{
			return new Dictionary<EventDescription, EventType>
			{
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.Submitted,
					EventType.Submit
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.Expanded,
					EventType.Expand
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.Delivered,
					EventType.Deliver
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.FailedGeneral,
					EventType.Fail
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.FailedModeration,
					EventType.Fail
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.FailedTransportRules,
					EventType.Fail
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.TransferredToForeignOrg,
					EventType.Transferred
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.TransferredToLegacyExchangeServer,
					EventType.Transferred
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.Read,
					EventType.Deliver
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.NotRead,
					EventType.Deliver
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.MovedToFolderByInboxRule,
					EventType.Deliver
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.ForwardedToDelegateAndDeleted,
					EventType.Deliver
				},
				{
					Microsoft.Exchange.InfoWorker.Common.MessageTracking.EventDescription.ExpiredWithNoModerationDecision,
					EventType.Fail
				}
			};
		}

		private static RecipientTrackingEvent.FormatterMethod SimpleDelegate(LocalizedString message)
		{
			return (RecipientTrackingEvent.FormatterSource source, string[] args) => message;
		}

		private static RecipientTrackingEvent.FormatterMethod ForeignOrgDelegate()
		{
			return delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (Parse.IsSMSRecipient(source.RecipientTrackingEvent.RecipientAddress.ToString()))
				{
					return CoreStrings.EventTransferredToSMSMessage;
				}
				return CoreStrings.EventTransferredToForeignOrgIW;
			};
		}

		private static RecipientTrackingEvent.FormatterMethod FailedGeneralDelegate()
		{
			return delegate(RecipientTrackingEvent.FormatterSource source, string[] args)
			{
				if (args != null && args.Length > 0)
				{
					string text;
					if (!LastError.TryParseSmtpResponseString(args[0], out text))
					{
						text = args[0];
					}
					SmtpResponse key;
					if (SmtpResponse.TryParse(text, out key))
					{
						if (key.SmtpResponseType == SmtpResponseType.PermanentError && args.Length >= 2 && string.Equals(args[1], "Content Filter Agent"))
						{
							return CoreStrings.RejectedExplanationContentFiltering;
						}
						LocalizedString result;
						if (AckReason.EnhancedTextGetter.TryGetValue(key, out result))
						{
							return result;
						}
						if (!string.IsNullOrEmpty(key.EnhancedStatusCode))
						{
							if (DsnShortMessages.TryGetResourceRecipientExplanation(key.EnhancedStatusCode, out result))
							{
								return result;
							}
							LocalizedString? customDsnCode = RecipientTrackingEvent.GetCustomDsnCode(key.EnhancedStatusCode, source.ConfigSession, source.UserLanguages);
							if (customDsnCode != null)
							{
								return customDsnCode.Value;
							}
						}
					}
				}
				return CoreStrings.EventFailedGeneral;
			};
		}

		private static LocalizedString? GetCustomDsnCode(string enhancedStatus, IConfigurationSession session, IEnumerable<CultureInfo> userLanguages)
		{
			if (string.IsNullOrEmpty(enhancedStatus))
			{
				throw new InvalidOperationException("Cannot find custom text without EnhancedStatus");
			}
			ADObjectId orgContainerId = session.GetOrgContainerId();
			ObjectId dsnCustomizationContainer = SystemMessage.GetDsnCustomizationContainer(orgContainerId);
			QueryFilter filter = new TextFilter(ADObjectSchema.Name, enhancedStatus, MatchOptions.FullString, MatchFlags.Default);
			SystemMessage[] array = (SystemMessage[])session.Find<SystemMessage>(filter, dsnCustomizationContainer, true, null);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			if (userLanguages != null)
			{
				int num = 0;
				foreach (CultureInfo cultureInfo in userLanguages)
				{
					if (num++ >= 3)
					{
						break;
					}
					foreach (SystemMessage systemMessage in array)
					{
						if (systemMessage.Internal && (systemMessage.Language.LCID == cultureInfo.LCID || systemMessage.Language.LCID == cultureInfo.Parent.LCID))
						{
							return new LocalizedString?(new LocalizedString(systemMessage.Text));
						}
					}
				}
			}
			return null;
		}

		private static readonly TimeSpan SmtpHandshakeMaximumSkew = new TimeSpan(0, 15, 0);

		private static readonly TimeSpan MaximumDelayForNormalMessage = new TimeSpan(0, 5, 0);

		private static readonly TimeSpan ExcessiveDelayTimeSpan = new TimeSpan(0, 30, 0);

		private static RecipientTrackingEventSchema schema = ObjectSchema.GetInstance<RecipientTrackingEventSchema>();

		internal static int EventDescriptionsLength = Enum.GetValues(typeof(EventDescription)).Length;

		private static RecipientTrackingEvent.FormatterMethod[] iWFormatters = RecipientTrackingEvent.CreateIWFormatTable();

		private static RecipientTrackingEvent.FormatterMethod[] helpDeskFormatters = RecipientTrackingEvent.CreateHelpDeskFormatTable();

		private static Dictionary<EventDescription, EventType> descriptionToTypeMap = RecipientTrackingEvent.CreateDescriptionToTypeMap();

		private EventDescription eventDescriptionEnum;

		private readonly long internalMessageId;

		private readonly bool bccRecipient;

		private delegate LocalizedString FormatterMethod(RecipientTrackingEvent.FormatterSource source, string[] eventData);

		internal class FormatterSource
		{
			internal IConfigurationSession ConfigSession { get; private set; }

			internal MultiValuedProperty<CultureInfo> UserLanguages { get; private set; }

			internal RecipientTrackingEvent RecipientTrackingEvent { get; private set; }

			internal FormatterSource(IConfigurationSession configSession, MultiValuedProperty<CultureInfo> userLanguages, RecipientTrackingEvent recipientTrackingEvent)
			{
				this.ConfigSession = configSession;
				this.UserLanguages = userLanguages;
				this.RecipientTrackingEvent = recipientTrackingEvent;
			}
		}
	}
}
