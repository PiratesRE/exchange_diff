using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class MeetingSeriesMessageOrderingAgent : StoreDriverDeliveryAgent
	{
		public MeetingSeriesMessageOrderingAgent()
		{
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
		}

		internal static bool SeriesMessageOrderingEnabled(MiniRecipient mailboxOwner)
		{
			return mailboxOwner != null && VariantConfiguration.GetSnapshot(mailboxOwner.GetContext(null), null, null).MailboxTransport.OrderSeriesMeetingMessages.Enabled;
		}

		private static int GetRetentionPeriod(MiniRecipient mailboxOwner)
		{
			if (mailboxOwner != null)
			{
				string value = VariantConfiguration.InvariantNoFlightingSnapshot.MailboxTransport.ParkedMeetingMessagesRetentionPeriod.Value;
				int result;
				if (!string.IsNullOrEmpty(value) && int.TryParse(value, out result))
				{
					return result;
				}
			}
			return 2;
		}

		private void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			if (MeetingSeriesMessageOrderingAgent.SeriesMessageOrderingEnabled(storeDriverDeliveryEventArgsImpl.MailboxOwner))
			{
				this.OrderMessages(storeDriverDeliveryEventArgsImpl);
			}
		}

		private void OrderMessages(StoreDriverDeliveryEventArgsImpl args)
		{
			string messageClass = args.MessageClass;
			if (this.IsCalendarMessage(messageClass))
			{
				MeetingSeriesMessageOrderingAgent.tracer.TraceDebug<string>((long)this.GetHashCode(), "MessageClass: {0}", messageClass);
				MeetingSeriesMessageOrderingAgent.tracer.TraceDebug<string>((long)this.GetHashCode(), "MessageId: {0}", args.MailItem.Message.MessageId);
				MeetingSeriesMessageOrderingAgent.SeriesHeadersData headersData;
				bool flag2;
				bool flag = this.IsParkingRequired(args, out headersData, out flag2);
				if (!headersData.UnparkedMessage)
				{
					MeetingSeriesMessageOrdering.MeetingMessages.Increment();
					if (flag2)
					{
						MeetingSeriesMessageOrdering.SeriesMeetingInstanceMessages.Increment();
					}
					if (flag)
					{
						MeetingSeriesMessageOrdering.ParkedSeriesMeetingInstanceMessages.Increment();
						if (this.TryParkThisMessage(args, headersData))
						{
							throw new SmtpResponseException(AckReason.MeetingMessageParkedSuccess, base.Name);
						}
					}
				}
			}
		}

		private bool IsParkingRequired(StoreDriverDeliveryEventArgsImpl args, out MeetingSeriesMessageOrderingAgent.SeriesHeadersData headersData, out bool canBeParked)
		{
			DeliverableMailItem mailItem = args.MailItem;
			HeaderList headers = mailItem.Message.MimeDocument.RootPart.Headers;
			headersData = MeetingSeriesMessageOrderingAgent.SeriesHeadersData.FromHeaderList(headers, args.MailItemDeliver.MbxTransportMailItem.NetworkMessageId);
			bool flag = !string.IsNullOrEmpty(headersData.SeriesId);
			canBeParked = (flag && this.CanMessageBeParked(args.MessageClass));
			if (headersData.UnparkedMessage || !flag)
			{
				return false;
			}
			MailboxSession mailboxSession = args.MailboxSession;
			if (headersData.SeriesSequenceNumber > 0 && headersData.InstanceGoid != null && headersData.MasterGoid != null)
			{
				StoreId storeId;
				if (!this.MasterMessageArrived(mailboxSession, headersData, out storeId))
				{
					return true;
				}
				if (storeId != null)
				{
					this.CacheInstanceId(headers, storeId);
				}
			}
			else if (MeetingSeriesMessageOrderingAgent.tracer.IsTraceEnabled(TraceType.WarningTrace))
			{
				MeetingSeriesMessageOrderingAgent.tracer.TraceWarning((long)this.GetHashCode(), "Series instance message is missing required information to be used for ordering detection. Mailbox = {0}, messageId = {1}, seriesId = {2}, SSN = {3}, instanceGoid = {4}, masterGoid = {5}", new object[]
				{
					mailboxSession.MailboxOwnerLegacyDN,
					mailItem.Message.MessageId,
					headersData.SeriesId,
					headersData.SeriesSequenceNumber,
					(headersData.InstanceGoid != null) ? Convert.ToBase64String(headersData.InstanceGoid) : string.Empty,
					(headersData.MasterGoid != null) ? Convert.ToBase64String(headersData.MasterGoid) : string.Empty
				});
			}
			return false;
		}

		private void CacheInstanceId(HeaderList headers, StoreId instanceCalendarItemId)
		{
			Header header = headers.FindFirst("X-MS-Exchange-Calendar-Series-Instance-Calendar-Item-Id");
			if (header == null)
			{
				header = Header.Create("X-MS-Exchange-Calendar-Series-Instance-Calendar-Item-Id");
				headers.AppendChild(header);
			}
			header.Value = StoreId.GetStoreObjectId(instanceCalendarItemId).ToBase64String();
		}

		private bool TryParkThisMessage(StoreDriverDeliveryEventArgsImpl args, MeetingSeriesMessageOrderingAgent.SeriesHeadersData headersData)
		{
			Guid networkMessageId = args.MailItemDeliver.MbxTransportMailItem.NetworkMessageId;
			this.AddUnparkedHeader(args.MailItem.Message.MimeDocument.RootPart.Headers, networkMessageId);
			bool encapsulationSucceeded = true;
			string text;
			bool encapsulationSucceeded2;
			using (EmailMessage emailMessage = ModerationHelper.EncapsulateOriginalMessage(args.MailItemDeliver.MbxTransportMailItem, new List<MailRecipient>
			{
				args.MailRecipient
			}, args.MailRecipient.Email.ToString(), args.MailRecipient.Email.ToString(), MeetingSeriesMessageOrderingAgent.tracer, delegate(Exception param0)
			{
				encapsulationSucceeded = false;
			}, out text))
			{
				if (encapsulationSucceeded)
				{
					MailboxSession mailboxSession = args.MailboxSession;
					using (ParkedMeetingMessage parkedMeetingMessage = ParkedMeetingMessage.Create(mailboxSession))
					{
						if (parkedMeetingMessage != null)
						{
							string internetMessageId = args.MailItemDeliver.MbxTransportMailItem.InternetMessageId;
							ItemConversion.ConvertAnyMimeToItem(parkedMeetingMessage, emailMessage.MimeDocument, new InboundConversionOptions(args.ADRecipientCache));
							parkedMeetingMessage.ParkedCorrelationId = ParkedMeetingMessage.GetCorrelationId(headersData.SeriesId, headersData.SeriesSequenceNumber);
							parkedMeetingMessage[StoreObjectSchema.ItemClass] = "IPM.Parked.MeetingMessage";
							parkedMeetingMessage.CleanGlobalObjectId = headersData.InstanceGoid;
							parkedMeetingMessage.OriginalMessageId = internetMessageId;
							int retentionPeriod = MeetingSeriesMessageOrderingAgent.GetRetentionPeriod(args.MailboxOwner);
							PolicyTagHelper.SetRetentionProperties(parkedMeetingMessage, ExDateTime.UtcNow.AddDays((double)retentionPeriod), retentionPeriod);
							parkedMeetingMessage.Save(SaveMode.NoConflictResolution);
						}
					}
				}
				encapsulationSucceeded2 = encapsulationSucceeded;
			}
			return encapsulationSucceeded2;
		}

		private void AddUnparkedHeader(HeaderList headers, Guid messageId)
		{
			headers.RemoveAll("X-MS-Exchange-Calendar-Series-Instance-Unparked");
			Header header = Header.Create("X-MS-Exchange-Calendar-Series-Instance-Unparked");
			header.Value = messageId.ToString();
			headers.AppendChild(header);
		}

		private bool MasterMessageArrived(MailboxSession session, MeetingSeriesMessageOrderingAgent.SeriesHeadersData headersData, out StoreId instanceCalendarItemId)
		{
			OrFilter seekFilter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, CalendarItemBaseSchema.GlobalObjectId, headersData.InstanceGoid),
				new ComparisonFilter(ComparisonOperator.Equal, CalendarItemBaseSchema.GlobalObjectId, headersData.MasterGoid)
			});
			instanceCalendarItemId = null;
			bool flag = false;
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(session, DefaultFolderType.Calendar))
			{
				using (QueryResult queryResult = calendarFolder.ItemQuery(ItemQueryType.None, null, MeetingSeriesMessageOrderingAgent.SortByGoid, MeetingSeriesMessageOrderingAgent.AdditionalQueryProperties))
				{
					bool flag2 = true;
					while (queryResult.SeekToCondition(flag2 ? SeekReference.OriginBeginning : SeekReference.OriginCurrent, seekFilter, SeekToConditionFlags.AllowExtendedFilters))
					{
						flag2 = false;
						IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
						if (propertyBags.Length == 1)
						{
							IStorePropertyBag storePropertyBag = propertyBags[0];
							if (ObjectClass.IsCalendarItemSeries(storePropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, null)))
							{
								int valueOrDefault = storePropertyBag.GetValueOrDefault<int>(CalendarItemBaseSchema.AppointmentSequenceNumber, -1);
								flag = (headersData.SeriesSequenceNumber <= valueOrDefault);
								MeetingSeriesMessageOrderingAgent.tracer.TraceDebug<int, int, bool>((long)this.GetHashCode(), "Message SSN: {0}, Master SSN: {1} => Master message arrived: {2}.", headersData.SeriesSequenceNumber, valueOrDefault, flag);
							}
							else
							{
								instanceCalendarItemId = storePropertyBag.GetValueOrDefault<StoreId>(ItemSchema.Id, null);
							}
						}
					}
				}
			}
			return flag;
		}

		private bool IsCalendarMessage(string messageClass)
		{
			return ObjectClass.IsMeetingMessage(messageClass) || ObjectClass.IsMeetingMessageSeries(messageClass);
		}

		private bool CanMessageBeParked(string messageClass)
		{
			return ObjectClass.IsMeetingRequest(messageClass) || ObjectClass.IsMeetingCancellation(messageClass);
		}

		internal const int DefaultParkedItemRetentionPeriodInDays = 2;

		private static readonly SortBy[] SortByGoid = new SortBy[]
		{
			new SortBy(CalendarItemBaseSchema.GlobalObjectId, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] AdditionalQueryProperties = new PropertyDefinition[]
		{
			CalendarItemBaseSchema.SeriesId,
			StoreObjectSchema.ItemClass,
			CalendarItemBaseSchema.AppointmentSequenceNumber,
			ItemSchema.Id
		};

		private static readonly Trace tracer = ExTraceGlobals.MeetingSeriesMessageOrderingAgentTracer;

		private struct SeriesHeadersData
		{
			public string SeriesId { get; private set; }

			public int SeriesSequenceNumber { get; private set; }

			public byte[] InstanceGoid { get; private set; }

			public byte[] MasterGoid { get; private set; }

			public bool UnparkedMessage { get; private set; }

			public static MeetingSeriesMessageOrderingAgent.SeriesHeadersData FromHeaderList(HeaderList headers, Guid messageId)
			{
				TextHeader textHeader = headers.FindFirst("X-MS-Exchange-Calendar-Series-Id") as TextHeader;
				TextHeader textHeader2 = headers.FindFirst("X-MS-Exchange-Calendar-Series-Sequence-Number") as TextHeader;
				TextHeader textHeader3 = headers.FindFirst("X-MS-Exchange-Calendar-Series-Instance-Id") as TextHeader;
				TextHeader textHeader4 = headers.FindFirst("X-MS-Exchange-Calendar-Series-Master-Id") as TextHeader;
				TextHeader textHeader5 = headers.FindFirst("X-MS-Exchange-Calendar-Series-Instance-Unparked") as TextHeader;
				MeetingSeriesMessageOrderingAgent.SeriesHeadersData result = new MeetingSeriesMessageOrderingAgent.SeriesHeadersData
				{
					SeriesId = ((textHeader != null) ? textHeader.Value : null)
				};
				MeetingSeriesMessageOrderingAgent.tracer.TraceDebug<string>(0L, "SeriesId: {0}", result.SeriesId ?? string.Empty);
				int num;
				if (textHeader2 != null && int.TryParse(textHeader2.Value, out num))
				{
					result.SeriesSequenceNumber = num;
					MeetingSeriesMessageOrderingAgent.tracer.TraceDebug<int>(0L, "SeriesSequenceNumber: {0}", num);
				}
				byte[] instanceGoid;
				if (textHeader3 != null && MeetingSeriesMessageOrderingAgent.SeriesHeadersData.TryParseGoid(textHeader3.Value, out instanceGoid))
				{
					result.InstanceGoid = instanceGoid;
					MeetingSeriesMessageOrderingAgent.tracer.TraceDebug<string>(0L, "InstanceGoid: {0}", textHeader3.Value);
				}
				byte[] masterGoid;
				if (textHeader4 != null && MeetingSeriesMessageOrderingAgent.SeriesHeadersData.TryParseGoid(textHeader4.Value, out masterGoid))
				{
					result.MasterGoid = masterGoid;
					MeetingSeriesMessageOrderingAgent.tracer.TraceDebug<string>(0L, "MasterGoid: {0}", textHeader4.Value);
				}
				if (textHeader5 != null && !string.IsNullOrEmpty(textHeader5.Value))
				{
					result.UnparkedMessage = (textHeader5.Value == messageId.ToString());
					MeetingSeriesMessageOrderingAgent.tracer.TraceDebug<string, Guid>(0L, "UnparkedMessageId: {0}, processed message id: {1}", textHeader5.Value, messageId);
				}
				return result;
			}

			private static bool TryParseGoid(string base64GoidString, out byte[] goidBytes)
			{
				goidBytes = null;
				if (base64GoidString != null)
				{
					try
					{
						goidBytes = Convert.FromBase64String(base64GoidString);
						return true;
					}
					catch (FormatException arg)
					{
						MeetingSeriesMessageOrderingAgent.tracer.TraceWarning<string, FormatException>(0L, "Error converting base64 GOID to byte array. Base64 GOID: {0}, error: {1}", base64GoidString, arg);
					}
					return false;
				}
				return false;
			}
		}
	}
}
