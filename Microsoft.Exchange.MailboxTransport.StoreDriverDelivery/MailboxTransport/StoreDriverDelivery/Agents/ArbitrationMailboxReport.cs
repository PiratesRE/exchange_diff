using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class ArbitrationMailboxReport
	{
		public static EmailMessage GenerateContentReport(SmtpAddress arbitrationAddress, SmtpAddress reportToAddress, StoreSession session, bool isArbitration)
		{
			if (arbitrationAddress.IsValidAddress && reportToAddress.IsValidAddress)
			{
				EmailMessage emailMessage = EmailMessage.Create();
				RoutingAddress address = GlobalConfigurationBase<MicrosoftExchangeRecipient, MicrosoftExchangeRecipientConfiguration>.Instance.Address;
				emailMessage.From = new EmailRecipient(null, (string)address);
				emailMessage.To.Add(new EmailRecipient(null, (string)reportToAddress));
				emailMessage.Subject = "Arbitration Mailbox Content Report: " + arbitrationAddress.ToString();
				using (Stream contentWriteStream = emailMessage.Body.GetContentWriteStream())
				{
					using (StreamWriter streamWriter = new StreamWriter(contentWriteStream))
					{
						if (!isArbitration)
						{
							streamWriter.WriteLine("This mailbox is not an approval arbitration mailbox. This report is only generated for approval arbitration mailboxes.");
							return emailMessage;
						}
						Attachment attachment = emailMessage.Attachments.Add("formattedDetails.txt");
						using (Stream contentWriteStream2 = attachment.GetContentWriteStream())
						{
							using (Folder folder = Folder.Bind((MailboxSession)session, DefaultFolderType.Inbox))
							{
								using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
								{
									ItemSchema.Id
								}))
								{
									object[][] rows = queryResult.GetRows(10000);
									int num = 0;
									int num2 = 0;
									TimeSpan t = default(TimeSpan);
									TimeSpan t2 = default(TimeSpan);
									while (rows != null && rows.Length > 0)
									{
										foreach (object[] array2 in rows)
										{
											StoreObjectId objectId = ((VersionedId)array2[0]).ObjectId;
											using (MessageItem messageItem = MessageItem.Bind(session, objectId, ArbitrationMailboxReport.ApprovalProperties))
											{
												string valueOrDefault = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestor);
												string valueOrDefault2 = messageItem.GetValueOrDefault<string>(ItemSchema.DisplayCc);
												string valueOrDefault3 = messageItem.GetValueOrDefault<string>(ItemSchema.Subject);
												string valueOrDefault4 = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalAllowedDecisionMakers);
												string valueOrDefault5 = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalDecisionMakersNdred);
												string valueOrDefault6 = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestMessageId);
												string valueOrDefault7 = messageItem.GetValueOrDefault<string>(ItemSchema.InternetMessageId);
												string valueOrDefault8 = messageItem.GetValueOrDefault<string>(ItemSchema.InternetReferences);
												string valueOrDefault9 = messageItem.GetValueOrDefault<string>(MessageItemSchema.ApprovalDecisionMaker);
												ApprovalStatus? valueAsNullable = messageItem.GetValueAsNullable<ApprovalStatus>(MessageItemSchema.ApprovalStatus);
												ExDateTime? valueAsNullable2 = messageItem.GetValueAsNullable<ExDateTime>(MessageItemSchema.ApprovalDecisionTime);
												ExDateTime? valueAsNullable3 = messageItem.GetValueAsNullable<ExDateTime>(ItemSchema.ReceivedTime);
												ExDateTime? valueAsNullable4 = messageItem.GetValueAsNullable<ExDateTime>(ItemSchema.RetentionDate);
												if (valueAsNullable3 != null)
												{
													if (valueAsNullable2 == null)
													{
														ExDateTime utcNow = ExDateTime.UtcNow;
														t2 += utcNow - valueAsNullable3.Value;
														num2++;
													}
													else if (valueAsNullable3 <= valueAsNullable2)
													{
														t += valueAsNullable2.Value - valueAsNullable3.Value;
														num++;
													}
												}
												streamWriter.WriteLine("Initiation Message ID:" + valueOrDefault7);
												streamWriter.WriteLine("Original Message ID:" + valueOrDefault8);
												streamWriter.WriteLine("Sender:" + valueOrDefault);
												streamWriter.WriteLine("Moderated Recipient Addresses:" + valueOrDefault2);
												streamWriter.WriteLine("Message Subject:" + valueOrDefault3);
												streamWriter.WriteLine("Moderators:" + valueOrDefault4);
												streamWriter.WriteLine("Moderators (total; not delivered; away):" + valueOrDefault5);
												streamWriter.WriteLine("Approval Request Message ID:" + valueOrDefault6);
												streamWriter.WriteLine("Decision maker:" + valueOrDefault9);
												streamWriter.WriteLine("Decision status:" + valueAsNullable);
												streamWriter.WriteLine("Decision Time:" + valueAsNullable2);
												streamWriter.WriteLine("Received Time:" + valueAsNullable3);
												streamWriter.WriteLine("Expiry Time:" + valueAsNullable4);
												streamWriter.WriteLine();
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault7);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault8);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault2);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault3);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault4);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault5);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault6);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault9);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueAsNullable);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueOrDefault6);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueAsNullable2);
												ArbitrationMailboxReport.CheckAndWrite(contentWriteStream2, valueAsNullable3);
												Utf8Csv.EncodeEscapeAndWriteLine(contentWriteStream2, (valueAsNullable4 != null) ? valueAsNullable4.ToString() : string.Empty);
											}
										}
										rows = queryResult.GetRows(10000);
									}
									streamWriter.WriteLine("Summary:");
									streamWriter.WriteLine("Messages with a decision:" + num);
									if (num > 0)
									{
										streamWriter.WriteLine("Average decision time:" + TimeSpan.FromSeconds(t.TotalSeconds / (double)num));
									}
									streamWriter.WriteLine("Messages without a decision:" + num2);
									if (num2 > 0)
									{
										streamWriter.WriteLine("Average waiting time:" + TimeSpan.FromSeconds(t2.TotalSeconds / (double)num2));
									}
								}
							}
						}
						return emailMessage;
					}
				}
			}
			return null;
		}

		private static void CheckAndWrite(Stream attachmentStream, object propertyValue)
		{
			if (propertyValue == null)
			{
				Utf8Csv.EncodeEscapeAndWrite(attachmentStream, string.Empty);
			}
			else
			{
				Utf8Csv.EncodeEscapeAndWrite(attachmentStream, propertyValue.ToString());
			}
			Utf8Csv.WriteByte(attachmentStream, 44);
		}

		private const byte CommaSeparator = 44;

		private const string AttachmentName = "formattedDetails.txt";

		internal static readonly PropertyDefinition[] ApprovalProperties = new PropertyDefinition[]
		{
			MessageItemSchema.ApprovalAllowedDecisionMakers,
			MessageItemSchema.ApprovalRequestor,
			MessageItemSchema.ApprovalDecisionMaker,
			MessageItemSchema.ApprovalDecisionTime,
			MessageItemSchema.ApprovalDecision,
			MessageItemSchema.ApprovalStatus,
			MessageItemSchema.ApprovalDecisionMakersNdred,
			MessageItemSchema.ApprovalApplicationId,
			MessageItemSchema.ApprovalApplicationData,
			MessageItemSchema.ApprovalRequestMessageId,
			MessageItemSchema.ExpiryTime,
			ItemSchema.InternetMessageId,
			ItemSchema.InternetReferences,
			ItemSchema.DisplayCc,
			ItemSchema.RetentionDate
		};
	}
}
