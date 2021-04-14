using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Data.Storage.FolderTask
{
	[DataContract]
	[Serializable]
	public sealed class FolderInfo
	{
		[DataMember]
		public string FolderPath { get; private set; }

		[DataMember]
		public ulong Size { get; private set; }

		public FolderInfo(string path, ulong size)
		{
			this.FolderPath = path;
			this.Size = size;
		}

		public static Dictionary<string, FolderInfo> BuildFolderHierarchyMap(HierarchyFolderNode rootFolder, List<FolderInfo> foldersToProcess, string folderPathSeparator, char[] folderPathSplitCharacters)
		{
			Dictionary<string, FolderInfo> dictionary = new Dictionary<string, FolderInfo>();
			foreach (FolderInfo folderInfo in foldersToProcess)
			{
				StringBuilder stringBuilder = new StringBuilder();
				HierarchyFolderNode hierarchyFolderNode = rootFolder;
				string[] array = folderInfo.FolderPath.Split(folderPathSplitCharacters, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					string value = array[i];
					stringBuilder.Append(folderPathSeparator);
					stringBuilder.Append(value);
					string text = stringBuilder.ToString();
					HierarchyFolderNode hierarchyFolderNode2 = hierarchyFolderNode.ChildNodes[text];
					if (hierarchyFolderNode2 == null)
					{
						hierarchyFolderNode2 = new HierarchyFolderNode(text);
						if (i == array.Length - 1)
						{
							hierarchyFolderNode2.TotalItemSize = folderInfo.Size;
							hierarchyFolderNode2.AggregateTotalItemSize = folderInfo.Size;
							dictionary[text] = folderInfo;
						}
						hierarchyFolderNode.ChildNodes.Add(text, hierarchyFolderNode2);
					}
					hierarchyFolderNode.AggregateTotalItemSize += folderInfo.Size;
					hierarchyFolderNode = hierarchyFolderNode2;
				}
			}
			return dictionary;
		}
	}
}
