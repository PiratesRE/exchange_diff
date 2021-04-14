using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Inference;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Inference.Common;
using Microsoft.Exchange.Inference.Mdb;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Inference.PeopleRelevance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecipientCacheContactWriter : BaseComponent
	{
		internal RecipientCacheContactWriter()
		{
			this.DiagnosticsSession.ComponentName = "RecipientCacheContactWriter";
			this.DiagnosticsSession.Tracer = ExTraceGlobals.RecipientCacheContactWriterTracer;
			this.MaxContactUpdatesCount = PeopleRelevanceConfig.Instance.MaxContactUpdatesCount;
		}

		public override string Description
		{
			get
			{
				return "RecipientCacheContactWriter updates the recipient cache contacts based on the capture flags set byt PeopleRelevanceClassifier.";
			}
		}

		public override string Name
		{
			get
			{
				return "RecipientCacheContactWriter";
			}
		}

		public int MaxContactUpdatesCount { get; set; }

		protected override void InternalProcessDocument(DocumentContext data)
		{
			this.DiagnosticsSession.TraceDebug<IIdentity>("Processing document - {0}", data.Document.Identity);
			DocumentProcessingContext documentProcessingContext = (DocumentProcessingContext)data.AsyncResult.AsyncState;
			Util.ThrowOnNullArgument(documentProcessingContext, "processingContext");
			Util.ThrowOnNullArgument(documentProcessingContext.Session, "session");
			string text = documentProcessingContext.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			object obj;
			if (!data.Document.TryGetProperty(PeopleRelevanceSchema.ContactList, out obj))
			{
				this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - Contact list is empty. No Recipient Cache changes to process.", text), new object[0]);
				return;
			}
			this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - Processing the Recipient Cache chages.", text), new object[0]);
			IDictionary<string, IInferenceRecipient> contactList = (IDictionary<string, IInferenceRecipient>)obj;
			data.Document.TryGetProperty(PeopleRelevanceSchema.TopRankedContacts, out obj);
			IList<string> topRanked = (IList<string>)obj;
			RecipientCacheContactWriter.RecipientCacheContactWriterContext recipientCacheContactWriterContext = new RecipientCacheContactWriter.RecipientCacheContactWriterContext(documentProcessingContext.Session, this.DiagnosticsSession)
			{
				ItemsToDelete = new Dictionary<StoreId, IInferenceRecipient>(),
				DeletedItemsToDelete = new HashSet<StoreId>()
			};
			try
			{
				this.ValidateRecipientCache(data, contactList, topRanked, recipientCacheContactWriterContext);
				RecipientCacheContactWriter.RecipientCacheChangeList recipientCacheChangeList = this.DetermineChangeList(contactList, topRanked, recipientCacheContactWriterContext);
				this.ProcessStoreChanges(recipientCacheChangeList, recipientCacheContactWriterContext);
				this.LogRecipientCacheUpdateSummary(recipientCacheChangeList, text);
			}
			finally
			{
				recipientCacheContactWriterContext.DisposeAll();
			}
		}

		private static void ClearCaptureFlags(IInferenceRecipient recipient)
		{
			recipient.CaptureFlag = 0;
			recipient.RelevanceCategoryAtLastCapture = recipient.RelevanceCategory;
			recipient.HasUpdatedData = false;
		}

		private static bool IsGlobalException(LocalizedException exception)
		{
			return exception is MailboxUnavailableException || exception is SessionDeadException || exception is ConnectionFailedPermanentException || exception is ObjectNotInitializedException;
		}

		private static StoreId FindContactAndRemoveDuplicates(IInferenceRecipient recipient, bool deleteAll, RecipientCacheContactWriter.RecipientCacheContactWriterContext context)
		{
			if (!context.SortedRecipientCacheData.ContainsKey(recipient.SmtpAddress))
			{
				return null;
			}
			StoreId storeId = (StoreId)context.SortedRecipientCacheData[recipient.SmtpAddress][0][0];
			if (deleteAll)
			{
				context.ItemsToDelete[storeId] = recipient;
			}
			RecipientCacheContactWriter.RemoveDuplicates(recipient.SmtpAddress, context);
			return storeId;
		}

		private static void RemoveDuplicates(string smtp, RecipientCacheContactWriter.RecipientCacheContactWriterContext context)
		{
			for (int i = 1; i < context.SortedRecipientCacheData[smtp].Count; i++)
			{
				StoreId key = (StoreId)context.SortedRecipientCacheData[smtp][i][0];
				context.ItemsToDelete[key] = null;
			}
		}

		private static void SetContactProperties(Contact contact, IInferenceRecipient recipient)
		{
			string displayName;
			if (!string.IsNullOrEmpty(recipient.DisplayName))
			{
				displayName = recipient.DisplayName;
			}
			else
			{
				displayName = recipient.SmtpAddress;
			}
			contact.EmailAddresses[EmailAddressIndex.Email1] = new Participant(displayName, recipient.SmtpAddress, "SMTP");
			contact.DisplayName = displayName;
			contact.PersonType = RecipientCacheContactWriter.GetPersonType(recipient);
			contact.RelevanceScore = recipient.RecipientRank;
			contact.PartnerNetworkId = WellKnownNetworkNames.RecipientCache;
			contact.SetOrDeleteProperty(ContactSchema.IMAddress, recipient.SipUri);
			contact.SetOrDeleteProperty(ContactSchema.Account, recipient.Alias);
		}

		private static PersonType GetPersonType(IInferenceRecipient recipient)
		{
			if (recipient.IsDistributionList)
			{
				return PersonType.DistributionList;
			}
			RecipientDisplayType recipientDisplayType = recipient.RecipientDisplayType;
			if (recipientDisplayType == RecipientDisplayType.SyncedConferenceRoomMailbox || recipientDisplayType == RecipientDisplayType.ConferenceRoomMailbox)
			{
				return PersonType.Room;
			}
			if (recipientDisplayType != RecipientDisplayType.GroupMailboxUser)
			{
				return PersonType.Person;
			}
			return PersonType.ModernGroup;
		}

		private static bool UpdateEmailAddress(Contact contact, string oldEmailAddress, string newEmailAddress, string oldEmailAddressDisplayName, string newDisplayName, StringBuilder updates)
		{
			if ((!string.IsNullOrEmpty(newEmailAddress) && !StringComparer.OrdinalIgnoreCase.Equals(newEmailAddress, oldEmailAddress)) || (!string.IsNullOrEmpty(newDisplayName) && !StringComparer.OrdinalIgnoreCase.Equals(newDisplayName, oldEmailAddressDisplayName)))
			{
				contact.EmailAddresses[EmailAddressIndex.Email1] = new Participant(newDisplayName, newEmailAddress, "SMTP");
				updates.AppendFormat("E:{0}->{1};", oldEmailAddress, newEmailAddress);
				return true;
			}
			return false;
		}

		private static bool UpdateDisplayName(Contact contact, string oldDisplayName, string displayName, StringBuilder updates)
		{
			if (!string.IsNullOrEmpty(displayName) && !StringComparer.OrdinalIgnoreCase.Equals(displayName, oldDisplayName))
			{
				contact.DisplayName = displayName;
				updates.AppendFormat("D:{0}->{1};", oldDisplayName, displayName);
				return true;
			}
			return false;
		}

		private static bool UpdateIMAddress(Contact contact, string newIMAddress, StringBuilder updates)
		{
			string valueOrDefault = contact.GetValueOrDefault<string>(ContactSchema.IMAddress, string.Empty);
			if (!string.IsNullOrEmpty(newIMAddress) && !StringComparer.OrdinalIgnoreCase.Equals(newIMAddress, valueOrDefault))
			{
				contact.ImAddress = newIMAddress;
				updates.AppendFormat("I:{0}->{1};", valueOrDefault, newIMAddress);
				return true;
			}
			return false;
		}

		private static bool UpdateAlias(Contact contact, string newAlias, StringBuilder updates)
		{
			string valueOrDefault = contact.GetValueOrDefault<string>(ContactSchema.Account, string.Empty);
			if (!string.IsNullOrEmpty(newAlias) && !StringComparer.OrdinalIgnoreCase.Equals(newAlias, valueOrDefault))
			{
				contact.SafeSetProperty(ContactSchema.Account, newAlias);
				updates.AppendFormat("A:{0}->{1};", valueOrDefault, newAlias);
				return true;
			}
			return false;
		}

		private static bool UpdateRelevance(Contact contact, int newRelevance, StringBuilder updates)
		{
			object obj = contact.TryGetProperty(ContactSchema.RelevanceScore);
			bool flag = false;
			int num;
			if (obj is PropertyError)
			{
				flag = true;
				num = int.MaxValue;
			}
			else
			{
				num = (int)obj;
				if (newRelevance != num)
				{
					flag = true;
				}
			}
			if (flag)
			{
				contact.SafeSetProperty(ContactSchema.RelevanceScore, newRelevance);
				updates.AppendFormat("R:{0}->{1};", num, newRelevance);
				return true;
			}
			return false;
		}

		private static bool UpdatePersonaType(Contact contact, IInferenceRecipient recipient, StringBuilder updates)
		{
			object obj = contact.TryGetProperty(ContactSchema.PersonType);
			PersonType personType = PersonType.Unknown;
			PersonType personType2 = RecipientCacheContactWriter.GetPersonType(recipient);
			bool flag;
			if (obj is PropertyError)
			{
				flag = true;
			}
			else
			{
				personType = (PersonType)obj;
				flag = (personType != personType2);
			}
			if (flag)
			{
				contact.PersonType = personType2;
				updates.AppendFormat("L:{0}->{1};", personType, personType2);
				return true;
			}
			return false;
		}

		private static bool UpdateNetworkId(Contact contact, StringBuilder updates)
		{
			string valueOrDefault = contact.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
			if (!StringComparer.Ordinal.Equals(WellKnownNetworkNames.RecipientCache, valueOrDefault))
			{
				contact.SafeSetProperty(ContactSchema.PartnerNetworkId, WellKnownNetworkNames.RecipientCache);
				updates.AppendFormat("N:{0}->{1};", valueOrDefault, WellKnownNetworkNames.RecipientCache);
				return true;
			}
			return false;
		}

		private static bool UpdateContactProperties(Contact contact, IInferenceRecipient recipient, out string updatedProperties)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string valueOrDefault = contact.GetValueOrDefault<string>(StoreObjectSchema.DisplayName, string.Empty);
			string valueOrDefault2 = contact.GetValueOrDefault<string>(ContactSchema.Email1EmailAddress, string.Empty);
			string valueOrDefault3 = contact.GetValueOrDefault<string>(ContactSchema.Email1DisplayName, string.Empty);
			string text;
			if (!string.IsNullOrEmpty(recipient.DisplayName))
			{
				text = recipient.DisplayName;
			}
			else if (string.IsNullOrEmpty(valueOrDefault3))
			{
				text = recipient.SmtpAddress;
			}
			else
			{
				text = valueOrDefault3;
			}
			bool flag = RecipientCacheContactWriter.UpdateEmailAddress(contact, valueOrDefault2, recipient.SmtpAddress, valueOrDefault3, text, stringBuilder);
			bool flag2 = RecipientCacheContactWriter.UpdateDisplayName(contact, valueOrDefault, text, stringBuilder);
			bool flag3 = RecipientCacheContactWriter.UpdateIMAddress(contact, recipient.SipUri, stringBuilder);
			bool flag4 = RecipientCacheContactWriter.UpdateAlias(contact, recipient.Alias, stringBuilder);
			bool flag5 = RecipientCacheContactWriter.UpdateRelevance(contact, recipient.RecipientRank, stringBuilder);
			bool flag6 = RecipientCacheContactWriter.UpdatePersonaType(contact, recipient, stringBuilder);
			bool flag7 = RecipientCacheContactWriter.UpdateNetworkId(contact, stringBuilder);
			updatedProperties = stringBuilder.ToString();
			return flag || flag2 || flag3 || flag4 || flag5 || flag6 || flag7;
		}

		private static void DeleteRecipientCacheContact(IInferenceRecipient recipient, RecipientCacheContactWriter.RecipientCacheContactWriterContext context)
		{
			Util.ThrowOnConditionFailed(recipient.RecipientRank == int.MaxValue, "Delete flag should be applied only for irrelevant entries");
			if (RecipientCacheContactWriter.FindContactAndRemoveDuplicates(recipient, true, context) == null)
			{
				RecipientCacheContactWriter.ClearCaptureFlags(recipient);
			}
		}

		private static string GetLowercaseStringProperty(object property)
		{
			if (property == null || property is PropertyError)
			{
				return string.Empty;
			}
			return ((string)property).ToLower(CultureInfo.InvariantCulture);
		}

		private void EnsureRecipientCacheContactUpToDate(IInferenceRecipient recipient, RecipientCacheContactWriter.RecipientCacheContactWriterContext context)
		{
			StoreId storeId = RecipientCacheContactWriter.FindContactAndRemoveDuplicates(recipient, false, context);
			try
			{
				if (storeId == null)
				{
					using (Contact contact = Contact.Create(context.Session, context.Session.GetDefaultFolderId(DefaultFolderType.RecipientCache)))
					{
						RecipientCacheContactWriter.SetContactProperties(contact, recipient);
						context.BulkAutomaticLink.Link(contact);
						contact.Save(SaveMode.NoConflictResolutionForceSave);
						contact.Load();
						context.BulkAutomaticLink.NotifyContactSaved(contact);
						goto IL_C8;
					}
				}
				using (Contact contact2 = Contact.Bind(context.Session, storeId, RecipientCacheContactWriter.QueryProperties))
				{
					string arg = null;
					if (RecipientCacheContactWriter.UpdateContactProperties(contact2, recipient, out arg))
					{
						contact2.Save(SaveMode.NoConflictResolutionForceSave);
						this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - Effective changes:{1}", context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress, arg), new object[0]);
					}
				}
				IL_C8:;
			}
			catch (MessageSubmissionExceededException ex)
			{
				this.DiagnosticsSession.TraceError<MessageSubmissionExceededException>("Received MessageSubmissionExceeded exception - {0}", ex);
				this.DiagnosticsSession.SendInformationalWatsonReport(ex, "Recipient Cache contact cannot be saved to store (EnsureRecipientCacheContactUpToDate)");
			}
		}

		private void ProcessChange(IInferenceRecipient recipient, RecipientCacheContactWriter.RecipientCacheContactWriterContext context, RecipientCacheContactWriter.RecipientCacheChangeList changeList)
		{
			CaptureFlag captureFlag = (CaptureFlag)recipient.CaptureFlag;
			if (captureFlag == CaptureFlag.None && recipient.HasUpdatedData)
			{
				captureFlag = CaptureFlag.Update;
			}
			switch (captureFlag)
			{
			case CaptureFlag.Add:
				this.EnsureRecipientCacheContactUpToDate(recipient, context);
				RecipientCacheContactWriter.ClearCaptureFlags(recipient);
				changeList.AddsProcessed++;
				return;
			case CaptureFlag.Update:
				this.EnsureRecipientCacheContactUpToDate(recipient, context);
				RecipientCacheContactWriter.ClearCaptureFlags(recipient);
				changeList.UpdatesProcessed++;
				return;
			case CaptureFlag.Delete:
				RecipientCacheContactWriter.DeleteRecipientCacheContact(recipient, context);
				return;
			}
			Util.ThrowOnConditionFailed(false, "Invalid capture flag " + recipient.CaptureFlag);
		}

		private RecipientCacheContactWriter.RecipientCacheChangeList DetermineChangeList(IDictionary<string, IInferenceRecipient> contactList, IList<string> topRanked, RecipientCacheContactWriter.RecipientCacheContactWriterContext context)
		{
			RecipientCacheContactWriter.RecipientCacheChangeList recipientCacheChangeList = new RecipientCacheContactWriter.RecipientCacheChangeList(this.MaxContactUpdatesCount);
			List<KeyValuePair<int, IInferenceRecipient>> list = new List<KeyValuePair<int, IInferenceRecipient>>();
			foreach (string text in context.DeletedRecipientCacheItems.Keys)
			{
				if (contactList.ContainsKey(text))
				{
					IInferenceRecipient inferenceRecipient = contactList[text];
					if (inferenceRecipient.CaptureFlag == 4 || (inferenceRecipient.CaptureFlag == 0 && inferenceRecipient.RecipientRank == 2147483647))
					{
						this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - Deleted recipient cache entry for {1} is already irrelevant or marked for deletion", context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress, text), new object[0]);
					}
					else
					{
						this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - Deleted recipient cache entry for {1} found, and deletion triggered.", context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress, text), new object[0]);
						inferenceRecipient.RecipientRank = int.MaxValue;
						inferenceRecipient.RawRecipientWeight = 0.0;
						inferenceRecipient.CaptureFlag = 4;
					}
				}
				else
				{
					this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - Deleted recipient cache entry for {1} is ignored, as the corresponding contact does not exist in the inference model", context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress, text), new object[0]);
				}
				context.DeletedItemsToDelete.Add(context.DeletedRecipientCacheItems[text]);
			}
			if (topRanked != null && topRanked.Count > 0)
			{
				bool flag = false;
				foreach (string key in topRanked)
				{
					IInferenceRecipient inferenceRecipient2 = contactList[key];
					CaptureFlag captureFlag = (CaptureFlag)inferenceRecipient2.CaptureFlag;
					switch (captureFlag)
					{
					case CaptureFlag.None:
						if (inferenceRecipient2.HasUpdatedData)
						{
							list.Add(new KeyValuePair<int, IInferenceRecipient>(int.MaxValue, inferenceRecipient2));
						}
						break;
					case CaptureFlag.Add:
						recipientCacheChangeList.AddRecipientToProcessingList(inferenceRecipient2, this.DiagnosticsSession, context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
						if (recipientCacheChangeList.QuotaRemaining == 0)
						{
							flag = true;
						}
						break;
					case CaptureFlag.Update:
						list.Add(new KeyValuePair<int, IInferenceRecipient>(Math.Abs(inferenceRecipient2.RelevanceCategory - inferenceRecipient2.RelevanceCategoryAtLastCapture), inferenceRecipient2));
						break;
					case (CaptureFlag)3:
						goto IL_242;
					case CaptureFlag.Delete:
						break;
					default:
						goto IL_242;
					}
					IL_259:
					if (!flag)
					{
						continue;
					}
					break;
					IL_242:
					Util.ThrowOnConditionFailed(false, "Invalid capture flag " + captureFlag);
					goto IL_259;
				}
			}
			list.Sort((KeyValuePair<int, IInferenceRecipient> a, KeyValuePair<int, IInferenceRecipient> b) => a.Key.CompareTo(b.Key));
			foreach (KeyValuePair<int, IInferenceRecipient> keyValuePair in list)
			{
				if (recipientCacheChangeList.QuotaRemaining == 0)
				{
					break;
				}
				recipientCacheChangeList.AddRecipientToProcessingList(keyValuePair.Value, this.DiagnosticsSession, context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			int num = int.MaxValue;
			if (topRanked != null && topRanked.Count > 0)
			{
				num = contactList[topRanked[topRanked.Count - 1]].RecipientRank;
			}
			foreach (IInferenceRecipient inferenceRecipient3 in contactList.Values)
			{
				if (inferenceRecipient3.SmtpAddress != null)
				{
					CaptureFlag captureFlag2 = (CaptureFlag)inferenceRecipient3.CaptureFlag;
					switch (captureFlag2)
					{
					case CaptureFlag.None:
						if (inferenceRecipient3.HasUpdatedData && inferenceRecipient3.RecipientRank <= num)
						{
							recipientCacheChangeList.UpdatesTotal++;
							continue;
						}
						continue;
					case CaptureFlag.Add:
						recipientCacheChangeList.AddsTotal++;
						continue;
					case CaptureFlag.Update:
						recipientCacheChangeList.UpdatesTotal++;
						continue;
					case CaptureFlag.Delete:
						recipientCacheChangeList.AddRecipientToProcessingList(inferenceRecipient3, this.DiagnosticsSession, context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
						recipientCacheChangeList.DeletesTotal++;
						continue;
					}
					Util.ThrowOnConditionFailed(false, "Invalid capture flag " + captureFlag2);
				}
			}
			return recipientCacheChangeList;
		}

		private void ValidateRecipientCache(DocumentContext data, IDictionary<string, IInferenceRecipient> contactList, IList<string> topRanked, RecipientCacheContactWriter.RecipientCacheContactWriterContext context)
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			object obj;
			bool flag;
			if (data.Document.TryGetProperty(PeopleRelevanceSchema.LastRecipientCacheValidationTime, out obj))
			{
				ExDateTime dt = (ExDateTime)obj;
				flag = (utcNow - dt > PeopleRelevanceConfig.Instance.RecipientCacheValidationInterval);
			}
			else
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			Exception ex = null;
			bool flag2 = false;
			HashSet<string> hashSet = (topRanked != null) ? new HashSet<string>(topRanked) : new HashSet<string>();
			int num = 0;
			int num2 = 0;
			try
			{
				foreach (KeyValuePair<string, List<object[]>> keyValuePair in context.SortedRecipientCacheData)
				{
					RecipientCacheContactWriter.RemoveDuplicates(keyValuePair.Key, context);
					object[] array = keyValuePair.Value[0];
					StoreId key = (StoreId)array[0];
					int relevanceRank = 1;
					if (array[2] != null && !(array[2] is PropertyError))
					{
						relevanceRank = (int)array[2];
					}
					if (contactList.ContainsKey(keyValuePair.Key))
					{
						hashSet.Remove(keyValuePair.Key);
						IInferenceRecipient inferenceRecipient = contactList[keyValuePair.Key];
						if (inferenceRecipient.CaptureFlag == 0)
						{
							if (inferenceRecipient.RelevanceCategory == 2147483647)
							{
								Util.ThrowOnConditionFailed(inferenceRecipient.RelevanceCategoryAtLastCapture == int.MaxValue, "Invalid relevance category in the model item");
								context.ItemsToDelete[key] = null;
							}
							else
							{
								int relevanceCategoryForRank = InferenceRecipient.GetRelevanceCategoryForRank(relevanceRank);
								if (relevanceCategoryForRank != inferenceRecipient.RelevanceCategoryAtLastCapture && relevanceCategoryForRank != inferenceRecipient.RelevanceCategory)
								{
									inferenceRecipient.CaptureFlag = 2;
								}
							}
						}
					}
					else
					{
						context.ItemsToDelete[key] = null;
					}
				}
			}
			catch (StorageTransientException ex2)
			{
				ex = ex2;
				flag2 = true;
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			foreach (string key2 in hashSet)
			{
				if (contactList[key2].CaptureFlag == 0)
				{
					contactList[key2].CaptureFlag = 1;
					num++;
				}
			}
			bool flag3;
			if (context.TryCheckRecipientCacheFolderExists(out flag3) && flag3)
			{
				context.SortedRecipientCache.SeekToOffset(SeekReference.OriginBeginning, 0);
			}
			if (ex != null)
			{
				this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, string.Format("U={0} - Encountered exception while validating the Recipient Cache: {1} {2}", context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress, ex.Message, ex.StackTrace), new object[0]);
			}
			if (!flag2)
			{
				data.Document.SetProperty(PeopleRelevanceSchema.LastRecipientCacheValidationTime, utcNow);
			}
			this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - Recipient Cache Validation found {1} entries to delete, {2} to update and {3} to add", new object[]
			{
				context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress,
				context.ItemsToDelete.Count,
				num2,
				num
			}), new object[0]);
		}

		private void ProcessStoreChanges(RecipientCacheContactWriter.RecipientCacheChangeList changeList, RecipientCacheContactWriter.RecipientCacheContactWriterContext context)
		{
			if (changeList.TotalCount == 0)
			{
				this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - No updates to process.", context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress), new object[0]);
				return;
			}
			foreach (IInferenceRecipient inferenceRecipient in changeList.ProcessingList.Values)
			{
				LocalizedException ex = null;
				try
				{
					this.LogContactUpdateData(inferenceRecipient, context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString());
					this.ProcessChange(inferenceRecipient, context, changeList);
				}
				catch (StorageTransientException ex2)
				{
					ex = ex2;
				}
				catch (StoragePermanentException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					changeList.FailureCount++;
					bool flag = RecipientCacheContactWriter.IsGlobalException(ex);
					this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, string.Format("U={0} - Encountered exception while processing a store update for recipient {1}. IsGlobalException? {2}: {3}", new object[]
					{
						context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress,
						inferenceRecipient.SmtpAddress,
						flag,
						ex.Message
					}), new object[0]);
					if (flag)
					{
						break;
					}
				}
			}
			this.ApplyItemDeletes(context, changeList);
		}

		private void ApplyItemDeletes(RecipientCacheContactWriter.RecipientCacheContactWriterContext context, RecipientCacheContactWriter.RecipientCacheChangeList changeList)
		{
			if (context.ItemsToDelete.Count == 0 && context.DeletedItemsToDelete.Count == 0)
			{
				return;
			}
			bool flag;
			if (!context.TryCheckRecipientCacheFolderExists(out flag))
			{
				changeList.FailureCount += context.ItemsToDelete.Count;
				return;
			}
			if (!flag)
			{
				this.DiagnosticsSession.TraceDebug("ApplyItemDeletes not executed. RecipientCacheFolder does not exist", new object[0]);
				changeList.DeletesProcessed = context.ItemsToDelete.Count;
			}
			else if (context.ItemsToDelete.Count > 0)
			{
				StoreId[] array = new StoreId[context.ItemsToDelete.Count];
				context.ItemsToDelete.Keys.CopyTo(array, 0);
				AggregateOperationResult aggregateOperationResult = context.RecipientCacheFolder.DeleteObjects(DeleteItemFlags.HardDelete, array);
				if (aggregateOperationResult.OperationResult == OperationResult.Succeeded)
				{
					changeList.DeletesProcessed = context.ItemsToDelete.Count;
				}
				else
				{
					for (int i = 0; i < aggregateOperationResult.GroupOperationResults.Length; i++)
					{
						if (aggregateOperationResult.GroupOperationResults[i].OperationResult != OperationResult.Succeeded)
						{
							IInferenceRecipient inferenceRecipient = context.ItemsToDelete[array[i]];
							this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, string.Format("U={0} - Encountered exception while attempting to delete {1}: {2}", context.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress, (inferenceRecipient == null) ? array[i].ToString() : inferenceRecipient.EmailAddress, aggregateOperationResult.GroupOperationResults[i].Exception.Message), new object[0]);
							context.ItemsToDelete.Remove(array[i]);
							changeList.FailureCount++;
						}
						else
						{
							changeList.DeletesProcessed++;
						}
					}
				}
			}
			if (context.DeletedItemsToDelete.Count > 0)
			{
				StoreId[] array2 = new StoreId[context.DeletedItemsToDelete.Count];
				context.DeletedItemsToDelete.CopyTo(array2, 0);
				context.DeletedItemsFolder.DeleteObjects(DeleteItemFlags.HardDelete, array2);
			}
			foreach (IInferenceRecipient inferenceRecipient2 in context.ItemsToDelete.Values)
			{
				if (inferenceRecipient2 != null)
				{
					RecipientCacheContactWriter.ClearCaptureFlags(inferenceRecipient2);
				}
			}
		}

		private void LogRecipientCacheUpdateSummary(RecipientCacheContactWriter.RecipientCacheChangeList recipientCacheChangeList, string userIdentity)
		{
			this.DiagnosticsSession.LogDiagnosticsInfo((recipientCacheChangeList.FailureCount == 0) ? DiagnosticsLoggingTag.Informational : DiagnosticsLoggingTag.Warnings, string.Format("Summary: U={0};TC={1};FC={2};ADD={3}/{4}/{5};UPT={6}/{7}/{8};DEL={9}/{10}/{11}", new object[]
			{
				userIdentity,
				recipientCacheChangeList.TotalCount,
				recipientCacheChangeList.FailureCount,
				recipientCacheChangeList.AddsProcessed,
				recipientCacheChangeList.AddsForProcessing,
				recipientCacheChangeList.AddsTotal,
				recipientCacheChangeList.UpdatesProcessed,
				recipientCacheChangeList.UpdatesForProcessing,
				recipientCacheChangeList.UpdatesTotal,
				recipientCacheChangeList.DeletesProcessed,
				recipientCacheChangeList.DeletesForProcessing,
				recipientCacheChangeList.DeletesTotal
			}), new object[0]);
		}

		private void LogContactUpdateData(IInferenceRecipient recipient, string userIdentity)
		{
			this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Informational, string.Format("U={0} - Applying '{1}' to E={2};TSC={3};R={4};C={5};LC={6};W={7};LTW={8};IM={9};A={10};T={11};UD={12}", new object[]
			{
				userIdentity,
				(CaptureFlag)recipient.CaptureFlag,
				recipient.SmtpAddress,
				recipient.TotalSentCount,
				recipient.RecipientRank,
				recipient.RelevanceCategory,
				recipient.RelevanceCategoryAtLastCapture,
				recipient.RawRecipientWeight,
				recipient.LastUsedInTimeWindow,
				recipient.SipUri,
				recipient.Alias,
				recipient.RecipientDisplayType,
				recipient.HasUpdatedData
			}), new object[0]);
		}

		private const string ComponentDescription = "RecipientCacheContactWriter updates the recipient cache contacts based on the capture flags set byt PeopleRelevanceClassifier.";

		private const string ComponentName = "RecipientCacheContactWriter";

		private static readonly PropertyDefinition[] QueryProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ContactSchema.Email1EmailAddress,
			ContactSchema.RelevanceScore,
			ContactSchema.Email1DisplayName,
			ContactSchema.IMAddress,
			ContactSchema.PartnerNetworkId,
			StoreObjectSchema.DisplayName,
			ContactSchema.Account,
			ContactSchema.PersonType
		};

		private class RecipientCacheChangeList
		{
			public RecipientCacheChangeList(int quota)
			{
				this.ProcessingList = new SortedList<string, IInferenceRecipient>();
				this.QuotaRemaining = quota;
			}

			public SortedList<string, IInferenceRecipient> ProcessingList { get; private set; }

			public int AddsForProcessing { get; private set; }

			public int DeletesForProcessing { get; private set; }

			public int UpdatesForProcessing { get; private set; }

			public int AddsTotal { get; set; }

			public int DeletesTotal { get; set; }

			public int UpdatesTotal { get; set; }

			public int AddsProcessed { get; set; }

			public int DeletesProcessed { get; set; }

			public int UpdatesProcessed { get; set; }

			public int FailureCount { get; set; }

			public int QuotaRemaining { get; private set; }

			public int TotalCount
			{
				get
				{
					return this.AddsTotal + this.DeletesTotal + this.UpdatesTotal;
				}
			}

			public void AddRecipientToProcessingList(IInferenceRecipient recipient, IDiagnosticsSession diagnosticsSession, string mailboxOwner)
			{
				if (this.ProcessingList.ContainsKey(recipient.SmtpAddress))
				{
					diagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Failures, string.Format("U={0} - Duplicate processing entry for {1} found. ", mailboxOwner, recipient.ToString()), new object[0]);
					return;
				}
				this.ProcessingList.Add(recipient.SmtpAddress, recipient);
				CaptureFlag captureFlag = (CaptureFlag)recipient.CaptureFlag;
				if (captureFlag == CaptureFlag.None && recipient.HasUpdatedData)
				{
					captureFlag = CaptureFlag.Update;
				}
				switch (captureFlag)
				{
				case CaptureFlag.Add:
					this.AddsForProcessing++;
					this.QuotaRemaining--;
					return;
				case CaptureFlag.Update:
					this.UpdatesForProcessing++;
					this.QuotaRemaining--;
					return;
				case CaptureFlag.Delete:
					this.DeletesForProcessing++;
					return;
				}
				Util.ThrowOnConditionFailed(false, "Invalid capture flag " + recipient.CaptureFlag);
			}
		}

		private class RecipientCacheContactWriterContext
		{
			internal RecipientCacheContactWriterContext(MailboxSession session, IDiagnosticsSession diagnosticSession)
			{
				this.Session = session;
				this.DiagnosticsSession = diagnosticSession;
			}

			internal Folder RecipientCacheFolder
			{
				get
				{
					Util.ThrowOnNullArgument(this.Session, "session");
					if (this.recipientCacheFolder == null)
					{
						this.recipientCacheFolder = Folder.Bind(this.Session, this.RecipientCacheFolderId);
					}
					return this.recipientCacheFolder;
				}
			}

			internal Folder DeletedItemsFolder
			{
				get
				{
					Util.ThrowOnNullArgument(this.Session, "session");
					if (this.deletedItemsFolder == null)
					{
						this.deletedItemsFolder = Folder.Bind(this.Session, this.DeletedItemsFolderId);
					}
					return this.deletedItemsFolder;
				}
			}

			internal QueryResult SortedRecipientCache
			{
				get
				{
					if (this.sortedRecipientCache == null)
					{
						SortBy[] sortColumns = new SortBy[]
						{
							new SortBy(ContactSchema.Email1EmailAddress, SortOrder.Ascending),
							new SortBy(StoreObjectSchema.LastModifiedTime, SortOrder.Descending)
						};
						this.sortedRecipientCache = this.RecipientCacheFolder.ItemQuery(ItemQueryType.None, null, sortColumns, RecipientCacheContactWriter.QueryProperties);
					}
					return this.sortedRecipientCache;
				}
			}

			internal Dictionary<string, List<object[]>> SortedRecipientCacheData
			{
				get
				{
					if (this.sortedRecipientCacheData == null)
					{
						bool flag = false;
						try
						{
							this.sortedRecipientCacheData = new Dictionary<string, List<object[]>>(PeopleRelevanceConfig.Instance.MaxRelevantRecipientsCount);
							bool flag2 = false;
							do
							{
								object[][] rows = this.SortedRecipientCache.GetRows(1000, out flag2);
								foreach (object[] array2 in rows)
								{
									string lowercaseStringProperty = RecipientCacheContactWriter.GetLowercaseStringProperty(array2[1]);
									if (this.sortedRecipientCacheData.ContainsKey(lowercaseStringProperty))
									{
										this.sortedRecipientCacheData[lowercaseStringProperty].Add(array2);
									}
									else
									{
										this.sortedRecipientCacheData.Add(lowercaseStringProperty, new List<object[]>(new object[][]
										{
											array2
										}));
									}
								}
							}
							while (flag2);
							flag = true;
						}
						finally
						{
							if (!flag)
							{
								this.sortedRecipientCacheData = null;
							}
						}
					}
					return this.sortedRecipientCacheData;
				}
			}

			internal Dictionary<string, StoreId> DeletedRecipientCacheItems
			{
				get
				{
					if (this.deletedRecipientCacheItems != null)
					{
						return this.deletedRecipientCacheItems;
					}
					Util.ThrowOnNullArgument(this.Session, "session");
					Dictionary<string, StoreId> dictionary = new Dictionary<string, StoreId>();
					Exception ex = null;
					try
					{
						using (QueryResult queryResult = this.DeletedItemsFolder.ItemQuery(ItemQueryType.None, null, RecipientCacheContactWriter.RecipientCacheContactWriterContext.DeletedRecipientCacheContactsSortBy, RecipientCacheContactWriter.QueryProperties))
						{
							bool flag = true;
							IL_C1:
							while (flag && queryResult.SeekToCondition(SeekReference.OriginCurrent, RecipientCacheContactWriter.RecipientCacheContactWriterContext.DeletedRecipientCacheContactsFilter))
							{
								object[][] rows = queryResult.GetRows(10000);
								int num = 0;
								for (;;)
								{
									StoreId storeId = (StoreId)rows[num][0];
									string lowercaseStringProperty = RecipientCacheContactWriter.GetLowercaseStringProperty(rows[num][1]);
									if (!dictionary.ContainsKey(lowercaseStringProperty))
									{
										dictionary.Add(lowercaseStringProperty, storeId);
									}
									else
									{
										this.DeletedItemsToDelete.Add(storeId);
									}
									if (++num == rows.Length)
									{
										break;
									}
									string lowercaseStringProperty2 = RecipientCacheContactWriter.GetLowercaseStringProperty(rows[num][5]);
									if (!lowercaseStringProperty2.Equals(WellKnownNetworkNames.RecipientCache, StringComparison.InvariantCultureIgnoreCase))
									{
										goto IL_C1;
									}
								}
								flag = false;
							}
						}
					}
					catch (StorageTransientException ex2)
					{
						ex = ex2;
					}
					catch (StoragePermanentException ex3)
					{
						ex = ex3;
					}
					if (ex != null)
					{
						this.DiagnosticsSession.LogDiagnosticsInfo(DiagnosticsLoggingTag.Warnings, string.Format("U={0} - Encountered exception while retrieving the deleted Recipient Cache items: {1} {2}", this.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress, ex.Message, ex.StackTrace), new object[0]);
					}
					this.deletedRecipientCacheItems = dictionary;
					return this.deletedRecipientCacheItems;
				}
			}

			internal MailboxSession Session { get; private set; }

			internal IDiagnosticsSession DiagnosticsSession { get; private set; }

			internal BulkAutomaticLink BulkAutomaticLink
			{
				get
				{
					if (this.bulkAutomaticLink == null)
					{
						this.bulkAutomaticLink = new BulkAutomaticLink(this.Session);
					}
					return this.bulkAutomaticLink;
				}
			}

			internal Dictionary<StoreId, IInferenceRecipient> ItemsToDelete { get; set; }

			internal HashSet<StoreId> DeletedItemsToDelete { get; set; }

			private StoreObjectId RecipientCacheFolderId
			{
				get
				{
					if (this.recipientCacheFolder == null)
					{
						this.recipientCacheFolderId = this.Session.GetDefaultFolderId(DefaultFolderType.RecipientCache);
						if (this.recipientCacheFolderId == null)
						{
							throw new ObjectNotInitializedException(new LocalizedString("RecipientCache folder not initialized"), null);
						}
					}
					return this.recipientCacheFolderId;
				}
			}

			private StoreObjectId DeletedItemsFolderId
			{
				get
				{
					if (this.deletedItemsFolderId == null)
					{
						this.deletedItemsFolderId = this.Session.GetDefaultFolderId(DefaultFolderType.DeletedItems);
					}
					return this.deletedItemsFolderId;
				}
			}

			internal bool TryCheckRecipientCacheFolderExists(out bool exists)
			{
				LocalizedException ex = null;
				exists = false;
				try
				{
					exists = (this.RecipientCacheFolder != null);
				}
				catch (StorageTransientException ex2)
				{
					ex = ex2;
				}
				catch (StoragePermanentException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					this.DiagnosticsSession.TraceDebug<string, string>("Encountered an exception checking if RecipientCache folder exists: {0} {1}", ex.Message, ex.StackTrace);
					return false;
				}
				return true;
			}

			internal void DisposeAll()
			{
				if (this.sortedRecipientCache != null)
				{
					this.sortedRecipientCache.Dispose();
					this.sortedRecipientCache = null;
				}
				if (this.recipientCacheFolder != null)
				{
					this.recipientCacheFolder.Dispose();
					this.recipientCacheFolder = null;
				}
				if (this.deletedItemsFolder != null)
				{
					this.deletedItemsFolder.Dispose();
					this.deletedItemsFolder = null;
				}
				if (this.bulkAutomaticLink != null)
				{
					this.bulkAutomaticLink.Dispose();
					this.bulkAutomaticLink = null;
				}
				this.Session = null;
			}

			private static readonly SortBy[] DeletedRecipientCacheContactsSortBy = new SortBy[]
			{
				new SortBy(ContactSchema.PartnerNetworkId, SortOrder.Descending)
			};

			private static readonly QueryFilter DeletedRecipientCacheContactsFilter = new ComparisonFilter(ComparisonOperator.Equal, ContactSchema.PartnerNetworkId, WellKnownNetworkNames.RecipientCache);

			private StoreObjectId recipientCacheFolderId;

			private Folder recipientCacheFolder;

			private StoreObjectId deletedItemsFolderId;

			private Folder deletedItemsFolder;

			private QueryResult sortedRecipientCache;

			private Dictionary<string, List<object[]>> sortedRecipientCacheData;

			private Dictionary<string, StoreId> deletedRecipientCacheItems;

			private BulkAutomaticLink bulkAutomaticLink;
		}
	}
}
