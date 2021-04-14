using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.NaturalLanguage;
using Microsoft.Exchange.Data.Storage.ReliableActions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(Microsoft.Exchange.Diagnostics.AccessLevel.Implementation)]
	internal static class InternalSchema
	{
		internal static string ToPropertyDefinitionString(PropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				return "<unknown>:<unknown>";
			}
			return propertyDefinition.Name + ":" + propertyDefinition.Type.ToString();
		}

		internal static StorePropertyDefinition ToStorePropertyDefinition(PropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			StorePropertyDefinition storePropertyDefinition = propertyDefinition as StorePropertyDefinition;
			if (storePropertyDefinition != null)
			{
				return storePropertyDefinition;
			}
			throw new InvalidOperationException(ServerStrings.ExNoMatchingStorePropertyDefinition(InternalSchema.ToPropertyDefinitionString(propertyDefinition)));
		}

		internal static StorePropertyDefinition[] ToStorePropertyDefinitions(ICollection<PropertyDefinition> propertyDefinitions)
		{
			int count = propertyDefinitions.Count;
			StorePropertyDefinition[] array = new StorePropertyDefinition[count];
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitions)
			{
				array[num++] = InternalSchema.ToStorePropertyDefinition(propertyDefinition);
			}
			return array;
		}

		internal static NativeStorePropertyDefinition[] RemoveNullPropertyDefinions(NativeStorePropertyDefinition[] definitions, out bool changed)
		{
			List<NativeStorePropertyDefinition> list = new List<NativeStorePropertyDefinition>(definitions.Length);
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in definitions)
			{
				if (nativeStorePropertyDefinition != null)
				{
					list.Add(nativeStorePropertyDefinition);
				}
			}
			changed = (list.Count != definitions.Length);
			return list.ToArray();
		}

		internal static PropType PropTagTypeFromClrType(Type type)
		{
			if (typeof(short) == type)
			{
				return PropType.Short;
			}
			if (typeof(int) == type)
			{
				return PropType.Int;
			}
			if (typeof(long) == type)
			{
				return PropType.Long;
			}
			if (typeof(float) == type)
			{
				return PropType.Float;
			}
			if (typeof(double) == type)
			{
				return PropType.Double;
			}
			if (typeof(ExDateTime) == type)
			{
				return PropType.SysTime;
			}
			if (typeof(bool) == type)
			{
				return PropType.Boolean;
			}
			if (typeof(string) == type)
			{
				return PropType.String;
			}
			if (typeof(Guid) == type)
			{
				return PropType.Guid;
			}
			if (typeof(byte[]) == type)
			{
				return PropType.Binary;
			}
			if (typeof(string[]) == type)
			{
				return PropType.StringArray;
			}
			if (typeof(byte[][]) == type)
			{
				return PropType.BinaryArray;
			}
			if (typeof(short[]) == type)
			{
				return PropType.ShortArray;
			}
			if (typeof(int[]) == type)
			{
				return PropType.IntArray;
			}
			if (typeof(float[]) == type)
			{
				return PropType.FloatArray;
			}
			if (typeof(double[]) == type)
			{
				return PropType.DoubleArray;
			}
			if (typeof(long[]) == type)
			{
				return PropType.LongArray;
			}
			if (typeof(ExDateTime[]) == type)
			{
				return PropType.SysTimeArray;
			}
			if (typeof(Guid[]) == type)
			{
				return PropType.GuidArray;
			}
			throw new ArgumentException(ServerStrings.ExUnsupportedMapiType(type));
		}

		internal static Type ClrTypeFromPropTag(PropTag propTag)
		{
			return InternalSchema.ClrTypeFromPropTagType(propTag.ValueType());
		}

		internal static Type ClrTypeFromPropTagType(PropType mapiType)
		{
			if (mapiType <= PropType.Binary)
			{
				if (mapiType <= PropType.SysTime)
				{
					switch (mapiType)
					{
					case PropType.Null:
						return typeof(int);
					case PropType.Short:
						return typeof(short);
					case PropType.Int:
						return typeof(int);
					case PropType.Float:
						return typeof(float);
					case PropType.Double:
						return typeof(double);
					case PropType.Currency:
						return typeof(long);
					case PropType.AppTime:
						return typeof(double);
					case (PropType)8:
					case (PropType)9:
					case (PropType)12:
					case (PropType)14:
					case (PropType)15:
					case (PropType)16:
					case (PropType)17:
					case (PropType)18:
					case (PropType)19:
						break;
					case PropType.Error:
						return typeof(int);
					case PropType.Boolean:
						return typeof(bool);
					case PropType.Object:
						return typeof(object);
					case PropType.Long:
						return typeof(long);
					default:
						switch (mapiType)
						{
						case PropType.AnsiString:
							return typeof(string);
						case PropType.String:
							return typeof(string);
						default:
							if (mapiType == PropType.SysTime)
							{
								return typeof(ExDateTime);
							}
							break;
						}
						break;
					}
				}
				else
				{
					if (mapiType == PropType.Guid)
					{
						return typeof(Guid);
					}
					switch (mapiType)
					{
					case PropType.Restriction:
						return typeof(QueryFilter);
					case PropType.Actions:
						return typeof(RuleAction[]);
					default:
						if (mapiType == PropType.Binary)
						{
							return typeof(byte[]);
						}
						break;
					}
				}
			}
			else if (mapiType <= PropType.StringArray)
			{
				switch (mapiType)
				{
				case PropType.ShortArray:
					return typeof(short[]);
				case PropType.IntArray:
					return typeof(int[]);
				case PropType.FloatArray:
					return typeof(float[]);
				case PropType.DoubleArray:
					return typeof(double[]);
				case PropType.CurrencyArray:
					return typeof(long[]);
				case PropType.AppTimeArray:
					return typeof(double[]);
				case (PropType)4104:
				case (PropType)4105:
				case (PropType)4106:
				case (PropType)4107:
				case (PropType)4108:
					break;
				case PropType.ObjectArray:
					return typeof(object[]);
				default:
					if (mapiType == PropType.LongArray)
					{
						return typeof(long[]);
					}
					switch (mapiType)
					{
					case PropType.AnsiStringArray:
						return typeof(string[]);
					case PropType.StringArray:
						return typeof(string[]);
					}
					break;
				}
			}
			else
			{
				if (mapiType == PropType.SysTimeArray)
				{
					return typeof(ExDateTime[]);
				}
				if (mapiType == PropType.GuidArray)
				{
					return typeof(Guid[]);
				}
				if (mapiType == PropType.BinaryArray)
				{
					return typeof(byte[][]);
				}
			}
			LocalizedString localizedString = ServerStrings.ExInvalidMAPIType((uint)mapiType);
			ExTraceGlobals.StorageTracer.TraceError(0L, localizedString);
			throw new InvalidPropertyTypeException(localizedString);
		}

		internal static void CheckPropertyValueType(PropertyDefinition propertyDefinition, object value)
		{
			try
			{
				if (!(value.GetType() == propertyDefinition.Type))
				{
					if (typeof(short) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<short>(value);
					}
					else if (typeof(int) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<int>(value);
					}
					else if (typeof(long) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<long>(value);
					}
					else if (typeof(float) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<float>(value);
					}
					else if (typeof(double) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<double>(value);
					}
					else if (typeof(ExDateTime) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<ExDateTime>(value);
					}
					else if (typeof(bool) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<bool>(value);
					}
					else if (typeof(string) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<string>(value);
					}
					else if (typeof(Guid) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<Guid>(value);
					}
					else if (typeof(byte[]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<byte[]>(value);
					}
					else if (typeof(string[]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<string[]>(value);
					}
					else if (typeof(byte[][]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<byte[][]>(value);
					}
					else if (typeof(short[]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<short[]>(value);
					}
					else if (typeof(int[]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<int[]>(value);
					}
					else if (typeof(float[]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<float[]>(value);
					}
					else if (typeof(double[]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<double[]>(value);
					}
					else if (typeof(long[]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<long[]>(value);
					}
					else if (typeof(ExDateTime[]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<ExDateTime[]>(value);
					}
					else if (typeof(Guid[]) == propertyDefinition.Type)
					{
						InternalSchema.CheckValueType<Guid[]>(value);
					}
					else
					{
						if (!(typeof(QueryFilter) == propertyDefinition.Type))
						{
							string message = string.Format("Value of type {0} was passed for property {1}; should be type {2}.", value.GetType(), propertyDefinition, propertyDefinition.Type);
							throw new ArgumentException(message);
						}
						InternalSchema.CheckValueType<QueryFilter>(value);
					}
				}
			}
			catch (InvalidCastException innerException)
			{
				string message2 = string.Format("Value of type {0} was passed for property {1}; should be type {2}.", value.GetType(), propertyDefinition, propertyDefinition.Type);
				throw new ArgumentException(message2, innerException);
			}
		}

		private static void CheckValueType<T>(object value)
		{
			T t = (T)((object)value);
		}

		internal static ICollection<T> Combine<T>(ICollection<T> first, ICollection<T> second) where T : PropertyDefinition
		{
			if (first == InternalSchema.ContentConversionPropertiesAsICollection)
			{
				return first;
			}
			if (second == InternalSchema.ContentConversionPropertiesAsICollection)
			{
				return second;
			}
			return first.Union(second);
		}

		private static GuidNamePropertyDefinition CreateGuidNameProperty(string displayName, Type propertyType, Guid propertyGuid, string propertyName = null)
		{
			return InternalSchema.CreateGuidNameProperty(displayName, propertyType, propertyGuid, propertyName ?? displayName, PropertyFlags.None, PropertyDefinitionConstraint.None);
		}

		private static GuidNamePropertyDefinition CreateGuidNameProperty(string displayName, Type propertyType, Guid propertyGuid, string propertyName, PropertyFlags propertyFlag, params PropertyDefinitionConstraint[] constraints)
		{
			PropType mapiPropType = InternalSchema.PropTagTypeFromClrType(propertyType);
			return GuidNamePropertyDefinition.InternalCreate(displayName, propertyType, mapiPropType, propertyGuid, propertyName, propertyFlag, NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, false, constraints);
		}

		private static GuidIdPropertyDefinition CreateGuidIdProperty(string displayName, Type propertyType, Guid propertyGuid, int dispId)
		{
			return InternalSchema.CreateGuidIdProperty(displayName, propertyType, propertyGuid, dispId, PropertyFlags.None, PropertyDefinitionConstraint.None);
		}

		private static GuidIdPropertyDefinition CreateGuidIdProperty(string displayName, Type propertyType, Guid propertyGuid, int dispId, PropertyFlags propertyFlag, params PropertyDefinitionConstraint[] constraints)
		{
			PropType mapiPropType = InternalSchema.PropTagTypeFromClrType(propertyType);
			return GuidIdPropertyDefinition.InternalCreate(displayName, propertyType, mapiPropType, propertyGuid, dispId, propertyFlag, NativeStorePropertyDefinition.TypeCheckingFlag.DisableTypeCheck, false, constraints);
		}

		private const int TaskWorkLimit = 1525252319;

		internal const int MaxSubjectLength = 255;

		internal static readonly StorePropertyDefinition[] ContentConversionProperties = new StorePropertyDefinition[0];

		private static readonly ICollection<PropertyDefinition> ContentConversionPropertiesAsICollection = (ICollection<PropertyDefinition>)InternalSchema.ContentConversionProperties;

		public static readonly PropertyTagPropertyDefinition PropertyGroupChangeMask = PropertyTagPropertyDefinition.InternalCreate("PropertyGroupChangeMask", PropTag.PropertyGroupChangeMask);

		public static readonly PropertyTagPropertyDefinition PropertyGroupMappingId = PropertyTagPropertyDefinition.InternalCreate("PropertyGroupMappingId ", PropTag.LdapReads);

		public static readonly GuidNamePropertyDefinition PropertyExistenceTracker = InternalSchema.CreateGuidNameProperty("PropertyExistenceTracker", typeof(long), WellKnownPropertySet.Common, "PropertyExistenceTracker");

		public static readonly PropertyTagPropertyDefinition MailEnabled = PropertyTagPropertyDefinition.InternalCreate("MailEnabled", PropTag.PfProxyRequired);

		public static readonly PropertyTagPropertyDefinition ProxyGuid = PropertyTagPropertyDefinition.InternalCreate("ProxyGuid", PropTag.PfProxy);

		public static readonly PropertyTagPropertyDefinition LocalDirectory = PropertyTagPropertyDefinition.InternalCreate("LocalDirectory", PropTag.LocalDirectory);

		public static readonly PropertyTagPropertyDefinition MemberEmailLocalDirectory = PropertyTagPropertyDefinition.InternalCreate("MemberEmail", PropTag.MemberEmail);

		public static readonly PropertyTagPropertyDefinition MemberExternalIdLocalDirectory = PropertyTagPropertyDefinition.InternalCreate("MemberExternalId", PropTag.MemberExternalId);

		public static readonly PropertyTagPropertyDefinition MemberSIDLocalDirectory = PropertyTagPropertyDefinition.InternalCreate("MemberSID", PropTag.MemberSID);

		public static readonly PropertyTagPropertyDefinition SenderTelephoneNumber = PropertyTagPropertyDefinition.InternalCreate("SenderTelephoneNumber", (PropTag)1744961567U);

		public static readonly PropertyTagPropertyDefinition RetentionAgeLimit = PropertyTagPropertyDefinition.InternalCreate("RetentionAgeLimit", PropTag.RetentionAgeLimit);

		public static readonly PropertyTagPropertyDefinition OverallAgeLimit = PropertyTagPropertyDefinition.InternalCreate("OverallAgeLimit", PropTag.OverallAgeLimit);

		public static readonly PropertyTagPropertyDefinition PfQuotaStyle = PropertyTagPropertyDefinition.InternalCreate("PfQuotaStyle", PropTag.PfQuotaStyle);

		public static readonly PropertyTagPropertyDefinition PfOverHardQuotaLimit = PropertyTagPropertyDefinition.InternalCreate("PfOverHardQuotaLimit", PropTag.PfOverHardQuotaLimit);

		public static readonly PropertyTagPropertyDefinition PfStorageQuota = PropertyTagPropertyDefinition.InternalCreate("PfStorageQuota", PropTag.PfStorageQuota);

		public static readonly PropertyTagPropertyDefinition PfMsgSizeLimit = PropertyTagPropertyDefinition.InternalCreate("PfMsgSizeLimit", PropTag.PfMsgSizeLimit);

		public static readonly PropertyTagPropertyDefinition DisablePerUserRead = PropertyTagPropertyDefinition.InternalCreate("DisablePerUserRead", PropTag.DisablePeruserRead);

		public static readonly PropertyTagPropertyDefinition EformsLocaleId = PropertyTagPropertyDefinition.InternalCreate("EformsLocaleId", PropTag.EformsLocaleId);

		public static readonly PropertyTagPropertyDefinition PublishInAddressBook = PropertyTagPropertyDefinition.InternalCreate("PublishInAddressBook", PropTag.PublishInAddressBook);

		public static readonly PropertyTagPropertyDefinition HasRules = PropertyTagPropertyDefinition.InternalCreate("HasRules", PropTag.HasRules);

		internal static readonly PropertyTagPropertyDefinition MapiRulesTable = PropertyTagPropertyDefinition.InternalCreate("MapiRulesTable", PropTag.RulesTable, PropertyFlags.Streamable);

		internal static readonly PropertyTagPropertyDefinition MapiRulesData = PropertyTagPropertyDefinition.InternalCreate("MapiRulesData", (PropTag)1071710466U);

		internal static readonly PropertyTagPropertyDefinition ExtendedRuleCondition = PropertyTagPropertyDefinition.InternalCreate("ExtendedRuleCondition", (PropTag)244973826U);

		internal static readonly PropertyTagPropertyDefinition ExtendedRuleSizeLimit = PropertyTagPropertyDefinition.InternalCreate("ExtendedRuleSizeLimit", (PropTag)245039107U);

		internal static readonly PropertyTagPropertyDefinition MapiAclTable = PropertyTagPropertyDefinition.InternalCreate("MapiAclTable", PropTag.AclTable);

		internal static readonly PropertyTagPropertyDefinition RawSecurityDescriptor = PropertyTagPropertyDefinition.InternalCreate("NTSD", PropTag.NTSD);

		internal static readonly PropertyTagPropertyDefinition RawSecurityDescriptorAsXml = PropertyTagPropertyDefinition.InternalCreate("NTSD", PropTag.NTSDAsXML, PropertyFlags.Streamable);

		internal static readonly PropertyTagPropertyDefinition AclTableAndSecurityDescriptor = PropertyTagPropertyDefinition.InternalCreate("AclTableAndNTSD", PropTag.AclTableAndNTSD);

		public static readonly StorePropertyDefinition SecurityDescriptor = new SecurityDescriptorProperty(InternalSchema.RawSecurityDescriptor);

		public static readonly GuidNamePropertyDefinition XSenderTelephoneNumber = InternalSchema.CreateGuidNameProperty("X-CallingTelephoneNumber", typeof(string), WellKnownPropertySet.InternetHeaders, "X-CallingTelephoneNumber");

		public static readonly GuidNamePropertyDefinition XTnefCorrelator = InternalSchema.CreateGuidNameProperty("X-MS-TNEF-Correlator", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-TNEF-Correlator");

		public static readonly GuidNamePropertyDefinition XMsExchOrganizationAuthDomain = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-Organization-AuthDomain", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-AuthDomain");

		public static readonly GuidNamePropertyDefinition XMsExchOrganizationAuthAs = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-Organization-AuthAs", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-AuthAs");

		public static readonly GuidNamePropertyDefinition XMsExchOrganizationAuthMechanism = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-Organization-AuthMechanism", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-AuthMechanism");

		public static readonly GuidNamePropertyDefinition XMsExchOrganizationAuthSource = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-Organization-AuthSource", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-AuthSource");

		public static readonly GuidNamePropertyDefinition XMsExchImapAppendStamp = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-ImapAppendStamp", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-ImapAppendStamp");

		public static readonly GuidNamePropertyDefinition XMsExchOrganizationOriginalClientIPAddress = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-Organization-OriginalClientIPAddress", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-OriginalClientIPAddress");

		public static readonly GuidNamePropertyDefinition XMsExchOrganizationOriginalServerIPAddress = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-Organization-OriginalServerIPAddress", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-OriginalServerIPAddress");

		public static readonly GuidNamePropertyDefinition LastMovedTimeStamp = InternalSchema.CreateGuidNameProperty("LastMovedTimeStamp", typeof(ExDateTime), WellKnownPropertySet.Elc, "LastMovedTimeStamp");

		public static readonly GuidNamePropertyDefinition OriginalScl = InternalSchema.CreateGuidNameProperty("OriginalScl", typeof(int), WellKnownPropertySet.Messaging, "OriginalScl");

		public static readonly PropertyTagPropertyDefinition SecureSubmitFlags = PropertyTagPropertyDefinition.InternalCreate("SecureSubmitFlags", PropTag.SecureSubmitFlags);

		public static readonly PropertyTagPropertyDefinition SubmitFlags = PropertyTagPropertyDefinition.InternalCreate("SubmitFlags", PropTag.SubmitFlags);

		public static readonly PropertyTagPropertyDefinition XMsExchOrganizationAVStampMailbox = PropertyTagPropertyDefinition.InternalCreate("XMsExchOrganizationAVStampMailbox", PropTag.TransportAntiVirusStamp);

		public static readonly PropertyTagPropertyDefinition VoiceMessageDuration = PropertyTagPropertyDefinition.InternalCreate("VoiceMessageDuration", (PropTag)1744896003U);

		public static readonly GuidNamePropertyDefinition XVoiceMessageDuration = InternalSchema.CreateGuidNameProperty("X-VoiceMessageDuration", typeof(string), WellKnownPropertySet.InternetHeaders, "X-VoiceMessageDuration");

		public static readonly GuidNamePropertyDefinition XMsExchangeOrganizationRightsProtectMessage = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-Organization-RightsProtectMessage", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-RightsProtectMessage");

		public static readonly PropertyTagPropertyDefinition VoiceMessageSenderName = PropertyTagPropertyDefinition.InternalCreate("VoiceMessageSenderName", (PropTag)1745027103U);

		public static readonly GuidNamePropertyDefinition XVoiceMessageSenderName = InternalSchema.CreateGuidNameProperty("X-VoiceMessageSenderName", typeof(string), WellKnownPropertySet.InternetHeaders, "X-VoiceMessageSenderName");

		public static readonly PropertyTagPropertyDefinition FaxNumberOfPages = PropertyTagPropertyDefinition.InternalCreate("FaxNumberOfPages", (PropTag)1745092611U);

		public static readonly GuidNamePropertyDefinition XFaxNumberOfPages = InternalSchema.CreateGuidNameProperty("X-FaxNumberOfPages", typeof(string), WellKnownPropertySet.InternetHeaders, "X-FaxNumberOfPages");

		public static readonly PropertyTagPropertyDefinition VoiceMessageAttachmentOrder = PropertyTagPropertyDefinition.InternalCreate("VoiceMessageAttachmentOrder", (PropTag)1745158175U);

		public static readonly GuidNamePropertyDefinition XVoiceMessageAttachmentOrder = InternalSchema.CreateGuidNameProperty("X-AttachmentOrder", typeof(string), WellKnownPropertySet.InternetHeaders, "X-AttachmentOrder");

		public static readonly PropertyTagPropertyDefinition CallId = PropertyTagPropertyDefinition.InternalCreate("CallId", (PropTag)1745223711U);

		public static readonly GuidNamePropertyDefinition XCallId = InternalSchema.CreateGuidNameProperty("X-CallID", typeof(string), WellKnownPropertySet.InternetHeaders, "X-CallID");

		public static readonly GuidNamePropertyDefinition XRequireProtectedPlayOnPhone = InternalSchema.CreateGuidNameProperty("X-RequireProtectedPlayOnPhone", typeof(string), WellKnownPropertySet.InternetHeaders, "X-RequireProtectedPlayOnPhone");

		public static readonly GuidNamePropertyDefinition XMsExchangeUMPartnerAssignedID = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-UM-PartnerAssignedID", typeof(string), WellKnownPropertySet.UnifiedMessaging, "X-MS-Exchange-UM-PartnerAssignedID");

		public static readonly GuidNamePropertyDefinition XMsExchangeUMPartnerContent = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-UM-PartnerContent", typeof(string), WellKnownPropertySet.UnifiedMessaging, "X-MS-Exchange-UM-PartnerContent");

		public static readonly GuidNamePropertyDefinition XMsExchangeUMPartnerContext = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-UM-PartnerContext", typeof(string), WellKnownPropertySet.UnifiedMessaging, "X-MS-Exchange-UM-PartnerContext");

		public static readonly GuidNamePropertyDefinition XMsExchangeUMPartnerStatus = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-UM-PartnerStatus", typeof(string), WellKnownPropertySet.UnifiedMessaging, "X-MS-Exchange-UM-PartnerStatus");

		public static readonly GuidNamePropertyDefinition XMsExchangeUMDialPlanLanguage = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-UM-DialPlanLanguage", typeof(string), WellKnownPropertySet.UnifiedMessaging, "X-MS-Exchange-UM-DialPlanLanguage");

		public static readonly GuidNamePropertyDefinition XMsExchangeUMCallerInformedOfAnalysis = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-UM-CallerInformedOfAnalysis", typeof(string), WellKnownPropertySet.UnifiedMessaging, "X-MS-Exchange-UM-CallerInformedOfAnalysis");

		public static readonly GuidNamePropertyDefinition PstnCallbackTelephoneNumber = InternalSchema.CreateGuidNameProperty("PstnCallbackTelephoneNumber", typeof(string), WellKnownPropertySet.UnifiedMessaging, "PstnCallbackTelephoneNumber");

		public static readonly GuidNamePropertyDefinition UcSubject = InternalSchema.CreateGuidNameProperty("UcSubject", typeof(string), WellKnownPropertySet.UnifiedMessaging, "UcSubject");

		public static readonly GuidNamePropertyDefinition ReceivedSPF = InternalSchema.CreateGuidNameProperty("Received-SPF", typeof(string), WellKnownPropertySet.InternetHeaders, "Received-SPF");

		public static readonly GuidNamePropertyDefinition XCDRDataCallStartTime = InternalSchema.CreateGuidNameProperty("CallStartTime", typeof(ExDateTime), WellKnownPropertySet.UnifiedMessaging, "CallStartTime");

		public static readonly GuidNamePropertyDefinition XCDRDataCallType = InternalSchema.CreateGuidNameProperty("CallType", typeof(string), WellKnownPropertySet.UnifiedMessaging, "CallType");

		public static readonly GuidNamePropertyDefinition XCDRDataCallIdentity = InternalSchema.CreateGuidNameProperty("CallIdentity", typeof(string), WellKnownPropertySet.UnifiedMessaging, "CallIdentity");

		public static readonly GuidNamePropertyDefinition XCDRDataParentCallIdentity = InternalSchema.CreateGuidNameProperty("ParentCallIdentity", typeof(string), WellKnownPropertySet.UnifiedMessaging, "ParentCallIdentity");

		public static readonly GuidNamePropertyDefinition XCDRDataUMServerName = InternalSchema.CreateGuidNameProperty("UMServerName", typeof(string), WellKnownPropertySet.UnifiedMessaging, "UMServerName");

		public static readonly GuidNamePropertyDefinition XCDRDataDialPlanGuid = InternalSchema.CreateGuidNameProperty("DialPlanGuid", typeof(Guid), WellKnownPropertySet.UnifiedMessaging, "DialPlanGuid");

		public static readonly GuidNamePropertyDefinition XCDRDataDialPlanName = InternalSchema.CreateGuidNameProperty("DialPlanName", typeof(string), WellKnownPropertySet.UnifiedMessaging, "DialPlanName");

		public static readonly GuidNamePropertyDefinition XCDRDataCallDuration = InternalSchema.CreateGuidNameProperty("CallDuration", typeof(int), WellKnownPropertySet.UnifiedMessaging, "CallDuration");

		public static readonly GuidNamePropertyDefinition XCDRDataIPGatewayAddress = InternalSchema.CreateGuidNameProperty("IPGatewayAddress", typeof(string), WellKnownPropertySet.UnifiedMessaging, "IPGatewayAddress");

		public static readonly GuidNamePropertyDefinition XCDRDataIPGatewayName = InternalSchema.CreateGuidNameProperty("IPGatewayName", typeof(string), WellKnownPropertySet.UnifiedMessaging, "IPGatewayName");

		public static readonly GuidNamePropertyDefinition XCDRDataGatewayGuid = InternalSchema.CreateGuidNameProperty("GatewayGuid", typeof(Guid), WellKnownPropertySet.UnifiedMessaging, "GatewayGuid");

		public static readonly GuidNamePropertyDefinition XCDRDataCalledPhoneNumber = InternalSchema.CreateGuidNameProperty("CalledPhoneNumber", typeof(string), WellKnownPropertySet.UnifiedMessaging, "CalledPhoneNumber");

		public static readonly GuidNamePropertyDefinition XCDRDataCallerPhoneNumber = InternalSchema.CreateGuidNameProperty("CallerPhoneNumber", typeof(string), WellKnownPropertySet.UnifiedMessaging, "CallerPhoneNumber");

		public static readonly GuidNamePropertyDefinition XCDRDataOfferResult = InternalSchema.CreateGuidNameProperty("OfferResult", typeof(string), WellKnownPropertySet.UnifiedMessaging, "OfferResult");

		public static readonly GuidNamePropertyDefinition XCDRDataDropCallReason = InternalSchema.CreateGuidNameProperty("DropCallReason", typeof(string), WellKnownPropertySet.UnifiedMessaging, "DropCallReason");

		public static readonly GuidNamePropertyDefinition XCDRDataReasonForCall = InternalSchema.CreateGuidNameProperty("ReasonForCall", typeof(string), WellKnownPropertySet.UnifiedMessaging, "ReasonForCall");

		public static readonly GuidNamePropertyDefinition XCDRDataTransferredNumber = InternalSchema.CreateGuidNameProperty("TransferredNumber", typeof(string), WellKnownPropertySet.UnifiedMessaging, "TransferredNumber");

		public static readonly GuidNamePropertyDefinition XCDRDataDialedString = InternalSchema.CreateGuidNameProperty("DialedString", typeof(string), WellKnownPropertySet.UnifiedMessaging, "DialedString");

		public static readonly GuidNamePropertyDefinition XCDRDataCallerMailboxAlias = InternalSchema.CreateGuidNameProperty("CallerMailboxAlias", typeof(string), WellKnownPropertySet.UnifiedMessaging, "CallerMailboxAlias");

		public static readonly GuidNamePropertyDefinition XCDRDataCalleeMailboxAlias = InternalSchema.CreateGuidNameProperty("CalleeMailboxAlias", typeof(string), WellKnownPropertySet.UnifiedMessaging, "CalleeMailboxAlias");

		public static readonly GuidNamePropertyDefinition XCDRDataAutoAttendantName = InternalSchema.CreateGuidNameProperty("AutoAttendantName", typeof(string), WellKnownPropertySet.UnifiedMessaging, "AutoAttendantName");

		public static readonly GuidNamePropertyDefinition XCDRDataAudioCodec = InternalSchema.CreateGuidNameProperty("AudioCodec", typeof(string), WellKnownPropertySet.UnifiedMessaging, "AudioCodec");

		public static readonly GuidNamePropertyDefinition XCDRDataBurstDensity = InternalSchema.CreateGuidNameProperty("BurstDensity", typeof(float), WellKnownPropertySet.UnifiedMessaging, "BurstDensity");

		public static readonly GuidNamePropertyDefinition XCDRDataBurstDuration = InternalSchema.CreateGuidNameProperty("BurstDuration", typeof(float), WellKnownPropertySet.UnifiedMessaging, "BurstDuration");

		public static readonly GuidNamePropertyDefinition XCDRDataJitter = InternalSchema.CreateGuidNameProperty("Jitter", typeof(float), WellKnownPropertySet.UnifiedMessaging, "Jitter");

		public static readonly GuidNamePropertyDefinition XCDRDataNMOS = InternalSchema.CreateGuidNameProperty("NMOS", typeof(float), WellKnownPropertySet.UnifiedMessaging, "NMOS");

		public static readonly GuidNamePropertyDefinition XCDRDataNMOSDegradation = InternalSchema.CreateGuidNameProperty("NMOSDegradation", typeof(float), WellKnownPropertySet.UnifiedMessaging, "NMOSDegradation");

		public static readonly GuidNamePropertyDefinition XCDRDataNMOSDegradationJitter = InternalSchema.CreateGuidNameProperty("NMOSDegradationJitter", typeof(float), WellKnownPropertySet.UnifiedMessaging, "NMOSDegradationJitter");

		public static readonly GuidNamePropertyDefinition XCDRDataNMOSDegradationPacketLoss = InternalSchema.CreateGuidNameProperty("NMOSDegradationPacketLoss", typeof(float), WellKnownPropertySet.UnifiedMessaging, "NMOSDegradationPacketLoss");

		public static readonly GuidNamePropertyDefinition XCDRDataPacketLoss = InternalSchema.CreateGuidNameProperty("PacketLoss", typeof(float), WellKnownPropertySet.UnifiedMessaging, "PacketLoss");

		public static readonly GuidNamePropertyDefinition XCDRDataRoundTrip = InternalSchema.CreateGuidNameProperty("RoundTrip", typeof(float), WellKnownPropertySet.UnifiedMessaging, "RoundTrip");

		public static readonly GuidNamePropertyDefinition AsrData = InternalSchema.CreateGuidNameProperty("AsrData", typeof(byte[]), WellKnownPropertySet.UnifiedMessaging, "AsrData");

		public static readonly PropertyTagPropertyDefinition Flags = PropertyTagPropertyDefinition.InternalCreate("Flags", PropTag.MessageFlags);

		public static readonly PropertyTagPropertyDefinition MessageStatus = PropertyTagPropertyDefinition.InternalCreate("MessageStatus", PropTag.MsgStatus);

		public static readonly PropertyTagPropertyDefinition ItemTemporaryFlag = PropertyTagPropertyDefinition.InternalCreate("PR_ITEM_TMPFLAGS", PropTag.ItemTemporaryFlag);

		public static readonly PropertyTagPropertyDefinition DeliverAsRead = PropertyTagPropertyDefinition.InternalCreate("DeliverAsRead", PropTag.DeliverAsRead);

		public static readonly GuidNamePropertyDefinition ContentClass = InternalSchema.CreateGuidNameProperty("ContentClass", typeof(string), WellKnownPropertySet.InternetHeaders, "content-class");

		public static readonly GuidNamePropertyDefinition AttachmentMacInfo = InternalSchema.CreateGuidNameProperty("AttachmentMacInfo", typeof(byte[]), WellKnownPropertySet.Attachment, "AttachmentMacInfo", PropertyFlags.Streamable, PropertyDefinitionConstraint.None);

		public static readonly GuidNamePropertyDefinition AttachmentMacContentType = InternalSchema.CreateGuidNameProperty("AttachmentMacContentType", typeof(string), WellKnownPropertySet.Attachment, "AttachmentMacContentType");

		public static readonly GuidNamePropertyDefinition AttachmentProviderEndpointUrl = InternalSchema.CreateGuidNameProperty("AttachmentProviderEndpointUrl", typeof(string), WellKnownPropertySet.Attachment, "AttachmentProviderEndpointUrl");

		public static readonly GuidNamePropertyDefinition AttachmentProviderType = InternalSchema.CreateGuidNameProperty("AttachmentProviderType", typeof(string), WellKnownPropertySet.Attachment, "AttachmentProviderType");

		public static readonly PropertyTagPropertyDefinition ConversationIndex = PropertyTagPropertyDefinition.InternalCreate("ConversationIndex", PropTag.ConversationIndex);

		public static readonly PropertyTagPropertyDefinition ConversationTopic = PropertyTagPropertyDefinition.InternalCreate("ConversationTopic", PropTag.ConversationTopic);

		public static readonly PropertyTagPropertyDefinition ConversationTopicHash = PropertyTagPropertyDefinition.InternalCreate("ConversationTopicHash", PropTag.ConversationTopicHash);

		public static readonly PropertyTagPropertyDefinition IsDeliveryReceiptRequestedInternal = PropertyTagPropertyDefinition.InternalCreate("IsDeliveryReceiptRequestedInternal", PropTag.OriginatorDeliveryReportRequested);

		public static readonly PropertyTagPropertyDefinition IsNonDeliveryReceiptRequestedInternal = PropertyTagPropertyDefinition.InternalCreate("IsNonDeliveryReceiptRequestedInternal", PropTag.OriginatorNonDeliveryReportRequested);

		public static readonly PropertyTagPropertyDefinition DisplayCcInternal = PropertyTagPropertyDefinition.InternalCreate("DisplayCc", PropTag.DisplayCc, PropertyFlags.ReadOnly | PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition DisplayToInternal = PropertyTagPropertyDefinition.InternalCreate("DisplayToInternal", PropTag.DisplayTo, PropertyFlags.ReadOnly | PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition MessageToMe = PropertyTagPropertyDefinition.InternalCreate("MessageToMe", PropTag.MessageToMe);

		public static readonly PropertyTagPropertyDefinition MessageCcMe = PropertyTagPropertyDefinition.InternalCreate("MessageCcMe", PropTag.MessageCcMe);

		public static readonly PropertyTagPropertyDefinition DisplayBccInternal = PropertyTagPropertyDefinition.InternalCreate("DisplayBccInternal", PropTag.DisplayBcc, PropertyFlags.ReadOnly | PropertyFlags.Streamable);

		public static readonly GuidIdPropertyDefinition DisplayAttendeesAll = InternalSchema.CreateGuidIdProperty("DisplayAttendeesAll", typeof(string), WellKnownPropertySet.Appointment, 33336, PropertyFlags.Streamable, PropertyDefinitionConstraint.None);

		public static readonly GuidIdPropertyDefinition DisplayAttendeesTo = InternalSchema.CreateGuidIdProperty("DisplayAttendeesTo", typeof(string), WellKnownPropertySet.Appointment, 33339, PropertyFlags.Streamable, PropertyDefinitionConstraint.None);

		public static readonly GuidIdPropertyDefinition DisplayAttendeesCc = InternalSchema.CreateGuidIdProperty("DisplayAttendeesCc", typeof(string), WellKnownPropertySet.Appointment, 33340, PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly PropertyTagPropertyDefinition ParentDisplayName = PropertyTagPropertyDefinition.InternalCreate("ParentDisplay", PropTag.ParentDisplay);

		public static readonly GuidIdPropertyDefinition FlagRequest = InternalSchema.CreateGuidIdProperty("FlagRequest", typeof(string), WellKnownPropertySet.Common, 34096);

		public static readonly GuidIdPropertyDefinition RequestedAction = InternalSchema.CreateGuidIdProperty("FlagStringEnum", typeof(int), WellKnownPropertySet.Common, 34240);

		public static readonly GuidIdPropertyDefinition InfoPathFormName = InternalSchema.CreateGuidIdProperty("InfoPathFormName", typeof(string), WellKnownPropertySet.Common, 34225);

		public static readonly PropertyTagPropertyDefinition InReplyTo = PropertyTagPropertyDefinition.InternalCreate("PR_IN_REPLY_TO_ID", (PropTag)272760863U);

		public static readonly PropertyTagPropertyDefinition MapiInternetCpid = PropertyTagPropertyDefinition.InternalCreate("MapiInternetCpid", PropTag.InternetCPID);

		internal static readonly PropertyTagPropertyDefinition TextAttachmentCharset = PropertyTagPropertyDefinition.InternalCreate("TextAttachmentCharset", (PropTag)924516383U);

		public static readonly PropertyTagPropertyDefinition InternetMessageId = PropertyTagPropertyDefinition.InternalCreate("InternetMessageId", PropTag.InternetMessageId);

		public static readonly PropertyTagPropertyDefinition InternetMessageIdHash = PropertyTagPropertyDefinition.InternalCreate("InternetMessageIdHash", PropTag.InternetMessageIdHash);

		public static readonly PropertyTagPropertyDefinition Preview = PropertyTagPropertyDefinition.InternalCreate("Preview", PropTag.Preview);

		public static readonly PropertyTagPropertyDefinition InternetReferences = PropertyTagPropertyDefinition.InternalCreate("PR_INTERNET_REFERENCES", (PropTag)272171039U);

		public static readonly PropertyTagPropertyDefinition IsAutoForwarded = PropertyTagPropertyDefinition.InternalCreate("IsAutoForwarded", PropTag.AutoForwarded);

		public static readonly PropertyTagPropertyDefinition IsReplyRequested = PropertyTagPropertyDefinition.InternalCreate("IsReplyRequested", PropTag.ReplyRequested);

		public static readonly PropertyTagPropertyDefinition IsResponseRequested = PropertyTagPropertyDefinition.InternalCreate("IsResponseRequested", PropTag.ResponseRequested);

		public static readonly PropertyTagPropertyDefinition IsReadReceiptRequestedInternal = PropertyTagPropertyDefinition.InternalCreate("IsReadReceiptRequestedInternal", PropTag.ReadReceiptRequested);

		public static readonly PropertyTagPropertyDefinition IsNotReadReceiptRequestedInternal = PropertyTagPropertyDefinition.InternalCreate("IsNotReadReceiptRequestedInternal", PropTag.NonReceiptNotificationRequested);

		public static readonly PropertyTagPropertyDefinition ListHelp = PropertyTagPropertyDefinition.InternalCreate("ListHelp", (PropTag)272826399U);

		public static readonly PropertyTagPropertyDefinition ListSubscribe = PropertyTagPropertyDefinition.InternalCreate("ListSubscribe", (PropTag)272891935U);

		public static readonly PropertyTagPropertyDefinition ListUnsubscribe = PropertyTagPropertyDefinition.InternalCreate("ListUnsubscribe", (PropTag)272957471U);

		public static readonly PropertyTagPropertyDefinition MapiHasAttachment = PropertyTagPropertyDefinition.InternalCreate("MapiHasAttachment", PropTag.Hasattach, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition MapiPriority = PropertyTagPropertyDefinition.InternalCreate("MapiPriority", PropTag.Priority);

		public static readonly PropertyTagPropertyDefinition MapiReplyToBlob = PropertyTagPropertyDefinition.InternalCreate("ReplyTo", PropTag.ReplyRecipientEntries, new PropertyDefinitionConstraint[]
		{
			new StoreByteArrayLengthConstraint(32767)
		});

		public static readonly PropertyTagPropertyDefinition MapiReplyToNames = PropertyTagPropertyDefinition.InternalCreate("ReplyToNames", PropTag.ReplyRecipientNames);

		public static readonly GuidNamePropertyDefinition MapiLikersBlob = InternalSchema.CreateGuidNameProperty("Likers", typeof(byte[]), WellKnownPropertySet.Messaging, "Likers");

		public static readonly PropertyTagPropertyDefinition MapiLikeCount = PropertyTagPropertyDefinition.InternalCreate("LikeCount", PropTag.LikeCount);

		public static readonly PropertyTagPropertyDefinition PeopleCentricConversationId = PropertyTagPropertyDefinition.InternalCreate("PeopleCentricConversationId", PropTag.PeopleCentricConversationId);

		public static readonly PropertyTagPropertyDefinition MessageRecipients = PropertyTagPropertyDefinition.InternalCreate("MessageRecipients", PropTag.MessageRecipients);

		public static readonly PropertyTagPropertyDefinition MID = PropertyTagPropertyDefinition.InternalCreate("MID", PropTag.Mid);

		public static readonly PropertyTagPropertyDefinition LTID = PropertyTagPropertyDefinition.InternalCreate("LTID", PropTag.LTID);

		public static readonly PropertyTagPropertyDefinition MappingSignature = PropertyTagPropertyDefinition.InternalCreate("MappingSignature", PropTag.MappingSignature);

		public static readonly PropertyTagPropertyDefinition MdbProvider = PropertyTagPropertyDefinition.InternalCreate("MdbProvider", PropTag.MdbProvider);

		public static readonly PropertyTagPropertyDefinition RuleTriggerHistory = PropertyTagPropertyDefinition.InternalCreate("RuleTriggerHistory", (PropTag)1072824578U);

		public static readonly PropertyTagPropertyDefinition RuleError = PropertyTagPropertyDefinition.InternalCreate("RuleError", PropTag.RuleError);

		public static readonly PropertyTagPropertyDefinition RuleActionType = PropertyTagPropertyDefinition.InternalCreate("RuleActionType", PropTag.RuleActionType);

		public static readonly PropertyTagPropertyDefinition RuleActionNumber = PropertyTagPropertyDefinition.InternalCreate("RuleActionNumber", PropTag.RuleActionNumber);

		public static readonly PropertyTagPropertyDefinition RuleId = PropertyTagPropertyDefinition.InternalCreate("RuleId", PropTag.RuleID);

		public static readonly PropertyTagPropertyDefinition RuleIds = PropertyTagPropertyDefinition.InternalCreate("RuleIds", PropTag.RuleIDs);

		public static readonly PropertyTagPropertyDefinition DelegatedByRule = PropertyTagPropertyDefinition.InternalCreate("DelegatedByRule", (PropTag)1071841291U);

		public static readonly PropertyTagPropertyDefinition RuleFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("RuleFolderEntryId", PropTag.RuleFolderEntryID);

		public static readonly PropertyTagPropertyDefinition RuleProvider = PropertyTagPropertyDefinition.InternalCreate("RuleProvider", PropTag.RuleProvider);

		public static readonly PropertyTagPropertyDefinition ClientActions = PropertyTagPropertyDefinition.InternalCreate("ClientActions", PropTag.PromotedProperties);

		public static readonly PropertyTagPropertyDefinition DeferredActionMessageBackPatched = PropertyTagPropertyDefinition.InternalCreate("DeferredActionMessageBackPatched", PropTag.DeferredActionMessageBackPatched);

		public static readonly PropertyTagPropertyDefinition HasDeferredActionMessage = PropertyTagPropertyDefinition.InternalCreate("HasDeferredActionMessage", PropTag.HasDeferredActionMessage);

		public static readonly PropertyTagPropertyDefinition MoveToFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("MoveToFolderEntryId", (PropTag)1072955650U);

		public static readonly PropertyTagPropertyDefinition MoveToStoreEntryId = PropertyTagPropertyDefinition.InternalCreate("MoveToStoreEntryId", (PropTag)1072890114U);

		public static readonly PropertyTagPropertyDefinition OriginalMessageEntryId = PropertyTagPropertyDefinition.InternalCreate("OriginalMessageEntryId", (PropTag)1715863810U);

		public static readonly PropertyTagPropertyDefinition OriginalMessageSvrEId = PropertyTagPropertyDefinition.InternalCreate("OriginalMessageSvrEId", (PropTag)1732313346U);

		public static readonly PropertyTagPropertyDefinition NormalizedSubjectInternal = PropertyTagPropertyDefinition.InternalCreate("NormalizedSubjectInternal", PropTag.NormalizedSubject);

		public static readonly PropertyTagPropertyDefinition ReceivedTime = PropertyTagPropertyDefinition.InternalCreate("ReceivedTime", PropTag.MessageDeliveryTime);

		public static readonly PropertyTagPropertyDefinition RenewTime = PropertyTagPropertyDefinition.InternalCreate("RenewTime", PropTag.RenewTime);

		public static readonly PropertyTagPropertyDefinition ReceivedOrRenewTime = PropertyTagPropertyDefinition.InternalCreate("ReceivedOrRenewTime", PropTag.DeliveryOrRenewTime);

		public static readonly PropertyTagPropertyDefinition MailboxGuidGuid = PropertyTagPropertyDefinition.InternalCreate("MailboxGuidGuid", PropTag.MailboxDSGuidGuid);

		public static readonly PropertyTagPropertyDefinition MailboxPartitionMailboxGuids = PropertyTagPropertyDefinition.InternalCreate("MailboxPartitionMailboxGuids", PropTag.MailboxPartitionMailboxGuids);

		public static readonly PropertyTagPropertyDefinition DeferredDeliveryTime = PropertyTagPropertyDefinition.InternalCreate("DeferredDeliveryTime", PropTag.DeferredDeliveryTime);

		public static readonly PropertyTagPropertyDefinition DeferredSendTime = PropertyTagPropertyDefinition.InternalCreate("DeferredSendTime", PropTag.DeferredSendTime);

		public static readonly PropertyTagPropertyDefinition ReplyTime = PropertyTagPropertyDefinition.InternalCreate("PR_REPLY_TIME", PropTag.ReplyTime);

		public static readonly PropertyTagPropertyDefinition SenderAddressType = PropertyTagPropertyDefinition.InternalCreate("SenderAddressType", PropTag.SenderAddrType, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly PropertyTagPropertyDefinition SenderDisplayName = PropertyTagPropertyDefinition.InternalCreate("SenderDisplayName", PropTag.SenderName);

		public static readonly PropertyTagPropertyDefinition SenderEmailAddress = PropertyTagPropertyDefinition.InternalCreate("SenderEmailAddress", PropTag.SenderEmailAddress);

		public static readonly PropertyTagPropertyDefinition SenderEntryId = PropertyTagPropertyDefinition.InternalCreate("SenderEntryId", PropTag.SenderEntryId);

		public static readonly PropertyTagPropertyDefinition SenderSearchKey = PropertyTagPropertyDefinition.InternalCreate("SenderSearchKey", PropTag.SenderSearchKey);

		public static readonly PropertyTagPropertyDefinition SenderIdStatus = PropertyTagPropertyDefinition.InternalCreate("SenderIdStatus", PropTag.SenderIdStatus);

		public static readonly PropertyTagPropertyDefinition SentMailEntryId = PropertyTagPropertyDefinition.InternalCreate("SentMailEntryId", PropTag.SentMailEntryId);

		public static readonly PropertyTagPropertyDefinition SenderSID = PropertyTagPropertyDefinition.InternalCreate("SenderSID", PropTag.SenderSID);

		public static readonly PropertyTagPropertyDefinition ConversationCreatorSID = PropertyTagPropertyDefinition.InternalCreate("ConversationCreatorSID", (PropTag)240845058U);

		public static readonly PropertyTagPropertyDefinition SenderGuid = PropertyTagPropertyDefinition.InternalCreate("SenderGuid", PropTag.SenderGuid);

		public static readonly PropertyTagPropertyDefinition SentRepresentingDisplayName = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingDisplayName", PropTag.SentRepresentingName);

		public static readonly PropertyTagPropertyDefinition SentRepresentingEmailAddress = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingEmailAddress", PropTag.SentRepresentingEmailAddress);

		public static readonly PropertyTagPropertyDefinition SentRepresentingEntryId = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingEntryId", PropTag.SentRepresentingEntryId);

		public static readonly PropertyTagPropertyDefinition SentRepresentingSearchKey = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingSearchKey", PropTag.SentRepresentingSearchKey);

		public static readonly PropertyTagPropertyDefinition SentRepresentingSimpleDisplayName = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingSimpleDisplayName", (PropTag)1076953119U);

		public static readonly PropertyTagPropertyDefinition SentRepresentingOrgAddressType = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingOrgAddressType", (PropTag)1078001695U);

		public static readonly PropertyTagPropertyDefinition SentRepresentingOrgEmailAddr = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingOrgEmailAddr", (PropTag)1078067231U);

		public static readonly PropertyTagPropertyDefinition SentRepresentingSID = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingSID", (PropTag)239993090U);

		public static readonly PropertyTagPropertyDefinition SentRepresentingGuid = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingGuid", (PropTag)239141122U);

		public static readonly PropertyTagPropertyDefinition SenderFlags = PropertyTagPropertyDefinition.InternalCreate("SenderFlags", (PropTag)1075380227U);

		public static readonly PropertyTagPropertyDefinition SentRepresentingFlags = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingFlags", (PropTag)1075445763U);

		public static readonly PropertyTagPropertyDefinition SentRepresentingType = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingType", PropTag.SentRepresentingAddrType, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly PropertyTagPropertyDefinition MessageSubmissionId = PropertyTagPropertyDefinition.InternalCreate("MessageSubmissionId", PropTag.MessageSubmissionId);

		public static readonly PropertyTagPropertyDefinition ProviderSubmitTime = PropertyTagPropertyDefinition.InternalCreate("ProviderSubmitTime", PropTag.ProviderSubmitTime);

		public static readonly PropertyTagPropertyDefinition SentTime = PropertyTagPropertyDefinition.InternalCreate("SentTime", PropTag.ClientSubmitTime);

		public static readonly PropertyTagPropertyDefinition SentMailSvrEId = PropertyTagPropertyDefinition.InternalCreate("SentMailSvrEId", (PropTag)1732247810U);

		public static readonly PropertyTagPropertyDefinition SubjectPrefixInternal = PropertyTagPropertyDefinition.InternalCreate("SubjectPrefixInternal", PropTag.SubjectPrefix);

		public static readonly PropertyTagPropertyDefinition MapiSubject = PropertyTagPropertyDefinition.InternalCreate("MapiSubject", PropTag.Subject);

		public static readonly PropertyTagPropertyDefinition TnefCorrelationKey = PropertyTagPropertyDefinition.InternalCreate("TnefCorrelationKey", PropTag.TnefCorrelationKey);

		public static readonly PropertyTagPropertyDefinition TransportMessageHeaders = PropertyTagPropertyDefinition.InternalCreate("TransportMessageHeaders", PropTag.TransportMessageHeaders, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition ParticipantSID = PropertyTagPropertyDefinition.InternalCreate("ParticipantSID", PropTag.ParticipantSID);

		public static readonly PropertyTagPropertyDefinition ParticipantGuid = PropertyTagPropertyDefinition.InternalCreate("ParticipantGuid", PropTag.ParticipantGuid);

		public static readonly PropertyTagPropertyDefinition DeleteAfterSubmit = PropertyTagPropertyDefinition.InternalCreate("DeleteAfterSubmit", PropTag.DeleteAfterSubmit);

		public static readonly PropertyTagPropertyDefinition TargetEntryId = PropertyTagPropertyDefinition.InternalCreate("TargetEntryId", PropTag.TargetEntryId);

		public static readonly PropertyTagPropertyDefinition MapiFlagStatus = PropertyTagPropertyDefinition.InternalCreate("PR_FLAG_STATUS", (PropTag)277872643U);

		public static readonly PropertyTagPropertyDefinition Associated = PropertyTagPropertyDefinition.InternalCreate("Associated", PropTag.Associated);

		public static readonly PropertyTagPropertyDefinition IconIndex = PropertyTagPropertyDefinition.InternalCreate("IconIndex", (PropTag)276824067U);

		public static readonly PropertyTagPropertyDefinition LastVerbExecuted = PropertyTagPropertyDefinition.InternalCreate("PR_LAST_VERB_EXECUTED", (PropTag)276889603U);

		public static readonly PropertyTagPropertyDefinition LastVerbExecutionTime = PropertyTagPropertyDefinition.InternalCreate("PR_LAST_VERB_EXECUTIONTIME", (PropTag)276955200U);

		public static readonly PropertyTagPropertyDefinition ReplyForwardStatus = PropertyTagPropertyDefinition.InternalCreate("ReplyForwardStatus", (PropTag)2081095711U);

		public static readonly PropertyTagPropertyDefinition PopImapPoisonMessageStamp = PropertyTagPropertyDefinition.InternalCreate("PopImapPoisonMessageStamp", (PropTag)2081161247U);

		public static readonly GuidNamePropertyDefinition PopImapConversionVersion = InternalSchema.CreateGuidNameProperty("PopImapConversionVersion", typeof(string), WellKnownPropertySet.IMAPMsg, "PopImap:PopImapConversionVersion");

		public static readonly GuidNamePropertyDefinition PopMIMESize = InternalSchema.CreateGuidNameProperty("PopMIMESize", typeof(long), WellKnownPropertySet.IMAPMsg, "PopImap:PopMIMESize");

		public static readonly GuidNamePropertyDefinition PopMIMEOptions = InternalSchema.CreateGuidNameProperty("PopMIMEOptions", typeof(long), WellKnownPropertySet.IMAPMsg, "PopImap:PopMIMEOptions");

		public static readonly GuidNamePropertyDefinition ImapMIMESize = InternalSchema.CreateGuidNameProperty("ImapMIMESize", typeof(long), WellKnownPropertySet.IMAPMsg, "PopImap:ImapMIMESize");

		public static readonly GuidNamePropertyDefinition ImapMIMEOptions = InternalSchema.CreateGuidNameProperty("ImapMIMEOptions", typeof(long), WellKnownPropertySet.IMAPMsg, "PopImap:ImapMIMEOptions");

		public static readonly GuidNamePropertyDefinition ImapLastUidFixTime = InternalSchema.CreateGuidNameProperty("ImapLastUidFixTime", typeof(ExDateTime), WellKnownPropertySet.IMAPMsg, "PopImap:ImapLastUidFixTime");

		public static readonly PropertyTagPropertyDefinition ExpiryTime = PropertyTagPropertyDefinition.InternalCreate("ExpiryTime", PropTag.ExpiryTime);

		public static readonly PropertyTagPropertyDefinition IsHidden = PropertyTagPropertyDefinition.InternalCreate("IsHidden", (PropTag)284426251U);

		public static readonly PropertyTagPropertyDefinition SystemFolderFlags = PropertyTagPropertyDefinition.InternalCreate("SystemFolderFlags", PropTag.SystemFolderFlags);

		public static readonly PropertyTagPropertyDefinition LocallyDelivered = PropertyTagPropertyDefinition.InternalCreate("LocallyDelivered", (PropTag)1732575490U);

		public static readonly GuidNamePropertyDefinition XLoop = InternalSchema.CreateGuidNameProperty("XLoop", typeof(string[]), WellKnownPropertySet.Messaging, "XLoop");

		public static readonly GuidNamePropertyDefinition DoNotDeliver = InternalSchema.CreateGuidNameProperty("DoNotDeliver", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-Test-Message");

		public static readonly GuidNamePropertyDefinition DropMessageInHub = InternalSchema.CreateGuidNameProperty("DropMessageInHub", typeof(string), WellKnownPropertySet.InternetHeaders, "X-Exchange-Probe-Drop-Message");

		public static readonly GuidNamePropertyDefinition SystemProbeDrop = InternalSchema.CreateGuidNameProperty("SystemProbeDrop", typeof(string), WellKnownPropertySet.InternetHeaders, "X-Exchange-System-Probe-Drop");

		public static readonly GuidNamePropertyDefinition XLAMNotificationId = InternalSchema.CreateGuidNameProperty("XLAMNotificationId", typeof(string), WellKnownPropertySet.InternetHeaders, "X-LAMNotificationId");

		public static readonly GuidNamePropertyDefinition MapiSubmitLamNotificationId = InternalSchema.CreateGuidNameProperty("MapiSubmitLamNotificationId", typeof(string), WellKnownPropertySet.Messaging, "MapiSubmitLamNotificationId");

		public static readonly GuidNamePropertyDefinition MapiSubmitSystemProbeActivityId = InternalSchema.CreateGuidNameProperty("MapiSubmitSystemProbeActivityId", typeof(Guid), WellKnownPropertySet.Messaging, "MapiSubmitSystemProbeActivityId");

		public static readonly GuidNamePropertyDefinition ResentFrom = InternalSchema.CreateGuidNameProperty("ResentFrom", typeof(string), WellKnownPropertySet.InternetHeaders, "resent-from");

		public static readonly GuidNamePropertyDefinition OriginalSentTimeForEscalation = InternalSchema.CreateGuidNameProperty("OriginalSentTimeForEscalation", typeof(ExDateTime), WellKnownPropertySet.Messaging, "OriginalSentTimeForEscalation");

		public static readonly PropertyTagPropertyDefinition MessageAnnotation = PropertyTagPropertyDefinition.InternalCreate("MessageAnnotation", PropTag.MessageAnnotation);

		public static readonly PropertyTagPropertyDefinition OriginalAuthorName = PropertyTagPropertyDefinition.InternalCreate("OriginalAuthorName", PropTag.OriginalAuthorName);

		public static readonly PropertyTagPropertyDefinition OriginalSensitivity = PropertyTagPropertyDefinition.InternalCreate("OriginalSensitivity", PropTag.OriginalSensitivity);

		public static readonly PropertyTagPropertyDefinition ReceivedRepresentingAddressType = PropertyTagPropertyDefinition.InternalCreate("ReceivedRepresentingAddressType", PropTag.RcvdRepresentingAddrType, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly PropertyTagPropertyDefinition ReceivedRepresentingDisplayName = PropertyTagPropertyDefinition.InternalCreate("ReceivedRepresentingDisplayName", PropTag.RcvdRepresentingName);

		public static readonly PropertyTagPropertyDefinition ReceivedRepresentingEmailAddress = PropertyTagPropertyDefinition.InternalCreate("ReceivedRepresentingEmailAddress", PropTag.RcvdRepresentingEmailAddress);

		public static readonly PropertyTagPropertyDefinition ReceivedRepresentingSearchKey = PropertyTagPropertyDefinition.InternalCreate("ReceivedRepresentingSearchKey", PropTag.RcvdRepresentingSearchKey);

		public static readonly PropertyTagPropertyDefinition ReceivedRepresentingEntryId = PropertyTagPropertyDefinition.InternalCreate("ReceivedRepresentingEntryId", PropTag.RcvdRepresentingEntryId);

		public static readonly PropertyTagPropertyDefinition EventEmailReminderTimer = PropertyTagPropertyDefinition.InternalCreate("EventEmailReminderTimer", PropTag.EventEmailReminderTimer);

		public static readonly PropertyTagPropertyDefinition AttachCalendarFlags = PropertyTagPropertyDefinition.InternalCreate("AttachFlags", (PropTag)2147287043U);

		public static readonly PropertyTagPropertyDefinition AttachCalendarHidden = PropertyTagPropertyDefinition.InternalCreate("AttachHidden", (PropTag)2147352587U);

		public static readonly PropertyTagPropertyDefinition AttachCalendarLinkId = PropertyTagPropertyDefinition.InternalCreate("AttachLinkId", (PropTag)2147090435U);

		public static readonly PropertyTagPropertyDefinition AttachContentBase = PropertyTagPropertyDefinition.InternalCreate("AttachContentBase", (PropTag)923861023U);

		public static readonly PropertyTagPropertyDefinition AttachContentId = PropertyTagPropertyDefinition.InternalCreate("AttachContentId", (PropTag)923926559U);

		public static readonly PropertyTagPropertyDefinition AttachContentLocation = PropertyTagPropertyDefinition.InternalCreate("AttachContentLocation", PropTag.AttachContentLocation);

		public static readonly PropertyTagPropertyDefinition AttachEncoding = PropertyTagPropertyDefinition.InternalCreate("AttachEncoding", PropTag.AttachEncoding);

		public static readonly PropertyTagPropertyDefinition AttachExtension = PropertyTagPropertyDefinition.InternalCreate("AttachExtension", PropTag.AttachExtension);

		public static readonly PropertyTagPropertyDefinition AttachFileName = PropertyTagPropertyDefinition.InternalCreate("AttachFileName", PropTag.AttachFileName);

		public static readonly PropertyTagPropertyDefinition AttachLongFileName = PropertyTagPropertyDefinition.InternalCreate("AttachLongFileName", PropTag.AttachLongFileName);

		public static readonly PropertyTagPropertyDefinition AttachLongPathName = PropertyTagPropertyDefinition.InternalCreate("AttachLongPathName", PropTag.AttachLongPathName);

		public static readonly PropertyTagPropertyDefinition AttachAdditionalInfo = PropertyTagPropertyDefinition.InternalCreate("AttachAdditionalInfo", PropTag.AttachAdditionalInfo);

		public static readonly PropertyTagPropertyDefinition AttachMethod = PropertyTagPropertyDefinition.InternalCreate("AttachMethod", PropTag.AttachMethod);

		public static readonly PropertyTagPropertyDefinition AttachMhtmlFlags = PropertyTagPropertyDefinition.InternalCreate("AttachMhtmlFlags", PropTag.AttachFlags);

		public static readonly PropertyTagPropertyDefinition AttachMimeTag = PropertyTagPropertyDefinition.InternalCreate("AttachMimeTag", PropTag.AttachMimeTag);

		public static readonly PropertyTagPropertyDefinition AttachNum = PropertyTagPropertyDefinition.InternalCreate("AttachNum", PropTag.AttachNum);

		public static readonly PropertyTagPropertyDefinition AttachInConflict = PropertyTagPropertyDefinition.InternalCreate("AttachInConflict", (PropTag)1718353931U);

		public static readonly GuidIdPropertyDefinition SideEffects = InternalSchema.CreateGuidIdProperty("SideEffects", typeof(int), WellKnownPropertySet.Common, 34064);

		public static readonly PropertyTagPropertyDefinition AttachSize = PropertyTagPropertyDefinition.InternalCreate("AttachSize", PropTag.AttachSize);

		public static readonly PropertyTagPropertyDefinition AttachTransportName = PropertyTagPropertyDefinition.InternalCreate("AttachTransportName", PropTag.AttachTransportName);

		[MessageClassSpecific("FixMe")]
		public static readonly PropertyTagPropertyDefinition AppointmentTombstones = PropertyTagPropertyDefinition.InternalCreate("PR_SCHDINFO_APPT_TOMBSTONE", (PropTag)1751777538U);

		public static readonly GuidIdPropertyDefinition AllAttachmentsHidden = InternalSchema.CreateGuidIdProperty("AllAttachmentsHidden", typeof(bool), WellKnownPropertySet.Common, 34068);

		public static readonly PropertyTagPropertyDefinition Not822Renderable = PropertyTagPropertyDefinition.InternalCreate("Not822Renderable", (PropTag)1733492747U);

		public static readonly PropertyTagPropertyDefinition RenderingPosition = PropertyTagPropertyDefinition.InternalCreate("RenderingPosition", PropTag.RenderingPosition);

		public static readonly GuidNamePropertyDefinition NamedContentType = InternalSchema.CreateGuidNameProperty("NamedContentType", typeof(string), WellKnownPropertySet.InternetHeaders, "content-type");

		public static readonly PropertyTagPropertyDefinition FailedInboundICalAsAttachment = PropertyTagPropertyDefinition.InternalCreate("FailedInboundICalAsAttachment", (PropTag)924581899U);

		public static readonly GuidIdPropertyDefinition FreeBusyStatus = InternalSchema.CreateGuidIdProperty("FreeBusyStatus", typeof(int), WellKnownPropertySet.Appointment, 33285);

		public static readonly GuidIdPropertyDefinition ResponseState = InternalSchema.CreateGuidIdProperty("ResponseState", typeof(int), WellKnownPropertySet.Meeting, 33);

		public static readonly GuidIdPropertyDefinition MapiResponseType = InternalSchema.CreateGuidIdProperty("ResponseType", typeof(int), WellKnownPropertySet.Appointment, 33304);

		public static readonly GuidNamePropertyDefinition BodyContentBase = InternalSchema.CreateGuidNameProperty("BodyContentBase", typeof(string), WellKnownPropertySet.InternetHeaders, "Content-Base");

		public static readonly PropertyTagPropertyDefinition BodyContentId = PropertyTagPropertyDefinition.InternalCreate("BodyContentId", (PropTag)269811743U);

		public static readonly PropertyTagPropertyDefinition BodyContentLocation = PropertyTagPropertyDefinition.InternalCreate("BodyContentLocation", (PropTag)269746207U);

		public static readonly PropertyTagPropertyDefinition Codepage = PropertyTagPropertyDefinition.InternalCreate("Codepage", (PropTag)1073545219U, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 65535)
		});

		public static readonly PropertyTagPropertyDefinition HtmlBody = PropertyTagPropertyDefinition.InternalCreate("HtmlBody", PropTag.Html, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition RtfBody = PropertyTagPropertyDefinition.InternalCreate("RtfBody", PropTag.RtfCompressed, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition RtfInSync = PropertyTagPropertyDefinition.InternalCreate("RtfInSync", PropTag.RtfInSync);

		public static readonly PropertyTagPropertyDefinition RtfSyncBodyCount = PropertyTagPropertyDefinition.InternalCreate("RtfSyncBodyCount", PropTag.RtfSyncBodyCount);

		public static readonly PropertyTagPropertyDefinition RtfSyncBodyCrc = PropertyTagPropertyDefinition.InternalCreate("RtfSyncBodyCrc", PropTag.RtfSyncBodyCrc);

		public static readonly PropertyTagPropertyDefinition RtfSyncBodyTag = PropertyTagPropertyDefinition.InternalCreate("RtfSyncBodyTag", PropTag.RtfSyncBodyTag);

		public static readonly PropertyTagPropertyDefinition RtfSyncPrefixCount = PropertyTagPropertyDefinition.InternalCreate("RtfSyncPrefixCount", PropTag.RtfSyncPrefixCount);

		public static readonly PropertyTagPropertyDefinition RtfSyncTrailingCount = PropertyTagPropertyDefinition.InternalCreate("RtfSyncTrailingCount", PropTag.RtfSyncTrailingCount);

		public static readonly PropertyTagPropertyDefinition TextBody = PropertyTagPropertyDefinition.InternalCreate("TextBody", PropTag.Body, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition NativeBodyInfo = PropertyTagPropertyDefinition.InternalCreate("NativeBodyInfo", PropTag.NativeBodyInfo, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition SendOutlookRecallReport = PropertyTagPropertyDefinition.InternalCreate("SendOutlookRecallReport", (PropTag)1745027083U);

		internal static readonly PropertyTagPropertyDefinition InternalAccess = PropertyTagPropertyDefinition.InternalCreate("InternalAccess", PropTag.InternalAccess);

		public static readonly PropertyTagPropertyDefinition RawFreeBusySecurityDescriptor = PropertyTagPropertyDefinition.InternalCreate("FreeBusySecurityDescriptor", PropTag.FreeBusyNTSD, PropertyFlags.Streamable);

		public static readonly StorePropertyDefinition FreeBusySecurityDescriptor = new SecurityDescriptorProperty(InternalSchema.RawFreeBusySecurityDescriptor);

		public static readonly GuidNamePropertyDefinition BirthdayCalendarFolderEntryId = InternalSchema.CreateGuidNameProperty("BirthdayCalendarFolderEntryId", typeof(byte[]), WellKnownPropertySet.Common, "BirthdayCalendarFolderEntryId");

		public static readonly GuidIdPropertyDefinition ReminderDueByInternal = InternalSchema.CreateGuidIdProperty("ReminderDueByInternal", typeof(ExDateTime), WellKnownPropertySet.Common, 34050);

		public static readonly GuidIdPropertyDefinition ReminderIsSetInternal = InternalSchema.CreateGuidIdProperty("ReminderIsSetInternal", typeof(bool), WellKnownPropertySet.Common, 34051);

		public static readonly GuidIdPropertyDefinition NonSendableTo = InternalSchema.CreateGuidIdProperty("NonSendableTo", typeof(string), WellKnownPropertySet.Common, 34102);

		public static readonly GuidIdPropertyDefinition NonSendableCC = InternalSchema.CreateGuidIdProperty("NonSendableCC", typeof(string), WellKnownPropertySet.Common, 34103);

		public static readonly GuidIdPropertyDefinition NonSendableBCC = InternalSchema.CreateGuidIdProperty("NonSendableBCC", typeof(string), WellKnownPropertySet.Common, 34104);

		public static readonly GuidIdPropertyDefinition NonSendToTrackStatus = InternalSchema.CreateGuidIdProperty("NonSendToTrackStatus", typeof(int[]), WellKnownPropertySet.Common, 34115);

		public static readonly GuidIdPropertyDefinition NonSendCCTrackStatus = InternalSchema.CreateGuidIdProperty("NonSendCCTrackStatus", typeof(int[]), WellKnownPropertySet.Common, 34116);

		public static readonly GuidIdPropertyDefinition NonSendBCCTrackStatus = InternalSchema.CreateGuidIdProperty("NonSendBCCTrackStatus", typeof(int[]), WellKnownPropertySet.Common, 34117);

		public static readonly GuidIdPropertyDefinition ReminderNextTime = InternalSchema.CreateGuidIdProperty("ReminderNextTime", typeof(ExDateTime), WellKnownPropertySet.Common, 34144);

		public static readonly GuidIdPropertyDefinition ReminderMinutesBeforeStartInternal = InternalSchema.CreateGuidIdProperty("ReminderMinutesBeforeStartInternal", typeof(int), WellKnownPropertySet.Common, 34049);

		public static readonly GuidIdPropertyDefinition AppointmentClass = InternalSchema.CreateGuidIdProperty("AppointmentClass", typeof(string), WellKnownPropertySet.Meeting, 36);

		public static readonly GuidIdPropertyDefinition AppointmentColor = InternalSchema.CreateGuidIdProperty("AppointmentColor", typeof(int), WellKnownPropertySet.Appointment, 33300);

		public static readonly PropertyTagPropertyDefinition AppointmentExceptionEndTime = PropertyTagPropertyDefinition.InternalCreate("AppointmentExceptionEndTime", (PropTag)2147221568U);

		public static readonly PropertyTagPropertyDefinition AppointmentExceptionStartTime = PropertyTagPropertyDefinition.InternalCreate("AppointmentExceptionStartTime", (PropTag)2147156032U);

		public static readonly GuidIdPropertyDefinition AppointmentStateInternal = InternalSchema.CreateGuidIdProperty("AppointmentStateInternal", typeof(int), WellKnownPropertySet.Appointment, 33303, PropertyFlags.TrackChange, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition AppointmentAuxiliaryFlags = InternalSchema.CreateGuidIdProperty("AppointmentAuxiliaryFlags", typeof(int), WellKnownPropertySet.Appointment, 33287);

		public static readonly GuidIdPropertyDefinition AppointmentLastSequenceNumber = InternalSchema.CreateGuidIdProperty("AppointmentLastSequenceNumber", typeof(int), WellKnownPropertySet.Appointment, 33283);

		public static readonly GuidNamePropertyDefinition CdoSequenceNumber = InternalSchema.CreateGuidNameProperty("CdoSequenceNumber", typeof(int), WellKnownPropertySet.PublicStrings, "urn:schemas:calendar:sequence");

		public static readonly GuidIdPropertyDefinition AppointmentRecurrenceBlob = InternalSchema.CreateGuidIdProperty("AppointmentRecurrenceBlob", typeof(byte[]), WellKnownPropertySet.Appointment, 33302, PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition AppointmentRecurring = InternalSchema.CreateGuidIdProperty("AppointmentRecurring", typeof(bool), WellKnownPropertySet.Appointment, 33315);

		public static readonly GuidIdPropertyDefinition AppointmentReplyTime = InternalSchema.CreateGuidIdProperty("AppointmentReplyTime", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33312);

		public static readonly GuidIdPropertyDefinition AppointmentSequenceNumber = InternalSchema.CreateGuidIdProperty("AppointmentSequenceNumber", typeof(int), WellKnownPropertySet.Appointment, 33281);

		public static readonly GuidIdPropertyDefinition AppointmentSequenceTime = InternalSchema.CreateGuidIdProperty("AppointmentSequenceTime", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33282);

		public static readonly GuidIdPropertyDefinition AppointmentExtractVersion = InternalSchema.CreateGuidIdProperty("AppointmentExtractVersion", typeof(long), WellKnownPropertySet.Appointment, 33324);

		public static readonly GuidIdPropertyDefinition AppointmentExtractTime = InternalSchema.CreateGuidIdProperty("AppointmentExtractTime", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33325);

		public static readonly GuidIdPropertyDefinition ConferenceType = InternalSchema.CreateGuidIdProperty("ConferenceType", typeof(int), WellKnownPropertySet.Appointment, 33345);

		public static readonly GuidIdPropertyDefinition DisallowNewTimeProposal = InternalSchema.CreateGuidIdProperty("DisallowNewTimeProposal", typeof(bool), WellKnownPropertySet.Appointment, 33370);

		public static readonly GuidIdPropertyDefinition Duration = InternalSchema.CreateGuidIdProperty("Duration", typeof(int), WellKnownPropertySet.Appointment, 33299);

		public static readonly GuidIdPropertyDefinition IsOnlineMeeting = InternalSchema.CreateGuidIdProperty("IsOnlineMeeting", typeof(bool), WellKnownPropertySet.Appointment, 33344);

		public static readonly GuidIdPropertyDefinition LidWhere = InternalSchema.CreateGuidIdProperty("LID_WHERE", typeof(string), WellKnownPropertySet.Meeting, 2);

		public static readonly GuidIdPropertyDefinition SeriesCreationHash = InternalSchema.CreateGuidIdProperty("SeriesCreationHash", typeof(int), WellKnownPropertySet.CalendarAssistant, 34146);

		public static readonly GuidNamePropertyDefinition SeriesMasterId = InternalSchema.CreateGuidNameProperty("SeriesMasterId", typeof(string), WellKnownPropertySet.Appointment, "SeriesMasterId");

		public static readonly GuidNamePropertyDefinition SeriesId = InternalSchema.CreateGuidNameProperty("SeriesId", typeof(string), WellKnownPropertySet.Appointment, "SeriesId");

		public static readonly GuidNamePropertyDefinition EventClientId = InternalSchema.CreateGuidNameProperty("EventClientId", typeof(string), WellKnownPropertySet.Appointment, "EventClientId");

		public static readonly GuidNamePropertyDefinition SeriesReminderIsSet = InternalSchema.CreateGuidNameProperty("SeriesReminderIsSet", typeof(bool), WellKnownPropertySet.Appointment, "SeriesReminderIsSet");

		public static readonly GuidNamePropertyDefinition InstanceCreationIndex = InternalSchema.CreateGuidNameProperty("InstanceCreationIndex", typeof(int), WellKnownPropertySet.Appointment, "InstanceCreationIndex");

		public static readonly GuidNamePropertyDefinition IsHiddenFromLegacyClients = InternalSchema.CreateGuidNameProperty("IsHiddenFromLegacyClients", typeof(bool), WellKnownPropertySet.Appointment, "IsHiddenFromLegacyClients");

		public static readonly GuidNamePropertyDefinition OccurrencesExceptionalViewProperties = InternalSchema.CreateGuidNameProperty("OccurrencesExceptionalViewProperties", typeof(string), WellKnownPropertySet.CalendarAssistant, "OccurrencesExceptionalViewProperties", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition SeriesSequenceNumber = InternalSchema.CreateGuidNameProperty("SeriesSequenceNumber", typeof(int), WellKnownPropertySet.CalendarAssistant, "SeriesSequenceNumber");

		public static readonly GuidNamePropertyDefinition PropertyChangeMetadataProcessingFlags = InternalSchema.CreateGuidNameProperty("PropertyChangeMetadataProcessingFlags", typeof(int), WellKnownPropertySet.CalendarAssistant, "PropertyChangeMetadataProcessingFlags");

		public static readonly GuidNamePropertyDefinition MasterGlobalObjectId = InternalSchema.CreateGuidNameProperty("MasterGlobalObjectId", typeof(byte[]), WellKnownPropertySet.CalendarAssistant, "MasterGlobalObjectId");

		public static readonly GuidNamePropertyDefinition ParkedCorrelationId = InternalSchema.CreateGuidNameProperty("ParkedCorrelationId", typeof(string), WellKnownPropertySet.CalendarAssistant, "ParkedCorrelationId");

		public static readonly GuidIdPropertyDefinition GlobalObjectId = InternalSchema.CreateGuidIdProperty("GlobalObjectId", typeof(byte[]), WellKnownPropertySet.Meeting, 3);

		public static readonly GuidIdPropertyDefinition IsSilent = InternalSchema.CreateGuidIdProperty("IsSilent", typeof(bool), WellKnownPropertySet.Meeting, 4);

		public static readonly GuidIdPropertyDefinition IsRecurring = InternalSchema.CreateGuidIdProperty("IsRecurring", typeof(bool), WellKnownPropertySet.Meeting, 5);

		public static readonly GuidIdPropertyDefinition CleanGlobalObjectId = InternalSchema.CreateGuidIdProperty("CleanGlobalObjectId", typeof(byte[]), WellKnownPropertySet.Meeting, 35);

		internal static readonly GuidIdPropertyDefinition MeetingUniqueId = InternalSchema.CreateGuidIdProperty("MeetingUniqueId", typeof(byte[]), WellKnownPropertySet.Meeting, 46);

		public static readonly GuidIdPropertyDefinition Location = InternalSchema.CreateGuidIdProperty("Location", typeof(string), WellKnownPropertySet.Appointment, 33288);

		public static readonly GuidIdPropertyDefinition MapiEndTime = InternalSchema.CreateGuidIdProperty("MapiEndTime", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33294);

		public static readonly GuidIdPropertyDefinition MapiStartTime = InternalSchema.CreateGuidIdProperty("MapiStartTime", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33293);

		public static readonly PropertyTagPropertyDefinition MapiPRStartDate = PropertyTagPropertyDefinition.InternalCreate("PR_START_DATE", PropTag.StartDate);

		public static readonly PropertyTagPropertyDefinition MapiPREndDate = PropertyTagPropertyDefinition.InternalCreate("PR_END_DATE", PropTag.EndDate);

		public static readonly GuidIdPropertyDefinition MapiIsAllDayEvent = InternalSchema.CreateGuidIdProperty("MapiIsAllDayEvent", typeof(bool), WellKnownPropertySet.Appointment, 33301);

		public static readonly GuidIdPropertyDefinition MeetingRequestWasSent = InternalSchema.CreateGuidIdProperty("MeetingRequestWasSent", typeof(bool), WellKnownPropertySet.Appointment, 33321);

		public static readonly GuidIdPropertyDefinition MeetingWorkspaceUrl = InternalSchema.CreateGuidIdProperty("MeetingWorkspaceUrl", typeof(string), WellKnownPropertySet.Appointment, 33289);

		public static readonly GuidIdPropertyDefinition NetShowURL = InternalSchema.CreateGuidIdProperty("NetShowURL", typeof(string), WellKnownPropertySet.Appointment, 33352);

		public static readonly GuidIdPropertyDefinition OnlineMeetingChanged = InternalSchema.CreateGuidIdProperty("OnlinePropertyChanged", typeof(bool), WellKnownPropertySet.Appointment, 33343);

		public static readonly GuidIdPropertyDefinition RecurrencePattern = InternalSchema.CreateGuidIdProperty("RecurrencePattern", typeof(string), WellKnownPropertySet.Appointment, 33330);

		public static readonly GuidIdPropertyDefinition MapiRecurrenceType = InternalSchema.CreateGuidIdProperty("MapiRecurrenceType", typeof(int), WellKnownPropertySet.Appointment, 33329);

		public static readonly GuidIdPropertyDefinition TimeZone = InternalSchema.CreateGuidIdProperty("TimeZone", typeof(string), WellKnownPropertySet.Appointment, 33332);

		public static readonly GuidIdPropertyDefinition TimeZoneBlob = InternalSchema.CreateGuidIdProperty("TimeZoneBlob", typeof(byte[]), WellKnownPropertySet.Appointment, 33331);

		public static readonly GuidIdPropertyDefinition TimeZoneDefinitionStart = InternalSchema.CreateGuidIdProperty("TimeZoneDefinitionStart", typeof(byte[]), WellKnownPropertySet.Appointment, 33374);

		public static readonly GuidIdPropertyDefinition TimeZoneDefinitionEnd = InternalSchema.CreateGuidIdProperty("TimeZoneDefinitionEnd", typeof(byte[]), WellKnownPropertySet.Appointment, 33375);

		public static readonly GuidIdPropertyDefinition TimeZoneDefinitionRecurring = InternalSchema.CreateGuidIdProperty("TimeZoneDefinitionRecurring", typeof(byte[]), WellKnownPropertySet.Appointment, 33376);

		internal static readonly GuidIdPropertyDefinition ClipStartTime = InternalSchema.CreateGuidIdProperty("ClipStartTime", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33333);

		internal static readonly GuidIdPropertyDefinition ClipEndTime = InternalSchema.CreateGuidIdProperty("ClipEndTime", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33334);

		internal static readonly GuidIdPropertyDefinition OriginalStoreEntryId = InternalSchema.CreateGuidIdProperty("OriginalStoreEntryId", typeof(byte[]), WellKnownPropertySet.Appointment, 33335);

		public static readonly GuidIdPropertyDefinition When = InternalSchema.CreateGuidIdProperty("When", typeof(string), WellKnownPropertySet.Meeting, 34);

		public static readonly GuidIdPropertyDefinition ExceptionReplaceTime = InternalSchema.CreateGuidIdProperty("ExceptionReplaceTime", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33320);

		public static readonly GuidNamePropertyDefinition UCOpenedConferenceID = InternalSchema.CreateGuidNameProperty("UCOpenedConferenceID", typeof(string), WellKnownPropertySet.PublicStrings, "UCOpenedConferenceID");

		public static readonly GuidNamePropertyDefinition OnlineMeetingExternalLink = InternalSchema.CreateGuidNameProperty("OnlineMeetingExternalLink", typeof(string), WellKnownPropertySet.PublicStrings, "OnlineMeetingExternalLink");

		public static readonly GuidNamePropertyDefinition OnlineMeetingInternalLink = InternalSchema.CreateGuidNameProperty("OnlineMeetingInternalLink", typeof(string), WellKnownPropertySet.PublicStrings, "OnlineMeetingInternalLink");

		public static readonly GuidNamePropertyDefinition OnlineMeetingConfLink = InternalSchema.CreateGuidNameProperty("OnlineMeetingConfLink", typeof(string), WellKnownPropertySet.PublicStrings, "OnlineMeetingConfLink");

		public static readonly GuidNamePropertyDefinition UCCapabilities = InternalSchema.CreateGuidNameProperty("UCCapabilities", typeof(string), WellKnownPropertySet.PublicStrings, "UCCapabilities", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UCInband = InternalSchema.CreateGuidNameProperty("UCInband", typeof(string), WellKnownPropertySet.PublicStrings, "UCInband", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UCMeetingSetting = InternalSchema.CreateGuidNameProperty("UCMeetingSetting", typeof(string), WellKnownPropertySet.PublicStrings, "UCMeetingSetting", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UCMeetingSettingSent = InternalSchema.CreateGuidNameProperty("UCMeetingSettingSent", typeof(string), WellKnownPropertySet.PublicStrings, "UCMeetingSettingSent", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition ConferenceTelURI = InternalSchema.CreateGuidNameProperty("ConferenceTelURI", typeof(string), WellKnownPropertySet.PublicStrings, "ConferenceTelURI");

		public static readonly GuidNamePropertyDefinition ConferenceInfo = InternalSchema.CreateGuidNameProperty("ConferenceInfo", typeof(string), WellKnownPropertySet.PublicStrings, "ConferenceInfo");

		public static readonly PropertyTagPropertyDefinition AttachPayloadProviderGuidString = PropertyTagPropertyDefinition.InternalCreate("AttachPayloadProviderGuidString", (PropTag)924385311U);

		public static readonly PropertyTagPropertyDefinition AttachPayloadClass = PropertyTagPropertyDefinition.InternalCreate("AttachPayloadProviderClass", (PropTag)924450847U);

		public static readonly GuidIdPropertyDefinition IsException = InternalSchema.CreateGuidIdProperty("IsException", typeof(bool), WellKnownPropertySet.Meeting, 10);

		public static readonly GuidNamePropertyDefinition AllContactsFolderEntryId = InternalSchema.CreateGuidNameProperty("AllContactsFolderEntryId", typeof(byte[]), WellKnownPropertySet.Address, "AllContactsFolderEntryId");

		public static readonly GuidNamePropertyDefinition MyContactsFolderEntryId = InternalSchema.CreateGuidNameProperty("MyContactsFolderEntryId", typeof(byte[]), WellKnownPropertySet.Address, "MyContactsFolderEntryId");

		public static readonly GuidNamePropertyDefinition MyContactsExtendedFolderEntryId = InternalSchema.CreateGuidNameProperty("MyContactsExtendedFolderEntryId", typeof(byte[]), WellKnownPropertySet.Address, "MyContactsExtendedFolderEntryId");

		public static readonly GuidNamePropertyDefinition MyContactsFoldersInternal = InternalSchema.CreateGuidNameProperty("MyContactsFoldersInternal", typeof(byte[][]), WellKnownPropertySet.Address, "MyContactsFolders");

		public static readonly StoreObjectIdCollectionProperty MyContactsFolders = new StoreObjectIdCollectionProperty(InternalSchema.MyContactsFoldersInternal, PropertyFlags.None, "MyContactsFolders");

		public static readonly GuidNamePropertyDefinition PeopleConnectFolderEntryId = InternalSchema.CreateGuidNameProperty("PeopleConnectFolderEntryId", typeof(byte[]), WellKnownPropertySet.Address, "PeopleConnectFolderEntryId");

		public static readonly GuidNamePropertyDefinition FavoritesFolderEntryId = InternalSchema.CreateGuidNameProperty("FavoritesFolderEntryId", typeof(byte[]), WellKnownPropertySet.Address, "FavoritesFolderEntryId");

		public static readonly GuidNamePropertyDefinition FromFavoriteSendersFolderEntryId = InternalSchema.CreateGuidNameProperty("FromFavoriteSendersFolderEntryId", typeof(byte[]), WellKnownPropertySet.Address, "FromFavoriteSendersFolderEntryId");

		public static readonly GuidNamePropertyDefinition FromPeopleFolderEntryId = InternalSchema.CreateGuidNameProperty("FromPeopleFolderEntryId", typeof(byte[]), WellKnownPropertySet.Address, "FromPeopleFolderEntryId");

		public static readonly GuidNamePropertyDefinition MailboxAssociationFolderEntryId = InternalSchema.CreateGuidNameProperty("MailboxAssociationFolderEntryId", typeof(byte[]), WellKnownPropertySet.Common, "MailboxAssociationFolderEntryId");

		public static readonly PropertyTagPropertyDefinition AssistantName = PropertyTagPropertyDefinition.InternalCreate("AssistantName", PropTag.Assistant);

		public static readonly PropertyTagPropertyDefinition AssistantPhoneNumber = PropertyTagPropertyDefinition.InternalCreate("AssistantPhoneNumber", PropTag.AssistantTelephoneNumber);

		public static readonly GuidIdPropertyDefinition BillingInformation = InternalSchema.CreateGuidIdProperty("BillingInformation", typeof(string), WellKnownPropertySet.Common, 34101);

		public static readonly GuidIdPropertyDefinition BirthdayLocal = InternalSchema.CreateGuidIdProperty("BirthdayLocal", typeof(ExDateTime), WellKnownPropertySet.Address, 32990);

		public static readonly PropertyTagPropertyDefinition Birthday = PropertyTagPropertyDefinition.InternalCreate("Birthday", PropTag.Birthday);

		public static readonly GuidNamePropertyDefinition NotInBirthdayCalendar = InternalSchema.CreateGuidNameProperty("NotInBirthdayCalendar", typeof(bool), WellKnownPropertySet.Address, "ContactNotInBirthdayCalendar");

		public static readonly GuidNamePropertyDefinition IsBirthdayContactWritable = InternalSchema.CreateGuidNameProperty("IsBirthdayContactWritable", typeof(bool), WellKnownPropertySet.Address, "IsBirthdayContactWritable");

		public static readonly GuidNamePropertyDefinition BirthdayContactEntryId = InternalSchema.CreateGuidNameProperty("BirthdayContactEntryId", typeof(byte[]), WellKnownPropertySet.Address, "BirthdayContactEntryId");

		public static readonly GuidNamePropertyDefinition BirthdayContactAttributionDisplayName = InternalSchema.CreateGuidNameProperty("BirthdayContactAttributionDisplayName", typeof(string), WellKnownPropertySet.Address, "BirthdayContactAttributionDisplayName");

		public static readonly PropertyTagPropertyDefinition BusinessHomePage = PropertyTagPropertyDefinition.InternalCreate("BusinessHomePage", PropTag.BusinessHomePage);

		public static readonly PropertyTagPropertyDefinition BusinessPhoneNumber = PropertyTagPropertyDefinition.InternalCreate("BusinessPhoneNumber", PropTag.BusinessTelephoneNumber);

		public static readonly PropertyTagPropertyDefinition BusinessPhoneNumber2 = PropertyTagPropertyDefinition.InternalCreate("BusinessPhoneNumber2", PropTag.Business2TelephoneNumber);

		public static readonly PropertyTagPropertyDefinition CallbackPhone = PropertyTagPropertyDefinition.InternalCreate("CallbackPhone", PropTag.CallbackTelephoneNumber);

		public static readonly PropertyTagPropertyDefinition CarPhone = PropertyTagPropertyDefinition.InternalCreate("CarPhone", PropTag.CarTelephoneNumber);

		public static readonly PropertyTagPropertyDefinition Children = PropertyTagPropertyDefinition.InternalCreate("Children", PropTag.ChildrensNames);

		public static readonly GuidIdPropertyDefinition Companies = InternalSchema.CreateGuidIdProperty("Companies", typeof(string[]), WellKnownPropertySet.Common, 34105);

		public static readonly PropertyTagPropertyDefinition CompanyName = PropertyTagPropertyDefinition.InternalCreate("CompanyName", PropTag.CompanyName);

		public static readonly PropertyTagPropertyDefinition Department = PropertyTagPropertyDefinition.InternalCreate("Department", PropTag.DepartmentName);

		public static readonly GuidIdPropertyDefinition Email1AddrType = InternalSchema.CreateGuidIdProperty("Email1AddrType", typeof(string), WellKnownPropertySet.Address, 32898, PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly GuidIdPropertyDefinition Email1DisplayName = InternalSchema.CreateGuidIdProperty("Email1DisplayName", typeof(string), WellKnownPropertySet.Address, 32896);

		public static readonly GuidIdPropertyDefinition Email1EmailAddress = InternalSchema.CreateGuidIdProperty("Email1EmailAddress", typeof(string), WellKnownPropertySet.Address, 32899);

		public static readonly GuidIdPropertyDefinition Email1OriginalDisplayName = InternalSchema.CreateGuidIdProperty("Email1OriginalDisplayName", typeof(string), WellKnownPropertySet.Address, 32900);

		public static readonly GuidIdPropertyDefinition Email1OriginalEntryID = InternalSchema.CreateGuidIdProperty("Email1OriginalEntryID", typeof(byte[]), WellKnownPropertySet.Address, 32901);

		public static readonly GuidIdPropertyDefinition Email2AddrType = InternalSchema.CreateGuidIdProperty("Email2AddrType", typeof(string), WellKnownPropertySet.Address, 32914, PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly GuidIdPropertyDefinition Email2DisplayName = InternalSchema.CreateGuidIdProperty("Email2DisplayName", typeof(string), WellKnownPropertySet.Address, 32912);

		public static readonly GuidIdPropertyDefinition Email2EmailAddress = InternalSchema.CreateGuidIdProperty("Email2EmailAddress", typeof(string), WellKnownPropertySet.Address, 32915);

		public static readonly GuidIdPropertyDefinition Email2OriginalDisplayName = InternalSchema.CreateGuidIdProperty("Email2OriginalDisplayName", typeof(string), WellKnownPropertySet.Address, 32916);

		public static readonly GuidIdPropertyDefinition Email2OriginalEntryID = InternalSchema.CreateGuidIdProperty("Email2OriginalEntryID", typeof(byte[]), WellKnownPropertySet.Address, 32917);

		public static readonly GuidIdPropertyDefinition Email3AddrType = InternalSchema.CreateGuidIdProperty("Email3AddrType", typeof(string), WellKnownPropertySet.Address, 32930, PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly GuidIdPropertyDefinition Email3DisplayName = InternalSchema.CreateGuidIdProperty("Email3DisplayName", typeof(string), WellKnownPropertySet.Address, 32928);

		public static readonly GuidIdPropertyDefinition Email3EmailAddress = InternalSchema.CreateGuidIdProperty("Email3EmailAddress", typeof(string), WellKnownPropertySet.Address, 32931);

		public static readonly GuidIdPropertyDefinition Email3OriginalDisplayName = InternalSchema.CreateGuidIdProperty("Email3OriginalDisplayName", typeof(string), WellKnownPropertySet.Address, 32932);

		public static readonly GuidIdPropertyDefinition Email3OriginalEntryID = InternalSchema.CreateGuidIdProperty("Email3OriginalEntryID", typeof(byte[]), WellKnownPropertySet.Address, 32933);

		public static readonly GuidIdPropertyDefinition Fax1AddrType = InternalSchema.CreateGuidIdProperty("Fax1AddrType", typeof(string), WellKnownPropertySet.Address, 32946, PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly GuidIdPropertyDefinition Fax1EmailAddress = InternalSchema.CreateGuidIdProperty("Fax1EmailAddress", typeof(string), WellKnownPropertySet.Address, 32947);

		public static readonly GuidIdPropertyDefinition Fax1OriginalDisplayName = InternalSchema.CreateGuidIdProperty("Fax1OriginalDisplayName", typeof(string), WellKnownPropertySet.Address, 32948);

		public static readonly GuidIdPropertyDefinition Fax1OriginalEntryID = InternalSchema.CreateGuidIdProperty("Fax1OriginalEntryID", typeof(byte[]), WellKnownPropertySet.Address, 32949);

		public static readonly GuidIdPropertyDefinition Fax2AddrType = InternalSchema.CreateGuidIdProperty("Fax2AddrType", typeof(string), WellKnownPropertySet.Address, 32962, PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly GuidIdPropertyDefinition Fax2EmailAddress = InternalSchema.CreateGuidIdProperty("Fax2EmailAddress", typeof(string), WellKnownPropertySet.Address, 32963);

		public static readonly GuidIdPropertyDefinition Fax2OriginalDisplayName = InternalSchema.CreateGuidIdProperty("Fax2OriginalDisplayName", typeof(string), WellKnownPropertySet.Address, 32964);

		public static readonly GuidIdPropertyDefinition Fax2OriginalEntryID = InternalSchema.CreateGuidIdProperty("Fax2OriginalEntryID", typeof(byte[]), WellKnownPropertySet.Address, 32965);

		public static readonly GuidIdPropertyDefinition Fax3AddrType = InternalSchema.CreateGuidIdProperty("Fax3AddrType", typeof(string), WellKnownPropertySet.Address, 32978, PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly GuidIdPropertyDefinition Fax3EmailAddress = InternalSchema.CreateGuidIdProperty("Fax3EmailAddress", typeof(string), WellKnownPropertySet.Address, 32979);

		public static readonly GuidIdPropertyDefinition Fax3OriginalDisplayName = InternalSchema.CreateGuidIdProperty("Fax3OriginalDisplayName", typeof(string), WellKnownPropertySet.Address, 32980);

		public static readonly GuidIdPropertyDefinition Fax3OriginalEntryID = InternalSchema.CreateGuidIdProperty("Fax3OriginalEntryID", typeof(byte[]), WellKnownPropertySet.Address, 32981);

		public static readonly PropertyTagPropertyDefinition EmailAddress = PropertyTagPropertyDefinition.InternalCreate("EmailAddress", PropTag.EmailAddress);

		public static readonly PropertyTagPropertyDefinition FaxNumber = PropertyTagPropertyDefinition.InternalCreate("FaxNumber", PropTag.BusinessFaxNumber);

		public static readonly GuidNamePropertyDefinition Transparent = InternalSchema.CreateGuidNameProperty("Transparent", typeof(string), WellKnownPropertySet.PublicStrings, "urn:schemas:calendar:transparent");

		public static readonly GuidIdPropertyDefinition FileAsStringInternal = InternalSchema.CreateGuidIdProperty("FileAsInternal", typeof(string), WellKnownPropertySet.Address, 32773);

		public static readonly GuidIdPropertyDefinition FileAsId = InternalSchema.CreateGuidIdProperty("FileAsId", typeof(int), WellKnownPropertySet.Address, 32774);

		public static readonly PropertyTagPropertyDefinition GivenName = PropertyTagPropertyDefinition.InternalCreate("GivenName", PropTag.GivenName, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 256))
		});

		public static readonly PropertyTagPropertyDefinition HomePostOfficeBox = PropertyTagPropertyDefinition.InternalCreate("HomePostOfficeBox", PropTag.HomeAddressPostOfficeBox);

		public static readonly PropertyTagPropertyDefinition HomeCity = PropertyTagPropertyDefinition.InternalCreate("HomeCity", PropTag.HomeAddressCity);

		public static readonly PropertyTagPropertyDefinition HomeCountry = PropertyTagPropertyDefinition.InternalCreate("HomeCountry", PropTag.HomeAddressCountry);

		public static readonly PropertyTagPropertyDefinition HomeFax = PropertyTagPropertyDefinition.InternalCreate("HomeFax", PropTag.HomeFaxNumber);

		public static readonly PropertyTagPropertyDefinition HomePhone = PropertyTagPropertyDefinition.InternalCreate("HomePhone", PropTag.HomeTelephoneNumber);

		public static readonly PropertyTagPropertyDefinition HomePhone2 = PropertyTagPropertyDefinition.InternalCreate("HomePhone2", PropTag.Home2TelephoneNumber);

		public static readonly PropertyTagPropertyDefinition HomePostalCode = PropertyTagPropertyDefinition.InternalCreate("HomePostalCode", PropTag.HomeAddressPostalCode);

		public static readonly PropertyTagPropertyDefinition HomeState = PropertyTagPropertyDefinition.InternalCreate("HomeState", PropTag.HomeAddressStateOrProvince);

		public static readonly PropertyTagPropertyDefinition HomeStreet = PropertyTagPropertyDefinition.InternalCreate("HomeStreet", PropTag.HomeAddressStreet);

		public static readonly GuidIdPropertyDefinition IMAddress = InternalSchema.CreateGuidIdProperty("IMAddress", typeof(string), WellKnownPropertySet.Address, 32866);

		public static GuidNamePropertyDefinition IMAddress2 = InternalSchema.CreateGuidNameProperty("IMAddress2", typeof(string), WellKnownPropertySet.AirSync, "AirSync:IMAddress2");

		public static GuidNamePropertyDefinition IMAddress3 = InternalSchema.CreateGuidNameProperty("IMAddress3", typeof(string), WellKnownPropertySet.AirSync, "AirSync:IMAddress3");

		public static readonly GuidNamePropertyDefinition ClientCategoryList = InternalSchema.CreateGuidNameProperty("ClientCategoryList", typeof(int[]), WellKnownPropertySet.AirSync, "AirSync:ClientCategoryList");

		public static readonly GuidNamePropertyDefinition LastSeenClientIds = InternalSchema.CreateGuidNameProperty("LastSeenClientIds", typeof(string[]), WellKnownPropertySet.AirSync, "AirSync:LastSeenClientIds");

		public static readonly GuidNamePropertyDefinition AirSyncSyncKey = InternalSchema.CreateGuidNameProperty("AirSyncSynckey", typeof(int), WellKnownPropertySet.AirSync, "AirSync:AirSyncSynckey");

		public static readonly GuidNamePropertyDefinition AirSyncFilter = InternalSchema.CreateGuidNameProperty("AirSyncFilter", typeof(int), WellKnownPropertySet.AirSync, "AirSync:AirSyncFilter");

		public static readonly GuidNamePropertyDefinition AirSyncConversationMode = InternalSchema.CreateGuidNameProperty("AirSyncConversationMode", typeof(bool), WellKnownPropertySet.AirSync, "AirSync:AirSyncConversationMode");

		public static readonly GuidNamePropertyDefinition AirSyncSettingsHash = InternalSchema.CreateGuidNameProperty("AirSyncSettingsHash", typeof(int), WellKnownPropertySet.AirSync, "AirSync:AirSyncSettingsHash");

		public static readonly GuidNamePropertyDefinition AirSyncMaxItems = InternalSchema.CreateGuidNameProperty("AirSyncMaxItems", typeof(int), WellKnownPropertySet.AirSync, "AirSync:AirSyncMaxItems");

		public static readonly GuidNamePropertyDefinition AirSyncDeletedCountTotal = InternalSchema.CreateGuidNameProperty("AirSyncDeletedCountTotal", typeof(int), WellKnownPropertySet.AirSync, "AirSync:AirSyncDeletedCountTotal");

		public static readonly GuidNamePropertyDefinition AirSyncLastSyncTime = InternalSchema.CreateGuidNameProperty("AirSyncLastSyncTime", typeof(long), WellKnownPropertySet.AirSync, "AirSync:AirSyncLastSyncTime");

		public static readonly GuidNamePropertyDefinition AirSyncLocalCommitTimeMax = InternalSchema.CreateGuidNameProperty("AirSyncLocalCommitTimeMax", typeof(long), WellKnownPropertySet.AirSync, "AirSync:AirSyncLocalCommitTimeMax");

		public static readonly GuidNamePropertyDefinition LastSyncAttemptTime = InternalSchema.CreateGuidNameProperty("LastSyncAttemptTime", typeof(ExDateTime), WellKnownPropertySet.AirSync, "AirSync:LastSyncAttemptTime");

		public static readonly GuidNamePropertyDefinition LastSyncSuccessTime = InternalSchema.CreateGuidNameProperty("LastSyncSuccessTime", typeof(ExDateTime), WellKnownPropertySet.AirSync, "AirSync:LastSyncSuccessTime");

		public static readonly GuidNamePropertyDefinition LastSyncUserAgent = InternalSchema.CreateGuidNameProperty("LastSyncUserAgent", typeof(string), WellKnownPropertySet.AirSync, "AirSync:LastSyncUserAgent");

		public static readonly GuidNamePropertyDefinition LastPingHeartbeatInterval = InternalSchema.CreateGuidNameProperty("LastPingHeartbeatInterval", typeof(int), WellKnownPropertySet.AirSync, "AirSync:LastPingHeartbeatInterval");

		public static readonly GuidNamePropertyDefinition DeviceBlockedUntil = InternalSchema.CreateGuidNameProperty("DeviceBlockedUntil", typeof(ExDateTime), WellKnownPropertySet.AirSync, "AirSync:DeviceBlockedUntil");

		public static readonly GuidNamePropertyDefinition DeviceBlockedAt = InternalSchema.CreateGuidNameProperty("DeviceBlockedAt", typeof(ExDateTime), WellKnownPropertySet.AirSync, "AirSync:DeviceBlockedAt");

		public static readonly GuidNamePropertyDefinition DeviceBlockedReason = InternalSchema.CreateGuidNameProperty("DeviceBlockedReason", typeof(string), WellKnownPropertySet.AirSync, "AirSync:DeviceBlockedReason");

		public static readonly GuidIdPropertyDefinition Linked = InternalSchema.CreateGuidIdProperty("Linked", typeof(bool), WellKnownPropertySet.Address, 32992);

		public static readonly GuidIdPropertyDefinition AddressBookEntryId = InternalSchema.CreateGuidIdProperty("AddressBookEntryId", typeof(byte[]), WellKnownPropertySet.Address, 32994);

		public static readonly GuidIdPropertyDefinition SmtpAddressCache = InternalSchema.CreateGuidIdProperty("SmtpAddressCache", typeof(string[]), WellKnownPropertySet.Address, 32995);

		public static readonly GuidIdPropertyDefinition LinkADID = InternalSchema.CreateGuidIdProperty("LinkADID", typeof(Guid), WellKnownPropertySet.Address, 32996);

		public static readonly GuidIdPropertyDefinition LinkRejectHistoryRaw = InternalSchema.CreateGuidIdProperty("LinkRejectHistoryRaw", typeof(byte[][]), WellKnownPropertySet.Address, 32997);

		public static readonly GuidIdPropertyDefinition GALLinkState = InternalSchema.CreateGuidIdProperty("GALLinkState", typeof(int), WellKnownPropertySet.Address, 32998);

		public static readonly GuidIdPropertyDefinition GALLinkID = InternalSchema.CreateGuidIdProperty("GALLinkID", typeof(Guid), WellKnownPropertySet.Address, 33000);

		public static readonly GuidIdPropertyDefinition UserApprovedLink = InternalSchema.CreateGuidIdProperty("UserApprovedLink", typeof(bool), WellKnownPropertySet.Address, 33003);

		public static readonly GuidNamePropertyDefinition LinkChangeHistory = InternalSchema.CreateGuidNameProperty("LinkChangeHistory", typeof(string), WellKnownPropertySet.Address, "LinkChangeHistory");

		public static readonly GuidNamePropertyDefinition TelUri = InternalSchema.CreateGuidNameProperty("TelURI", typeof(string), WellKnownPropertySet.PublicStrings, "TelURI");

		public static readonly GuidNamePropertyDefinition ImContactSipUriAddress = InternalSchema.CreateGuidNameProperty("ImContactSipUriAddress", typeof(string), WellKnownPropertySet.Address, "ImContactSipUriAddress");

		public static readonly GuidIdPropertyDefinition OutlookContactLinkDateTime = InternalSchema.CreateGuidIdProperty("OutlookContactLinkDateTime", typeof(ExDateTime), WellKnownPropertySet.PublicStrings, 32993);

		public static readonly GuidIdPropertyDefinition OutlookContactLinkVersion = InternalSchema.CreateGuidIdProperty("OutlookContactLinkVersion", typeof(long), WellKnownPropertySet.PublicStrings, 32999);

		public static readonly GuidNamePropertyDefinition MailboxAssociationExternalId = InternalSchema.CreateGuidNameProperty("MailboxAssociationExternalId", typeof(string), WellKnownPropertySet.Common, "MailboxAssociationExternalId");

		public static readonly GuidNamePropertyDefinition MailboxAssociationLegacyDN = InternalSchema.CreateGuidNameProperty("MailboxAssociationLegacyDN", typeof(string), WellKnownPropertySet.Common, "MailboxAssociationLegacyDN");

		public static readonly GuidNamePropertyDefinition MailboxAssociationIsMember = InternalSchema.CreateGuidNameProperty("MailboxAssociationIsMember", typeof(bool), WellKnownPropertySet.Common, "MailboxAssociationIsMember");

		public static readonly GuidNamePropertyDefinition MailboxAssociationShouldEscalate = InternalSchema.CreateGuidNameProperty("MailboxAssociationShouldEscalate", typeof(bool), WellKnownPropertySet.Common, "MailboxAssociationShouldEscalate");

		public static readonly GuidNamePropertyDefinition MailboxAssociationIsAutoSubscribed = InternalSchema.CreateGuidNameProperty("MailboxAssociationIsAutoSubscribed", typeof(bool), WellKnownPropertySet.Common, "MailboxAssociationIsAutoSubscribed");

		public static readonly GuidNamePropertyDefinition MailboxAssociationSmtpAddress = InternalSchema.CreateGuidNameProperty("MailboxAssociationSmtpAddress", typeof(string), WellKnownPropertySet.Common, "MailboxAssociationSmtpAddress");

		public static readonly GuidNamePropertyDefinition MailboxAssociationJoinedBy = InternalSchema.CreateGuidNameProperty("MailboxAssociationJoinedBy", typeof(string), WellKnownPropertySet.Common, "MailboxAssociationJoinedBy");

		public static readonly GuidNamePropertyDefinition MailboxAssociationIsPin = InternalSchema.CreateGuidNameProperty("MailboxAssociationIsPin", typeof(bool), WellKnownPropertySet.Common, "MailboxAssociationIsPin");

		public static readonly GuidNamePropertyDefinition MailboxAssociationJoinDate = InternalSchema.CreateGuidNameProperty("MailboxAssociationJoinDate", typeof(ExDateTime), WellKnownPropertySet.Common, "MailboxAssociationJoinDate");

		public static readonly GuidNamePropertyDefinition MailboxAssociationLastVisitedDate = InternalSchema.CreateGuidNameProperty("MailboxAssociationLastVisitedDate", typeof(ExDateTime), WellKnownPropertySet.Common, "MailboxAssociationLastVisitedDate");

		public static readonly GuidNamePropertyDefinition MailboxAssociationPinDate = InternalSchema.CreateGuidNameProperty("MailboxAssociationPinDate", typeof(ExDateTime), WellKnownPropertySet.Common, "MailboxAssociationPinDate");

		public static readonly GuidNamePropertyDefinition MailboxAssociationSyncedIdentityHash = InternalSchema.CreateGuidNameProperty("MailboxAssociationSyncedIdentityHash", typeof(string), WellKnownPropertySet.Common, "MailboxAssociationSyncedIdentityHash");

		public static readonly GuidNamePropertyDefinition MailboxAssociationCurrentVersion = InternalSchema.CreateGuidNameProperty("MailboxAssociationCurrentVersion", typeof(int), WellKnownPropertySet.Common, "MailboxAssociationCurrentVersion");

		public static readonly GuidNamePropertyDefinition MailboxAssociationSyncedVersion = InternalSchema.CreateGuidNameProperty("MailboxAssociationSyncedVersion", typeof(int), WellKnownPropertySet.Common, "MailboxAssociationSyncedVersion");

		public static readonly GuidNamePropertyDefinition MailboxAssociationLastSyncError = InternalSchema.CreateGuidNameProperty("MailboxAssociationLastSyncError", typeof(string), WellKnownPropertySet.Common, "MailboxAssociationLastSyncError");

		public static readonly GuidNamePropertyDefinition MailboxAssociationSyncAttempts = InternalSchema.CreateGuidNameProperty("MailboxAssociationSyncAttempts", typeof(int), WellKnownPropertySet.Common, "MailboxAssociationSyncAttempts");

		public static readonly GuidNamePropertyDefinition MailboxAssociationSyncedSchemaVersion = InternalSchema.CreateGuidNameProperty("MailboxAssociationSyncedSchemaVersion", typeof(string), WellKnownPropertySet.Common, "MailboxAssociationSyncedSchemaVersion");

		public static readonly PropertyTagPropertyDefinition ControlDataForSearchIndexRepairAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForSearchIndexRepairAssistant", PropTag.ControlDataForSearchIndexRepairAssistant);

		public static readonly PropertyTagPropertyDefinition ControlDataForGroupMailboxAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForGroupMailboxAssistant", PropTag.ControlDataForGroupMailboxAssistant);

		public static readonly PropertyTagPropertyDefinition ControlDataForMailboxAssociationReplicationAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForMailboxAssociationReplicationAssistant", PropTag.ControlDataForMailboxAssociationReplicationAssistant);

		public static readonly PropertyTagPropertyDefinition MailboxAssociationNextReplicationTime = PropertyTagPropertyDefinition.InternalCreate("MailboxAssociationNextReplicationTime", PropTag.MailboxAssociationNextReplicationTime);

		public static readonly PropertyTagPropertyDefinition MailboxAssociationProcessingFlags = PropertyTagPropertyDefinition.InternalCreate("MailboxAssociationProcessingFlags", PropTag.MailboxAssociationProcessingFlags);

		public static readonly PropertyTagPropertyDefinition GroupMailboxPermissionsVersion = PropertyTagPropertyDefinition.InternalCreate("GroupMailboxPermissionsVersion", PropTag.GroupMailboxPermissionsVersion);

		public static readonly PropertyTagPropertyDefinition GroupMailboxGeneratedPhotoSignature = PropertyTagPropertyDefinition.InternalCreate("GroupMailboxGeneratedPhotoSignature", PropTag.GroupMailboxGeneratedPhotoSignature);

		public static readonly PropertyTagPropertyDefinition GroupMailboxGeneratedPhotoVersion = PropertyTagPropertyDefinition.InternalCreate("GroupMailboxGeneratedPhotoVersion", PropTag.GroupMailboxGeneratedPhotoVersion);

		public static readonly PropertyTagPropertyDefinition GroupMailboxExchangeResourcesPublishedVersion = PropertyTagPropertyDefinition.InternalCreate("GroupMailboxExchangeResourcesPublishedVersion", PropTag.GroupMailboxExchangeResourcesPublishedVersion);

		public static readonly GuidNamePropertyDefinition HierarchySyncLastAttemptedSyncTime = InternalSchema.CreateGuidNameProperty("HierarchySyncLastAttemptedSyncTime", typeof(ExDateTime), WellKnownPropertySet.Common, "HierarchySyncLastAttemptedSyncTime");

		public static readonly GuidNamePropertyDefinition HierarchySyncLastFailedSyncTime = InternalSchema.CreateGuidNameProperty("HierarchySyncLastFailedSyncTime", typeof(ExDateTime), WellKnownPropertySet.Common, "HierarchySyncLastFailedSyncTime");

		public static readonly GuidNamePropertyDefinition HierarchySyncLastSuccessfulSyncTime = InternalSchema.CreateGuidNameProperty("HierarchySyncLastSuccessfulSyncTime", typeof(ExDateTime), WellKnownPropertySet.Common, "HierarchySyncLastSuccessfulSyncTime");

		public static readonly GuidNamePropertyDefinition HierarchySyncFirstFailedSyncTimeAfterLastSuccess = InternalSchema.CreateGuidNameProperty("HierarchySyncFirstFailedSyncTimeAfterLastSuccess", typeof(ExDateTime), WellKnownPropertySet.Common, "HierarchySyncFirstFailedSyncTimeAfterLastSuccess");

		public static readonly GuidNamePropertyDefinition HierarchySyncLastSyncFailure = InternalSchema.CreateGuidNameProperty("HierarchySyncLastSyncFailure", typeof(string), WellKnownPropertySet.Common, "HierarchySyncLastSyncFailure");

		public static readonly GuidNamePropertyDefinition HierarchySyncNumberOfAttemptsAfterLastSuccess = InternalSchema.CreateGuidNameProperty("HierarchySyncNumberOfAttemptsAfterLastSuccess", typeof(int), WellKnownPropertySet.Common, "HierarchySyncNumberOfAttemptsAfterLastSuccess");

		public static readonly GuidNamePropertyDefinition HierarchySyncNumberOfBatchesExecuted = InternalSchema.CreateGuidNameProperty("HierarchySyncNumberOfBatchesExecuted", typeof(int), WellKnownPropertySet.Common, "HierarchySyncNumberOfBatchesExecuted");

		public static readonly GuidNamePropertyDefinition HierarchySyncNumberOfFoldersSynced = InternalSchema.CreateGuidNameProperty("HierarchySyncNumberOfFoldersSynced", typeof(int), WellKnownPropertySet.Common, "HierarchySyncNumberOfFoldersSynced");

		public static readonly GuidNamePropertyDefinition HierarchySyncNumberOfFoldersToBeSynced = InternalSchema.CreateGuidNameProperty("HierarchySyncNumberOfFoldersToBeSynced", typeof(int), WellKnownPropertySet.Common, "HierarchySyncNumberOfFoldersToBeSynced");

		public static readonly GuidNamePropertyDefinition HierarchySyncBatchSize = InternalSchema.CreateGuidNameProperty("HierarchySyncBatchSize", typeof(int), WellKnownPropertySet.Common, "HierarchySyncBatchSize");

		public static readonly GuidIdPropertyDefinition Schools = InternalSchema.CreateGuidIdProperty("Schools", typeof(string), WellKnownPropertySet.Address, 33001);

		public static readonly GuidIdPropertyDefinition InternalPersonType = InternalSchema.CreateGuidIdProperty("InternalPersonType", typeof(int), WellKnownPropertySet.Address, 33002);

		public static GuidNamePropertyDefinition MMS = InternalSchema.CreateGuidNameProperty("MMS", typeof(string), WellKnownPropertySet.AirSync, "AirSync:MMS");

		public static readonly PropertyTagPropertyDefinition Initials = PropertyTagPropertyDefinition.InternalCreate("Initials", PropTag.Initials);

		public static readonly PropertyTagPropertyDefinition InternationalIsdnNumber = PropertyTagPropertyDefinition.InternalCreate("InternationalIsdnNumber", PropTag.IsdnNumber);

		public static readonly PropertyTagPropertyDefinition Manager = PropertyTagPropertyDefinition.InternalCreate("Manager", PropTag.ManagerName);

		public static readonly PropertyTagPropertyDefinition MiddleName = PropertyTagPropertyDefinition.InternalCreate("MiddleName", PropTag.MiddleName, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 256))
		});

		public static readonly GuidIdPropertyDefinition Mileage = InternalSchema.CreateGuidIdProperty("Mileage", typeof(string), WellKnownPropertySet.Common, 34100);

		public static readonly PropertyTagPropertyDefinition MobilePhone = PropertyTagPropertyDefinition.InternalCreate("MobilePhone", PropTag.MobileTelephoneNumber);

		public static readonly PropertyTagPropertyDefinition Nickname = PropertyTagPropertyDefinition.InternalCreate("Nickname", PropTag.Nickname);

		public static readonly PropertyTagPropertyDefinition OfficeLocation = PropertyTagPropertyDefinition.InternalCreate("OfficeLocation", PropTag.OfficeLocation);

		public static readonly PropertyTagPropertyDefinition OrganizationMainPhone = PropertyTagPropertyDefinition.InternalCreate("OrganizationMainPhone", PropTag.CompanyMainPhoneNumber);

		public static readonly PropertyTagPropertyDefinition OtherPostOfficeBox = PropertyTagPropertyDefinition.InternalCreate("OtherPostOfficeBox", PropTag.OtherAddressPostOfficeBox);

		public static readonly PropertyTagPropertyDefinition OtherCity = PropertyTagPropertyDefinition.InternalCreate("OtherCity", PropTag.OtherAddressCity);

		public static readonly PropertyTagPropertyDefinition OtherCountry = PropertyTagPropertyDefinition.InternalCreate("OtherCountry", PropTag.OtherAddressCountry);

		public static readonly PropertyTagPropertyDefinition OtherFax = PropertyTagPropertyDefinition.InternalCreate("OtherFax", PropTag.PrimaryFaxNumber);

		public static readonly PropertyTagPropertyDefinition OtherPostalCode = PropertyTagPropertyDefinition.InternalCreate("OtherPostalCode", PropTag.OtherAddressPostalCode);

		public static readonly PropertyTagPropertyDefinition OtherState = PropertyTagPropertyDefinition.InternalCreate("OtherState", PropTag.OtherAddressStateOrProvince);

		public static readonly PropertyTagPropertyDefinition OtherStreet = PropertyTagPropertyDefinition.InternalCreate("OtherStreet", PropTag.OtherAddressStreet);

		public static readonly PropertyTagPropertyDefinition OtherTelephone = PropertyTagPropertyDefinition.InternalCreate("OtherTelephone", PropTag.OtherTelephoneNumber);

		public static readonly PropertyTagPropertyDefinition Pager = PropertyTagPropertyDefinition.InternalCreate("Pager", PropTag.PagerTelephoneNumber);

		public static readonly PropertyTagPropertyDefinition PartnerNetworkId = PropertyTagPropertyDefinition.InternalCreate("PartnerNetworkId", PropTag.PartnerNetworkId);

		public static readonly PropertyTagPropertyDefinition PartnerNetworkUserId = PropertyTagPropertyDefinition.InternalCreate("PartnerNetworkUserId", PropTag.PartnerNetworkUserId);

		public static readonly PropertyTagPropertyDefinition PartnerNetworkThumbnailPhotoUrl = PropertyTagPropertyDefinition.InternalCreate("PartnerNetworkThumbnailPhotoUrl", PropTag.PartnerNetworkThumbnailPhotoUrl);

		public static readonly PropertyTagPropertyDefinition PartnerNetworkProfilePhotoUrl = PropertyTagPropertyDefinition.InternalCreate("PartnerNetworkProfilePhotoUrl", PropTag.PartnerNetworkProfilePhotoUrl);

		public static readonly PropertyTagPropertyDefinition PartnerNetworkContactType = PropertyTagPropertyDefinition.InternalCreate("PartnerNetworkContactType", PropTag.PartnerNetworkContactType);

		public static readonly GuidNamePropertyDefinition GALContactType = InternalSchema.CreateGuidNameProperty("GALContactType", typeof(int), WellKnownPropertySet.PublicStrings, "GALContactType");

		public static readonly GuidNamePropertyDefinition GALObjectId = InternalSchema.CreateGuidNameProperty("GALObjectId", typeof(byte[]), WellKnownPropertySet.PublicStrings, "GALObjectId");

		public static readonly GuidNamePropertyDefinition GALRecipientType = InternalSchema.CreateGuidNameProperty("GALRecipientType", typeof(int), WellKnownPropertySet.PublicStrings, "GALRecipientType");

		public static readonly GuidNamePropertyDefinition GALHiddenFromAddressListsEnabled = InternalSchema.CreateGuidNameProperty("GALHiddenFromAddressListsEnabled", typeof(bool), WellKnownPropertySet.PublicStrings, "GALHiddenFromAddressListsEnabled");

		public static readonly GuidNamePropertyDefinition GALSpeechNormalizedNamesForDisplayName = InternalSchema.CreateGuidNameProperty("GALSpeechNormalizedNamesForDisplayName", typeof(byte[]), WellKnownPropertySet.PublicStrings, "GALSpeechNormalizedNamesForDisplayName", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition GALSpeechNormalizedNamesForPhoneticDisplayName = InternalSchema.CreateGuidNameProperty("GALSpeechNormalizedNamesForPhoneticDisplayName", typeof(byte[]), WellKnownPropertySet.PublicStrings, "GALSpeechNormalizedNamesForPhoneticDisplayName", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition GALUMDialplanId = InternalSchema.CreateGuidNameProperty("GALUMDialplanId", typeof(Guid), WellKnownPropertySet.PublicStrings, "GALUMDialplanId");

		public static readonly GuidNamePropertyDefinition GALCurrentSpeechGrammarVersion = InternalSchema.CreateGuidNameProperty("GALCurrentSpeechGrammarVersion", typeof(int), WellKnownPropertySet.PublicStrings, "GALCurrentSpeechGrammarVersion");

		public static readonly GuidNamePropertyDefinition GALPreviousSpeechGrammarVersion = InternalSchema.CreateGuidNameProperty("GALPreviousSpeechGrammarVersion", typeof(int), WellKnownPropertySet.PublicStrings, "GALPreviousSpeechGrammarVersion");

		public static readonly GuidNamePropertyDefinition GALCurrentUMDtmfMapVersion = InternalSchema.CreateGuidNameProperty("GALCurrentUMDtmfMapVersion", typeof(int), WellKnownPropertySet.PublicStrings, "GALCurrentUMDtmfMapVersion");

		public static readonly GuidNamePropertyDefinition GALPreviousUMDtmfMapVersion = InternalSchema.CreateGuidNameProperty("GALPreviousUMDtmfMapVersion", typeof(int), WellKnownPropertySet.PublicStrings, "GALPreviousUMDtmfMapVersion");

		public static readonly GuidIdPropertyDefinition PostalAddressId = InternalSchema.CreateGuidIdProperty("PostalAddressId", typeof(int), WellKnownPropertySet.Address, 32802);

		public static readonly PropertyTagPropertyDefinition Profession = PropertyTagPropertyDefinition.InternalCreate("Profession", PropTag.Profession);

		public static readonly PropertyTagPropertyDefinition RadioPhone = PropertyTagPropertyDefinition.InternalCreate("RadioPhone", PropTag.RadioTelephoneNumber);

		public static readonly PropertyTagPropertyDefinition SpouseName = PropertyTagPropertyDefinition.InternalCreate("SpouseName", PropTag.SpouseName);

		public static readonly PropertyTagPropertyDefinition Surname = PropertyTagPropertyDefinition.InternalCreate("Surname", PropTag.Surname, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 256))
		});

		public static readonly PropertyTagPropertyDefinition Title = PropertyTagPropertyDefinition.InternalCreate("Title", PropTag.Title);

		public static readonly PropertyTagPropertyDefinition WeddingAnniversary = PropertyTagPropertyDefinition.InternalCreate("WeddingAnniversary", PropTag.WeddingAnniversary);

		public static readonly GuidIdPropertyDefinition WeddingAnniversaryLocal = InternalSchema.CreateGuidIdProperty("WeddingAnniversaryLocal", typeof(ExDateTime), WellKnownPropertySet.Address, 32991);

		public static readonly PropertyTagPropertyDefinition WorkPostOfficeBox = PropertyTagPropertyDefinition.InternalCreate("WorkPostOfficeBox", PropTag.PostOfficeBox);

		public static readonly GuidIdPropertyDefinition WorkAddressCity = InternalSchema.CreateGuidIdProperty("WorkAddressCity", typeof(string), WellKnownPropertySet.Address, 32838);

		public static readonly GuidIdPropertyDefinition WorkAddressCountry = InternalSchema.CreateGuidIdProperty("WorkAddressCountry", typeof(string), WellKnownPropertySet.Address, 32841);

		public static readonly GuidIdPropertyDefinition WorkAddressPostalCode = InternalSchema.CreateGuidIdProperty("WorkAddressPostalCode", typeof(string), WellKnownPropertySet.Address, 32840);

		public static readonly GuidIdPropertyDefinition WorkAddressState = InternalSchema.CreateGuidIdProperty("WorkAddressState", typeof(string), WellKnownPropertySet.Address, 32839);

		public static readonly GuidIdPropertyDefinition WorkAddressStreet = InternalSchema.CreateGuidIdProperty("WorkAddressStreet", typeof(string), WellKnownPropertySet.Address, 32837);

		public static readonly PropertyTagPropertyDefinition Generation = PropertyTagPropertyDefinition.InternalCreate("Generation", PropTag.Generation);

		public static readonly GuidNamePropertyDefinition DisplayNameFirstLast = InternalSchema.CreateGuidNameProperty("DisplayNameFirstLast", typeof(string), WellKnownPropertySet.Address, "DisplayNameFirstLast");

		public static readonly GuidNamePropertyDefinition DisplayNameLastFirst = InternalSchema.CreateGuidNameProperty("DisplayNameLastFirst", typeof(string), WellKnownPropertySet.Address, "DisplayNameLastFirst");

		public static readonly GuidNamePropertyDefinition DisplayNamePriority = InternalSchema.CreateGuidNameProperty("DisplayNamePriority", typeof(int), WellKnownPropertySet.Address, "DisplayNamePriority");

		public static readonly GuidIdPropertyDefinition ProtectedEmailAddress = InternalSchema.CreateGuidIdProperty("ProtectedEmailAddress", typeof(string), WellKnownPropertySet.Address, 33008);

		public static readonly GuidIdPropertyDefinition ProtectedPhoneNumber = InternalSchema.CreateGuidIdProperty("ProtectedPhoneNumber", typeof(string), WellKnownPropertySet.Address, 33010);

		public static readonly GuidIdPropertyDefinition DLChecksum = InternalSchema.CreateGuidIdProperty("DLChecksum", typeof(int), WellKnownPropertySet.Address, 32844);

		public static readonly GuidIdPropertyDefinition Members = InternalSchema.CreateGuidIdProperty("Members", typeof(byte[][]), WellKnownPropertySet.Address, 32853);

		public static readonly GuidIdPropertyDefinition OneOffMembers = InternalSchema.CreateGuidIdProperty("OneOffMembers", typeof(byte[][]), WellKnownPropertySet.Address, 32852);

		public static readonly GuidIdPropertyDefinition DLName = InternalSchema.CreateGuidIdProperty("DLName", typeof(string), WellKnownPropertySet.Address, 32851);

		public static readonly GuidIdPropertyDefinition DLStream = InternalSchema.CreateGuidIdProperty("DLStream", typeof(byte[]), WellKnownPropertySet.Address, 32868, PropertyFlags.Streamable, PropertyDefinitionConstraint.None);

		public static readonly GuidNamePropertyDefinition DLAlias = InternalSchema.CreateGuidNameProperty("DLAlias", typeof(string), WellKnownPropertySet.PublicStrings, "DLAlias");

		public static readonly PropertyTagPropertyDefinition FolderQuota = PropertyTagPropertyDefinition.InternalCreate("FolderQuota", PropTag.PagePreread);

		public static readonly PropertyTagPropertyDefinition FolderSize = PropertyTagPropertyDefinition.InternalCreate("FolderSize", PropTag.PageRead);

		public static readonly PropertyTagPropertyDefinition FolderSizeExtended = PropertyTagPropertyDefinition.InternalCreate("FolderSizeExtended", PropTag.FolderSizeExtended);

		public static readonly PropertyTagPropertyDefinition FolderRulesSize = PropertyTagPropertyDefinition.InternalCreate("FolderRulesSize", PropTag.RulesSize);

		public static readonly PropertyTagPropertyDefinition FolderWebViewInfo = PropertyTagPropertyDefinition.InternalCreate("FolderWebViewInfo", (PropTag)920584450U);

		public static readonly PropertyTagPropertyDefinition HasChildren = PropertyTagPropertyDefinition.InternalCreate("HasSubFolders", PropTag.SubFolders, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ItemCount = PropertyTagPropertyDefinition.InternalCreate("ItemCount", PropTag.ContentCount, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition AssociatedItemCount = PropertyTagPropertyDefinition.InternalCreate("AssociatedItemCount", PropTag.AssocContentCount);

		public static readonly PropertyTagPropertyDefinition SearchFolderItemCount = PropertyTagPropertyDefinition.InternalCreate("SearchFolderItemCount", PropTag.SearchFolderMsgCount);

		public static readonly PropertyTagPropertyDefinition SearchBacklinkNames = PropertyTagPropertyDefinition.InternalCreate("SearchBacklinkNames", PropTag.SearchBacklinkNames);

		[Obsolete("Use InternalSchema.SearchFolderItemCount instead.")]
		public static readonly PropertyTagPropertyDefinition SearchFolderMessageCount = InternalSchema.SearchFolderItemCount;

		public static readonly PropertyTagPropertyDefinition SearchFolderAllowAgeout = PropertyTagPropertyDefinition.InternalCreate("SearchFolderAllowAgeout", PropTag.AllowAgeout);

		public static readonly PropertyTagPropertyDefinition SearchFolderAgeOutTimeout = PropertyTagPropertyDefinition.InternalCreate("SearchFolderAgeOutTimeout", PropTag.SearchFolderAgeOutTimeout);

		public static readonly PropertyTagPropertyDefinition IPMFolder = PropertyTagPropertyDefinition.InternalCreate("ptagIPMFolder", PropTag.IPMFolder);

		public static readonly PropertyTagPropertyDefinition MapiFolderType = PropertyTagPropertyDefinition.InternalCreate("MapiFolderType", PropTag.FolderType);

		public static readonly PropertyTagPropertyDefinition ChildCount = PropertyTagPropertyDefinition.InternalCreate("SubFolderCount", (PropTag)1714946051U, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition PublicFolderDumpsterHolderEntryId = PropertyTagPropertyDefinition.InternalCreate("PublicFolderDumpsterHolderEntryId", PropTag.CurrentIPMWasteBasketContainerEntryId);

		public static readonly PropertyTagPropertyDefinition UnreadCount = PropertyTagPropertyDefinition.InternalCreate("UnreadCount", PropTag.ContentUnread, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ContainerClass = PropertyTagPropertyDefinition.InternalCreate("ContainerClass", PropTag.ContainerClass, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255)
		});

		public static readonly PropertyTagPropertyDefinition ELCFolderComment = PropertyTagPropertyDefinition.InternalCreate("ELCFolderComment", PropTag.ELCPolicyComment);

		public static readonly PropertyTagPropertyDefinition ELCPolicyIds = PropertyTagPropertyDefinition.InternalCreate("ELCPolicyIds", (PropTag)1731330079U);

		public static readonly PropertyTagPropertyDefinition NextArticleId = PropertyTagPropertyDefinition.InternalCreate("NextArticleId", PropTag.NextArticleId);

		public static readonly PropertyTagPropertyDefinition ExtendedFolderFlagsInternal = PropertyTagPropertyDefinition.InternalCreate("ExtendedFolderFlagsInternal", (PropTag)920256770U);

		public static readonly PropertyTagPropertyDefinition PartOfContentIndexing = PropertyTagPropertyDefinition.InternalCreate("PartOfContentIndexing", (PropTag)910491659U);

		public static readonly PropertyTagPropertyDefinition UrlName = PropertyTagPropertyDefinition.InternalCreate("UrlName", PropTag.UrlName);

		public static readonly PropertyTagPropertyDefinition FolderPathName = PropertyTagPropertyDefinition.InternalCreate("FolderPathName", PropTag.FolderPathName);

		public static readonly PropertyTagPropertyDefinition UrlCompName = PropertyTagPropertyDefinition.InternalCreate("UrlCompName", (PropTag)284360735U);

		public static readonly PropertyTagPropertyDefinition UrlCompNamePostfix = PropertyTagPropertyDefinition.InternalCreate("UrlCompNamePostfix", (PropTag)241238019U);

		public static readonly PropertyTagPropertyDefinition OutOfOfficeHistory = PropertyTagPropertyDefinition.InternalCreate("OutOfOfficeHistory", PropTag.OofHistory, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition FolderFlags = PropertyTagPropertyDefinition.InternalCreate("FolderFlags", (PropTag)1722286083U);

		public static readonly PropertyTagPropertyDefinition ContentAggregationFlags = PropertyTagPropertyDefinition.InternalCreate("ContentAggregationFlags", PropTag.ContentAggregationFlags);

		public static readonly GuidNamePropertyDefinition PeopleIKnowEmailAddressCollection = InternalSchema.CreateGuidNameProperty("PeopleIKnowEmailAddressCollection", typeof(byte[]), WellKnownPropertySet.Address, "PeopleIKnowEmailAddressCollection");

		public static readonly GuidNamePropertyDefinition PeopleIKnowEmailAddressRelevanceScoreCollection = InternalSchema.CreateGuidNameProperty("PeopleIKnowEmailAddressRelevanceScoreCollection", typeof(byte[]), WellKnownPropertySet.Address, "PeopleIKnowEmailAddressRelevanceScoreCollection");

		public static readonly GuidNamePropertyDefinition OfficeGraphLocation = InternalSchema.CreateGuidNameProperty("OfficeGraphLocation", typeof(string), WellKnownPropertySet.Common, "OfficeGraphLocation");

		public static readonly GuidNamePropertyDefinition GalContactsFolderState = InternalSchema.CreateGuidNameProperty("GALContactsFolderState", typeof(int), WellKnownPropertySet.Common, "GALContactsFolderState");

		public static readonly GuidNamePropertyDefinition PermissionChangeBlocked = InternalSchema.CreateGuidNameProperty("PermissionChangeBlocked", typeof(bool), WellKnownPropertySet.ExternalSharing, "PermissionChangeBlocked");

		public static readonly GuidNamePropertyDefinition RecentBindingHistory = InternalSchema.CreateGuidNameProperty("RecentBindingHistory", typeof(string[]), WellKnownPropertySet.Elc, "RecentBindingHistory");

		public static readonly GuidNamePropertyDefinition PeopleHubSortGroupPriority = InternalSchema.CreateGuidNameProperty("PeopleHubSortGroupPriority", typeof(int), WellKnownPropertySet.Common, "PeopleHubSortGroupPriority");

		public static readonly GuidNamePropertyDefinition PeopleHubSortGroupPriorityVersion = InternalSchema.CreateGuidNameProperty("PeopleHubSortGroupPriorityVersion", typeof(int), WellKnownPropertySet.Common, "PeopleHubSortGroupPriorityVersion");

		public static readonly GuidNamePropertyDefinition IsPeopleConnectSyncFolder = InternalSchema.CreateGuidNameProperty("IsPeopleConnectSyncFolder", typeof(bool), WellKnownPropertySet.Common, "IsPeopleConnectSyncFolder");

		public static readonly GuidNamePropertyDefinition LocationFolderEntryId = InternalSchema.CreateGuidNameProperty("LocationFolderEntryId", typeof(byte[]), WellKnownPropertySet.Location, "LocationFolderEntryId");

		public static readonly GuidNamePropertyDefinition LocationRelevanceRank = InternalSchema.CreateGuidNameProperty("LocationRelevanceRank", typeof(int), WellKnownPropertySet.Location, "LocationRelevanceRank");

		public static readonly GuidNamePropertyDefinition LocationDisplayName = InternalSchema.CreateGuidNameProperty("LocationDisplayName", typeof(string), WellKnownPropertySet.Location, "LocationDisplayName");

		public static readonly GuidNamePropertyDefinition LocationAnnotation = InternalSchema.CreateGuidNameProperty("LocationAnnotation", typeof(string), WellKnownPropertySet.Location, "LocationAnnotation");

		public static readonly GuidNamePropertyDefinition LocationSource = InternalSchema.CreateGuidNameProperty("LocationSource", typeof(int), WellKnownPropertySet.Location, "LocationSource");

		public static readonly GuidNamePropertyDefinition LocationUri = InternalSchema.CreateGuidNameProperty("LocationUri", typeof(string), WellKnownPropertySet.Location, "LocationUri");

		public static readonly GuidNamePropertyDefinition Latitude = InternalSchema.CreateGuidNameProperty("Latitude", typeof(double), WellKnownPropertySet.Location, "Latitude");

		public static readonly GuidNamePropertyDefinition Longitude = InternalSchema.CreateGuidNameProperty("Longitude", typeof(double), WellKnownPropertySet.Location, "Longitude");

		public static readonly GuidNamePropertyDefinition Accuracy = InternalSchema.CreateGuidNameProperty("Accuracy", typeof(double), WellKnownPropertySet.Location, "Accuracy");

		public static readonly GuidNamePropertyDefinition Altitude = InternalSchema.CreateGuidNameProperty("Altitude", typeof(double), WellKnownPropertySet.Location, "Altitude");

		public static readonly GuidNamePropertyDefinition AltitudeAccuracy = InternalSchema.CreateGuidNameProperty("AltitudeAccuracy", typeof(double), WellKnownPropertySet.Location, "AltitudeAccuracy");

		public static readonly GuidNamePropertyDefinition LocationAddressInternal = InternalSchema.CreateGuidNameProperty("LocationAddressInternal", typeof(string), WellKnownPropertySet.Location, "LocationAddressInternal");

		public static readonly GuidNamePropertyDefinition LocationStreet = InternalSchema.CreateGuidNameProperty("StreetAddress", typeof(string), WellKnownPropertySet.Location, "StreetAddress");

		public static readonly GuidNamePropertyDefinition LocationCity = InternalSchema.CreateGuidNameProperty("LocationCity", typeof(string), WellKnownPropertySet.Location, "LocationCity");

		public static readonly GuidNamePropertyDefinition LocationState = InternalSchema.CreateGuidNameProperty("LocationState", typeof(string), WellKnownPropertySet.Location, "LocationState");

		public static readonly GuidNamePropertyDefinition LocationCountry = InternalSchema.CreateGuidNameProperty("LocationCountry", typeof(string), WellKnownPropertySet.Location, "LocationCountry");

		public static readonly GuidNamePropertyDefinition LocationPostalCode = InternalSchema.CreateGuidNameProperty("LocationPostalCode", typeof(string), WellKnownPropertySet.Location, "LocationPostalCode");

		public static readonly PhysicalAddressProperty LocationAddress = new PhysicalAddressProperty("LocationAddress", InternalSchema.LocationAddressInternal, InternalSchema.LocationStreet, InternalSchema.LocationCity, InternalSchema.LocationState, InternalSchema.LocationPostalCode, InternalSchema.LocationCountry);

		public static readonly GuidNamePropertyDefinition HomeLocationSource = InternalSchema.CreateGuidNameProperty("HomeLocationSource", typeof(int), WellKnownPropertySet.Location, "HomeLocationSource");

		public static readonly GuidNamePropertyDefinition HomeLocationUri = InternalSchema.CreateGuidNameProperty("HomeLocationUri", typeof(string), WellKnownPropertySet.Location, "HomeLocationUri");

		public static readonly GuidNamePropertyDefinition HomeLatitude = InternalSchema.CreateGuidNameProperty("HomeLatitude", typeof(double), WellKnownPropertySet.Location, "HomeLatitude", PropertyFlags.SetIfNotChanged, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<double>(-90.0, 90.0)
		});

		public static readonly GuidNamePropertyDefinition HomeLongitude = InternalSchema.CreateGuidNameProperty("HomeLongitude", typeof(double), WellKnownPropertySet.Location, "HomeLongitude", PropertyFlags.SetIfNotChanged, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<double>(-180.0, 180.0)
		});

		public static readonly GuidNamePropertyDefinition HomeAccuracy = InternalSchema.CreateGuidNameProperty("HomeAccuracy", typeof(double), WellKnownPropertySet.Location, "HomeAccuracy");

		public static readonly GuidNamePropertyDefinition HomeAltitude = InternalSchema.CreateGuidNameProperty("HomeAltitude", typeof(double), WellKnownPropertySet.Location, "HomeAltitude");

		public static readonly GuidNamePropertyDefinition HomeAltitudeAccuracy = InternalSchema.CreateGuidNameProperty("HomeAltitudeAccuracy", typeof(double), WellKnownPropertySet.Location, "HomeAltitudeAccuracy");

		public static readonly GuidNamePropertyDefinition WorkLocationSource = InternalSchema.CreateGuidNameProperty("WorkLocationSource", typeof(int), WellKnownPropertySet.Location, "WorkLocationSource");

		public static readonly GuidNamePropertyDefinition WorkLocationUri = InternalSchema.CreateGuidNameProperty("WorkLocationUri", typeof(string), WellKnownPropertySet.Location, "WorkLocationUri");

		public static readonly GuidNamePropertyDefinition WorkLatitude = InternalSchema.CreateGuidNameProperty("WorkLatitude", typeof(double), WellKnownPropertySet.Location, "WorkLatitude", PropertyFlags.SetIfNotChanged, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<double>(-90.0, 90.0)
		});

		public static readonly GuidNamePropertyDefinition WorkLongitude = InternalSchema.CreateGuidNameProperty("WorkLongitude", typeof(double), WellKnownPropertySet.Location, "WorkLongitude", PropertyFlags.SetIfNotChanged, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<double>(-180.0, 180.0)
		});

		public static readonly GuidNamePropertyDefinition WorkAccuracy = InternalSchema.CreateGuidNameProperty("WorkAccuracy", typeof(double), WellKnownPropertySet.Location, "WorkAccuracy");

		public static readonly GuidNamePropertyDefinition WorkAltitude = InternalSchema.CreateGuidNameProperty("WorkAltitude", typeof(double), WellKnownPropertySet.Location, "WorkAltitude");

		public static readonly GuidNamePropertyDefinition WorkAltitudeAccuracy = InternalSchema.CreateGuidNameProperty("WorkAltitudeAccuracy", typeof(double), WellKnownPropertySet.Location, "WorkAltitudeAccuracy");

		public static readonly GuidNamePropertyDefinition OtherLocationSource = InternalSchema.CreateGuidNameProperty("OtherLocationSource", typeof(int), WellKnownPropertySet.Location, "OtherLocationSource");

		public static readonly GuidNamePropertyDefinition OtherLocationUri = InternalSchema.CreateGuidNameProperty("OtherLocationUri", typeof(string), WellKnownPropertySet.Location, "OtherLocationUri");

		public static readonly GuidNamePropertyDefinition OtherLatitude = InternalSchema.CreateGuidNameProperty("OtherLatitude", typeof(double), WellKnownPropertySet.Location, "OtherLatitude", PropertyFlags.SetIfNotChanged, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<double>(-90.0, 90.0)
		});

		public static readonly GuidNamePropertyDefinition OtherLongitude = InternalSchema.CreateGuidNameProperty("OtherLongitude", typeof(double), WellKnownPropertySet.Location, "OtherLongitude", PropertyFlags.SetIfNotChanged, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<double>(-180.0, 180.0)
		});

		public static readonly GuidNamePropertyDefinition OtherAccuracy = InternalSchema.CreateGuidNameProperty("OtherAccuracy", typeof(double), WellKnownPropertySet.Location, "OtherAccuracy");

		public static readonly GuidNamePropertyDefinition OtherAltitude = InternalSchema.CreateGuidNameProperty("OtherAltitude", typeof(double), WellKnownPropertySet.Location, "OtherAltitude");

		public static readonly GuidNamePropertyDefinition OtherAltitudeAccuracy = InternalSchema.CreateGuidNameProperty("OtherAltitudeAccuracy", typeof(double), WellKnownPropertySet.Location, "OtherAltitudeAccuracy");

		public static readonly GuidNamePropertyDefinition Categories = InternalSchema.CreateGuidNameProperty("Categories", typeof(string[]), WellKnownPropertySet.PublicStrings, "Keywords", PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 255)),
			new NonMoveMailboxPropertyConstraint(new CharacterConstraint(Category.ProhibitedCharacters, false))
		});

		public static readonly GuidNamePropertyDefinition LinkedId = InternalSchema.CreateGuidNameProperty("LinkedId", typeof(Guid), WellKnownPropertySet.LinkedFolder, "LinkedId");

		public static readonly GuidNamePropertyDefinition LinkedUrl = InternalSchema.CreateGuidNameProperty("LinkedUrl", typeof(string), WellKnownPropertySet.LinkedFolder, "LinkedUrl");

		public static readonly GuidNamePropertyDefinition LinkedObjectVersion = InternalSchema.CreateGuidNameProperty("LinkedObjectVersion", typeof(string), WellKnownPropertySet.LinkedFolder, "LinkedObjectVersion");

		public static readonly GuidNamePropertyDefinition LinkedSiteUrl = InternalSchema.CreateGuidNameProperty("LinkedSiteUrl", typeof(string), WellKnownPropertySet.LinkedFolder, "LinkedSiteUrl");

		public static readonly GuidNamePropertyDefinition LinkedListId = InternalSchema.CreateGuidNameProperty("LinkedListId", typeof(Guid), WellKnownPropertySet.LinkedFolder, "LinkedListId");

		public static readonly GuidNamePropertyDefinition IsDocumentLibraryFolder = InternalSchema.CreateGuidNameProperty("IsDocumentLibraryFolder", typeof(bool), WellKnownPropertySet.LinkedFolder, "IsDocumentLibraryFolder");

		public static readonly GuidNamePropertyDefinition SharePointChangeToken = InternalSchema.CreateGuidNameProperty("SharePointChangeToken", typeof(string), WellKnownPropertySet.LinkedFolder, "SharePointChangeToken");

		public static readonly GuidNamePropertyDefinition LinkedItemUpdateHistory = InternalSchema.CreateGuidNameProperty("LinkedItemUpdateHistory", typeof(string[]), WellKnownPropertySet.LinkedFolder, "LinkedItemUpdateHistory");

		public static readonly GuidIdPropertyDefinition LinkedDocumentCheckOutTo = InternalSchema.CreateGuidIdProperty("LinkedDocumentCheckOutTo", typeof(string), WellKnownPropertySet.Common, 34241);

		public static readonly GuidIdPropertyDefinition LinkedDocumentSize = InternalSchema.CreateGuidIdProperty("LinkedDocumentSize", typeof(int), WellKnownPropertySet.Remote, 36613);

		public static readonly GuidIdPropertyDefinition LinkedPendingState = InternalSchema.CreateGuidIdProperty("LinkedPendingState", typeof(int), WellKnownPropertySet.Common, 34272);

		public static readonly GuidNamePropertyDefinition LinkedLastFullSyncTime = InternalSchema.CreateGuidNameProperty("LinkedLastFullSyncTime", typeof(ExDateTime), WellKnownPropertySet.LinkedFolder, "LinkedLastFullSyncTime");

		public static readonly PropertyTagPropertyDefinition LinkedSiteAuthorityUrl = PropertyTagPropertyDefinition.InternalCreate("LinkedSiteAuthorityUrl", PropTag.LinkedSiteAuthorityUrl);

		public static readonly GuidNamePropertyDefinition UnifiedPolicyNotificationId = InternalSchema.CreateGuidNameProperty("UnifiedPolicyNotificationId", typeof(string), WellKnownPropertySet.UnifiedPolicy, "UnifiedPolicyNotificationId");

		public static readonly GuidNamePropertyDefinition UnifiedPolicyNotificationData = InternalSchema.CreateGuidNameProperty("UnifiedPolicyNotificationData", typeof(byte[]), WellKnownPropertySet.UnifiedPolicy, "UnifiedPolicyNotificationData", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly PropertyTagPropertyDefinition ControlDataForSiteMailboxAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForSiteMailboxAssistant", PropTag.ControlDataForSiteMailboxAssistant);

		public static readonly PropertyTagPropertyDefinition Description = PropertyTagPropertyDefinition.InternalCreate("Description", PropTag.Comment);

		public static readonly PropertyTagPropertyDefinition MapiImportance = PropertyTagPropertyDefinition.InternalCreate("MapiImportance", PropTag.Importance);

		public static readonly PropertyTagPropertyDefinition MapiSensitivity = PropertyTagPropertyDefinition.InternalCreate("MapiSensitivity", PropTag.Sensitivity);

		public static readonly PropertyTagPropertyDefinition MessageAttachments = PropertyTagPropertyDefinition.InternalCreate("MessageAttachments", PropTag.MessageAttachments);

		public static readonly PropertyTagPropertyDefinition MessageDeepAttachments = PropertyTagPropertyDefinition.InternalCreate("MessageDeepAttachments", PropTag.MessageDeepAttachments);

		public static readonly PropertyTagPropertyDefinition Size = PropertyTagPropertyDefinition.InternalCreate("Size", PropTag.MessageSize, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ExtendedSize = PropertyTagPropertyDefinition.InternalCreate("ExtendedSize", PropTag.MessageSizeExtended, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ExtendedDumpsterSize = PropertyTagPropertyDefinition.InternalCreate("ExtendedDumpsterSize", PropTag.DeletedMessageSizeExtended, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ExtendedAssociatedItemSize = PropertyTagPropertyDefinition.InternalCreate("ExtendedAssociatedItemSize", PropTag.AssocMessageSizeExtended, PropertyFlags.ReadOnly);

		public static readonly GuidIdPropertyDefinition Privacy = InternalSchema.CreateGuidIdProperty("Privacy", typeof(bool), WellKnownPropertySet.Common, 34054);

		public static readonly PropertyTagPropertyDefinition AccessRights = PropertyTagPropertyDefinition.InternalCreate("AccessRights", (PropTag)1715011587U);

		public static readonly PropertyTagPropertyDefinition AccessLevel = PropertyTagPropertyDefinition.InternalCreate("AccessLevel", PropTag.AccessLevel, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ChangeKey = PropertyTagPropertyDefinition.InternalCreate("ChangeKey", PropTag.ChangeKey, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ArticleId = PropertyTagPropertyDefinition.InternalCreate("ArticleId", PropTag.InternetArticleNumber);

		public static readonly PropertyTagPropertyDefinition ImapId = PropertyTagPropertyDefinition.InternalCreate("ImapId", PropTag.ImapId);

		public static readonly PropertyTagPropertyDefinition OriginalSourceServerVersion = PropertyTagPropertyDefinition.InternalCreate("OriginalSourceServerVersion", PropTag.OriginalSourceServerVersion);

		public static readonly GuidIdPropertyDefinition MarkedForDownload = InternalSchema.CreateGuidIdProperty("MarkedForDownload", typeof(int), WellKnownPropertySet.Common, 34161);

		public static readonly PropertyTagPropertyDefinition ObjectType = PropertyTagPropertyDefinition.InternalCreate("ObjectType", PropTag.ObjectType);

		public static readonly PropertyTagPropertyDefinition RowId = PropertyTagPropertyDefinition.InternalCreate("RowId", PropTag.RowId);

		public static readonly PropertyTagPropertyDefinition RowType = PropertyTagPropertyDefinition.InternalCreate("RowType", PropTag.RowType);

		public static readonly PropertyTagPropertyDefinition SyncCustomState = PropertyTagPropertyDefinition.InternalCreate("SyncCustomState", PropTag.FavPublicSourceKey, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition SyncFolderChangeKey = PropertyTagPropertyDefinition.InternalCreate("SyncFolderChangeKey", (PropTag)2080637186U);

		public static readonly PropertyTagPropertyDefinition SyncFolderLastModificationTime = PropertyTagPropertyDefinition.InternalCreate("SyncFolderLastModificationTime", PropTag.DeletedMessageSizeExtendedLastModificationTime);

		public static readonly PropertyTagPropertyDefinition SyncFolderSourceKey = PropertyTagPropertyDefinition.InternalCreate("SyncFolderSourceId", (PropTag)2080571650U);

		public static readonly PropertyTagPropertyDefinition SyncState = PropertyTagPropertyDefinition.InternalCreate("SyncState", (PropTag)2081030402U, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition ImapInternalDate = PropertyTagPropertyDefinition.InternalCreate("ImapInternalDate", PropTag.ImapInternalDate);

		public static readonly PropertyTagPropertyDefinition ImapLastSeenArticleId = PropertyTagPropertyDefinition.InternalCreate("ImapLastSeenArticleId", PropTag.ImapLastArticleId);

		public static readonly PropertyTagPropertyDefinition ImapSubscribeList = PropertyTagPropertyDefinition.InternalCreate("ImapSubscribeList", PropTag.ImapSubscribeList);

		public static readonly GuidNamePropertyDefinition ProtocolLog = InternalSchema.CreateGuidNameProperty("ProtocolLog", typeof(byte[]), WellKnownPropertySet.IMAPMsg, "ProtocolLog", PropertyFlags.Streamable, PropertyDefinitionConstraint.None);

		public static readonly PropertyTagPropertyDefinition MailboxOofStateInternal = PropertyTagPropertyDefinition.InternalCreate("PR_OOF_STATE", PropTag.OofState);

		public static readonly PropertyTagPropertyDefinition UserName = PropertyTagPropertyDefinition.InternalCreate("PR_USER_NAME", PropTag.UserDisplayName);

		public static readonly PropertyTagPropertyDefinition MailboxOofStateEx = PropertyTagPropertyDefinition.InternalCreate("PR_OOF_STATE_EX", PropTag.OofStateEx);

		public static readonly PropertyTagPropertyDefinition UserOofSettingsItemId = PropertyTagPropertyDefinition.InternalCreate("PR_OOF_SETTINGS_ITEM_ID", PropTag.UserOofSettingsItemId);

		public static readonly PropertyTagPropertyDefinition MailboxOofStateUserChangeTime = PropertyTagPropertyDefinition.InternalCreate("PR_OOF_STATE_USER_CHANGE_TIME", PropTag.OofStateUserChangeTime);

		public static readonly PropertyTagPropertyDefinition IsContentIndexingEnabled = PropertyTagPropertyDefinition.InternalCreate("IsContentIndexingEnabled", PropTag.CISearchEnabled);

		public static readonly PropertyTagPropertyDefinition IsMailboxLocalized = PropertyTagPropertyDefinition.InternalCreate("IsMailboxLocalized", PropTag.Localized);

		public static readonly GuidIdPropertyDefinition UnifiedMessagingOptions = InternalSchema.CreateGuidIdProperty("UnifiedMessagingOptions", typeof(int), WellKnownPropertySet.UnifiedMessaging, 1);

		public static readonly GuidIdPropertyDefinition OfficeCommunicatorOptions = InternalSchema.CreateGuidIdProperty("OfficeCommunicatorOptions", typeof(int), WellKnownPropertySet.UnifiedMessaging, 2);

		public static readonly PropertyTagPropertyDefinition DefaultFoldersLocaleId = PropertyTagPropertyDefinition.InternalCreate("DefaultFolderLocaleId", PropTag.DefaultFoldersLocaleId);

		public static readonly PropertyTagPropertyDefinition SendReadNotifications = PropertyTagPropertyDefinition.InternalCreate("SendReadNotifications", PropTag.InternetMdns);

		public static readonly PropertyTagPropertyDefinition DraftsFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("DraftsFolderEntryId", PropTag.DraftsFolderEntryId);

		public static readonly PropertyTagPropertyDefinition AdditionalRenEntryIds = PropertyTagPropertyDefinition.InternalCreate("AdditionalRenEntryIds", (PropTag)920129794U);

		public static readonly PropertyTagPropertyDefinition AdditionalRenEntryIdsEx = PropertyTagPropertyDefinition.InternalCreate("AdditionalRenEntryIdsEx", (PropTag)920191234U);

		public static readonly PropertyTagPropertyDefinition RemindersSearchFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("RemindersSearchFolderEntryId", (PropTag)919929090U);

		public static readonly PropertyTagPropertyDefinition DeferredActionFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("DeferredActionFolderEntryId", PropTag.DeferredActionFolderEntryID);

		public static readonly PropertyTagPropertyDefinition LegacyScheduleFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("LegacyScheduleFolderEntryId", (PropTag)1713242370U);

		public static readonly PropertyTagPropertyDefinition LegacyShortcutsFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("LegacyShortcutsFolderEntryId", PropTag.LegacyShortcutsFolderEntryId);

		public static readonly PropertyTagPropertyDefinition LegacyViewsFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("LegacyViewsFolderEntryId", PropTag.ViewsEntryId);

		public static readonly PropertyTagPropertyDefinition DeletedItemsEntryId = PropertyTagPropertyDefinition.InternalCreate("DeletedItemsEntryId", PropTag.IpmWasteBasketEntryId);

		public static readonly PropertyTagPropertyDefinition SentItemsEntryId = PropertyTagPropertyDefinition.InternalCreate("SentItemsEntryId", PropTag.IpmSentMailEntryId);

		public static readonly PropertyTagPropertyDefinition OutboxEntryId = PropertyTagPropertyDefinition.InternalCreate("OutboxEntryId", PropTag.IpmOutboxEntryId);

		public static readonly PropertyTagPropertyDefinition CalendarFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("CalendarFolderId", PropTag.CalendarFolderEntryId);

		public static readonly PropertyTagPropertyDefinition ContactsFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("ContactsFolderEntryId", PropTag.ContactsFolderEntryId);

		public static readonly PropertyTagPropertyDefinition JournalFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("JournalFolderEntryId", PropTag.JournalFolderEntryId);

		public static readonly PropertyTagPropertyDefinition NotesFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("NotesFolderEntryId", PropTag.NotesFolderEntryId);

		public static readonly PropertyTagPropertyDefinition TasksFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("TasksFolderEntryId", PropTag.TasksFolderEntryId);

		public static readonly PropertyTagPropertyDefinition FinderEntryId = PropertyTagPropertyDefinition.InternalCreate("FinderEntryId", PropTag.FinderEntryId);

		public static readonly PropertyTagPropertyDefinition CommonViewsEntryId = PropertyTagPropertyDefinition.InternalCreate("CommonViewsEntryId", PropTag.CommonViewsEntryId);

		public static readonly PropertyTagPropertyDefinition ElcRootFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("ElcRootFolderEntryId", PropTag.SpoolerQueueEntryId);

		public static readonly GuidNamePropertyDefinition RetentionTagEntryId = InternalSchema.CreateGuidNameProperty("RetentionTagEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "RetentionTagEntryId");

		public static readonly PropertyTagPropertyDefinition CommunicatorHistoryFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("CommunicatorHistoryFolderEntryId", (PropTag)904462594U);

		public static readonly PropertyTagPropertyDefinition SyncRootFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("SyncRootFolderEntryId", (PropTag)904528130U);

		public static readonly PropertyTagPropertyDefinition UMVoicemailFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("UMVoicemailFolderEntryId", (PropTag)904593666U);

		public static readonly PropertyTagPropertyDefinition UMFaxFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("UMFaxFolderEntryId", (PropTag)918487298U);

		public static readonly PropertyTagPropertyDefinition SharingFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("SharingFolderEntryId", PropTag.SharingFolderEntryId);

		public static readonly PropertyTagPropertyDefinition AllItemsFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("AllItemsFolderEntryId", PropTag.AllItemsEntryId);

		public static readonly GuidNamePropertyDefinition RecoverableItemsRootFolderEntryId = InternalSchema.CreateGuidNameProperty("DumpsterEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "DumpsterEntryId");

		public static readonly GuidNamePropertyDefinition RecoverableItemsDeletionsFolderEntryId = InternalSchema.CreateGuidNameProperty("RecoverableItemsDeletionsEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "RecoverableItemsDeletionsEntryId");

		public static readonly GuidNamePropertyDefinition RecoverableItemsVersionsFolderEntryId = InternalSchema.CreateGuidNameProperty("RecoverableItemsVersionsEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "RecoverableItemsVersionsEntryId");

		public static readonly GuidNamePropertyDefinition RecoverableItemsPurgesFolderEntryId = InternalSchema.CreateGuidNameProperty("RecoverableItemsPurgesEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "RecoverableItemsPurgesEntryId");

		public static readonly GuidNamePropertyDefinition RecoverableItemsDiscoveryHoldsFolderEntryId = InternalSchema.CreateGuidNameProperty("RecoverableItemsDiscoveryHoldsEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "RecoverableItemsDiscoveryHoldsEntryId");

		public static readonly GuidNamePropertyDefinition RecoverableItemsMigratedMessagesFolderEntryId = InternalSchema.CreateGuidNameProperty("RecoverableItemsMigratedMessagesEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "RecoverableItemsMigratedMessagesEntryId");

		public static readonly GuidNamePropertyDefinition CalendarLoggingFolderEntryId = InternalSchema.CreateGuidNameProperty("CalendarLoggingEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "CalendarLoggingEntryId");

		public static readonly PropertyTagPropertyDefinition SystemFolderEntryId = PropertyTagPropertyDefinition.InternalCreate("SystemFolderEntryId", (PropTag)905773314U);

		public static readonly GuidNamePropertyDefinition CalendarVersionStoreFolderEntryId = InternalSchema.CreateGuidNameProperty("CalendarVersionStoreEntryId", typeof(byte[]), WellKnownPropertySet.CalendarAssistant, "CalendarVersionStoreEntryId");

		public static readonly GuidNamePropertyDefinition AdminAuditLogsFolderEntryId = InternalSchema.CreateGuidNameProperty("AdminAuditLogsFolderEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "AdminAuditLogsFolderEntryId");

		public static readonly GuidNamePropertyDefinition AuditsFolderEntryId = InternalSchema.CreateGuidNameProperty("AuditsFolderEntryId", typeof(byte[]), WellKnownPropertySet.Elc, "AuditsFolderEntryId");

		public static readonly GuidNamePropertyDefinition RecipientCacheFolderEntryId = InternalSchema.CreateGuidNameProperty("RecipientCacheFolderEntryId", typeof(byte[]), WellKnownPropertySet.Common, "RecipientCacheFolderEntryId");

		public static readonly GuidNamePropertyDefinition SmsAndChatsSyncFolderEntryId = InternalSchema.CreateGuidNameProperty("SmsAndChatsSyncFolderEntryId", typeof(byte[]), WellKnownPropertySet.Common, "SmsAndChatsSyncFolderEntryId");

		public static readonly GuidNamePropertyDefinition GalContactsFolderEntryId = InternalSchema.CreateGuidNameProperty("GALContactsFolderEntryId", typeof(byte[]), WellKnownPropertySet.Common, "GALContactsFolderEntryId");

		public static readonly GuidNamePropertyDefinition QuickContactsFolderEntryId = InternalSchema.CreateGuidNameProperty("QuickContactsFolderEntryId", typeof(byte[]), WellKnownPropertySet.UnifiedContactStore, "QuickContactsFolderEntryId");

		public static readonly GuidNamePropertyDefinition ImContactListFolderEntryId = InternalSchema.CreateGuidNameProperty("ImContactListFolderEntryId", typeof(byte[]), WellKnownPropertySet.UnifiedContactStore, "ImContactListFolderEntryId");

		public static readonly GuidNamePropertyDefinition OrganizationalContactsFolderEntryId = InternalSchema.CreateGuidNameProperty("OrganizationalContactsFolderEntryId", typeof(byte[]), WellKnownPropertySet.UnifiedContactStore, "OrganizationalContactsFolderEntryId");

		public static readonly GuidNamePropertyDefinition LegacyArchiveJournalsFolderEntryId = InternalSchema.CreateGuidNameProperty("LegacyArchiveJournalsFolderEntryId", typeof(byte[]), WellKnownPropertySet.Messaging, "LegacyArchiveJournalsFolderEntryId");

		public static readonly GuidNamePropertyDefinition DocumentSyncIssuesFolderEntryId = InternalSchema.CreateGuidNameProperty("DocumentSyncIssuesFolderEntryId", typeof(byte[]), WellKnownPropertySet.LinkedFolder, "DocumentSyncIssuesFolderEntryId");

		public static readonly GuidIdPropertyDefinition AttendeeCriticalChangeTime = InternalSchema.CreateGuidIdProperty("AttendeeCriticalChangeTime", typeof(ExDateTime), WellKnownPropertySet.Meeting, 1);

		public static readonly GuidIdPropertyDefinition CalendarProcessed = InternalSchema.CreateGuidIdProperty("CalendarProcessed", typeof(bool), WellKnownPropertySet.CalendarAssistant, 1);

		public static readonly GuidIdPropertyDefinition CalendarProcessingSteps = InternalSchema.CreateGuidIdProperty("CalendarProcessingSteps", typeof(int), WellKnownPropertySet.CalendarAssistant, 3);

		public static readonly GuidIdPropertyDefinition OriginalMeetingType = InternalSchema.CreateGuidIdProperty("OriginalMeetingType", typeof(int), WellKnownPropertySet.CalendarAssistant, 4);

		public static readonly GuidIdPropertyDefinition ChangeList = InternalSchema.CreateGuidIdProperty("ChangeList", typeof(byte[]), WellKnownPropertySet.CalendarAssistant, 5);

		public static readonly GuidIdPropertyDefinition CalendarLogTriggerAction = InternalSchema.CreateGuidIdProperty("CalendarLogTriggerAction", typeof(string), WellKnownPropertySet.CalendarAssistant, 6);

		public static readonly GuidIdPropertyDefinition OriginalFolderId = InternalSchema.CreateGuidIdProperty("OriginalFolderId", typeof(byte[]), WellKnownPropertySet.CalendarAssistant, 7);

		public static readonly GuidIdPropertyDefinition OriginalCreationTime = InternalSchema.CreateGuidIdProperty("OriginalCreationTime", typeof(ExDateTime), WellKnownPropertySet.CalendarAssistant, 8);

		public static readonly GuidIdPropertyDefinition OriginalLastModifiedTime = InternalSchema.CreateGuidIdProperty("OriginalLastModifiedTime", typeof(ExDateTime), WellKnownPropertySet.CalendarAssistant, 9);

		public static readonly GuidIdPropertyDefinition ResponsibleUserName = InternalSchema.CreateGuidIdProperty("ResponsibleUserName", typeof(string), WellKnownPropertySet.CalendarAssistant, 10);

		public static readonly GuidIdPropertyDefinition ClientInfoString = InternalSchema.CreateGuidIdProperty("ClientInfoString", typeof(string), WellKnownPropertySet.CalendarAssistant, 11);

		public static readonly GuidIdPropertyDefinition ClientProcessName = InternalSchema.CreateGuidIdProperty("ClientProcessName", typeof(string), WellKnownPropertySet.CalendarAssistant, 12);

		public static readonly GuidIdPropertyDefinition ClientMachineName = InternalSchema.CreateGuidIdProperty("ClientMachineName", typeof(string), WellKnownPropertySet.CalendarAssistant, 13);

		public static readonly GuidIdPropertyDefinition ClientBuildVersion = InternalSchema.CreateGuidIdProperty("ClientBuildVersion", typeof(string), WellKnownPropertySet.CalendarAssistant, 14);

		public static readonly GuidIdPropertyDefinition MiddleTierProcessName = InternalSchema.CreateGuidIdProperty("MiddleTierProcessName", typeof(string), WellKnownPropertySet.CalendarAssistant, 15);

		public static readonly GuidIdPropertyDefinition MiddleTierServerName = InternalSchema.CreateGuidIdProperty("MiddleTierServerName", typeof(string), WellKnownPropertySet.CalendarAssistant, 16);

		public static readonly GuidIdPropertyDefinition MiddleTierServerBuildVersion = InternalSchema.CreateGuidIdProperty("MiddleTierServerBuildVersion", typeof(string), WellKnownPropertySet.CalendarAssistant, 17);

		public static readonly GuidIdPropertyDefinition MailboxServerName = InternalSchema.CreateGuidIdProperty("ServerName", typeof(string), WellKnownPropertySet.CalendarAssistant, 18);

		public static readonly GuidIdPropertyDefinition MailboxServerBuildVersion = InternalSchema.CreateGuidIdProperty("ServerBuildVersion", typeof(string), WellKnownPropertySet.CalendarAssistant, 19);

		public static readonly GuidIdPropertyDefinition MailboxDatabaseName = InternalSchema.CreateGuidIdProperty("MailboxDatabaseName", typeof(string), WellKnownPropertySet.CalendarAssistant, 20);

		public static readonly GuidIdPropertyDefinition ClientIntent = InternalSchema.CreateGuidIdProperty("ClientIntent", typeof(int), WellKnownPropertySet.CalendarAssistant, 21, PropertyFlags.SetIfNotChanged, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition ItemVersion = InternalSchema.CreateGuidIdProperty("ItemVersion", typeof(int), WellKnownPropertySet.CalendarAssistant, 22);

		public static readonly GuidIdPropertyDefinition OriginalEntryId = InternalSchema.CreateGuidIdProperty("OriginalEntryId", typeof(byte[]), WellKnownPropertySet.CalendarAssistant, 23);

		public static readonly GuidIdPropertyDefinition CalendarOriginatorId = InternalSchema.CreateGuidIdProperty("CalendarOriginatorId", typeof(string), WellKnownPropertySet.CalendarAssistant, 24, PropertyFlags.TrackChange, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition HijackedMeeting = InternalSchema.CreateGuidIdProperty("HijackedMeeting", typeof(bool), WellKnownPropertySet.CalendarAssistant, 25);

		public static readonly GuidIdPropertyDefinition MFNAddedRecipients = InternalSchema.CreateGuidIdProperty("MFNAddedRecipients", typeof(byte[]), WellKnownPropertySet.CalendarAssistant, 32, PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition ViewStartTime = InternalSchema.CreateGuidIdProperty("ViewStartTime", typeof(ExDateTime), WellKnownPropertySet.CalendarAssistant, 33);

		public static readonly GuidIdPropertyDefinition ViewEndTime = InternalSchema.CreateGuidIdProperty("ViewEndTime", typeof(ExDateTime), WellKnownPropertySet.CalendarAssistant, 34);

		public static readonly GuidIdPropertyDefinition CalendarFolderVersion = InternalSchema.CreateGuidIdProperty("CalendarFolderVersion", typeof(int), WellKnownPropertySet.CalendarAssistant, 35);

		public static readonly GuidIdPropertyDefinition HasAttendees = InternalSchema.CreateGuidIdProperty("HasAttendees", typeof(bool), WellKnownPropertySet.CalendarAssistant, 36);

		public static readonly GuidIdPropertyDefinition CharmId = InternalSchema.CreateGuidIdProperty("CharmId", typeof(string), WellKnownPropertySet.CalendarAssistant, 37);

		public static readonly GuidNamePropertyDefinition CalendarInteropActionQueueInternal = InternalSchema.CreateGuidNameProperty("CalendarInteropActionQueueInternal", typeof(byte[]), WellKnownPropertySet.CalendarAssistant, "CalendarInteropActionQueueInternal", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition CalendarInteropActionQueueHasDataInternal = InternalSchema.CreateGuidNameProperty("CalendarInteropActionQueueHasDataInternal", typeof(bool), WellKnownPropertySet.CalendarAssistant, "CalendarInteropActionQueueHasDataInternal");

		public static readonly GuidNamePropertyDefinition LastExecutedCalendarInteropAction = InternalSchema.CreateGuidNameProperty("LastExecutedCalendarInteropAction", typeof(Guid), WellKnownPropertySet.CalendarAssistant, "LastExecutedCalendarInteropAction");

		public static readonly GuidIdPropertyDefinition ChangeHighlight = InternalSchema.CreateGuidIdProperty("ChangeHighlight", typeof(int), WellKnownPropertySet.Appointment, 33284);

		public static readonly GuidIdPropertyDefinition IntendedFreeBusyStatus = InternalSchema.CreateGuidIdProperty("IntendedFreeBusyStatus", typeof(int), WellKnownPropertySet.Appointment, 33316);

		public static readonly PropertyTagPropertyDefinition IsProcessed = PropertyTagPropertyDefinition.InternalCreate("IsProcessed", (PropTag)2097217547U);

		public static readonly GuidIdPropertyDefinition MeetingRequestType = InternalSchema.CreateGuidIdProperty("MeetingRequestType", typeof(int), WellKnownPropertySet.Meeting, 38);

		public static readonly GuidIdPropertyDefinition OldLocation = InternalSchema.CreateGuidIdProperty("OldLocation", typeof(string), WellKnownPropertySet.Meeting, 40);

		public static readonly GuidIdPropertyDefinition OldStartWhole = InternalSchema.CreateGuidIdProperty("OldStartWhole", typeof(ExDateTime), WellKnownPropertySet.Meeting, 41);

		public static readonly GuidIdPropertyDefinition OldEndWhole = InternalSchema.CreateGuidIdProperty("OldEndWhole", typeof(ExDateTime), WellKnownPropertySet.Meeting, 42);

		public static readonly PropertyTagPropertyDefinition OwnerAppointmentID = PropertyTagPropertyDefinition.InternalCreate("OwnerAppointmentID", PropTag.OwnerApptId);

		public static readonly GuidIdPropertyDefinition OwnerCriticalChangeTime = InternalSchema.CreateGuidIdProperty("OwnerCriticalChangeTime", typeof(ExDateTime), WellKnownPropertySet.Meeting, 26);

		public static readonly GuidIdPropertyDefinition LidSingleInvite = InternalSchema.CreateGuidIdProperty("LID_SINGLE_INVITE", typeof(bool), WellKnownPropertySet.Meeting, 11);

		public static readonly GuidIdPropertyDefinition LidTimeZone = InternalSchema.CreateGuidIdProperty("LID_TIME_ZONE", typeof(int), WellKnownPropertySet.Meeting, 12);

		public static readonly GuidIdPropertyDefinition StartRecurDate = InternalSchema.CreateGuidIdProperty("LID_START_RECUR_DATE", typeof(int), WellKnownPropertySet.Meeting, 13);

		public static readonly GuidIdPropertyDefinition StartRecurTime = InternalSchema.CreateGuidIdProperty("LID_START_RECUR_TIME", typeof(int), WellKnownPropertySet.Meeting, 14);

		public static readonly GuidIdPropertyDefinition EndRecurDate = InternalSchema.CreateGuidIdProperty("LID_END_RECUR_DATE", typeof(int), WellKnownPropertySet.Meeting, 15);

		public static readonly GuidIdPropertyDefinition EndRecurTime = InternalSchema.CreateGuidIdProperty("LID_END_RECUR_TIME", typeof(int), WellKnownPropertySet.Meeting, 16);

		public static readonly GuidIdPropertyDefinition LidDayInterval = InternalSchema.CreateGuidIdProperty("LID_DAY_INTERVAL", typeof(short), WellKnownPropertySet.Meeting, 17);

		public static readonly GuidIdPropertyDefinition LidWeekInterval = InternalSchema.CreateGuidIdProperty("LID_WEEK_INTERVAL", typeof(short), WellKnownPropertySet.Meeting, 18);

		public static readonly GuidIdPropertyDefinition LidMonthInterval = InternalSchema.CreateGuidIdProperty("LID_MONTH_INTERVAL", typeof(short), WellKnownPropertySet.Meeting, 19);

		public static readonly GuidIdPropertyDefinition LidYearInterval = InternalSchema.CreateGuidIdProperty("LID_YEAR_INTERVAL", typeof(short), WellKnownPropertySet.Meeting, 20);

		public static readonly GuidIdPropertyDefinition LidDayOfWeekMask = InternalSchema.CreateGuidIdProperty("LID_DOW_MASK", typeof(int), WellKnownPropertySet.Meeting, 21);

		public static readonly GuidIdPropertyDefinition LidDayOfMonthMask = InternalSchema.CreateGuidIdProperty("LID_DOM_MASK", typeof(int), WellKnownPropertySet.Meeting, 22);

		public static readonly GuidIdPropertyDefinition LidMonthOfYearMask = InternalSchema.CreateGuidIdProperty("LID_MOY_MASK", typeof(int), WellKnownPropertySet.Meeting, 23);

		public static readonly GuidIdPropertyDefinition LidRecurType = InternalSchema.CreateGuidIdProperty("LID_RECUR_TYPE", typeof(short), WellKnownPropertySet.Meeting, 24);

		public static readonly GuidIdPropertyDefinition LidFirstDayOfWeek = InternalSchema.CreateGuidIdProperty("LID_DOW_PREF", typeof(short), WellKnownPropertySet.Meeting, 25);

		public static readonly PropertyTagPropertyDefinition AddrType = PropertyTagPropertyDefinition.InternalCreate("AddrType", PropTag.AddrType, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 64))
		});

		public static readonly PropertyTagPropertyDefinition RecipientFlags = PropertyTagPropertyDefinition.InternalCreate("RecipientFlags", PropTag.RecipientFlags);

		public static readonly PropertyTagPropertyDefinition RecipientTrackStatus = PropertyTagPropertyDefinition.InternalCreate("RecipientTrackStatus", PropTag.RecipientTrackStatus);

		public static readonly PropertyTagPropertyDefinition RecipientTrackStatusTime = PropertyTagPropertyDefinition.InternalCreate("RecipientTrackStatusTime", PropTag.RecipientTrackStatusTime);

		public static readonly PropertyTagPropertyDefinition RecipientType = PropertyTagPropertyDefinition.InternalCreate("RecipientType", PropTag.RecipientType);

		public static readonly PropertyTagPropertyDefinition SmtpAddress = PropertyTagPropertyDefinition.InternalCreate("SmtpAddress", PropTag.SmtpAddress);

		public static readonly PropertyTagPropertyDefinition SenderSmtpAddress = PropertyTagPropertyDefinition.InternalCreate("SenderSmtpAddress", PropTag.SenderSmtpAddress);

		public static readonly PropertyTagPropertyDefinition SentRepresentingSmtpAddress = PropertyTagPropertyDefinition.InternalCreate("SentRepresentingSmtpAddress", PropTag.SentRepresentingSmtpAddress);

		public static readonly PropertyTagPropertyDefinition ReceivedBySmtpAddress = PropertyTagPropertyDefinition.InternalCreate("ReceivedBySmtpAddress", PropTag.ReceivedBySmtpAddress);

		public static readonly PropertyTagPropertyDefinition ReceivedRepresentingSmtpAddress = PropertyTagPropertyDefinition.InternalCreate("ReceivedRepresentingSmtpAddress", PropTag.RcvdRepresentingSmtpAddress);

		public static readonly PropertyTagPropertyDefinition OriginalSentRepresentingSmtpAddress = PropertyTagPropertyDefinition.InternalCreate("OriginalSentRepresentingSmtpAddress", (PropTag)1560543263U);

		public static readonly PropertyTagPropertyDefinition OriginalSenderSmtpAddress = PropertyTagPropertyDefinition.InternalCreate("OriginalSenderSmtpAddress", (PropTag)1560477727U);

		public static readonly PropertyTagPropertyDefinition OriginalAuthorSmtpAddress = PropertyTagPropertyDefinition.InternalCreate("OriginalAuthorSmtpAddress", (PropTag)1560674335U);

		public static readonly PropertyTagPropertyDefinition ReadReceiptSmtpAddress = PropertyTagPropertyDefinition.InternalCreate("ReadReceiptSmtpAddress", (PropTag)1560608799U);

		public static readonly PropertyTagPropertyDefinition CreationTime = PropertyTagPropertyDefinition.InternalCreate("CreationTime", PropTag.CreationTime);

		public static readonly PropertyTagPropertyDefinition DisplayName = PropertyTagPropertyDefinition.InternalCreate("DisplayName", PropTag.DisplayName);

		public static readonly GuidNamePropertyDefinition PhoneticDisplayName = InternalSchema.CreateGuidNameProperty("PhoneticDisplayName", typeof(string), WellKnownPropertySet.PublicStrings, "PhoneticDisplayName");

		public static readonly PropertyTagPropertyDefinition EntryId = PropertyTagPropertyDefinition.InternalCreate("EntryId", PropTag.EntryId);

		public static readonly PropertyTagPropertyDefinition FavoriteDisplayAlias = PropertyTagPropertyDefinition.InternalCreate("FavoriteDisplayAlias", PropTag.FavoriteDisplayAlias);

		public static readonly PropertyTagPropertyDefinition FavoriteDisplayName = PropertyTagPropertyDefinition.InternalCreate("FavoriteDisplayName", PropTag.FavoriteDisplayName);

		public static readonly PropertyTagPropertyDefinition FavLevelMask = PropertyTagPropertyDefinition.InternalCreate("FavLevelMask", PropTag.FavLevelMask);

		public static readonly PropertyTagPropertyDefinition FavPublicSourceKey = PropertyTagPropertyDefinition.InternalCreate("FavPublicSourceKey", PropTag.FavPublicSourceKey);

		public static readonly PropertyTagPropertyDefinition DocumentId = PropertyTagPropertyDefinition.InternalCreate("DocumentId", PropTag.DocumentId);

		public static readonly PropertyTagPropertyDefinition ConversationDocumentId = PropertyTagPropertyDefinition.InternalCreate("ConversationDocumentId", PropTag.ConversationDocumentId);

		public static readonly PropertyTagPropertyDefinition ParentSourceKey = PropertyTagPropertyDefinition.InternalCreate("ParentSourceKey", PropTag.ParentSourceKey);

		public static readonly PropertyTagPropertyDefinition SourceKey = PropertyTagPropertyDefinition.InternalCreate("SourceKey", PropTag.SourceKey);

		public static readonly PropertyTagPropertyDefinition PredecessorChangeList = PropertyTagPropertyDefinition.InternalCreate("PredecessorChangeList", PropTag.PredecessorChangeList);

		public static readonly PropertyTagPropertyDefinition StoreEntryId = PropertyTagPropertyDefinition.InternalCreate("StoreEntryId", PropTag.StoreEntryid);

		public static readonly PropertyTagPropertyDefinition StoreRecordKey = PropertyTagPropertyDefinition.InternalCreate("StoreRecordKey", PropTag.StoreRecordKey);

		public static readonly PropertyTagPropertyDefinition StoreSupportMask = PropertyTagPropertyDefinition.InternalCreate("StoreSupportMask", PropTag.StoreSupportMask);

		public static readonly PropertyTagPropertyDefinition ItemClass = PropertyTagPropertyDefinition.InternalCreate("ItemClass", PropTag.MessageClass, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255)
		});

		public static readonly GuidNamePropertyDefinition OriginalMimeReadTime = InternalSchema.CreateGuidNameProperty("OriginalMimeReadTime", typeof(ExDateTime), WellKnownPropertySet.Attachment, "OriginalMimeReadTime");

		public static readonly PropertyTagPropertyDefinition LastModifiedTime = PropertyTagPropertyDefinition.InternalCreate("LastModifiedTime", PropTag.LastModificationTime);

		public static readonly PropertyTagPropertyDefinition LastModifierName = PropertyTagPropertyDefinition.InternalCreate("LastModifierName", (PropTag)1073348639U);

		public static readonly PropertyTagPropertyDefinition LastModifierEntryId = PropertyTagPropertyDefinition.InternalCreate("LastModifierEntryId", (PropTag)1073414402U);

		public static readonly PropertyTagPropertyDefinition CreatorEntryId = PropertyTagPropertyDefinition.InternalCreate("CreatorEntryId", (PropTag)1073283330U);

		public static readonly PropertyTagPropertyDefinition ParentEntryId = PropertyTagPropertyDefinition.InternalCreate("ParentEntryId", PropTag.ParentEntryId);

		public static readonly PropertyTagPropertyDefinition SearchKey = PropertyTagPropertyDefinition.InternalCreate("SearchKey", PropTag.SearchKey);

		public static readonly PropertyTagPropertyDefinition RecordKey = PropertyTagPropertyDefinition.InternalCreate("RecordKey", PropTag.RecordKey);

		public static readonly PropertyTagPropertyDefinition UserConfigurationDictionary = PropertyTagPropertyDefinition.InternalCreate("UserConfigurationDictionary", (PropTag)2080833794U, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition UserConfigurationStream = PropertyTagPropertyDefinition.InternalCreate("UserConfigurationStream", (PropTag)2080964866U, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition OwnerLogonUserConfigurationCache = PropertyTagPropertyDefinition.InternalCreate("OwnerLogonUserConfigurationCacheEntryId", PropTag.OwnerLogonUserConfigurationCache);

		public static readonly PropertyTagPropertyDefinition UserConfigurationType = PropertyTagPropertyDefinition.InternalCreate("UserConfigurationDataType", (PropTag)2080768003U);

		public static readonly PropertyTagPropertyDefinition UserConfigurationXml = PropertyTagPropertyDefinition.InternalCreate("UserConfigurationXmlStream", (PropTag)2080899330U, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition AdminFolderFlags = PropertyTagPropertyDefinition.InternalCreate("AdminFolderFlags", PropTag.TimeInServer);

		public static readonly PropertyTagPropertyDefinition DeletedCountTotal = PropertyTagPropertyDefinition.InternalCreate("DeletedCount", PropTag.DeletedCountTotal, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition DeletedMsgCount = PropertyTagPropertyDefinition.InternalCreate("DeletedCount", PropTag.DeletedMsgCount, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition DeletedAssocMsgCount = PropertyTagPropertyDefinition.InternalCreate("DeletedCount", PropTag.DeletedAssocMsgCount, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition LocalCommitTimeMax = PropertyTagPropertyDefinition.InternalCreate("LocalCommitTimeMax", PropTag.LocalCommitTimeMax, PropertyFlags.ReadOnly);

		public static readonly GuidIdPropertyDefinition NetMeetingOrganizerAlias = InternalSchema.CreateGuidIdProperty("NetMeetingOrganizerAlias", typeof(string), WellKnownPropertySet.Appointment, 33347);

		public static readonly GuidIdPropertyDefinition NetMeetingDocPathName = InternalSchema.CreateGuidIdProperty("NetMeetingDocPathName", typeof(string), WellKnownPropertySet.Meeting, 33345);

		public static readonly GuidIdPropertyDefinition NetMeetingConferenceServerAllowExternal = InternalSchema.CreateGuidIdProperty("NetMeetingConferenceServerAllowExternal", typeof(bool), WellKnownPropertySet.Meeting, 33350);

		public static readonly GuidIdPropertyDefinition NetMeetingConferenceSerPassword = InternalSchema.CreateGuidIdProperty("NetMeetingConferenceSerPassword", typeof(string), WellKnownPropertySet.Meeting, 33353);

		public static readonly GuidIdPropertyDefinition NetMeetingServer = InternalSchema.CreateGuidIdProperty("NetMeetingServer", typeof(string), WellKnownPropertySet.Appointment, 33346);

		public static readonly PropertyTagPropertyDefinition DisplayNamePrefix = PropertyTagPropertyDefinition.InternalCreate("DisplayNamePrefix", PropTag.DisplayNamePrefix);

		public static readonly PropertyTagPropertyDefinition InstanceKey = PropertyTagPropertyDefinition.InternalCreate("InstanceKey", PropTag.InstanceKey);

		public static readonly PropertyTagPropertyDefinition AttachmentContent = PropertyTagPropertyDefinition.InternalCreate("AttachmentContent", PropTag.SearchAttachments);

		public static readonly GuidNamePropertyDefinition MessageAudioNotes = InternalSchema.CreateGuidNameProperty("UMAudioNotes", typeof(string), WellKnownPropertySet.UnifiedMessaging, "UMAudioNotes");

		public static readonly GuidNamePropertyDefinition MessageAudioNotesIncorrectType = InternalSchema.CreateGuidNameProperty("UMAudioNotesIncorrectType", typeof(string[]), WellKnownPropertySet.UnifiedMessaging, "UMAudioNotes");

		public static readonly PropertyTagPropertyDefinition PersonalHomePage = PropertyTagPropertyDefinition.InternalCreate("PersonalHomePage", PropTag.PersonalHomePage);

		public static readonly GuidIdPropertyDefinition LegacyWebPage = InternalSchema.CreateGuidIdProperty("LegacyWebPage", typeof(string), WellKnownPropertySet.Address, 32811);

		public static readonly GuidIdPropertyDefinition OutlookCardDesign = InternalSchema.CreateGuidIdProperty("OutlookCardDesign", typeof(byte[]), WellKnownPropertySet.Address, 32832);

		public static readonly GuidIdPropertyDefinition UserText1 = InternalSchema.CreateGuidIdProperty("UserText1", typeof(string), WellKnownPropertySet.Address, 32847);

		public static readonly GuidIdPropertyDefinition UserText2 = InternalSchema.CreateGuidIdProperty("UserText2", typeof(string), WellKnownPropertySet.Address, 32848);

		public static readonly GuidIdPropertyDefinition UserText3 = InternalSchema.CreateGuidIdProperty("UserText3", typeof(string), WellKnownPropertySet.Address, 32849);

		public static readonly GuidIdPropertyDefinition UserText4 = InternalSchema.CreateGuidIdProperty("UserText4", typeof(string), WellKnownPropertySet.Address, 32850);

		public static readonly GuidIdPropertyDefinition FreeBusyUrl = InternalSchema.CreateGuidIdProperty("FreeBusyUrl", typeof(string), WellKnownPropertySet.Address, 32984);

		public static readonly PropertyTagPropertyDefinition Hobbies = PropertyTagPropertyDefinition.InternalCreate("Hobbies", PropTag.Hobbies);

		public static readonly GuidIdPropertyDefinition YomiFirstName = InternalSchema.CreateGuidIdProperty("YomiFirstName", typeof(string), WellKnownPropertySet.Address, 32812);

		public static readonly GuidIdPropertyDefinition YomiLastName = InternalSchema.CreateGuidIdProperty("YomiLastName", typeof(string), WellKnownPropertySet.Address, 32813);

		public static readonly GuidIdPropertyDefinition YomiCompany = InternalSchema.CreateGuidIdProperty("YomiCompany", typeof(string), WellKnownPropertySet.Address, 32814);

		public static readonly GuidIdPropertyDefinition HomeAddressInternal = InternalSchema.CreateGuidIdProperty("HomeAddressInternal", typeof(string), WellKnownPropertySet.Address, 32794);

		public static readonly GuidIdPropertyDefinition BusinessAddressInternal = InternalSchema.CreateGuidIdProperty("BusinessAddressInternal", typeof(string), WellKnownPropertySet.Address, 32795);

		public static readonly GuidIdPropertyDefinition OtherAddressInternal = InternalSchema.CreateGuidIdProperty("OtherAddressInternal", typeof(string), WellKnownPropertySet.Address, 32796);

		public static readonly PropertyTagPropertyDefinition TtyTddPhoneNumber = PropertyTagPropertyDefinition.InternalCreate("TtyTddPhoneNumber", PropTag.TtytddPhoneNumber);

		public static readonly PropertyTagPropertyDefinition PrimaryTelephoneNumber = PropertyTagPropertyDefinition.InternalCreate("PrimaryTelephoneNumber", PropTag.PrimaryTelephoneNumber);

		public static readonly PropertyTagPropertyDefinition TelexNumber = PropertyTagPropertyDefinition.InternalCreate("TelexNumber", PropTag.TelexNumber);

		public static readonly PropertyTagPropertyDefinition ItemColor = PropertyTagPropertyDefinition.InternalCreate("ItemColor", (PropTag)278200323U);

		public static readonly PropertyTagPropertyDefinition FlagCompleteTime = PropertyTagPropertyDefinition.InternalCreate("FlagCompleteTime", (PropTag)277938240U);

		public static readonly PropertyTagPropertyDefinition AttachDataBin = PropertyTagPropertyDefinition.InternalCreate("AttachDataBin", PropTag.AttachDataBin, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition AttachDataObj = PropertyTagPropertyDefinition.InternalCreate("AttachDataObj", PropTag.AttachDataObj, PropertyFlags.Streamable);

		public static readonly GuidNamePropertyDefinition AttachHash = InternalSchema.CreateGuidNameProperty("AttachHash", typeof(byte[]), WellKnownPropertySet.Attachment, "AttachHash");

		public static readonly GuidNamePropertyDefinition ImageThumbnail = InternalSchema.CreateGuidNameProperty("ImageThumbnail", typeof(byte[]), WellKnownPropertySet.Attachment, "ImageThumbnail", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition ImageThumbnailSalientRegions = InternalSchema.CreateGuidNameProperty("ImageThumbnailSalientRegions", typeof(byte[]), WellKnownPropertySet.Attachment, "ImageThumbnailSalientRegions");

		public static readonly GuidNamePropertyDefinition ImageThumbnailHeight = InternalSchema.CreateGuidNameProperty("ImageThumbnailHeight", typeof(int), WellKnownPropertySet.Attachment, "ImageThumbnailHeight");

		public static readonly GuidNamePropertyDefinition ImageThumbnailWidth = InternalSchema.CreateGuidNameProperty("ImageThumbnailWidth", typeof(int), WellKnownPropertySet.Attachment, "ImageThumbnailWidth");

		public static readonly PropertyTagPropertyDefinition UserPhotoCacheId = PropertyTagPropertyDefinition.InternalCreate("UserPhotoCacheId", PropTag.UserPhotoCacheId);

		public static readonly PropertyTagPropertyDefinition UserPhotoPreviewCacheId = PropertyTagPropertyDefinition.InternalCreate("UserPhotoPreviewCacheId", PropTag.UserPhotoPreviewCacheId);

		public static readonly PropertyTagPropertyDefinition InferenceTrainedModelVersionBreadCrumb = PropertyTagPropertyDefinition.InternalCreate("InferenceTrainedModelVersionBreadCrumb", PropTag.InferenceTrainedModelVersionBreadCrumb);

		public static readonly GuidNamePropertyDefinition UserPhotoHR648x648 = InternalSchema.CreateGuidNameProperty("UserPhotoHR648x648", typeof(byte[]), WellKnownPropertySet.Common, "UserPhotoHR648x648", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UserPhotoHR504x504 = InternalSchema.CreateGuidNameProperty("UserPhotoHR504x504", typeof(byte[]), WellKnownPropertySet.Common, "UserPhotoHR504x504", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UserPhotoHR432x432 = InternalSchema.CreateGuidNameProperty("UserPhotoHR432x432", typeof(byte[]), WellKnownPropertySet.Common, "UserPhotoHR432x432", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UserPhotoHR360x360 = InternalSchema.CreateGuidNameProperty("UserPhotoHR360x360", typeof(byte[]), WellKnownPropertySet.Common, "UserPhotoHR360x360", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UserPhotoHR240x240 = InternalSchema.CreateGuidNameProperty("UserPhotoHR240x240", typeof(byte[]), WellKnownPropertySet.Common, "UserPhotoHR240x240", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UserPhotoHR120x120 = InternalSchema.CreateGuidNameProperty("UserPhotoHR120x120", typeof(byte[]), WellKnownPropertySet.Common, "UserPhotoHR120x120", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UserPhotoHR96x96 = InternalSchema.CreateGuidNameProperty("UserPhotoHR96x96", typeof(byte[]), WellKnownPropertySet.Common, "UserPhotoHR96x96", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UserPhotoHR64x64 = InternalSchema.CreateGuidNameProperty("UserPhotoHR64x64", typeof(byte[]), WellKnownPropertySet.Common, "UserPhotoHR64x64", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition UserPhotoHR48x48 = InternalSchema.CreateGuidNameProperty("UserPhotoHR48x48", typeof(byte[]), WellKnownPropertySet.Common, "UserPhotoHR48x48", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly PropertyTagPropertyDefinition FolderHierarchyDepth = PropertyTagPropertyDefinition.InternalCreate("FolderHierarchyDepth", PropTag.Depth, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ProhibitReceiveQuota = PropertyTagPropertyDefinition.InternalCreate("ProhibitReceiveQuota", PropTag.ProhibitReceiveQuota);

		public static readonly PropertyTagPropertyDefinition MaxSubmitMessageSize = PropertyTagPropertyDefinition.InternalCreate("MaxSubmitMessageSize", PropTag.MaxSubmitMessageSize);

		public static readonly PropertyTagPropertyDefinition MaxMessageSize = PropertyTagPropertyDefinition.InternalCreate("MaxMessageSize", PropTag.MaxMessageSize);

		public static readonly PropertyTagPropertyDefinition ProhibitSendQuota = PropertyTagPropertyDefinition.InternalCreate("ProhibitSendQuota", PropTag.ProhibitSendQuota);

		public static readonly PropertyTagPropertyDefinition SubmittedByAdmin = PropertyTagPropertyDefinition.InternalCreate("SubmittedByAdmin", PropTag.SubmittedByAdmin);

		public static readonly PropertyTagPropertyDefinition OofReplyType = PropertyTagPropertyDefinition.InternalCreate("OofReplyType", PropTag.OofReplyType);

		public static readonly PropertyTagPropertyDefinition MemberId = PropertyTagPropertyDefinition.InternalCreate("MemberId", PropTag.MemberId);

		public static readonly PropertyTagPropertyDefinition MemberEntryId = InternalSchema.EntryId;

		public static readonly PropertyTagPropertyDefinition MemberName = PropertyTagPropertyDefinition.InternalCreate("MemberName", PropTag.MemberName);

		[Obsolete("Use InternalSchema.MemberName instead.")]
		public static readonly PropertyTagPropertyDefinition MemberNameLocalDirectory = InternalSchema.MemberName;

		public static readonly PropertyTagPropertyDefinition ShortTermEntryIdFromObject = PropertyTagPropertyDefinition.InternalCreate("ShortTermEntryIdFromObject", (PropTag)1718747394U);

		public static readonly PropertyTagPropertyDefinition MemberRights = PropertyTagPropertyDefinition.InternalCreate("MemberRights", PropTag.MemberRights);

		public static readonly PropertyTagPropertyDefinition MemberSecurityIdentifier = PropertyTagPropertyDefinition.InternalCreate("MemberSecurityIdentifier", PropTag.MemberSecurityIdentifier);

		public static readonly PropertyTagPropertyDefinition MemberIsGroup = PropertyTagPropertyDefinition.InternalCreate("MemberIsGroup", PropTag.MemberIsGroup);

		public static readonly PropertyTagPropertyDefinition EffectiveRights = PropertyTagPropertyDefinition.InternalCreate("EffectiveRights", PropTag.Access, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ConversationKey = PropertyTagPropertyDefinition.InternalCreate("ConversationKey", PropTag.ConversationKey);

		public static readonly PropertyTagPropertyDefinition AttachRendering = PropertyTagPropertyDefinition.InternalCreate("AttachRendering", PropTag.AttachRendering);

		public static readonly PropertyTagPropertyDefinition OrigMessageClass = PropertyTagPropertyDefinition.InternalCreate("OrigMessageClass", PropTag.OrigMessageClass);

		public static readonly PropertyTagPropertyDefinition TransmitableDisplayName = PropertyTagPropertyDefinition.InternalCreate("TransmitableDisplayName", PropTag.TransmitableDisplayName);

		public static readonly PropertyTagPropertyDefinition DisplayName7Bit = PropertyTagPropertyDefinition.InternalCreate("DisplayName7Bit", PropTag.DisplayNamePrintable);

		public static readonly PropertyTagPropertyDefinition DisplayType = PropertyTagPropertyDefinition.InternalCreate("DisplayType", PropTag.DisplayType);

		public static readonly PropertyTagPropertyDefinition AutoResponseSuppressInternal = PropertyTagPropertyDefinition.InternalCreate("AutoResponseSuppressInternal", PropTag.AutoResponseSuppress);

		public static readonly PropertyTagPropertyDefinition MessageLocaleId = PropertyTagPropertyDefinition.InternalCreate("MessageLocaleId", PropTag.MessageLocaleId);

		public static readonly PropertyTagPropertyDefinition LocaleId = PropertyTagPropertyDefinition.InternalCreate("LocaleId", PropTag.LocaleId);

		public static readonly PropertyTagPropertyDefinition InternetMdns = PropertyTagPropertyDefinition.InternalCreate("InternetMdns", PropTag.InternetMdns);

		public static readonly PropertyTagPropertyDefinition NonReceiptReason = PropertyTagPropertyDefinition.InternalCreate("NonReceiptReason", PropTag.NonReceiptReason);

		public static readonly PropertyTagPropertyDefinition DiscardReason = PropertyTagPropertyDefinition.InternalCreate("DiscardReason", PropTag.DiscardReason);

		public static readonly PropertyTagPropertyDefinition OriginallyIntendedRecipientName = PropertyTagPropertyDefinition.InternalCreate("OriginallyIntendedRecipientName", PropTag.OriginallyIntendedRecipientName);

		public static readonly PropertyTagPropertyDefinition OriginallyIntendedRecipEntryId = PropertyTagPropertyDefinition.InternalCreate("OriginallyIntendedRecipEntryId", PropTag.OriginallyIntendedRecipEntryId);

		public static readonly PropertyTagPropertyDefinition OriginalSearchKey = PropertyTagPropertyDefinition.InternalCreate("OriginalSearchKey", PropTag.OriginalSearchKey);

		public static readonly PropertyTagPropertyDefinition ContentIdentifier = PropertyTagPropertyDefinition.InternalCreate("ContentIdentifier", PropTag.ContentIdentifier);

		public static readonly PropertyTagPropertyDefinition DeliveryTime = PropertyTagPropertyDefinition.InternalCreate("DeliverTime", PropTag.DeliverTime);

		public static readonly PropertyTagPropertyDefinition ReportText = PropertyTagPropertyDefinition.InternalCreate("ReportText", PropTag.ReportText);

		public static readonly PropertyTagPropertyDefinition ReportTime = PropertyTagPropertyDefinition.InternalCreate("ReportTime", PropTag.ReportTime);

		public static readonly PropertyTagPropertyDefinition NdrDiagnosticCode = PropertyTagPropertyDefinition.InternalCreate("NdrDiagnosticCode", PropTag.NdrDiagCode);

		public static readonly PropertyTagPropertyDefinition NdrReasonCode = PropertyTagPropertyDefinition.InternalCreate("NdrReasonCode", PropTag.NdrReasonCode);

		public static readonly PropertyTagPropertyDefinition NdrStatusCode = PropertyTagPropertyDefinition.InternalCreate("NdrStatusCode", (PropTag)203423747U);

		public static readonly PropertyTagPropertyDefinition OriginalDisplayTo = PropertyTagPropertyDefinition.InternalCreate("OriginalDisplayTo", PropTag.OriginalDisplayTo, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition OriginalDisplayCc = PropertyTagPropertyDefinition.InternalCreate("OriginalDisplayCc", PropTag.OriginalDisplayCc, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition OriginalDisplayBcc = PropertyTagPropertyDefinition.InternalCreate("OriginalDisplayBcc", PropTag.OriginalDisplayBcc, PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition OriginalDeliveryTime = PropertyTagPropertyDefinition.InternalCreate("OriginalDeliveryTime", PropTag.OriginalDeliveryTime);

		public static readonly PropertyTagPropertyDefinition OriginalAuthorAddressType = PropertyTagPropertyDefinition.InternalCreate("OriginalAuthorAddressType", PropTag.OriginalAuthorAddrType, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly PropertyTagPropertyDefinition OriginalAuthorEmailAddress = PropertyTagPropertyDefinition.InternalCreate("OriginalAuthorEmailAddress", PropTag.OriginalAuthorEmailAddress);

		public static readonly PropertyTagPropertyDefinition OriginalAuthorEntryId = PropertyTagPropertyDefinition.InternalCreate("OriginalAuthorEntryId", PropTag.OriginalAuthorEntryId);

		public static readonly PropertyTagPropertyDefinition OriginalAuthorSearchKey = PropertyTagPropertyDefinition.InternalCreate("OriginalAuthorSearchKey", PropTag.OriginalAuthorSearchKey);

		public static readonly PropertyTagPropertyDefinition OriginalSenderAddressType = PropertyTagPropertyDefinition.InternalCreate("OriginalSenderAddressType", PropTag.OriginalSenderAddrType, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly PropertyTagPropertyDefinition OriginalSenderDisplayName = PropertyTagPropertyDefinition.InternalCreate("OriginalSenderDisplayName", PropTag.OriginalSenderName);

		public static readonly PropertyTagPropertyDefinition OriginalSenderEmailAddress = PropertyTagPropertyDefinition.InternalCreate("OriginalSenderEmailAddress", PropTag.OriginalSenderEmailAddress);

		public static readonly PropertyTagPropertyDefinition OriginalSenderEntryId = PropertyTagPropertyDefinition.InternalCreate("OriginalSenderEntryId", PropTag.OriginalSenderEntryId);

		public static readonly PropertyTagPropertyDefinition OriginalSenderSearchKey = PropertyTagPropertyDefinition.InternalCreate("OriginalSenderSearchKey", PropTag.OriginalSenderSearchKey);

		public static readonly PropertyTagPropertyDefinition OriginalSentRepresentingAddressType = PropertyTagPropertyDefinition.InternalCreate("OriginalSentRepresentingAddressType", PropTag.OriginalSentRepresentingAddrType, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly PropertyTagPropertyDefinition OriginalSentRepresentingDisplayName = PropertyTagPropertyDefinition.InternalCreate("OriginalSentRepresentingDisplayName", PropTag.OriginalSentRepresentingName);

		public static readonly PropertyTagPropertyDefinition OriginalSentRepresentingEmailAddress = PropertyTagPropertyDefinition.InternalCreate("OriginalSentRepresentingEmailAddress", PropTag.OriginalSentRepresentingEmailAddress);

		public static readonly PropertyTagPropertyDefinition OriginalSentRepresentingEntryId = PropertyTagPropertyDefinition.InternalCreate("OriginalSentRepresentingEntryId", PropTag.OriginalSentRepresentingEntryId);

		public static readonly PropertyTagPropertyDefinition OriginalSentRepresentingSearchKey = PropertyTagPropertyDefinition.InternalCreate("OriginalSentRepresentingSearchKey", PropTag.OriginalSentRepresentingSearchKey);

		public static readonly PropertyTagPropertyDefinition OriginalSubject = PropertyTagPropertyDefinition.InternalCreate("OriginalSubject", PropTag.OriginalSubject);

		public static readonly PropertyTagPropertyDefinition OriginalSentTime = PropertyTagPropertyDefinition.InternalCreate("OriginalSentTime", PropTag.OriginalSubmitTime);

		public static readonly PropertyTagPropertyDefinition ReceiptTime = PropertyTagPropertyDefinition.InternalCreate("ReceiptTime", PropTag.ReceiptTime);

		public static readonly PropertyTagPropertyDefinition SupplementaryInfo = PropertyTagPropertyDefinition.InternalCreate("SupplementaryInfo", PropTag.SupplementaryInfo);

		public static readonly PropertyTagPropertyDefinition ReceivedByAddrType = PropertyTagPropertyDefinition.InternalCreate("ReceivedByAddrType", PropTag.ReceivedByAddrType, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly PropertyTagPropertyDefinition ReceivedByEmailAddress = PropertyTagPropertyDefinition.InternalCreate("ReceivedByEmailAddress", PropTag.ReceivedByEmailAddress);

		public static readonly PropertyTagPropertyDefinition ReceivedByEntryId = PropertyTagPropertyDefinition.InternalCreate("ReceivedByEntryId", PropTag.ReceivedByEntryId);

		public static readonly PropertyTagPropertyDefinition ReceivedByName = PropertyTagPropertyDefinition.InternalCreate("ReceivedByName", PropTag.ReceivedByName);

		public static readonly PropertyTagPropertyDefinition ReceivedBySearchKey = PropertyTagPropertyDefinition.InternalCreate("ReceivedBySearchKey", PropTag.ReceivedBySearchKey);

		public static readonly PropertyTagPropertyDefinition ElcAutoCopyLabel = PropertyTagPropertyDefinition.InternalCreate("ElcAutoCopyLabel", PropTag.ElcAutoCopyLabel);

		public static readonly PropertyTagPropertyDefinition ElcAutoCopyTag = PropertyTagPropertyDefinition.InternalCreate("ElcAutoCopyTag", PropTag.ElcAutoCopyTag);

		public static readonly PropertyTagPropertyDefinition ElcMoveDate = PropertyTagPropertyDefinition.InternalCreate("ElcMoveDate", PropTag.ElcMoveDate);

		public static readonly PropertyTagPropertyDefinition ControlDataForCalendarRepairAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForCalendarRepairAssistant", PropTag.ControlDataForCalendarRepairAssistant);

		public static readonly PropertyTagPropertyDefinition ControlDataForSharingPolicyAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForSharingPolicyAssistant", PropTag.ControlDataForSharingPolicyAssistant);

		public static readonly PropertyTagPropertyDefinition ControlDataForElcAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForElcAssistant", PropTag.ControlDataForElcAssistant);

		public static readonly PropertyTagPropertyDefinition ElcLastRunTotalProcessingTime = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunTotalProcessingTime", PropTag.ElcLastRunTotalProcessingTime);

		public static readonly PropertyTagPropertyDefinition ElcLastRunSubAssistantProcessingTime = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunSubAssistantProcessingTime", PropTag.ElcLastRunSubAssistantProcessingTime);

		public static readonly PropertyTagPropertyDefinition ElcLastRunUpdatedFolderCount = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunUpdatedFolderCount", PropTag.ElcLastRunUpdatedFolderCount);

		public static readonly PropertyTagPropertyDefinition ElcLastRunTaggedFolderCount = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunTaggedFolderCount", PropTag.ElcLastRunTaggedFolderCount);

		public static readonly PropertyTagPropertyDefinition ElcLastRunUpdatedItemCount = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunUpdatedItemCount", PropTag.ElcLastRunUpdatedItemCount);

		public static readonly PropertyTagPropertyDefinition ElcLastRunTaggedWithArchiveItemCount = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunTaggedWithArchiveItemCount", PropTag.ElcLastRunTaggedWithArchiveItemCount);

		public static readonly PropertyTagPropertyDefinition ElcLastRunTaggedWithExpiryItemCount = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunTaggedWithExpiryItemCount", PropTag.ElcLastRunTaggedWithExpiryItemCount);

		public static readonly PropertyTagPropertyDefinition ElcLastRunDeletedFromRootItemCount = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunDeletedFromRootItemCount", PropTag.ElcLastRunDeletedFromRootItemCount);

		public static readonly PropertyTagPropertyDefinition ElcLastRunDeletedFromDumpsterItemCount = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunDeletedFromDumpsterItemCount", PropTag.ElcLastRunDeletedFromDumpsterItemCount);

		public static readonly PropertyTagPropertyDefinition ElcLastRunArchivedFromRootItemCount = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunArchivedFromRootItemCount", PropTag.ElcLastRunArchivedFromRootItemCount);

		public static readonly PropertyTagPropertyDefinition ElcLastRunArchivedFromDumpsterItemCount = PropertyTagPropertyDefinition.InternalCreate("ElcLastRunArchivedFromDumpsterItemCount", PropTag.ElcLastRunArchivedFromDumpsterItemCount);

		public static readonly PropertyTagPropertyDefinition ELCLastSuccessTimestamp = PropertyTagPropertyDefinition.InternalCreate("ELCLastSuccessTimestamp", PropTag.ELCLastSuccessTimestamp);

		public static readonly PropertyTagPropertyDefinition ControlDataForTopNWordsAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForTopNWordsAssistant", PropTag.ControlDataForTopNWordsAssistant);

		public static readonly PropertyTagPropertyDefinition IsTopNEnabled = PropertyTagPropertyDefinition.InternalCreate("IsTopNEnabled", PropTag.IsTopNEnabled);

		public static readonly PropertyTagPropertyDefinition ControlDataForJunkEmailAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForJunkEmailAssistant", PropTag.ControlDataForJunkEmailAssistant);

		public static readonly PropertyTagPropertyDefinition ControlDataForCalendarSyncAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForCalendarSyncAssistant", PropTag.ControlDataForCalendarSyncAssistant);

		public static readonly PropertyTagPropertyDefinition ExternalSharingCalendarSubscriptionCount = PropertyTagPropertyDefinition.InternalCreate("ExternalSharingCalendarSubscriptionCount", PropTag.ExternalSharingCalendarSubscriptionCount);

		public static readonly PropertyTagPropertyDefinition ControlDataForUMReportingAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForUMReportingAssistant", PropTag.ControlDataForUMReportingAssistant);

		public static readonly PropertyTagPropertyDefinition HasUMReportData = PropertyTagPropertyDefinition.InternalCreate("HasUMReportData", PropTag.HasUMReportData);

		public static readonly PropertyTagPropertyDefinition PredictedActionsSummaryDeprecated = PropertyTagPropertyDefinition.InternalCreate("PredictedActionsSummary", PropTag.PredictedActionsSummary, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition GroupingActionsDeprecated = PropertyTagPropertyDefinition.InternalCreate("GroupingActions", PropTag.GroupingActions, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition ControlDataForInferenceTrainingAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForInferenceTrainingAssistant", PropTag.ControlDataForInferenceTrainingAssistant);

		public static readonly PropertyTagPropertyDefinition InferenceEnabled = PropertyTagPropertyDefinition.InternalCreate("InferenceEnabled", PropTag.InferenceEnabled);

		public static readonly PropertyTagPropertyDefinition InferenceClientActivityFlags = PropertyTagPropertyDefinition.InternalCreate("InferenceClientActivityFlags", PropTag.InferenceClientActivityFlags);

		public static readonly GuidNamePropertyDefinition IsVoiceReminderEnabled = InternalSchema.CreateGuidNameProperty("IsVoiceReminderEnabled", typeof(bool), WellKnownPropertySet.UnifiedMessaging, "IsVoiceReminderEnabled");

		public static readonly GuidNamePropertyDefinition VoiceReminderPhoneNumber = InternalSchema.CreateGuidNameProperty("VoiceReminderPhoneNumber", typeof(string), WellKnownPropertySet.UnifiedMessaging, "VoiceReminderPhoneNumber");

		public static readonly PropertyTagPropertyDefinition ControlDataForDirectoryProcessorAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForDirectoryProcessorAssistant", PropTag.ControlDataForDirectoryProcessorAssistant);

		public static readonly PropertyTagPropertyDefinition NeedsDirectoryProcessor = PropertyTagPropertyDefinition.InternalCreate("NeedsDirectoryProcessor", PropTag.NeedsDirectoryProcessor);

		public static readonly PropertyTagPropertyDefinition ControlDataForOABGeneratorAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForOABGeneratorAssistant", PropTag.ControlDataForOABGeneratorAssistant);

		public static readonly PropertyTagPropertyDefinition InternetCalendarSubscriptionCount = PropertyTagPropertyDefinition.InternalCreate("InternetCalendarSubscriptionCount", PropTag.InternetCalendarSubscriptionCount);

		public static readonly PropertyTagPropertyDefinition ExternalSharingContactSubscriptionCount = PropertyTagPropertyDefinition.InternalCreate("ExternalSharingContactSubscriptionCount", PropTag.ExternalSharingContactSubscriptionCount);

		public static readonly PropertyTagPropertyDefinition JunkEmailSafeListDirty = PropertyTagPropertyDefinition.InternalCreate("JunkEmailSafeListDirty", PropTag.JunkEmailSafeListDirty);

		public static readonly PropertyTagPropertyDefinition LastSharingPolicyAppliedId = PropertyTagPropertyDefinition.InternalCreate("LastSharingPolicyAppliedId", PropTag.LastSharingPolicyAppliedId);

		public static readonly PropertyTagPropertyDefinition LastSharingPolicyAppliedHash = PropertyTagPropertyDefinition.InternalCreate("LastSharingPolicyAppliedHash", PropTag.LastSharingPolicyAppliedHash);

		public static readonly PropertyTagPropertyDefinition LastSharingPolicyAppliedTime = PropertyTagPropertyDefinition.InternalCreate("LastSharingPolicyAppliedTime", PropTag.LastSharingPolicyAppliedTime);

		public static readonly PropertyTagPropertyDefinition OofScheduleStart = PropertyTagPropertyDefinition.InternalCreate("OofScheduleStart", PropTag.OofScheduleStart);

		public static readonly PropertyTagPropertyDefinition OofScheduleEnd = PropertyTagPropertyDefinition.InternalCreate("OofScheduleEnd", PropTag.OofScheduleEnd);

		public static readonly PropertyTagPropertyDefinition RetentionQueryInfo = PropertyTagPropertyDefinition.InternalCreate("RetentionQueryInfo", PropTag.RetentionQueryInfo);

		public static readonly PropertyTagPropertyDefinition ControlDataForPublicFolderAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForPublicFolderAssistant", PropTag.ControlDataForPublicFolderAssistant);

		public static readonly PropertyTagPropertyDefinition IsMarkedMailbox = PropertyTagPropertyDefinition.InternalCreate("IsMarkedMailbox", PropTag.MailboxMarked);

		public static readonly PropertyTagPropertyDefinition MailboxLastProcessedTimestamp = PropertyTagPropertyDefinition.InternalCreate("MailboxLastProcessedTimestamp", PropTag.MailboxLastProcessedTimestamp);

		public static readonly PropertyTagPropertyDefinition MailboxType = PropertyTagPropertyDefinition.InternalCreate("MailboxType", PropTag.MailboxType);

		public static readonly PropertyTagPropertyDefinition MailboxTypeDetail = PropertyTagPropertyDefinition.InternalCreate("MailboxTypeDetail", PropTag.MailboxTypeDetail);

		public static readonly PropertyTagPropertyDefinition ContactLinking = PropertyTagPropertyDefinition.InternalCreate("ContactLinking", PropTag.ContactLinking);

		public static readonly PropertyTagPropertyDefinition ContactSaveVersion = PropertyTagPropertyDefinition.InternalCreate("ContactSaveVersion", PropTag.ContactSaveVersion);

		public static readonly PropertyTagPropertyDefinition PushNotificationSubscriptionType = PropertyTagPropertyDefinition.InternalCreate("PushNotificationSubscriptionType", PropTag.PushNotificationSubscriptionType);

		public static readonly PropertyTagPropertyDefinition NotificationBrokerSubscriptions = PropertyTagPropertyDefinition.InternalCreate("NotificationBrokerSubscriptions", PropTag.NotificationBrokerSubscriptions);

		public static readonly PropertyTagPropertyDefinition ControlDataForInferenceDataCollectionAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForInferenceDataCollectionAssistant", PropTag.ControlDataForInferenceDataCollectionAssistant);

		public static readonly PropertyTagPropertyDefinition InferenceDataCollectionProcessingState = PropertyTagPropertyDefinition.InternalCreate("InferenceDataCollectionProcessingState", PropTag.InferenceDataCollectionProcessingState);

		public static readonly PropertyTagPropertyDefinition SiteMailboxInternalState = PropertyTagPropertyDefinition.InternalCreate("SiteMailboxInternalState", PropTag.SiteMailboxInternalState);

		public static readonly PropertyTagPropertyDefinition ControlDataForPeopleRelevanceAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForPeopleRelevanceAssistant", PropTag.ControlDataForPeopleRelevanceAssistant);

		public static readonly PropertyTagPropertyDefinition ControlDataForSharePointSignalStoreAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForSharePointSignalStoreAssistant", PropTag.ControlDataForSharePointSignalStoreAssistant);

		public static readonly PropertyTagPropertyDefinition ControlDataForPeopleCentricTriageAssistant = PropertyTagPropertyDefinition.InternalCreate("ControlDataForPeopleCentricTriageAssistant", PropTag.ControlDataForPeopleCentricTriageAssistant);

		public static readonly PropertyTagPropertyDefinition InferenceTrainingLastContentCount = PropertyTagPropertyDefinition.InternalCreate("InferenceTrainingLastContentCount", PropTag.InferenceTrainingLastContentCount);

		public static readonly PropertyTagPropertyDefinition InferenceTrainingLastAttemptTimestamp = PropertyTagPropertyDefinition.InternalCreate("InferenceTrainingLastAttemptTimestamp", PropTag.InferenceTrainingLastAttemptTimestamp);

		public static readonly PropertyTagPropertyDefinition InferenceTrainingLastSuccessTimestamp = PropertyTagPropertyDefinition.InternalCreate("InferenceTrainingLastSuccessTimestamp", PropTag.InferenceTrainingLastSuccessTimestamp);

		public static readonly PropertyTagPropertyDefinition InferenceTruthLoggingLastAttemptTimestamp = PropertyTagPropertyDefinition.InternalCreate("InferenceTruthLoggingLastAttemptTimestamp", PropTag.InferenceTruthLoggingLastAttemptTimestamp);

		public static readonly PropertyTagPropertyDefinition InferenceTruthLoggingLastSuccessTimestamp = PropertyTagPropertyDefinition.InternalCreate("InferenceTruthLoggingLastSuccessTimestamp", PropTag.InferenceTruthLoggingLastSuccessTimestamp);

		public static readonly PropertyTagPropertyDefinition InferenceUserCapabilityFlags = PropertyTagPropertyDefinition.InternalCreate("InferenceUserCapabilityFlags", PropTag.InferenceUserCapabilityFlags);

		public static readonly StorePropertyDefinition InferenceUserClassificationReady = new InferenceUserCapabilityFlagsProperty(Microsoft.Exchange.Data.Storage.InferenceUserCapabilityFlags.ClassificationReady);

		public static readonly StorePropertyDefinition InferenceUserUIReady = new InferenceUserCapabilityFlagsProperty(Microsoft.Exchange.Data.Storage.InferenceUserCapabilityFlags.UIReady);

		public static readonly StorePropertyDefinition InferenceClassificationEnabled = new InferenceUserCapabilityFlagsProperty(Microsoft.Exchange.Data.Storage.InferenceUserCapabilityFlags.ClassificationEnabled);

		public static readonly StorePropertyDefinition InferenceClutterEnabled = new InferenceUserCapabilityFlagsProperty(Microsoft.Exchange.Data.Storage.InferenceUserCapabilityFlags.ClutterEnabled);

		public static readonly StorePropertyDefinition InferenceHasBeenClutterInvited = new InferenceUserCapabilityFlagsProperty(Microsoft.Exchange.Data.Storage.InferenceUserCapabilityFlags.HasBeenClutterInvited);

		public static readonly PropertyTagPropertyDefinition PolicyTag = PropertyTagPropertyDefinition.InternalCreate("PolicyTag", PropTag.PolicyTag);

		public static readonly GuidNamePropertyDefinition ExplicitPolicyTag = InternalSchema.CreateGuidNameProperty("ExplicitPolicyTag", typeof(byte[]), WellKnownPropertySet.Elc, "ExplicitPolicyTag");

		public static readonly PropertyTagPropertyDefinition RetentionPeriod = PropertyTagPropertyDefinition.InternalCreate("RetentionPeriod", PropTag.RetentionPeriod);

		public static readonly PropertyTagPropertyDefinition StartDateEtc = PropertyTagPropertyDefinition.InternalCreate("StartDateEtc", PropTag.StartDateEtc);

		public static readonly PropertyTagPropertyDefinition RetentionDate = PropertyTagPropertyDefinition.InternalCreate("RetentionDate", PropTag.RetentionDate);

		public static readonly GuidNamePropertyDefinition EHAMigrationExpirationDate = InternalSchema.CreateGuidNameProperty("EHAMigrationExpirationDate", typeof(ExDateTime), WellKnownPropertySet.Elc, "EHAMigrationExpirationDate");

		public static readonly GuidNamePropertyDefinition EHAMigrationMessageCount = InternalSchema.CreateGuidNameProperty("EHAMigrationMessageCount", typeof(long), WellKnownPropertySet.Elc, "EHAMigrationMessageCount");

		public static readonly PropertyTagPropertyDefinition RetentionFlags = PropertyTagPropertyDefinition.InternalCreate("RetentionFlags", PropTag.RetentionFlags);

		public static readonly PropertyTagPropertyDefinition ArchiveTag = PropertyTagPropertyDefinition.InternalCreate("ArchiveTag", PropTag.ArchiveTag);

		public static readonly GuidNamePropertyDefinition ExplicitArchiveTag = InternalSchema.CreateGuidNameProperty("ExplicitArchiveTag", typeof(byte[]), WellKnownPropertySet.Elc, "ExplicitArchiveTag");

		public static readonly PropertyTagPropertyDefinition ArchiveDate = PropertyTagPropertyDefinition.InternalCreate("ArchiveDate", PropTag.ArchiveDate);

		public static readonly PropertyTagPropertyDefinition ArchivePeriod = PropertyTagPropertyDefinition.InternalCreate("ArchivePeriod", PropTag.ArchivePeriod);

		public static readonly GuidIdPropertyDefinition IsClassified = InternalSchema.CreateGuidIdProperty("IsClassified", typeof(bool), WellKnownPropertySet.Common, 34229);

		public static readonly PropertyTagPropertyDefinition SwappedToDoData = PropertyTagPropertyDefinition.InternalCreate("SwappedToDoData", (PropTag)237830402U);

		public static readonly PropertyTagPropertyDefinition SwappedToDoStore = PropertyTagPropertyDefinition.InternalCreate("SwappedToDoStore", (PropTag)237764866U);

		public static readonly GuidIdPropertyDefinition Classification = InternalSchema.CreateGuidIdProperty("Classification", typeof(string), WellKnownPropertySet.Common, 34230);

		public static readonly GuidIdPropertyDefinition ClassificationDescription = InternalSchema.CreateGuidIdProperty("ClassificationDescription", typeof(string), WellKnownPropertySet.Common, 34231);

		public static readonly GuidIdPropertyDefinition ClassificationGuid = InternalSchema.CreateGuidIdProperty("ClassificationGuid", typeof(string), WellKnownPropertySet.Common, 34232);

		public static readonly GuidIdPropertyDefinition ClassificationKeep = InternalSchema.CreateGuidIdProperty("ClassificationKeep", typeof(bool), WellKnownPropertySet.Common, 34234);

		public static readonly GuidNamePropertyDefinition QuarantineOriginalSender = InternalSchema.CreateGuidNameProperty("QuarantineOriginalSender", typeof(string), WellKnownPropertySet.PublicStrings, "quarantine-original-sender");

		public static readonly GuidNamePropertyDefinition JournalingRemoteAccounts = InternalSchema.CreateGuidNameProperty("JournalingRemoteAccounts", typeof(string[]), WellKnownPropertySet.PublicStrings, "journal-remote-accounts");

		public static readonly GuidIdPropertyDefinition EmailListType = InternalSchema.CreateGuidIdProperty("EmailListType", typeof(int), WellKnownPropertySet.Address, 32809);

		public static readonly GuidIdPropertyDefinition EmailList = InternalSchema.CreateGuidIdProperty("EmailList", typeof(int[]), WellKnownPropertySet.Address, 32808);

		public static readonly PropertyTagPropertyDefinition PurportedSenderDomain = PropertyTagPropertyDefinition.InternalCreate("PurportedSenderDomain", PropTag.PurportedSenderDomain);

		public static readonly PropertyTagPropertyDefinition ParentKey = PropertyTagPropertyDefinition.InternalCreate("ParentKey", PropTag.ParentKey);

		public static readonly PropertyTagPropertyDefinition OriginalMessageId = PropertyTagPropertyDefinition.InternalCreate("OriginalMessageId", (PropTag)273023007U);

		public static readonly GuidIdPropertyDefinition AppointmentCounterStartWhole = InternalSchema.CreateGuidIdProperty("AppointmentCounterStartWhole", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33360);

		public static readonly GuidIdPropertyDefinition AppointmentCounterEndWhole = InternalSchema.CreateGuidIdProperty("AppointmentCounterEndWhole", typeof(ExDateTime), WellKnownPropertySet.Appointment, 33361);

		public static readonly GuidIdPropertyDefinition AppointmentProposedDuration = InternalSchema.CreateGuidIdProperty("AppointmentProposedDuration", typeof(int), WellKnownPropertySet.Appointment, 33366);

		public static readonly GuidIdPropertyDefinition AppointmentCounterProposal = InternalSchema.CreateGuidIdProperty("AppointmentCounterProposal", typeof(bool), WellKnownPropertySet.Appointment, 33367);

		public static readonly GuidIdPropertyDefinition UnsendableRecipients = InternalSchema.CreateGuidIdProperty("UnsendableRecipients", typeof(byte[]), WellKnownPropertySet.Appointment, 33373, PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition ForwardNotificationRecipients = InternalSchema.CreateGuidIdProperty("ForwardNotificationRecipients", typeof(byte[]), WellKnownPropertySet.Appointment, 33377, PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition AppointmentCounterProposalCount = InternalSchema.CreateGuidIdProperty("AppointmentCounterProposalCount", typeof(int), WellKnownPropertySet.Appointment, 33369);

		public static readonly GuidNamePropertyDefinition PropertyChangeMetadataRaw = InternalSchema.CreateGuidNameProperty("PropertyChangeMetadataRaw", typeof(byte[]), WellKnownPropertySet.Appointment, "PropertyChangeMetadataRaw");

		public static readonly PropertyTagPropertyDefinition RecipientProposed = PropertyTagPropertyDefinition.InternalCreate("RecipientProposed", (PropTag)1608581131U);

		public static readonly PropertyTagPropertyDefinition RecipientProposedStartTime = PropertyTagPropertyDefinition.InternalCreate("RecipientProposedStartTime", (PropTag)1608712256U);

		public static readonly PropertyTagPropertyDefinition RecipientProposedEndTime = PropertyTagPropertyDefinition.InternalCreate("RecipientProposedEndTime", (PropTag)1608777792U);

		public static readonly PropertyTagPropertyDefinition RecipientOrder = PropertyTagPropertyDefinition.InternalCreate("RecipientOrder", (PropTag)1608450051U);

		public static readonly GuidIdPropertyDefinition OutlookVersion = InternalSchema.CreateGuidIdProperty("OutlookVersion", typeof(string), WellKnownPropertySet.Common, 34132);

		public static readonly GuidIdPropertyDefinition OutlookInternalVersion = InternalSchema.CreateGuidIdProperty("OutlookInternalVersion", typeof(int), WellKnownPropertySet.Common, 34130);

		public static readonly PropertyTagPropertyDefinition NativeBlockStatus = PropertyTagPropertyDefinition.InternalCreate("NativeBlockStatus", (PropTag)278265859U);

		public static readonly PropertyTagPropertyDefinition SelectedPreferredPhoneNumber = InternalSchema.UserConfigurationType;

		public static readonly PropertyTagPropertyDefinition ReadReceiptDisplayName = PropertyTagPropertyDefinition.InternalCreate("ReadReceiptDisplayName", (PropTag)1076559903U);

		public static readonly PropertyTagPropertyDefinition ReadReceiptEmailAddress = PropertyTagPropertyDefinition.InternalCreate("ReadReceiptEmailAddress", (PropTag)1076494367U);

		public static readonly PropertyTagPropertyDefinition ReadReceiptAddrType = PropertyTagPropertyDefinition.InternalCreate("ReadReceiptAddrType", (PropTag)1076428831U, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new StringLengthConstraint(0, 9))
		});

		public static readonly PropertyTagPropertyDefinition ReportEntryId = PropertyTagPropertyDefinition.InternalCreate("ReportEntryId", PropTag.ReportEntryId);

		public static readonly PropertyTagPropertyDefinition ReadReceiptEntryId = PropertyTagPropertyDefinition.InternalCreate("ReadReceiptEntryId", PropTag.ReadReceiptEntryId);

		public static readonly PropertyTagPropertyDefinition ReplyTemplateId = PropertyTagPropertyDefinition.InternalCreate("REPLY_TEMPLATE_ID", PropTag.ReplyTemplateID);

		[MessageClassSpecific("IPM.Microsoft.WunderBar.SFInfo")]
		public static readonly PropertyTagPropertyDefinition AssociatedSearchFolderId = PropertyTagPropertyDefinition.InternalCreate("AssociatedSearchFolderId", (PropTag)1749156098U);

		public static readonly PropertyTagPropertyDefinition AssociatedSearchFolderFlags = PropertyTagPropertyDefinition.InternalCreate("AssociatedSearchFolderFlags", (PropTag)1749549059U);

		public static readonly PropertyTagPropertyDefinition AssociatedSearchFolderExpiration = PropertyTagPropertyDefinition.InternalCreate("AssociatedSearchFolderExpiration", (PropTag)1748631555U);

		public static readonly PropertyTagPropertyDefinition AssociatedSearchFolderLastUsedTime = PropertyTagPropertyDefinition.InternalCreate("AssociatedSearchFolderLastUsedTime", PropTag.InferenceClientActivityFlags);

		public static readonly PropertyTagPropertyDefinition AssociatedSearchFolderTemplateId = PropertyTagPropertyDefinition.InternalCreate("AssociatedSearchFolderTemplateId", (PropTag)1749090307U);

		[MessageClassSpecific("IPM.Microsoft.WunderBar.SFInfo")]
		public static readonly PropertyTagPropertyDefinition AssociatedSearchFolderTag = PropertyTagPropertyDefinition.InternalCreate("AssociatedSearchFolderTag", (PropTag)1749483523U);

		public static readonly PropertyTagPropertyDefinition AssociatedSearchFolderStorageType = PropertyTagPropertyDefinition.InternalCreate("AssociatedSearchFolderStorageType", (PropTag)1749417987U);

		public static readonly PropertyTagPropertyDefinition AssociatedSearchFolderDefinition = PropertyTagPropertyDefinition.InternalCreate("AssociatedSearchFolderDefinition", (PropTag)1749352706U, PropertyFlags.Streamable);

		[MessageClassSpecific("IPM.Microsoft.WunderBar.Link")]
		public static readonly PropertyTagPropertyDefinition NavigationNodeGroupClassId = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeGroupClassId", (PropTag)1749156098U);

		[MessageClassSpecific("IPM.Microsoft.WunderBar.Link")]
		public static readonly PropertyTagPropertyDefinition NavigationNodeOutlookTagId = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeOutlookTagId", (PropTag)1749483523U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeType = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeType", (PropTag)1749614595U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeFlags = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeFlags", (PropTag)1749680131U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeOrdinal = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeOrdinal", (PropTag)1749745922U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeEntryId = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeEntryId", (PropTag)1749811458U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeRecordKey = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeRecordKey", (PropTag)1749876994U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeStoreEntryId = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeStoreEntryId", (PropTag)1749942530U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeClassId = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeClassId", (PropTag)1750008066U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeParentGroupClassId = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeParentGroupClassId", (PropTag)1750073602U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeGroupName = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeGroupName", (PropTag)1750138911U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeGroupSection = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeGroupSection", (PropTag)1750204419U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeCalendarColor = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeCalendarColor", (PropTag)1750269955U);

		public static readonly PropertyTagPropertyDefinition NavigationNodeAddressBookEntryId = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeAddressBookEntryId", (PropTag)1750335746U);

		public static readonly GuidNamePropertyDefinition NavigationNodeCalendarTypeFromOlderExchange = InternalSchema.CreateGuidNameProperty("OWA-NavigationNodeCalendarTypeFromOlderExchange", typeof(int), WellKnownPropertySet.Sharing, "OWA-NavigationNodeCalendarTypeFromOlderExchange");

		public static readonly PropertyTagPropertyDefinition NavigationNodeAddressBookStoreEntryId = PropertyTagPropertyDefinition.InternalCreate("NavigationNodeAddressBookStoreEntryId", (PropTag)1754333442U);

		public static readonly PropertyTagPropertyDefinition SearchAllIndexedProps = PropertyTagPropertyDefinition.InternalCreate("SearchAllIndexedProps", PropTag.SearchAllIndexedProps, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition SearchIsPartiallyIndexed = PropertyTagPropertyDefinition.InternalCreate("SearchIsPartiallyIndexed", (PropTag)248381451U, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition SearchSender = PropertyTagPropertyDefinition.InternalCreate("SearchSender", PropTag.SearchSender, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition SearchRecipients = PropertyTagPropertyDefinition.InternalCreate("SearchRecipients", PropTag.SearchRecipients, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition SearchRecipientsTo = PropertyTagPropertyDefinition.InternalCreate("SearchRecipientsTo", PropTag.SearchRecipientsTo, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition SearchRecipientsCc = PropertyTagPropertyDefinition.InternalCreate("SearchRecipientsCc", PropTag.SearchRecipientsCc, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition SearchRecipientsBcc = PropertyTagPropertyDefinition.InternalCreate("SearchRecipientsBcc", PropTag.SearchRecipientsBcc, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition SearchFullText = PropertyTagPropertyDefinition.InternalCreate("SearchFullText", PropTag.SearchFullText, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition SearchFullTextSubject = PropertyTagPropertyDefinition.InternalCreate("SearchFullTextSubject", PropTag.SearchFullTextSubject, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition SearchFullTextBody = PropertyTagPropertyDefinition.InternalCreate("SearchFullTextBody", PropTag.SearchFullTextBody, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition QuotaStorageWarning = PropertyTagPropertyDefinition.InternalCreate("ptagStorageQuota", PropTag.PfStorageQuota);

		public static PropertyTagPropertyDefinition CustomerId = PropertyTagPropertyDefinition.InternalCreate("CustomerId", PropTag.CustomerId);

		public static PropertyTagPropertyDefinition GovernmentIdNumber = PropertyTagPropertyDefinition.InternalCreate("GovernmentIdNumber", PropTag.GovernmentIdNumber);

		public static PropertyTagPropertyDefinition Account = PropertyTagPropertyDefinition.InternalCreate("Account", PropTag.Account);

		public static readonly PropertyTagPropertyDefinition DelegateNames = PropertyTagPropertyDefinition.InternalCreate("DelegateNames", (PropTag)1749684255U);

		public static readonly PropertyTagPropertyDefinition DelegateEntryIds = PropertyTagPropertyDefinition.InternalCreate("DelegateEntryIds", (PropTag)1749356802U);

		public static readonly PropertyTagPropertyDefinition DelegateFlags = PropertyTagPropertyDefinition.InternalCreate("DelegateFlags", (PropTag)1751846915U);

		public static readonly PropertyTagPropertyDefinition DelegateEntryIds2 = PropertyTagPropertyDefinition.InternalCreate("DelegateEntryIds2", (PropTag)1752174850U);

		public static readonly PropertyTagPropertyDefinition DelegateFlags2 = PropertyTagPropertyDefinition.InternalCreate("DelegateFlags2", (PropTag)1752240131U);

		public static readonly PropertyTagPropertyDefinition DelegateBossWantsCopy = PropertyTagPropertyDefinition.InternalCreate("DelegateBossWantsCopy", (PropTag)1749155851U);

		public static readonly PropertyTagPropertyDefinition DelegateBossWantsInfo = PropertyTagPropertyDefinition.InternalCreate("DelegateBossWantsInfo", (PropTag)1749745675U);

		public static readonly PropertyTagPropertyDefinition DelegateDontMail = PropertyTagPropertyDefinition.InternalCreate("DelegateDontMail", (PropTag)1749221387U);

		public static readonly PropertyTagPropertyDefinition FreeBusyEntryIds = PropertyTagPropertyDefinition.InternalCreate("FreeBusyEntryIds", (PropTag)920916226U);

		public static readonly PropertyTagPropertyDefinition ScheduleInfoMonthsTentative = PropertyTagPropertyDefinition.InternalCreate("ScheduleInfoMonthsTentative", (PropTag)1750142979U);

		public static readonly PropertyTagPropertyDefinition ScheduleInfoFreeBusyTentative = PropertyTagPropertyDefinition.InternalCreate("ScheduleInfoFreeBusyTentative", (PropTag)1750208770U);

		public static readonly PropertyTagPropertyDefinition ScheduleInfoMonthsBusy = PropertyTagPropertyDefinition.InternalCreate("ScheduleInfoMonthsBusy", (PropTag)1750274051U);

		public static readonly PropertyTagPropertyDefinition ScheduleInfoFreeBusyBusy = PropertyTagPropertyDefinition.InternalCreate("ScheduleInfoFreeBusyBusy", (PropTag)1750339842U);

		public static readonly PropertyTagPropertyDefinition ScheduleInfoMonthsOof = PropertyTagPropertyDefinition.InternalCreate("ScheduleInfoMonthsOof", (PropTag)1750405123U);

		public static readonly PropertyTagPropertyDefinition ScheduleInfoFreeBusyOof = PropertyTagPropertyDefinition.InternalCreate("ScheduleInfoFreeBusyOof", (PropTag)1750470914U);

		public static readonly PropertyTagPropertyDefinition ScheduleInfoMonthsMerged = PropertyTagPropertyDefinition.InternalCreate("ScheduleInfoMonthsMerged", (PropTag)1750011907U);

		public static readonly PropertyTagPropertyDefinition ScheduleInfoFreeBusyMerged = PropertyTagPropertyDefinition.InternalCreate("ScheduleInfoFreeBusyMerged", (PropTag)1750077698U);

		public static readonly PropertyTagPropertyDefinition ScheduleInfoRecipientLegacyDn = PropertyTagPropertyDefinition.InternalCreate("ScheduleInfoRecipientLegacyDn", (PropTag)1749614622U);

		public static readonly PropertyTagPropertyDefinition OutlookFreeBusyMonthCount = PropertyTagPropertyDefinition.InternalCreate("OutlookFreeBusyMonthCount", (PropTag)1751711747U);

		public static readonly GuidNamePropertyDefinition DRMLicense = InternalSchema.CreateGuidNameProperty("DRMLicense", typeof(byte[][]), WellKnownPropertySet.PublicStrings, "DRMLicense");

		public static readonly GuidNamePropertyDefinition DRMServerLicense = InternalSchema.CreateGuidNameProperty("DRMServerLicense", typeof(string), WellKnownPropertySet.PublicStrings, "DRMServerLicense");

		public static readonly GuidNamePropertyDefinition DRMServerLicenseCompressed = InternalSchema.CreateGuidNameProperty("DRMServerLicenseCompressed", typeof(byte[]), WellKnownPropertySet.PublicStrings, "DRMServerLicenseCompressed", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition DRMRights = InternalSchema.CreateGuidNameProperty("DRMRights", typeof(int), WellKnownPropertySet.PublicStrings, "DRMRights");

		public static readonly GuidNamePropertyDefinition DRMExpiryTime = InternalSchema.CreateGuidNameProperty("DRMExpiryTime", typeof(ExDateTime), WellKnownPropertySet.PublicStrings, "DRMExpiryTime");

		public static readonly GuidNamePropertyDefinition DRMPropsSignature = InternalSchema.CreateGuidNameProperty("DRMPropsSignature", typeof(byte[]), WellKnownPropertySet.PublicStrings, "DRMPropsSignature");

		public static readonly GuidNamePropertyDefinition DrmPublishLicense = InternalSchema.CreateGuidNameProperty("DrmPublishLicense", typeof(string), WellKnownPropertySet.PublicStrings, "DrmPublishLicense");

		public static readonly GuidNamePropertyDefinition DRMPrelicenseFailure = InternalSchema.CreateGuidNameProperty("DRMPrelicenseFailure", typeof(int), WellKnownPropertySet.PublicStrings, "DRMPrelicenseFailure");

		public static readonly GuidNamePropertyDefinition AcceptLanguage = InternalSchema.CreateGuidNameProperty("AcceptLanguage", typeof(string), WellKnownPropertySet.InternetHeaders, "AcceptLanguage");

		public static readonly GuidNamePropertyDefinition OutlookSpoofingStamp = InternalSchema.CreateGuidNameProperty("SpoofingStamp", typeof(int), WellKnownPropertySet.PublicStrings, "http://schemas.microsoft.com/outlook/spoofingstamp");

		public static readonly GuidNamePropertyDefinition OutlookPhishingStamp = InternalSchema.CreateGuidNameProperty("PhishingStamp", typeof(int), WellKnownPropertySet.PublicStrings, "http://schemas.microsoft.com/outlook/phishingstamp");

		public static readonly GuidNamePropertyDefinition OwaViewStateSortColumn = InternalSchema.CreateGuidNameProperty("OwaViewStateSortColumn", typeof(string), WellKnownPropertySet.PublicStrings, "http://schemas.microsoft.com/exchange/wcsortcolumn");

		public static readonly GuidNamePropertyDefinition OwaViewStateSortOrder = InternalSchema.CreateGuidNameProperty("OwaViewStateSortOrder", typeof(int), WellKnownPropertySet.PublicStrings, "http://schemas.microsoft.com/exchange/wcsortorder");

		public static readonly PropertyTagPropertyDefinition ContentFilterPcl = PropertyTagPropertyDefinition.InternalCreate("ContenFilterPcl", (PropTag)1082392579U);

		public static readonly GuidIdPropertyDefinition ProviderGuidBinary = InternalSchema.CreateGuidIdProperty("OutlookProviderGuidBinary", typeof(byte[]), WellKnownPropertySet.Sharing, 35329);

		public static readonly PropertyTagPropertyDefinition RecipientEntryId = PropertyTagPropertyDefinition.InternalCreate("RecipientEntryId", (PropTag)1610023170U);

		public static readonly PropertyTagPropertyDefinition DisplayTypeExInternal = PropertyTagPropertyDefinition.InternalCreate("DisplayTypeExInternal", PropTag.DisplayTypeEx);

		public static readonly PropertyTagPropertyDefinition RemoteMta = PropertyTagPropertyDefinition.InternalCreate("ReportingMta", (PropTag)203489311U);

		public static readonly PropertyTagPropertyDefinition Responsibility = PropertyTagPropertyDefinition.InternalCreate("PR_RESPONSIBILITY", PropTag.Responsibility);

		public static readonly PropertyTagPropertyDefinition DavSubmitData = PropertyTagPropertyDefinition.InternalCreate("ptagDAVSubmitData", PropTag.DavSubmitData);

		public static readonly PropertyTagPropertyDefinition SendRichInfo = PropertyTagPropertyDefinition.InternalCreate("SendRichInfo", PropTag.SendRichInfo);

		public static readonly PropertyTagPropertyDefinition SendInternetEncoding = PropertyTagPropertyDefinition.InternalCreate("SendInternetEncoding", PropTag.SendInternetEncoding);

		public static readonly PropertyTagPropertyDefinition SpamConfidenceLevel = PropertyTagPropertyDefinition.InternalCreate("SpamConfidenceLevel", PropTag.SpamConfidenceLevel);

		public static readonly PropertyTagPropertyDefinition StorageQuotaLimit = PropertyTagPropertyDefinition.InternalCreate("StorageQuotaLimit", PropTag.StorageQuotaLimit);

		public static readonly PropertyTagPropertyDefinition PersistableTenantPartitionHint = PropertyTagPropertyDefinition.InternalCreate("PersistableTenantPartitionHint", PropTag.PersistableTenantPartitionHint);

		public static readonly PropertyTagPropertyDefinition ExcessStorageUsed = PropertyTagPropertyDefinition.InternalCreate("ExcessStorageUsed", PropTag.ExcessStorageUsed);

		public static readonly PropertyTagPropertyDefinition SvrGeneratingQuotaMsg = PropertyTagPropertyDefinition.InternalCreate("SvrGeneratingQuotaMsg", PropTag.SvrGeneratingQuotaMsg);

		public static readonly PropertyTagPropertyDefinition PrimaryMbxOverQuota = PropertyTagPropertyDefinition.InternalCreate("PrimaryMbxOverQuota", PropTag.PrimaryMbxOverQuota);

		public static readonly PropertyTagPropertyDefinition QuotaType = PropertyTagPropertyDefinition.InternalCreate("QuotaType", PropTag.QuotaType);

		public static readonly PropertyTagPropertyDefinition IsPublicFolderQuotaMessage = PropertyTagPropertyDefinition.InternalCreate("IsPublicFolderQuotaMessage", PropTag.IsPublicFolderQuotaMessage);

		public static readonly GuidNamePropertyDefinition Contact = InternalSchema.CreateGuidNameProperty("Contact", typeof(string), WellKnownPropertySet.PublicStrings, "urn:schemas:calendar:contact");

		public static readonly GuidNamePropertyDefinition ContactURL = InternalSchema.CreateGuidNameProperty("ContactURL", typeof(string), WellKnownPropertySet.PublicStrings, "urn:schemas:calendar:contacturl");

		public static readonly GuidNamePropertyDefinition MobilePhone2 = InternalSchema.CreateGuidNameProperty("MobilePhone2", typeof(string), WellKnownPropertySet.Address, "ContactMobilePhone2");

		public static readonly GuidNamePropertyDefinition OtherPhone2 = InternalSchema.CreateGuidNameProperty("OtherPhone2", typeof(string), WellKnownPropertySet.Address, "ContactOtherPhone2");

		public static readonly GuidNamePropertyDefinition HomePhoneAttributes = InternalSchema.CreateGuidNameProperty("HomePhoneAttributes", typeof(string), WellKnownPropertySet.Address, "ContactHomePhoneAttributes");

		public static readonly GuidNamePropertyDefinition WorkPhoneAttributes = InternalSchema.CreateGuidNameProperty("WorkPhoneAttributes", typeof(string), WellKnownPropertySet.Address, "ContactWorkPhoneAttributes");

		public static readonly GuidNamePropertyDefinition MobilePhoneAttributes = InternalSchema.CreateGuidNameProperty("MobilePhoneAttributes", typeof(string), WellKnownPropertySet.Address, "ContactMobilePhoneAttributes");

		public static readonly GuidNamePropertyDefinition OtherPhoneAttributes = InternalSchema.CreateGuidNameProperty("OtherPhoneAttributes", typeof(string), WellKnownPropertySet.Address, "ContactOtherPhoneAttributes");

		public static readonly GuidNamePropertyDefinition LocationURL = InternalSchema.CreateGuidNameProperty("LocationURL", typeof(string), WellKnownPropertySet.PublicStrings, "urn:schemas:calendar:locationurl");

		public static readonly GuidNamePropertyDefinition Keywords = InternalSchema.Categories;

		public static readonly PropertyTagPropertyDefinition UserX509Certificates = PropertyTagPropertyDefinition.InternalCreate("UserX509Certificates", PropTag.UserSMimeCertificate);

		public static readonly PropertyTagPropertyDefinition DeletedOnTime = PropertyTagPropertyDefinition.InternalCreate("DeletedOnTime", (PropTag)1720647744U);

		public static readonly PropertyTagPropertyDefinition IsSoftDeleted = PropertyTagPropertyDefinition.InternalCreate("IsSoftDeleted", (PropTag)1735393291U);

		public static readonly GuidIdPropertyDefinition ExceptionalBody = InternalSchema.CreateGuidIdProperty("ExceptionalBody", typeof(bool), WellKnownPropertySet.Appointment, 33286);

		public static readonly GuidIdPropertyDefinition ExceptionalAttendees = InternalSchema.CreateGuidIdProperty("ExceptionalAttendees", typeof(bool), WellKnownPropertySet.Appointment, 33323);

		public static readonly GuidIdPropertyDefinition AppointmentReplyName = InternalSchema.CreateGuidIdProperty("AppointmentReplyName", typeof(string), WellKnownPropertySet.Appointment, 33328);

		public static readonly GuidIdPropertyDefinition InboundICalStream = InternalSchema.CreateGuidIdProperty("InboundICalStream", typeof(byte[]), WellKnownPropertySet.Appointment, 33402, PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition IsSingleBodyICal = InternalSchema.CreateGuidIdProperty("IsSingleBodyICal", typeof(bool), WellKnownPropertySet.Appointment, 33403);

		public static readonly GuidNamePropertyDefinition ElcFolderLocalizedName = InternalSchema.CreateGuidNameProperty("ElcFolderLocalizedName", typeof(string), WellKnownPropertySet.Elc, "ElcFolderLocalizedName");

		public static readonly GuidIdPropertyDefinition OutlookUserPropsFormStorage = InternalSchema.CreateGuidIdProperty("dispidFormStorage", typeof(byte[]), WellKnownPropertySet.Common, 34063);

		public static readonly GuidIdPropertyDefinition OutlookUserPropsScriptStream = InternalSchema.CreateGuidIdProperty("dispidScriptStream", typeof(byte[]), WellKnownPropertySet.Common, 34113);

		public static readonly GuidIdPropertyDefinition OutlookUserPropsFormPropStream = InternalSchema.CreateGuidIdProperty("dispidFormPropStream", typeof(byte[]), WellKnownPropertySet.Common, 34075);

		public static readonly GuidIdPropertyDefinition OutlookUserPropsPageDirStream = InternalSchema.CreateGuidIdProperty("dispidPageDirStream", typeof(byte[]), WellKnownPropertySet.Common, 34067);

		public static readonly GuidIdPropertyDefinition OutlookUserPropsPropDefStream = InternalSchema.CreateGuidIdProperty("dispidPropDefStream", typeof(byte[]), WellKnownPropertySet.Common, 34112);

		public static readonly GuidIdPropertyDefinition OutlookUserPropsCustomFlag = InternalSchema.CreateGuidIdProperty("dispidCustomFlag", typeof(int), WellKnownPropertySet.Common, 34114);

		public static readonly PropertyTagPropertyDefinition ConversationIndexTracking = PropertyTagPropertyDefinition.InternalCreate("ConversationIndexTracking", PropTag.ConversationIndexTracking);

		public static readonly GuidNamePropertyDefinition ConversationIndexTrackingEx = InternalSchema.CreateGuidNameProperty("ConversationIndexTrackingEx", typeof(string), WellKnownPropertySet.Conversations, "ConversationIndexTrackingEx");

		public static readonly PropertyTagPropertyDefinition ConversationIdHash = PropertyTagPropertyDefinition.InternalCreate("ConversationIdHash", PropTag.ConversationIdHash);

		public static readonly PropertyTagPropertyDefinition MapiConversationId = PropertyTagPropertyDefinition.InternalCreate("Conversation Id", PropTag.ConversationId);

		public static readonly PropertyTagPropertyDefinition InternalConversationMVFrom = PropertyTagPropertyDefinition.InternalCreate("Conversation MVFrom", PropTag.ConversationMvFrom);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalMVFrom = PropertyTagPropertyDefinition.InternalCreate("Conversation Global MVFrom", PropTag.ConversationMvFromMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationMVTo = PropertyTagPropertyDefinition.InternalCreate("Conversation MVTo", PropTag.ConversationMvTo);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalMVTo = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide MVTo", PropTag.ConversationMvToMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationLastDeliveryTime = PropertyTagPropertyDefinition.InternalCreate("Conversation LastDeliveryTime", PropTag.ConversationMsgDeliveryTime);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalLastDeliveryTime = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide LastDeliveryTime", PropTag.ConversationMsgDeliveryTimeMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationLastDeliveryOrRenewTime = PropertyTagPropertyDefinition.InternalCreate("Conversation LastDeliveryOrRenewTime", PropTag.ConversationMessageDeliveryOrRenewTime);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalLastDeliveryOrRenewTime = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide LastDeliveryOrRenewTime", PropTag.ConversationMessageDeliveryOrRenewTimeMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationMailboxGuid = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxGuid", PropTag.MailboxDSGuidGuid);

		public static readonly PropertyTagPropertyDefinition InternalConversationCategories = PropertyTagPropertyDefinition.InternalCreate("Conversation Categories", PropTag.ConversationCategories);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalCategories = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide Categories", PropTag.ConversationCategoriesMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationFlagStatus = PropertyTagPropertyDefinition.InternalCreate("Conversation FlagStatus", PropTag.ConversationFlagStatus);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalFlagStatus = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide FlagStatus", PropTag.ConversationFlagStatusMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationFlagCompleteTime = PropertyTagPropertyDefinition.InternalCreate("Conversation FlagCompleteTime", PropTag.ConversationFlagCompleteTime);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalFlagCompleteTime = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide FlagCompleteTime", PropTag.ConversationFlagCompleteTimeMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationHasAttach = PropertyTagPropertyDefinition.InternalCreate("Conversation HasAttach", PropTag.ConversationHasAttach);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalHasAttach = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide HasAttach", PropTag.ConversationHasAttachMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationHasIrm = PropertyTagPropertyDefinition.InternalCreate("Conversation HasIrm", PropTag.ConversationHasIrm);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalHasIrm = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide HasIrm", PropTag.ConversationHasIrmMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationMessageCount = PropertyTagPropertyDefinition.InternalCreate("Conversation Message Count", PropTag.ConversationContentCount);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalMessageCount = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide Message Count", PropTag.ConversationContentCountMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationUnreadMessageCount = PropertyTagPropertyDefinition.InternalCreate("Conversation Unread Message Count", PropTag.ConversationContentUnread);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalUnreadMessageCount = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide Unread Message Count", PropTag.ConversationContentUnreadMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationMessageSize = PropertyTagPropertyDefinition.InternalCreate("Conversation MessageSize", PropTag.ConversationMessageSize);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalMessageSize = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide MessageSize", PropTag.ConversationMessageSizeMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationMessageClasses = PropertyTagPropertyDefinition.InternalCreate("Conversation MessageClasses", PropTag.ConversationMessageClasses);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalMessageClasses = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide MessageClasses", PropTag.ConversationMessageClassesMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationReplyForwardState = PropertyTagPropertyDefinition.InternalCreate("Conversation ReplyForwardState", PropTag.ConversationReplyForwardState);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalReplyForwardState = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide ReplyForwardState", PropTag.ConversationReplyForwardStateMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationImportance = PropertyTagPropertyDefinition.InternalCreate("Conversation Importance", PropTag.ConversationImportance);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalImportance = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide Importance", PropTag.ConversationImportanceMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalFamilyId = PropertyTagPropertyDefinition.InternalCreate("Conversation level Family id", PropTag.FamilyId);

		public static readonly PropertyTagPropertyDefinition InternalConversationMVUnreadFrom = PropertyTagPropertyDefinition.InternalCreate("Conversation Unread MVFrom", PropTag.ConversationMvFromUnread);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalMVUnreadFrom = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide Unread MVFrom", PropTag.ConversationMvFromUnreadMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationLastMemberDocumentId = PropertyTagPropertyDefinition.InternalCreate("Conversation Last Member DocumentId", PropTag.ConversationLastMemberDocumentId);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalLastMemberDocumentId = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide Last Member DocumentId", PropTag.ConversationLastMemberDocumentIdMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationPreview = PropertyTagPropertyDefinition.InternalCreate("Conversation Preview", PropTag.ConversationPreview);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalPreview = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide Preview", PropTag.ConversationPreviewMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationWorkingSetSourcePartition = PropertyTagPropertyDefinition.InternalCreate("Conversation WorkingSetSourcePartition", PropTag.ConversationWorkingSetSourcePartition);

		public static readonly PropertyTagPropertyDefinition InternalConversationMVItemIds = PropertyTagPropertyDefinition.InternalCreate("Conversation ItemIds", PropTag.ConversationMvItemIds);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalMVItemIds = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide ItemIds", PropTag.ConversationMvItemIdsMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalRichContent = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide RichContent", PropTag.ConversationMessageRichContentMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonCompanyName = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide CompanyName", PropTag.PersonCompanyNameMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonDisplayName = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide DisplayName", PropTag.PersonDisplayNameMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonGivenName = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide GivenName", PropTag.PersonGivenNameMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonSurname = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide Surname", PropTag.PersonSurnameMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonFileAs = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide FileAs", PropTag.PersonFileAsMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonHomeCity = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide HomeCity", PropTag.PersonHomeCityMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonCreationTime = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide CreationTime", PropTag.PersonCreationTimeMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonRelevanceScore = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide RelevanceScore", PropTag.PersonRelevanceScoreMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonWorkCity = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide WorkCity", PropTag.PersonWorkCityMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonDisplayNameFirstLast = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide DisplayNameFirstLast", PropTag.PersonDisplayNameFirstLastMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalPersonDisplayNameLastFirst = PropertyTagPropertyDefinition.InternalCreate("Person MailboxWide DisplayNameLastFirst", PropTag.PersonDisplayNameLastFirstMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationHasClutter = PropertyTagPropertyDefinition.InternalCreate("Conversation HasClutter", PropTag.ConversationHasClutter);

		public static readonly PropertyTagPropertyDefinition InternalConversationGlobalHasClutter = PropertyTagPropertyDefinition.InternalCreate("Conversation MailboxWide HasClutter", PropTag.ConversationHasClutterMailboxWide);

		public static readonly PropertyTagPropertyDefinition InternalConversationInitialMemberDocumentId = PropertyTagPropertyDefinition.InternalCreate("Conversation Initial Member DocumentId", PropTag.ConversationInitialMemberDocumentId);

		public static readonly PropertyTagPropertyDefinition InternalConversationMemberDocumentIds = PropertyTagPropertyDefinition.InternalCreate("Conversation Member DocumentIds", PropTag.ConversationMemberDocumentIds);

		public static readonly GuidIdPropertyDefinition HasWrittenTracking = InternalSchema.CreateGuidIdProperty("HasWrittenTracking", typeof(bool), WellKnownPropertySet.Tracking, 34824);

		public static readonly PropertyTagPropertyDefinition ReportTag = PropertyTagPropertyDefinition.InternalCreate("ReportTag", PropTag.ReportTag);

		public static readonly GuidIdPropertyDefinition VotingResponse = InternalSchema.CreateGuidIdProperty("VotingResponse", typeof(string), WellKnownPropertySet.Common, 34084);

		public static readonly GuidIdPropertyDefinition OutlookUserPropsVerbStream = InternalSchema.CreateGuidIdProperty("dispidVerbStream", typeof(byte[]), WellKnownPropertySet.Common, 34080);

		[Obsolete("Use InternalSchema.OutlookUserPropsVerbStream instead.")]
		public static readonly GuidIdPropertyDefinition VotingBlob = InternalSchema.OutlookUserPropsVerbStream;

		public static readonly GuidIdPropertyDefinition IsVotingResponse = InternalSchema.CreateGuidIdProperty("IsVotingResponse", typeof(int), WellKnownPropertySet.Common, 34074);

		public static readonly PropertyTagPropertyDefinition BodyTag = PropertyTagPropertyDefinition.InternalCreate("BodyTag", PropTag.BodyTag);

		public static readonly PropertyTagPropertyDefinition RuleActions = PropertyTagPropertyDefinition.InternalCreate("RuleActions", PropTag.RuleActions);

		public static readonly PropertyTagPropertyDefinition RuleCondition = PropertyTagPropertyDefinition.InternalCreate("RuleCondition", PropTag.RuleCondition);

		public static readonly GuidNamePropertyDefinition XSharingBrowseUrl = InternalSchema.CreateGuidNameProperty("x-sharing-browse-url", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-browse-url");

		public static readonly GuidNamePropertyDefinition XSharingCapabilities = InternalSchema.CreateGuidNameProperty("x-sharing-capabilities", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-capabilities");

		public static readonly GuidNamePropertyDefinition XSharingFlavor = InternalSchema.CreateGuidNameProperty("x-sharing-flavor", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-flavor");

		public static readonly GuidNamePropertyDefinition XSharingInstanceGuid = InternalSchema.CreateGuidNameProperty("x-sharing-instance-guid", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-instance-guid");

		public static readonly GuidNamePropertyDefinition XSharingLocalType = InternalSchema.CreateGuidNameProperty("x-sharing-local-type", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-local-type");

		public static readonly GuidNamePropertyDefinition XSharingProviderGuid = InternalSchema.CreateGuidNameProperty("x-sharing-provider-guid", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-provider-guid");

		public static readonly GuidNamePropertyDefinition XSharingProviderName = InternalSchema.CreateGuidNameProperty("x-sharing-provider-name", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-provider-name");

		public static readonly GuidNamePropertyDefinition XSharingProviderUrl = InternalSchema.CreateGuidNameProperty("x-sharing-provider-url", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-provider-url");

		public static readonly GuidNamePropertyDefinition XSharingRemoteName = InternalSchema.CreateGuidNameProperty("x-sharing-remote-name", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-remote-name");

		public static readonly GuidNamePropertyDefinition XSharingRemotePath = InternalSchema.CreateGuidNameProperty("x-sharing-remote-path", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-remote-path");

		public static readonly GuidNamePropertyDefinition XSharingRemoteType = InternalSchema.CreateGuidNameProperty("x-sharing-remote-type", typeof(string), WellKnownPropertySet.InternetHeaders, "x-sharing-remote-type");

		public static readonly GuidNamePropertyDefinition XGroupMailboxSmtpAddressId = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-GroupMailbox-Id", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-GroupMailbox-Id");

		public static readonly GuidNamePropertyDefinition TextMessageDeliveryStatus = InternalSchema.CreateGuidNameProperty("TextMessageDeliveryStatus", typeof(int), WellKnownPropertySet.Messaging, "TextMessaging:DeliveryStatus");

		public static readonly PropertyTagPropertyDefinition ParentFid = PropertyTagPropertyDefinition.InternalCreate("Parent FID", (PropTag)1732837396U, PropertyFlags.ReadOnly);

		public static readonly PropertyTagPropertyDefinition Fid = PropertyTagPropertyDefinition.InternalCreate("FID", PropTag.Fid, PropertyFlags.ReadOnly);

		public static readonly GuidNamePropertyDefinition PushNotificationFolderEntryId = InternalSchema.CreateGuidNameProperty("PushNotificationFolderEntryId", typeof(byte[]), WellKnownPropertySet.PushNotificationSubscription, "PushNotificationFolderEntryId");

		public static readonly GuidNamePropertyDefinition PushNotificationSubscriptionId = InternalSchema.CreateGuidNameProperty("PushNotificationSubscriptionId", typeof(string), WellKnownPropertySet.PushNotificationSubscription, "PushNotificationSubscriptionId");

		public static readonly GuidNamePropertyDefinition PushNotificationSubscriptionLastUpdateTimeUTC = InternalSchema.CreateGuidNameProperty("PushNotificationSubscriptionLastUpdateTimeUTC", typeof(ExDateTime), WellKnownPropertySet.PushNotificationSubscription, "PushNotificationSubscriptionLastUpdateTimeUTC");

		public static readonly GuidNamePropertyDefinition PushNotificationSubscription = InternalSchema.CreateGuidNameProperty("SerializedPushNotificationSubscription", typeof(string), WellKnownPropertySet.PushNotificationSubscription, "SerializedPushNotificationSubscription");

		public static readonly GuidNamePropertyDefinition GroupNotificationsFolderEntryId = InternalSchema.CreateGuidNameProperty("GroupNotificationsFolderEntryId", typeof(byte[]), WellKnownPropertySet.GroupNotifications, "GroupNotificationsFolderEntryId");

		public static readonly GuidNamePropertyDefinition OutlookServiceFolderEntryId = InternalSchema.CreateGuidNameProperty("OutlookServiceFolderEntryId", typeof(byte[]), WellKnownPropertySet.OutlookService, "OutlookServiceFolderEntryId");

		public static readonly GuidNamePropertyDefinition OutlookServiceSubscriptionId = InternalSchema.CreateGuidNameProperty("OutlookServiceSubscriptionId", typeof(string), WellKnownPropertySet.OutlookService, "OutlookServiceSubscriptionId");

		public static readonly GuidNamePropertyDefinition OutlookServiceSubscriptionLastUpdateTimeUTC = InternalSchema.CreateGuidNameProperty("OutlookServiceSubscriptionLastUpdateTimeUTC", typeof(ExDateTime), WellKnownPropertySet.OutlookService, "OutlookServiceSubscriptionLastUpdateTimeUTC");

		public static readonly GuidNamePropertyDefinition OutlookServiceAppId = InternalSchema.CreateGuidNameProperty("OutlookServiceAppId", typeof(string), WellKnownPropertySet.OutlookService, "OutlookServiceAppId");

		public static readonly GuidNamePropertyDefinition OutlookServicePackageId = InternalSchema.CreateGuidNameProperty("OutlookServicePackageId", typeof(string), WellKnownPropertySet.OutlookService, "OutlookServicePackageId");

		public static readonly GuidNamePropertyDefinition OutlookServiceDeviceNotificationId = InternalSchema.CreateGuidNameProperty("OutlookServiceDeviceNotificationId", typeof(string), WellKnownPropertySet.OutlookService, "OutlookServiceDeviceNotificationId");

		public static readonly GuidNamePropertyDefinition OutlookServiceExpirationTime = InternalSchema.CreateGuidNameProperty("OutlookServiceExpirationTime", typeof(ExDateTime), WellKnownPropertySet.OutlookService, "OutlookServiceExpirationTime");

		public static readonly GuidNamePropertyDefinition OutlookServiceLockScreen = InternalSchema.CreateGuidNameProperty("OutlookServiceLockScreen", typeof(bool), WellKnownPropertySet.OutlookService, "OutlookServiceLockScreen");

		public static readonly GuidNamePropertyDefinition SnackyAppsFolderEntryId = InternalSchema.CreateGuidNameProperty("SnackyAppsFolderEntryId", typeof(byte[]), WellKnownPropertySet.Common, "SnackyAppsFolderEntryId");

		public static readonly GuidNamePropertyDefinition EventTimeBasedInboxReminders = InternalSchema.CreateGuidNameProperty("EventTimeBasedInboxReminders", typeof(byte[]), WellKnownPropertySet.Reminders, "EventTimeBasedInboxReminders", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition HasExceptionalInboxReminders = InternalSchema.CreateGuidNameProperty("HasExceptionalInboxReminders", typeof(bool), WellKnownPropertySet.Reminders, "HasExceptionalInboxReminders");

		public static readonly GuidNamePropertyDefinition EventTimeBasedInboxRemindersState = InternalSchema.CreateGuidNameProperty("EventTimeBasedInboxRemindersState", typeof(byte[]), WellKnownPropertySet.Reminders, "EventTimeBasedInboxRemindersState", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition ModernReminders = InternalSchema.CreateGuidNameProperty("ModernReminders", typeof(byte[]), WellKnownPropertySet.Reminders, "ModernReminders", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition ModernRemindersState = InternalSchema.CreateGuidNameProperty("ModernRemindersState", typeof(byte[]), WellKnownPropertySet.Reminders, "ModernRemindersState", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition ReminderId = InternalSchema.CreateGuidNameProperty("ReminderId", typeof(Guid), WellKnownPropertySet.Reminders, "ReminderId");

		public static readonly GuidNamePropertyDefinition ReminderItemGlobalObjectId = InternalSchema.CreateGuidNameProperty("ReminderItemGlobalObjectId", typeof(byte[]), WellKnownPropertySet.Reminders, "ReminderItemGlobalObjectId");

		public static readonly GuidNamePropertyDefinition ReminderOccurrenceGlobalObjectId = InternalSchema.CreateGuidNameProperty("ReminderOccurrenceGlobalObjectId", typeof(byte[]), WellKnownPropertySet.Reminders, "ReminderOccurrenceGlobalObjectId");

		public static readonly GuidNamePropertyDefinition ReminderText = InternalSchema.CreateGuidNameProperty("ReminderText", typeof(string), WellKnownPropertySet.Reminders, "ReminderText");

		public static readonly GuidNamePropertyDefinition ReminderStartTime = InternalSchema.CreateGuidNameProperty("ReminderStartTime", typeof(ExDateTime), WellKnownPropertySet.Reminders, "ReminderStartTime");

		public static readonly GuidNamePropertyDefinition ReminderEndTime = InternalSchema.CreateGuidNameProperty("ReminderEndTime", typeof(ExDateTime), WellKnownPropertySet.Reminders, "ReminderEndTime");

		public static readonly GuidNamePropertyDefinition ScheduledReminderTime = InternalSchema.CreateGuidNameProperty("ScheduledReminderTime", typeof(ExDateTime), WellKnownPropertySet.Reminders, "ScheduledReminderTime");

		public static readonly PropertyTagPropertyDefinition IsContactPhoto = PropertyTagPropertyDefinition.InternalCreate("IsContactPhoto", (PropTag)2147418123U);

		public static readonly GuidIdPropertyDefinition HasPicture = InternalSchema.CreateGuidIdProperty("HasPicture", typeof(bool), WellKnownPropertySet.Address, 32789);

		public static readonly PropertyTagPropertyDefinition RelevanceScore = PropertyTagPropertyDefinition.InternalCreate("RelevanceScore", PropTag.RelevanceScore);

		public static readonly PropertyTagPropertyDefinition ReportingMta = PropertyTagPropertyDefinition.InternalCreate("ReportingMta", (PropTag)1746927647U);

		public static readonly GuidNamePropertyDefinition XMSJournalReport = InternalSchema.CreateGuidNameProperty("XMSJournalReport", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Journal-Report");

		public static readonly GuidNamePropertyDefinition ApprovalAllowedDecisionMakers = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-Approval-Allowed-Decision-Makers");

		public static readonly GuidNamePropertyDefinition ApprovalRequestor = InternalSchema.CreateGuidNameProperty("X-MS-Exchange-Organization-Approval-Requestor", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MS-Exchange-Organization-Approval-Requestor");

		public static readonly GuidNamePropertyDefinition ApprovalDecisionMaker = InternalSchema.CreateGuidNameProperty("ApprovalDecisionMaker", typeof(string), WellKnownPropertySet.Messaging, "ApprovalDecisionMaker");

		public static readonly GuidNamePropertyDefinition ApprovalDecision = InternalSchema.CreateGuidNameProperty("ApprovalDecision", typeof(int), WellKnownPropertySet.Messaging, "ApprovalDecision");

		public static readonly GuidNamePropertyDefinition ApprovalDecisionTime = InternalSchema.CreateGuidNameProperty("ApprovalDecisionTime", typeof(ExDateTime), WellKnownPropertySet.Messaging, "ApprovalDecisionTime");

		public static readonly GuidNamePropertyDefinition ApprovalRequestMessageId = InternalSchema.CreateGuidNameProperty("ApprovalRequestMessageId", typeof(string), WellKnownPropertySet.Messaging, "ApprovalRequestMessageId");

		public static readonly GuidNamePropertyDefinition ApprovalStatus = InternalSchema.CreateGuidNameProperty("ApprovalStatus", typeof(int), WellKnownPropertySet.Messaging, "ApprovalStatus");

		public static readonly GuidNamePropertyDefinition ApprovalDecisionMakersNdred = InternalSchema.CreateGuidNameProperty("ApprovalDecisionMakersNdred", typeof(string), WellKnownPropertySet.Messaging, "ApprovalDecisionMakersNdred");

		public static readonly GuidNamePropertyDefinition ApprovalApplicationId = InternalSchema.CreateGuidNameProperty("ApprovalApplicationId", typeof(int), WellKnownPropertySet.Messaging, "ApprovalApplicationId");

		public static readonly GuidNamePropertyDefinition ApprovalApplicationData = InternalSchema.CreateGuidNameProperty("ApprovalApplicationData", typeof(string), WellKnownPropertySet.Messaging, "ApprovalApplicationData");

		public static readonly GuidNamePropertyDefinition MessageBccMe = InternalSchema.CreateGuidNameProperty("MessageBccMe", typeof(bool), WellKnownPropertySet.Messaging, "MessageBccMe");

		public static readonly GuidNamePropertyDefinition IsSigned = InternalSchema.CreateGuidNameProperty("IsSigned", typeof(bool), WellKnownPropertySet.Messaging, "IsSigned");

		public static readonly GuidNamePropertyDefinition IsReadReceipt = InternalSchema.CreateGuidNameProperty("IsReadReceipt", typeof(bool), WellKnownPropertySet.Messaging, "IsReadReceipt");

		public static readonly GuidNamePropertyDefinition XMSExchangeOutlookProtectionRuleVersion = InternalSchema.CreateGuidNameProperty("OutlookProtectionRuleVersion", typeof(string), WellKnownPropertySet.UnifiedMessaging, "X-MS-Exchange-Organization-Outlook-Protection-Rule-Addin-Version");

		public static readonly GuidNamePropertyDefinition XMSExchangeOutlookProtectionRuleConfigTimestamp = InternalSchema.CreateGuidNameProperty("OutlookProtectionRuleConfigTimestamp", typeof(string), WellKnownPropertySet.UnifiedMessaging, "X-MS-Exchange-Organization-Outlook-Protection-Rule-Config-Timestamp");

		public static readonly GuidNamePropertyDefinition XMSExchangeOutlookProtectionRuleOverridden = InternalSchema.CreateGuidNameProperty("OutlookProtectionRuleOverridden", typeof(string), WellKnownPropertySet.UnifiedMessaging, "X-MS-Exchange-Organization-Outlook-Protection-Rule-Overridden");

		public static readonly GuidNamePropertyDefinition OscContactSources = InternalSchema.CreateGuidNameProperty("OscContactSources", typeof(string[]), WellKnownPropertySet.PublicStrings, "OscContactSources");

		public static readonly GuidNamePropertyDefinition OscContactSourcesForContact = InternalSchema.CreateGuidNameProperty("OscContactSourcesForContact", typeof(byte[]), WellKnownPropertySet.PublicStrings, "OscContactSources");

		public static readonly PropertyTagPropertyDefinition OscSyncEnabledOnServer = PropertyTagPropertyDefinition.InternalCreate("OscSyncEnabledOnServer", PropTag.OscSyncEnabledOnServer);

		public static readonly GuidNamePropertyDefinition PeopleConnectionCreationTime = InternalSchema.CreateGuidNameProperty("PeopleConnectionCreationTime", typeof(ExDateTime), WellKnownPropertySet.PublicStrings, "PeopleConnectionCreationTime");

		public static readonly GuidNamePropertyDefinition DlpSenderOverride = InternalSchema.CreateGuidNameProperty("DlpSenderOverride", typeof(string), WellKnownPropertySet.InternetHeaders, "X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification");

		public static readonly GuidNamePropertyDefinition DlpFalsePositive = InternalSchema.CreateGuidNameProperty("DlpFalsePositive", typeof(string), WellKnownPropertySet.InternetHeaders, "X-Ms-Exchange-Organization-Dlp-FalsePositive");

		public static readonly GuidNamePropertyDefinition DlpDetectedClassifications = InternalSchema.CreateGuidNameProperty("DlpDetectedClassifications", typeof(string), WellKnownPropertySet.InternetHeaders, "X-Ms-Exchange-Organization-Dlp-DetectedClassifications");

		public static readonly GuidNamePropertyDefinition DlpDetectedClassificationObjects = InternalSchema.CreateGuidNameProperty("DlpDetectedClassificationObjects", typeof(byte[]), WellKnownPropertySet.Messaging, "DlpDetectedClassificationObjects", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition HasDlpDetectedClassifications = InternalSchema.CreateGuidNameProperty("HasDlpDetectedClassifications", typeof(string), WellKnownPropertySet.Messaging, "HasDlpDetectedClassifications");

		public static readonly GuidNamePropertyDefinition RecoveryOptions = InternalSchema.CreateGuidNameProperty("RecoveryOptions", typeof(int), WellKnownPropertySet.Messaging, "RecoveryOptions");

		public static readonly PropertyTagPropertyDefinition RichContent = PropertyTagPropertyDefinition.InternalCreate("RichContent", PropTag.RichContent);

		public static readonly GuidIdPropertyDefinition TaskResetReminder = InternalSchema.CreateGuidIdProperty("TaskResetReminder", typeof(bool), WellKnownPropertySet.Task, 33031);

		public static readonly GuidIdPropertyDefinition IsOneOff = InternalSchema.CreateGuidIdProperty("IsOneOff", typeof(bool), WellKnownPropertySet.Task, 33033);

		public static readonly GuidIdPropertyDefinition TaskDueDate = InternalSchema.CreateGuidIdProperty("TaskDueDate", typeof(string), WellKnownPropertySet.Common, 33029);

		public static readonly GuidIdPropertyDefinition CompleteDate = InternalSchema.CreateGuidIdProperty("DateCompleted", typeof(ExDateTime), WellKnownPropertySet.Task, 33039);

		public static readonly GuidIdPropertyDefinition FlagSubject = InternalSchema.CreateGuidIdProperty("FlagSubject", typeof(string), WellKnownPropertySet.Common, 34212);

		public static readonly GuidIdPropertyDefinition LocalDueDate = InternalSchema.CreateGuidIdProperty("LocalDueDate", typeof(ExDateTime), WellKnownPropertySet.Task, 33029);

		public static readonly GuidIdPropertyDefinition LocalStartDate = InternalSchema.CreateGuidIdProperty("LocalStartDate", typeof(ExDateTime), WellKnownPropertySet.Task, 33028);

		public static readonly GuidIdPropertyDefinition UtcStartDate = InternalSchema.CreateGuidIdProperty("CommonStart", typeof(ExDateTime), WellKnownPropertySet.Common, 34070);

		public static readonly GuidNamePropertyDefinition DoItTime = InternalSchema.CreateGuidNameProperty("DoItTime", typeof(ExDateTime), WellKnownPropertySet.Task, "DoItTime", PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition UtcDueDate = InternalSchema.CreateGuidIdProperty("CommonEnd", typeof(ExDateTime), WellKnownPropertySet.Common, 34071);

		public static readonly GuidIdPropertyDefinition TaskStatus = InternalSchema.CreateGuidIdProperty("Status", typeof(int), WellKnownPropertySet.Task, 33025);

		public static readonly GuidIdPropertyDefinition StatusDescription = InternalSchema.CreateGuidIdProperty("StatusDescription", typeof(string), WellKnownPropertySet.Task, 33079);

		public static readonly GuidIdPropertyDefinition PercentComplete = InternalSchema.CreateGuidIdProperty("PercentComplete", typeof(double), WellKnownPropertySet.Task, 33026, PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new RangedValueConstraint<double>(0.0, 1.0))
		});

		public static readonly GuidIdPropertyDefinition TaskRecurrence = InternalSchema.CreateGuidIdProperty("TaskRecurrence", typeof(byte[]), WellKnownPropertySet.Task, 33046);

		public static readonly GuidIdPropertyDefinition MapiIsTaskRecurring = InternalSchema.CreateGuidIdProperty("MapiIsTaskRecurring", typeof(bool), WellKnownPropertySet.Task, 33062);

		public static readonly GuidIdPropertyDefinition IsComplete = InternalSchema.CreateGuidIdProperty("IsComplete", typeof(bool), WellKnownPropertySet.Task, 33052);

		public static readonly GuidIdPropertyDefinition TotalWork = InternalSchema.CreateGuidIdProperty("TotalWork", typeof(int), WellKnownPropertySet.Task, 33041, PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new NonMoveMailboxPropertyConstraint(new RangedValueConstraint<int>(0, 1525252319))
		});

		public static readonly GuidIdPropertyDefinition ActualWork = InternalSchema.CreateGuidIdProperty("ActualWork", typeof(int), WellKnownPropertySet.Task, 33040, PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1525252319)
		});

		public static readonly GuidIdPropertyDefinition Contacts = InternalSchema.CreateGuidIdProperty("Contacts", typeof(string[]), WellKnownPropertySet.Common, 34106);

		public static readonly GuidIdPropertyDefinition TaskOwner = InternalSchema.CreateGuidIdProperty("Owner", typeof(string), WellKnownPropertySet.Task, 33055);

		public static readonly GuidIdPropertyDefinition LastModifiedBy = InternalSchema.CreateGuidIdProperty("LastModifiedBy", typeof(string), WellKnownPropertySet.Task, 33058);

		public static readonly GuidIdPropertyDefinition TaskDelegator = InternalSchema.CreateGuidIdProperty("Delegator", typeof(string), WellKnownPropertySet.Task, 33057);

		public static readonly GuidIdPropertyDefinition AssignedTime = InternalSchema.CreateGuidIdProperty("AssignedTime", typeof(ExDateTime), WellKnownPropertySet.Task, 33045);

		public static readonly GuidIdPropertyDefinition OwnershipState = InternalSchema.CreateGuidIdProperty("Ownership", typeof(int), WellKnownPropertySet.Task, 33065);

		public static readonly GuidIdPropertyDefinition DelegationState = InternalSchema.CreateGuidIdProperty("DelegationState", typeof(int), WellKnownPropertySet.Task, 33066);

		public static readonly GuidIdPropertyDefinition IsAssignmentEditable = InternalSchema.CreateGuidIdProperty("IsAssignmentEditable", typeof(int), WellKnownPropertySet.Common, 34072);

		public static readonly GuidIdPropertyDefinition TaskType = InternalSchema.CreateGuidIdProperty("TaskType", typeof(int), WellKnownPropertySet.Task, 33043);

		public static readonly GuidIdPropertyDefinition IsTeamTask = InternalSchema.CreateGuidIdProperty("TeamTask", typeof(bool), WellKnownPropertySet.Task, 33027);

		public static readonly GuidIdPropertyDefinition TaskChangeCount = InternalSchema.CreateGuidIdProperty("TaskChangeCount", typeof(int), WellKnownPropertySet.Task, 33042);

		public static readonly GuidIdPropertyDefinition LastUpdateType = InternalSchema.CreateGuidIdProperty("LastUpdateType", typeof(int), WellKnownPropertySet.Task, 33050);

		public static readonly GuidIdPropertyDefinition TaskAccepted = InternalSchema.CreateGuidIdProperty("TaskAccepted", typeof(bool), WellKnownPropertySet.Task, 33032);

		public static readonly PropertyTagPropertyDefinition MapiToDoItemFlag = PropertyTagPropertyDefinition.InternalCreate("MapiToDoItemFlag", (PropTag)237699075U);

		public static readonly GuidIdPropertyDefinition ToDoSubOrdinal = InternalSchema.CreateGuidIdProperty("dispidToDoSubOrdinal", typeof(string), WellKnownPropertySet.Common, 34209);

		public static readonly GuidIdPropertyDefinition ToDoOrdinalDate = InternalSchema.CreateGuidIdProperty("dispidToDoOrdinalDate", typeof(ExDateTime), WellKnownPropertySet.Common, 34208);

		public static readonly GuidIdPropertyDefinition FlagStringAllowed = InternalSchema.CreateGuidIdProperty("dispidAllowedFlagString", typeof(int), WellKnownPropertySet.Common, 61624);

		public static readonly GuidIdPropertyDefinition ValidFlagStringProof = InternalSchema.CreateGuidIdProperty("dispidValidFlagStringProof", typeof(ExDateTime), WellKnownPropertySet.Common, 34239);

		public static readonly XmlAttributePropertyDefinition CategoryListDefaultCategory = new XmlAttributePropertyDefinition("CategoryListDefaultCategory", typeof(string), "default", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryListLastSavedSession = new XmlAttributePropertyDefinition("CategoryListLastSaveSession", typeof(int), "lastSavedSession", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryListLastSavedTime = new XmlAttributePropertyDefinition("CategoryListLastSaveTime", typeof(ExDateTime), "lastSavedTime", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryAllowRenameOnFirstUse = new XmlAttributePropertyDefinition("CategoryAllowRenameOnFirstUse", typeof(bool), "renameOnFirstUse", () => false, new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryName = new XmlAttributePropertyDefinition("CategoryName", typeof(string), "name", new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255),
			new CharacterConstraint(Category.ProhibitedCharacters, false)
		});

		public static readonly XmlAttributePropertyDefinition CategoryColor = new XmlAttributePropertyDefinition("CategoryColor", typeof(int), "color", () => -1, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(-1, 24)
		});

		public static readonly XmlAttributePropertyDefinition CategoryKeyboardShortcut = new XmlAttributePropertyDefinition("CategoryKeyboardShortcut", typeof(int), "keyboardShortcut", () => 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 11)
		});

		public static readonly XmlAttributePropertyDefinition CategoryLastTimeUsedNotes = new XmlAttributePropertyDefinition("CategoryLastTimeUsedNotes", typeof(ExDateTime), "lastTimeUsedNotes", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryLastTimeUsedJournal = new XmlAttributePropertyDefinition("CategoryLastTimeUsedJournal", typeof(ExDateTime), "lastTimeUsedJournal", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryLastTimeUsedContacts = new XmlAttributePropertyDefinition("CategoryLastTimeUsedContacts", typeof(ExDateTime), "lastTimeUsedContacts", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryLastTimeUsedTasks = new XmlAttributePropertyDefinition("CategoryLastTimeUsedTasks", typeof(ExDateTime), "lastTimeUsedTasks", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryLastTimeUsedCalendar = new XmlAttributePropertyDefinition("CategoryLastTimeUsedCalendar", typeof(ExDateTime), "lastTimeUsedCalendar", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryLastTimeUsedMail = new XmlAttributePropertyDefinition("CategoryLastTimeUsedMail", typeof(ExDateTime), "lastTimeUsedMail", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryLastTimeUsed = new XmlAttributePropertyDefinition("CategoryLastTimeUsed", typeof(ExDateTime), "lastTimeUsed", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryLastSessionUsed = new XmlAttributePropertyDefinition("CategoryLastSessionUsed", typeof(int), "lastSessionUsed", new PropertyDefinitionConstraint[0]);

		public static readonly XmlAttributePropertyDefinition CategoryGuid = new XmlAttributePropertyDefinition("CategoryGuid", typeof(Guid), "guid", () => Guid.NewGuid(), new PropertyDefinitionConstraint[0]);

		public static readonly PropertyTagPropertyDefinition OriginatorRequestedAlternateRecipientEntryId = PropertyTagPropertyDefinition.InternalCreate("OriginatorRequestedAlternateRecipientEntryId", PropTag.OriginatorRequestedAlternateRecipient);

		public static readonly PropertyTagPropertyDefinition RedirectionHistory = PropertyTagPropertyDefinition.InternalCreate("RedirectionHistory", PropTag.RedirectionHistory);

		public static readonly PropertyTagPropertyDefinition DlExpansionProhibited = PropertyTagPropertyDefinition.InternalCreate("DlExpansionProhibited", PropTag.DlExpansionProhibited);

		public static readonly PropertyTagPropertyDefinition RecipientReassignmentProhibited = PropertyTagPropertyDefinition.InternalCreate("RecipientReassignmentProhibited", PropTag.RecipientReassignmentProhibited);

		public static readonly GuidIdPropertyDefinition SharingStatus = InternalSchema.CreateGuidIdProperty("SharingStatus", typeof(int), WellKnownPropertySet.Sharing, 35328);

		public static readonly GuidIdPropertyDefinition SharingProviderGuid = InternalSchema.CreateGuidIdProperty("SharingProviderGuid", typeof(Guid), WellKnownPropertySet.Sharing, 35329);

		[Obsolete("Use InternalSchema.SharingProviderGuid instead.")]
		public static readonly GuidIdPropertyDefinition ProviderGuid = InternalSchema.SharingProviderGuid;

		public static readonly GuidIdPropertyDefinition SharingProviderName = InternalSchema.CreateGuidIdProperty("SharingProviderName", typeof(string), WellKnownPropertySet.Sharing, 35330);

		public static readonly GuidIdPropertyDefinition SharingProviderUrl = InternalSchema.CreateGuidIdProperty("SharingProviderUrl", typeof(string), WellKnownPropertySet.Sharing, 35331);

		public static readonly GuidIdPropertyDefinition SharingRemotePath = InternalSchema.CreateGuidIdProperty("SharingRemotePath", typeof(string), WellKnownPropertySet.Sharing, 35332);

		[Obsolete("Use InternalSchema.SharingRemotePath instead.")]
		public static readonly GuidIdPropertyDefinition SharepointFolder = InternalSchema.SharingRemotePath;

		public static readonly GuidIdPropertyDefinition SharingRemoteName = InternalSchema.CreateGuidIdProperty("SharingRemoteName", typeof(string), WellKnownPropertySet.Sharing, 35333);

		[Obsolete("Use InternalSchema.SharingRemoteName instead.")]
		public static readonly GuidIdPropertyDefinition SharepointFolderDisplayName = InternalSchema.SharingRemoteName;

		public static readonly GuidIdPropertyDefinition SharingRemoteUid = InternalSchema.CreateGuidIdProperty("SharingRemoteUid", typeof(string), WellKnownPropertySet.Sharing, 35334);

		public static readonly GuidIdPropertyDefinition SharingInitiatorName = InternalSchema.CreateGuidIdProperty("SharingInitiatorName", typeof(string), WellKnownPropertySet.Sharing, 35335);

		public static readonly GuidIdPropertyDefinition SharingInitiatorSmtp = InternalSchema.CreateGuidIdProperty("SharingInitiatorSmtp", typeof(string), WellKnownPropertySet.Sharing, 35336);

		public static readonly GuidIdPropertyDefinition SharingInitiatorEntryId = InternalSchema.CreateGuidIdProperty("SharingInitiatorEntryId", typeof(byte[]), WellKnownPropertySet.Sharing, 35337);

		public static readonly GuidIdPropertyDefinition SharingLocalName = InternalSchema.CreateGuidIdProperty("SharingLocalName", typeof(string), WellKnownPropertySet.Sharing, 35343);

		public static readonly GuidIdPropertyDefinition SharingLocalUid = InternalSchema.CreateGuidIdProperty("SharingLocalUid", typeof(string), WellKnownPropertySet.Sharing, 35344);

		public static readonly GuidIdPropertyDefinition SharingRemoteUser = InternalSchema.CreateGuidIdProperty("SharingRemoteUser", typeof(string), WellKnownPropertySet.Sharing, 35340);

		public static readonly GuidIdPropertyDefinition SharingRemotePass = InternalSchema.CreateGuidIdProperty("SharingRemotePass", typeof(string), WellKnownPropertySet.Sharing, 35341);

		public static readonly GuidIdPropertyDefinition SharingLocalType = InternalSchema.CreateGuidIdProperty("SharingLocalType", typeof(string), WellKnownPropertySet.Sharing, 35348);

		public static readonly GuidIdPropertyDefinition SharingCapabilities = InternalSchema.CreateGuidIdProperty("SharingCapabilities", typeof(int), WellKnownPropertySet.Sharing, 35351);

		public static readonly GuidIdPropertyDefinition SharingFlavor = InternalSchema.CreateGuidIdProperty("SharingFlavor", typeof(int), WellKnownPropertySet.Sharing, 35352);

		public static readonly GuidIdPropertyDefinition SharingPermissions = InternalSchema.CreateGuidIdProperty("SharingPermissions", typeof(int), WellKnownPropertySet.Sharing, 35355);

		public static readonly GuidIdPropertyDefinition SharingInstanceGuid = InternalSchema.CreateGuidIdProperty("SharingInstanceGuid", typeof(Guid), WellKnownPropertySet.Sharing, 35356);

		public static readonly GuidIdPropertyDefinition SharingRemoteType = InternalSchema.CreateGuidIdProperty("SharingRemoteType", typeof(string), WellKnownPropertySet.Sharing, 35357);

		public static readonly GuidIdPropertyDefinition SharingLastSync = InternalSchema.CreateGuidIdProperty("SharingLastSync", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35359);

		public static readonly GuidIdPropertyDefinition SharingRssHash = InternalSchema.CreateGuidIdProperty("SharingRssHash", typeof(byte[]), WellKnownPropertySet.Sharing, 35360);

		public static readonly GuidIdPropertyDefinition SharingRemoteLastMod = InternalSchema.CreateGuidIdProperty("SharingRemoteLastMod", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35362);

		public static readonly GuidIdPropertyDefinition SharingConfigUrl = InternalSchema.CreateGuidIdProperty("SharingConfigUrl", typeof(string), WellKnownPropertySet.Sharing, 35364);

		public static readonly GuidIdPropertyDefinition SharingResponseType = InternalSchema.CreateGuidIdProperty("SharingResponseType", typeof(int), WellKnownPropertySet.Sharing, 35367);

		public static readonly GuidIdPropertyDefinition SharingResponseTime = InternalSchema.CreateGuidIdProperty("SharingResponseTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35368);

		public static readonly GuidIdPropertyDefinition SharingOriginalMessageEntryId = InternalSchema.CreateGuidIdProperty("SharingOriginalMessageEntryId", typeof(byte[]), WellKnownPropertySet.Sharing, 35369);

		public static readonly GuidIdPropertyDefinition SharingDetail = InternalSchema.CreateGuidIdProperty("SharingDetail", typeof(int), WellKnownPropertySet.Sharing, 35371);

		public static readonly GuidIdPropertyDefinition SharingTimeToLive = InternalSchema.CreateGuidIdProperty("SharingTimeToLive", typeof(int), WellKnownPropertySet.Sharing, 35372);

		public static readonly GuidIdPropertyDefinition SharingBindingEid = InternalSchema.CreateGuidIdProperty("SharingBindingEid", typeof(byte[]), WellKnownPropertySet.Sharing, 35373);

		public static readonly GuidIdPropertyDefinition SharingIndexEid = InternalSchema.CreateGuidIdProperty("SharingIndexEid", typeof(byte[]), WellKnownPropertySet.Sharing, 35374);

		public static readonly GuidIdPropertyDefinition SharingRemoteComment = InternalSchema.CreateGuidIdProperty("SharingRemoteComment", typeof(string), WellKnownPropertySet.Sharing, 35375);

		public static readonly GuidIdPropertyDefinition SharingRemoteStoreUid = InternalSchema.CreateGuidIdProperty("SharingRemoteStoreUid", typeof(string), WellKnownPropertySet.Sharing, 35400);

		public static readonly GuidIdPropertyDefinition SharingLocalStoreUid = InternalSchema.CreateGuidIdProperty("SharingLocalStoreUid", typeof(string), WellKnownPropertySet.Sharing, 35401);

		public static readonly GuidIdPropertyDefinition SharingRemoteByteSize = InternalSchema.CreateGuidIdProperty("SharingRemoteByteSize", typeof(int), WellKnownPropertySet.Sharing, 35403);

		public static readonly GuidIdPropertyDefinition SharingRemoteCrc = InternalSchema.CreateGuidIdProperty("SharingRemoteCrc", typeof(int), WellKnownPropertySet.Sharing, 35404);

		public static readonly GuidIdPropertyDefinition SharingRoamLog = InternalSchema.CreateGuidIdProperty("SharingRoamLog", typeof(int), WellKnownPropertySet.Sharing, 35406);

		public static readonly GuidIdPropertyDefinition SharingBrowseUrl = InternalSchema.CreateGuidIdProperty("SharingBrowseUrl", typeof(string), WellKnownPropertySet.Sharing, 35409);

		public static readonly GuidIdPropertyDefinition SharingLastAutoSync = InternalSchema.CreateGuidIdProperty("SharingLastAutoSync", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35413);

		public static readonly GuidIdPropertyDefinition SharingSavedSession = InternalSchema.CreateGuidIdProperty("SharingSavedSession", typeof(Guid), WellKnownPropertySet.Sharing, 35422);

		public static readonly GuidIdPropertyDefinition SharingRemoteFolderId = InternalSchema.CreateGuidIdProperty("SharingRemoteFolderId", typeof(string), WellKnownPropertySet.Sharing, 35429);

		public static readonly GuidIdPropertyDefinition SharingLocalFolderEwsId = InternalSchema.CreateGuidIdProperty("SharingLocalFolderEwsId", typeof(string), WellKnownPropertySet.Sharing, 35430);

		public static readonly GuidNamePropertyDefinition SharingLastSubscribeTime = InternalSchema.CreateGuidNameProperty("SharingLastSubscribeTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, "SharingLastSubscribeTime");

		public static readonly GuidIdPropertyDefinition SharingDetailedStatus = InternalSchema.CreateGuidIdProperty("SharingDetailedStatus", typeof(int), WellKnownPropertySet.Sharing, 35488);

		public static readonly GuidIdPropertyDefinition SharingLastSuccessSyncTime = InternalSchema.CreateGuidIdProperty("SharingLastSuccessSyncTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35492);

		public static readonly GuidIdPropertyDefinition SharingSyncRange = InternalSchema.CreateGuidIdProperty("SharingSyncRange", typeof(int), WellKnownPropertySet.Sharing, 35493);

		public static readonly GuidIdPropertyDefinition SharingAggregationStatus = InternalSchema.CreateGuidIdProperty("SharingAggregationStatus", typeof(int), WellKnownPropertySet.Sharing, 35494);

		public static readonly GuidIdPropertyDefinition SharingWlidAuthPolicy = InternalSchema.CreateGuidIdProperty("SharingWlidAuthPolicy", typeof(string), WellKnownPropertySet.Sharing, 35504);

		public static readonly GuidIdPropertyDefinition SharingWlidUserPuid = InternalSchema.CreateGuidIdProperty("SharingWlidUserPuid", typeof(string), WellKnownPropertySet.Sharing, 35505);

		public static readonly GuidIdPropertyDefinition SharingWlidAuthToken = InternalSchema.CreateGuidIdProperty("SharingWlidAuthToken", typeof(string), WellKnownPropertySet.Sharing, 35506);

		public static readonly GuidIdPropertyDefinition SharingWlidAuthTokenExpireTime = InternalSchema.CreateGuidIdProperty("SharingWlidAuthTokenExpireTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35507);

		public static readonly GuidIdPropertyDefinition SharingMinSyncPollInterval = InternalSchema.CreateGuidIdProperty("SharingMinSyncPollInterval", typeof(int), WellKnownPropertySet.Sharing, 35520);

		public static readonly GuidIdPropertyDefinition SharingMinSettingPollInterval = InternalSchema.CreateGuidIdProperty("SharingMinSettingPollInterval", typeof(int), WellKnownPropertySet.Sharing, 35521);

		public static readonly GuidIdPropertyDefinition SharingSyncMultiplier = InternalSchema.CreateGuidIdProperty("SharingSyncMultiplier", typeof(double), WellKnownPropertySet.Sharing, 35522);

		public static readonly GuidIdPropertyDefinition SharingMaxObjectsInSync = InternalSchema.CreateGuidIdProperty("SharingMaxObjectsInSync", typeof(int), WellKnownPropertySet.Sharing, 35523);

		public static readonly GuidIdPropertyDefinition SharingMaxNumberOfEmails = InternalSchema.CreateGuidIdProperty("SharingMaxNumberOfEmails", typeof(int), WellKnownPropertySet.Sharing, 35524);

		public static readonly GuidIdPropertyDefinition SharingMaxNumberOfFolders = InternalSchema.CreateGuidIdProperty("SharingMaxNumberOfFolders", typeof(int), WellKnownPropertySet.Sharing, 35525);

		public static readonly GuidIdPropertyDefinition SharingMaxAttachments = InternalSchema.CreateGuidIdProperty("SharingMaxAttachments", typeof(int), WellKnownPropertySet.Sharing, 35526);

		public static readonly GuidIdPropertyDefinition SharingMaxMessageSize = InternalSchema.CreateGuidIdProperty("SharingMaxMessageSize", typeof(long), WellKnownPropertySet.Sharing, 35527);

		public static readonly GuidIdPropertyDefinition SharingMaxRecipients = InternalSchema.CreateGuidIdProperty("SharingMaxRecipients", typeof(int), WellKnownPropertySet.Sharing, 35528);

		public static readonly GuidIdPropertyDefinition SharingMigrationState = InternalSchema.CreateGuidIdProperty("SharingMigrationState", typeof(int), WellKnownPropertySet.Sharing, 35529);

		public static readonly GuidIdPropertyDefinition SharingDiagnostics = InternalSchema.CreateGuidIdProperty("SharingDiagnostics", typeof(string), WellKnownPropertySet.Sharing, 35530);

		public static readonly GuidIdPropertyDefinition SharingPoisonCallstack = InternalSchema.CreateGuidIdProperty("SharingPoisonCallstack", typeof(string), WellKnownPropertySet.Sharing, 35531);

		public static readonly GuidIdPropertyDefinition SharingAggregationType = InternalSchema.CreateGuidIdProperty("SharingAggregationType", typeof(int), WellKnownPropertySet.Sharing, 35532);

		public static readonly GuidIdPropertyDefinition SharingSubscriptionConfiguration = InternalSchema.CreateGuidIdProperty("SharingSubscriptionConfiguration", typeof(int), WellKnownPropertySet.Sharing, 35584);

		public static readonly GuidIdPropertyDefinition SharingAggregationProtocolVersion = InternalSchema.CreateGuidIdProperty("SharingSharingAggregationProtocolVersion", typeof(int), WellKnownPropertySet.Sharing, 35585);

		public static readonly GuidIdPropertyDefinition SharingAggregationProtocolName = InternalSchema.CreateGuidIdProperty("SharingAggregationProtocolName", typeof(string), WellKnownPropertySet.Sharing, 35586);

		public static readonly GuidIdPropertyDefinition SharingSubscriptionName = InternalSchema.CreateGuidIdProperty("SharingSubscriptionName", typeof(string), WellKnownPropertySet.Sharing, 35587);

		public static readonly GuidNamePropertyDefinition SharingSubscriptionVersion = InternalSchema.CreateGuidNameProperty("SharingSubscriptionVersion", typeof(long), WellKnownPropertySet.Sharing, "SharingSubscriptionVersion");

		public static readonly GuidIdPropertyDefinition SharingSubscriptionsCache = InternalSchema.CreateGuidIdProperty("SharingSubscriptions", typeof(byte[]), WellKnownPropertySet.Sharing, 35840, PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidIdPropertyDefinition SharingAdjustedLastSuccessfulSyncTime = InternalSchema.CreateGuidIdProperty("SharingAdjustedLastSuccessfulSyncTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35841);

		public static readonly GuidIdPropertyDefinition SharingOutageDetectionDiagnostics = InternalSchema.CreateGuidIdProperty("SharingOutageDetectionDiagnostics", typeof(string), WellKnownPropertySet.Sharing, 35842);

		public static readonly GuidNamePropertyDefinition SharingSendAsState = InternalSchema.CreateGuidNameProperty("SharingSendAsState", typeof(int), WellKnownPropertySet.Sharing, "SharingSendAsState");

		public static readonly GuidNamePropertyDefinition SharingSendAsValidatedEmail = InternalSchema.CreateGuidNameProperty("SharingSendAsValidatedEmail", typeof(string), WellKnownPropertySet.Sharing, "SharingSendAsValidatedEmail");

		public static readonly GuidNamePropertyDefinition SharingSendAsSubmissionUrl = InternalSchema.CreateGuidNameProperty("SharingSendAsSubmissionUrl", typeof(string), WellKnownPropertySet.Sharing, "SharingSendAsSubmissionUrl");

		public static readonly GuidNamePropertyDefinition SharingEwsUri = InternalSchema.CreateGuidNameProperty("SharingEwsUri", typeof(string), WellKnownPropertySet.Sharing, "SharingEwsUri");

		public static readonly GuidNamePropertyDefinition SharingRemoteExchangeVersion = InternalSchema.CreateGuidNameProperty("SharingRemoteExchangeVersion", typeof(string), WellKnownPropertySet.Sharing, "SharingRemoteExchangeVersion");

		public static readonly GuidNamePropertyDefinition SharingSubscriptionCreationType = InternalSchema.CreateGuidNameProperty("SharingSubscriptionCreationType", typeof(int), WellKnownPropertySet.Sharing, "SharingSubscriptionCreationType");

		public static readonly GuidNamePropertyDefinition SharingSubscriptionSyncPhase = InternalSchema.CreateGuidNameProperty("SharingSubscriptionSyncPhase", typeof(int), WellKnownPropertySet.Sharing, "SharingSubscriptionSyncPhase");

		public static readonly GuidNamePropertyDefinition SharingSendAsVerificationEmailState = InternalSchema.CreateGuidNameProperty("SharingSendAsVerificationEmailState", typeof(int), WellKnownPropertySet.Sharing, "SharingSendAsVerificationEmailState");

		public static readonly GuidNamePropertyDefinition SharingSendAsVerificationMessageId = InternalSchema.CreateGuidNameProperty("SharingSendAsVerificationMessageId", typeof(string), WellKnownPropertySet.Sharing, "SharingSendAsVerificationMessageId");

		public static readonly GuidNamePropertyDefinition SharingSendAsVerificationTimestamp = InternalSchema.CreateGuidNameProperty("SharingSendAsVerificationTimestamp", typeof(ExDateTime), WellKnownPropertySet.Sharing, "SharingSendAsVerificationTimestamp");

		public static readonly GuidNamePropertyDefinition SharingSubscriptionEvents = InternalSchema.CreateGuidNameProperty("SharingSubscriptionEvents", typeof(int), WellKnownPropertySet.Sharing, "SharingSubscriptionEvents");

		public static readonly GuidNamePropertyDefinition SharingSubscriptionItemsSynced = InternalSchema.CreateGuidNameProperty("SharingSubscriptionItemsSynced", typeof(long), WellKnownPropertySet.Sharing, "SharingSubscriptionItemsSynced");

		public static readonly GuidNamePropertyDefinition SharingSubscriptionItemsSkipped = InternalSchema.CreateGuidNameProperty("SharingSubscriptionItemsSkipped", typeof(long), WellKnownPropertySet.Sharing, "SharingSubscriptionItemsSkipped");

		public static readonly GuidNamePropertyDefinition SharingSubscriptionTotalItemsInSourceMailbox = InternalSchema.CreateGuidNameProperty("SharingSubscriptionTotalItemsInSourceMailbox", typeof(long), WellKnownPropertySet.Sharing, "SharingSubscriptionTotalItemsInSourceMailbox");

		public static readonly GuidNamePropertyDefinition SharingSubscriptionTotalSizeOfSourceMailbox = InternalSchema.CreateGuidNameProperty("SharingSubscriptionTotalSizeOfSourceMailbox", typeof(long), WellKnownPropertySet.Sharing, "SharingSubscriptionTotalSizeOfSourceMailbox");

		public static readonly GuidNamePropertyDefinition SharingImapPathPrefix = InternalSchema.CreateGuidNameProperty("SharingImapPathPrefix", typeof(string), WellKnownPropertySet.Sharing, "SharingImapPathPrefix");

		public static readonly GuidNamePropertyDefinition SharingRemoteUserDomain = InternalSchema.CreateGuidNameProperty("SharingRemoteUserDomain", typeof(string), WellKnownPropertySet.Sharing, "SharingRemoteUserDomain");

		public static readonly GuidNamePropertyDefinition SharingSubscriptionExclusionFolders = InternalSchema.CreateGuidNameProperty("SharingSubscriptionExclusionFolders", typeof(string), WellKnownPropertySet.Sharing, "SharingSubscriptionExclusionFolders");

		public static readonly GuidNamePropertyDefinition SharingLastSyncNowRequest = InternalSchema.CreateGuidNameProperty("SharingLastSyncNowRequest", typeof(ExDateTime), WellKnownPropertySet.Sharing, "SharingLastSyncNowRequest");

		public static readonly PropertyTagPropertyDefinition RssServerLockStartTime = PropertyTagPropertyDefinition.InternalCreate("RssServerLockStartTime", (PropTag)1610612739U);

		public static readonly PropertyTagPropertyDefinition RssServerLockTimeout = PropertyTagPropertyDefinition.InternalCreate("RssServerLockTimeout", (PropTag)1610678275U);

		public static readonly PropertyTagPropertyDefinition RssServerLockClientName = PropertyTagPropertyDefinition.InternalCreate("RssServerLockClientName", (PropTag)1610743839U);

		public static readonly GuidNamePropertyDefinition SharingEncryptedAccessToken = InternalSchema.CreateGuidNameProperty("SharingEncryptedAccessToken", typeof(string), WellKnownPropertySet.Sharing, "SharingEncryptedAccessToken");

		public static readonly GuidNamePropertyDefinition SharingConnectState = InternalSchema.CreateGuidNameProperty("SharingConnectState", typeof(int), WellKnownPropertySet.Sharing, "SharingConnectState");

		public static readonly GuidNamePropertyDefinition SharingAppId = InternalSchema.CreateGuidNameProperty("SharingAppId", typeof(string), WellKnownPropertySet.Sharing, "SharingAppId");

		public static readonly GuidNamePropertyDefinition SharingUserId = InternalSchema.CreateGuidNameProperty("SharingUserId", typeof(string), WellKnownPropertySet.Sharing, "SharingUserId");

		public static readonly GuidNamePropertyDefinition SharingEncryptedAccessTokenSecret = InternalSchema.CreateGuidNameProperty("SharingEncryptedAccessTokenSecret", typeof(string), WellKnownPropertySet.Sharing, "SharingEncryptedAccessTokenSecret");

		public static readonly GuidNamePropertyDefinition SharingInitialSyncInRecoveryMode = InternalSchema.CreateGuidNameProperty("SharingInitialSyncInRecoveryMode", typeof(bool), WellKnownPropertySet.Sharing, "SharingInitialSyncInRecoveryMode");

		public static readonly GuidNamePropertyDefinition CloudId = InternalSchema.CreateGuidNameProperty("CloudId", typeof(string), WellKnownPropertySet.Messaging, "CloudId");

		public static readonly GuidNamePropertyDefinition CloudVersion = InternalSchema.CreateGuidNameProperty("CloudVersion", typeof(string), WellKnownPropertySet.Messaging, "CloudVersion");

		public static readonly GuidNamePropertyDefinition AggregationSyncProgress = InternalSchema.CreateGuidNameProperty("AggregationSyncProgress", typeof(int), WellKnownPropertySet.Messaging, "AggregationSyncProgress");

		public static readonly GuidIdPropertyDefinition MigrationVersion = InternalSchema.CreateGuidIdProperty("MigrationVersion", typeof(long), WellKnownPropertySet.Sharing, 35600);

		public static readonly GuidIdPropertyDefinition MigrationJobId = InternalSchema.CreateGuidIdProperty("MigrationJobId", typeof(Guid), WellKnownPropertySet.Sharing, 35601);

		public static readonly GuidIdPropertyDefinition MigrationJobName = InternalSchema.CreateGuidIdProperty("MigrationJobName", typeof(string), WellKnownPropertySet.Sharing, 35602);

		public static readonly GuidIdPropertyDefinition MigrationJobSubmittedBy = InternalSchema.CreateGuidIdProperty("MigrationJobSubmittedBy", typeof(string), WellKnownPropertySet.Sharing, 35603);

		public static readonly GuidIdPropertyDefinition MigrationJobItemStatus = InternalSchema.CreateGuidIdProperty("MigrationJobItemStatus", typeof(int), WellKnownPropertySet.Sharing, 35604);

		public static readonly GuidIdPropertyDefinition MigrationJobTotalRowCount = InternalSchema.CreateGuidIdProperty("MigrationJobTotalItemCount", typeof(int), WellKnownPropertySet.Sharing, 35605);

		public static readonly GuidIdPropertyDefinition MigrationJobExcludedFolders = InternalSchema.CreateGuidIdProperty("MigrationJobExcludedFolders", typeof(string), WellKnownPropertySet.Sharing, 35606);

		public static readonly GuidIdPropertyDefinition MigrationJobNotificationEmails = InternalSchema.CreateGuidIdProperty("MigrationJobNotificationEmails", typeof(string), WellKnownPropertySet.Sharing, 35607);

		public static readonly GuidIdPropertyDefinition MigrationJobStartTime = InternalSchema.CreateGuidIdProperty("MigrationJobStartTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35609);

		public static readonly GuidIdPropertyDefinition MigrationJobUserTimeZone = InternalSchema.CreateGuidIdProperty("MigrationJobUserTimeZone", typeof(string), WellKnownPropertySet.Sharing, 35610);

		public static readonly GuidIdPropertyDefinition MigrationJobCancelledFlag = InternalSchema.CreateGuidIdProperty("MigrationJobCancelledFlag", typeof(bool), WellKnownPropertySet.Sharing, 35611);

		public static readonly GuidIdPropertyDefinition MigrationJobItemIdentifier = InternalSchema.CreateGuidIdProperty("MigrationJobItemEmailAddress", typeof(string), WellKnownPropertySet.Sharing, 35612);

		public static readonly GuidIdPropertyDefinition MigrationJobItemEncryptedIncomingPassword = InternalSchema.CreateGuidIdProperty("MigrationJobItemEncryptedIncomingPassword", typeof(string), WellKnownPropertySet.Sharing, 35613);

		public static readonly GuidIdPropertyDefinition MigrationJobItemIncomingUsername = InternalSchema.CreateGuidIdProperty("MigrationJobItemIncomingUsername", typeof(string), WellKnownPropertySet.Sharing, 35614);

		public static readonly GuidIdPropertyDefinition MigrationJobItemSubscriptionMessageId = InternalSchema.CreateGuidIdProperty("MigrationJobItemSubscriptionMessageId", typeof(byte[]), WellKnownPropertySet.Sharing, 35615);

		public static readonly GuidIdPropertyDefinition MigrationJobItemMailboxServer = InternalSchema.CreateGuidIdProperty("MigrationJobItemMailboxServer", typeof(string), WellKnownPropertySet.Sharing, 35616);

		public static readonly GuidIdPropertyDefinition MigrationJobItemMailboxId = InternalSchema.CreateGuidIdProperty("MigrationJobItemMailboxId", typeof(Guid), WellKnownPropertySet.Sharing, 35617);

		public static readonly GuidIdPropertyDefinition MigrationJobItemStateLastUpdated = InternalSchema.CreateGuidIdProperty("MigrationJobItemStateLastUpdated", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35618);

		public static readonly GuidIdPropertyDefinition MigrationJobItemMailboxLegacyDN = InternalSchema.CreateGuidIdProperty("MigrationJobItemMailboxLegacyDN", typeof(string), WellKnownPropertySet.Sharing, 35619);

		public static readonly GuidIdPropertyDefinition MigrationJobRemoteServerHostName = InternalSchema.CreateGuidIdProperty("MigrationJobRemoteServerHostName", typeof(string), WellKnownPropertySet.Sharing, 35620);

		public static readonly GuidIdPropertyDefinition MigrationJobRemoteServerPortNumber = InternalSchema.CreateGuidIdProperty("MigrationJobRemoteServerPortNumber", typeof(int), WellKnownPropertySet.Sharing, 35621);

		public static readonly GuidIdPropertyDefinition MigrationJobRemoteServerAuth = InternalSchema.CreateGuidIdProperty("MigrationJobRemoteServerAuth", typeof(int), WellKnownPropertySet.Sharing, 35622);

		public static readonly GuidIdPropertyDefinition MigrationJobRemoteServerSecurity = InternalSchema.CreateGuidIdProperty("MigrationJobRemoteServerSecurity", typeof(int), WellKnownPropertySet.Sharing, 35623);

		public static readonly GuidIdPropertyDefinition MigrationJobMaxConcurrentMigrations = InternalSchema.CreateGuidIdProperty("MigrationJobMaxConcurrentMigrations", typeof(int), WellKnownPropertySet.Sharing, 35624);

		public static readonly GuidIdPropertyDefinition MigrationCacheEntryMailboxLegacyDN = InternalSchema.CreateGuidIdProperty("MigrationCacheEntryMailboxLegacyDN", typeof(string), WellKnownPropertySet.Sharing, 35625);

		public static readonly GuidIdPropertyDefinition MigrationCacheEntryTenantPartitionHint = InternalSchema.CreateGuidIdProperty("MigrationCacheEntryTenantPartitionHint", typeof(byte[]), WellKnownPropertySet.Sharing, 35772);

		public static readonly GuidIdPropertyDefinition MigrationJobItemRowIndex = InternalSchema.CreateGuidIdProperty("MigrationJobItemRowIndex", typeof(int), WellKnownPropertySet.Sharing, 35627);

		public static readonly GuidIdPropertyDefinition MigrationJobItemLocalizedError = InternalSchema.CreateGuidIdProperty("MigrationJobItemLocalizedError", typeof(string), WellKnownPropertySet.Sharing, 35628);

		public static readonly GuidIdPropertyDefinition MigrationJobItemSubscriptionCreated = InternalSchema.CreateGuidIdProperty("MigrationJobItemSubscriptionCreated", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35629);

		public static readonly GuidIdPropertyDefinition MigrationJobItemSubscriptionLastChecked = InternalSchema.CreateGuidIdProperty("MigrationJobItemSubscriptionStateLastChecked", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35630);

		public static readonly GuidIdPropertyDefinition MigrationJobInternalError = InternalSchema.CreateGuidIdProperty("MigrationJobInternalError", typeof(string), WellKnownPropertySet.Sharing, 35631);

		public static readonly GuidIdPropertyDefinition MigrationCacheEntryLastUpdated = InternalSchema.CreateGuidIdProperty("MigrationCacheEntryLastUpdated", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35632);

		public static readonly GuidIdPropertyDefinition MigrationJobOriginalCreationTime = InternalSchema.CreateGuidIdProperty("MigrationJobOriginalCreationTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35633);

		public static readonly GuidIdPropertyDefinition MigrationJobAdminCulture = InternalSchema.CreateGuidIdProperty("MigrationJobAdminCulture", typeof(string), WellKnownPropertySet.Sharing, 35634);

		public static readonly GuidIdPropertyDefinition MigrationUserRootFolder = InternalSchema.CreateGuidIdProperty("MigrationUserRootFolder", typeof(string), WellKnownPropertySet.Sharing, 35635);

		public static readonly GuidIdPropertyDefinition MigrationType = InternalSchema.CreateGuidIdProperty("MigrationType", typeof(int), WellKnownPropertySet.Sharing, 35636);

		public static readonly GuidIdPropertyDefinition MigrationJobWindowsLiveNetId = InternalSchema.CreateGuidIdProperty("MigrationJobWindowsLiveNetId", typeof(string), WellKnownPropertySet.Sharing, 35637);

		public static readonly GuidIdPropertyDefinition MigrationJobCursorPosition = InternalSchema.CreateGuidIdProperty("MigrationJobCursorPosition", typeof(string), WellKnownPropertySet.Sharing, 35638);

		public static readonly GuidIdPropertyDefinition MigrationJobItemWLSASigned = InternalSchema.CreateGuidIdProperty("MigrationJobItemWLSASigned", typeof(bool), WellKnownPropertySet.Sharing, 35639);

		public static readonly GuidIdPropertyDefinition MigrationHotmailSubscriptionStatus = InternalSchema.CreateGuidIdProperty("MigrationHotmailSubscriptionStatus", typeof(int), WellKnownPropertySet.Sharing, 35641);

		public static readonly GuidIdPropertyDefinition MigrationHotmailSubscriptionMessageId = InternalSchema.CreateGuidIdProperty("MigrationHotmailSubscriptionMessageId", typeof(byte[]), WellKnownPropertySet.Sharing, 35642);

		public static readonly GuidIdPropertyDefinition MigrationJobHasAdminPrivilege = InternalSchema.CreateGuidIdProperty("MigrationJobHasAdminPrivilege", typeof(bool), WellKnownPropertySet.Sharing, 35644);

		public static readonly GuidIdPropertyDefinition MigrationJobHasAutodiscovery = InternalSchema.CreateGuidIdProperty("MigrationJobHasAutodiscovery", typeof(bool), WellKnownPropertySet.Sharing, 35645);

		public static readonly GuidIdPropertyDefinition MigrationJobRemoteAutodiscoverUrl = InternalSchema.CreateGuidIdProperty("MigrationJobRemoteAutodiscoverUrl", typeof(string), WellKnownPropertySet.Sharing, 35646);

		public static readonly GuidIdPropertyDefinition MigrationJobEmailAddress = InternalSchema.CreateGuidIdProperty("MigrationJobEmailAddress", typeof(string), WellKnownPropertySet.Sharing, 35647);

		public static readonly GuidIdPropertyDefinition MigrationJobProxyServerHostName = InternalSchema.CreateGuidIdProperty("MigrationJobProxyServerHostName", typeof(string), WellKnownPropertySet.Sharing, 35648);

		public static readonly GuidIdPropertyDefinition MigrationJobExchangeRemoteServerHostName = InternalSchema.CreateGuidIdProperty("MigrationJobExchangeRemoteServerHostName", typeof(string), WellKnownPropertySet.Sharing, 35649);

		public static readonly GuidIdPropertyDefinition MigrationJobRemoteNSPIServerHostName = InternalSchema.CreateGuidIdProperty("MigrationJobRemoteNSPIServerHostName", typeof(string), WellKnownPropertySet.Sharing, 35650);

		public static readonly GuidIdPropertyDefinition MigrationJobRemoteDomain = InternalSchema.CreateGuidIdProperty("MigrationJobRemoteDomain", typeof(string), WellKnownPropertySet.Sharing, 35651);

		public static readonly GuidIdPropertyDefinition MigrationJobRemoteServerVersion = InternalSchema.CreateGuidIdProperty("MigrationJobRemoteServerVersion", typeof(string), WellKnownPropertySet.Sharing, 35652);

		public static readonly GuidIdPropertyDefinition MigrationJobItemRemoteMailboxLegacyDN = InternalSchema.CreateGuidIdProperty("MigrationJobItemRemoteMailboxLegacyDN", typeof(string), WellKnownPropertySet.Sharing, 35653);

		public static readonly GuidIdPropertyDefinition MigrationJobItemRemoteServerLegacyDN = InternalSchema.CreateGuidIdProperty("MigrationJobItemRemoteServerLegacyDN", typeof(string), WellKnownPropertySet.Sharing, 35654);

		public static readonly GuidIdPropertyDefinition MigrationJobItemProxyServerHostName = InternalSchema.CreateGuidIdProperty("MigrationJobItemProxyServerHostName", typeof(string), WellKnownPropertySet.Sharing, 35655);

		public static readonly GuidIdPropertyDefinition MigrationJobItemRemoteAutodiscoverUrl = InternalSchema.CreateGuidIdProperty("MigrationJobItemRemoteAutodiscoverUrl", typeof(string), WellKnownPropertySet.Sharing, 35656);

		public static readonly GuidIdPropertyDefinition MigrationJobExchangeRemoteServerAuth = InternalSchema.CreateGuidIdProperty("MigrationJobExchangeRemoteServerAuth", typeof(int), WellKnownPropertySet.Sharing, 35657);

		public static readonly GuidIdPropertyDefinition MigrationJobItemHotmailLocalizedError = InternalSchema.CreateGuidIdProperty("MigrationJobItemHotmailLocalizedError", typeof(string), WellKnownPropertySet.Sharing, 35659);

		public static readonly GuidIdPropertyDefinition MigrationJobItemExchangeRemoteServerHostName = InternalSchema.CreateGuidIdProperty("MigrationJobItemExchangeRemoteServerHostName", typeof(string), WellKnownPropertySet.Sharing, 35660);

		public static readonly GuidIdPropertyDefinition MigrationJobOwnerId = InternalSchema.CreateGuidIdProperty("MigrationJobOwnerId", typeof(byte[]), WellKnownPropertySet.Sharing, 35661);

		public static readonly GuidIdPropertyDefinition MigrationJobSuppressErrors = InternalSchema.CreateGuidIdProperty("MigrationJobSuppressErrors", typeof(bool), WellKnownPropertySet.Sharing, 35662);

		public static readonly GuidIdPropertyDefinition MigrationJobFinalizeTime = InternalSchema.CreateGuidIdProperty("MigrationJobFinalizeTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35663);

		public static readonly GuidIdPropertyDefinition MigrationSubmittedByUserAdminType = InternalSchema.CreateGuidIdProperty("MigrationSubmittedByUserAdminType", typeof(int), WellKnownPropertySet.Sharing, 35664);

		public static readonly GuidIdPropertyDefinition MigrationJobItemExchangeRecipientIndex = InternalSchema.CreateGuidIdProperty("MigrationJobItemExchangeRecipientIndex", typeof(int), WellKnownPropertySet.Sharing, 35665);

		public static readonly GuidIdPropertyDefinition MigrationJobItemExchangeRecipientProperties = InternalSchema.CreateGuidIdProperty("MigrationJobItemExchangeRecipientProperties", typeof(string), WellKnownPropertySet.Sharing, 35667);

		public static readonly GuidIdPropertyDefinition MigrationJobItemMRSId = InternalSchema.CreateGuidIdProperty("MigrationJobItemMRSId", typeof(Guid), WellKnownPropertySet.Sharing, 35668);

		public static readonly GuidIdPropertyDefinition MigrationJobItemExchangeMsExchHomeServerName = InternalSchema.CreateGuidIdProperty("MigrationJobItemExchangeMsExchHomeServerName", typeof(string), WellKnownPropertySet.Sharing, 35669);

		public static readonly GuidIdPropertyDefinition MigrationJobItemProvisioningData = InternalSchema.CreateGuidIdProperty("MigrationJobItemProvisioningData", typeof(string), WellKnownPropertySet.Sharing, 35670);

		public static readonly GuidIdPropertyDefinition MigrationJobStatisticsEnabled = InternalSchema.CreateGuidIdProperty("MigrationJobStatisticsEnabled", typeof(bool), WellKnownPropertySet.Sharing, 35671);

		public static readonly GuidIdPropertyDefinition MigrationJobItemItemsSynced = InternalSchema.CreateGuidIdProperty("MigrationJobItemItemsSynced", typeof(long), WellKnownPropertySet.Sharing, 35672);

		public static readonly GuidIdPropertyDefinition MigrationJobItemItemsSkipped = InternalSchema.CreateGuidIdProperty("MigrationJobItemItemsSkipped", typeof(long), WellKnownPropertySet.Sharing, 35673);

		public static readonly GuidIdPropertyDefinition MigrationJobItemLastProvisionedMemberIndex = InternalSchema.CreateGuidIdProperty("MigrationJobItemLastProvisionedMemberIndex", typeof(int), WellKnownPropertySet.Sharing, 35674);

		public static readonly GuidIdPropertyDefinition MigrationJobItemMailboxDatabaseId = InternalSchema.CreateGuidIdProperty("MigrationJobItemMailboxDatabaseId", typeof(Guid), WellKnownPropertySet.Sharing, 35675);

		public static readonly GuidIdPropertyDefinition MigrationReportName = InternalSchema.CreateGuidIdProperty("MigrationReportName", typeof(string), WellKnownPropertySet.Sharing, 35676);

		public static readonly GuidIdPropertyDefinition MigrationJobItemOwnerId = InternalSchema.CreateGuidIdProperty("MigrationJobItemOwnerId", typeof(byte[]), WellKnownPropertySet.Sharing, 35677);

		public static readonly GuidIdPropertyDefinition MigrationJobTotalItemCountLegacy = InternalSchema.CreateGuidIdProperty("MigrationJobTotalItemCountR5", typeof(string), WellKnownPropertySet.Sharing, 35678);

		public static readonly GuidIdPropertyDefinition MigrationJobItemADObjectExists = InternalSchema.CreateGuidIdProperty("MigrationJobItemADObjectExists", typeof(bool), WellKnownPropertySet.Sharing, 35679);

		public static readonly GuidIdPropertyDefinition MigrationJobCancellationReason = InternalSchema.CreateGuidIdProperty("MigrationJobCancellationReason", typeof(int), WellKnownPropertySet.Sharing, 35680);

		public static readonly GuidIdPropertyDefinition MigrationJobItemExchangeMbxEncryptedPassword = InternalSchema.CreateGuidIdProperty("MigrationJobItemExchangeMbxEncryptedPassword", typeof(string), WellKnownPropertySet.Sharing, 35681);

		public static readonly GuidIdPropertyDefinition MigrationJobItemRecipientType = InternalSchema.CreateGuidIdProperty("MigrationJobItemRecipientType", typeof(int), WellKnownPropertySet.Sharing, 35682);

		public static readonly GuidIdPropertyDefinition MigrationJobDelegatedAdminOwnerId = InternalSchema.CreateGuidIdProperty("MigrationJobDelegatedAdminOwnerId", typeof(string), WellKnownPropertySet.Sharing, 35683);

		public static readonly GuidIdPropertyDefinition MigrationJobCheckWLSA = InternalSchema.CreateGuidIdProperty("MigrationJobCheckWLSA", typeof(bool), WellKnownPropertySet.Sharing, 35684);

		public static readonly GuidIdPropertyDefinition MigrationJobItemHotmailPodID = InternalSchema.CreateGuidIdProperty("MigrationJobItemHotmailPodID", typeof(int), WellKnownPropertySet.Sharing, 35685);

		public static readonly GuidIdPropertyDefinition MigrationJobItemTransientErrorCount = InternalSchema.CreateGuidIdProperty("MigrationJobItemTransientErrorCount", typeof(int), WellKnownPropertySet.Sharing, 35686);

		public static readonly GuidIdPropertyDefinition MigrationJobItemPreviousStatus = InternalSchema.CreateGuidIdProperty("MigrationJobItemPreviousStatus", typeof(int), WellKnownPropertySet.Sharing, 35687);

		public static readonly GuidIdPropertyDefinition MigrationJobItemSubscriptionId = InternalSchema.CreateGuidIdProperty("MigrationJobItemSubscriptionId", typeof(Guid), WellKnownPropertySet.Sharing, 35688);

		public static readonly GuidIdPropertyDefinition MigrationHotmailSubscriptionId = InternalSchema.CreateGuidIdProperty("MigrationHotmailSubscriptionId", typeof(Guid), WellKnownPropertySet.Sharing, 35689);

		public static readonly GuidIdPropertyDefinition MigrationJobInternalErrorTime = InternalSchema.CreateGuidIdProperty("JobInternalErrorTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35691);

		public static readonly GuidIdPropertyDefinition MigrationJobPoisonCount = InternalSchema.CreateGuidIdProperty("MigrationJobPoisonCount", typeof(int), WellKnownPropertySet.Sharing, 35692);

		public static readonly GuidIdPropertyDefinition MigrationReportType = InternalSchema.CreateGuidIdProperty("MigrationReportType", typeof(int), WellKnownPropertySet.Sharing, 35693);

		public static readonly GuidIdPropertyDefinition MigrationSuccessReportUrl = InternalSchema.CreateGuidIdProperty("MigrationSuccessReportUrl", typeof(string), WellKnownPropertySet.Sharing, 35694);

		public static readonly GuidIdPropertyDefinition MigrationErrorReportUrl = InternalSchema.CreateGuidIdProperty("MigrationErrorReportUrl", typeof(string), WellKnownPropertySet.Sharing, 35695);

		public static readonly GuidIdPropertyDefinition MigrationJobTargetDomainName = InternalSchema.CreateGuidIdProperty("MigrationJobTargetDomainName", typeof(string), WellKnownPropertySet.Sharing, 35696);

		public static readonly GuidIdPropertyDefinition MigrationJobIsStaged = InternalSchema.CreateGuidIdProperty("MigrationJobIsStaged", typeof(bool), WellKnownPropertySet.Sharing, 35697);

		public static readonly GuidIdPropertyDefinition MigrationJobItemForceChangePassword = InternalSchema.CreateGuidIdProperty("MigrationJobItemForceChangePassword", typeof(bool), WellKnownPropertySet.Sharing, 35698);

		public static readonly GuidIdPropertyDefinition MigrationJobItemLocalizedErrorID = InternalSchema.CreateGuidIdProperty("MigrationJobItemLocalizedErrorID", typeof(string), WellKnownPropertySet.Sharing, 35699);

		public static readonly GuidIdPropertyDefinition MigrationJobItemHotmailLocalizedErrorID = InternalSchema.CreateGuidIdProperty("MigrationJobItemHotmailLocalizedErrorID", typeof(string), WellKnownPropertySet.Sharing, 35701);

		public static readonly GuidIdPropertyDefinition MigrationJobItemGroupMemberSkipped = InternalSchema.CreateGuidIdProperty("MigrationJobItemGroupMemberSkipped", typeof(int), WellKnownPropertySet.Sharing, 35702);

		public static readonly GuidIdPropertyDefinition MigrationJobItemGroupMemberProvisioned = InternalSchema.CreateGuidIdProperty("MigrationJobItemGroupMemberProvisioned", typeof(int), WellKnownPropertySet.Sharing, 35703);

		public static readonly GuidIdPropertyDefinition MigrationJobItemGroupMemberProvisioningState = InternalSchema.CreateGuidIdProperty("MigrationJobItemGroupMemberProvisioningState", typeof(int), WellKnownPropertySet.Sharing, 35704);

		public static readonly GuidIdPropertyDefinition MigrationJobItemStatusHistory = InternalSchema.CreateGuidIdProperty("MigrationJobItemStatusHistory", typeof(string), WellKnownPropertySet.Sharing, 35705);

		public static readonly GuidIdPropertyDefinition MigrationJobItemIDSIdentityFlags = InternalSchema.CreateGuidIdProperty("MigrationJobItemIDSIdentityFlags", typeof(int), WellKnownPropertySet.Sharing, 35706);

		public static readonly GuidIdPropertyDefinition MigrationJobItemLocalizedMessage = InternalSchema.CreateGuidIdProperty("MigrationJobItemLocalizedMessage", typeof(string), WellKnownPropertySet.Sharing, 35707);

		public static readonly GuidIdPropertyDefinition MigrationJobItemLocalizedMessageID = InternalSchema.CreateGuidIdProperty("MigrationJobItemLocalizedMessageID", typeof(string), WellKnownPropertySet.Sharing, 35708);

		public static readonly GuidIdPropertyDefinition MigrationLastSuccessfulSyncTime = InternalSchema.CreateGuidIdProperty("MigrationLastSuccessfulSyncTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35709);

		public static readonly GuidIdPropertyDefinition MigrationPersistableDictionary = InternalSchema.CreateGuidIdProperty("MigrationPersistableDictionary", typeof(string), WellKnownPropertySet.Sharing, 35710);

		public static readonly GuidIdPropertyDefinition MigrationReportSets = InternalSchema.CreateGuidIdProperty("MigrationReportSets", typeof(string), WellKnownPropertySet.Sharing, 35711);

		public static readonly GuidIdPropertyDefinition MigrationDisableTime = InternalSchema.CreateGuidIdProperty("MigrationDisableTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35712);

		public static readonly GuidIdPropertyDefinition MigrationProvisionedTime = InternalSchema.CreateGuidIdProperty("MigrationProvisionedTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35713);

		public static readonly GuidIdPropertyDefinition MigrationSameStatusCount = InternalSchema.CreateGuidIdProperty("MigrationSameStatusCount", typeof(int), WellKnownPropertySet.Sharing, 35715);

		public static readonly GuidIdPropertyDefinition MigrationTransitionTime = InternalSchema.CreateGuidIdProperty("MigrationTransitionTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35716);

		public static readonly GuidIdPropertyDefinition MigrationDeltaSyncShouldSync = InternalSchema.CreateGuidIdProperty("MigrationDeltaSyncShouldSync", typeof(bool), WellKnownPropertySet.Sharing, 35718);

		public static readonly GuidIdPropertyDefinition MigrationJobItemId = InternalSchema.CreateGuidIdProperty("MigrationJobItemId", typeof(Guid), WellKnownPropertySet.Sharing, 35719);

		public static readonly GuidIdPropertyDefinition MigrationEndpointType = InternalSchema.CreateGuidIdProperty("MigrationEndpointType", typeof(int), WellKnownPropertySet.Sharing, 35751);

		public static readonly GuidIdPropertyDefinition MigrationEndpointName = InternalSchema.CreateGuidIdProperty("MigrationEndpointName", typeof(string), WellKnownPropertySet.Sharing, 35752);

		public static readonly GuidIdPropertyDefinition MigrationEndpointGuid = InternalSchema.CreateGuidIdProperty("MigrationEndpointGuid", typeof(Guid), WellKnownPropertySet.Sharing, 35753);

		public static readonly GuidIdPropertyDefinition MigrationEndpointRemoteHostName = InternalSchema.CreateGuidIdProperty("MigrationEndpointRemoteHostName", typeof(string), WellKnownPropertySet.Sharing, 35754);

		public static readonly GuidIdPropertyDefinition MigrationEndpointExchangeServer = InternalSchema.CreateGuidIdProperty("MigrationEndpointExchangeServer", typeof(string), WellKnownPropertySet.Sharing, 35755);

		public static readonly GuidIdPropertyDefinition MigrationJobSourceEndpoint = InternalSchema.CreateGuidIdProperty("MigrationJobSourceEndpoint", typeof(Guid), WellKnownPropertySet.Sharing, 35756);

		public static readonly GuidIdPropertyDefinition MigrationJobTargetEndpoint = InternalSchema.CreateGuidIdProperty("MigrationJobTargetEndpoint", typeof(Guid), WellKnownPropertySet.Sharing, 35757);

		public static readonly GuidIdPropertyDefinition MigrationRuntimeJobData = InternalSchema.CreateGuidIdProperty("MigrationRuntimeJobData", typeof(string), WellKnownPropertySet.Sharing, 35758);

		public static readonly GuidIdPropertyDefinition MigrationJobDirection = InternalSchema.CreateGuidIdProperty("MigrationJobDirection", typeof(int), WellKnownPropertySet.Sharing, 35759);

		public static readonly GuidIdPropertyDefinition MigrationJobTargetDatabase = InternalSchema.CreateGuidIdProperty("MigrationJobTargetDatabase", typeof(string[]), WellKnownPropertySet.Sharing, 35760);

		public static readonly GuidIdPropertyDefinition MigrationJobTargetArchiveDatabase = InternalSchema.CreateGuidIdProperty("MigrationJobTargetArchiveDatabase", typeof(string[]), WellKnownPropertySet.Sharing, 35761);

		public static readonly GuidIdPropertyDefinition MigrationJobBadItemLimit = InternalSchema.CreateGuidIdProperty("MigrationJobBadItemLimit", typeof(string), WellKnownPropertySet.Sharing, 35762);

		public static readonly GuidIdPropertyDefinition MigrationJobLargeItemLimit = InternalSchema.CreateGuidIdProperty("MigrationJobLargeItemLimit", typeof(string), WellKnownPropertySet.Sharing, 35763);

		public static readonly GuidIdPropertyDefinition MigrationJobPrimaryOnly = InternalSchema.CreateGuidIdProperty("MigrationJobPrimaryOnly", typeof(bool), WellKnownPropertySet.Sharing, 35764);

		public static readonly GuidIdPropertyDefinition MigrationJobArchiveOnly = InternalSchema.CreateGuidIdProperty("MigrationJobArchiveOnly", typeof(bool), WellKnownPropertySet.Sharing, 35765);

		public static readonly GuidIdPropertyDefinition MigrationSlotMaximumInitialSeedings = InternalSchema.CreateGuidIdProperty("MigrationSlotMaximumInitialSeedings", typeof(string), WellKnownPropertySet.Sharing, 35766);

		public static readonly GuidIdPropertyDefinition MigrationSlotMaximumIncrementalSeedings = InternalSchema.CreateGuidIdProperty("MigrationSlotMaximumIncrementalSeedings", typeof(string), WellKnownPropertySet.Sharing, 35767);

		public static readonly GuidIdPropertyDefinition MigrationJobTargetDeliveryDomain = InternalSchema.CreateGuidIdProperty("MigrationJobTargetDeliveryDomain", typeof(string), WellKnownPropertySet.Sharing, 35768);

		public static readonly GuidIdPropertyDefinition MigrationJobSkipSteps = InternalSchema.CreateGuidIdProperty("MigrationJobSkipSteps", typeof(int), WellKnownPropertySet.Sharing, 35769);

		public static readonly GuidIdPropertyDefinition MigrationJobItemSlotType = InternalSchema.CreateGuidIdProperty("MigrationJobItemSlotType", typeof(int), WellKnownPropertySet.Sharing, 35770);

		public static readonly GuidIdPropertyDefinition MigrationJobItemSlotProviderId = InternalSchema.CreateGuidIdProperty("MigrationJobItemSlotProviderId", typeof(Guid), WellKnownPropertySet.Sharing, 35771);

		public static readonly GuidIdPropertyDefinition MigrationJobCountCache = InternalSchema.CreateGuidIdProperty("MigrationJobCountCache", typeof(string), WellKnownPropertySet.Sharing, 35772);

		public static readonly GuidIdPropertyDefinition MigrationJobCountCacheFullScanTime = InternalSchema.CreateGuidIdProperty("MigrationJobCountCacheFullScanTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35773);

		public static readonly GuidIdPropertyDefinition MigrationJobLastRestartTime = InternalSchema.CreateGuidIdProperty("MigrationJobLastRestartTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35774);

		public static readonly GuidIdPropertyDefinition MigrationFailureRecord = InternalSchema.CreateGuidIdProperty("MigrationFailureRecord", typeof(string), WellKnownPropertySet.Sharing, 35775);

		public static readonly GuidIdPropertyDefinition MigrationJobIsRunning = InternalSchema.CreateGuidIdProperty("MigrationJobIsRunning", typeof(bool), WellKnownPropertySet.Sharing, 35776);

		public static readonly GuidIdPropertyDefinition MigrationSubscriptionSettingsLastModifiedTime = InternalSchema.CreateGuidIdProperty("MigrationSubscriptionSettingsLastModifiedTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35777);

		public static readonly GuidIdPropertyDefinition MigrationJobItemSubscriptionSettingsLastUpdatedTime = InternalSchema.CreateGuidIdProperty("MigrationJobItemSubscriptionSettingsLastUpdatedTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35778);

		public static readonly GuidIdPropertyDefinition MigrationJobStartAfter = InternalSchema.CreateGuidIdProperty("MigrationJobStartAfter", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35779);

		public static readonly GuidIdPropertyDefinition MigrationJobCompleteAfter = InternalSchema.CreateGuidIdProperty("MigrationJobCompleteAfter", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35780);

		public static readonly GuidIdPropertyDefinition MigrationNextProcessTime = InternalSchema.CreateGuidIdProperty("MigrationNextProcessTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35781);

		public static readonly GuidIdPropertyDefinition MigrationStatusDataFailureWatsonHash = InternalSchema.CreateGuidIdProperty("MigrationStatusDataFailureWatsonHash", typeof(string), WellKnownPropertySet.Sharing, 35782);

		public static readonly GuidIdPropertyDefinition MigrationState = InternalSchema.CreateGuidIdProperty("MigrationState", typeof(int), WellKnownPropertySet.Sharing, 35783);

		public static readonly GuidIdPropertyDefinition MigrationFlags = InternalSchema.CreateGuidIdProperty("MigrationFlags", typeof(int), WellKnownPropertySet.Sharing, 35784);

		public static readonly GuidIdPropertyDefinition MigrationStage = InternalSchema.CreateGuidIdProperty("MigrationStage", typeof(int), WellKnownPropertySet.Sharing, 35785);

		public static readonly GuidIdPropertyDefinition MigrationStep = InternalSchema.CreateGuidIdProperty("MigrationStep", typeof(int), WellKnownPropertySet.Sharing, 35786);

		public static readonly GuidIdPropertyDefinition MigrationWorkflow = InternalSchema.CreateGuidIdProperty("MigrationWorkflow", typeof(string), WellKnownPropertySet.Sharing, 35787);

		public static readonly GuidIdPropertyDefinition MigrationPSTFilePath = InternalSchema.CreateGuidIdProperty("MigrationPSTFilePath", typeof(string), WellKnownPropertySet.Sharing, 35788);

		public static readonly GuidIdPropertyDefinition MigrationSourceRootFolder = InternalSchema.CreateGuidIdProperty("MigrationSourceRootFolder", typeof(string), WellKnownPropertySet.Sharing, 35789);

		public static readonly GuidIdPropertyDefinition MigrationTargetRootFolder = InternalSchema.CreateGuidIdProperty("MigrationTargetRootFolder", typeof(string), WellKnownPropertySet.Sharing, 35790);

		public static readonly GuidIdPropertyDefinition MigrationJobItemLocalMailboxIdentifier = InternalSchema.CreateGuidIdProperty("MigrationJobItemLocalMailboxIdentifier", typeof(string), WellKnownPropertySet.Sharing, 35791);

		public static readonly GuidIdPropertyDefinition MigrationExchangeObjectId = InternalSchema.CreateGuidIdProperty("MigrationJobExchangeObjectGuid", typeof(Guid), WellKnownPropertySet.Sharing, 35792);

		public static readonly GuidIdPropertyDefinition MigrationJobItemSubscriptionQueuedTime = InternalSchema.CreateGuidIdProperty("MigrationJobItemSubscriptionQueuedTime", typeof(ExDateTime), WellKnownPropertySet.Sharing, 35793);

		public static readonly GuidIdPropertyDefinition MigrationJobSourcePublicFolderDatabase = InternalSchema.CreateGuidIdProperty("MigrationJobSourcePublicFolderDatabase", typeof(string), WellKnownPropertySet.Sharing, 35794);

		public static readonly GuidIdPropertyDefinition MigrationJobItemPuid = InternalSchema.CreateGuidIdProperty("MigrationJobItemPuid", typeof(long), WellKnownPropertySet.Sharing, 35795);

		public static readonly GuidIdPropertyDefinition MigrationJobItemFirstName = InternalSchema.CreateGuidIdProperty("MigrationJobItemFirstName", typeof(string), WellKnownPropertySet.Sharing, 35796);

		public static readonly GuidIdPropertyDefinition MigrationJobItemLastName = InternalSchema.CreateGuidIdProperty("MigrationJobItemLastName", typeof(string), WellKnownPropertySet.Sharing, 35797);

		public static readonly GuidIdPropertyDefinition MigrationJobItemTimeZone = InternalSchema.CreateGuidIdProperty("MigrationJobItemTimeZone", typeof(string), WellKnownPropertySet.Sharing, 35798);

		public static readonly GuidIdPropertyDefinition MigrationJobItemLocaleId = InternalSchema.CreateGuidIdProperty("MigrationJobItemLocaleId", typeof(int), WellKnownPropertySet.Sharing, 35799);

		public static readonly GuidIdPropertyDefinition MigrationJobItemAliases = InternalSchema.CreateGuidIdProperty("MigrationJobItemAliases", typeof(string), WellKnownPropertySet.Sharing, 35800);

		public static readonly GuidIdPropertyDefinition MigrationJobItemAccountSize = InternalSchema.CreateGuidIdProperty("MigrationJobItemAccountSize", typeof(long), WellKnownPropertySet.Sharing, 35801);

		public static readonly GuidIdPropertyDefinition MigrationJobLastFinalizationAttempt = InternalSchema.CreateGuidIdProperty("MigrationJobLastFinalizationAttempt", typeof(int), WellKnownPropertySet.Sharing, 35805);

		public static readonly GuidNamePropertyDefinition MonitoringEventInstanceId = InternalSchema.CreateGuidNameProperty("MonitoringEventInstanceId", typeof(int), WellKnownPropertySet.PublicStrings, "MonitoringEventInstanceId");

		public static readonly GuidNamePropertyDefinition MonitoringEventSource = InternalSchema.CreateGuidNameProperty("MonitoringEventSource", typeof(string), WellKnownPropertySet.PublicStrings, "MonitoringEventSource");

		public static readonly GuidNamePropertyDefinition MonitoringEventCategoryId = InternalSchema.CreateGuidNameProperty("MonitoringEventCategoryId", typeof(int), WellKnownPropertySet.PublicStrings, "MonitoringEventCategoryId");

		public static readonly GuidNamePropertyDefinition MonitoringEventTimeUtc = InternalSchema.CreateGuidNameProperty("MonitoringEventTimeUtc", typeof(ExDateTime), WellKnownPropertySet.PublicStrings, "MonitoringEventTimeUtc");

		public static readonly GuidNamePropertyDefinition MonitoringEventEntryType = InternalSchema.CreateGuidNameProperty("MonitoringEventEntryType", typeof(int), WellKnownPropertySet.PublicStrings, "MonitoringEventEntryType");

		public static readonly GuidNamePropertyDefinition MonitoringInsertionStrings = InternalSchema.CreateGuidNameProperty("MonitoringInsertionStrings", typeof(string[]), WellKnownPropertySet.PublicStrings, "MonitoringInsertionStrings");

		public static readonly GuidNamePropertyDefinition MonitoringUniqueId = InternalSchema.CreateGuidNameProperty("MonitoringUniqueId", typeof(byte[]), WellKnownPropertySet.PublicStrings, "MonitoringUniqueId");

		public static readonly GuidNamePropertyDefinition MonitoringNotificationEmailSent = InternalSchema.CreateGuidNameProperty("MonitoringNotificationEmailSent", typeof(bool), WellKnownPropertySet.PublicStrings, "MonitoringNotificationEmailSent");

		public static readonly GuidNamePropertyDefinition MonitoringCreationTimeUtc = InternalSchema.CreateGuidNameProperty("MonitoringCreationTimeUtc", typeof(ExDateTime), WellKnownPropertySet.PublicStrings, "MonitoringCreationTimeUtc");

		public static readonly GuidNamePropertyDefinition MonitoringCountOfNotificationsSentInPast24Hours = InternalSchema.CreateGuidNameProperty("MonitoringCountOfNotificationsSentInPast24Hours", typeof(long), WellKnownPropertySet.PublicStrings, "MonitoringCountOfNotificationsSentInPast24Hours");

		public static readonly GuidNamePropertyDefinition MonitoringNotificationRecipients = InternalSchema.CreateGuidNameProperty("MonitoringNotificationRecipients", typeof(string[]), WellKnownPropertySet.PublicStrings, "MonitoringNotificationRecipients");

		public static readonly GuidNamePropertyDefinition MonitoringHashCodeForDuplicateDetection = InternalSchema.CreateGuidNameProperty("MonitoringHashCodeForDuplicateDetection", typeof(long), WellKnownPropertySet.PublicStrings, "MonitoringHashCodeForDuplicateDetection");

		public static readonly GuidNamePropertyDefinition MonitoringNotificationMessageIds = InternalSchema.CreateGuidNameProperty("MonitoringNotificationMessageIds", typeof(string[]), WellKnownPropertySet.PublicStrings, "MonitoringNotificationMessageIds");

		public static readonly GuidNamePropertyDefinition MonitoringEventPeriodicKey = InternalSchema.CreateGuidNameProperty("MonitoringEventPeriodicKey", typeof(string), WellKnownPropertySet.PublicStrings, "MonitoringEventPeriodicKey");

		public static readonly StorePropertyDefinition IsTaskRecurring = new IsTaskRecurringProperty();

		public static readonly StorePropertyDefinition CalculatedRecurrenceType = new RecurrenceTypeProperty();

		public static readonly StorePropertyDefinition LinkEnabled = new LinkEnabled();

		public static readonly StorePropertyDefinition StartDate = new StartDate();

		public static readonly StorePropertyDefinition DueDate = new DueDate();

		public static readonly StorePropertyDefinition IsToDoItem = new FlagsProperty("IsToDoItem", InternalSchema.MapiToDoItemFlag, 1, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition IsFlagSetForRecipient = new FlagsProperty("IsFlagSetForRecipient", InternalSchema.MapiToDoItemFlag, 8, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition FlagStatus = new FlagStatusProperty();

		public static readonly StorePropertyDefinition Subject = new SubjectProperty();

		public static readonly StorePropertyDefinition SubjectPrefix = new SubjectPortionProperty("SubjectPrefix", InternalSchema.SubjectPrefixInternal);

		public static readonly StorePropertyDefinition NormalizedSubject = new SubjectPortionProperty("NormalizedSubject", InternalSchema.NormalizedSubjectInternal);

		public static readonly StorePropertyDefinition HasAttachment = new HasAttachmentProperty();

		public static readonly StorePropertyDefinition ClientSubmittedSecurely = new FlagsProperty("ClientSubmittedSecurely", InternalSchema.SecureSubmitFlags, 1, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition ServerSubmittedSecurely = new FlagsProperty("ServerSubmittedSecurely", InternalSchema.SecureSubmitFlags, 2, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition HasBeenSubmitted = new FlagsProperty("HasBeenSubmitted", InternalSchema.Flags, 4, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition IsAssociated = new FlagsProperty("IsAssociated", InternalSchema.Flags, 64, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition IsResend = new FlagsProperty("IsResend", InternalSchema.Flags, 128, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition NeedSpecialRecipientProcessing = new FlagsProperty("NeedSpecialRecipientProcessing", InternalSchema.Flags, 131072, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition IsReadReceiptPendingInternal = new FlagsProperty("IsReadReceiptPendingInternal", InternalSchema.Flags, 256, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition IsNotReadReceiptPendingInternal = new FlagsProperty("IsNotReadReceiptPendingInternal", InternalSchema.Flags, 512, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition IsDraft = new MessageFlagsProperty("IsDraft", InternalSchema.Flags, 8);

		public static readonly StorePropertyDefinition SuppressReadReceipt = new MessageFlagsProperty("SuppressReadReceipt", InternalSchema.Flags, 512);

		public static readonly StorePropertyDefinition IsRestricted = new MessageFlagsProperty("IsRestricted", InternalSchema.Flags, 2048);

		public static readonly StorePropertyDefinition MessageDeliveryNotificationSent = new MessageFlagsProperty("MessageDeliveryNotificationSent", InternalSchema.MessageStatus, 16384);

		public static readonly StorePropertyDefinition MimeConversionFailed = new MessageFlagsProperty("MimeConversionFailed", InternalSchema.MessageStatus, 32768);

		public static readonly StorePropertyDefinition AttachmentIsInline = new AttachmentIsInlineProperty();

		public static readonly StorePropertyDefinition ReplyToBlob = new ReplyToBlobProperty();

		public static readonly StorePropertyDefinition ReplyToNames = new ReplyToNamesProperty();

		public static readonly StorePropertyDefinition LikersBlob = new LikersBlobProperty();

		public static readonly StorePropertyDefinition LikeCount = new LikeCountProperty();

		public static readonly StorePropertyDefinition IsRead = new MessageFlagsProperty("IsRead", InternalSchema.Flags, 1);

		public static readonly StorePropertyDefinition WasEverRead = new MessageFlagsProperty("WasEverRead", InternalSchema.Flags, 1024);

		public static readonly StorePropertyDefinition MessageAnswered = new MessageFlagsProperty("MessageAnswered", InternalSchema.MessageStatus, 512);

		public static readonly StorePropertyDefinition MessageDelMarked = new MessageFlagsProperty("MessageDelMarked", InternalSchema.MessageStatus, 8);

		public static readonly StorePropertyDefinition MessageDraft = new MessageFlagsProperty("MessageDraft", InternalSchema.MessageStatus, 256);

		public static readonly StorePropertyDefinition MessageHidden = new MessageFlagsProperty("MessageHidden", InternalSchema.MessageStatus, 4);

		public static readonly StorePropertyDefinition MessageHighlighted = new MessageFlagsProperty("MessageHighlighted", InternalSchema.MessageStatus, 1);

		public static readonly StorePropertyDefinition MessageInConflict = new MessageFlagsProperty("MessageInConflict", InternalSchema.MessageStatus, 2048);

		public static readonly StorePropertyDefinition MessageRemoteDelete = new MessageFlagsProperty("MessageRemoteDelete", InternalSchema.MessageStatus, 8192);

		public static readonly StorePropertyDefinition MessageRemoteDownload = new MessageFlagsProperty("MessageRemoteDownload", InternalSchema.MessageStatus, 4096);

		public static readonly StorePropertyDefinition MessageTagged = new MessageFlagsProperty("MessageTagged", InternalSchema.MessageStatus, 2);

		public static readonly StorePropertyDefinition ReminderDueBy = new ReminderDueByProperty();

		public static readonly ReminderAdjustmentProperty ReminderMinutesBeforeStart = new ReminderAdjustmentProperty("ReminderMinutesBeforeStartProperty", InternalSchema.ReminderMinutesBeforeStartInternal);

		public static readonly ReminderAdjustmentProperty ReminderIsSet = new ReminderAdjustmentProperty("ReminderIsSetProperty", InternalSchema.ReminderIsSetInternal);

		public static readonly AutoResponseSuppressProperty AutoResponseSuppress = new AutoResponseSuppressProperty();

		public static readonly AutoResponseRequestProperty IsDeliveryReceiptRequested = new AutoResponseRequestProperty("IsDeliveryReceiptRequested", InternalSchema.IsDeliveryReceiptRequestedInternal);

		public static readonly AutoResponseRequestProperty IsNonDeliveryReceiptRequested = new AutoResponseRequestProperty("IsNonDeliveryReceiptRequested", InternalSchema.IsNonDeliveryReceiptRequestedInternal);

		public static readonly AutoResponseRequestProperty IsReadReceiptRequested = new AutoResponseRequestProperty("IsReadReceiptRequested", InternalSchema.IsReadReceiptRequestedInternal);

		public static readonly AutoResponseRequestProperty IsNotReadReceiptRequested = new AutoResponseRequestProperty("IsNotReadReceiptRequested", InternalSchema.IsNotReadReceiptRequestedInternal);

		public static readonly AutoResponseFlagProperty IsReadReceiptPending = new AutoResponseFlagProperty("IsReadReceiptPending", MessageFlags.IsReadReceiptPending, Microsoft.Exchange.Data.Storage.AutoResponseSuppress.RN);

		public static readonly AutoResponseFlagProperty IsNotReadReceiptPending = new AutoResponseFlagProperty("IsNotReadReceiptPending", MessageFlags.IsNotReadReceiptPending, Microsoft.Exchange.Data.Storage.AutoResponseSuppress.NRN);

		public static readonly StorePropertyDefinition AppointmentState = new AppointmentStateProperty();

		public static readonly StorePropertyDefinition IsAllDayEvent = new IsAllDayEventProperty();

		public static readonly StorePropertyDefinition IsEvent = new IsEventProperty();

		public static readonly StorePropertyDefinition CalendarItemType = new CalendarItemTypeProperty();

		public static readonly StorePropertyDefinition StartTime = new StartTimeProperty();

		public static readonly StorePropertyDefinition EndTime = new EndTimeProperty();

		public static readonly StorePropertyDefinition StartTimeZone = new StartTimeZoneProperty();

		public static readonly StorePropertyDefinition EndTimeZone = new EndTimeZoneProperty();

		public static readonly StorePropertyDefinition StartTimeZoneId = new StartTimeZoneIdProperty();

		public static readonly StorePropertyDefinition EndTimeZoneId = new EndTimeZoneIdProperty();

		public static readonly StorePropertyDefinition StartWallClock = new StartWallClockProperty();

		public static readonly StorePropertyDefinition EndWallClock = new EndWallClockProperty();

		public static readonly StorePropertyDefinition IsMeeting = new IsMeetingProperty();

		public static readonly StorePropertyDefinition IsSeriesCancelled = new IsSeriesCancelledProperty();

		public static readonly StorePropertyDefinition PropertyChangeMetadata = new PropertyChangeMetadataProperty();

		public static readonly StorePropertyDefinition CalendarInteropActionQueue = new ActionQueueProperty(InternalSchema.CalendarInteropActionQueueInternal, InternalSchema.CalendarInteropActionQueueHasDataInternal);

		public static readonly StorePropertyDefinition CalendarInteropActionQueueHasData = new ActionQueueHasDataProperty(InternalSchema.CalendarInteropActionQueueHasDataInternal);

		public static readonly StorePropertyDefinition ExtendedFolderFlags = new ExtendedFolderFlagsProperty(ExtendedFolderFlagsProperty.FlagTag.Flags);

		public static readonly StorePropertyDefinition ExtendedFolderToDoVersion = new ExtendedFolderFlagsProperty(ExtendedFolderFlagsProperty.FlagTag.ToDoVersion);

		public static readonly StorePropertyDefinition InternetCpid = new InternetCpidProperty();

		public static readonly StorePropertyDefinition Importance = new ImportanceProperty();

		public static readonly StorePropertyDefinition Sensitivity = new SensitivityProperty();

		public static readonly StorePropertyDefinition IsUnmodified = new FlagsProperty("IsUnmodified", InternalSchema.Flags, 2, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition IsOutOfDate = new IsOutOfDateProperty();

		public static readonly StorePropertyDefinition EmailRoutingType = new SimpleVirtualPropertyDefinition("EmailAddrType", typeof(string), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static readonly StorePropertyDefinition EmailDisplayName = new SimpleVirtualPropertyDefinition("EmailDisplayName", typeof(string), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static readonly StorePropertyDefinition EmailOriginalDisplayName = new SimpleVirtualPropertyDefinition("EmailOriginalDisplayName", typeof(string), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static readonly StorePropertyDefinition EmailAddressForDisplay = new SimpleVirtualPropertyDefinition("EmailAddressForDisplay", typeof(string), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static readonly StorePropertyDefinition ParticipantOriginItemId = new SimpleVirtualPropertyDefinition("ParticipantOriginItemId", typeof(StoreObjectId), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static readonly StorePropertyDefinition LegacyExchangeDN = new SimpleVirtualPropertyDefinition("LegacyExchangeDN", typeof(string), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static readonly StorePropertyDefinition Alias = new SimpleVirtualPropertyDefinition("Alias", typeof(string), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static readonly PropertyTagPropertyDefinition ReadCnNew = PropertyTagPropertyDefinition.InternalCreate("ReadCnNew", PropTag.ReadCnNew);

		public static readonly PropertyTagPropertyDefinition SipUri = PropertyTagPropertyDefinition.InternalCreate("SipUri", (PropTag)1608843295U);

		public static readonly StorePropertyDefinition DisplayTypeEx = new DisplayTypeExProperty();

		public static readonly StorePropertyDefinition IsDistributionList = new IsDistributionListProperty();

		public static readonly PropertyTagPropertyDefinition IsDistributionListContact = PropertyTagPropertyDefinition.InternalCreate("IsDistributionListContact", PropTag.IsDistributionListContact);

		public static readonly GuidNamePropertyDefinition IsFavorite = InternalSchema.CreateGuidNameProperty("IsFavorite", typeof(bool), WellKnownPropertySet.Address, "IsFavorite");

		public static readonly PropertyTagPropertyDefinition IsPromotedContact = PropertyTagPropertyDefinition.InternalCreate("IsPromotedContact", PropTag.IsPromotedContact);

		public static readonly StorePropertyDefinition IsRoom = new IsRoomProperty();

		public static readonly StorePropertyDefinition IsResource = new IsResourceProperty();

		public static readonly StorePropertyDefinition IsGroupMailbox = new IsGroupMailboxProperty();

		public static readonly StorePropertyDefinition IsMailboxUser = new IsMailboxUserProperty();

		public static readonly IdProperty ItemId = new ItemIdProperty();

		public static readonly IdProperty FolderId = new FolderIdProperty();

		public static readonly IdProperty MailboxId = new MailboxIdProperty();

		public static readonly StorePropertyDefinition ParentItemId = new FolderItemIdProperty(InternalSchema.ParentEntryId, "ParentUniqueItemId");

		public static readonly StorePropertyDefinition IsOrganizer = new IsOrganizerProperty();

		public static readonly StorePropertyDefinition BlockStatus = new OutlookBlockStatusProperty();

		public static readonly StorePropertyDefinition MeetingMessageResponseType = new MeetingResponseType();

		public static readonly SmartPropertyDefinition IsOutlookSearchFolder = new IsOutlookSearchFolderProperty();

		public static readonly SmartPropertyDefinition FolderHomePageUrl = new FolderHomePageUrlProperty();

		public static readonly SmartPropertyDefinition OutlookSearchFolderClsId = new OutlookSearchFolderClsIdProperty();

		public static readonly StorePropertyDefinition DisplayAll = new DisplayXXProperty("DisplayAll");

		public static readonly StorePropertyDefinition DisplayTo = new DisplayXXProperty("DisplayTo", InternalSchema.DisplayToInternal, new RecipientItemType?(RecipientItemType.To));

		public static readonly StorePropertyDefinition DisplayCc = new DisplayXXProperty("DisplayCc", InternalSchema.DisplayCcInternal, new RecipientItemType?(RecipientItemType.Cc));

		public static readonly StorePropertyDefinition DisplayBcc = new DisplayXXProperty("DisplayBcc", InternalSchema.DisplayBccInternal, new RecipientItemType?(RecipientItemType.Bcc));

		public static readonly FileAsStringProperty FileAsString = new FileAsStringProperty();

		public static readonly PhysicalAddressProperty HomeAddress = new PhysicalAddressProperty("HomeAddress", InternalSchema.HomeAddressInternal, InternalSchema.HomeStreet, InternalSchema.HomeCity, InternalSchema.HomeState, InternalSchema.HomePostalCode, InternalSchema.HomeCountry);

		public static readonly PhysicalAddressProperty BusinessAddress = new PhysicalAddressProperty("BusinessAddress", InternalSchema.BusinessAddressInternal, InternalSchema.WorkAddressStreet, InternalSchema.WorkAddressCity, InternalSchema.WorkAddressState, InternalSchema.WorkAddressPostalCode, InternalSchema.WorkAddressCountry);

		public static readonly PhysicalAddressProperty OtherAddress = new PhysicalAddressProperty("OtherAddress", InternalSchema.OtherAddressInternal, InternalSchema.OtherStreet, InternalSchema.OtherCity, InternalSchema.OtherState, InternalSchema.OtherPostalCode, InternalSchema.OtherCountry);

		public static readonly PropertyTagPropertyDefinition PredictedActionsInternal = PropertyTagPropertyDefinition.InternalCreate("PredictedActionsInternal", PropTag.PredictedActions);

		public static readonly PredictedActionsProperty PredictedActions = new PredictedActionsProperty("PredictedActions", InternalSchema.PredictedActionsInternal, PropertyFlags.None);

		public static readonly PropertyTagPropertyDefinition InferencePredictedReplyForwardReasons = PropertyTagPropertyDefinition.InternalCreate("InferencePredictedReplyForwardReasons", PropTag.InferencePredictedReplyForwardReasons);

		public static readonly PropertyTagPropertyDefinition InferencePredictedDeleteReasons = PropertyTagPropertyDefinition.InternalCreate("InferencePredictedDeleteReasons", PropTag.InferencePredictedDeleteReasons);

		public static readonly PropertyTagPropertyDefinition InferencePredictedIgnoreReasons = PropertyTagPropertyDefinition.InternalCreate("InferencePredictedIgnoreReasons", PropTag.InferencePredictedIgnoreReasons);

		public static readonly PropertyTagPropertyDefinition IsClutter = PropertyTagPropertyDefinition.InternalCreate("IsClutter", PropTag.IsClutter);

		public static readonly PropertyTagPropertyDefinition OriginalDeliveryFolderInfo = PropertyTagPropertyDefinition.InternalCreate("OriginalDeliveryFolderInfo", PropTag.OriginalDeliveryFolderInfo);

		public static readonly GuidNamePropertyDefinition XmlExtractedMeetings = InternalSchema.CreateGuidNameProperty("XmlExtractedMeetings", typeof(string), WellKnownPropertySet.Inference, "XmlExtractedMeetings");

		public static readonly GuidNamePropertyDefinition XmlExtractedTasks = InternalSchema.CreateGuidNameProperty("XmlExtractedTasks", typeof(string), WellKnownPropertySet.Inference, "XmlExtractedTasks");

		public static readonly GuidNamePropertyDefinition XmlExtractedKeywords = InternalSchema.CreateGuidNameProperty("XmlExtractedKeywords", typeof(string), WellKnownPropertySet.Inference, "XmlExtractedKeywords");

		public static readonly GuidNamePropertyDefinition XmlExtractedAddresses = InternalSchema.CreateGuidNameProperty("XmlExtractedAddresses", typeof(string), WellKnownPropertySet.Inference, "XmlExtractedAddresses");

		public static readonly GuidNamePropertyDefinition XmlExtractedPhones = InternalSchema.CreateGuidNameProperty("XmlExtractedPhones", typeof(string), WellKnownPropertySet.Inference, "XmlExtractedPhones");

		public static readonly GuidNamePropertyDefinition XmlExtractedEmails = InternalSchema.CreateGuidNameProperty("XmlExtractedEmails", typeof(string), WellKnownPropertySet.Inference, "XmlExtractedEmails");

		public static readonly GuidNamePropertyDefinition XmlExtractedUrls = InternalSchema.CreateGuidNameProperty("XmlExtractedUrls", typeof(string), WellKnownPropertySet.Inference, "XmlExtractedUrls");

		public static readonly GuidNamePropertyDefinition XmlExtractedContacts = InternalSchema.CreateGuidNameProperty("XmlExtractedContacts", typeof(string), WellKnownPropertySet.Inference, "XmlExtractedContacts");

		public static readonly GuidNamePropertyDefinition SenderRelevanceScore = InternalSchema.CreateGuidNameProperty("SenderRelevanceScore", typeof(int), WellKnownPropertySet.Address, "SenderRelevanceScore");

		public static readonly GuidNamePropertyDefinition DetectedLanguage = InternalSchema.CreateGuidNameProperty("DetectedLanguage", typeof(string), WellKnownPropertySet.Search, "DetectedLanguage");

		public static readonly GuidNamePropertyDefinition IndexingErrorCode = InternalSchema.CreateGuidNameProperty("IndexingErrorCode", typeof(int), WellKnownPropertySet.Search, "IndexingErrorCode");

		public static readonly GuidNamePropertyDefinition IsPartiallyIndexed = InternalSchema.CreateGuidNameProperty("IsPartiallyIndexed", typeof(bool), WellKnownPropertySet.Search, "IsPartiallyIndexed");

		public static readonly GuidNamePropertyDefinition LastIndexingAttemptTime = InternalSchema.CreateGuidNameProperty("LastIndexingAttemptTime", typeof(ExDateTime), WellKnownPropertySet.Search, "LastIndexingAttemptTime");

		public static readonly ExtractedNaturalLanguageProperty<Meeting, MeetingSet> ExtractedMeetings = new ExtractedNaturalLanguageProperty<Meeting, MeetingSet>("ExtractedMeetings", InternalSchema.XmlExtractedMeetings);

		public static readonly ExtractedNaturalLanguageProperty<Task, TaskSet> ExtractedTasks = new ExtractedNaturalLanguageProperty<Task, TaskSet>("ExtractedTasks", InternalSchema.XmlExtractedTasks);

		public static readonly ExtractedNaturalLanguageProperty<Address, AddressSet> ExtractedAddresses = new ExtractedNaturalLanguageProperty<Address, AddressSet>("ExtractedAddresses", InternalSchema.XmlExtractedAddresses);

		public static readonly ExtractedNaturalLanguageProperty<Keyword, KeywordSet> ExtractedKeywords = new ExtractedNaturalLanguageProperty<Keyword, KeywordSet>("ExtractedKeywords", InternalSchema.XmlExtractedKeywords);

		public static readonly ExtractedNaturalLanguageProperty<Url, UrlSet> ExtractedUrls = new ExtractedNaturalLanguageProperty<Url, UrlSet>("ExtractedUrls", InternalSchema.XmlExtractedUrls);

		public static readonly ExtractedNaturalLanguageProperty<Phone, PhoneSet> ExtractedPhones = new ExtractedNaturalLanguageProperty<Phone, PhoneSet>("ExtractedPhones", InternalSchema.XmlExtractedPhones);

		public static readonly ExtractedNaturalLanguageProperty<Email, EmailSet> ExtractedEmails = new ExtractedNaturalLanguageProperty<Email, EmailSet>("ExtractedEmails", InternalSchema.XmlExtractedEmails);

		public static readonly ExtractedNaturalLanguageProperty<Contact, ContactSet> ExtractedContacts = new ExtractedNaturalLanguageProperty<Contact, ContactSet>("ExtractedContacts", InternalSchema.XmlExtractedContacts);

		public static readonly GuidNamePropertyDefinition XSimSlotNumber = InternalSchema.CreateGuidNameProperty("X-SimSlotNumber", typeof(string), WellKnownPropertySet.InternetHeaders, "X-SimSlotNumber");

		public static readonly GuidNamePropertyDefinition XMmsMessageId = InternalSchema.CreateGuidNameProperty("X-MmsMessageId", typeof(string), WellKnownPropertySet.InternetHeaders, "X-MmsMessageId");

		public static readonly GuidNamePropertyDefinition XSentItem = InternalSchema.CreateGuidNameProperty("X-SentItem", typeof(string), WellKnownPropertySet.InternetHeaders, "X-SentItem");

		public static readonly GuidNamePropertyDefinition XSentTime = InternalSchema.CreateGuidNameProperty("X-SentTime", typeof(string), WellKnownPropertySet.InternetHeaders, "X-SentTime");

		public static readonly GuidNamePropertyDefinition ExternalSharingSharerIdentity = InternalSchema.CreateGuidNameProperty("ExternalSharingSharerIdentity", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingSharerIdentity");

		public static readonly GuidNamePropertyDefinition ExternalSharingSharerName = InternalSchema.CreateGuidNameProperty("ExternalSharingSharerName", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingSharerName");

		public static readonly GuidNamePropertyDefinition ExternalSharingRemoteFolderId = InternalSchema.CreateGuidNameProperty("ExternalSharingRemoteFolderId", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingRemoteFolderId");

		public static readonly GuidNamePropertyDefinition ExternalSharingRemoteFolderName = InternalSchema.CreateGuidNameProperty("ExternalSharingRemoteFolderName", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingRemoteFolderName");

		public static readonly GuidNamePropertyDefinition ExternalSharingLevelOfDetails = InternalSchema.CreateGuidNameProperty("ExternalSharingLevelOfDetails", typeof(int), WellKnownPropertySet.ExternalSharing, "ExternalSharingLevelOfDetails");

		public static readonly GuidNamePropertyDefinition ExternalSharingIsPrimary = InternalSchema.CreateGuidNameProperty("ExternalSharingIsPrimary", typeof(bool), WellKnownPropertySet.ExternalSharing, "ExternalSharingIsPrimary");

		public static readonly GuidNamePropertyDefinition ExternalSharingSharerIdentityFederationUri = InternalSchema.CreateGuidNameProperty("ExternalSharingSharerIdentityFederationUri", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingSharerIdentityFederationUri");

		public static readonly GuidNamePropertyDefinition ExternalSharingUrl = InternalSchema.CreateGuidNameProperty("ExternalSharingUrl", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingUrl");

		public static readonly GuidNamePropertyDefinition ExternalSharingLocalFolderId = InternalSchema.CreateGuidNameProperty("ExternalSharingLocalFolderId", typeof(byte[]), WellKnownPropertySet.ExternalSharing, "ExternalSharingLocalFolderId");

		public static readonly GuidNamePropertyDefinition ExternalSharingDataType = InternalSchema.CreateGuidNameProperty("ExternalSharingDataType", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingDataType");

		public static readonly GuidNamePropertyDefinition ExternalSharingSharingKey = InternalSchema.CreateGuidNameProperty("ExternalSharingSharingKey", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingSharingKey");

		public static readonly GuidNamePropertyDefinition ExternalSharingSubscriberIdentity = InternalSchema.CreateGuidNameProperty("ExternalSharingSubscriberIdentity", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingSubscriberIdentity");

		public static readonly GuidNamePropertyDefinition ExternalSharingMasterId = InternalSchema.CreateGuidNameProperty("ExternalSharingMasterId", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingMasterId");

		public static readonly GuidNamePropertyDefinition ExternalSharingSyncState = InternalSchema.CreateGuidNameProperty("ExternalSharingSyncState", typeof(string), WellKnownPropertySet.ExternalSharing, "ExternalSharingSyncState", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition SubscriptionLastSuccessfulSyncTime = InternalSchema.CreateGuidNameProperty("ExternalSharingLastSuccessfulSyncTime", typeof(ExDateTime), WellKnownPropertySet.ExternalSharing, "ExternalSharingLastSuccessfulSyncTime");

		public static readonly PropertyTagPropertyDefinition MimeSkeleton = PropertyTagPropertyDefinition.InternalCreate("MimeSkeleton", (PropTag)1693450498U, PropertyFlags.Streamable);

		public static readonly GuidNamePropertyDefinition ExchangeApplicationFlags = InternalSchema.CreateGuidNameProperty("ExchangeApplicationFlags", typeof(int), WellKnownPropertySet.Common, "ExchangeApplicationFlags");

		public static readonly StorePropertyDefinition ConversationId = new ConversationIdFromIndexProperty();

		public static readonly PropertyTagPropertyDefinition ConversationTopicHashEntries = PropertyTagPropertyDefinition.InternalCreate("ConversationTopicHashEntries", PropTag.ConversationTopicHashEntries);

		public static readonly PropertyTagPropertyDefinition MapiConversationFamilyId = PropertyTagPropertyDefinition.InternalCreate("MapiConversationFamilyId", PropTag.ConversationFamilyId);

		public static readonly StorePropertyDefinition ConversationFamilyId = new ConversationFamilyIdProperty();

		public static readonly StorePropertyDefinition MessageSentRepresentingType = new MessageSentRepresentingTypeProperty();

		public static readonly StorePropertyDefinition ConversationFamilyIndex = new ConversationFamilyIndexProperty();

		public static readonly StorePropertyDefinition IsFromFavoriteSender = new ExchangeApplicationFlagsProperty(Microsoft.Exchange.Data.Storage.ExchangeApplicationFlags.IsFromFavoriteSender);

		public static readonly StorePropertyDefinition IsFromPerson = new ExchangeApplicationFlagsProperty(Microsoft.Exchange.Data.Storage.ExchangeApplicationFlags.IsFromPerson);

		public static readonly StorePropertyDefinition IsSpecificMessageReplyStamped = new ExchangeApplicationFlagsProperty(Microsoft.Exchange.Data.Storage.ExchangeApplicationFlags.IsSpecificMessageReplyStamped);

		public static readonly StorePropertyDefinition IsSpecificMessageReply = new ExchangeApplicationFlagsProperty(Microsoft.Exchange.Data.Storage.ExchangeApplicationFlags.IsSpecificMessageReply);

		public static readonly StorePropertyDefinition RelyOnConversationIndex = new ExchangeApplicationFlagsProperty(Microsoft.Exchange.Data.Storage.ExchangeApplicationFlags.RelyOnConversationIndex);

		public static readonly StorePropertyDefinition SupportsSideConversation = new ExchangeApplicationFlagsProperty(Microsoft.Exchange.Data.Storage.ExchangeApplicationFlags.SupportsSideConversation);

		public static readonly StorePropertyDefinition IsGroupEscalationMessage = new ExchangeApplicationFlagsProperty(Microsoft.Exchange.Data.Storage.ExchangeApplicationFlags.IsGroupEscalationMessage);

		public static readonly StorePropertyDefinition IsClutterOverridden = new ExchangeApplicationFlagsProperty(Microsoft.Exchange.Data.Storage.ExchangeApplicationFlags.IsClutterOverridden);

		public static readonly GuidIdPropertyDefinition ConversationActionMoveFolderId = InternalSchema.CreateGuidIdProperty("ConversationActionMoveFolderId", typeof(byte[]), WellKnownPropertySet.Common, 34246);

		public static readonly GuidIdPropertyDefinition ConversationActionMoveStoreId = InternalSchema.CreateGuidIdProperty("ConversationActionMoveStoreId", typeof(byte[]), WellKnownPropertySet.Common, 34247);

		public static readonly GuidIdPropertyDefinition ConversationActionMaxDeliveryTime = InternalSchema.CreateGuidIdProperty("ConversationActionMaxDeliveryTime", typeof(ExDateTime), WellKnownPropertySet.Common, 34248);

		public static readonly GuidIdPropertyDefinition ConversationActionVersion = InternalSchema.CreateGuidIdProperty("ConversationActionVersion", typeof(int), WellKnownPropertySet.Common, 34251);

		public static readonly GuidIdPropertyDefinition ConversationActionPolicyTag = InternalSchema.CreateGuidIdProperty("ConversationActionPolicyTag", typeof(byte[]), WellKnownPropertySet.Common, 34254);

		public static readonly GuidNamePropertyDefinition ConversationActionLastMoveFolderId = InternalSchema.CreateGuidNameProperty("ConversationActionLastMoveFolderId", typeof(byte[]), WellKnownPropertySet.Conversations, "ConversationActionLastMoveFolderId");

		public static readonly GuidNamePropertyDefinition ConversationActionLastCategorySet = InternalSchema.CreateGuidNameProperty("ConversationActionLastCategorySet", typeof(string[]), WellKnownPropertySet.Conversations, "ConversationActionLastCategorySet", PropertyFlags.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(1, 255),
			new CharacterConstraint(Category.ProhibitedCharacters, false)
		});

		public static readonly PropertyTagPropertyDefinition ActivityId = PropertyTagPropertyDefinition.InternalCreate("ActivityId", PropTag.InferenceActivityId);

		public static readonly PropertyTagPropertyDefinition ActivityItemIdBytes = PropertyTagPropertyDefinition.InternalCreate("ActivityItemIdBytes", PropTag.InferenceItemId);

		public static readonly PropertyTagPropertyDefinition ActivityTimeStamp = PropertyTagPropertyDefinition.InternalCreate("ActivityTimeStamp", PropTag.InferenceTimeStamp);

		public static readonly PropertyTagPropertyDefinition ActivityClientId = PropertyTagPropertyDefinition.InternalCreate("ActivityClientId", PropTag.InferenceClientId);

		public static readonly PropertyTagPropertyDefinition ActivityWindowId = PropertyTagPropertyDefinition.InternalCreate("ActivityWindowId", PropTag.InferenceWindowId);

		public static readonly PropertyTagPropertyDefinition ActivitySessionId = PropertyTagPropertyDefinition.InternalCreate("ActivitySessionId", PropTag.InferenceSessionId);

		public static readonly PropertyTagPropertyDefinition ActivityFolderIdBytes = PropertyTagPropertyDefinition.InternalCreate("ActivityFolderIdBytes", PropTag.InferenceFolderId);

		public static readonly PropertyTagPropertyDefinition ActivityDeleteType = PropertyTagPropertyDefinition.InternalCreate("ActivityDeleteType", PropTag.InferenceDeleteType);

		public static readonly PropertyTagPropertyDefinition ActivityOofEnabled = PropertyTagPropertyDefinition.InternalCreate("ActivityOofEnabled", PropTag.InferenceOofEnabled);

		public static readonly PropertyTagPropertyDefinition ActivityBrowser = PropertyTagPropertyDefinition.InternalCreate("ActivityBrowser", PropTag.MemberEmail);

		public static readonly PropertyTagPropertyDefinition ActivityLocaleId = PropertyTagPropertyDefinition.InternalCreate("ActivityLocaleId", PropTag.InferenceLocaleId);

		public static readonly PropertyTagPropertyDefinition ActivityLocation = PropertyTagPropertyDefinition.InternalCreate("ActivityLocation", PropTag.InferenceLocation);

		public static readonly PropertyTagPropertyDefinition ActivityConversationId = PropertyTagPropertyDefinition.InternalCreate("ActivityConversationId", PropTag.InferenceConversationId);

		public static readonly PropertyTagPropertyDefinition ActivityTimeZone = PropertyTagPropertyDefinition.InternalCreate("ActivityTimeZone", PropTag.InferenceTimeZone);

		public static readonly PropertyTagPropertyDefinition ActivityIpAddress = PropertyTagPropertyDefinition.InternalCreate("ActivityIpAddress", PropTag.InferenceIpAddress);

		public static readonly PropertyTagPropertyDefinition ActivityCategory = PropertyTagPropertyDefinition.InternalCreate("ActivityCategory", PropTag.InferenceCategory);

		public static readonly PropertyTagPropertyDefinition ActivityAttachmentIdBytes = PropertyTagPropertyDefinition.InternalCreate("ActivityAttachmentIdBytes", PropTag.InferenceAttachmentId);

		public static readonly PropertyTagPropertyDefinition ActivityGlobalObjectIdBytes = PropertyTagPropertyDefinition.InternalCreate("ActivityGlobalObjectIdBytes", PropTag.InferenceGlobalObjectId);

		public static readonly PropertyTagPropertyDefinition ActivityModuleSelected = PropertyTagPropertyDefinition.InternalCreate("ActivityModuleSelected", PropTag.InferenceModuleSelected);

		public static readonly PropertyTagPropertyDefinition ActivityLayoutType = PropertyTagPropertyDefinition.InternalCreate("ActivityLayoutType", PropTag.InferenceLayoutType);

		public static readonly EmbeddedParticipantProperty ReceivedBy = new EmbeddedParticipantProperty("ReceivedBy", ParticipantEntryIdConsumer.SupportsADParticipantEntryId, InternalSchema.ReceivedByName, InternalSchema.ReceivedByEmailAddress, InternalSchema.ReceivedByAddrType, InternalSchema.ReceivedByEntryId, InternalSchema.ReceivedBySmtpAddress, null, null, null);

		public static readonly EmbeddedParticipantProperty ReceivedRepresenting = new EmbeddedParticipantProperty("ReceivedRepresenting", ParticipantEntryIdConsumer.SupportsADParticipantEntryId, InternalSchema.ReceivedRepresentingDisplayName, InternalSchema.ReceivedRepresentingEmailAddress, InternalSchema.ReceivedRepresentingAddressType, InternalSchema.ReceivedRepresentingEntryId, InternalSchema.ReceivedRepresentingSmtpAddress, null, null, null);

		public static readonly SenderParticipantProperty Sender = new SenderParticipantProperty("Sender", InternalSchema.SenderDisplayName, InternalSchema.SenderEmailAddress, InternalSchema.SenderAddressType, InternalSchema.SenderEntryId, InternalSchema.SenderSmtpAddress, null, InternalSchema.SenderSID, null);

		public static readonly SenderParticipantProperty From = new SenderParticipantProperty("From", InternalSchema.SentRepresentingDisplayName, InternalSchema.SentRepresentingEmailAddress, InternalSchema.SentRepresentingType, InternalSchema.SentRepresentingEntryId, InternalSchema.SentRepresentingSmtpAddress, InternalSchema.SipUri, InternalSchema.SentRepresentingSID, null);

		public static readonly EmbeddedParticipantProperty OriginalFrom = new EmbeddedParticipantProperty("OriginalFrom", ParticipantEntryIdConsumer.SupportsADParticipantEntryId, InternalSchema.OriginalSentRepresentingDisplayName, InternalSchema.OriginalSentRepresentingEmailAddress, InternalSchema.OriginalSentRepresentingAddressType, InternalSchema.OriginalSentRepresentingEntryId, InternalSchema.OriginalSentRepresentingSmtpAddress, null, null, null);

		public static readonly EmbeddedParticipantProperty OriginalSender = new EmbeddedParticipantProperty("OriginalSender", ParticipantEntryIdConsumer.SupportsADParticipantEntryId, InternalSchema.OriginalSenderDisplayName, InternalSchema.OriginalSenderEmailAddress, InternalSchema.OriginalSenderAddressType, InternalSchema.OriginalSenderEntryId, InternalSchema.OriginalSenderSmtpAddress, null, null, null);

		public static readonly EmbeddedParticipantProperty OriginalAuthor = new EmbeddedParticipantProperty("OriginalAuthor", ParticipantEntryIdConsumer.SupportsADParticipantEntryId, InternalSchema.OriginalAuthorName, InternalSchema.OriginalAuthorEmailAddress, InternalSchema.OriginalAuthorAddressType, InternalSchema.OriginalAuthorEntryId, InternalSchema.OriginalAuthorSmtpAddress, null, null, null);

		public static readonly EmbeddedParticipantProperty ReadReceiptAddressee = new EmbeddedParticipantProperty("ReadReceiptAddressee", ParticipantEntryIdConsumer.SupportsADParticipantEntryId, InternalSchema.ReadReceiptDisplayName, InternalSchema.ReadReceiptEmailAddress, InternalSchema.ReadReceiptAddrType, InternalSchema.ReadReceiptEntryId, InternalSchema.ReadReceiptSmtpAddress, null, null, null);

		public static readonly EmbeddedParticipantProperty RecipientBaseParticipant = new RecipientBaseParticipantProperty();

		public static readonly ContactEmailSlotParticipantProperty ContactEmail1 = new ContactEmailSlotParticipantProperty(EmailAddressIndex.Email1, InternalSchema.Email1DisplayName, InternalSchema.Email1EmailAddress, InternalSchema.Email1AddrType, InternalSchema.Email1OriginalEntryID, InternalSchema.Email1OriginalDisplayName, new PropertyDependency[0]);

		public static readonly ContactEmailSlotParticipantProperty ContactEmail2 = new ContactEmailSlotParticipantProperty(EmailAddressIndex.Email2, InternalSchema.Email2DisplayName, InternalSchema.Email2EmailAddress, InternalSchema.Email2AddrType, InternalSchema.Email2OriginalEntryID, InternalSchema.Email2OriginalDisplayName, new PropertyDependency[0]);

		public static readonly ContactEmailSlotParticipantProperty ContactEmail3 = new ContactEmailSlotParticipantProperty(EmailAddressIndex.Email3, InternalSchema.Email3DisplayName, InternalSchema.Email3EmailAddress, InternalSchema.Email3AddrType, InternalSchema.Email3OriginalEntryID, InternalSchema.Email3OriginalDisplayName, new PropertyDependency[0]);

		public static readonly ContactFaxSlotParticipantProperty ContactOtherFax = new ContactFaxSlotParticipantProperty(EmailAddressIndex.OtherFax, InternalSchema.Fax1OriginalDisplayName, InternalSchema.Fax1EmailAddress, InternalSchema.Fax1AddrType, InternalSchema.Fax1OriginalEntryID, InternalSchema.OtherFax);

		public static readonly ContactFaxSlotParticipantProperty ContactBusinessFax = new ContactFaxSlotParticipantProperty(EmailAddressIndex.BusinessFax, InternalSchema.Fax2OriginalDisplayName, InternalSchema.Fax2EmailAddress, InternalSchema.Fax2AddrType, InternalSchema.Fax2OriginalEntryID, InternalSchema.FaxNumber);

		public static readonly ContactFaxSlotParticipantProperty ContactHomeFax = new ContactFaxSlotParticipantProperty(EmailAddressIndex.HomeFax, InternalSchema.Fax3OriginalDisplayName, InternalSchema.Fax3EmailAddress, InternalSchema.Fax3AddrType, InternalSchema.Fax3OriginalEntryID, InternalSchema.HomeFax);

		public static readonly DistributionListParticipantProperty DistributionListParticipant = new DistributionListParticipantProperty();

		public static readonly SimpleVirtualPropertyDefinition AnrViewParticipant = new SimpleVirtualPropertyDefinition("ItemAsParticipant", typeof(Participant), PropertyFlags.ReadOnly, new PropertyDefinitionConstraint[0]);

		public static readonly StorePropertyDefinition PrimarySmtpAddress = new PrimarySmtpAddressProperty();

		public static readonly PropertyTagPropertyDefinition LogonRightsOnMailbox = PropertyTagPropertyDefinition.InternalCreate("LogonRightsOnMailbox", (PropTag)1736245251U);

		public static readonly StorePropertyDefinition CanActAsOwner = new FlagsProperty("CanActAsOwner", InternalSchema.LogonRightsOnMailbox, 1, PropertyDefinitionConstraint.None);

		public static readonly StorePropertyDefinition CanSendAs = new FlagsProperty("CanSendAs", InternalSchema.LogonRightsOnMailbox, 2, PropertyDefinitionConstraint.None);

		public static readonly PropertyTagPropertyDefinition MergeMidsetDeleted = PropertyTagPropertyDefinition.InternalCreate("MergeMidsetDeleted", (PropTag)242876674U);

		public static readonly PropertyTagPropertyDefinition MailboxMiscFlags = PropertyTagPropertyDefinition.InternalCreate("MailboxMiscFlags", PropTag.MailboxMiscFlags);

		public static readonly PropertyTagPropertyDefinition MailboxGuid = PropertyTagPropertyDefinition.InternalCreate("MailboxGuid", PropTag.UserGuid);

		public static readonly PropertyTagPropertyDefinition MailboxNumber = PropertyTagPropertyDefinition.InternalCreate("MailboxNumber", PropTag.MailboxNumber);

		public static readonly PropertyTagPropertyDefinition InTransitStatus = PropertyTagPropertyDefinition.InternalCreate("InTransitStatus", PropTag.InTransitStatus);

		public static readonly StorePropertyDefinition PublicFolderFreeBusy = new PublicFolderFreeBusyProperty();

		public static readonly ReplyAllDisplayNamesProperty ReplyAllDisplayNames = new ReplyAllDisplayNamesProperty();

		public static readonly ReplyAllParticipantsProperty ReplyAllParticipants = new ReplyAllParticipantsProperty();

		public static readonly ReplyDisplayNamesProperty ReplyDisplayNames = new ReplyDisplayNamesProperty();

		public static StorePropertyDefinition LastDelegatedAuditTime = new SimpleVirtualPropertyDefinition("LastDelegatedAuditTime", typeof(ExDateTime), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static StorePropertyDefinition LastExternalAuditTime = new SimpleVirtualPropertyDefinition("LastExternalAuditTime", typeof(ExDateTime), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static StorePropertyDefinition LastNonOwnerAuditTime = new SimpleVirtualPropertyDefinition("LastNonOwnerAuditTime", typeof(ExDateTime), PropertyFlags.None, new PropertyDefinitionConstraint[0]);

		public static readonly PropertyTagPropertyDefinition UnsearchableItemsStream = PropertyTagPropertyDefinition.InternalCreate("UnsearchableItemsStream", PropTag.UnsearchableItemsStream, PropertyFlags.ReadOnly | PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition AnnotationToken = PropertyTagPropertyDefinition.InternalCreate("AnnotationToken", PropTag.AnnotationToken, PropertyFlags.Streamable);

		public static readonly PersonIdProperty PersonId = new PersonIdProperty();

		public static readonly BirthdayContactIdProperty BirthdayContactId = new BirthdayContactIdProperty();

		public static readonly AttributionDisplayNameProperty AttributionDisplayName = new AttributionDisplayNameProperty();

		public static readonly IsContactWritableProperty IsContactWritable = new IsContactWritableProperty();

		public static readonly PropertyTagPropertyDefinition ResolveMethod = PropertyTagPropertyDefinition.InternalCreate("PR_RESOLVE_METHOD", (PropTag)1072103427U);

		public static readonly PropertyTagPropertyDefinition ReplicaListBinary = PropertyTagPropertyDefinition.InternalCreate("ReplicaListBinary", PropTag.ReplicaList);

		public static readonly ReplicaListProperty ReplicaList = new ReplicaListProperty();

		public static readonly PropertyTagPropertyDefinition CorrelationId = PropertyTagPropertyDefinition.InternalCreate("CorrelationId", PropTag.CorrelationId, PropertyFlags.ReadOnly);

		public static readonly GuidNamePropertyDefinition UnifiedInboxFolderEntryId = InternalSchema.CreateGuidNameProperty("UnifiedInboxFolderEntryId", typeof(byte[]), WellKnownPropertySet.Address, "UnifiedInboxFolderEntryId");

		public static readonly GuidNamePropertyDefinition TemporarySavesFolderEntryId = InternalSchema.CreateGuidNameProperty("TemporarySavesFolderEntryId", typeof(byte[]), WellKnownPropertySet.Common, "TemporarySavesFolderEntryId");

		public static readonly PropertyTagPropertyDefinition ItemsPendingUpgrade = PropertyTagPropertyDefinition.InternalCreate("ItemsPendingUpgrade", PropTag.ItemsPendingUpgrade);

		public static readonly PropertyTagPropertyDefinition LastLogonTime = PropertyTagPropertyDefinition.InternalCreate("LastLogonTime", PropTag.LastLogonTime);

		public static readonly GuidNamePropertyDefinition ExtractionResult = InternalSchema.CreateGuidNameProperty("ExtractionResult", typeof(string), WellKnownPropertySet.Inference, "ExtractionResult");

		public static readonly GuidNamePropertyDefinition TriageFeatureVector = InternalSchema.CreateGuidNameProperty("TriageFeatureVector", typeof(byte[]), WellKnownPropertySet.Inference, "TriageFeatureVector", PropertyFlags.Streamable, new PropertyDefinitionConstraint[0]);

		public static readonly GuidNamePropertyDefinition InferenceClassificationTrackingEx = InternalSchema.CreateGuidNameProperty("InferenceClassificationTrackingEx", typeof(string), WellKnownPropertySet.Inference, "InferenceClassificationTrackingEx");

		public static readonly GuidNamePropertyDefinition LatestMessageWordCount = InternalSchema.CreateGuidNameProperty("LatestMessageWordCount", typeof(int), WellKnownPropertySet.Inference, "LatestMessageWordCount");

		public static readonly GuidNamePropertyDefinition UnClutteredViewFolderEntryId = InternalSchema.CreateGuidNameProperty("UnClutteredViewFolderEntryId", typeof(byte[]), WellKnownPropertySet.Inference, "UnClutteredViewFolderEntryId");

		public static readonly GuidNamePropertyDefinition ClutteredViewFolderEntryId = InternalSchema.CreateGuidNameProperty("ClutteredViewFolderEntryId", typeof(byte[]), WellKnownPropertySet.Inference, "ClutteredViewFolderEntryId");

		public static readonly GuidNamePropertyDefinition InferenceProcessingNeeded = InternalSchema.CreateGuidNameProperty("InferenceProcessingNeeded", typeof(bool), WellKnownPropertySet.Inference, "InferenceProcessingNeeded");

		public static readonly GuidNamePropertyDefinition InferenceProcessingActions = InternalSchema.CreateGuidNameProperty("InferenceProcessingActions", typeof(long), WellKnownPropertySet.Inference, "InferenceProcessingActions");

		public static readonly GuidNamePropertyDefinition UserActivityFolderEntryId = InternalSchema.CreateGuidNameProperty("UserActivityFolderEntryId", typeof(byte[]), WellKnownPropertySet.Inference, "UserActivityFolderEntryId");

		public static readonly GuidNamePropertyDefinition ClutterFolderEntryId = InternalSchema.CreateGuidNameProperty("ClutterFolderEntryId", typeof(byte[]), WellKnownPropertySet.Inference, "ClutterFolderEntryId");

		public static readonly GuidNamePropertyDefinition ConversationLoadRequiredByInference = InternalSchema.CreateGuidNameProperty("ConversationLoadRequiredByInference", typeof(bool), WellKnownPropertySet.Inference, "ConversationLoadRequiredByInference");

		public static readonly GuidNamePropertyDefinition InferenceActionTruth = InternalSchema.CreateGuidNameProperty("InferenceActionTruth", typeof(int), WellKnownPropertySet.Inference, "InferenceActionTruth");

		public static readonly GuidNamePropertyDefinition InferenceConversationClutterActionApplied = InternalSchema.CreateGuidNameProperty("InferenceConversationClutterActionApplied", typeof(bool), WellKnownPropertySet.Inference, "InferenceConversationClutterActionApplied");

		public static readonly GuidNamePropertyDefinition InferenceNeverClutterOverrideApplied = InternalSchema.CreateGuidNameProperty("InferenceNeverClutterOverrideApplied", typeof(bool), WellKnownPropertySet.Inference, "InferenceNeverClutterOverrideApplied");

		public static readonly GuidNamePropertyDefinition InferenceClassificationResult = InternalSchema.CreateGuidNameProperty("InferenceClassificationResult", typeof(int), WellKnownPropertySet.Inference, "InferenceClassificationResult");

		public static readonly GuidNamePropertyDefinition ItemMovedByRule = InternalSchema.CreateGuidNameProperty("ItemMovedByRule", typeof(bool), WellKnownPropertySet.Common, "ItemMovedByRule");

		public static readonly GuidNamePropertyDefinition ItemMovedByConversationAction = InternalSchema.CreateGuidNameProperty("ItemMovedByConversationAction", typeof(bool), WellKnownPropertySet.Common, "ItemMovedByConversationAction");

		public static readonly GuidNamePropertyDefinition IsStopProcessingRuleApplicable = InternalSchema.CreateGuidNameProperty("IsStopProcessingRuleApplicable", typeof(bool), WellKnownPropertySet.Common, "IsStopProcessingRuleApplicable");

		public static readonly GuidNamePropertyDefinition InferenceMessageIdentifier = InternalSchema.CreateGuidNameProperty("InferenceMessageIdentifier", typeof(Guid), WellKnownPropertySet.Inference, "InferenceMessageIdentifier");

		public static readonly GuidNamePropertyDefinition InferenceUniqueActionLabelData = InternalSchema.CreateGuidNameProperty("InferenceUniqueActionLabelData", typeof(byte[]), WellKnownPropertySet.Inference, "InferenceUniqueActionLabelData");

		public static readonly GuidNamePropertyDefinition NeedGroupExpansion = InternalSchema.CreateGuidNameProperty("NeedGroupExpansion", typeof(bool), WellKnownPropertySet.Compliance, "NeedGroupExpansion");

		public static readonly GuidNamePropertyDefinition GroupExpansionRecipients = InternalSchema.CreateGuidNameProperty("GroupExpansionRecipients", typeof(string), WellKnownPropertySet.Compliance, "GroupExpansionRecipients");

		public static readonly GuidNamePropertyDefinition GroupExpansionError = InternalSchema.CreateGuidNameProperty("GroupExpansionError", typeof(int), WellKnownPropertySet.Compliance, "GroupExpansionError");

		public static readonly PropertyTagPropertyDefinition ToGroupExpansionRecipientsInternal = PropertyTagPropertyDefinition.InternalCreate("ToGroupExpansionRecipientsInternal", PropTag.ToGroupExpansionRecipients, PropertyFlags.ReadOnly | PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition CcGroupExpansionRecipientsInternal = PropertyTagPropertyDefinition.InternalCreate("CcGroupExpansionRecipientsInternal", PropTag.CcGroupExpansionRecipients, PropertyFlags.ReadOnly | PropertyFlags.Streamable);

		public static readonly PropertyTagPropertyDefinition BccGroupExpansionRecipientsInternal = PropertyTagPropertyDefinition.InternalCreate("BccGroupExpansionRecipientsInternal", PropTag.BccGroupExpansionRecipients, PropertyFlags.ReadOnly | PropertyFlags.Streamable);

		public static readonly StorePropertyDefinition ToGroupExpansionRecipients = new RecipientToIndexProperty("ToGroupExpansionRecipients", new RecipientItemType?(RecipientItemType.To), InternalSchema.ToGroupExpansionRecipientsInternal);

		public static readonly StorePropertyDefinition CcGroupExpansionRecipients = new RecipientToIndexProperty("CcGroupExpansionRecipients", new RecipientItemType?(RecipientItemType.Cc), InternalSchema.CcGroupExpansionRecipientsInternal);

		public static readonly StorePropertyDefinition BccGroupExpansionRecipients = new RecipientToIndexProperty("BccGroupExpansionRecipients", new RecipientItemType?(RecipientItemType.Bcc), InternalSchema.BccGroupExpansionRecipientsInternal);

		public static readonly GuidNamePropertyDefinition WorkingSetId = InternalSchema.CreateGuidNameProperty("WorkingSetId", typeof(string), WellKnownPropertySet.WorkingSet, "WorkingSetId");

		public static readonly GuidNamePropertyDefinition WorkingSetSource = InternalSchema.CreateGuidNameProperty("WorkingSetSource", typeof(int), WellKnownPropertySet.WorkingSet, "WorkingSetSource");

		public static readonly GuidNamePropertyDefinition WorkingSetSourcePartition = InternalSchema.CreateGuidNameProperty("WorkingSetSourcePartition", typeof(string), WellKnownPropertySet.WorkingSet, "WorkingSetSourcePartition");

		public static readonly GuidNamePropertyDefinition WorkingSetSourcePartitionInternal = InternalSchema.CreateGuidNameProperty("WorkingSetSourcePartitionInternal", typeof(string), WellKnownPropertySet.WorkingSet, "WorkingSetSourcePartitionInternal");

		public static readonly GuidNamePropertyDefinition WorkingSetFlags = InternalSchema.CreateGuidNameProperty("WorkingSetFlags", typeof(int), WellKnownPropertySet.WorkingSet, "WorkingSetFlags");

		public static readonly GuidNamePropertyDefinition WorkingSetFolderEntryId = InternalSchema.CreateGuidNameProperty("WorkingSetFolderEntryId", typeof(byte[]), WellKnownPropertySet.WorkingSet, "WorkingSetFolderEntryId");

		public static readonly GuidNamePropertyDefinition ParkedMessagesFolderEntryId = InternalSchema.CreateGuidNameProperty("ParkedMessagesFolderEntryId", typeof(byte[]), WellKnownPropertySet.CalendarAssistant, "ParkedMessagesFolderEntryId");

		public static readonly StorePropertyDefinition ReplyToBlobExists = new PropertyExistenceTracker(InternalSchema.MapiReplyToBlob);

		public static readonly StorePropertyDefinition ReplyToNamesExists = new PropertyExistenceTracker(InternalSchema.MapiReplyToNames);

		public static readonly PropertyExistenceTracker ExtractedMeetingsExists = new PropertyExistenceTracker(InternalSchema.XmlExtractedMeetings);

		public static readonly PropertyExistenceTracker ExtractedTasksExists = new PropertyExistenceTracker(InternalSchema.XmlExtractedTasks);

		public static readonly PropertyExistenceTracker ExtractedAddressesExists = new PropertyExistenceTracker(InternalSchema.XmlExtractedAddresses);

		public static readonly PropertyExistenceTracker ExtractedKeywordsExists = new PropertyExistenceTracker(InternalSchema.XmlExtractedKeywords);

		public static readonly PropertyExistenceTracker ExtractedUrlsExists = new PropertyExistenceTracker(InternalSchema.XmlExtractedUrls);

		public static readonly PropertyExistenceTracker ExtractedPhonesExists = new PropertyExistenceTracker(InternalSchema.XmlExtractedPhones);

		public static readonly PropertyExistenceTracker ExtractedEmailsExists = new PropertyExistenceTracker(InternalSchema.XmlExtractedEmails);

		public static readonly PropertyExistenceTracker ExtractedContactsExists = new PropertyExistenceTracker(InternalSchema.XmlExtractedContacts);

		public static readonly PropertyTagPropertyDefinition ConsumerSharingCalendarSubscriptionCount = PropertyTagPropertyDefinition.InternalCreate("ConsumerSharingCalendarSubscriptionCount", PropTag.ConsumerSharingCalendarSubscriptionCount);

		public static readonly GuidNamePropertyDefinition ConsumerCalendarGuid = InternalSchema.CreateGuidNameProperty("ConsumerCalendarGuid", typeof(Guid), WellKnownPropertySet.ConsumerCalendar, null);

		public static readonly GuidNamePropertyDefinition ConsumerCalendarOwnerId = InternalSchema.CreateGuidNameProperty("ConsumerCalendarOwnerId", typeof(long), WellKnownPropertySet.ConsumerCalendar, null);

		public static readonly GuidNamePropertyDefinition ConsumerCalendarPrivateFreeBusyId = InternalSchema.CreateGuidNameProperty("ConsumerCalendarPrivateFreeBusyId", typeof(Guid), WellKnownPropertySet.ConsumerCalendar, null);

		public static readonly GuidNamePropertyDefinition ConsumerCalendarPrivateDetailId = InternalSchema.CreateGuidNameProperty("ConsumerCalendarPrivateDetailId", typeof(Guid), WellKnownPropertySet.ConsumerCalendar, null);

		public static readonly GuidNamePropertyDefinition ConsumerCalendarPublishVisibility = InternalSchema.CreateGuidNameProperty("ConsumerCalendarPublishVisibility", typeof(int), WellKnownPropertySet.ConsumerCalendar, null);

		public static readonly GuidNamePropertyDefinition ConsumerCalendarSharingInvitations = InternalSchema.CreateGuidNameProperty("ConsumerCalendarSharingInvitations", typeof(string), WellKnownPropertySet.ConsumerCalendar, null);

		public static readonly GuidNamePropertyDefinition ConsumerCalendarPermissionLevel = InternalSchema.CreateGuidNameProperty("ConsumerCalendarPermissionLevel", typeof(int), WellKnownPropertySet.ConsumerCalendar, null);

		public static readonly GuidNamePropertyDefinition ConsumerCalendarSynchronizationState = InternalSchema.CreateGuidNameProperty("ConsumerCalendarSynchronizationState", typeof(string), WellKnownPropertySet.ConsumerCalendar, null);

		public static readonly GuidNamePropertyDefinition SenderClass = InternalSchema.CreateGuidNameProperty("ItemSenderClass", typeof(short), WellKnownPropertySet.Common, "ItemSenderClass");

		public static readonly GuidNamePropertyDefinition CurrentFolderReason = InternalSchema.CreateGuidNameProperty("ItemCurrentFolderReason", typeof(short), WellKnownPropertySet.Common, "ItemCurrentFolderReason");

		public static readonly StorePropertyDefinition InferenceOLKUserActivityLoggingEnabled = new InferenceOLKUserActivityLoggingEnabledSmartProperty();

		public static readonly SmartPropertyDefinition ActivityItemId = new ActivityObjectIdProperty("ActivityItemId", InternalSchema.ActivityItemIdBytes);

		public static readonly SmartPropertyDefinition ActivityFolderId = new ActivityObjectIdProperty("ActivityFolderId", InternalSchema.ActivityFolderIdBytes);

		internal enum AdditionalPropTags
		{
			AttachCalendarHidden = 2147352587,
			IsContactPhoto = 2147418123,
			StorageQuota = 1736114179,
			LocallyDelivered = 1732575490,
			ContentFilterPCL = 1082392579,
			LastModifierName = 1073348639,
			CreatorEntryId = 1073283330,
			LastModifierEntryId = 1073414402,
			RecipientEntryId = 1610023170,
			DisplayTypeEx = 956628995,
			SenderFlags = 1075380227,
			SentRepresentingFlags = 1075445763,
			UrlCompName = 284360735,
			UrlCompNamePostfix = 241238019,
			ReadReceiptDisplayName = 1076559903,
			ReadReceiptEmailAddress = 1076494367,
			ReadReceiptAddrType = 1076428831,
			ReadReceiptEntryId = 4587778,
			OriginalMessageId = 273023007,
			SenderSmtpAddress = 1560346655,
			SentRepresentingSmtpAddress = 1560412191,
			OriginalSenderSmtpAddress = 1560477727,
			OriginalSentRepresentingSmtpAddress = 1560543263,
			ReadReceiptSmtpAddress = 1560608799,
			OriginalAuthorSmtpAddress = 1560674335,
			ReceivedBySmtpAddress = 1560739871,
			ReceivedRepresentingSmtpAddress = 1560805407,
			ReportingMta = 1746927647,
			RemoteMta = 203489311,
			SendOutlookRecallReport = 1745027083,
			BlockStatus = 278265859,
			UserX509Certificates = 980422914,
			RulesTable = 1071710221,
			MsgHasBeenDelegated = 1071841291,
			MoveToFolderEntryId = 1072955650,
			MoveToStoreEntryId = 1072890114,
			OriginalMessageEntryId = 1715863810,
			OriginalMessageSvrEId = 1732313346,
			SentMailSvrEId = 1732247810,
			ConversationsFolderEntryId = 904659202,
			RuleTriggerHistory = 1072824578,
			ReportEntryId = 4522242,
			RssServerLockStartTime = 1610612739,
			RssServerLockTimeout = 1610678275,
			RssServerLockClientName = 1610743839,
			LegacyScheduleFolderEntryId = 1713242370,
			LegacyShortcutsFolderEntryId = 1714422018,
			HomePostOfficeBox = 979238943,
			OtherPostOfficeBox = 979632159,
			SendReadNotifications = 1722089483,
			VoiceMessageDuration = 1744896003,
			SenderTelephoneNumber = 1744961567,
			VoiceMessageSenderName = 1745027103,
			FaxNumberOfPages = 1745092611,
			VoiceMessageAttachmentOrder = 1745158175,
			CallId = 1745223711,
			InternetReferences = 272171039,
			InReplyTo = 272760863,
			MapiInternetCpid = 1071513603,
			TextAttachmentCharset = 924516383,
			ListHelp = 272826399,
			ListSubscribe = 272891935,
			ListUnsubscribe = 272957471,
			MapiFlagStatus = 277872643,
			IconIndex = 276824067,
			LastVerbExecuted = 276889603,
			LastVerbExecutionTime = 276955200,
			ReplyForwardStatus = 2081095711,
			PopImapPoisonMessageStamp = 2081161247,
			IsHidden = 284426251,
			AttachCalendarFlags = 2147287043,
			AttachCalendarLinkId = 2147090435,
			AttachContentBase = 923861023,
			AttachContentId = 923926559,
			Not822Renderable = 1733492747,
			BodyContentLocation = 269746207,
			Codepage = 1073545219,
			AttachInConflict = 1718353931,
			AppointmentExceptionEndTime = 2147221568,
			AppointmentExceptionStartTime = 2147156032,
			AttachPayloadProviderGuidString = 924385311,
			AttachPayloadClass = 924450847,
			FolderViewWebInfo = 920584450,
			ChildCount = 1714946051,
			ELCFolderComment = 1731395615,
			ELCPolicyIds = 1731330079,
			ExtendedFolderFlags = 920256770,
			AccessRights = 1715011587,
			ArticleId = 237174787,
			SyncCustomState = 2080506114,
			SyncFolderSourceKey = 2080571650,
			SyncFolderChangeKey = 2080637186,
			SyncFolderLastModificationTime = 2080702528,
			SyncState = 2081030402,
			ImapSubscribeList = 1710624799,
			IsContentIndexingEnabled = 240910347,
			AdditionalRenEntryIds = 920129794,
			AdditionalRenEntryIdsEx = 920191234,
			RemindersSearchFolderEntryId = 919929090,
			ElcRootFolderEntryId = 904397058,
			CommunicatorHistoryFolderEntryId = 904462594,
			SyncRootFolderEntryId = 904528130,
			UMVoicemailFolderEntryId = 904593666,
			EHAMigrationFolderEntryId = 904724738,
			UMFaxFolderEntryId = 918487298,
			AllItemsFolderEntryId = 904790274,
			IsProcessed = 2097217547,
			RecipientTrackStatus = 1610547203,
			RecipientTrackStatusTime = 1610285120,
			RecipientSipUri = 1608843295,
			UserConfigurationDictionary = 2080833794,
			UserConfigurationStream = 2080964866,
			UserConfigurationType = 2080768003,
			UserConfigurationXml = 2080899330,
			AdminFolderFlags = 1731002371,
			AttachmentContent = 1718419487,
			ItemColor = 278200323,
			FlagCompleteTime = 277938240,
			DisplayName7Bit = 973013023,
			DisplayType = 956301315,
			NdrStatusCode = 203423747,
			SwappedTodoData = 237830402,
			SwappedTodoStore = 237764866,
			RecipientProposed = 1608581131,
			RecipientProposedStartTime = 1608712256,
			RecipientProposedEndTime = 1608777792,
			RecipientOrder = 1608450051,
			ShortTermEntryIdFromObject = 1718747394,
			AssociatedSearchFolderId = 1749156098,
			AssociatedSearchFolderFlags = 1749549059,
			AssociatedSearchFolderExpiration = 1748631555,
			AssociatedSearchFolderLastUsedTime = 1748238339,
			AssociatedSearchFolderTemplateId = 1749090307,
			AssociatedSearchFolderTag = 1749483523,
			AssociatedSearchFolderStorageType = 1749417987,
			AssociatedSearchFolderDefinition = 1749352706,
			NavigationNodeGroupClassId = 1749156098,
			NavigationNodeOutlookTagId = 1749483523,
			NavigationNodeType = 1749614595,
			NavigationNodeFlags = 1749680131,
			NavigationNodeOrdinal = 1749745922,
			NavigationNodeEntryId = 1749811458,
			NavigationNodeRecordKey = 1749876994,
			NavigationNodeStoreEntryId = 1749942530,
			NavigationNodeClassId = 1750008066,
			NavigationNodeParentGroupClassId = 1750073602,
			NavigationNodeGroupName = 1750138911,
			NavigationNodeGroupSection = 1750204419,
			NavigationNodeCalendarColor = 1750269955,
			NavigationNodeAddressBookEntryId = 1750335746,
			NavigationNodeAddressBookStoreEntryId = 1754333442,
			DelegateNames = 1749684255,
			DelegateEntryIds = 1749356802,
			DelegateEntryIds2 = 1752174850,
			DelegateFlags = 1751846915,
			DelegateFlags2 = 1752240131,
			DelegateBossWantsCopy = 1749155851,
			DelegateBossWantsInfo = 1749745675,
			DelegateDontMail = 1749221387,
			FreeBusyEntryIds = 920916226,
			DeletedOnTime = 1720647744,
			IsSoftDeleted = 1735393291,
			MapiToDoItemFlag = 237699075,
			ScheduleInfoMonthsTentative = 1750142979,
			ScheduleInfoFreeBusyTentative = 1750208770,
			ScheduleInfoMonthsBusy = 1750274051,
			ScheduleInfoFreeBusyBusy = 1750339842,
			ScheduleInfoMonthsOof = 1750405123,
			ScheduleInfoFreeBusyOof = 1750470914,
			ScheduleInfoMonthsMerged = 1750011907,
			ScheduleInfoFreeBusyMerged = 1750077698,
			ScheduleInfoRecipientLegacyDn = 1749614622,
			OutlookFreeBusyMonthCount = 1751711747,
			LocalDirectory = 1747452162,
			MemberEmailLocalDirectory = 1747517471,
			MemberExternalIdLocalDirectory = 1747583007,
			MemberSIDLocalDirectory = 1747648770,
			ContentAggregationSubscriptions = 1765015810,
			ContentAggregationMessageIndex = 1765081346,
			FailedInboundICalAsAttachment = 924581899,
			BodyContentId = 269811743,
			MimeSkeleton = 1693450498,
			LogonRightsOnMailbox = 1736245251,
			MergeMidsetDeleted = 242876674,
			RuleActions = 1719664894,
			RuleCondition = 1719206141,
			ExtendedRuleCondition = 244973826,
			ExtendedRuleSizeLimit = 245039107,
			SystemFolderEntryId = 905773314,
			AppointmentTombstonesId = 1751777538,
			MapiRulesData = 1071710466,
			FolderFlags = 1722286083,
			ResolveMethod = 1072103427,
			ParentFid = 1732837396,
			Fid = 1732771860,
			ActivityId = 1342308355,
			ActivityItemIdBytes = 1342374146,
			ActivityTimeStamp = 1342242880,
			ActivityClientId = 1342504963,
			LogName = 1610678303,
			SentRepresentingSimpleDisplayName = 1076953119,
			SentRepresentingOrgAddressType = 1078001695,
			SentRepresentingOrgEmailAddr = 1078067231,
			SentRepresentingSID = 239993090,
			SentRepresentingGuid = 239141122,
			PartOfContentIndexing = 910491659,
			SenderSID = 239927554,
			SenderGuid = 239075586,
			ParticipantSID = 203686146,
			ParticipantGuid = 203751682,
			SearchIsPartiallyIndexed = 248381451,
			ConversationCreatorSID = 240845058
		}

		internal enum ToDoItemFlags
		{
			IsToDoItem = 1,
			IsFlagSetForRecipient = 8
		}

		[Flags]
		internal enum LogonRightsOnMailboxFlags
		{
			None = 0,
			CanActAsOwner = 1,
			CanSendAs = 2
		}
	}
}
