using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderTreeProcessor
	{
		internal static T[] ShiftDefaultFoldersToTop<T>(T[] folderCollection, Func<T, DistinguishedFolderIdName> distinguishedFolderIdNameExtractor, Func<T, string> parentFolderIdExtractor, DistinguishedFolderIdName[] foldersToMoveToTop)
		{
			try
			{
				List<T> list = new List<T>(folderCollection.Length);
				FolderTreeNode[] requiredFoldersSubTrees = FolderTreeProcessor.GetRequiredFoldersSubTrees<T>(folderCollection, distinguishedFolderIdNameExtractor, parentFolderIdExtractor, foldersToMoveToTop);
				FolderTreeProcessor.AddRequiredFolderSubTree<T>(list, folderCollection, requiredFoldersSubTrees);
				FolderTreeProcessor.AddRestOfTheFolderTree<T>(list, folderCollection, requiredFoldersSubTrees);
				return list.ToArray();
			}
			catch (Exception ex)
			{
				if (!ExWatson.IsWatsonReportAlreadySent(ex))
				{
					ExWatson.SendReport(ex, ReportOptions.ReportTerminateAfterSend, null);
					ExWatson.SetWatsonReportAlreadySent(ex);
				}
			}
			return folderCollection;
		}

		private static void AddRestOfTheFolderTree<T>(List<T> reOrderedList, T[] folderCollection, FolderTreeNode[] requiredFoldersSubTrees)
		{
			int i = 0;
			while (i < folderCollection.Length)
			{
				int num = FolderTreeProcessor.IsIndexSubFolderOfRequiredFolders(i, requiredFoldersSubTrees);
				if (num == -1)
				{
					reOrderedList.Add(folderCollection[i]);
					i++;
				}
				else
				{
					i += num;
				}
			}
		}

		private static int IsIndexSubFolderOfRequiredFolders(int folderIndex, FolderTreeNode[] requiredFoldersSubTrees)
		{
			foreach (FolderTreeNode folderTreeNode in requiredFoldersSubTrees)
			{
				if (folderTreeNode != null && folderIndex == folderTreeNode.Index)
				{
					return folderTreeNode.DescendantCount;
				}
			}
			return -1;
		}

		private static void AddRequiredFolderSubTree<T>(List<T> reOrderedList, T[] folderCollection, FolderTreeNode[] requiredFoldersSubTrees)
		{
			foreach (FolderTreeNode folderTreeNode in requiredFoldersSubTrees)
			{
				if (folderTreeNode != null)
				{
					for (int j = folderTreeNode.Index; j < folderTreeNode.Index + folderTreeNode.DescendantCount; j++)
					{
						reOrderedList.Add(folderCollection[j]);
					}
				}
			}
		}

		private static FolderTreeNode[] GetRequiredFoldersSubTrees<T>(T[] folderCollection, Func<T, DistinguishedFolderIdName> distinguishedFolderIdNameExtractor, Func<T, string> parentFolderIdExtractor, DistinguishedFolderIdName[] foldersToMoveToTop)
		{
			FolderTreeNode[] array = new FolderTreeNode[foldersToMoveToTop.Length];
			int num = 0;
			int num2 = 0;
			while (num2 < folderCollection.Length && num <= array.Length)
			{
				T arg = folderCollection[num2];
				DistinguishedFolderIdName folderIdName = distinguishedFolderIdNameExtractor(arg);
				int indexToInsertTreeNode = FolderTreeProcessor.GetIndexToInsertTreeNode(folderIdName, foldersToMoveToTop);
				if (indexToInsertTreeNode == -1)
				{
					num2++;
				}
				else
				{
					int numberOfSubFolders = FolderTreeProcessor.GetNumberOfSubFolders<T>(folderCollection, num2, parentFolderIdExtractor);
					FolderTreeNode folderTreeNode = new FolderTreeNode(num2, numberOfSubFolders);
					array[indexToInsertTreeNode] = folderTreeNode;
					num2 += numberOfSubFolders;
					num++;
				}
			}
			return array;
		}

		private static int GetIndexToInsertTreeNode(DistinguishedFolderIdName folderIdName, DistinguishedFolderIdName[] foldersToMoveToTop)
		{
			for (int i = 0; i < foldersToMoveToTop.Length; i++)
			{
				DistinguishedFolderIdName distinguishedFolderIdName = foldersToMoveToTop[i];
				if (folderIdName == distinguishedFolderIdName)
				{
					return i;
				}
			}
			return -1;
		}

		private static int GetNumberOfSubFolders<T>(T[] folderCollection, int index, Func<T, string> parentFolderIdExtractor)
		{
			int num = 1;
			T arg = folderCollection[index];
			for (int i = index + 1; i < folderCollection.Length; i++)
			{
				T arg2 = folderCollection[i];
				if (!(parentFolderIdExtractor(arg2) != parentFolderIdExtractor(arg)))
				{
					break;
				}
				num++;
			}
			return num;
		}
	}
}
