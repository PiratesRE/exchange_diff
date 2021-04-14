using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal class MarkAsJunk : MultiStepServiceCommand<MarkAsJunkRequest, ItemId>
	{
		public MarkAsJunk(CallContext callContext, MarkAsJunkRequest request) : base(callContext, request)
		{
			this.session = callContext.SessionCache.GetMailboxIdentityMailboxSession();
			this.itemIds = request.ItemIds;
			this.isJunk = request.IsJunk;
			this.moveItem = request.MoveItem;
			this.sendCopy = request.SendCopy;
			this.junkMessageHeader = request.JunkMessageHeader;
			this.junkMessageBody = request.JunkMessageBody;
			ServiceCommandBase.ThrowIfNullOrEmpty<ItemId>(this.itemIds, "itemIds", "MarkAsJunk::Constructor");
		}

		internal override int StepCount
		{
			get
			{
				return this.itemIds.Count;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			MarkAsJunkResponse markAsJunkResponse = new MarkAsJunkResponse();
			markAsJunkResponse.AddResponses(base.Results);
			return markAsJunkResponse;
		}

		internal override ServiceResult<ItemId> Execute()
		{
			ItemId value = this.InternalMarkAsJunk(this.itemIds[base.CurrentStep]);
			return new ServiceResult<ItemId>(value);
		}

		private ItemId InternalMarkAsJunk(ItemId itemId)
		{
			ItemId result = null;
			JunkEmailRule junkEmailRule = this.session.JunkEmailRule;
			bool flag = false;
			PropertyDefinition[] propsToReturn = new PropertyDefinition[]
			{
				MessageItemSchema.TransportMessageHeaders
			};
			using (MessageItem messageItem = MessageItem.Bind(this.session, IdConverter.EwsIdToMessageStoreObjectId(itemId.Id), propsToReturn))
			{
				if (ServiceCommandBase.IsAssociated(messageItem))
				{
					throw new ServiceInvalidOperationException((CoreResources.IDs)3859804741U);
				}
				string text = (string)messageItem[MessageItemSchema.SenderSmtpAddress];
				JunkEmailCollection.ValidationProblem validationProblem = JunkEmailCollection.ValidationProblem.NoError;
				try
				{
					if (this.isJunk)
					{
						validationProblem = junkEmailRule.BlockedSenderEmailCollection.TryAdd(text);
						junkEmailRule.TrustedSenderEmailCollection.Remove(text);
					}
					else
					{
						junkEmailRule.BlockedSenderEmailCollection.Remove(text);
						validationProblem = junkEmailRule.TrustedSenderEmailCollection.TryAdd(text);
					}
				}
				catch (JunkEmailValidationException ex)
				{
					validationProblem = ex.Problem;
				}
				finally
				{
					switch (validationProblem)
					{
					case JunkEmailCollection.ValidationProblem.NoError:
					case JunkEmailCollection.ValidationProblem.Duplicate:
					case JunkEmailCollection.ValidationProblem.Empty:
						flag = true;
						break;
					}
				}
				if (flag)
				{
					junkEmailRule.Save();
					if (this.moveItem)
					{
						DefaultFolderType defaultFolderType = this.isJunk ? DefaultFolderType.JunkEmail : DefaultFolderType.Inbox;
						using (Folder folder = Folder.Bind(this.session, defaultFolderType))
						{
							AggregateOperationResult aggregateOperationResult = messageItem.Session.Move(folder.Session, folder.Id, true, new StoreId[]
							{
								messageItem.Id
							});
							if (aggregateOperationResult != null && aggregateOperationResult.GroupOperationResults != null && aggregateOperationResult.GroupOperationResults.Length == 1 && aggregateOperationResult.GroupOperationResults[0].OperationResult == OperationResult.Succeeded && aggregateOperationResult.GroupOperationResults[0].ResultObjectIds != null && aggregateOperationResult.GroupOperationResults[0].ResultObjectIds.Count == 1)
							{
								StoreId storeItemId = IdConverter.CombineStoreObjectIdWithChangeKey(aggregateOperationResult.GroupOperationResults[0].ResultObjectIds[0], aggregateOperationResult.GroupOperationResults[0].ResultChangeKeys[0]);
								result = IdConverter.ConvertStoreItemIdToItemId(storeItemId, this.session);
							}
						}
					}
					if (this.sendCopy)
					{
						string value = (string)messageItem[MessageItemSchema.TransportMessageHeaders];
						EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
						if (this.isJunk)
						{
							emailAddressWrapper.EmailAddress = Global.JunkMailReportingAddress;
						}
						else
						{
							emailAddressWrapper.EmailAddress = Global.NotJunkMailReportingAddress;
						}
						emailAddressWrapper.RoutingType = "SMTP";
						Participant participant = new Participant(emailAddressWrapper.Name, emailAddressWrapper.EmailAddress, emailAddressWrapper.RoutingType);
						try
						{
							using (MessageItem messageItem2 = MessageItem.Create(this.session, Folder.Bind(this.session, DefaultFolderType.Drafts).Id))
							{
								if (this.junkMessageHeader != null)
								{
									messageItem2.Subject = "[" + this.junkMessageHeader + "] " + messageItem.Subject;
								}
								else
								{
									messageItem2.Subject = messageItem.Subject;
								}
								using (ItemAttachment itemAttachment = messageItem2.AttachmentCollection.AddExistingItem(messageItem))
								{
									itemAttachment[AttachmentSchema.DisplayName] = messageItem.Subject;
									itemAttachment.Save();
								}
								using (TextWriter textWriter = messageItem2.Body.OpenTextWriter(BodyFormat.TextPlain))
								{
									if (this.junkMessageBody != null)
									{
										textWriter.WriteLine(this.junkMessageBody);
									}
									textWriter.Write(value);
								}
								messageItem2.Recipients.Add(participant);
								messageItem2.SendWithoutSavingMessage();
							}
						}
						catch (Exception arg)
						{
							ExTraceGlobals.ExceptionTracer.TraceError<Exception>((long)this.GetHashCode(), "MarkAsJunk.InternalMarkAsJunk called for exception: {0}", arg);
						}
					}
				}
			}
			return result;
		}

		private readonly bool isJunk;

		private readonly bool moveItem;

		private readonly bool sendCopy;

		private readonly string junkMessageHeader;

		private readonly string junkMessageBody;

		private readonly MailboxSession session;

		private IList<ItemId> itemIds;
	}
}
