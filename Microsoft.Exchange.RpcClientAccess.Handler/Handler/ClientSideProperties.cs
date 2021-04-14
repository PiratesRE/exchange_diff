using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ClientSideProperties : IEnumerable<PropertyTag>, IEnumerable
	{
		internal ClientSideProperties(PropertyTag[] clientSidePropertyTags, IEnumerable<PropertyId> excludeFromGetPropertyIds)
		{
			this.clientSidePropertyTags = new Dictionary<PropertyId, PropertyTag>(clientSidePropertyTags.Length, PropertyIdComparer.Instance);
			foreach (PropertyTag value in clientSidePropertyTags)
			{
				this.clientSidePropertyTags.Add(value.PropertyId, value);
			}
			this.excludeFromGetPropertyCallsSet = new HashSet<PropertyId>(PropertyIdComparer.Instance);
			foreach (PropertyId item in excludeFromGetPropertyIds)
			{
				this.excludeFromGetPropertyCallsSet.Add(item);
			}
		}

		internal static ClientSideProperties LogonInstance
		{
			get
			{
				return ClientSideProperties.instanceLogon;
			}
		}

		internal static ClientSideProperties MessageInstance
		{
			get
			{
				return ClientSideProperties.instanceMessage;
			}
		}

		internal static ClientSideProperties FolderInstance
		{
			get
			{
				return ClientSideProperties.instanceFolder;
			}
		}

		internal static ClientSideProperties AttachmentInstance
		{
			get
			{
				return ClientSideProperties.instanceAttachment;
			}
		}

		internal static ClientSideProperties ContentViewInstance
		{
			get
			{
				return ClientSideProperties.instanceContentView;
			}
		}

		internal static ClientSideProperties HierarchyViewInstance
		{
			get
			{
				return ClientSideProperties.instanceHierarchyView;
			}
		}

		internal static ClientSideProperties DeepHierarchyViewInstance
		{
			get
			{
				return ClientSideProperties.instanceDeepHierarchyView;
			}
		}

		internal static ClientSideProperties RecipientInstance
		{
			get
			{
				return ClientSideProperties.instanceRecipient;
			}
		}

		internal static ClientSideProperties EmptyInstance
		{
			get
			{
				return ClientSideProperties.instanceEmpty;
			}
		}

		IEnumerator<PropertyTag> IEnumerable<PropertyTag>.GetEnumerator()
		{
			return this.clientSidePropertyTags.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<PropertyTag>)this).GetEnumerator();
		}

		internal bool Contains(PropertyId propertyId)
		{
			return this.clientSidePropertyTags.ContainsKey(propertyId);
		}

		internal bool ExcludeFromGetPropertyList(PropertyId propertyId)
		{
			return this.clientSidePropertyTags.ContainsKey(propertyId) || this.excludeFromGetPropertyCallsSet.Contains(propertyId);
		}

		internal bool ShouldBeReturnedIfRequested(PropertyId propertyId)
		{
			return !this.Contains(propertyId) || propertyId == PropertyTag.Subject.PropertyId;
		}

		private static readonly ClientSideProperties instanceLogon = new ClientSideProperties(new PropertyTag[]
		{
			PropertyTag.StoreEntryId,
			PropertyTag.EntryId,
			PropertyTag.SearchKey,
			PropertyTag.RecordKey,
			PropertyTag.StoreRecordKey,
			PropertyTag.StoreSupportMask,
			PropertyTag.ObjectType,
			PropertyTag.ValidFolderMask,
			PropertyTag.AccessLevel,
			PropertyTag.CodePageId,
			PropertyTag.LocaleId,
			PropertyTag.SortLocaleId,
			PropertyTag.IPMSubtreeFolder,
			PropertyTag.IPMOutboxFolder,
			PropertyTag.IPMSentmailFolder,
			PropertyTag.IPMWastebasketFolder,
			PropertyTag.IPMFinderFolder,
			PropertyTag.IPMShortcutsFolder,
			PropertyTag.IPMViewsFolder,
			PropertyTag.IPMCommonViewsFolder,
			PropertyTag.IPMDafFolder,
			PropertyTag.NonIPMSubtreeFolder,
			PropertyTag.EformsRegistryFolder,
			PropertyTag.SplusFreeBusyFolder,
			PropertyTag.OfflineAddrBookFolder,
			PropertyTag.ArticleIndexFolder,
			PropertyTag.LocaleEformsRegistryFolder,
			PropertyTag.LocalSiteFreeBusyFolder,
			PropertyTag.LocalSiteAddrBookFolder,
			PropertyTag.MTSInFolder,
			PropertyTag.MTSOutFolder,
			PropertyTag.ScheduleFolder,
			PropertyTag.MdbProvider,
			PropertyTag.StoreState,
			PropertyTag.HierarchyServer,
			PropertyTag.LogonRightsOnMailbox
		}, Array<PropertyId>.Empty);

		private static readonly ClientSideProperties instanceMessage = new ClientSideProperties(new PropertyTag[]
		{
			PropertyTag.ObjectType,
			PropertyTag.StoreSupportMask,
			PropertyTag.MappingSignature,
			PropertyTag.MdbProvider,
			PropertyTag.StoreEntryId,
			PropertyTag.StoreRecordKey,
			PropertyTag.LongTermEntryIdFromTable,
			PropertyTag.EntryId,
			PropertyTag.RecordKey,
			PropertyTag.InstanceKey,
			PropertyTag.ParentEntryId,
			PropertyTag.ReplicaServer,
			PropertyTag.ReplicaVersion,
			PropertyTag.Subject
		}, new PropertyId[]
		{
			PropertyTag.NormalizedSubject.PropertyId,
			PropertyTag.SubjectPrefix.PropertyId,
			PropertyTag.ParentFid.PropertyId,
			PropertyTag.PreviewUnread.PropertyId
		});

		private static readonly ClientSideProperties instanceFolder = new ClientSideProperties(new PropertyTag[]
		{
			PropertyTag.AccessLevel,
			PropertyTag.ObjectType,
			PropertyTag.StoreSupportMask,
			PropertyTag.MappingSignature,
			PropertyTag.DisplayType,
			PropertyTag.EntryId,
			PropertyTag.RecordKey,
			PropertyTag.SearchKey,
			PropertyTag.MdbProvider,
			PropertyTag.ReplicaServer,
			PropertyTag.ReplicaVersion,
			PropertyTag.StoreEntryId,
			PropertyTag.StoreRecordKey,
			PropertyTag.OfflineFlags
		}, Array<PropertyId>.Empty);

		private static readonly ClientSideProperties instanceAttachment = new ClientSideProperties(new PropertyTag[]
		{
			PropertyTag.ObjectType,
			PropertyTag.MappingSignature,
			PropertyTag.InstanceKey,
			PropertyTag.SentMailEntryId,
			PropertyTag.DamOrgMsgEntryId,
			PropertyTag.RuleFolderEntryId
		}, Array<PropertyId>.Empty);

		private static readonly ClientSideProperties instanceContentView = new ClientSideProperties(new PropertyTag[]
		{
			PropertyTag.ObjectType,
			PropertyTag.StoreSupportMask,
			PropertyTag.MappingSignature,
			PropertyTag.MdbProvider,
			PropertyTag.StoreEntryId,
			PropertyTag.StoreRecordKey,
			PropertyTag.LongTermEntryIdFromTable,
			PropertyTag.EntryId,
			PropertyTag.RecordKey,
			PropertyTag.InstanceKey,
			PropertyTag.ParentEntryId,
			PropertyTag.ReplicaServer,
			PropertyTag.ReplicaVersion
		}, new PropertyId[]
		{
			PropertyTag.NormalizedSubject.PropertyId,
			PropertyTag.SubjectPrefix.PropertyId
		});

		private static readonly ClientSideProperties instanceHierarchyView = new ClientSideProperties(new PropertyTag[]
		{
			PropertyTag.ObjectType,
			PropertyTag.StoreSupportMask,
			PropertyTag.MappingSignature,
			PropertyTag.DisplayType,
			PropertyTag.EntryId,
			PropertyTag.RecordKey,
			PropertyTag.SearchKey,
			PropertyTag.MdbProvider,
			PropertyTag.StoreEntryId,
			PropertyTag.StoreRecordKey,
			PropertyTag.Depth,
			PropertyTag.LongTermEntryIdFromTable,
			PropertyTag.InstanceKey,
			PropertyTag.Access
		}, Array<PropertyId>.Empty);

		private static readonly ClientSideProperties instanceDeepHierarchyView = new ClientSideProperties(new PropertyTag[]
		{
			PropertyTag.ObjectType,
			PropertyTag.StoreSupportMask,
			PropertyTag.MappingSignature,
			PropertyTag.DisplayType,
			PropertyTag.EntryId,
			PropertyTag.RecordKey,
			PropertyTag.SearchKey,
			PropertyTag.MdbProvider,
			PropertyTag.StoreEntryId,
			PropertyTag.StoreRecordKey,
			PropertyTag.LongTermEntryIdFromTable,
			PropertyTag.InstanceKey,
			PropertyTag.Access
		}, Array<PropertyId>.Empty);

		private static readonly ClientSideProperties instanceRecipient = new ClientSideProperties(Array<PropertyTag>.Empty, Array<PropertyId>.Empty);

		private static readonly ClientSideProperties instanceEmpty = new ClientSideProperties(Array<PropertyTag>.Empty, Array<PropertyId>.Empty);

		private readonly Dictionary<PropertyId, PropertyTag> clientSidePropertyTags;

		private readonly HashSet<PropertyId> excludeFromGetPropertyCallsSet;
	}
}
