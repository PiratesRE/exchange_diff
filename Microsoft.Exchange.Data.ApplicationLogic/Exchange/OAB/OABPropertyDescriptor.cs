using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OABPropertyDescriptor
	{
		public PropTag PropTag { get; set; }

		public OABPropertyFlags PropFlags { get; set; }

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write((uint)this.PropTag);
			writer.Write((uint)this.PropFlags);
		}

		public static OABPropertyDescriptor ReadFrom(BinaryReader reader, string elementName)
		{
			return new OABPropertyDescriptor
			{
				PropTag = (PropTag)reader.ReadUInt32(elementName + ".PropTag"),
				PropFlags = (OABPropertyFlags)reader.ReadUInt32(elementName + ".PropFlags")
			};
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"PropTag=",
				this.PropTag.ToString("X8"),
				", PropFlags=",
				this.PropFlags
			});
		}

		public static readonly int Size = 8;
	}
}
