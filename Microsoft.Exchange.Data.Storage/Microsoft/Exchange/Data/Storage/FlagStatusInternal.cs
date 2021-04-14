using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FlagStatusInternal
	{
		internal FlagStatusInternal()
		{
		}

		internal StoreObjectId ExistingItemObjectId
		{
			get
			{
				return this.existingItemObjectId;
			}
			set
			{
				this.existingItemObjectId = value;
			}
		}

		internal StoreObjectId ParentId
		{
			get
			{
				return this.parentId;
			}
			set
			{
				this.parentId = value;
			}
		}

		internal bool IsDirty()
		{
			return this.markMsgFlagsToSet != 0 || this.markMsgStatusToSet != 0;
		}

		private static int GetSetReadFlagsBitMask()
		{
			return 513;
		}

		internal int GetSetReadFlag()
		{
			int num = this.markMsgFlagsToSet & FlagStatusInternal.GetSetReadFlagsBitMask();
			if (num == 0)
			{
				return -1;
			}
			SetReadFlags setReadFlags = SetReadFlags.None;
			if ((num & 1) != 0)
			{
				if ((this.msgFlags & 1) != 0)
				{
					setReadFlags = setReadFlags;
				}
				else
				{
					setReadFlags |= SetReadFlags.ClearRead;
				}
			}
			if ((num & 512) != 0 && (this.msgFlags & 512) != 0)
			{
				setReadFlags |= SetReadFlags.SuppressReceipt;
			}
			return (int)setReadFlags;
		}

		internal bool? TryGetValue(PropertyDefinition propertyDefinition, int flag)
		{
			if (propertyDefinition.Equals(InternalSchema.Flags))
			{
				if ((this.markMsgFlagsForRead & flag) == 0)
				{
					return null;
				}
				return new bool?((this.msgFlags & flag) != 0);
			}
			else
			{
				if (propertyDefinition != InternalSchema.MessageStatus)
				{
					throw new InvalidOperationException();
				}
				if ((this.markMsgStatusForRead & flag) == 0)
				{
					return null;
				}
				return new bool?((this.msgStatus & flag) != 0);
			}
		}

		internal bool GetDirtyStatusBits(out int bitsSet, out int bitsClear)
		{
			bitsSet = 0;
			bitsClear = 0;
			if (this.markMsgStatusToSet == 0)
			{
				return false;
			}
			bitsSet = (this.markMsgStatusToSet & this.msgStatus);
			bitsClear = (this.markMsgStatusToSet & ~this.msgStatus);
			return true;
		}

		internal bool GetNonReadFlagsBits(out int bitsSet, out int bitsClear)
		{
			bitsSet = 0;
			bitsClear = 0;
			int num = this.markMsgFlagsToSet & ~FlagStatusInternal.GetSetReadFlagsBitMask();
			if (num == 0)
			{
				return false;
			}
			bitsSet = (num & this.msgFlags);
			bitsClear = (num & ~this.msgFlags);
			return true;
		}

		internal void ClearFlagsForSet(PropertyDefinition propertyDefinition)
		{
			MessageFlagsProperty messageFlagsProperty = propertyDefinition as MessageFlagsProperty;
			if (messageFlagsProperty.NativeProperty == InternalSchema.Flags)
			{
				this.markMsgFlagsToSet &= ~messageFlagsProperty.Flag;
				return;
			}
			if (messageFlagsProperty.NativeProperty == InternalSchema.MessageStatus)
			{
				this.markMsgStatusToSet &= ~messageFlagsProperty.Flag;
			}
		}

		internal void SetFlagsPropertyOnItem(PropertyDefinition propertyDefinition, int flag, bool value)
		{
			if (propertyDefinition != InternalSchema.Flags)
			{
				if (propertyDefinition == InternalSchema.MessageStatus)
				{
					this.markMsgStatusToSet |= flag;
					this.markMsgStatusForRead |= flag;
					if (value)
					{
						this.msgStatus |= flag;
						return;
					}
					this.msgStatus &= ~flag;
				}
				return;
			}
			this.markMsgFlagsToSet |= flag;
			this.markMsgFlagsForRead |= flag;
			if (value)
			{
				this.msgFlags |= flag;
				return;
			}
			this.msgFlags &= ~flag;
		}

		private StoreObjectId existingItemObjectId;

		private StoreObjectId parentId;

		private int markMsgFlagsToSet;

		private int markMsgFlagsForRead;

		private int msgFlags;

		private int markMsgStatusToSet;

		private int markMsgStatusForRead;

		private int msgStatus;
	}
}
