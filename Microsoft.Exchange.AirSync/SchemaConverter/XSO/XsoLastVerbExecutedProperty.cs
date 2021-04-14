using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal class XsoLastVerbExecutedProperty : XsoIntegerProperty
	{
		public XsoLastVerbExecutedProperty() : base(MessageItemSchema.LastVerbExecuted, false)
		{
		}

		public override int IntegerData
		{
			get
			{
				switch (base.XsoItem.GetValueOrDefault<int>(base.PropertyDef, -1))
				{
				case 102:
					return 1;
				case 103:
					return 2;
				case 104:
					return 3;
				default:
					return 0;
				}
			}
		}

		protected override void InternalCopyFromModified(IProperty srcProperty)
		{
			switch (((IIntegerProperty)srcProperty).IntegerData)
			{
			case 1:
				base.XsoItem[base.PropertyDef] = 102;
				return;
			case 2:
				base.XsoItem[base.PropertyDef] = 103;
				return;
			case 3:
				base.XsoItem[base.PropertyDef] = 104;
				return;
			default:
				return;
			}
		}

		private struct ProtocolValue
		{
			public const int Unknown = 0;

			public const int ReplyToSender = 1;

			public const int ReplyToAll = 2;

			public const int Forward = 3;
		}

		private struct StoreValue
		{
			public const int ReplyToSender = 102;

			public const int ReplyToAll = 103;

			public const int Forward = 104;
		}
	}
}
