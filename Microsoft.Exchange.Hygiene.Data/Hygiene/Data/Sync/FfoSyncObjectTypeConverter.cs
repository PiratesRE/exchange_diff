using System;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal static class FfoSyncObjectTypeConverter
	{
		public static DirectoryObjectClass FromFfoType(FfoSyncObjectType ffoType)
		{
			string name = Enum.GetName(typeof(FfoSyncObjectType), ffoType);
			return (DirectoryObjectClass)Enum.Parse(typeof(DirectoryObjectClass), name);
		}

		public static FfoSyncObjectType ToFfoType(DirectoryObjectClass msodsType)
		{
			string name = Enum.GetName(typeof(DirectoryObjectClass), msodsType);
			return (FfoSyncObjectType)Enum.Parse(typeof(FfoSyncObjectType), name);
		}
	}
}
