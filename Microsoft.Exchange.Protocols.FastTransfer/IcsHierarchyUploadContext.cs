using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.FastTransfer;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class IcsHierarchyUploadContext : IcsUploadContext
	{
		public override ErrorCode Configure(MapiContext operationContext, MapiFolder folder)
		{
			ErrorCode first = base.Configure(operationContext, folder);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)65372U);
			}
			base.ParentObject = folder;
			return ErrorCode.NoError;
		}

		public ErrorCode ImportFolderChange(MapiContext operationContext, Properties hierarchyProps, Properties folderProps, out ExchangeId outputFolderId)
		{
			base.CheckDisposed();
			base.TraceInitialState(operationContext);
			outputFolderId = ExchangeId.Zero;
			ErrorCode first = this.ValidateFolderChangeParams(hierarchyProps);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)40796U);
			}
			byte[] array = (byte[])hierarchyProps[0].Value;
			byte[] array2 = (byte[])hierarchyProps[1].Value;
			DateTime dateTime = (DateTime)hierarchyProps[2].Value;
			byte[] array3 = (byte[])hierarchyProps[3].Value;
			byte[] array4 = (byte[])hierarchyProps[4].Value;
			string text = hierarchyProps[5].Value as string;
			if (ExTraceGlobals.IcsUploadStateTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("ImportFolderChange: ParentSourceKey=[");
				stringBuilder.AppendAsString(array);
				stringBuilder.Append("] SourceKey=[");
				stringBuilder.AppendAsString(array2);
				stringBuilder.Append("] LastModified=[");
				stringBuilder.AppendAsString(dateTime);
				stringBuilder.Append("] ChangeKey=[");
				stringBuilder.AppendAsString(array3);
				stringBuilder.Append("] PCL=[");
				stringBuilder.AppendAsString(array4);
				stringBuilder.Append("] DisplayName=[");
				stringBuilder.AppendAsString(text);
				stringBuilder.Append("]");
				ExTraceGlobals.IcsUploadStateTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			ExchangeId exchangeId;
			if (array.Length == 0)
			{
				exchangeId = ((MapiFolder)base.ParentObject).Fid;
				array = null;
			}
			else
			{
				exchangeId = base.FolderIdFromSourceKey(operationContext, ref array);
				if (exchangeId.IsNullOrZero)
				{
					return ErrorCode.CreateSyncNoParent((LID)59544U);
				}
			}
			ExchangeId exchangeId2 = base.FolderIdFromSourceKey(operationContext, ref array2);
			using (MapiFolder mapiFolder = MapiFolder.OpenFolder(operationContext, base.Logon, exchangeId))
			{
				if (mapiFolder == null)
				{
					return ErrorCode.CreateSyncNoParent((LID)32853U);
				}
				PCL pcl = default(PCL);
				if (!pcl.TryLoadBinaryLXCN(array4))
				{
					return ErrorCode.CreateInvalidParameter((LID)34968U);
				}
				pcl.Add(array3);
				Properties properties = new Properties(folderProps.Count);
				bool flag = false;
				bool flag2 = false;
				MapiFolder mapiFolder2 = null;
				try
				{
					if (exchangeId2.IsValid)
					{
						mapiFolder2 = MapiFolder.OpenFolder(operationContext, base.Logon, exchangeId2);
					}
					if (mapiFolder2 == null)
					{
						bool isValid = exchangeId2.IsValid;
						FolderConfigureFlags flags = FolderConfigureFlags.None;
						object value = folderProps.GetValue(PropTag.Folder.FolderType);
						if (value is int && (int)value == 2)
						{
							flags = FolderConfigureFlags.CreateSearchFolder;
						}
						ErrorCode first2 = MapiFolder.CreateFolder(operationContext, base.Logon, ref exchangeId2, false, flags, mapiFolder, MapiFolder.ManageAssociatedDumpsterFolder(base.Logon, false), out mapiFolder2);
						if (first2 != ErrorCode.NoError)
						{
							return first2.Propagate((LID)48988U);
						}
						flag = true;
						if (array2 != null)
						{
							mapiFolder2.InternalSetOnePropShouldNotFail(operationContext, PropTag.Folder.InternalSourceKey, array2);
						}
					}
					else if (mapiFolder2.GetParentFid(operationContext) != exchangeId)
					{
						flag2 = true;
					}
					PCL pcl2 = pcl;
					bool flag3 = false;
					if (!flag)
					{
						PCL pcl3 = default(PCL);
						pcl3.LoadBinaryLXCN((byte[])mapiFolder2.GetOnePropValue(operationContext, PropTag.Folder.PCL));
						if (pcl3.IgnoreChange(pcl))
						{
							return ErrorCode.CreateSyncIgnore((LID)49237U);
						}
						if (!pcl.IgnoreChange(pcl3))
						{
							if (DateTime.Compare(dateTime, (DateTime)mapiFolder2.GetOnePropValue(operationContext, PropTag.Folder.LastModificationTime)) <= 0)
							{
								pcl3.Merge(pcl);
								mapiFolder2.InternalSetOnePropShouldNotFail(operationContext, PropTag.Folder.PCL, pcl3.DumpBinaryLXCN());
								mapiFolder2.StoreFolder.Save(operationContext);
								return ErrorCode.NoError;
							}
							flag3 = true;
						}
						pcl3.Merge(pcl);
						pcl2 = pcl3;
					}
					foreach (Property property in folderProps)
					{
						if (IcsHierarchyUploadContext.CanImportProperty(property))
						{
							properties.Add(property);
						}
					}
					List<MapiPropertyProblem> list = null;
					mapiFolder2.SetProps(operationContext, properties, ref list);
					mapiFolder2.InternalSetOnePropShouldNotFail(operationContext, PropTag.Folder.LastModificationTime, dateTime);
					mapiFolder2.InternalSetOnePropShouldNotFail(operationContext, PropTag.Folder.PCL, pcl2.DumpBinaryLXCN());
					mapiFolder2.StoreFolder.DirtyLastModificationTime(operationContext);
					mapiFolder2.InternalSetOnePropShouldNotFail(operationContext, PropTag.Folder.InternalChangeKey, array3);
					if (flag2)
					{
						mapiFolder2.Move(operationContext, mapiFolder, text, true);
					}
					else
					{
						mapiFolder2.StoreFolder.SetName(operationContext, text);
						mapiFolder2.StoreFolder.Save(operationContext);
					}
					if (!flag3)
					{
						ExchangeId lcnCurrent = mapiFolder2.StoreFolder.GetLcnCurrent(operationContext);
						base.IcsState.CnsetSeen.Insert(lcnCurrent);
					}
				}
				finally
				{
					if (mapiFolder2 != null)
					{
						mapiFolder2.Dispose();
					}
				}
			}
			if (array2 != null)
			{
				outputFolderId = exchangeId2;
			}
			return ErrorCode.NoError;
		}

		public override ErrorCode ImportDelete(MapiContext operationContext, byte[][] sourceKeys)
		{
			base.CheckDisposed();
			base.TraceInitialState(operationContext);
			foreach (byte[] array in sourceKeys)
			{
				ExchangeId fid = base.FolderIdFromSourceKey(operationContext, ref array);
				if (fid.IsValid)
				{
					using (MapiFolder mapiFolder = MapiFolder.OpenFolder(operationContext, base.Logon, fid))
					{
						if (mapiFolder != null)
						{
							using (DeleteFolderOperation deleteFolderOperation = new DeleteFolderOperation(mapiFolder, true, true, MapiFolder.ManageAssociatedDumpsterFolder(base.Logon, false), false))
							{
								deleteFolderOperation.DoAll(operationContext, false);
							}
						}
					}
				}
			}
			return ErrorCode.NoError;
		}

		internal static bool CanImportProperty(Property property)
		{
			return !property.Tag.IsCategory(3);
		}

		private ErrorCode ValidateFolderChangeParams(Properties hierarchyProps)
		{
			if (hierarchyProps.Count != IcsHierarchyUploadContext.changeProperties.Length)
			{
				return ErrorCode.CreateInvalidParameter((LID)51352U);
			}
			for (int i = 0; i < hierarchyProps.Count; i++)
			{
				if (hierarchyProps[i].Tag != IcsHierarchyUploadContext.changeProperties[i])
				{
					return ErrorCode.CreateInvalidParameter((LID)45208U);
				}
			}
			if (string.IsNullOrEmpty(hierarchyProps[5].Value as string))
			{
				return ErrorCode.CreateInvalidParameter((LID)61592U);
			}
			if (!IcsUploadContext.ValidChangeKey(hierarchyProps[3].Value as byte[]))
			{
				return ErrorCode.CreateInvalidParameter((LID)37016U);
			}
			byte[] array = hierarchyProps[0].Value as byte[];
			if (array == null || (array.Length != 0 && !IcsUploadContext.ValidSourceKey(array)))
			{
				return ErrorCode.CreateInvalidParameter((LID)53400U);
			}
			if (!IcsUploadContext.ValidSourceKey(hierarchyProps[1].Value as byte[]))
			{
				return ErrorCode.CreateInvalidParameter((LID)48725U);
			}
			return ErrorCode.NoError;
		}

		private static StorePropTag[] changeProperties = new StorePropTag[]
		{
			PropTag.Folder.ParentSourceKey,
			PropTag.Folder.SourceKey,
			PropTag.Folder.LastModificationTime,
			PropTag.Folder.ChangeKey,
			PropTag.Folder.PredecessorChangeList,
			PropTag.Folder.DisplayName
		};

		private enum ChangePropertyIndex
		{
			ParentSourceKey,
			SourceKey,
			LastModificationTime,
			ChangeKey,
			PredecessorChangeList,
			DisplayName
		}
	}
}
