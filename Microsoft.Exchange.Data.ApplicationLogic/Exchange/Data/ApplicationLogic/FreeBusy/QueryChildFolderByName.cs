using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.FreeBusy
{
	internal static class QueryChildFolderByName
	{
		public static StoreObjectId Query(Folder parentFolder, string childFolderName)
		{
			StoreObjectId objectId;
			using (QueryResult queryResult = QueryChildFolderByName.CreateQueryResult(parentFolder))
			{
				VersionedId versionedId;
				for (;;)
				{
					object[][] rows = queryResult.GetRows(100);
					if (rows == null || rows.Length == 0)
					{
						break;
					}
					foreach (object[] array2 in rows)
					{
						string text = array2[0] as string;
						versionedId = (array2[1] as VersionedId);
						if (text != null && versionedId != null && StringComparer.InvariantCultureIgnoreCase.Equals(text, childFolderName))
						{
							goto Block_5;
						}
					}
				}
				return null;
				Block_5:
				objectId = versionedId.ObjectId;
			}
			return objectId;
		}

		private static QueryResult CreateQueryResult(Folder folder)
		{
			return folder.FolderQuery(FolderQueryFlags.None, null, null, QueryChildFolderByName.properties);
		}

		private const int DisplayNameIndex = 0;

		private const int IdIndex = 1;

		private const int QueryRowBatch = 100;

		private static readonly PropertyDefinition[] properties = new PropertyDefinition[]
		{
			FolderSchema.DisplayName,
			FolderSchema.Id
		};
	}
}
