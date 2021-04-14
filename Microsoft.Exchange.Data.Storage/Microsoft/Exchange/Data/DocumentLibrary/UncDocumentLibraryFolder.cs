using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UncDocumentLibraryFolder : UncDocumentLibraryItem, IDocumentLibraryFolder, IDocumentLibraryItem, IReadOnlyPropertyBag
	{
		internal UncDocumentLibraryFolder(UncSession session, UncObjectId objectId) : base(session, objectId, new DirectoryInfo(objectId.Path.LocalPath), UncFolderSchema.Instance)
		{
			this.directoryInfo = (this.fileSystemInfo as DirectoryInfo);
		}

		public new static UncDocumentLibraryFolder Read(UncSession session, ObjectId folderId)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			UncObjectId uncObjectId = folderId as UncObjectId;
			if (uncObjectId == null)
			{
				throw new ArgumentException("folderId");
			}
			return Utils.DoUncTask<UncDocumentLibraryFolder>(session.Identity, uncObjectId, false, Utils.MethodType.Read, delegate
			{
				FileSystemInfo fileSystemInfo = new FileInfo(uncObjectId.Path.LocalPath);
				if (fileSystemInfo.Attributes != (FileAttributes)(-1) && (fileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
				{
					return new UncDocumentLibraryFolder(session, uncObjectId);
				}
				throw new ObjectNotFoundException(uncObjectId, Strings.ExObjectNotFound(uncObjectId.Path.LocalPath));
			});
		}

		public ITableView GetView(QueryFilter query, SortBy[] sortBy, DocumentLibraryQueryOptions queryOptions, params PropertyDefinition[] propsToReturn)
		{
			return UncDocumentLibraryFolder.InternalGetView(query, sortBy, queryOptions, propsToReturn, this.session, this.directoryInfo, base.UncId);
		}

		protected override string GetParentDirectoryNameInternal()
		{
			if (this.directoryInfo.Parent == null)
			{
				return null;
			}
			return this.directoryInfo.Parent.FullName;
		}

		internal static ITableView InternalGetView(QueryFilter query, SortBy[] sortBy, DocumentLibraryQueryOptions queryOptions, PropertyDefinition[] propsToReturn, UncSession session, DirectoryInfo directoryInfo, UncObjectId id)
		{
			if (propsToReturn == null)
			{
				throw new ArgumentNullException("propsToReturn");
			}
			if (propsToReturn.Length == 0)
			{
				throw new ArgumentException("propsToReturn");
			}
			DocumentLibraryPropertyDefinition[] propertyDefinitions = new DocumentLibraryPropertyDefinition[propsToReturn.Length];
			for (int i = 0; i < propsToReturn.Length; i++)
			{
				propertyDefinitions[i] = (propsToReturn[i] as DocumentLibraryPropertyDefinition);
				if (propertyDefinitions[i] == null)
				{
					throw new ArgumentException("propsToReturn");
				}
			}
			return Utils.DoUncTask<ArrayTableView>(session.Identity, id, true, Utils.MethodType.GetView, delegate
			{
				FileSystemInfo[] array;
				switch (queryOptions)
				{
				case DocumentLibraryQueryOptions.Folders:
					array = directoryInfo.GetDirectories();
					break;
				case DocumentLibraryQueryOptions.Files:
					array = directoryInfo.GetFiles();
					break;
				case DocumentLibraryQueryOptions.FoldersAndFiles:
					array = directoryInfo.GetFileSystemInfos();
					break;
				default:
					throw new ArgumentOutOfRangeException("queryOptions");
				}
				List<object[]> list = new List<object[]>();
				int num = 0;
				int num2 = Utils.GetViewMaxRows;
				while (num < array.Length && num2 > 0)
				{
					object[] array2 = new object[propertyDefinitions.Length];
					for (int j = 0; j < propertyDefinitions.Length; j++)
					{
						array2[j] = UncDocumentLibraryItem.GetValueFromFileSystemInfo(propertyDefinitions[j], array[num]);
					}
					list.Add(array2);
					num2--;
					num++;
				}
				return new ArrayTableView(query, sortBy, propertyDefinitions, list);
			});
		}

		private DirectoryInfo directoryInfo;
	}
}
