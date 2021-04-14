using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RuleAction
	{
		public uint UserFlags
		{
			get
			{
				return this._UserFlags;
			}
			set
			{
				this._UserFlags = value;
			}
		}

		public RuleAction.Type ActionType
		{
			get
			{
				return this._Type;
			}
			set
			{
				this._Type = value;
			}
		}

		internal abstract int GetBytesToMarshal();

		internal unsafe abstract void MarshalToNative(_Action* pact, ref byte* pExtra);

		private unsafe void BaseMarshal(_Action* pact)
		{
			pact->ActType = (uint)this._Type;
			pact->Zero1 = IntPtr.Zero;
			pact->Zero2 = IntPtr.Zero;
			pact->ulFlags = this._UserFlags;
		}

		private RuleAction(RuleAction.Type type)
		{
			this._Type = type;
			this._UserFlags = 0U;
		}

		private unsafe RuleAction(_Action* pact)
		{
			this._Type = (RuleAction.Type)pact->ActType;
			this._UserFlags = pact->ulFlags;
		}

		internal static bool Equals(RuleAction a, RuleAction b)
		{
			return a == b || (a != null && b != null);
		}

		internal unsafe static RuleAction Unmarshal(_Action* pact)
		{
			if (null != pact)
			{
				switch (pact->ActType)
				{
				case 1U:
				{
					RuleAction.MoveCopy moveCopy = new RuleAction.ExternalMove(pact);
					if (moveCopy.FolderIsInThisStore)
					{
						moveCopy = new RuleAction.InMailboxMove(pact);
					}
					return moveCopy;
				}
				case 2U:
				{
					RuleAction.MoveCopy moveCopy2 = new RuleAction.ExternalCopy(pact);
					if (moveCopy2.FolderIsInThisStore)
					{
						moveCopy2 = new RuleAction.InMailboxCopy(pact);
					}
					return moveCopy2;
				}
				case 3U:
					if ((pact->ActFlavor & 4294967292U) == 0U)
					{
						return new RuleAction.Reply(pact);
					}
					return null;
				case 4U:
					return new RuleAction.OOFReply(pact);
				case 5U:
					return new RuleAction.Defer(pact);
				case 6U:
				{
					RuleAction.Bounce.BounceCode scBounceCode = (RuleAction.Bounce.BounceCode)pact->union.scBounceCode;
					if (scBounceCode == RuleAction.Bounce.BounceCode.TooLarge || scBounceCode == RuleAction.Bounce.BounceCode.FormsMismatch || scBounceCode == RuleAction.Bounce.BounceCode.AccessDenied)
					{
						return new RuleAction.Bounce(pact);
					}
					return null;
				}
				case 7U:
					if ((pact->ActFlavor & 4294967280U) == 0U)
					{
						return new RuleAction.Forward(pact);
					}
					return null;
				case 8U:
					return new RuleAction.Delegate(pact);
				case 9U:
					return new RuleAction.Tag(pact);
				case 10U:
					return new RuleAction.Delete(pact);
				case 11U:
					return new RuleAction.MarkAsRead(pact);
				}
			}
			return null;
		}

		private uint _UserFlags;

		protected RuleAction.Type _Type;

		internal enum Type : uint
		{
			OP_INVALID,
			OP_MOVE,
			OP_COPY,
			OP_REPLY,
			OP_OOF_REPLY,
			OP_DEFER_ACTION,
			OP_BOUNCE,
			OP_FORWARD,
			OP_DELEGATE,
			OP_TAG,
			OP_DELETE,
			OP_MARK_AS_READ
		}

		public class MoveCopy : RuleAction
		{
			public byte[] FolderEntryID
			{
				get
				{
					return this._FolderEntryID;
				}
				set
				{
					this._FolderEntryID = value;
				}
			}

			public byte[] StoreEntryID
			{
				get
				{
					return this._StoreEntryID;
				}
				set
				{
					this._StoreEntryID = value;
				}
			}

			public bool FolderIsInThisStore
			{
				get
				{
					return this._StoreEntryID == null || this._StoreEntryID.Length == 0;
				}
				set
				{
					if (value)
					{
						this._StoreEntryID = null;
					}
				}
			}

			public MoveCopy(RuleAction.Type Type, byte[] FolderEntryID) : base(Type)
			{
				this._FolderEntryID = FolderEntryID;
				this.FolderIsInThisStore = true;
			}

			public MoveCopy(RuleAction.Type Type, byte[] StoreEntryID, byte[] FolderEntryID) : base(Type)
			{
				this._StoreEntryID = StoreEntryID;
				this._FolderEntryID = FolderEntryID;
			}

			internal unsafe MoveCopy(_Action* pact) : base(pact)
			{
				this._FolderEntryID = new byte[pact->union.actMoveCopy.cbFolderEntryID];
				Marshal.Copy((IntPtr)((void*)pact->union.actMoveCopy.lpbFolderEntryID), this._FolderEntryID, 0, this._FolderEntryID.Length);
				byte[] array = new byte[pact->union.actMoveCopy.cbStoreEntryID];
				Marshal.Copy((IntPtr)((void*)pact->union.actMoveCopy.lpbStoreEntryID), array, 0, array.Length);
				if (pact->union.actMoveCopy.cbStoreEntryID == 16 && new Guid(array).Equals(RuleAction.MoveCopy.InThisStoreGuid))
				{
					this.FolderIsInThisStore = true;
					return;
				}
				this._StoreEntryID = array;
			}

			internal override int GetBytesToMarshal()
			{
				int num = 0;
				num += (_Action.SizeOf + 7 & -8);
				num += (this._FolderEntryID.Length + 7 & -8);
				return num + ((this.FolderIsInThisStore ? RuleAction.MoveCopy.InThisStoreBytes.Length : this._StoreEntryID.Length) + 7 & -8);
			}

			internal unsafe override void MarshalToNative(_Action* pact, ref byte* pExtra)
			{
				base.BaseMarshal(pact);
				byte* ptr = pExtra;
				pExtra += (IntPtr)(this._FolderEntryID.Length + 7 & -8);
				pact->union.actMoveCopy.cbFolderEntryID = this._FolderEntryID.Length;
				pact->union.actMoveCopy.lpbFolderEntryID = ptr;
				Marshal.Copy(this._FolderEntryID, 0, (IntPtr)((void*)ptr), this._FolderEntryID.Length);
				byte[] array;
				if (this.FolderIsInThisStore)
				{
					array = RuleAction.MoveCopy.InThisStoreBytes;
				}
				else
				{
					array = this._StoreEntryID;
				}
				ptr = pExtra;
				pExtra += (IntPtr)(array.Length + 7 & -8);
				pact->union.actMoveCopy.cbStoreEntryID = array.Length;
				pact->union.actMoveCopy.lpbStoreEntryID = ptr;
				Marshal.Copy(array, 0, (IntPtr)((void*)ptr), array.Length);
			}

			public static readonly Guid InThisStoreGuid = new Guid("62ef076a-3149-4920-b80c-4c3ee1cf2698");

			public static readonly byte[] InThisStoreBytes = RuleAction.MoveCopy.InThisStoreGuid.ToByteArray();

			private byte[] _StoreEntryID;

			private byte[] _FolderEntryID;
		}

		public abstract class BaseReply : RuleAction
		{
			internal BaseReply(RuleAction.Type Type, byte[] ReplyTemplateMessageEntryID, Guid ReplyTemplateGuid, RuleAction.Reply.ActionFlags raf) : base(Type)
			{
				this._ReplyTemplateMessageEntryID = ReplyTemplateMessageEntryID;
				this._ReplyTemplateGuid = ReplyTemplateGuid;
				this._raf = raf;
			}

			internal unsafe BaseReply(_Action* pact) : base(pact)
			{
				this._raf = (RuleAction.Reply.ActionFlags)pact->ActFlavor;
				this._ReplyTemplateGuid = pact->union.actReply.guidReplyTemplate;
				if (pact->union.actReply.cbMessageEntryID != 0)
				{
					this._ReplyTemplateMessageEntryID = new byte[pact->union.actReply.cbMessageEntryID];
					Marshal.Copy((IntPtr)((void*)pact->union.actReply.lpbMessageEntryID), this._ReplyTemplateMessageEntryID, 0, this._ReplyTemplateMessageEntryID.Length);
					return;
				}
				this._ReplyTemplateMessageEntryID = null;
			}

			internal override int GetBytesToMarshal()
			{
				int num = 0;
				num += (_Action.SizeOf + 7 & -8);
				if (this._ReplyTemplateMessageEntryID != null && this._ReplyTemplateMessageEntryID.Length != 0)
				{
					num += (this._ReplyTemplateMessageEntryID.Length + 7 & -8);
				}
				return num;
			}

			internal unsafe override void MarshalToNative(_Action* pact, ref byte* pExtra)
			{
				base.BaseMarshal(pact);
				pact->union.actReply.guidReplyTemplate = this._ReplyTemplateGuid;
				pact->ActFlavor = (uint)this._raf;
				if (this._ReplyTemplateMessageEntryID != null && this._ReplyTemplateMessageEntryID.Length != 0)
				{
					byte* ptr = pExtra;
					pExtra += (IntPtr)(this._ReplyTemplateMessageEntryID.Length + 7 & -8);
					pact->union.actReply.cbMessageEntryID = this._ReplyTemplateMessageEntryID.Length;
					pact->union.actReply.lpbMessageEntryID = ptr;
					Marshal.Copy(this._ReplyTemplateMessageEntryID, 0, (IntPtr)((void*)ptr), this._ReplyTemplateMessageEntryID.Length);
					return;
				}
				pact->union.actReply.cbMessageEntryID = 0;
				pact->union.actReply.lpbMessageEntryID = null;
			}

			protected internal byte[] _ReplyTemplateMessageEntryID;

			protected internal Guid _ReplyTemplateGuid;

			protected internal RuleAction.Reply.ActionFlags _raf;
		}

		public abstract class FwdDelegate : RuleAction
		{
			public AdrEntry[] Recipients
			{
				get
				{
					return this._Recipients;
				}
				set
				{
					this._Recipients = value;
				}
			}

			internal FwdDelegate(RuleAction.Type Type, AdrEntry[] Recipients, RuleAction.Forward.ActionFlags faf) : base(Type)
			{
				this._Recipients = Recipients;
				this._faf = faf;
			}

			internal unsafe FwdDelegate(_Action* pact) : base(pact)
			{
				this._faf = (RuleAction.Forward.ActionFlags)pact->ActFlavor;
				this._Recipients = AdrList.Unmarshal(pact->union.lpadrlist);
			}

			internal override int GetBytesToMarshal()
			{
				int num = 0;
				num += (_Action.SizeOf + 7 & -8);
				return num + AdrList.GetBytesToMarshal(this._Recipients);
			}

			internal unsafe override void MarshalToNative(_Action* pact, ref byte* pExtra)
			{
				base.BaseMarshal(pact);
				pact->ActFlavor = (uint)this._faf;
				pact->union.lpadrlist = pExtra;
				AdrList.MarshalToNative(pExtra, this._Recipients);
				pExtra += (IntPtr)AdrList.GetBytesToMarshal(this._Recipients);
			}

			protected RuleAction.Forward.ActionFlags _faf;

			private AdrEntry[] _Recipients;
		}

		public class Bounce : RuleAction
		{
			public RuleAction.Bounce.BounceCode Code
			{
				get
				{
					return this._sc;
				}
				set
				{
					this._sc = value;
				}
			}

			public Bounce(RuleAction.Bounce.BounceCode Code) : base(RuleAction.Type.OP_BOUNCE)
			{
				this._sc = Code;
			}

			internal unsafe Bounce(_Action* pact) : base(pact)
			{
				this._sc = (RuleAction.Bounce.BounceCode)pact->union.scBounceCode;
			}

			internal override int GetBytesToMarshal()
			{
				int num = 0;
				return num + (_Action.SizeOf + 7 & -8);
			}

			internal unsafe override void MarshalToNative(_Action* pact, ref byte* pExtra)
			{
				base.BaseMarshal(pact);
				pact->union.scBounceCode = (uint)this._sc;
			}

			private RuleAction.Bounce.BounceCode _sc;

			internal enum BounceCode : uint
			{
				TooLarge = 13U,
				FormsMismatch = 31U,
				AccessDenied = 38U
			}
		}

		public class InMailboxMove : RuleAction.MoveCopy
		{
			public InMailboxMove(byte[] FolderEntryID) : base(RuleAction.Type.OP_MOVE, FolderEntryID)
			{
			}

			internal unsafe InMailboxMove(_Action* pact) : base(pact)
			{
			}
		}

		public class ExternalMove : RuleAction.MoveCopy
		{
			public ExternalMove(byte[] StoreEntryID, byte[] FolderEntryID) : base(RuleAction.Type.OP_MOVE, StoreEntryID, FolderEntryID)
			{
			}

			internal unsafe ExternalMove(_Action* pact) : base(pact)
			{
			}
		}

		public class InMailboxCopy : RuleAction.MoveCopy
		{
			public InMailboxCopy(byte[] FolderEntryID) : base(RuleAction.Type.OP_COPY, FolderEntryID)
			{
			}

			internal unsafe InMailboxCopy(_Action* pact) : base(pact)
			{
			}
		}

		public class ExternalCopy : RuleAction.MoveCopy
		{
			public ExternalCopy(byte[] StoreEntryID, byte[] FolderEntryID) : base(RuleAction.Type.OP_COPY, StoreEntryID, FolderEntryID)
			{
			}

			internal unsafe ExternalCopy(_Action* pact) : base(pact)
			{
			}
		}

		public class Reply : RuleAction.BaseReply
		{
			public byte[] ReplyTemplateMessageEntryID
			{
				get
				{
					return this._ReplyTemplateMessageEntryID;
				}
				set
				{
					this._ReplyTemplateMessageEntryID = value;
				}
			}

			public Guid ReplyTemplateGuid
			{
				get
				{
					return this._ReplyTemplateGuid;
				}
				set
				{
					this._ReplyTemplateGuid = value;
				}
			}

			public RuleAction.Reply.ActionFlags Flags
			{
				get
				{
					return this._raf;
				}
				set
				{
					this._raf = value;
				}
			}

			public Reply(byte[] ReplyTemplateMessageEntryID, Guid ReplyTemplateGuid, RuleAction.Reply.ActionFlags Flags) : base(RuleAction.Type.OP_REPLY, ReplyTemplateMessageEntryID, ReplyTemplateGuid, Flags)
			{
			}

			internal unsafe Reply(_Action* pact) : base(pact)
			{
			}

			internal enum ActionFlags : uint
			{
				None,
				DoNotSendToOriginator,
				UseStockReplyTemplate
			}
		}

		public class OOFReply : RuleAction.BaseReply
		{
			public byte[] ReplyTemplateMessageEntryID
			{
				get
				{
					return this._ReplyTemplateMessageEntryID;
				}
				set
				{
					this._ReplyTemplateMessageEntryID = value;
				}
			}

			public Guid ReplyTemplateGuid
			{
				get
				{
					return this._ReplyTemplateGuid;
				}
				set
				{
					this._ReplyTemplateGuid = value;
				}
			}

			public OOFReply(byte[] ReplyTemplateMessageEntryID, Guid ReplyTemplateGuid) : base(RuleAction.Type.OP_OOF_REPLY, ReplyTemplateMessageEntryID, ReplyTemplateGuid, RuleAction.Reply.ActionFlags.None)
			{
			}

			internal unsafe OOFReply(_Action* pact) : base(pact)
			{
			}
		}

		public class Defer : RuleAction
		{
			public byte[] Data
			{
				get
				{
					return this._Data;
				}
				set
				{
					this._Data = value;
				}
			}

			public Defer(byte[] Data) : base(RuleAction.Type.OP_DEFER_ACTION)
			{
				this._Data = Data;
			}

			internal unsafe Defer(_Action* pact) : base(pact)
			{
				this._Data = new byte[pact->union.actDeferAction.cb];
				if (pact->union.actDeferAction.cb != 0)
				{
					Marshal.Copy((IntPtr)((void*)pact->union.actDeferAction.lpb), this._Data, 0, this._Data.Length);
				}
			}

			internal override int GetBytesToMarshal()
			{
				int num = 0;
				num += (_Action.SizeOf + 7 & -8);
				return num + (this._Data.Length + 7 & -8);
			}

			internal unsafe override void MarshalToNative(_Action* pact, ref byte* pExtra)
			{
				base.BaseMarshal(pact);
				byte* ptr = pExtra;
				pExtra += (IntPtr)(this._Data.Length + 7 & -8);
				pact->union.actDeferAction.cb = this._Data.Length;
				if (this._Data.Length != 0)
				{
					pact->union.actDeferAction.lpb = ptr;
					Marshal.Copy(this._Data, 0, (IntPtr)((void*)ptr), this._Data.Length);
					return;
				}
				pact->union.actDeferAction.lpb = null;
			}

			private byte[] _Data;
		}

		public class Forward : RuleAction.FwdDelegate
		{
			public RuleAction.Forward.ActionFlags Flags
			{
				get
				{
					return this._faf;
				}
				set
				{
					this._faf = value;
				}
			}

			public Forward(AdrEntry[] Recipients, RuleAction.Forward.ActionFlags Flags) : base(RuleAction.Type.OP_FORWARD, Recipients, Flags)
			{
			}

			internal unsafe Forward(_Action* pact) : base(pact)
			{
			}

			internal enum ActionFlags : uint
			{
				None,
				PreserveSender,
				DoNotMungeMessage,
				ForwardAsAttachment = 4U,
				SendSmsAlert = 8U
			}
		}

		public class Delegate : RuleAction.FwdDelegate
		{
			public Delegate(AdrEntry[] Recipients) : base(RuleAction.Type.OP_DELEGATE, Recipients, RuleAction.Forward.ActionFlags.None)
			{
			}

			internal unsafe Delegate(_Action* pact) : base(pact)
			{
			}
		}

		public class Tag : RuleAction
		{
			public PropValue Value
			{
				get
				{
					return this._value;
				}
				set
				{
					this._value = value;
				}
			}

			public Tag(PropValue Value) : base(RuleAction.Type.OP_TAG)
			{
				this._value = Value;
			}

			internal unsafe Tag(_Action* pact) : base(pact)
			{
				this._value = new PropValue(&pact->union.propTag);
			}

			internal override int GetBytesToMarshal()
			{
				int num = 0;
				num += (_Action.SizeOf + 7 & -8);
				return num + this._value.GetBytesToMarshal();
			}

			internal unsafe override void MarshalToNative(_Action* pact, ref byte* pExtra)
			{
				base.BaseMarshal(pact);
				this._value.MarshalToNative(&pact->union.propTag, ref pExtra);
			}

			private PropValue _value;
		}

		public class Delete : RuleAction
		{
			public Delete() : base(RuleAction.Type.OP_DELETE)
			{
			}

			internal unsafe Delete(_Action* pact) : base(pact)
			{
			}

			internal override int GetBytesToMarshal()
			{
				int num = 0;
				return num + (_Action.SizeOf + 7 & -8);
			}

			internal unsafe override void MarshalToNative(_Action* pact, ref byte* pExtra)
			{
				base.BaseMarshal(pact);
			}
		}

		public class MarkAsRead : RuleAction
		{
			public MarkAsRead() : base(RuleAction.Type.OP_MARK_AS_READ)
			{
			}

			internal unsafe MarkAsRead(_Action* pact) : base(pact)
			{
			}

			internal override int GetBytesToMarshal()
			{
				int num = 0;
				return num + (_Action.SizeOf + 7 & -8);
			}

			internal unsafe override void MarshalToNative(_Action* pact, ref byte* pExtra)
			{
				base.BaseMarshal(pact);
			}
		}
	}
}
