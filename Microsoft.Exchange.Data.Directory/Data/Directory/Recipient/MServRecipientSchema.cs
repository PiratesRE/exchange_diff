using System;
using System.Net;
using System.Security.Principal;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal class MServRecipientSchema : ObjectSchema
	{
		private static object NameGetter(IPropertyBag propertyBag)
		{
			ulong puid = (ulong)propertyBag[MServRecipientSchema.Puid];
			return ConsumerIdentityHelper.GetCommonNameFromPuid(puid);
		}

		internal static object AliasGetter(IPropertyBag propertyBag)
		{
			bool flag = false;
			if (MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Exo, out flag) == null)
			{
				return null;
			}
			return MServRecipientSchema.NameGetter(propertyBag);
		}

		internal static void AliasSetter(object value, IPropertyBag propertyBag)
		{
			if (!string.IsNullOrEmpty((string)value))
			{
				throw new ArgumentOutOfRangeException("Alias");
			}
		}

		private static object GuidGetter(IPropertyBag propertyBag)
		{
			ulong puid = (ulong)propertyBag[MServRecipientSchema.Puid];
			return ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid);
		}

		private static object NetIdGetter(IPropertyBag propertyBag)
		{
			ulong netID = (ulong)propertyBag[MServRecipientSchema.Puid];
			return new NetID((long)netID);
		}

		private static object ExchangeGuidGetter(IPropertyBag propertyBag)
		{
			ulong puid = (ulong)propertyBag[MServRecipientSchema.Puid];
			return ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid);
		}

		private static object SidGetter(IPropertyBag propertyBag)
		{
			ulong puid = (ulong)propertyBag[MServRecipientSchema.Puid];
			return ConsumerIdentityHelper.GetSecurityIdentifierFromPuid(puid);
		}

		private static object LegacyExchangeDNGetter(IPropertyBag propertyBag)
		{
			ulong puid = (ulong)propertyBag[MServRecipientSchema.Puid];
			return ConsumerIdentityHelper.GetLegacyExchangeDNFromPuid(puid);
		}

		private static object DistinguishedNameGetter(IPropertyBag propertyBag)
		{
			ulong puid = (ulong)propertyBag[MServRecipientSchema.Puid];
			return ConsumerIdentityHelper.GetDistinguishedNameFromPuid(puid);
		}

		private static object ObjectIdGetter(IPropertyBag propertyBag)
		{
			ulong puid = (ulong)propertyBag[MServRecipientSchema.Puid];
			return ConsumerIdentityHelper.GetADObjectIdFromPuid(puid);
		}

		internal static void ObjectIdSetter(object value, IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = value as ADObjectId;
			if (adobjectId == null)
			{
				throw new ArgumentNullException("Id");
			}
			ulong num;
			if (!ConsumerIdentityHelper.TryGetPuidFromGuid(adobjectId.ObjectGuid, out num))
			{
				throw new ArgumentException("Id.ObjectGuid");
			}
			ulong num2;
			if (!ConsumerIdentityHelper.TryGetPuidFromDN(adobjectId.DistinguishedName, out num2))
			{
				throw new ArgumentException("Id.DistinguishedName");
			}
			if (num != num2)
			{
				throw new ArgumentException("Id");
			}
			propertyBag[MServRecipientSchema.Puid] = num2;
		}

		private static object DatabaseGetter(IPropertyBag propertyBag)
		{
			bool flag;
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Exo, out flag);
			if (record == null)
			{
				return null;
			}
			string exoForestFqdn = record.ExoForestFqdn;
			if (!Fqdn.IsValidFqdn(exoForestFqdn))
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculatePropertyGeneric(MServRecipientSchema.Database.Name), MServRecipientSchema.Database, record));
			}
			Guid guid;
			try
			{
				guid = new Guid(record.ExoDatabaseId);
			}
			catch (Exception ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty(MServRecipientSchema.Database.Name, ex.Message), MServRecipientSchema.Database, record), ex);
			}
			return new ADObjectId(guid, exoForestFqdn);
		}

		internal static void DatabaseSetter(object value, IPropertyBag propertyBag)
		{
			MServPropertyDefinition propertyDefinition;
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Exo, true, out propertyDefinition);
			ADObjectId adobjectId = (ADObjectId)value;
			MservRecord value2;
			if (adobjectId == null)
			{
				value2 = null;
			}
			else
			{
				value2 = record.GetUpdatedExoRecord(adobjectId.PartitionFQDN, adobjectId.ObjectGuid.ToString());
			}
			propertyBag[propertyDefinition] = value2;
		}

		private static object IsDeletedGetter(IPropertyBag propertyBag)
		{
			MservRecord mservRecord = (MservRecord)propertyBag[MServRecipientSchema.MservPrimaryRecord];
			MservRecord mservRecord2 = (MservRecord)propertyBag[MServRecipientSchema.MservSecondaryRecord];
			MservRecord mservRecord3 = (MservRecord)propertyBag[MServRecipientSchema.MservSoftDeletedPrimaryRecord];
			return mservRecord == null && mservRecord2 == null && mservRecord3 != null;
		}

		private static object EmailAddressesGetter(IPropertyBag propertyBag)
		{
			ProxyAddressCollection proxyAddressCollection = new ProxyAddressCollection();
			proxyAddressCollection.CopyChangesOnly = true;
			MultiValuedProperty<MservRecord> multiValuedProperty = (MultiValuedProperty<MservRecord>)propertyBag[MServRecipientSchema.MservEmailAddressesRecord];
			foreach (MservRecord mservRecord in multiValuedProperty)
			{
				try
				{
					proxyAddressCollection.Add(new SmtpProxyAddress(mservRecord.Key, true));
				}
				catch (ArgumentException)
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculatePropertyGeneric(MServRecipientSchema.EmailAddresses.Name), MServRecipientSchema.EmailAddresses, mservRecord.Key));
				}
			}
			return proxyAddressCollection;
		}

		internal static void EmailAddressesSetter(object value, IPropertyBag propertyBag)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)value;
			if (proxyAddressCollection == null || proxyAddressCollection.WasCleared)
			{
				throw new MservOperationException(DirectoryStrings.NoResetOrAssignedMvp);
			}
			string puidKey = MServRecipientSchema.GetPuidKey(propertyBag);
			MultiValuedProperty<MservRecord> multiValuedProperty = (MultiValuedProperty<MservRecord>)propertyBag[MServRecipientSchema.MservEmailAddressesRecord];
			foreach (ProxyAddress proxyAddress in proxyAddressCollection.Added)
			{
				bool flag = true;
				MservRecord mservRecord = new MservRecord(proxyAddress.AddressString, 0, null, puidKey, 0);
				foreach (MservRecord record in multiValuedProperty.Added)
				{
					if (mservRecord.SameRecord(record))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					multiValuedProperty.Add(mservRecord);
				}
			}
			foreach (ProxyAddress proxyAddress2 in proxyAddressCollection.Removed)
			{
				foreach (MservRecord mservRecord2 in multiValuedProperty.ToArray())
				{
					if (proxyAddress2.AddressString.Equals(mservRecord2.Key, StringComparison.OrdinalIgnoreCase))
					{
						multiValuedProperty.Remove(mservRecord2);
						break;
					}
				}
			}
		}

		private static object SatchmoClusterIpGetter(IPropertyBag propertyBag)
		{
			bool flag;
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Hotmail, out flag);
			if (record == null || record.HotmailClusterIp == null)
			{
				return null;
			}
			IPAddress result;
			if (!IPAddress.TryParse(record.HotmailClusterIp, out result))
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculatePropertyGeneric(MServRecipientSchema.SatchmoClusterIp.Name), MServRecipientSchema.SatchmoClusterIp, record));
			}
			return result;
		}

		internal static void SatchmoClusterIpSetter(object value, IPropertyBag propertyBag)
		{
			MServPropertyDefinition propertyDefinition;
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Hotmail, true, out propertyDefinition);
			MservRecord value2;
			if (record.HotmailDGroupId == null && value == null)
			{
				value2 = null;
			}
			else
			{
				value2 = record.GetUpdatedHotmailRecord((value == null) ? null : value.ToString(), record.HotmailDGroupId);
			}
			propertyBag[propertyDefinition] = value2;
		}

		private static object SatchmoDGroupGetter(IPropertyBag propertyBag)
		{
			bool flag;
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Hotmail, out flag);
			if (record == null)
			{
				return string.Empty;
			}
			return record.HotmailDGroupId;
		}

		internal static void SatchmoDGroupSetter(object value, IPropertyBag propertyBag)
		{
			MServPropertyDefinition propertyDefinition;
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Hotmail, true, out propertyDefinition);
			MservRecord value2;
			if (record.HotmailClusterIp == null && value == null)
			{
				value2 = null;
			}
			else
			{
				value2 = record.GetUpdatedHotmailRecord(record.HotmailClusterIp, (string)value);
			}
			propertyBag[propertyDefinition] = value2;
		}

		private static object PrimaryMailboxSourceGetter(IPropertyBag propertyBag)
		{
			bool flag;
			MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Exo, out flag);
			if (flag)
			{
				return PrimaryMailboxSourceType.Exo;
			}
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Hotmail, out flag);
			if (!flag)
			{
				return PrimaryMailboxSourceType.None;
			}
			if (record.IsXmr)
			{
				return PrimaryMailboxSourceType.Exo;
			}
			return PrimaryMailboxSourceType.Hotmail;
		}

		internal static void PrimaryMailboxSourceSetter(object value, IPropertyBag propertyBag)
		{
			bool flag;
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Exo, out flag);
			bool flag2;
			MservRecord record2 = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Hotmail, out flag2);
			if (record != null)
			{
				bool isEmpty = record.IsEmpty;
			}
			if (record2 != null)
			{
				bool isEmpty2 = record2.IsEmpty;
			}
			switch ((PrimaryMailboxSourceType)value)
			{
			case PrimaryMailboxSourceType.None:
				if ((flag && !record.IsEmpty) || (flag2 && !record2.IsEmpty))
				{
					throw new DataValidationException(new PropertyValidationError(DirectoryStrings.MailboxPropertiesMustBeClearedFirst(flag ? record : record2), MServRecipientSchema.MservPrimaryRecord, flag ? record : record2));
				}
				return;
			case PrimaryMailboxSourceType.Hotmail:
				if (flag2)
				{
					return;
				}
				if (flag)
				{
					propertyBag[MServRecipientSchema.MservPrimaryRecord] = record.GetUpdatedRecord(7);
					propertyBag[MServRecipientSchema.MservSecondaryRecord] = record2.GetUpdatedRecord(0);
					return;
				}
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotMakePrimary(record2, "Exo"), MServRecipientSchema.MservPrimaryRecord, record2));
			case PrimaryMailboxSourceType.Exo:
				if (flag)
				{
					return;
				}
				if (flag2)
				{
					propertyBag[MServRecipientSchema.MservPrimaryRecord] = record2.GetUpdatedRecord(7);
					propertyBag[MServRecipientSchema.MservSecondaryRecord] = record.GetUpdatedRecord(0);
					return;
				}
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotMakePrimary(record, "Hotmail"), MServRecipientSchema.MservPrimaryRecord, record));
			default:
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ExArgumentOutOfRangeException("PrimaryMailboxSource", value), MServRecipientSchema.PrimaryMailboxSource, value));
			}
		}

		private static object RecipientDisplayTypeGetter(IPropertyBag propertyBag)
		{
			bool flag;
			MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Exo, out flag);
			if (flag)
			{
				return Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.MailboxUser;
			}
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, MservValueFormat.Hotmail, out flag);
			if (flag && record.IsXmr)
			{
				return Microsoft.Exchange.Data.Directory.Recipient.RecipientDisplayType.MailboxUser;
			}
			return null;
		}

		private static GetterDelegate MservFlagGetterDelegate(byte mask, ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(IPropertyBag propertyBag)
			{
				MservRecord mservRecord = (MservRecord)propertyBag[MServRecipientSchema.MservPrimaryRecord];
				if (mservRecord == null)
				{
					return propertyDefinition.DefaultValue;
				}
				bool value = 0 != (mask & mservRecord.Flags);
				return BoxedConstants.GetBool(value);
			};
		}

		private static SetterDelegate MservFlagSetterDelegate(byte mask, ProviderPropertyDefinition propertyDefinition)
		{
			return delegate(object value, IPropertyBag propertyBag)
			{
				MServRecipientSchema.UpdateRecordFlag((bool)value, mask, propertyBag, MServRecipientSchema.MservPrimaryRecord);
				MServRecipientSchema.UpdateRecordFlag((bool)value, mask, propertyBag, MServRecipientSchema.MservSecondaryRecord);
			};
		}

		private static void UpdateRecordFlag(bool value, byte mask, IPropertyBag propertyBag, MServPropertyDefinition propertyDefinition)
		{
			MservRecord mservRecord = (MservRecord)propertyBag[propertyDefinition];
			if (mservRecord != null)
			{
				MservRecord updatedRecordFlag = mservRecord.GetUpdatedRecordFlag(value, mask);
				propertyBag[propertyDefinition] = updatedRecordFlag;
			}
		}

		private static MservRecord GetRecord(IPropertyBag propertyBag, MservValueFormat format, out bool isPrimary)
		{
			MServPropertyDefinition mservPropertyDefinition;
			MservRecord record = MServRecipientSchema.GetRecord(propertyBag, format, false, out mservPropertyDefinition);
			isPrimary = (mservPropertyDefinition == MServRecipientSchema.MservPrimaryRecord);
			return record;
		}

		private static MservRecord GetRecord(IPropertyBag propertyBag, MservValueFormat format, bool createIfMissing, out MServPropertyDefinition recordPropDef)
		{
			MservRecord mservRecord = (MservRecord)propertyBag[MServRecipientSchema.MservPrimaryRecord];
			MservRecord mservRecord2 = (MservRecord)propertyBag[MServRecipientSchema.MservSecondaryRecord];
			if (mservRecord != null && mservRecord.ValueFormat == format)
			{
				recordPropDef = MServRecipientSchema.MservPrimaryRecord;
				return mservRecord;
			}
			if (mservRecord2 != null && mservRecord2.ValueFormat == format)
			{
				recordPropDef = MServRecipientSchema.MservSecondaryRecord;
				return mservRecord2;
			}
			if (!createIfMissing)
			{
				recordPropDef = null;
				return null;
			}
			bool flag = mservRecord == null;
			if (mservRecord != null && mservRecord2 != null)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CrossRecordMismatch(mservRecord, mservRecord2), MServRecipientSchema.MservPrimaryRecord, mservRecord));
			}
			string puidKey = MServRecipientSchema.GetPuidKey(propertyBag);
			byte resourceId = flag ? 0 : 7;
			recordPropDef = (flag ? MServRecipientSchema.MservPrimaryRecord : MServRecipientSchema.MservSecondaryRecord);
			byte flags = 0;
			if (!flag)
			{
				flags = mservRecord.Flags;
			}
			MservRecord mservRecord3 = new MservRecord(puidKey, resourceId, null, null, flags);
			propertyBag[recordPropDef] = mservRecord3;
			return mservRecord3;
		}

		private static string GetPuidKey(IPropertyBag propertyBag)
		{
			object obj = propertyBag[MServRecipientSchema.Puid];
			if (obj == null)
			{
				throw new ArgumentNullException("Puid");
			}
			return MservRecord.KeyFromPuid((ulong)obj);
		}

		public static readonly MServPropertyDefinition MservPrimaryRecord = MServPropertyDefinition.RawRecordPropertyDefinition("MservPrimaryRecord", PropertyDefinitionFlags.None);

		public static readonly MServPropertyDefinition MservSoftDeletedPrimaryRecord = MServPropertyDefinition.RawRecordPropertyDefinition("MservSoftDeletedPrimaryRecord", PropertyDefinitionFlags.None);

		public static readonly MServPropertyDefinition MservCalendarRecord = MServPropertyDefinition.RawRecordPropertyDefinition("MservCalendarRecord", PropertyDefinitionFlags.None);

		public static readonly MServPropertyDefinition PasRecord = MServPropertyDefinition.RawRecordPropertyDefinition("PasRecord", PropertyDefinitionFlags.None);

		public static readonly MServPropertyDefinition MservSecondaryRecord = MServPropertyDefinition.RawRecordPropertyDefinition("MservSecondaryRecord", PropertyDefinitionFlags.None);

		public static readonly MServPropertyDefinition MservSoftDeletedCalendarRecord = MServPropertyDefinition.RawRecordPropertyDefinition("MservSoftDeletedCalendarRecord", PropertyDefinitionFlags.None);

		public static readonly MServPropertyDefinition MservAggregatedGuidsRecord = MServPropertyDefinition.RawRecordPropertyDefinition("MservAggregatedGuidsRecord", PropertyDefinitionFlags.None);

		public static readonly MServPropertyDefinition MservEmailAddressesRecord = MServPropertyDefinition.RawRecordPropertyDefinition("MservEmailAddressesRecord", PropertyDefinitionFlags.MultiValued);

		public static readonly MServPropertyDefinition Puid = new MServPropertyDefinition("Puid", typeof(ulong), PropertyDefinitionFlags.TaskPopulated, 0UL, SimpleProviderPropertyDefinition.None, null, null);

		public static readonly MServPropertyDefinition Id = new MServPropertyDefinition("Id", typeof(ADObjectId), PropertyDefinitionFlags.Calculated, null, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.ObjectIdGetter), new SetterDelegate(MServRecipientSchema.ObjectIdSetter));

		public static readonly MServPropertyDefinition Name = new MServPropertyDefinition("Name", typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.NameGetter), null);

		public static readonly MServPropertyDefinition CommonName = new MServPropertyDefinition("CommonName", typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.NameGetter), null);

		public static readonly MServPropertyDefinition DisplayName = new MServPropertyDefinition("DisplayName", typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.NameGetter), null);

		public static readonly MServPropertyDefinition Alias = new MServPropertyDefinition("Alias", typeof(string), PropertyDefinitionFlags.Calculated, string.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.AliasGetter), new SetterDelegate(MServRecipientSchema.AliasSetter));

		public static readonly MServPropertyDefinition CanonicalName = new MServPropertyDefinition("CanonicalName", typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.NameGetter), null);

		public static readonly MServPropertyDefinition DistinguishedName = new MServPropertyDefinition("DistinguishedName", typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.DistinguishedNameGetter), null);

		public static readonly MServPropertyDefinition Guid = new MServPropertyDefinition("Guid", typeof(Guid), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, System.Guid.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.GuidGetter), null);

		public static readonly MServPropertyDefinition ExchangeObjectId = new MServPropertyDefinition("ExchangeObjectId", typeof(Guid), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, System.Guid.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.GuidGetter), null);

		public static readonly MServPropertyDefinition CorrelationId = new MServPropertyDefinition("CorrelationId", typeof(Guid), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, System.Guid.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.GuidGetter), null);

		public static readonly MServPropertyDefinition NetID = new MServPropertyDefinition("NetID", typeof(NetID), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.NetIdGetter), null);

		public static readonly MServPropertyDefinition ExchangeGuid = new MServPropertyDefinition("ExchangeGuid", typeof(Guid), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, System.Guid.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.ExchangeGuidGetter), null);

		public static readonly MServPropertyDefinition Sid = new MServPropertyDefinition("Sid", typeof(SecurityIdentifier), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.SidGetter), null);

		public static readonly MServPropertyDefinition LegacyExchangeDN = new MServPropertyDefinition("LegacyExchangeDN", typeof(string), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, string.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.Puid
		}, new GetterDelegate(MServRecipientSchema.LegacyExchangeDNGetter), null);

		public static readonly MServPropertyDefinition Database = new MServPropertyDefinition("Database", typeof(ADObjectId), PropertyDefinitionFlags.Calculated, null, new MServPropertyDefinition[]
		{
			MServRecipientSchema.MservPrimaryRecord,
			MServRecipientSchema.MservSecondaryRecord
		}, new GetterDelegate(MServRecipientSchema.DatabaseGetter), new SetterDelegate(MServRecipientSchema.DatabaseSetter));

		public static readonly MServPropertyDefinition IsDeleted = new MServPropertyDefinition("IsDeleted", typeof(bool), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, false, new MServPropertyDefinition[]
		{
			MServRecipientSchema.MservSoftDeletedPrimaryRecord,
			MServRecipientSchema.MservPrimaryRecord,
			MServRecipientSchema.MservSecondaryRecord
		}, new GetterDelegate(MServRecipientSchema.IsDeletedGetter), null);

		public static readonly MServPropertyDefinition EmailAddresses = new MServPropertyDefinition("EmailAddresses", typeof(ProxyAddress), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.Calculated, null, new MServPropertyDefinition[]
		{
			MServRecipientSchema.MservEmailAddressesRecord
		}, new GetterDelegate(MServRecipientSchema.EmailAddressesGetter), new SetterDelegate(MServRecipientSchema.EmailAddressesSetter));

		public static readonly MServPropertyDefinition SatchmoClusterIp = new MServPropertyDefinition("SatchmoClusterIp", typeof(IPAddress), PropertyDefinitionFlags.Calculated, null, new MServPropertyDefinition[]
		{
			MServRecipientSchema.MservPrimaryRecord,
			MServRecipientSchema.MservSecondaryRecord
		}, new GetterDelegate(MServRecipientSchema.SatchmoClusterIpGetter), new SetterDelegate(MServRecipientSchema.SatchmoClusterIpSetter));

		public static readonly MServPropertyDefinition SatchmoDGroup = new MServPropertyDefinition("SatchmoDGroup", typeof(string), PropertyDefinitionFlags.Calculated, string.Empty, new MServPropertyDefinition[]
		{
			MServRecipientSchema.MservPrimaryRecord,
			MServRecipientSchema.MservSecondaryRecord
		}, new GetterDelegate(MServRecipientSchema.SatchmoDGroupGetter), new SetterDelegate(MServRecipientSchema.SatchmoDGroupSetter));

		public static readonly MServPropertyDefinition PrimaryMailboxSource = new MServPropertyDefinition("PrimaryMailboxSource", typeof(PrimaryMailboxSourceType), PropertyDefinitionFlags.Calculated, PrimaryMailboxSourceType.None, new MServPropertyDefinition[]
		{
			MServRecipientSchema.MservPrimaryRecord,
			MServRecipientSchema.MservSecondaryRecord
		}, new GetterDelegate(MServRecipientSchema.PrimaryMailboxSourceGetter), new SetterDelegate(MServRecipientSchema.PrimaryMailboxSourceSetter));

		public static readonly MServPropertyDefinition FblEnabled = new MServPropertyDefinition("FblEnabled", typeof(bool), PropertyDefinitionFlags.Calculated, false, new MServPropertyDefinition[]
		{
			MServRecipientSchema.MservPrimaryRecord,
			MServRecipientSchema.MservSecondaryRecord
		}, MServRecipientSchema.MservFlagGetterDelegate(4, MServRecipientSchema.FblEnabled), MServRecipientSchema.MservFlagSetterDelegate(4, MServRecipientSchema.FblEnabled));

		public static readonly MServPropertyDefinition RecipientDisplayType = new MServPropertyDefinition("RecipientDisplayType", typeof(RecipientDisplayType?), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, new MServPropertyDefinition[]
		{
			MServRecipientSchema.MservPrimaryRecord,
			MServRecipientSchema.MservSecondaryRecord
		}, new GetterDelegate(MServRecipientSchema.RecipientDisplayTypeGetter), null);
	}
}
