using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class EmailMessageToMessageItem
	{
		private static bool IsOutlookAutoRedirectionMessage(EmailMessage email)
		{
			if (email.RootPart == null)
			{
				return false;
			}
			HeaderList headers = email.RootPart.Headers;
			if (headers == null)
			{
				return false;
			}
			Header header = headers.FindFirst("X-MS-Exchange-Organization-AutoForwarded");
			return header != null && string.Equals("true", header.Value, StringComparison.OrdinalIgnoreCase) && string.Equals(email.MapiMessageClass, "IPM.Note") && 1 == email.Attachments.Count && email.Attachments[0].IsEmbeddedMessage;
		}

		private static bool AreAllCalEventsInTheSameDay(List<CalendarEvent> calEvents)
		{
			CalendarEvent calendarEvent = calEvents[0];
			CalendarEvent calendarEvent2 = calEvents[calEvents.Count - 1];
			return string.Equals(calendarEvent.DateOfStartTime, calendarEvent2.DateOfStartTime, StringComparison.OrdinalIgnoreCase);
		}

		public MessageItem Convert(ADSessionSettings adSessionSettings, EmailMessage email, string msgCls, ICollection<MobileRecipient> recipients, CultureInfo preferredCulture, out CalendarNotificationType calNotifTypeHint)
		{
			calNotifTypeHint = CalendarNotificationType.Uninteresting;
			if (email == null)
			{
				throw new ArgumentNullException("email");
			}
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			bool flag = string.Equals("IPM.Note.Mobile.SMS.Alert", msgCls, StringComparison.OrdinalIgnoreCase);
			bool flag2 = flag || EmailMessageToMessageItem.IsOutlookAutoRedirectionMessage(email);
			bool flag3 = flag2 || ObjectClass.IsOfClass(msgCls, "IPM.Note.Mobile.SMS.Alert");
			List<ProportionedText> list = new List<ProportionedText>();
			EmailRecipient from = EmailMessageHelper.GetFrom(email);
			string smtpAddress = from.SmtpAddress;
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, adSessionSettings, 173, "Convert", "f:\\15.00.1497\\sources\\dev\\SMS\\src\\MobileDriver\\EmailMessageToMessageItem.cs");
			ADRawEntry adrawEntry = tenantOrRootOrgRecipientSession.FindByProxyAddress(ProxyAddress.Parse(from.NativeAddressType, from.NativeAddress), EmailMessageToMessageItem.InterestedPropertiesForSender);
			if (adrawEntry == null)
			{
				throw new MobileDriverObjectNotFoundException(Strings.ErrorObjectNotFound(from.SmtpAddress.ToString()));
			}
			string text = string.IsNullOrEmpty(from.DisplayName) ? ((string)adrawEntry[ADRecipientSchema.DisplayName]) : from.DisplayName;
			if (string.IsNullOrEmpty(text))
			{
				text = ((adrawEntry[ADRecipientSchema.PrimarySmtpAddress] == null || !((SmtpAddress)adrawEntry[ADRecipientSchema.PrimarySmtpAddress]).IsValidAddress) ? from.SmtpAddress : ((string)((SmtpAddress)adrawEntry[ADRecipientSchema.PrimarySmtpAddress])));
			}
			string text2 = (string)adrawEntry[ADOrgPersonSchema.MobilePhone];
			string number = text2;
			if (flag2)
			{
				foreach (Attachment attachment in email.Attachments)
				{
					if (attachment.IsEmbeddedMessage)
					{
						email = attachment.EmbeddedMessage;
						from = EmailMessageHelper.GetFrom(email);
						if (from == null)
						{
							text = string.Empty;
							text2 = string.Empty;
							break;
						}
						text = from.DisplayName;
						ADRawEntry adrawEntry2 = tenantOrRootOrgRecipientSession.FindByProxyAddress(ProxyAddress.Parse(from.NativeAddressType, from.NativeAddress), EmailMessageToMessageItem.InterestedPropertiesForSender);
						if (adrawEntry2 == null)
						{
							text2 = string.Empty;
							if (string.IsNullOrEmpty(text))
							{
								text = from.SmtpAddress;
							}
						}
						else
						{
							if (string.IsNullOrEmpty(text))
							{
								text = (string)adrawEntry2[ADRecipientSchema.DisplayName];
							}
							if (string.IsNullOrEmpty(text) && adrawEntry2[ADRecipientSchema.PrimarySmtpAddress] != null && ((SmtpAddress)adrawEntry2[ADRecipientSchema.PrimarySmtpAddress]).IsValidAddress)
							{
								text = (string)((SmtpAddress)adrawEntry2[ADRecipientSchema.PrimarySmtpAddress]);
							}
							text2 = (string)adrawEntry2[ADOrgPersonSchema.MobilePhone];
						}
						if (!flag)
						{
							break;
						}
						SmtpAddress smtpAddress2 = new SmtpAddress(smtpAddress);
						SmtpAddress address = new SmtpAddress(from.SmtpAddress);
						if (smtpAddress2.IsValidAddress && address.IsValidAddress && smtpAddress2.Equals(address))
						{
							throw new MobileDriverEmailNotificationDeadLoopException(Strings.ErrorEmailNotificationDeadLoop);
						}
						break;
					}
				}
				if (email == null)
				{
					throw new MobileDriverObjectNotFoundException(Strings.ErrorEmailMessageNotFound);
				}
			}
			text = (text ?? string.Empty);
			text2 = (text2 ?? string.Empty);
			string text3 = EmailMessageHelper.GetBodyText(email);
			if (flag3)
			{
				list.Clear();
				if (ObjectClass.IsOfClass(msgCls, "IPM.Note.Mobile.SMS.Alert.Calendar"))
				{
					CalendarNotificationContentVersion1Point0 calendarNotificationContentVersion1Point = null;
					try
					{
						int num = text3.IndexOf("<?xml");
						int num2 = text3.LastIndexOf("</CalendarNotificationContent>");
						int num3 = num2 + "</CalendarNotificationContent>".Length - num;
						if (-1 < num && -1 < num2 && num < num2 && num3 < text3.Length)
						{
							text3 = text3.Substring(num, num3);
						}
						calendarNotificationContentVersion1Point = (CalendarNotificationContentVersion1Point0)VersionedXmlBase.Parse(text3);
					}
					catch (XmlException ex)
					{
						throw new MobileDriverDataException(Strings.ErrorInvalidCalNotifContent(text3, ex.ToString()), ex);
					}
					catch (InvalidOperationException ex2)
					{
						throw new MobileDriverDataException(Strings.ErrorInvalidCalNotifContent(text3, ex2.ToString()), ex2);
					}
					if (calendarNotificationContentVersion1Point == null)
					{
						throw new MobileDriverDataException(Strings.ErrorEmptyCalNotifContent);
					}
					calNotifTypeHint = calendarNotificationContentVersion1Point.CalNotifType;
					switch (calendarNotificationContentVersion1Point.CalNotifType)
					{
					case CalendarNotificationType.Summary:
						goto IL_6AC;
					case CalendarNotificationType.Reminder:
						break;
					case CalendarNotificationType.NewUpdate:
					case CalendarNotificationType.ChangedUpdate:
					case CalendarNotificationType.DeletedUpdate:
						list.Add(new ProportionedText(calendarNotificationContentVersion1Point.CalNotifTypeDesc));
						using (List<CalendarEvent>.Enumerator enumerator2 = calendarNotificationContentVersion1Point.CalEvents.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								CalendarEvent calendarEvent = enumerator2.Current;
								bool flag4 = string.Equals(calendarEvent.DateOfStartTime, calendarEvent.DateOfEndTime, StringComparison.OrdinalIgnoreCase);
								if (!string.IsNullOrEmpty(calendarEvent.Subject))
								{
									list.Add(new ProportionedText("\n"));
									list.Add(new ProportionedText(calendarEvent.Subject, 20, 160, 1));
								}
								list.Add(new ProportionedText("\n"));
								list.Add(new ProportionedText(calendarEvent.TimeOfStartTime));
								if (!flag4)
								{
									list.Add(new ProportionedText(","));
									list.Add(new ProportionedText(calendarEvent.DateOfStartTime));
								}
								list.Add(new ProportionedText("-"));
								list.Add(new ProportionedText(calendarEvent.TimeOfEndTime));
								list.Add(new ProportionedText(","));
								list.Add(new ProportionedText(calendarEvent.DateOfEndTime));
								if (!string.IsNullOrEmpty(calendarEvent.Location))
								{
									list.Add(new ProportionedText("\n"));
									list.Add(new ProportionedText(calendarEvent.Location, 20, 160, 1));
								}
							}
							goto IL_AE2;
						}
						break;
					default:
						goto IL_98C;
					}
					bool flag5 = true;
					using (List<CalendarEvent>.Enumerator enumerator3 = calendarNotificationContentVersion1Point.CalEvents.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							CalendarEvent calendarEvent2 = enumerator3.Current;
							bool flag6 = string.Equals(calendarEvent2.DateOfStartTime, calendarEvent2.DateOfEndTime, StringComparison.OrdinalIgnoreCase);
							if (flag5)
							{
								flag5 = false;
							}
							else
							{
								list.Add(new ProportionedText("\n"));
							}
							list.Add(new ProportionedText(calendarEvent2.TimeOfStartTime));
							if (!flag6)
							{
								list.Add(new ProportionedText(","));
								list.Add(new ProportionedText(calendarEvent2.DateOfStartTime));
							}
							list.Add(new ProportionedText("-"));
							list.Add(new ProportionedText(calendarEvent2.TimeOfEndTime));
							list.Add(new ProportionedText(","));
							list.Add(new ProportionedText(calendarEvent2.DateOfEndTime));
							if (!string.IsNullOrEmpty(calendarEvent2.Subject))
							{
								list.Add(new ProportionedText("\n"));
								list.Add(new ProportionedText(calendarEvent2.Subject, 20, 160, 1));
							}
							if (!string.IsNullOrEmpty(calendarEvent2.Location))
							{
								list.Add(new ProportionedText("\n"));
								list.Add(new ProportionedText(calendarEvent2.Location, 20, 160, 1));
							}
						}
						goto IL_AE2;
					}
					IL_6AC:
					if (calendarNotificationContentVersion1Point.CalEvents == null || calendarNotificationContentVersion1Point.CalEvents.Count == 0)
					{
						goto IL_AE2;
					}
					string text4 = calendarNotificationContentVersion1Point.CalEvents[0].DateOfStartTime;
					bool flag7 = false;
					list.Add(new ProportionedText(calendarNotificationContentVersion1Point.CalNotifTypeDesc));
					if (EmailMessageToMessageItem.AreAllCalEventsInTheSameDay(calendarNotificationContentVersion1Point.CalEvents))
					{
						list.Add(new ProportionedText(" "));
						list.Add(new ProportionedText(text4));
						flag7 = true;
					}
					else
					{
						text4 = string.Empty;
					}
					list.Add(new ProportionedText(" ", true));
					string text5 = Strings.notifCountOfEventsDesc(calendarNotificationContentVersion1Point.CalEvents.Count.ToString()).ToString(preferredCulture);
					list.Add(new ProportionedText(text5, true));
					list.Add(ProportionedText.Delimiter);
					using (List<CalendarEvent>.Enumerator enumerator4 = calendarNotificationContentVersion1Point.CalEvents.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							CalendarEvent calendarEvent3 = enumerator4.Current;
							list.Add(new ProportionedText("\n"));
							list.Add(new ProportionedText("\n"));
							if (!flag7 && !string.Equals(calendarEvent3.DateOfStartTime, text4, StringComparison.OrdinalIgnoreCase))
							{
								list.Add(new ProportionedText(calendarEvent3.DateOfStartTime));
								list.Add(new ProportionedText("\n"));
							}
							text4 = calendarEvent3.DateOfStartTime;
							if (string.IsNullOrEmpty(calendarEvent3.TimeOfStartTime))
							{
								list.Add(new ProportionedText(Strings.calNotifAllDayEventsDesc.ToString(preferredCulture)));
							}
							else
							{
								list.Add(new ProportionedText(calendarEvent3.TimeOfStartTime));
								list.Add(new ProportionedText("-"));
								list.Add(new ProportionedText(calendarEvent3.TimeOfEndTime));
								if (!string.Equals(calendarEvent3.DateOfEndTime, text4, StringComparison.OrdinalIgnoreCase))
								{
									list.Add(new ProportionedText("("));
									list.Add(new ProportionedText(calendarEvent3.DateOfEndTime));
									list.Add(new ProportionedText(")"));
								}
							}
							if (!string.IsNullOrEmpty(calendarEvent3.Subject))
							{
								list.Add(new ProportionedText(" \""));
								if (this.GetCodingScheme(calendarEvent3.Subject) == CodingScheme.Unicode)
								{
									list.Add(new ProportionedText(calendarEvent3.Subject, 6, 15, 1));
								}
								else
								{
									list.Add(new ProportionedText(calendarEvent3.Subject, 12, 30, 1));
								}
								list.Add(new ProportionedText("\""));
							}
							if (!string.IsNullOrEmpty(calendarEvent3.Location))
							{
								list.Add(new ProportionedText(" @"));
								if (this.GetCodingScheme(calendarEvent3.Location) == CodingScheme.Unicode)
								{
									list.Add(new ProportionedText(calendarEvent3.Location, 6, 15, 1));
								}
								else
								{
									list.Add(new ProportionedText(calendarEvent3.Location, 12, 30, 1));
								}
							}
							list.Add(ProportionedText.Delimiter);
						}
						goto IL_AE2;
					}
					IL_98C:
					throw new MobileDriverDataException(Strings.ErrorUnknownCalNotifType(calendarNotificationContentVersion1Point.CalNotifType.ToString()));
				}
				else if (ObjectClass.IsOfClass(msgCls, "IPM.Note.Mobile.SMS.Alert.Voicemail"))
				{
					list.Add(new ProportionedText(text3));
				}
				else if (ObjectClass.IsOfClass(msgCls, "IPM.Note.Mobile.SMS.Alert.Info"))
				{
					list.Add(new ProportionedText(text3));
				}
				else
				{
					string text6 = StringNormalizer.TrimTrailingAndNormalizeEol(text);
					if (!string.IsNullOrEmpty(text6))
					{
						list.Add(new ProportionedText(text6, 30, 30, 0));
					}
					string text7 = StringNormalizer.TrimTrailingAndNormalizeEol(text2);
					if (!string.IsNullOrEmpty(text7))
					{
						list.Add(new ProportionedText("("));
						list.Add(new ProportionedText(text7, 28, 28, 0));
						list.Add(new ProportionedText(")"));
					}
					string text8 = StringNormalizer.TrimTrailingAndNormalizeEol(email.Subject);
					if (!string.IsNullOrEmpty(text8))
					{
						list.Add(new ProportionedText("\n"));
						list.Add(new ProportionedText("\n"));
						list.Add(new ProportionedText(text8, 30, 30, 0));
					}
					if (!string.IsNullOrEmpty(text3))
					{
						list.Add(new ProportionedText("\n"));
						list.Add(new ProportionedText("\n"));
						list.Add(new ProportionedText(text3, 0, 1000, 1));
					}
				}
			}
			else
			{
				list.Add(new ProportionedText(text3));
			}
			IL_AE2:
			MobileRecipient sender = null;
			MobileRecipient.TryParse(number, out sender);
			return new MessageItem(new Message(list.AsReadOnly()), sender, recipients, (flag3 || recipients.Count == 0) ? 1 : (10 / recipients.Count));
		}

		private CodingScheme GetCodingScheme(string text)
		{
			CodedText codedText = CodingSchemeInfo.GetCodingSchemeInfo(CodingScheme.GsmDefault).Coder.Code(text);
			if (codedText.CanBeCodedEntirely)
			{
				return CodingScheme.GsmDefault;
			}
			return CodingScheme.Unicode;
		}

		private const int SenderCharacters = 30;

		private const int SenderPhoneNumberCharacters = 28;

		private const int SubjectCharacters = 30;

		private const int MaximumSegments = 10;

		private static readonly PropertyDefinition[] InterestedPropertiesForSender = new PropertyDefinition[]
		{
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.PrimarySmtpAddress,
			ADOrgPersonSchema.MobilePhone
		};
	}
}
