using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal struct PropertyTag : IEquatable<PropertyTag>, IEquatable<string>, IComparable<PropertyTag>
	{
		public PropertyTag(uint propertyTag)
		{
			this.propertyTag = propertyTag;
		}

		public PropertyTag(PropertyId propertyId, PropertyType propertyType)
		{
			this.propertyTag = (uint)((uint)propertyId << 16 | (PropertyId)propertyType);
		}

		public PropertyId PropertyId
		{
			get
			{
				return (PropertyId)((this.propertyTag & 4294901760U) >> 16);
			}
		}

		public PropertyType PropertyType
		{
			get
			{
				return (PropertyType)(this.propertyTag & 57343U);
			}
		}

		public int? EstimatedValueSize
		{
			get
			{
				PropertyType propertyType = this.PropertyType;
				if (propertyType <= PropertyType.SysTime)
				{
					switch (propertyType)
					{
					case PropertyType.Int16:
						return new int?(2);
					case PropertyType.Int32:
						return new int?(4);
					case PropertyType.Float:
						return new int?(4);
					case PropertyType.Double:
						return new int?(8);
					case PropertyType.Currency:
						return new int?(8);
					case PropertyType.AppTime:
						return new int?(8);
					case (PropertyType)8:
					case (PropertyType)9:
					case (PropertyType)12:
					case (PropertyType)14:
					case (PropertyType)15:
					case (PropertyType)16:
					case (PropertyType)17:
					case (PropertyType)18:
					case (PropertyType)19:
						goto IL_E4;
					case PropertyType.Error:
						return new int?(4);
					case PropertyType.Bool:
						return new int?(2);
					case PropertyType.Object:
						break;
					case PropertyType.Int64:
						return new int?(8);
					default:
						switch (propertyType)
						{
						case PropertyType.String8:
						case PropertyType.Unicode:
							break;
						default:
							if (propertyType != PropertyType.SysTime)
							{
								goto IL_E4;
							}
							return new int?(8);
						}
						break;
					}
				}
				else
				{
					if (propertyType == PropertyType.Guid)
					{
						return new int?(16);
					}
					if (propertyType != PropertyType.ServerId && propertyType != PropertyType.Binary)
					{
						goto IL_E4;
					}
				}
				return new int?(0);
				IL_E4:
				return null;
			}
		}

		public bool IsMultiValuedProperty
		{
			get
			{
				return (this.PropertyType & (PropertyType)4096) != PropertyType.Unspecified;
			}
		}

		public bool IsMultiValueInstanceProperty
		{
			get
			{
				return ((ushort)this.propertyTag & 8192) != 0;
			}
		}

		public bool IsStringProperty
		{
			get
			{
				return PropertyTag.IsStringPropertyType(this.PropertyType);
			}
		}

		public PropertyType ElementPropertyType
		{
			get
			{
				return this.PropertyType & (PropertyType)53247;
			}
		}

		public bool IsNamedProperty
		{
			get
			{
				return PropertyTag.IsNamedPropertyId(this.PropertyId);
			}
		}

		public bool IsMarker
		{
			get
			{
				return PropertyTag.MarkerPropertyTags.Contains(this);
			}
		}

		public bool IsMetaProperty
		{
			get
			{
				return PropertyTag.MetaPropertyTags.Contains(this);
			}
		}

		public bool IsProviderDefinedNonTransmittable
		{
			get
			{
				ushort propertyId = (ushort)this.PropertyId;
				return propertyId >= 26112 && propertyId <= 26623;
			}
		}

		public static implicit operator uint(PropertyTag propertyTag)
		{
			return propertyTag.propertyTag;
		}

		public static PropertyTag CreateError(PropertyId propertyId)
		{
			return new PropertyTag(propertyId, PropertyType.Error);
		}

		public static bool operator ==(PropertyTag a, PropertyTag b)
		{
			return a.propertyTag == b.propertyTag;
		}

		public static bool operator !=(PropertyTag a, PropertyTag b)
		{
			return a.propertyTag != b.propertyTag;
		}

		public static bool HasCompatiblePropertyType(PropertyTag propTag1, PropertyTag propTag2)
		{
			return propTag1.ElementPropertyType == propTag2.ElementPropertyType || (PropertyTag.IsStringPropertyType(propTag1.ElementPropertyType) && PropertyTag.IsStringPropertyType(propTag2.ElementPropertyType));
		}

		public static PropertyTag RemoveMviWithMvIfNeeded(PropertyTag propertyTag)
		{
			if (propertyTag.IsMultiValueInstanceProperty)
			{
				return new PropertyTag(propertyTag.PropertyId, propertyTag.ElementPropertyType);
			}
			return propertyTag;
		}

		public static bool IsStringPropertyType(PropertyType propertyType)
		{
			return propertyType == PropertyType.String8 || propertyType == PropertyType.Unicode;
		}

		public bool Equals(PropertyTag other)
		{
			return this.propertyTag == other.propertyTag;
		}

		public bool Equals(string other)
		{
			return other != null && StringComparer.OrdinalIgnoreCase.Equals(this.ToString(), other);
		}

		public int CompareTo(PropertyTag other)
		{
			return this.propertyTag.CompareTo(other.propertyTag);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PropertyTag))
			{
				return this.Equals(obj as string);
			}
			return this.Equals((PropertyTag)obj);
		}

		public override int GetHashCode()
		{
			return this.propertyTag.GetHashCode();
		}

		public PropertyTag ChangeElementPropertyType(PropertyType newPropertyType)
		{
			PropertyTag propertyTag = new PropertyTag(this.PropertyId, (newPropertyType & (PropertyType)53247) | checked((PropertyType)(this.propertyTag & 12288U)));
			if (PropertyTag.HasCompatiblePropertyType(this, propertyTag))
			{
				return propertyTag;
			}
			throw new InvalidOperationException(string.Format("The new requested element property type {0} is incompatible with the current one {1} for the PropertyTag {2}", newPropertyType, this.ElementPropertyType, this));
		}

		public override string ToString()
		{
			return this.propertyTag.ToString("X8");
		}

		internal static bool IsNamedPropertyId(PropertyId propertyId)
		{
			return propertyId >= (PropertyId)32768;
		}

		internal static PropertyType FromClrType(Type type)
		{
			if (typeof(short) == type)
			{
				return PropertyType.Int16;
			}
			if (typeof(int) == type)
			{
				return PropertyType.Int32;
			}
			if (typeof(long) == type)
			{
				return PropertyType.Int64;
			}
			if (typeof(float) == type)
			{
				return PropertyType.Float;
			}
			if (typeof(double) == type)
			{
				return PropertyType.Double;
			}
			if (typeof(ExDateTime) == type)
			{
				return PropertyType.SysTime;
			}
			if (typeof(bool) == type)
			{
				return PropertyType.Bool;
			}
			if (typeof(string) == type)
			{
				return PropertyType.Unicode;
			}
			if (typeof(Guid) == type)
			{
				return PropertyType.Guid;
			}
			if (typeof(byte[]) == type)
			{
				return PropertyType.Binary;
			}
			if (typeof(string[]) == type)
			{
				return PropertyType.MultiValueUnicode;
			}
			if (typeof(byte[][]) == type)
			{
				return PropertyType.MultiValueBinary;
			}
			if (typeof(short[]) == type)
			{
				return PropertyType.MultiValueInt16;
			}
			if (typeof(int[]) == type)
			{
				return PropertyType.MultiValueInt32;
			}
			if (typeof(float[]) == type)
			{
				return PropertyType.MultiValueFloat;
			}
			if (typeof(double[]) == type)
			{
				return PropertyType.MultiValueDouble;
			}
			if (typeof(long[]) == type)
			{
				return PropertyType.MultiValueInt64;
			}
			if (typeof(ExDateTime[]) == type)
			{
				return PropertyType.MultiValueSysTime;
			}
			if (typeof(Guid[]) == type)
			{
				return PropertyType.MultiValueGuid;
			}
			throw new ArgumentException(string.Format("Invalid property type {0}", type));
		}

		public const int Size = 4;

		private const uint PropertyIdMask = 4294901760U;

		private const uint PropertyTypeMask = 57343U;

		private const ushort ElementPropertyTypeMask = 53247;

		public static readonly IEqualityComparer<PropertyTag> PropertyIdComparer = new PropertyTag.PropertyIdComparerImpl();

		private readonly uint propertyTag;

		internal static readonly PropertyTag StartTopFld = new PropertyTag(1074331651U);

		internal static readonly PropertyTag StartSubFld = new PropertyTag(1074397187U);

		internal static readonly PropertyTag EndFolder = new PropertyTag(1074462723U);

		internal static readonly PropertyTag StartMessage = new PropertyTag(1074528259U);

		internal static readonly PropertyTag StartFAIMsg = new PropertyTag(1074790403U);

		internal static readonly PropertyTag EndMessage = new PropertyTag(1074593795U);

		internal static readonly PropertyTag StartEmbed = new PropertyTag(1073807363U);

		internal static readonly PropertyTag EndEmbed = new PropertyTag(1073872899U);

		internal static readonly PropertyTag StartRecip = new PropertyTag(1073938435U);

		internal static readonly PropertyTag EndRecip = new PropertyTag(1074003971U);

		internal static readonly PropertyTag NewAttach = new PropertyTag(1073741827U);

		internal static readonly PropertyTag EndAttach = new PropertyTag(1074659331U);

		internal static readonly PropertyTag EcWarning = new PropertyTag(1074724867U);

		private static readonly PropertyTag FXErrorInfo = new PropertyTag(1075314691U);

		internal static readonly PropertyTag FXDelProp = new PropertyTag(1075183619U);

		internal static readonly PropertyTag IncrSyncChg = new PropertyTag(1074921475U);

		internal static readonly PropertyTag IncrSyncChgPartial = new PropertyTag(1081933827U);

		internal static readonly PropertyTag IncrSyncDel = new PropertyTag(1074987011U);

		internal static readonly PropertyTag IncrSyncEnd = new PropertyTag(1075052547U);

		internal static readonly PropertyTag IncrSyncRead = new PropertyTag(1076822019U);

		internal static readonly PropertyTag IncrSyncStateBegin = new PropertyTag(1077542915U);

		internal static readonly PropertyTag IncrSyncStateEnd = new PropertyTag(1077608451U);

		internal static readonly PropertyTag IncrSyncProgressMode = new PropertyTag(1081344011U);

		internal static readonly PropertyTag IncrSyncProgressPerMsg = new PropertyTag(1081409547U);

		internal static readonly PropertyTag IncrSyncGroupInfo = new PropertyTag(1081803010U);

		internal static readonly PropertyTag IncrSyncGroupId = new PropertyTag(1081868291U);

		internal static readonly PropertyTag IncrSyncMsg = new PropertyTag(1075118083U);

		internal static readonly PropertyTag IncrSyncMsgPartial = new PropertyTag(1081737219U);

		internal static readonly PropertyTag MessageAttachments = new PropertyTag(236126221U);

		internal static readonly PropertyTag MessageRecipients = new PropertyTag(236060685U);

		internal static readonly PropertyTag ContainerHierarchy = new PropertyTag(906887181U);

		internal static readonly PropertyTag ContainerContents = new PropertyTag(906952717U);

		internal static readonly PropertyTag FolderAssociatedContents = new PropertyTag(907018253U);

		internal static readonly PropertyTag DNPrefix = new PropertyTag(1074266142U);

		internal static readonly PropertyTag NewFXFolder = new PropertyTag(1074856194U);

		private static readonly PropertyTag IncrSyncImailStreamContinue = new PropertyTag(1080426499U);

		private static readonly PropertyTag IncrSyncImailStreamCancel = new PropertyTag(1080492035U);

		private static readonly PropertyTag IncrSyncImailStream2Continue = new PropertyTag(1081147395U);

		internal static readonly PropertyTag PropertyGroupMappingInfo = new PropertyTag(258U);

		internal static readonly PropertyTag OriginalMessageEntryId = new PropertyTag(806289666U);

		internal static readonly PropertyTag Fid = new PropertyTag(1732771860U);

		internal static readonly PropertyTag Mid = new PropertyTag(1732902932U);

		internal static readonly PropertyTag ParentFid = new PropertyTag(1732837396U);

		internal static readonly PropertyTag SourceKey = new PropertyTag(1709179138U);

		internal static readonly PropertyTag ParentSourceKey = new PropertyTag(1709244674U);

		internal static readonly PropertyTag ExternalFid = PropertyTag.SourceKey;

		internal static readonly PropertyTag ExternalParentFid = PropertyTag.ParentSourceKey;

		internal static readonly PropertyTag ExternalMid = PropertyTag.SourceKey;

		internal static readonly PropertyTag LastModificationTime = new PropertyTag(805830720U);

		internal static readonly PropertyTag ChangeKey = new PropertyTag(1709310210U);

		internal static readonly PropertyTag ChangeNumber = new PropertyTag(1738801172U);

		internal static readonly PropertyTag ExternalChangeNumber = PropertyTag.ChangeKey;

		internal static readonly PropertyTag PredecessorChangeList = new PropertyTag(1709375746U);

		internal static readonly PropertyTag ExternalPredecessorChangeList = PropertyTag.PredecessorChangeList;

		internal static readonly PropertyTag ReadChangeNumber = new PropertyTag(1744699412U);

		internal static readonly PropertyTag DisplayName = new PropertyTag(805371935U);

		internal static readonly PropertyTag Comment = new PropertyTag(805568543U);

		internal static readonly PropertyTag MessageSize = new PropertyTag(235405315U);

		internal static readonly PropertyTag Associated = new PropertyTag(1739194379U);

		internal static readonly PropertyTag IdsetDeleted = new PropertyTag(1743061250U);

		internal static readonly PropertyTag IdsetExpired = new PropertyTag(1737687298U);

		internal static readonly PropertyTag IdsetSoftDeleted = new PropertyTag(1075904770U);

		internal static readonly PropertyTag IdsetRead = new PropertyTag(1076691202U);

		internal static readonly PropertyTag IdsetUnread = new PropertyTag(1076756738U);

		internal static readonly PropertyTag FreeBusyNTSD = new PropertyTag(251658498U);

		internal static readonly PropertyTag AttachmentNumber = new PropertyTag(237043715U);

		internal static readonly PropertyTag ObjectType = new PropertyTag(268304387U);

		internal static readonly PropertyTag AttachmentDataObject = new PropertyTag(922812429U);

		internal static readonly PropertyTag AttachmentMethod = new PropertyTag(923074563U);

		internal static readonly PropertyTag ContainerClass = new PropertyTag(907214879U);

		internal static readonly PropertyTag PackedNamedProps = new PropertyTag(907804930U);

		private static readonly HashSet<PropertyTag> MarkerPropertyTags = new HashSet<PropertyTag>(new PropertyTag[]
		{
			PropertyTag.StartTopFld,
			PropertyTag.StartSubFld,
			PropertyTag.EndFolder,
			PropertyTag.StartMessage,
			PropertyTag.StartFAIMsg,
			PropertyTag.EndMessage,
			PropertyTag.StartEmbed,
			PropertyTag.EndEmbed,
			PropertyTag.StartRecip,
			PropertyTag.EndRecip,
			PropertyTag.NewAttach,
			PropertyTag.EndAttach,
			PropertyTag.IncrSyncChg,
			PropertyTag.IncrSyncChgPartial,
			PropertyTag.IncrSyncDel,
			PropertyTag.IncrSyncEnd,
			PropertyTag.IncrSyncMsg,
			PropertyTag.IncrSyncRead,
			PropertyTag.IncrSyncStateBegin,
			PropertyTag.IncrSyncStateEnd,
			PropertyTag.IncrSyncProgressMode,
			PropertyTag.IncrSyncProgressPerMsg,
			PropertyTag.IncrSyncGroupInfo,
			PropertyTag.FXErrorInfo,
			PropertyTag.IncrSyncImailStreamContinue,
			PropertyTag.IncrSyncImailStreamCancel,
			PropertyTag.IncrSyncImailStream2Continue,
			PropertyTag.IncrSyncGroupId,
			PropertyTag.IncrSyncMsgPartial
		});

		private static readonly HashSet<PropertyTag> MetaPropertyTags = new HashSet<PropertyTag>(new PropertyTag[]
		{
			PropertyTag.DNPrefix,
			PropertyTag.FXDelProp,
			PropertyTag.EcWarning,
			PropertyTag.NewFXFolder,
			PropertyTag.FXErrorInfo,
			PropertyTag.IncrSyncGroupId,
			PropertyTag.IncrSyncMsgPartial
		});

		internal static readonly PropertyTag EntryId = new PropertyTag(268370178U);

		internal static readonly PropertyTag SenderEntryId = new PropertyTag(202965250U);

		internal static readonly PropertyTag SentRepresentingEntryId = new PropertyTag(4260098U);

		internal static readonly PropertyTag ReceivedByEntryId = new PropertyTag(4129026U);

		internal static readonly PropertyTag ReceivedRepresentingEntryId = new PropertyTag(4391170U);

		internal static readonly PropertyTag ReadReceiptEntryId = new PropertyTag(4587778U);

		internal static readonly PropertyTag ReportEntryId = new PropertyTag(4522242U);

		internal static readonly PropertyTag OriginatorEntryId = new PropertyTag(1717436674U);

		internal static readonly PropertyTag CreatorEntryId = new PropertyTag(1073283330U);

		internal static readonly PropertyTag LastModifierEntryId = new PropertyTag(1073414402U);

		internal static readonly PropertyTag OriginalSenderEntryId = new PropertyTag(5964034U);

		internal static readonly PropertyTag OriginalSentRepresentingEntryId = new PropertyTag(6160642U);

		internal static readonly PropertyTag OriginalEntryId = new PropertyTag(974258434U);

		internal static readonly PropertyTag ReportDestinationEntryId = new PropertyTag(1717895426U);

		internal static readonly PropertyTag OriginalAuthorEntryId = new PropertyTag(4980994U);

		public static readonly PropertyTag[] OneOffEntryIdPropertyTags = new PropertyTag[]
		{
			PropertyTag.EntryId,
			PropertyTag.SenderEntryId,
			PropertyTag.SentRepresentingEntryId,
			PropertyTag.ReceivedByEntryId,
			PropertyTag.ReceivedRepresentingEntryId,
			PropertyTag.ReadReceiptEntryId,
			PropertyTag.ReportEntryId,
			PropertyTag.OriginatorEntryId,
			PropertyTag.CreatorEntryId,
			PropertyTag.LastModifierEntryId,
			PropertyTag.OriginalSenderEntryId,
			PropertyTag.OriginalSentRepresentingEntryId,
			PropertyTag.OriginalEntryId,
			PropertyTag.ReportDestinationEntryId,
			PropertyTag.OriginalAuthorEntryId
		};

		internal static readonly PropertyTag ParentEntryId = new PropertyTag(235471106U);

		internal static readonly PropertyTag ConflictEntryId = new PropertyTag(1072693506U);

		internal static readonly PropertyTag RuleFolderEntryId = new PropertyTag(1716584706U);

		internal static readonly PropertyTag AddressType = new PropertyTag(805437471U);

		internal static readonly PropertyTag EmailAddress = new PropertyTag(805503007U);

		internal static readonly PropertyTag RowId = new PropertyTag(805306371U);

		internal static readonly PropertyTag InstanceKey = new PropertyTag(267780354U);

		internal static readonly PropertyTag RecipientType = new PropertyTag(202702851U);

		internal static readonly PropertyTag SearchKey = new PropertyTag(806027522U);

		internal static readonly PropertyTag TransmittableDisplayName = new PropertyTag(975175711U);

		public static readonly PropertyTag SimpleDisplayName = new PropertyTag(973013023U);

		internal static readonly PropertyTag Responsibility = new PropertyTag(235864075U);

		internal static readonly PropertyTag SendRichInfo = new PropertyTag(977272843U);

		public static readonly PropertyTag SendInternetEncoding = new PropertyTag(980484099U);

		internal static readonly PropertyTag[] StandardRecipientPropertyTags = new PropertyTag[]
		{
			PropertyTag.DisplayName,
			PropertyTag.AddressType,
			PropertyTag.EmailAddress,
			PropertyTag.RowId,
			PropertyTag.InstanceKey,
			PropertyTag.RecipientType,
			PropertyTag.EntryId,
			PropertyTag.SearchKey,
			PropertyTag.TransmittableDisplayName,
			PropertyTag.Responsibility,
			PropertyTag.SendRichInfo
		};

		internal static readonly PropertyTag RtfCompressed = new PropertyTag(269025538U);

		internal static readonly PropertyTag AlternateBestBody = new PropertyTag(269091074U);

		internal static readonly PropertyTag HtmlBody = new PropertyTag(269680671U);

		internal static readonly PropertyTag Html = new PropertyTag(269680898U);

		internal static readonly PropertyTag Body = new PropertyTag(268435487U);

		internal static readonly PropertyTag NativeBodyInfo = new PropertyTag(269877251U);

		internal static readonly PropertyTag RtfInSync = new PropertyTag(236912651U);

		internal static readonly PropertyTag Preview = new PropertyTag(1071185951U);

		internal static readonly PropertyTag PreviewUnread = new PropertyTag(1071120415U);

		internal static readonly PropertyTag SentMailServerId = new PropertyTag(1732247803U);

		internal static readonly PropertyTag DamOrgMsgServerId = new PropertyTag(1732313339U);

		internal static readonly PropertyTag SentMailEntryId = new PropertyTag(235536642U);

		internal static readonly PropertyTag DamOrgMsgEntryId = new PropertyTag(1715863810U);

		internal static readonly PropertyTag ConflictMsgKey = new PropertyTag(1070203138U);

		internal static readonly PropertyTag RuleFolderFid = new PropertyTag(1731592212U);

		internal static readonly PropertyTag MessageClass = new PropertyTag(1703966U);

		internal static readonly PropertyTag MessageSubmissionIdFromClient = new PropertyTag(1076625666U);

		internal static readonly PropertyTag MessageSubmissionId = new PropertyTag(4653314U);

		internal static readonly PropertyTag DeferredSendTime = new PropertyTag(1072627776U);

		internal static readonly PropertyTag ConversationItemIds = new PropertyTag(1755320578U);

		internal static readonly PropertyTag ConversationItemIdsMailboxWide = new PropertyTag(1755386114U);

		internal static readonly PropertyTag UnsearchableItems = new PropertyTag(905838850U);

		public static readonly PropertyTag MimeSkeleton = new PropertyTag(1693450498U);

		internal static readonly PropertyTag Read = new PropertyTag(241762315U);

		internal static readonly PropertyTag NTSecurityDescriptor = new PropertyTag(237437186U);

		internal static readonly PropertyTag AclTableAndSecurityDescriptor = new PropertyTag(239010050U);

		internal static readonly PropertyTag AccessLevel = new PropertyTag(267845635U);

		internal static readonly PropertyTag DisplayType = new PropertyTag(956301315U);

		internal static readonly PropertyTag LongTermEntryIdFromTable = new PropertyTag(1718616322U);

		internal static readonly PropertyTag LongtermEntryId = new PropertyTag(1718616322U);

		internal static readonly PropertyTag MappingSignature = new PropertyTag(267911426U);

		internal static readonly PropertyTag MdbProvider = new PropertyTag(873726210U);

		internal static readonly PropertyTag NormalizedSubject = new PropertyTag(236781599U);

		internal static readonly PropertyTag OfflineFlags = new PropertyTag(1715273731U);

		internal static readonly PropertyTag RecordKey = new PropertyTag(267976962U);

		internal static readonly PropertyTag ReplicaServer = new PropertyTag(1715732511U);

		internal static readonly PropertyTag ReplicaVersion = new PropertyTag(1716191252U);

		internal static readonly PropertyTag StoreEntryId = new PropertyTag(268108034U);

		internal static readonly PropertyTag StoreRecordKey = new PropertyTag(268042498U);

		internal static readonly PropertyTag StoreSupportMask = new PropertyTag(873267203U);

		internal static readonly PropertyTag Subject = new PropertyTag(3604511U);

		internal static readonly PropertyTag SubjectPrefix = new PropertyTag(3997727U);

		internal static readonly PropertyTag PostReplyFolderEntries = new PropertyTag(272433410U);

		internal static readonly PropertyTag Depth = new PropertyTag(805634051U);

		internal static readonly PropertyTag RuleCondition = new PropertyTag(1719206141U);

		internal static readonly PropertyTag ValidFolderMask = new PropertyTag(903806979U);

		internal static readonly PropertyTag CodePageId = new PropertyTag(1724055555U);

		internal static readonly PropertyTag LocaleId = new PropertyTag(1721827331U);

		internal static readonly PropertyTag SortLocaleId = new PropertyTag(1728380931U);

		internal static readonly PropertyTag IPMSubtreeFolder = new PropertyTag(903872770U);

		internal static readonly PropertyTag IPMInboxFolder = new PropertyTag(903938306U);

		internal static readonly PropertyTag IPMOutboxFolder = new PropertyTag(904003842U);

		internal static readonly PropertyTag IPMSentmailFolder = new PropertyTag(904134914U);

		internal static readonly PropertyTag IPMWastebasketFolder = new PropertyTag(904069378U);

		internal static readonly PropertyTag IPMFinderFolder = new PropertyTag(904331522U);

		internal static readonly PropertyTag IPMShortcutsFolder = new PropertyTag(1714422018U);

		internal static readonly PropertyTag IPMViewsFolder = new PropertyTag(904200450U);

		internal static readonly PropertyTag IPMCommonViewsFolder = new PropertyTag(904265986U);

		internal static readonly PropertyTag IPMDafFolder = new PropertyTag(1713307906U);

		internal static readonly PropertyTag NonIPMSubtreeFolder = new PropertyTag(1713373442U);

		internal static readonly PropertyTag EformsRegistryFolder = new PropertyTag(1713438978U);

		internal static readonly PropertyTag SplusFreeBusyFolder = new PropertyTag(1713504514U);

		internal static readonly PropertyTag OfflineAddrBookFolder = new PropertyTag(1713570050U);

		internal static readonly PropertyTag ArticleIndexFolder = new PropertyTag(1737097474U);

		internal static readonly PropertyTag LocaleEformsRegistryFolder = new PropertyTag(1713635586U);

		internal static readonly PropertyTag LocalSiteFreeBusyFolder = new PropertyTag(1713701122U);

		internal static readonly PropertyTag LocalSiteAddrBookFolder = new PropertyTag(1713766658U);

		internal static readonly PropertyTag MTSInFolder = new PropertyTag(1713897730U);

		internal static readonly PropertyTag MTSOutFolder = new PropertyTag(1713963266U);

		internal static readonly PropertyTag ScheduleFolder = new PropertyTag(1713242370U);

		internal static readonly PropertyTag StoreState = new PropertyTag(873332739U);

		internal static readonly PropertyTag HierarchyServer = new PropertyTag(1714618626U);

		internal static readonly PropertyTag LogonRightsOnMailbox = new PropertyTag(1736245251U);

		internal static readonly PropertyTag MessageFlags = new PropertyTag(235339779U);

		internal static readonly PropertyTag OriginalDisplayBcc = new PropertyTag(7471135U);

		internal static readonly PropertyTag OriginalDisplayCc = new PropertyTag(7536671U);

		internal static readonly PropertyTag OriginalDisplayTo = new PropertyTag(7602207U);

		internal static readonly PropertyTag MessageDeliveryTime = new PropertyTag(235274304U);

		internal static readonly PropertyTag RtfSyncBodyCRC = new PropertyTag(268828675U);

		internal static readonly PropertyTag RtfSyncBodyCount = new PropertyTag(268894211U);

		internal static readonly PropertyTag RtfSyncBodyTag = new PropertyTag(268959775U);

		internal static readonly PropertyTag RtfSyncPrefixCount = new PropertyTag(269484035U);

		internal static readonly PropertyTag RtfSyncTrailingCount = new PropertyTag(269549571U);

		internal static readonly PropertyTag CreatorName = new PropertyTag(1073217567U);

		internal static readonly PropertyTag UrlCompNamePostfix = new PropertyTag(241238019U);

		internal static readonly PropertyTag MimeSize = new PropertyTag(1732640771U);

		internal static readonly PropertyTag FileSize = new PropertyTag(1732706307U);

		internal static readonly PropertyTag InternetReference = new PropertyTag(272171039U);

		internal static readonly PropertyTag InternetNewsGroups = new PropertyTag(271974431U);

		internal static readonly PropertyTag ImapCachedBodystructure = new PropertyTag(1735196930U);

		internal static readonly PropertyTag ImapCachedEnvelope = new PropertyTag(1735131394U);

		internal static readonly PropertyTag LocalCommitTime = new PropertyTag(1728643136U);

		internal static readonly PropertyTag AutoReset = new PropertyTag(1728843848U);

		internal static readonly PropertyTag DeletedOn = new PropertyTag(1720647744U);

		internal static readonly PropertyTag SMTPTempTblData = new PropertyTag(281018626U);

		internal static readonly PropertyTag SMTPTempTblData2 = new PropertyTag(281084162U);

		internal static readonly PropertyTag SMTPTempTblData3 = new PropertyTag(281149698U);

		internal static readonly PropertyTag AttachmentSize = new PropertyTag(236978179U);

		internal static readonly PropertyTag DisplayBcc = new PropertyTag(235012127U);

		internal static readonly PropertyTag DisplayCc = new PropertyTag(235077663U);

		internal static readonly PropertyTag DisplayTo = new PropertyTag(235143199U);

		internal static readonly PropertyTag HasAttach = new PropertyTag(236650507U);

		internal static readonly PropertyTag Access = new PropertyTag(267649027U);

		internal static readonly PropertyTag InstanceId = new PropertyTag(1733099540U);

		internal static readonly PropertyTag RowType = new PropertyTag(267714563U);

		internal static readonly PropertyTag SecureSubmitFlags = new PropertyTag(1707474947U);

		internal static readonly PropertyTag FolderNamedProperties = new PropertyTag(907804930U);

		internal static readonly PropertyTag ContentCount = new PropertyTag(906100739U);

		internal static readonly PropertyTag ContentUnread = new PropertyTag(906166275U);

		internal static readonly PropertyTag FolderType = new PropertyTag(906035203U);

		internal static readonly PropertyTag IsNewsgroupAnchor = new PropertyTag(1721106443U);

		internal static readonly PropertyTag IsNewsgroup = new PropertyTag(1721171979U);

		internal static readonly PropertyTag NewsgroupComp = new PropertyTag(1722089503U);

		internal static readonly PropertyTag InetNewsgroupName = new PropertyTag(1722220575U);

		internal static readonly PropertyTag NewsfeedAcl = new PropertyTag(1722155266U);

		internal static readonly PropertyTag DeletedMsgCt = new PropertyTag(1715470339U);

		internal static readonly PropertyTag DeletedAssocMsgCt = new PropertyTag(1715666947U);

		internal static readonly PropertyTag DeletedFolderCt = new PropertyTag(1715535875U);

		internal static readonly PropertyTag DeletedMessageSizeExtended = new PropertyTag(1721434132U);

		internal static readonly PropertyTag DeletedAssocMessageSizeExtended = new PropertyTag(1721565204U);

		internal static readonly PropertyTag DeletedNormalMessageSizeExtended = new PropertyTag(1721499668U);

		internal static readonly PropertyTag LocalCommitTimeMax = new PropertyTag(1728708672U);

		internal static readonly PropertyTag DeletedCountTotal = new PropertyTag(1728774147U);

		internal static readonly PropertyTag ICSChangeKey = new PropertyTag(1716846850U);

		internal static readonly PropertyTag URLName = new PropertyTag(1728512031U);

		internal static readonly PropertyTag HierRev = new PropertyTag(1082261568U);

		internal static readonly PropertyTag Subfolders = new PropertyTag(906625035U);

		internal static readonly PropertyTag CreationTime = new PropertyTag(805765184U);

		internal static readonly PropertyTag FolderChildCount = new PropertyTag(1714946051U);

		internal static readonly PropertyTag Rights = new PropertyTag(1715011587U);

		internal static readonly PropertyTag AddressBookEntryId = new PropertyTag(1715142914U);

		internal static readonly PropertyTag DisablePeruserRead = new PropertyTag(1724186635U);

		internal static readonly PropertyTag SecureInSite = new PropertyTag(1721630731U);

		internal static readonly PropertyTag PublicFolderPlatinumHomeMDB = new PropertyTag(1730019339U);

		internal static readonly PropertyTag PublicFolderProxyRequired = new PropertyTag(1730084875U);

		internal static readonly PropertyTag LongTermId = new PropertyTag(1733820674U);

		internal static readonly PropertyTag InstanceIdBin = new PropertyTag(1739587842U);

		internal static readonly PropertyTag LocalDirectoryEntryId = new PropertyTag(873857282U);

		internal static readonly PropertyTag ConversationId = new PropertyTag(806551810U);

		internal static readonly PropertyTag ChangeType = new PropertyTag(1733296130U);

		internal static readonly PropertyTag MailboxOwnerName = new PropertyTag(1713111071U);

		internal static readonly PropertyTag OOFState = new PropertyTag(1713176587U);

		internal static readonly PropertyTag Anr = new PropertyTag(906756127U);

		private sealed class PropertyIdComparerImpl : IEqualityComparer<PropertyTag>
		{
			public bool Equals(PropertyTag x, PropertyTag y)
			{
				return x.PropertyId == y.PropertyId;
			}

			public int GetHashCode(PropertyTag x)
			{
				return (int)x.PropertyId;
			}
		}
	}
}
