using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ContentsManifestCallbackHelperBase<TCallback> : ManifestCallbackHelperBase<TCallback> where TCallback : class
	{
		protected ContentsManifestCallbackHelperBase(bool conversations)
		{
			this.conversations = conversations;
		}

		public ManifestCallbackQueue<TCallback> Reads
		{
			get
			{
				return this.readList;
			}
		}

		public bool Conversations
		{
			get
			{
				return this.conversations;
			}
		}

		protected abstract ManifestCallbackStatus DoChangeCallback(TCallback callback, ManifestChangeType changeType, PropValue[] headerProps, PropValue[] messageProps);

		protected unsafe int Change(ExchangeManifestCallbackChangeFlags flags, int cpvalHeader, SPropValue* ppvalHeader, int cpvalProps, SPropValue* ppvalProps)
		{
			PropValue[] pvaHeader = new PropValue[cpvalHeader];
			if ((!this.conversations && cpvalHeader != 9 && cpvalHeader != 10) || (this.conversations && cpvalHeader != 5))
			{
				return -2147221221;
			}
			for (int i = 0; i < cpvalHeader; i++)
			{
				pvaHeader[i] = new PropValue(ppvalHeader + i);
			}
			if (!this.conversations)
			{
				if (cpvalHeader == 9)
				{
					if (pvaHeader[0].PropTag != PropTag.SourceKey || pvaHeader[1].PropTag != PropTag.LastModificationTime || pvaHeader[2].PropTag != PropTag.ChangeKey || pvaHeader[3].PropTag != PropTag.PredecessorChangeList || pvaHeader[4].PropTag != PropTag.Associated || pvaHeader[5].PropTag != PropTag.Mid || pvaHeader[7].PropTag != PropTag.Cn || pvaHeader[8].PropTag != PropTag.EntryId)
					{
						return -2147221221;
					}
				}
				else if (pvaHeader[0].PropTag != PropTag.SourceKey || pvaHeader[1].PropTag != PropTag.LastModificationTime || pvaHeader[2].PropTag != PropTag.ChangeKey || pvaHeader[3].PropTag != PropTag.PredecessorChangeList || pvaHeader[4].PropTag != PropTag.Associated || pvaHeader[5].PropTag != PropTag.Mid || pvaHeader[7].PropTag != PropTag.Cn || pvaHeader[8].PropTag != PropTag.ReadCn || pvaHeader[9].PropTag != PropTag.EntryId)
				{
					return -2147221221;
				}
			}
			else if (pvaHeader[0].PropTag != PropTag.Mid || pvaHeader[1].PropTag != PropTag.Cn || pvaHeader[2].PropTag != PropTag.LastModificationTime || pvaHeader[3].PropTag != PropTag.ChangeType || pvaHeader[4].PropTag != PropTag.EntryId)
			{
				return -2147221221;
			}
			PropValue[] pvaProps = new PropValue[cpvalProps];
			for (int j = 0; j < cpvalProps; j++)
			{
				pvaProps[j] = new PropValue(ppvalProps + j);
			}
			ManifestChangeType changeType;
			if (!this.conversations)
			{
				changeType = (((flags & ExchangeManifestCallbackChangeFlags.NewMessage) == ExchangeManifestCallbackChangeFlags.NewMessage) ? ManifestChangeType.Add : ManifestChangeType.Change);
			}
			else
			{
				changeType = (ManifestChangeType)((short)pvaHeader[3].Value);
			}
			base.Changes.Enqueue((TCallback callback) => this.DoChangeCallback(callback, changeType, pvaHeader, pvaProps));
			return 0;
		}

		private readonly ManifestCallbackQueue<TCallback> readList = new ManifestCallbackQueue<TCallback>();

		private readonly bool conversations;

		protected enum ChangePropertyIndexOld
		{
			SourceKey,
			LastModificationTime,
			ChangeKey,
			PredecessorChangeList,
			Associated,
			Mid,
			MessageSize,
			Cn,
			EntryId
		}

		protected enum ChangePropertyIndex
		{
			SourceKey,
			LastModificationTime,
			ChangeKey,
			PredecessorChangeList,
			Associated,
			Mid,
			MessageSize,
			Cn,
			ReadCn,
			EntryId
		}

		protected enum ConversationChangePropertyIndex
		{
			Mid,
			Cn,
			LastModificationTime,
			ChangeType,
			EntryId
		}
	}
}
