using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RPCPrimaryHierarchyHandler : IPrimaryHierarchyHandler
	{
		public RPCPrimaryHierarchyHandler(PublicFolderSession session, string userDn, ClientSecurityContext clientSecurityContext)
		{
			this.session = session;
			this.provider = new RPCPrimaryHierarchyProvider(session.OrganizationId, userDn, clientSecurityContext, new Func<ExchangePrincipal, LegacyDN>(Logon.CreatePersonalizedServerRedirectLegacyDN));
		}

		public StoreId CreateFolder(string folderName, string folderDescription, StoreId parentFolderId, CreateFolderFlags flags, out Guid contentMailboxGuid)
		{
			CreateMode mode = ((flags & CreateFolderFlags.OpenIfExists) == CreateFolderFlags.OpenIfExists) ? CreateMode.OpenIfExists : CreateMode.CreateNew;
			return this.provider.CreateFolder(folderName, folderDescription, parentFolderId, mode, out contentMailboxGuid);
		}

		public void DeleteFolder(StoreId parentFolderId, StoreId folderId, DeleteFolderFlags deleteFlags)
		{
			DeleteFolderFlags deleteFolderFlags = DeleteFolderFlags.None;
			if ((byte)(deleteFlags & DeleteFolderFlags.DeleteMessages) != 0)
			{
				deleteFolderFlags |= DeleteFolderFlags.DeleteMessages;
			}
			if ((byte)(deleteFlags & DeleteFolderFlags.DeleteFolders) != 0)
			{
				deleteFolderFlags |= DeleteFolderFlags.DelSubFolders;
			}
			if ((byte)(deleteFlags & DeleteFolderFlags.HardDelete) != 0)
			{
				deleteFolderFlags |= DeleteFolderFlags.ForceHardDelete;
			}
			this.provider.DeleteFolder(parentFolderId, folderId, deleteFolderFlags);
		}

		public void MoveFolder(StoreId parentFolderId, StoreId destinationFolderId, StoreId folderId, string newFolderName)
		{
			this.provider.MoveFolder(parentFolderId, destinationFolderId, folderId, newFolderName);
		}

		public PropertyProblem[] SetProperties(StoreId folderId, PropertyValue[] propertyValues, out Guid contentMailboxGuid)
		{
			contentMailboxGuid = Guid.Empty;
			PropertyTag[] propertyTags = (from propValue in propertyValues
			select propValue.PropertyTag).ToArray<PropertyTag>();
			List<ushort> unResolvedIndexes;
			PropTag[] array = this.ConvertToMapiTags(propertyTags, out unResolvedIndexes);
			List<PropValue> list = new List<PropValue>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != PropTag.Null)
				{
					object obj = propertyValues[i].Value;
					PropType propType = array[i].ValueType();
					if (propType != PropType.SysTime)
					{
						if (propType == PropType.SysTimeArray)
						{
							obj = ExDateTime.ToDateTimeArray((ExDateTime[])obj);
						}
					}
					else
					{
						obj = (DateTime)((ExDateTime)obj);
					}
					list.Add(new PropValue(array[i], obj));
				}
			}
			PropProblem[] propProblems = null;
			if (list.Count > 0)
			{
				propProblems = this.provider.SetProperties(folderId, list.ToArray(), out contentMailboxGuid);
			}
			return this.ConvertProblems(propProblems, unResolvedIndexes, propertyTags);
		}

		public PropertyProblem[] DeleteProperties(StoreId folderId, PropertyTag[] propertyTags, out Guid contentMailboxGuid)
		{
			contentMailboxGuid = Guid.Empty;
			List<ushort> unResolvedIndexes;
			PropTag[] array = this.ConvertToMapiTags(propertyTags, out unResolvedIndexes);
			array = (from tag in array
			where tag != PropTag.Null
			select tag).ToArray<PropTag>();
			PropProblem[] propProblems = null;
			if (array.Length > 0)
			{
				propProblems = this.provider.DeleteProperties(folderId, array, out contentMailboxGuid);
			}
			return this.ConvertProblems(propProblems, unResolvedIndexes, propertyTags);
		}

		public void ModifyPermissions(CoreFolder coreFolder, IModifyTable permissionsTable, IEnumerable<ModifyTableRow> modifyTableRows, ModifyTableOptions options, bool shouldReplaceAllRows)
		{
			using (PrimaryHierarchyAclModifyTable primaryHierarchyAclModifyTable = new PrimaryHierarchyAclModifyTable(this.provider, coreFolder, permissionsTable, options))
			{
				if (shouldReplaceAllRows)
				{
					primaryHierarchyAclModifyTable.Clear();
				}
				foreach (ModifyTableRow modifyTableRow in modifyTableRows)
				{
					PropValue[] propValues = Folder.ConvertPropertyValueToXSOPropValue(coreFolder, modifyTableRow.PropertyValues);
					switch (modifyTableRow.ModifyTableFlags)
					{
					case ModifyTableFlags.AddRow:
						primaryHierarchyAclModifyTable.AddRow(propValues);
						continue;
					case ModifyTableFlags.ModifyRow:
						primaryHierarchyAclModifyTable.ModifyRow(propValues);
						continue;
					case ModifyTableFlags.RemoveRow:
						primaryHierarchyAclModifyTable.RemoveRow(propValues);
						continue;
					}
					throw new RopExecutionException(string.Format("ModifyTableFlags is not valid. ModifyTableFlags = {0}.", modifyTableRow.ModifyTableFlags), (ErrorCode)2147942487U);
				}
				primaryHierarchyAclModifyTable.ApplyPendingChanges();
			}
		}

		private PropTag[] ConvertToMapiTags(PropertyTag[] propertyTags, out List<ushort> unResolvedIndexes)
		{
			PropTag[] array = new PropTag[propertyTags.Length];
			unResolvedIndexes = null;
			List<ushort> list = new List<ushort>();
			List<ushort> list2 = new List<ushort>();
			ushort num = 0;
			while ((int)num < propertyTags.Length)
			{
				if (propertyTags[(int)num].IsNamedProperty)
				{
					list.Add((ushort)propertyTags[(int)num].PropertyId);
					list2.Add(num);
				}
				else
				{
					array[(int)num] = (PropTag)propertyTags[(int)num];
				}
				num += 1;
			}
			if (list.Count > 0)
			{
				NamedProp[] namedPropsFromIds = NamedPropConverter.GetNamedPropsFromIds(this.session, this.session.Mailbox.MapiStore, list);
				List<NamedProp> list3 = new List<NamedProp>();
				int[] array2 = new int[list.Count];
				int i = 0;
				int num2 = 0;
				while (i < namedPropsFromIds.Length)
				{
					if (namedPropsFromIds[i] != null)
					{
						list3.Add(namedPropsFromIds[i]);
						array2[i] = num2++;
					}
					else
					{
						array2[i] = -1;
					}
					i++;
				}
				PropTag[] array3 = null;
				if (list3.Count > 0)
				{
					array3 = this.provider.GetIDsFromNames(true, list3);
				}
				for (int j = 0; j < namedPropsFromIds.Length; j++)
				{
					ushort num3 = list2[j];
					if (namedPropsFromIds[j] == null)
					{
						if (unResolvedIndexes == null)
						{
							unResolvedIndexes = new List<ushort>();
						}
						unResolvedIndexes.Add(num3);
						ExTraceGlobals.FolderTracer.TraceDebug<PropertyTag>(Activity.TraceId, "Missing named property definition for property tag {0}", propertyTags[(int)num3]);
					}
					else
					{
						array[(int)num3] = array3[array2[j]].ChangePropType((PropType)propertyTags[(int)num3].PropertyType);
					}
				}
			}
			return array;
		}

		private PropertyProblem[] ConvertProblems(PropProblem[] propProblems, List<ushort> unResolvedIndexes, PropertyTag[] propertyTags)
		{
			PropertyProblem[] array = Array<PropertyProblem>.Empty;
			if (propProblems != null && propProblems.Length > 0)
			{
				array = new PropertyProblem[propProblems.Length];
				int num = 0;
				for (int i = 0; i < propProblems.Length; i++)
				{
					while (unResolvedIndexes != null && num < unResolvedIndexes.Count && (int)unResolvedIndexes[num] <= propProblems[i].Index + num)
					{
						num++;
					}
					string text;
					ErrorCode errorCode = MEDSPropertyTranslator.PropertyErrorCodeToErrorCode(MapiPropertyHelper.MapiErrorToXsoError(propProblems[i].Scode, out text));
					array[i] = new PropertyProblem((ushort)(propProblems[i].Index + num), propertyTags[propProblems[i].Index + num], errorCode);
				}
			}
			return array;
		}

		private static readonly PropValue EmptyPropValue = new PropValue(PropTag.Null, null);

		private RPCPrimaryHierarchyProvider provider;

		private PublicFolderSession session;
	}
}
