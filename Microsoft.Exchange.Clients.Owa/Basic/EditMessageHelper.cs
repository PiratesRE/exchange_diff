using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal static class EditMessageHelper
	{
		public static string ExecutePostCommand(string postCommand, HttpRequest request, MessageItem messageItem, UserContext userContext)
		{
			if (string.IsNullOrEmpty(postCommand))
			{
				throw new ArgumentNullException("postCommand");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (messageItem == null)
			{
				throw new ArgumentNullException("messageItem");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			string result = null;
			if (postCommand != null)
			{
				if (<PrivateImplementationDetails>{83F8DD10-61AE-4283-B829-2F464F055E61}.$$method0x60001aa-1 == null)
				{
					<PrivateImplementationDetails>{83F8DD10-61AE-4283-B829-2F464F055E61}.$$method0x60001aa-1 = new Dictionary<string, int>(6)
					{
						{
							"rmrcp",
							0
						},
						{
							"addanrrcp",
							1
						},
						{
							"delmrrrcp",
							2
						},
						{
							"addmrrrcp",
							3
						},
						{
							"chknm",
							4
						},
						{
							"sv",
							5
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{83F8DD10-61AE-4283-B829-2F464F055E61}.$$method0x60001aa-1.TryGetValue(postCommand, out num))
				{
					switch (num)
					{
					case 0:
					{
						string formParameter = Utilities.GetFormParameter(request, "hidrmrcp");
						EditMessageHelper.RemoveRecipientByRecipientIdString(messageItem, formParameter);
						EditMessageHelper.UpdateMessage(messageItem, userContext, request, out result);
						break;
					}
					case 1:
					{
						string formParameter = Utilities.GetFormParameter(request, "hidrmrcp");
						EditMessageHelper.RemoveRecipientByRecipientIdString(messageItem, formParameter);
						int intValueFromForm = RequestParser.GetIntValueFromForm(request, "hidaddrcptype");
						ResolvedRecipientDetail[] array = ResolvedRecipientDetail.ParseFromForm(request, "hidaddrcp", true);
						if (array.Length != 1)
						{
							throw new OwaInvalidRequestException("Only one resolved recipient can be passed to server by one click, currently there are " + array.Length + " recipients clicked");
						}
						EditMessageHelper.AddResolvedRecipients(messageItem.Recipients, (RecipientItemType)intValueFromForm, array, userContext);
						EditMessageHelper.UpdateMessage(messageItem, userContext, request, out result);
						break;
					}
					case 2:
					{
						ResolvedRecipientDetail[] array = ResolvedRecipientDetail.ParseFromForm(request, "hidaddrcp", true);
						AutoCompleteCache autoCompleteCache = AutoCompleteCache.TryGetCache(OwaContext.Current.UserContext);
						if (autoCompleteCache != null)
						{
							autoCompleteCache.RemoveResolvedRecipients(array);
							autoCompleteCache.Commit(true);
						}
						EditMessageHelper.UpdateMessage(messageItem, userContext, request, out result);
						break;
					}
					case 3:
					{
						int intValueFromForm = RequestParser.GetIntValueFromForm(request, "hidaddrcptype");
						ResolvedRecipientDetail[] array = ResolvedRecipientDetail.ParseFromForm(request, "hidaddrcp", true);
						EditMessageHelper.AddResolvedRecipients(messageItem.Recipients, (RecipientItemType)intValueFromForm, array, userContext);
						EditMessageHelper.UpdateMessage(messageItem, userContext, request, out result);
						break;
					}
					case 4:
					case 5:
						EditMessageHelper.UpdateMessage(messageItem, userContext, request, out result);
						break;
					default:
						goto IL_1F5;
					}
					return result;
				}
			}
			IL_1F5:
			throw new OwaInvalidRequestException("Invalid command form parameter");
		}

		public static bool UpdateMessage(MessageItem message, UserContext userContext, HttpRequest request, out string errorMessage)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			bool flag = false;
			bool isExistingItem = message.Id != null;
			object formParameter = Utilities.GetFormParameter(request, "txtsbj", false);
			if (formParameter != null)
			{
				try
				{
					message.Subject = (string)formParameter;
				}
				catch (PropertyValidationException ex)
				{
					throw new OwaInvalidRequestException(ex.Message);
				}
			}
			formParameter = Utilities.GetFormParameter(request, "hidmsgimp", false);
			string text = formParameter as string;
			if (!string.IsNullOrEmpty(text))
			{
				int importance;
				if (!int.TryParse(text, out importance))
				{
					throw new OwaInvalidRequestException("Invalid form parameter:importance");
				}
				message.Importance = (Importance)importance;
			}
			formParameter = Utilities.GetFormParameter(request, "txtbdy", false);
			if (formParameter != null)
			{
				ItemUtility.SetItemBody(message, BodyFormat.TextPlain, (string)formParameter);
			}
			AttachmentUtility.PromoteInlineAttachments(message);
			flag |= EditMessageHelper.AddMessageRecipients(message.Recipients, RecipientItemType.To, "txtto", userContext, request);
			flag |= EditMessageHelper.AddMessageRecipients(message.Recipients, RecipientItemType.Cc, "txtcc", userContext, request);
			flag |= EditMessageHelper.AddMessageRecipients(message.Recipients, RecipientItemType.Bcc, "txtbcc", userContext, request);
			errorMessage = EditMessageHelper.SaveItem(message, isExistingItem);
			return flag;
		}

		public static MessageItem CreateDraft(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			MessageItem messageItem = null;
			bool flag = true;
			try
			{
				messageItem = MessageItem.Create(userContext.MailboxSession, userContext.DraftsFolderId);
				messageItem[ItemSchema.ConversationIndexTracking] = true;
				flag = false;
			}
			finally
			{
				if (flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return messageItem;
		}

		private static string SaveItem(Item item, bool isExistingItem)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			ConflictResolutionResult conflictResolutionResult = item.Save(SaveMode.ResolveConflicts);
			item.Load();
			if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
			{
				if (ExTraceGlobals.MailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					ExTraceGlobals.MailTracer.TraceDebug(0L, "Saving item failed due to conflict resolution.");
				}
				return LocalizedStrings.GetNonEncoded(-482397486);
			}
			if (Globals.ArePerfCountersEnabled)
			{
				if (isExistingItem)
				{
					OwaSingleCounters.ItemsUpdated.Increment();
				}
				else
				{
					OwaSingleCounters.ItemsCreated.Increment();
				}
			}
			return string.Empty;
		}

		private static bool AddMessageRecipients(RecipientCollection recipients, RecipientItemType recipientItemType, string wellName, UserContext userContext, HttpRequest request)
		{
			bool flag = false;
			Participant participant = null;
			string formParameter = Utilities.GetFormParameter(request, wellName, false);
			if (string.IsNullOrEmpty(formParameter))
			{
				return false;
			}
			ArrayList arrayList = new ArrayList();
			RecipientWell.ResolveRecipients(formParameter, arrayList, userContext, userContext.UserOptions.CheckNameInContactsFirst);
			for (int i = 0; i < arrayList.Count; i++)
			{
				RecipientWellNode recipientWellNode = (RecipientWellNode)arrayList[i];
				flag |= Utilities.CreateExchangeParticipant(out participant, recipientWellNode.DisplayName, recipientWellNode.RoutingAddress, recipientWellNode.RoutingType, recipientWellNode.AddressOrigin, recipientWellNode.StoreObjectId, recipientWellNode.EmailAddressIndex);
				if (participant != null)
				{
					recipients.Add(participant, recipientItemType);
				}
			}
			return flag;
		}

		public static void AddResolvedRecipients(RecipientCollection recipients, RecipientItemType recipientItemType, ResolvedRecipientDetail[] resolvedRecipientDetails, UserContext userContext)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			if (resolvedRecipientDetails == null)
			{
				throw new ArgumentNullException("resolvedRecipientDetails");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			RecipientCache recipientCache = AutoCompleteCache.TryGetCache(OwaContext.Current.UserContext);
			for (int i = 0; i < resolvedRecipientDetails.Length; i++)
			{
				Participant participant = resolvedRecipientDetails[i].ToParticipant();
				if (!(participant == null))
				{
					recipients.Add(participant, recipientItemType);
					if (userContext.UserOptions.AddRecipientsToAutoCompleteCache && recipientCache != null)
					{
						recipientCache.AddEntry(resolvedRecipientDetails[i].DisplayName, resolvedRecipientDetails[i].SmtpAddress, resolvedRecipientDetails[i].RoutingAddress, string.Empty, resolvedRecipientDetails[i].RoutingType, resolvedRecipientDetails[i].AddressOrigin, resolvedRecipientDetails[i].RecipientFlags, resolvedRecipientDetails[i].ItemId, resolvedRecipientDetails[i].EmailAddressIndex);
					}
				}
			}
			if (recipientCache != null && recipientCache.IsDirty)
			{
				recipientCache.Commit(true);
			}
		}

		public static bool RemoveRecipientByRecipientIdString(MessageItem messageItem, string idString)
		{
			if (messageItem == null)
			{
				throw new ArgumentNullException("messageItem");
			}
			if (string.IsNullOrEmpty(idString))
			{
				throw new ArgumentException("idString is null or empty", "idString");
			}
			RecipientItemType recipientItemType = RecipientItemType.Unknown;
			int num = -1;
			ItemRecipientWell.ParseRecipientIdString(idString, out recipientItemType, out num);
			if (recipientItemType == RecipientItemType.Unknown || num < 0)
			{
				return false;
			}
			bool property = ItemUtility.GetProperty<bool>(messageItem, MessageItemSchema.IsResend, false);
			Recipient recipient = null;
			int num2 = 0;
			foreach (Recipient recipient2 in messageItem.Recipients)
			{
				if (recipient2.RecipientItemType.Equals(recipientItemType) && (!property || !recipient2.Submitted))
				{
					if (num2 == num)
					{
						recipient = recipient2;
						break;
					}
					num2++;
				}
			}
			if (recipient != null)
			{
				messageItem.Recipients.Remove(recipient);
				return true;
			}
			return false;
		}

		public static string SendMessage(UserContext userContext, MessageItem message)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (message.Recipients.Count == 0)
			{
				return LocalizedStrings.GetNonEncoded(1878192149);
			}
			if (Utilities.RecipientsOnlyHaveEmptyPDL<Recipient>(userContext, message.Recipients))
			{
				return LocalizedStrings.GetNonEncoded(1389137820);
			}
			ExTraceGlobals.MailTracer.TraceDebug(0L, "Sending message");
			try
			{
				message.Send();
			}
			catch (StoragePermanentException exception)
			{
				return Utilities.GetExceptionHandlingInformation(exception, userContext.MailboxIdentity).Message;
			}
			catch (StorageTransientException exception2)
			{
				return Utilities.GetExceptionHandlingInformation(exception2, userContext.MailboxIdentity).Message;
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.MessagesSent.Increment();
			}
			return null;
		}

		public static PreFormActionResponse RedirectToRecipient(OwaContext owaContext, Item item, AddressBook.Mode editMode)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Item;
			preFormActionResponse.Type = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "sT", false);
			string formParameter = Utilities.GetFormParameter(owaContext.HttpContext.Request, "hidrcptid");
			preFormActionResponse.AddParameter("id", formParameter);
			if (item.Id != null)
			{
				preFormActionResponse.AddParameter("oId", item.Id.ObjectId.ToBase64String());
				preFormActionResponse.AddParameter("oCk", item.Id.ChangeKeyAsBase64String());
			}
			preFormActionResponse.AddParameter("oT", owaContext.FormsRegistryContext.Type);
			preFormActionResponse.AddParameter("oS", "Draft");
			PreFormActionResponse preFormActionResponse2 = preFormActionResponse;
			string name = "ctx";
			int num = (int)editMode;
			preFormActionResponse2.AddParameter(name, num.ToString());
			return preFormActionResponse;
		}

		public static PreFormActionResponse RedirectToPeoplePicker(OwaContext owaContext, Item item, AddressBook.Mode editMode)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			PreFormActionResponse preFormActionResponse = new PreFormActionResponse();
			preFormActionResponse.ApplicationElement = ApplicationElement.Dialog;
			preFormActionResponse.Type = "AddressBook";
			PreFormActionResponse preFormActionResponse2 = preFormActionResponse;
			string name = "ctx";
			int num = (int)editMode;
			preFormActionResponse2.AddParameter(name, num.ToString());
			if (item.Id != null)
			{
				preFormActionResponse.AddParameter("id", item.Id.ObjectId.ToBase64String());
			}
			string queryStringParameter = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "sch", false);
			if (!string.IsNullOrEmpty(queryStringParameter))
			{
				preFormActionResponse.AddParameter("sch", queryStringParameter);
				string queryStringParameter2 = Utilities.GetQueryStringParameter(owaContext.HttpContext.Request, "ab", false);
				if (!string.IsNullOrEmpty(queryStringParameter2))
				{
					preFormActionResponse.AddParameter("ab", queryStringParameter2);
				}
			}
			string formParameter = Utilities.GetFormParameter(owaContext.HttpContext.Request, "hidrw", false);
			if (!string.IsNullOrEmpty(formParameter))
			{
				preFormActionResponse.AddParameter("rw", formParameter);
			}
			return preFormActionResponse;
		}

		private const string Subject = "txtsbj";

		private const string Importance = "hidmsgimp";

		private const string To = "txtto";

		private const string Cc = "txtcc";

		private const string Bcc = "txtbcc";

		private const string Body = "txtbdy";

		private const string RemoveRecipientFormParameter = "hidrmrcp";

		private const string AddRecipientTypeFormParameter = "hidaddrcptype";

		private const string AddRecipientFormParameter = "hidaddrcp";

		private const string RecipientWellFormParameter = "hidrw";
	}
}
