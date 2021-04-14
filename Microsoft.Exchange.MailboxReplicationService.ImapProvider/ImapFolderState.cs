using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class ImapFolderState : XMLSerializableBase
	{
		public ImapFolderState()
		{
		}

		internal ImapFolderState(int seqNumCrawl, uint uidNext, uint uidValidity, GlobCountSet uidReadSet, GlobCountSet uidDeletedSet)
		{
			this.SeqNumCrawl = seqNumCrawl;
			this.UidNext = uidNext;
			this.UidValidity = uidValidity;
			this.uidReadSet = uidReadSet;
			this.uidDeletedSet = uidDeletedSet;
		}

		[XmlElement(ElementName = "SeqNumCrawl")]
		public int SeqNumCrawl { get; set; }

		[XmlElement(ElementName = "UidNext")]
		public uint UidNext { get; set; }

		[XmlElement(ElementName = "UidValidity")]
		public uint UidValidity { get; set; }

		[XmlElement(ElementName = "UidSetRead")]
		public byte[] UidSetRead
		{
			get
			{
				return ImapFolderState.SerializeUidSet(this.uidReadSet);
			}
			set
			{
				this.uidReadSet = ImapFolderState.ParseUidSet(value);
			}
		}

		[XmlElement(ElementName = "UidSetDeleted")]
		public byte[] UidSetDeleted
		{
			get
			{
				return ImapFolderState.SerializeUidSet(this.uidDeletedSet);
			}
			set
			{
				this.uidDeletedSet = ImapFolderState.ParseUidSet(value);
			}
		}

		public static ImapFolderState Deserialize(byte[] compressedXml)
		{
			byte[] bytes = CommonUtils.DecompressData(compressedXml);
			string @string = Encoding.UTF7.GetString(bytes);
			if (string.IsNullOrEmpty(@string))
			{
				throw new CorruptSyncStateException(new ArgumentNullException("data", "Cannot deserialize null or empty data"));
			}
			return XMLSerializableBase.Deserialize<ImapFolderState>(@string, true);
		}

		public byte[] Serialize()
		{
			return CommonUtils.CompressData(Encoding.UTF7.GetBytes(base.Serialize(false)));
		}

		internal static ImapFolderState CreateNew(ImapClientFolder folder)
		{
			return new ImapFolderState
			{
				SeqNumCrawl = int.MaxValue,
				UidNext = folder.UidNext,
				UidValidity = folder.UidValidity
			};
		}

		internal static ImapFolderState Create(List<MessageRec> messages, int seqNumCrawl, uint uidNext, uint uidValidity)
		{
			if (messages.Count == 0)
			{
				return new ImapFolderState
				{
					SeqNumCrawl = seqNumCrawl,
					UidNext = uidNext,
					UidValidity = uidValidity
				};
			}
			Dictionary<uint, MessageRec> dictionary = new Dictionary<uint, MessageRec>();
			foreach (MessageRec messageRec in messages)
			{
				uint key = ImapEntryId.ParseUid(messageRec.EntryId);
				dictionary.Add(key, messageRec);
			}
			GlobCountSet globCountSet = new GlobCountSet();
			GlobCountSet globCountSet2 = new GlobCountSet();
			for (uint num = uidNext - 1U; num > 0U; num -= 1U)
			{
				MessageRec messageRec2 = null;
				if (!dictionary.TryGetValue(num, out messageRec2))
				{
					globCountSet2.Insert((ulong)num);
				}
				else
				{
					MessageFlags messageFlags = (MessageFlags)((int)messageRec2[PropTag.MessageFlags]);
					if (messageFlags.HasFlag(MessageFlags.Read))
					{
						globCountSet.Insert((ulong)num);
					}
				}
			}
			return new ImapFolderState(seqNumCrawl, uidNext, uidValidity, globCountSet, globCountSet2);
		}

		internal static void EnumerateReadUnreadFlagChanges(ImapFolderState sourceState, ImapFolderState syncState, Action<uint> uidInclusionAction, Action<uint> uidExclusionAction)
		{
			ImapFolderState.EnumerateChanges(sourceState.uidReadSet, syncState.uidReadSet, syncState.UidNext, uidInclusionAction, uidExclusionAction);
		}

		internal static void EnumerateMessageDeletes(ImapFolderState sourceState, ImapFolderState syncState, Action<uint> uidInclusionAction, Action<uint> uidExclusionAction)
		{
			ImapFolderState.EnumerateChanges(sourceState.uidDeletedSet, syncState.uidDeletedSet, syncState.UidNext, uidInclusionAction, uidExclusionAction);
		}

		private static void EnumerateChanges(GlobCountSet sourceUidSet, GlobCountSet targetUidSet, uint uidNext, Action<uint> uidInclusionAction, Action<uint> uidExclusionAction)
		{
			uint highestUid = uidNext - 1U;
			if (sourceUidSet == null || targetUidSet == null)
			{
				return;
			}
			if (sourceUidSet.IsEmpty && targetUidSet.IsEmpty)
			{
				return;
			}
			if (sourceUidSet.IsEmpty)
			{
				ImapFolderState.PerformAction(targetUidSet, highestUid, uidExclusionAction);
				return;
			}
			if (targetUidSet.IsEmpty)
			{
				ImapFolderState.PerformAction(sourceUidSet, highestUid, uidInclusionAction);
				return;
			}
			GlobCountSet globCountSet = GlobCountSet.Subtract(sourceUidSet, targetUidSet);
			GlobCountSet globCountSet2 = GlobCountSet.Subtract(targetUidSet, sourceUidSet);
			if (globCountSet != null && !globCountSet.IsEmpty)
			{
				ImapFolderState.PerformAction(globCountSet, highestUid, uidInclusionAction);
			}
			if (globCountSet2 != null && !globCountSet2.IsEmpty)
			{
				ImapFolderState.PerformAction(globCountSet2, highestUid, uidExclusionAction);
			}
		}

		private static void PerformAction(GlobCountSet uidSet, uint highestUid, Action<uint> action)
		{
			foreach (GlobCountRange globCountRange in uidSet)
			{
				uint num = (uint)globCountRange.LowBound;
				while (num <= (uint)globCountRange.HighBound && num <= highestUid)
				{
					action(num);
					num += 1U;
				}
			}
		}

		private static GlobCountSet ParseUidSet(byte[] input)
		{
			if (input == null)
			{
				return null;
			}
			GlobCountSet result;
			using (BufferReader bufferReader = Reader.CreateBufferReader(input))
			{
				try
				{
					result = GlobCountSet.Parse(bufferReader);
				}
				catch (BufferParseException innerException)
				{
					throw new CorruptSyncStateException(innerException);
				}
			}
			return result;
		}

		private static byte[] SerializeUidSet(GlobCountSet uidSet)
		{
			if (uidSet == null)
			{
				return null;
			}
			return BufferWriter.Serialize(new BufferWriter.SerializeDelegate(uidSet.Serialize));
		}

		[Conditional("DEBUG")]
		private static void ValidateEnumerationInputs(ImapFolderState sourceState, ImapFolderState syncState, Action<uint> uidInclusionAction, Action<uint> uidExclusionAction)
		{
		}

		internal const int Recrawl = 2147483647;

		internal const int CrawlCompleted = 0;

		private GlobCountSet uidReadSet;

		private GlobCountSet uidDeletedSet;
	}
}
