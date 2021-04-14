using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public abstract class ObjectCreatedModifiedNotificationEvent : ObjectNotificationEvent
	{
		public ObjectCreatedModifiedNotificationEvent(StoreDatabase database, int mailboxNumber, EventType eventType, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, ExtendedEventFlags extendedEventFlags, ExchangeId fid, ExchangeId mid, ExchangeId parentFid, int? documentId, int? conversationDocumentId, StorePropTag[] changedPropTags, string objectClass, Guid? userIdentityContext) : base(database, mailboxNumber, eventType, userIdentity, clientType, eventFlags, extendedEventFlags, fid, mid, parentFid, documentId, conversationDocumentId, objectClass, userIdentityContext)
		{
			this.changedPropTags = changedPropTags;
		}

		public StorePropTag[] ChangedPropTags
		{
			get
			{
				return this.changedPropTags;
			}
		}

		protected static bool PropTagArraysEqual(StorePropTag[] array1, StorePropTag[] array2)
		{
			if (array1 == array2)
			{
				return true;
			}
			if (array1 == null || array1.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array1.Length; i++)
			{
				if (array1[i] != array2[i])
				{
					return false;
				}
			}
			return true;
		}

		protected static StorePropTag[] MergeChangedPropTagArrays(StorePropTag[] ptags1, StorePropTag[] ptags2)
		{
			if (ptags1 == null || ptags1.Length == 0)
			{
				return ptags2;
			}
			if (ptags2 == null || ptags2.Length == 0)
			{
				return ptags1;
			}
			List<StorePropTag> list = new List<StorePropTag>(ptags1.Length + ptags2.Length);
			foreach (StorePropTag item in ptags1)
			{
				list.Add(item);
			}
			foreach (StorePropTag item2 in ptags2)
			{
				if (!list.Contains(item2))
				{
					list.Add(item2);
				}
			}
			return list.ToArray();
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" ChangedPropTags:[");
			sb.AppendAsString(this.changedPropTags);
			sb.Append("]");
		}

		private StorePropTag[] changedPropTags;
	}
}
