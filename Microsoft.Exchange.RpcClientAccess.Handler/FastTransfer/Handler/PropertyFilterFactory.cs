using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyFilterFactory : IPropertyFilterFactory
	{
		public PropertyFilterFactory(bool isShallowCopy, bool isInclusion, PropertyTag[] propertyTags)
		{
			this.isShallowCopy = isShallowCopy;
			this.isInclusion = isInclusion;
			this.propertyTags = PropertyFilterFactory.ConvertPropertyTagsToUnicode(propertyTags);
		}

		private static PropertyTag[] ConvertPropertyTagsToUnicode(PropertyTag[] propertyTags)
		{
			bool flag = false;
			foreach (PropertyTag propertyTag in propertyTags)
			{
				if (propertyTag.ElementPropertyType == PropertyType.String8)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				PropertyTag[] array = new PropertyTag[propertyTags.Length];
				Array.Copy(propertyTags, array, propertyTags.Length);
				for (int j = 0; j < array.Length; j++)
				{
					if (array[j].ElementPropertyType == PropertyType.String8)
					{
						array[j] = array[j].ChangeElementPropertyType(PropertyType.Unicode);
					}
				}
				return array;
			}
			return propertyTags;
		}

		private static HashSet<PropertyTag> Combine(IEnumerable<PropertyTag> set1, IEnumerable<PropertyTag> set2)
		{
			HashSet<PropertyTag> hashSet = new HashSet<PropertyTag>(set1);
			hashSet.UnionWith(set2);
			return hashSet;
		}

		private IPropertyFilter EnsureCachedPropertyFilterCreated(ref IPropertyFilter filter, bool isTopLevel, HashSet<PropertyTag> staticIncludeProperties, HashSet<PropertyTag> staticExcludeProperties)
		{
			if (filter == null)
			{
				filter = new PropertyFilterFactory.PropertyFilter(isTopLevel && this.isInclusion, isTopLevel ? this.propertyTags : Array<PropertyTag>.Empty, staticIncludeProperties, staticExcludeProperties);
			}
			return filter;
		}

		private IMessagePropertyFilter EnsureCachedMessagePropertyFilterCreated(ref IMessagePropertyFilter filter, bool isTopLevel, HashSet<PropertyTag> staticIncludeProperties, HashSet<PropertyTag> staticExcludeProperties)
		{
			if (filter == null)
			{
				filter = new PropertyFilterFactory.MessagePropertyFilter(this.isShallowCopy, isTopLevel && this.isInclusion, isTopLevel ? this.propertyTags : Array<PropertyTag>.Empty, staticIncludeProperties, staticExcludeProperties);
			}
			return filter;
		}

		public IPropertyFilter GetAttachmentCopyToFilter(bool isTopLevel)
		{
			if (isTopLevel)
			{
				return this.EnsureCachedPropertyFilterCreated(ref this.attachmentCopyToFilterTopLevel, true, PropertyFilterFactory.IncludePropertiesForFxAttachmentCopyToCollection, PropertyFilterFactory.ExcludePropertiesForFxAttachmentCopyToCollection);
			}
			return this.EnsureCachedPropertyFilterCreated(ref this.attachmentCopyToFilter, false, PropertyFilterFactory.IncludePropertiesForFxAttachmentCopyToCollection, PropertyFilterFactory.ExcludePropertiesForFxAttachmentCopyToCollection);
		}

		public IMessagePropertyFilter GetMessageCopyToFilter(bool isTopLevel)
		{
			if (isTopLevel)
			{
				return this.EnsureCachedMessagePropertyFilterCreated(ref this.messageCopyToFilterTopLevel, true, PropertyFilterFactory.IncludePropertiesForFxMessageCopyToCollection, PropertyFilterFactory.ExcludePropertiesForFxMessageCopyToCollection);
			}
			return this.EnsureCachedMessagePropertyFilterCreated(ref this.messageCopyToFilter, false, PropertyFilterFactory.IncludePropertiesForFxMessageCopyToCollection, PropertyFilterFactory.ExcludePropertiesForFxMessageCopyToCollection);
		}

		public IPropertyFilter GetFolderCopyToFilter()
		{
			return this.EnsureCachedPropertyFilterCreated(ref this.folderCopyToFilter, true, PropertyFilterFactory.IncludePropertiesForFxFolderCopyToCollection, PropertyFilterFactory.ExcludePropertiesForFxFolderCopyToCollection);
		}

		public IPropertyFilter GetCopyFolderFilter()
		{
			return this.EnsureCachedPropertyFilterCreated(ref this.copyFolderFilter, true, PropertyFilterFactory.IncludePropertiesForFxCopyFolderCollection, PropertyFilterFactory.ExcludePropertiesForFxCopyFolderCollection);
		}

		public IPropertyFilter GetCopySubfolderFilter()
		{
			return this.EnsureCachedPropertyFilterCreated(ref this.subfolderFilter, false, PropertyFilterFactory.IncludePropertiesForFxSubfolderCopyCollection, PropertyFilterFactory.ExcludePropertiesForFxSubfolderCopyCollection);
		}

		public IPropertyFilter GetAttachmentFilter(bool isTopLevel)
		{
			return PropertyFilterFactory.AttachmentDownloadPropertyFilter;
		}

		public IPropertyFilter GetEmbeddedMessageFilter(bool isTopLevel)
		{
			return PropertyFilterFactory.AttachmentEmbeddedMessagePropertyFilter;
		}

		public IPropertyFilter GetMessageFilter(bool isTopLevel)
		{
			return this.EnsureCachedPropertyFilterCreated(ref this.messageDownloadFilter, isTopLevel, PropertyFilterFactory.IncludePropertiesForFxMessageDownloadCollection, PropertyFilterFactory.ExcludePropertiesForFxMessageDownloadCollection);
		}

		public IPropertyFilter GetRecipientFilter()
		{
			return PropertyFilterFactory.RecipientPropertyFilter;
		}

		public IPropertyFilter GetIcsHierarchyFilter(bool includeFid, bool includeChangeNumber)
		{
			if (includeFid && includeChangeNumber)
			{
				return IncludeAllPropertyFilter.Instance;
			}
			if (includeFid && !includeChangeNumber)
			{
				return PropertyFilterFactory.IcsHierarchyPropertyFilterOnlyChangeNumber;
			}
			if (!includeFid && includeChangeNumber)
			{
				return PropertyFilterFactory.IcsHierarchyPropertyFilterOnlyFid;
			}
			return PropertyFilterFactory.IcsHierarchyPropertyFilter;
		}

		internal static readonly PropertyTag[] CommonIncludePropertiesForFxCopyTo = new PropertyTag[]
		{
			PropertyTag.MessageFlags,
			PropertyTag.OriginalDisplayBcc,
			PropertyTag.OriginalDisplayCc,
			PropertyTag.OriginalDisplayTo,
			PropertyTag.MessageDeliveryTime,
			PropertyTag.NormalizedSubject
		};

		internal static readonly PropertyTag[] CommonExcludePropertiesForFxCopyTo = new PropertyTag[]
		{
			PropertyTag.Body,
			PropertyTag.RtfSyncBodyCRC,
			PropertyTag.RtfSyncBodyCount,
			PropertyTag.RtfSyncBodyTag,
			PropertyTag.RtfSyncPrefixCount,
			PropertyTag.RtfSyncTrailingCount
		};

		internal static readonly PropertyTag[] IncludePropertiesForFxAttachmentCopyTo = new PropertyTag[0];

		internal static readonly PropertyTag[] ExcludePropertiesForFxAttachmentCopyTo = new PropertyTag[0];

		internal static readonly PropertyTag[] IncludePropertiesForFxMessageCopyTo = new PropertyTag[]
		{
			PropertyTag.Body,
			PropertyTag.SentMailServerId
		};

		internal static readonly PropertyTag[] ExcludePropertiesForFxMessageCommon = new PropertyTag[]
		{
			PropertyTag.ObjectType,
			PropertyTag.EntryId,
			PropertyTag.RecordKey,
			PropertyTag.StoreEntryId,
			PropertyTag.StoreRecordKey,
			PropertyTag.ParentEntryId,
			PropertyTag.DisplayBcc,
			PropertyTag.DisplayCc,
			PropertyTag.DisplayTo,
			PropertyTag.HasAttach,
			PropertyTag.Access,
			PropertyTag.Mid,
			PropertyTag.Associated,
			PropertyTag.MessageSize,
			PropertyTag.MimeSize,
			PropertyTag.FileSize,
			PropertyTag.InternetNewsGroups,
			PropertyTag.ImapCachedBodystructure,
			PropertyTag.ImapCachedEnvelope,
			PropertyTag.LocalCommitTime,
			PropertyTag.AutoReset,
			PropertyTag.DeletedOn,
			PropertyTag.SMTPTempTblData,
			PropertyTag.SMTPTempTblData2,
			PropertyTag.SMTPTempTblData3,
			PropertyTag.MimeSkeleton
		};

		internal static readonly PropertyTag[] IncludePropertiesForFxFolderCopy = Array<PropertyTag>.Empty;

		internal static readonly PropertyTag[] ExcludePropertiesForFxFolderCopy = Array<PropertyTag>.Empty;

		internal static readonly PropertyTag[] IncludePropertiesForFxSubFolderCopy = Array<PropertyTag>.Empty;

		internal static readonly PropertyTag[] ExcludePropertiesForFxSubFolderCopy = new PropertyTag[]
		{
			PropertyTag.Comment,
			PropertyTag.DisplayName
		};

		internal static readonly PropertyTag[] IncludePropertiesForFxCopyFolder = new PropertyTag[]
		{
			PropertyTag.PublicFolderPlatinumHomeMDB,
			PropertyTag.PublicFolderProxyRequired,
			PropertyTag.FolderChildCount,
			PropertyTag.Rights,
			PropertyTag.AddressBookEntryId,
			PropertyTag.DisablePeruserRead,
			PropertyTag.SecureInSite
		};

		internal static readonly PropertyTag[] ExcludePropertiesForFxCopyFolder = new PropertyTag[]
		{
			PropertyTag.ObjectType,
			PropertyTag.EntryId,
			PropertyTag.RecordKey,
			PropertyTag.ContentCount,
			PropertyTag.ContentUnread,
			PropertyTag.StoreEntryId,
			PropertyTag.StoreRecordKey,
			PropertyTag.FolderType,
			PropertyTag.ParentEntryId,
			PropertyTag.IsNewsgroupAnchor,
			PropertyTag.IsNewsgroup,
			PropertyTag.NewsgroupComp,
			PropertyTag.InetNewsgroupName,
			PropertyTag.NewsfeedAcl,
			PropertyTag.Fid,
			PropertyTag.ParentFid,
			PropertyTag.DisplayName,
			PropertyTag.DeletedMsgCt,
			PropertyTag.DeletedAssocMsgCt,
			PropertyTag.DeletedFolderCt,
			PropertyTag.DeletedMessageSizeExtended,
			PropertyTag.DeletedAssocMessageSizeExtended,
			PropertyTag.DeletedNormalMessageSizeExtended,
			PropertyTag.DeletedOn,
			PropertyTag.LocalCommitTime,
			PropertyTag.LocalCommitTimeMax,
			PropertyTag.DeletedCountTotal,
			PropertyTag.ICSChangeKey,
			PropertyTag.URLName,
			PropertyTag.HierRev,
			PropertyTag.CreationTime,
			PropertyTag.LastModificationTime,
			PropertyTag.Subfolders,
			PropertyTag.SourceKey,
			PropertyTag.ParentSourceKey,
			PropertyTag.ChangeKey,
			PropertyTag.PredecessorChangeList
		};

		internal static readonly PropertyTag[] IncludePropertiesForFxFolderCopyTo = Array<PropertyTag>.Empty;

		internal static readonly PropertyTag[] ExcludePropertiesForFxFolderCopyTo = Array<PropertyTag>.Empty;

		internal static readonly PropertyTag[] ExcludePropertiesForAttachmentDownload = new PropertyTag[]
		{
			PropertyTag.AttachmentNumber,
			PropertyTag.ObjectType
		};

		internal static readonly PropertyTag[] ExcludePropertiesForAttachmentEmbeddedMessage = new PropertyTag[]
		{
			PropertyTag.AttachmentNumber,
			PropertyTag.ObjectType,
			PropertyTag.AttachmentDataObject
		};

		internal static readonly PropertyTag[] ExcludePropertiesRecipient = new PropertyTag[]
		{
			PropertyTag.RowId
		};

		internal static readonly PropertyTag[] ExcludePropertiesIcsHierarchy = new PropertyTag[]
		{
			PropertyTag.Fid,
			PropertyTag.ChangeNumber
		};

		internal static readonly PropertyTag[] ExcludeOnlyFidIcsHierarchy = new PropertyTag[]
		{
			PropertyTag.Fid
		};

		internal static readonly PropertyTag[] ExcludeOnlyChangeNumberIcsHierarchy = new PropertyTag[]
		{
			PropertyTag.ChangeNumber
		};

		private static readonly HashSet<PropertyTag> IncludePropertiesForFxAttachmentCopyToCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.CommonIncludePropertiesForFxCopyTo, PropertyFilterFactory.IncludePropertiesForFxAttachmentCopyTo);

		private static readonly HashSet<PropertyTag> ExcludePropertiesForFxAttachmentCopyToCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.CommonExcludePropertiesForFxCopyTo, PropertyFilterFactory.ExcludePropertiesForFxAttachmentCopyTo);

		private static readonly HashSet<PropertyTag> IncludePropertiesForFxMessageCopyToCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.CommonIncludePropertiesForFxCopyTo, PropertyFilterFactory.IncludePropertiesForFxMessageCopyTo);

		private static readonly HashSet<PropertyTag> ExcludePropertiesForFxMessageCopyToCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.CommonExcludePropertiesForFxCopyTo, PropertyFilterFactory.ExcludePropertiesForFxMessageCommon);

		private static readonly HashSet<PropertyTag> IncludePropertiesForFxMessageDownloadCollection = new HashSet<PropertyTag>();

		private static readonly HashSet<PropertyTag> ExcludePropertiesForFxMessageDownloadCollection = new HashSet<PropertyTag>(PropertyFilterFactory.ExcludePropertiesForFxMessageCommon);

		private static readonly HashSet<PropertyTag> IncludePropertiesForFxFolderCopyCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.CommonIncludePropertiesForFxCopyTo, PropertyFilterFactory.IncludePropertiesForFxFolderCopy);

		private static readonly HashSet<PropertyTag> ExcludePropertiesForFxFolderCopyCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.CommonExcludePropertiesForFxCopyTo, PropertyFilterFactory.ExcludePropertiesForFxFolderCopy);

		private static readonly HashSet<PropertyTag> IncludePropertiesForFxSubfolderCopyCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.IncludePropertiesForFxFolderCopyCollection, PropertyFilterFactory.IncludePropertiesForFxSubFolderCopy);

		private static readonly HashSet<PropertyTag> ExcludePropertiesForFxSubfolderCopyCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.ExcludePropertiesForFxFolderCopyCollection, PropertyFilterFactory.ExcludePropertiesForFxSubFolderCopy);

		private static readonly HashSet<PropertyTag> IncludePropertiesForFxFolderCopyToCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.IncludePropertiesForFxFolderCopyCollection, PropertyFilterFactory.IncludePropertiesForFxFolderCopyTo);

		private static readonly HashSet<PropertyTag> ExcludePropertiesForFxFolderCopyToCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.ExcludePropertiesForFxFolderCopyCollection, PropertyFilterFactory.ExcludePropertiesForFxFolderCopyTo);

		private static readonly HashSet<PropertyTag> IncludePropertiesForFxCopyFolderCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.IncludePropertiesForFxFolderCopyCollection, PropertyFilterFactory.IncludePropertiesForFxCopyFolder);

		private static readonly HashSet<PropertyTag> ExcludePropertiesForFxCopyFolderCollection = PropertyFilterFactory.Combine(PropertyFilterFactory.ExcludePropertiesForFxFolderCopyCollection, PropertyFilterFactory.ExcludePropertiesForFxCopyFolder);

		private static readonly IPropertyFilter AttachmentDownloadPropertyFilter = new ExcludingPropertyFilter(PropertyFilterFactory.ExcludePropertiesForAttachmentDownload);

		private static readonly IPropertyFilter AttachmentEmbeddedMessagePropertyFilter = new ExcludingPropertyFilter(PropertyFilterFactory.ExcludePropertiesForAttachmentEmbeddedMessage);

		private static readonly IPropertyFilter RecipientPropertyFilter = new ExcludingPropertyFilter(PropertyFilterFactory.ExcludePropertiesRecipient);

		private static readonly IPropertyFilter IcsHierarchyPropertyFilter = new ExcludingPropertyFilter(PropertyFilterFactory.ExcludePropertiesIcsHierarchy);

		private static readonly IPropertyFilter IcsHierarchyPropertyFilterOnlyFid = new ExcludingPropertyFilter(PropertyFilterFactory.ExcludeOnlyFidIcsHierarchy);

		private static readonly IPropertyFilter IcsHierarchyPropertyFilterOnlyChangeNumber = new ExcludingPropertyFilter(PropertyFilterFactory.ExcludeOnlyChangeNumberIcsHierarchy);

		public static readonly PropertyFilterFactory IncludeAllFactory = new PropertyFilterFactory(false, false, Array<PropertyTag>.Empty);

		private readonly bool isShallowCopy;

		private readonly bool isInclusion;

		private readonly PropertyTag[] propertyTags;

		private IPropertyFilter attachmentCopyToFilterTopLevel;

		private IPropertyFilter attachmentCopyToFilter;

		private IMessagePropertyFilter messageCopyToFilterTopLevel;

		private IMessagePropertyFilter messageCopyToFilter;

		private IPropertyFilter folderCopyToFilter;

		private IPropertyFilter copyFolderFilter;

		private IPropertyFilter subfolderFilter;

		private IPropertyFilter messageDownloadFilter;

		[ClassAccessLevel(AccessLevel.Implementation)]
		private class PropertyFilter : IPropertyFilter
		{
			public PropertyFilter(bool isInclusion, ICollection<PropertyTag> clientPropertyTags, ICollection<PropertyTag> staticIncludedPropertyTags, ICollection<PropertyTag> staticExcludedPropertyTags)
			{
				this.isInclusion = isInclusion;
				this.clientPropertyTags = clientPropertyTags;
				this.staticIncludedPropertyTags = staticIncludedPropertyTags;
				this.staticExcludedPropertyTags = staticExcludedPropertyTags;
				List<PropertyId> list = null;
				foreach (PropertyTag propertyTag in clientPropertyTags)
				{
					if (propertyTag.PropertyType == PropertyType.Unspecified)
					{
						if (list == null)
						{
							list = new List<PropertyId>();
						}
						list.Add(propertyTag.PropertyId);
					}
				}
				if (list != null)
				{
					this.clientPropertyIds = list;
					return;
				}
				this.clientPropertyIds = Array<PropertyId>.Empty;
			}

			protected bool IsInclusionList
			{
				get
				{
					return this.isInclusion;
				}
			}

			protected ICollection<PropertyTag> ClientProperties
			{
				get
				{
					return this.clientPropertyTags;
				}
			}

			public bool IncludeProperty(PropertyTag propertyTag)
			{
				if (this.isInclusion)
				{
					if (!this.clientPropertyTags.Contains(propertyTag) && !this.clientPropertyIds.Contains(propertyTag.PropertyId))
					{
						return false;
					}
					if (this.staticIncludedPropertyTags.Contains(propertyTag))
					{
						return true;
					}
				}
				else
				{
					if (this.clientPropertyTags.Contains(propertyTag) || this.clientPropertyIds.Contains(propertyTag.PropertyId))
					{
						return false;
					}
					if (propertyTag.IsStringProperty)
					{
						propertyTag = propertyTag.ChangeElementPropertyType(PropertyType.Unicode);
					}
					if (this.staticIncludedPropertyTags.Contains(propertyTag))
					{
						return true;
					}
					if (this.staticExcludedPropertyTags.Contains(propertyTag))
					{
						return false;
					}
				}
				return !propertyTag.IsProviderDefinedNonTransmittable;
			}

			private readonly bool isInclusion;

			private readonly ICollection<PropertyTag> clientPropertyTags;

			private readonly ICollection<PropertyId> clientPropertyIds;

			private readonly ICollection<PropertyTag> staticIncludedPropertyTags;

			private readonly ICollection<PropertyTag> staticExcludedPropertyTags;
		}

		[ClassAccessLevel(AccessLevel.Implementation)]
		private class MessagePropertyFilter : PropertyFilterFactory.PropertyFilter, IMessagePropertyFilter, IPropertyFilter
		{
			public MessagePropertyFilter(bool isShallowCopy, bool isInclusion, ICollection<PropertyTag> clientPropertyTags, ICollection<PropertyTag> staticIncludedPropertyTags, ICollection<PropertyTag> staticExcludedPropertyTags) : base(isInclusion, clientPropertyTags, staticIncludedPropertyTags, staticExcludedPropertyTags)
			{
				this.isShallowCopy = isShallowCopy;
			}

			public bool IncludeRecipients
			{
				get
				{
					if (!base.IsInclusionList)
					{
						return !base.ClientProperties.Contains(PropertyTag.MessageRecipients);
					}
					return !this.isShallowCopy && base.ClientProperties.Contains(PropertyTag.MessageRecipients);
				}
			}

			public bool IncludeAttachments
			{
				get
				{
					if (!base.IsInclusionList)
					{
						return !base.ClientProperties.Contains(PropertyTag.MessageAttachments);
					}
					return !this.isShallowCopy && base.ClientProperties.Contains(PropertyTag.MessageAttachments);
				}
			}

			private readonly bool isShallowCopy;
		}
	}
}
