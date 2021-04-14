using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StringData : ConstStringData
	{
		public StringData()
		{
		}

		public StringData(string data) : base(data)
		{
		}

		public override ushort TypeId
		{
			get
			{
				return StringData.typeId;
			}
			set
			{
				StringData.typeId = value;
			}
		}

		public override void DeserializeData(BinaryReader reader, ComponentDataPool componentDataPool)
		{
			if (reader.ReadBoolean())
			{
				base.Data = null;
				return;
			}
			base.Data = reader.ReadString();
		}

		public override ICustomSerializable BuildObject()
		{
			return new StringData();
		}

		private static ushort typeId;
	}
}
