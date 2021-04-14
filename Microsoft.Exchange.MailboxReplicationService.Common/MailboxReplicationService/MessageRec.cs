using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class MessageRec
	{
		[DataMember(IsRequired = true)]
		public byte[] EntryId
		{
			get
			{
				return this.entryId;
			}
			set
			{
				this.entryId = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public byte[] FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		[DataMember(IsRequired = true, Name = "Flags")]
		public MessageRecFlags LegacyFlags
		{
			get
			{
				return (MessageRecFlags)(this.flags & MsgRecFlags.AllLegacy);
			}
			set
			{
				this.flags |= (MsgRecFlags)value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public int FlagsInt
		{
			get
			{
				return (int)this.flags;
			}
			set
			{
				this.flags |= (MsgRecFlags)value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public DateTime CreationTimestamp
		{
			get
			{
				return this.creationTimestamp;
			}
			set
			{
				this.creationTimestamp = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public int MessageSize
		{
			get
			{
				return this.messageSize;
			}
			set
			{
				this.messageSize = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public PropValueData[] AdditionalProps
		{
			get
			{
				return this.additionalProps;
			}
			set
			{
				this.additionalProps = value;
			}
		}

		public MessageRec()
		{
		}

		public MessageRec(byte[] entryId, byte[] folderId, DateTime creationTimestamp, int messageSize, MsgRecFlags flags, PropValueData[] additionalProps)
		{
			this.entryId = entryId;
			this.folderId = folderId;
			this.creationTimestamp = creationTimestamp;
			this.messageSize = messageSize;
			this.additionalProps = additionalProps;
			this.flags = flags;
		}

		public MsgRecFlags Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		public bool IsDeleted
		{
			get
			{
				return this.flags.HasFlag(MsgRecFlags.Deleted);
			}
		}

		public bool IsNew
		{
			get
			{
				return this.flags.HasFlag(MsgRecFlags.New);
			}
		}

		public bool IsFAI
		{
			get
			{
				return this.flags.HasFlag(MsgRecFlags.Associated);
			}
		}

		public int CompareTo(MessageRecSortBy sortBy, DateTime creationTimestamp, byte[] folderId, byte[] entryId)
		{
			int num = this.CreationTimestamp.CompareTo(creationTimestamp);
			if (num == 0)
			{
				num = ArrayComparer<byte>.Comparer.Compare(this.FolderId, folderId);
				if (num == 0)
				{
					num = ArrayComparer<byte>.Comparer.Compare(this.EntryId, entryId);
				}
			}
			if (sortBy == MessageRecSortBy.DescendingTimeStamp)
			{
				num = -num;
			}
			return num;
		}

		public object this[PropTag ptag]
		{
			get
			{
				if (this.additionalProps != null)
				{
					foreach (PropValueData propValueData in this.additionalProps)
					{
						if (propValueData.PropTag == (int)ptag)
						{
							return propValueData.Value;
						}
					}
				}
				return null;
			}
		}

		[Conditional("DEBUG")]
		private void Validate()
		{
			if (this.flags.HasFlag(MsgRecFlags.New))
			{
				this.flags.HasFlag(MsgRecFlags.Deleted);
			}
			if (this.flags.HasFlag(MsgRecFlags.Regular))
			{
				this.flags.HasFlag(MsgRecFlags.Associated);
			}
		}

		private byte[] entryId;

		private byte[] folderId;

		private DateTime creationTimestamp;

		private int messageSize;

		private MsgRecFlags flags;

		private PropValueData[] additionalProps;
	}
}
