using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MessageClassBasedDefaultFolderCreator : DefaultFolderCreator
	{
		internal MessageClassBasedDefaultFolderCreator(DefaultFolderType container, string containerClass, bool bindByNameIfAlreadyExists = true) : base(container, StoreObjectType.Folder, bindByNameIfAlreadyExists)
		{
			this.containerClass = containerClass;
		}

		internal override Folder Create(DefaultFolderContext context, string folderName, StoreObjectId parentId, out bool hasCreatedNew)
		{
			hasCreatedNew = false;
			bool flag = false;
			Folder folder = null;
			try
			{
				using (Folder folder2 = Folder.Bind(context.Session, parentId))
				{
					using (QueryResult queryResult = folder2.FolderQuery(FolderQueryFlags.None, null, null, MessageClassBasedDefaultFolderCreator.LoadProperties))
					{
						ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ContainerClass, this.containerClass);
						StoreObjectId storeObjectId = null;
						ExDateTime t = ExDateTime.MaxValue;
						while (queryResult.SeekToCondition(SeekReference.OriginCurrent, seekFilter))
						{
							object[][] rows = queryResult.GetRows(1);
							ExDateTime exDateTime = (ExDateTime)rows[0][2];
							if (exDateTime < t)
							{
								storeObjectId = ((VersionedId)rows[0][0]).ObjectId;
								t = exDateTime;
							}
						}
						if (storeObjectId != null)
						{
							folder = Folder.Bind(context.Session, storeObjectId);
						}
					}
				}
				if (folder == null)
				{
					folder = base.Create(context, folderName, parentId, out hasCreatedNew);
					if (hasCreatedNew)
					{
						this.StampExtraPropertiesOnNewlyCreatedFolder(folder);
					}
				}
				flag = true;
			}
			finally
			{
				if (!flag && folder != null)
				{
					folder.Dispose();
					folder = null;
				}
			}
			return folder;
		}

		protected virtual void StampExtraPropertiesOnNewlyCreatedFolder(Folder folder)
		{
		}

		private static readonly PropertyDefinition[] LoadProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			StoreObjectSchema.ContainerClass,
			StoreObjectSchema.CreationTime
		};

		private readonly string containerClass;
	}
}
