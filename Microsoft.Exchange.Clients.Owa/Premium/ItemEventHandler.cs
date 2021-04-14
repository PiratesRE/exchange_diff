using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Premium.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	internal abstract class ItemEventHandler : OwaEventHandlerBase
	{
		protected AnrManager.Options AnrOptions
		{
			get
			{
				return this.anrOptions;
			}
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, false)]
		[OwaEvent("Delete")]
		public virtual void Delete()
		{
			this.DoDelete(false);
		}

		[OwaEvent("PermanentDelete")]
		[OwaEventParameter("Id", typeof(OwaStoreObjectId), false, false)]
		public virtual void PermanentDelete()
		{
			this.DoDelete(true);
		}

		[OwaEventParameter("Id", typeof(OwaStoreObjectId))]
		[OwaEvent("CleanupDelete")]
		[OwaEventParameter("CK", typeof(string))]
		public virtual void CleanupDelete()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ItemEventHandler.CleanupDelete");
			Item item = null;
			try
			{
				item = this.GetReadOnlyRequestItem<Item>(new PropertyDefinition[0]);
				string strB = (string)base.GetParameter("CK");
				if (string.CompareOrdinal(item.Id.ChangeKeyAsBase64String(), strB) != 0)
				{
					return;
				}
			}
			finally
			{
				if (item != null)
				{
					item.Dispose();
					item = null;
				}
			}
			this.DoDelete(true, false);
		}

		protected bool GetExchangeParticipantsFromRecipientInfo(RecipientInfo recipientInfo, List<Participant> exchangeParticipants)
		{
			bool flag = false;
			Participant item = null;
			this.AnrOptions.ResolveContactsFirst = base.UserContext.UserOptions.CheckNameInContactsFirst;
			if (recipientInfo.PendingChunk != null)
			{
				RecipientCache recipientCache = AutoCompleteCache.TryGetCache(OwaContext.Current.UserContext);
				ArrayList arrayList = new ArrayList();
				RecipientWell.ResolveAndRenderChunk(this.Writer, recipientInfo.PendingChunk, arrayList, recipientCache, base.UserContext, this.AnrOptions);
				for (int i = 0; i < arrayList.Count; i++)
				{
					RecipientWellNode recipientWellNode = (RecipientWellNode)arrayList[i];
					flag |= Utilities.CreateExchangeParticipant(out item, recipientWellNode.DisplayName, recipientWellNode.RoutingAddress, recipientWellNode.RoutingType, recipientWellNode.AddressOrigin, recipientWellNode.StoreObjectId, recipientWellNode.EmailAddressIndex);
					exchangeParticipants.Add(item);
				}
			}
			else
			{
				flag |= recipientInfo.ToParticipant(out item);
				exchangeParticipants.Add(item);
			}
			return flag;
		}

		protected T GetRequestItem<T>(params PropertyDefinition[] prefetchProperties) where T : Item
		{
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			string changeKey = (string)base.GetParameter("CK");
			return Utilities.GetItem<T>(base.UserContext, owaStoreObjectId, changeKey, prefetchProperties);
		}

		protected T GetRequestItem<T>(VersionedId versionedId, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return this.GetRequestItem<T>(versionedId, ItemBindOption.None, prefetchProperties);
		}

		protected T GetRequestItem<T>(VersionedId versionedId, ItemBindOption itemBindOption, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItem<T>(base.UserContext, versionedId, itemBindOption, prefetchProperties);
		}

		protected T GetReadOnlyRequestItem<T>(params PropertyDefinition[] prefetchProperties) where T : Item
		{
			OwaStoreObjectId itemId = (OwaStoreObjectId)base.GetParameter("Id");
			return this.GetReadOnlyRequestItem<T>(itemId, prefetchProperties);
		}

		protected T GetReadOnlyRequestItem<T>(OwaStoreObjectId itemId, params PropertyDefinition[] prefetchProperties) where T : Item
		{
			return Utilities.GetItem<T>(base.UserContext, itemId, prefetchProperties);
		}

		protected void MoveItemToDestinationFolderIfInScratchPad(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			OwaStoreObjectId owaStoreObjectId = base.GetParameter("fId") as OwaStoreObjectId;
			if (owaStoreObjectId == null)
			{
				return;
			}
			if (!owaStoreObjectId.IsPublic)
			{
				return;
			}
			item.Load();
			if (item.ParentId.Equals(owaStoreObjectId.StoreObjectId))
			{
				return;
			}
			OwaStoreObjectId owaStoreObjectId2 = OwaStoreObjectId.CreateFromStoreObject(item);
			OperationResult operationResult = Utilities.CopyOrMoveItems(base.UserContext, false, owaStoreObjectId, new OwaStoreObjectId[]
			{
				owaStoreObjectId2
			}).OperationResult;
			if (operationResult == OperationResult.Succeeded)
			{
				this.Writer.Write("<div id=divFC>");
				this.Writer.Write(LocalizedStrings.GetHtmlEncoded(-1580283653));
				this.Writer.Write("</div>");
				return;
			}
			this.SaveIdAndChangeKeyInCustomErrorInfo(item);
			throw new OwaEventHandlerException("Could not move the item out from scratch pad into target folder", LocalizedStrings.GetNonEncoded(1665365872), true);
		}

		protected void SaveIdAndChangeKeyInCustomErrorInfo(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			item.Load();
			if (item.Id == null)
			{
				ExTraceGlobals.MailDataTracer.TraceDebug((long)this.GetHashCode(), "ItemEventHandler.SaveIdAndChangeKeyInCustomErrorInfo: Unable to send id and change key, item Id is null");
				return;
			}
			dictionary.Add("itemId", Utilities.GetIdAsString(item));
			dictionary.Add("ck", item.Id.ChangeKeyAsBase64String());
			base.OwaContext.CustomErrorInfo = dictionary;
		}

		private void DoDelete(bool permanentDelete)
		{
			this.DoDelete(permanentDelete, true);
		}

		private void DoDelete(bool permanentDelete, bool doThrow)
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ItemEventHandler." + (permanentDelete ? "PermanentDelete" : "Delete"));
			OwaStoreObjectId owaStoreObjectId = (OwaStoreObjectId)base.GetParameter("Id");
			Utilities.Delete(base.UserContext, permanentDelete, doThrow, new OwaStoreObjectId[]
			{
				owaStoreObjectId
			});
		}

		public const string MethodAutoSave = "AutoSave";

		public const string MethodSave = "Save";

		public const string MethodDelete = "Delete";

		public const string MethodPromoteInlineAttachments = "PromoteInlineAttachments";

		public const string MethodCleanupDelete = "CleanupDelete";

		public const string MethodPermanentDelete = "PermanentDelete";

		public const string Id = "Id";

		public const string ChangeKey = "CK";

		public const string FolderID = "fId";

		private AnrManager.Options anrOptions = new AnrManager.Options();
	}
}
