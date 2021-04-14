using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class TextMessagingDeliveryPointState : TextMessagingStateBase
	{
		public TextMessagingDeliveryPointState(int rawValue) : base(rawValue)
		{
		}

		public TextMessagingDeliveryPointState(bool shared, bool p2pEnabled, bool m2pEnabled, DeliveryPointType type, byte id, byte p2pPriority, byte m2pPriority) : base(0)
		{
			this.Shared = shared;
			this.PersonToPersonMessagingEnabled = p2pEnabled;
			this.MachineToPersonMessagingEnabled = m2pEnabled;
			this.Type = type;
			this.Identity = id;
			this.PersonToPersonMessagingPriority = p2pPriority;
			this.MachineToPersonMessagingPriority = m2pPriority;
		}

		public bool Shared
		{
			get
			{
				return 0 != (1073741824 & base.RawValue);
			}
			private set
			{
				base.RawValue &= -1073741825;
				base.RawValue ^= (value ? 1073741824 : 0);
			}
		}

		public bool PersonToPersonMessagingEnabled
		{
			get
			{
				return 0 != (536870912 & base.RawValue);
			}
			private set
			{
				base.RawValue &= -536870913;
				base.RawValue ^= (value ? 536870912 : 0);
			}
		}

		public bool MachineToPersonMessagingEnabled
		{
			get
			{
				return 0 != (268435456 & base.RawValue);
			}
			private set
			{
				base.RawValue &= -268435457;
				base.RawValue ^= (value ? 268435456 : 0);
			}
		}

		public DeliveryPointType Type
		{
			get
			{
				int num = (251658240 & base.RawValue) >> 24;
				if (!Enum.IsDefined(typeof(DeliveryPointType), num))
				{
					return DeliveryPointType.Unknown;
				}
				return (DeliveryPointType)num;
			}
			private set
			{
				base.RawValue &= -251658241;
				base.RawValue ^= (int)((int)value << 24);
			}
		}

		public byte Identity
		{
			get
			{
				return (byte)((16711680 & base.RawValue) >> 16);
			}
			private set
			{
				base.RawValue &= -16711681;
				base.RawValue ^= (int)value << 16;
			}
		}

		public byte PersonToPersonMessagingPriority
		{
			get
			{
				return (byte)((65280 & base.RawValue) >> 8);
			}
			private set
			{
				base.RawValue &= -65281;
				base.RawValue ^= (int)value << 8;
			}
		}

		public byte MachineToPersonMessagingPriority
		{
			get
			{
				return (byte)(255 & base.RawValue);
			}
			private set
			{
				base.RawValue &= -256;
				base.RawValue ^= (int)value;
			}
		}

		internal const int StartBitShared = 30;

		internal const int StartBitP2pEnabled = 29;

		internal const int StartBitM2pEnabled = 28;

		internal const int StartBitType = 24;

		internal const int StartBitIdentity = 16;

		internal const int StartBitP2pPriority = 8;

		internal const int StartBitM2pPriority = 0;

		internal const int MaskShared = 1073741824;

		internal const int MaskP2pEnabled = 536870912;

		internal const int MaskM2pEnabled = 268435456;

		internal const int MaskType = 251658240;

		internal const int MaskIdentity = 16711680;

		internal const int MaskP2pPriority = 65280;

		internal const int MaskM2pPriority = 255;
	}
}
