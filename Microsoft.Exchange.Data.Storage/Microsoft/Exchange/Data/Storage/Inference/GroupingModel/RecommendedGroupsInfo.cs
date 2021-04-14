using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Inference.GroupingModel
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecommendedGroupsInfo
	{
		public RecommendedGroupsInfo()
		{
			this.RecommendedGroups = new List<RecommendedGroupInfo>();
		}

		public List<RecommendedGroupInfo> RecommendedGroups { get; set; }

		public void Write(BinaryWriter writer)
		{
			writer.Write((short)RecommendedGroupsInfo.serializationVersion);
			writer.Write(this.serializationFlags);
			writer.Write(this.RecommendedGroups.Count);
			foreach (RecommendedGroupInfo recommendedGroupInfo in this.RecommendedGroups)
			{
				recommendedGroupInfo.Write(writer);
			}
		}

		public void Read(BinaryReader reader)
		{
			int num = (int)reader.ReadInt16();
			if (num > RecommendedGroupsInfo.serializationVersion)
			{
				throw new SerializationException(string.Format("Recommended groups info was serialized using an unsupported serialization version. Recommended groups info serialization version: {0}, Current serialization version: {1}", num, RecommendedGroupsInfo.serializationVersion));
			}
			this.serializationFlags = reader.ReadInt64();
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				RecommendedGroupInfo recommendedGroupInfo = new RecommendedGroupInfo();
				recommendedGroupInfo.Read(reader);
				this.RecommendedGroups.Add(recommendedGroupInfo);
			}
		}

		public const string Name = "Inference.RecommendedGroups";

		private static readonly int serializationVersion = 1;

		private long serializationFlags;
	}
}
