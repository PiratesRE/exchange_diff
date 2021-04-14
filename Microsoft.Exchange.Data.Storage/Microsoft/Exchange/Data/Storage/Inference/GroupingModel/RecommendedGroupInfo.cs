using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecommendedGroupInfo : IRecommendedGroupInfo
	{
		public RecommendedGroupInfo()
		{
			this.ID = Guid.Empty;
			this.Members = new List<string>();
			this.Words = new List<string>();
		}

		public Guid ID { get; set; }

		public List<string> Members { get; set; }

		public List<string> Words { get; set; }

		public void Write(BinaryWriter writer)
		{
			writer.Write(this.ID.ToByteArray());
			writer.Write(this.Members.Count);
			foreach (string value in this.Members)
			{
				writer.Write(value);
			}
			writer.Write(this.Words.Count);
			foreach (string value2 in this.Words)
			{
				writer.Write(value2);
			}
		}

		public void Read(BinaryReader reader)
		{
			Guid id = new Guid(reader.ReadBytes(16));
			this.ID = id;
			int num = reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string item = reader.ReadString();
				this.Members.Add(item);
			}
			int num2 = reader.ReadInt32();
			for (int j = 0; j < num2; j++)
			{
				string item2 = reader.ReadString();
				this.Words.Add(item2);
			}
		}

		private const int GuidBytesLength = 16;
	}
}
