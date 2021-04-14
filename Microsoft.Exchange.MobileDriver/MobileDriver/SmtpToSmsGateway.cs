using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.TextMessaging;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.MailboxTransport.StoreDriver;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class SmtpToSmsGateway : IMobileService
	{
		public SmtpToSmsGateway(SmtpToSmsGatewaySelector selector)
		{
			this.Manager = new SmtpToSmsGatewayManager(selector);
		}

		public SmtpToSmsGatewayManager Manager { get; private set; }

		IMobileServiceManager IMobileService.Manager
		{
			get
			{
				return this.Manager;
			}
		}

		public void Send(IList<TextSendingPackage> textPackages, Message message, MobileRecipient sender)
		{
			this.Send(textPackages, message, sender, DsnFormat.Default, null, null);
		}

		internal void Send(IList<TextSendingPackage> textPackages, Message message, MobileRecipient sender, DsnFormat dsnFormat, DsnParameters msgDsnParam, IDictionary<MobileRecipient, EnvelopeRecipient> recipientMap)
		{
			ExSmsCounters.NumberOfTextMessagesSentViaSmtp.Increment();
			bool needUpdateDsnParameter = msgDsnParam != null && null != recipientMap;
			Dictionary<string, MobileRecipient> addressMap = null;
			if (needUpdateDsnParameter)
			{
				addressMap = new Dictionary<string, MobileRecipient>(recipientMap.Count);
			}
			foreach (TextSendingPackage textSendingPackage in textPackages)
			{
				TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer = TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer.Body;
				List<string> list = new List<string>(textSendingPackage.Recipients.Count);
				foreach (MobileRecipient mobileRecipient in textSendingPackage.Recipients)
				{
					TextMessagingHostingDataServicesServiceSmtpToSmsGateway parameters = this.Manager.GetParameters(mobileRecipient);
					if (parameters == null)
					{
						mobileRecipient.Exceptions.Add(new MobileServiceTransientException(Strings.ErrorUnableDeliverForSmtpToSmsGateway(MobileRecipient.GetNumberString(mobileRecipient))));
					}
					else
					{
						textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer = parameters.MessageRendering.Container;
						string text = null;
						if (!this.TryBuildSmtpAddress(parameters.RecipientAddressing.SmtpAddress, mobileRecipient.E164Number, out text))
						{
							mobileRecipient.Exceptions.Add(new MobileServiceTransientException(Strings.ErrorUnableDeliverForSmtpToSmsGateway(MobileRecipient.GetNumberString(mobileRecipient))));
						}
						else
						{
							if (needUpdateDsnParameter)
							{
								addressMap[text] = mobileRecipient;
							}
							list.Add(text);
						}
					}
				}
				if (list.Count != 0)
				{
					foreach (Bookmark bookmark in textSendingPackage.BookmarkRetriever.Parts)
					{
						using (MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties))
						{
							messageItem.ClassName = "IPM.Note.Mobile.SMS";
							messageItem.Sender = new Participant(this.Manager.Selector.Principal);
							messageItem.From = new Participant(this.Manager.Selector.Principal);
							foreach (string emailAddress in list)
							{
								Participant participant = new Participant(null, emailAddress, ProxyAddressPrefix.Smtp.PrimaryPrefix);
								Recipient recipient = messageItem.Recipients.Add(participant, RecipientItemType.To);
								recipient[ItemSchema.Responsibility] = true;
							}
							if (textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer == TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingContainer.Body)
							{
								using (TextWriter textWriter = messageItem.Body.OpenTextWriter(new BodyWriteConfiguration(BodyFormat.TextPlain, Charset.Unicode.Name)))
								{
									textWriter.WriteLine(bookmark.ToString());
									goto IL_2DA;
								}
							}
							messageItem.Subject = bookmark.ToString();
							using (TextWriter textWriter2 = messageItem.Body.OpenTextWriter(new BodyWriteConfiguration(BodyFormat.TextPlain, Charset.Unicode.Name)))
							{
								textWriter2.Write("\r\n");
							}
							IL_2DA:
							messageItem.Save(SaveMode.NoConflictResolution);
							using (MemorySubmissionItem memorySubmissionItem = new MemorySubmissionItem(messageItem, this.Manager.Selector.Principal.MailboxInfo.OrganizationId))
							{
								memorySubmissionItem.Submit(MessageTrackingSource.AGENT, delegate(TransportMailItem mailItem, bool isValid)
								{
									mailItem.DsnFormat = dsnFormat;
									if (needUpdateDsnParameter)
									{
										SmtpToSmsGateway.UpdateDsnParameters(mailItem, msgDsnParam, recipientMap, addressMap);
									}
									return true;
								}, null);
							}
						}
					}
				}
			}
		}

		private static void UpdateDsnParameters(TransportMailItem tmi, DsnParameters msgDsnParam, IDictionary<MobileRecipient, EnvelopeRecipient> mobileRecipToTransportRecip, IDictionary<string, MobileRecipient> addressToMobileRecip)
		{
			tmi.DsnParameters = TransportAgentWrapper.CloneDsnParameters(msgDsnParam);
			foreach (MailRecipient mailRecipient in tmi.Recipients)
			{
				MobileRecipient key = addressToMobileRecip[(string)mailRecipient.Email];
				if (mobileRecipToTransportRecip.ContainsKey(key))
				{
					EnvelopeRecipient envelopeRecipient = mobileRecipToTransportRecip[key];
					MailRecipient mailRecipient2 = TransportAgentWrapper.CastEnvelopeRecipientToMailRecipient(envelopeRecipient);
					mailRecipient.DsnRequested = mailRecipient2.DsnRequested;
					TransportAgentWrapper.AddDsnParameters(mailRecipient, mailRecipient2.DsnParameters);
				}
			}
		}

		private bool TryBuildSmtpAddress(string template, E164Number number, out string address)
		{
			if (string.IsNullOrEmpty(template))
			{
				throw new ArgumentNullException("template");
			}
			if (null == number)
			{
				throw new ArgumentNullException("number");
			}
			address = null;
			StringBuilder stringBuilder = new StringBuilder(template.Length + number.Number.Length);
			bool flag = true;
			int num = 0;
			while (flag && template.Length > num)
			{
				char c = template[num];
				if ('%' != c)
				{
					stringBuilder.Append(c);
				}
				else
				{
					if (template.Length - 1 == num)
					{
						flag = false;
						break;
					}
					char c2 = template[1 + num];
					if (c2 <= 'C')
					{
						if (c2 != '%')
						{
							if (c2 != 'C')
							{
								goto IL_CD;
							}
							goto IL_AF;
						}
						else
						{
							stringBuilder.Append('%');
						}
					}
					else
					{
						if (c2 != 'N')
						{
							if (c2 == 'c')
							{
								goto IL_AF;
							}
							if (c2 != 'n')
							{
								goto IL_CD;
							}
						}
						stringBuilder.Append(number.SignificantNumber);
					}
					IL_CF:
					num++;
					goto IL_D3;
					IL_AF:
					stringBuilder.Append(number.CountryCode);
					goto IL_CF;
					IL_CD:
					flag = false;
					goto IL_CF;
				}
				IL_D3:
				num++;
			}
			string text = stringBuilder.ToString();
			if (flag)
			{
				flag = SmtpAddress.IsValidSmtpAddress(text);
			}
			if (flag)
			{
				address = text;
			}
			return flag;
		}
	}
}
