using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal sealed class ConsumerMailboxHelper
	{
		public static IConfigDataProvider CreateConsumerOrganizationSession()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromConsumerOrganization();
			return DirectorySessionFactory.NonCacheSessionFactory.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.FullyConsistent, sessionSettings, 71, "CreateConsumerOrganizationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\ConsumerMailbox\\ConsumerMailboxHelper.cs");
		}

		public static void CreateOrUpdateConsumerMailbox(WriteOperationType writeOperationType, PropertyBag propertyValuesBag, IRecipientSession session, Action<string> logAction, Action<string> warningLogAction = null)
		{
			MbxReadMode mbxReadMode = ((IAggregateSession)session).MbxReadMode;
			BackendWriteMode backendWriteMode = ((IAggregateSession)session).BackendWriteMode;
			try
			{
				if (logAction == null)
				{
					throw new ArgumentNullException("logAction");
				}
				if (warningLogAction == null)
				{
					throw new ArgumentNullException("warningLogAction");
				}
				if (!propertyValuesBag.IsModified(ADUserSchema.NetID))
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Property 'ADUserSchema.NetID' must be set", new object[0]));
				}
				ulong puid = ((NetID)propertyValuesBag[ADUserSchema.NetID]).ToUInt64();
				string text = ((NetID)propertyValuesBag[ADUserSchema.NetID]).ToString();
				bool flag = propertyValuesBag.IsModified("MakeExoPrimary") && (bool)propertyValuesBag["MakeExoPrimary"];
				bool flag2 = propertyValuesBag.IsModified("MakeExoSecondary") && (bool)propertyValuesBag["MakeExoSecondary"];
				bool flag3 = propertyValuesBag.IsModified("SkipMigration") && (bool)propertyValuesBag["SkipMigration"];
				ADUser aduser = ConsumerMailboxHelper.ReadUser(session, puid, true);
				if (aduser == null && (writeOperationType == WriteOperationType.Update || writeOperationType == WriteOperationType.RepairUpdate))
				{
					throw new ADNoSuchObjectException(new LocalizedString(string.Format(CultureInfo.CurrentUICulture, "Cannot update object. Mserv entry for PUID: '{0}' not found", new object[]
					{
						text
					})));
				}
				if (aduser != null && writeOperationType == WriteOperationType.Create && !flag2 && (!flag || !flag3))
				{
					throw new ADObjectAlreadyExistsException(new LocalizedString(string.Format(CultureInfo.CurrentUICulture, "Cannot create object. Mserv entry for PUID: '{0}' already exists", new object[]
					{
						text
					})));
				}
				if (aduser != null && aduser.Database != null && propertyValuesBag.IsModified(ADMailboxRecipientSchema.Database) && !aduser.Database.Equals((ADObjectId)propertyValuesBag[ADMailboxRecipientSchema.Database]))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Cannot update database of an existing consumer mailbox in EXO. PUID: '{0}' Existing database: {1}", new object[]
					{
						text,
						aduser.Database
					}));
				}
				if (flag && flag2)
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Cannot set MakeExoPrimary and MakeExoSecondary at the same time. PUID: '{0}'", new object[]
					{
						text
					}));
				}
				if (aduser == null)
				{
					aduser = new ADUser();
				}
				if (flag)
				{
					if (writeOperationType == WriteOperationType.Create && aduser.PrimaryMailboxSource() == PrimaryMailboxSourceType.None)
					{
						logAction("Current state: No mserv footprint - i.e. no mailbox in EXO and Hotmail.");
						logAction("Action requested: MakeExoPrimary: Create a brand new primary consumer mailbox in EXO. ");
					}
					else if (writeOperationType == WriteOperationType.RepairCreate && (aduser.PrimaryMailboxSource() == PrimaryMailboxSourceType.None || aduser.PrimaryMailboxSource() == PrimaryMailboxSourceType.Exo))
					{
						logAction("Current state: Either no mserv footprint exists or a primary mailbox exists in EXO.");
						logAction("Action requested: MakeExoPrimary: Retry creation of primary consumer mailbox in EXO. ");
					}
					else if ((writeOperationType == WriteOperationType.Update || writeOperationType == WriteOperationType.RepairUpdate) && aduser.Database != null && aduser.PrimaryMailboxSource() == PrimaryMailboxSourceType.Hotmail)
					{
						logAction("Current state: Primary mailbox exists in Hotmail and Secondary mailbox exists in EXO.");
						logAction("Action requested: MakeExoPrimary: Switch from hotmail to EXO. ");
					}
					else
					{
						if ((writeOperationType != WriteOperationType.Create && writeOperationType != WriteOperationType.RepairCreate) || aduser.PrimaryMailboxSource() != PrimaryMailboxSourceType.Hotmail || aduser.Database != null || !flag3)
						{
							throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Cannot proceed with MakeExoPrimary operation. Invalid state detected. PUID: '{0}' Current primary mailbox source: {1} Write operation type: {2}", new object[]
							{
								text,
								aduser.PrimaryMailboxSource(),
								writeOperationType
							}));
						}
						logAction("Current state: Primary mailbox exists in Hotmail and no secondary mailbox exists in EXO.");
						logAction("Action requested: MakeExoPrimary: Switch directly from hotmail to EXO w/o migrating email contents and SkipMigration flag is supplied. ");
					}
				}
				if (flag2)
				{
					if (writeOperationType == WriteOperationType.Create && aduser.PrimaryMailboxSource() == PrimaryMailboxSourceType.Hotmail)
					{
						logAction("Current state: An existing primary mailbox in Hotmail.");
						logAction("Action requested: MakeExoSecondary: Create a brand new secondary consumer mailbox in EXO. ");
					}
					else
					{
						if (writeOperationType != WriteOperationType.RepairCreate || aduser.PrimaryMailboxSource() != PrimaryMailboxSourceType.Hotmail)
						{
							throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Invalid state detected. Cannot proceed with MakeExoSecondary operation. A hotmail account must exist for PUID: '{0}'. Current primary mailbox source: {1} Write operation type: {2}", new object[]
							{
								text,
								aduser.PrimaryMailboxSource(),
								writeOperationType
							}));
						}
						logAction("Current state: An existing primary mailbox in Hotmail with or without a secondary mailbox in EXO.");
						logAction("Action requested: MakeExoSecondary: Retry creating a brand new secondary consumer mailbox in EXO. ");
					}
					propertyValuesBag[ADUserSchema.IsMigratedConsumerMailbox] = true;
				}
				if ((flag || flag2) && aduser.Database == null && !propertyValuesBag.IsModified(ADMailboxRecipientSchema.Database))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "You must specify Database for creating a new mailbox. PUID: '{0}'", new object[]
					{
						text
					}));
				}
				if (!flag && !flag2 && aduser.Database == null)
				{
					throw new ADNoSuchObjectException(new LocalizedString(string.Format(CultureInfo.CurrentUICulture, "Cannot perform this operation. No consumer mailbox (primary/secondary) exists for this user in EXO. PUID: '{0}'", new object[]
					{
						text
					})));
				}
				ADRawEntry adrawEntry = new ADRawEntry();
				ADObjectId adobjectId = aduser.Database ?? (propertyValuesBag[ADMailboxRecipientSchema.Database] as ADObjectId);
				if (adobjectId != null && ConsumerMailboxHelper.PopulateStoreOnlyProperties(adrawEntry, puid, propertyValuesBag, true))
				{
					logAction(string.Format("Saving store properties for user - puid: {0}", text));
					AggregationHelper.PerformMbxModification(adobjectId.ObjectGuid, ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid), adrawEntry.propertyBag as ADPropertyBag, false);
				}
				if (ConsumerMailboxHelper.PopulateMservPuidRecordProperties(aduser, puid, flag, flag2, propertyValuesBag))
				{
					logAction(string.Format("Saving PUID records in Mserv for user - puid: {0}", text));
					((IAggregateSession)session).MbxReadMode = MbxReadMode.NoMbxRead;
					((IAggregateSession)session).BackendWriteMode = BackendWriteMode.WriteToMServ;
					session.Save(aduser);
					aduser = ConsumerMailboxHelper.ReadUser(session, puid, true);
				}
				if (writeOperationType == WriteOperationType.RepairCreate || writeOperationType == WriteOperationType.RepairUpdate)
				{
					((IAggregateSession)session).MbxReadMode = MbxReadMode.NoMbxRead;
					((IAggregateSession)session).BackendWriteMode = BackendWriteMode.WriteToMServ;
					ConsumerMailboxHelper.SaveChangesInRepairMode(session, puid, ConsumerMailboxHelper.MServAliasAndOtherRecordProperties, propertyValuesBag, logAction, warningLogAction);
				}
				else if (ConsumerMailboxHelper.PopulateAliasAndOtherRecordProperties(aduser, puid, propertyValuesBag))
				{
					logAction(string.Format("Saving Aliases and other records in Mserv for user - puid: {0}", text));
					((IAggregateSession)session).MbxReadMode = MbxReadMode.NoMbxRead;
					((IAggregateSession)session).BackendWriteMode = BackendWriteMode.WriteToMServ;
					session.Save(aduser);
				}
			}
			finally
			{
				((IAggregateSession)session).MbxReadMode = mbxReadMode;
				((IAggregateSession)session).BackendWriteMode = backendWriteMode;
			}
		}

		public static void RemoveConsumerMailbox(PropertyBag propertyValuesBag, IRecipientSession session, Action<string> logAction)
		{
			MbxReadMode mbxReadMode = ((IAggregateSession)session).MbxReadMode;
			BackendWriteMode backendWriteMode = ((IAggregateSession)session).BackendWriteMode;
			try
			{
				if (logAction == null)
				{
					throw new ArgumentNullException("logAction");
				}
				if (!propertyValuesBag.IsModified(ADUserSchema.NetID))
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "Property 'ADUserSchema.NetID' must be set", new object[0]));
				}
				ulong puid = ((NetID)propertyValuesBag[ADUserSchema.NetID]).ToUInt64();
				string text = ((NetID)propertyValuesBag[ADUserSchema.NetID]).ToString();
				ADUser aduser = ConsumerMailboxHelper.ReadUser(session, puid, false);
				if (aduser == null)
				{
					throw new ADNoSuchObjectException(new LocalizedString(string.Format(CultureInfo.CurrentUICulture, "Cannot update object. Mserv entry for PUID: '{0}' not found", new object[]
					{
						text
					})));
				}
				if (aduser.Database == null)
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "user (puid: {0}) is not a mailbox in EXO. user.Database is null", new object[]
					{
						text
					}));
				}
				bool flag = propertyValuesBag.IsModified("RemoveExoPrimary") && (bool)propertyValuesBag["RemoveExoPrimary"];
				bool flag2 = propertyValuesBag.IsModified("RemoveExoSecondary") && (bool)propertyValuesBag["RemoveExoSecondary"];
				bool flag3 = propertyValuesBag.IsModified("SwitchToSecondary") && (bool)propertyValuesBag["SwitchToSecondary"];
				((IAggregateSession)session).MbxReadMode = MbxReadMode.NoMbxRead;
				((IAggregateSession)session).BackendWriteMode = BackendWriteMode.WriteToMServ;
				if (flag && flag3)
				{
					if (aduser.PrimaryMailboxSource() != PrimaryMailboxSourceType.Exo || aduser.SatchmoClusterIp() == null || aduser.SatchmoDGroup() == null)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Cannot proceed with RemovePrimary operation. Invalid state detected. PUID: '{0}' Current primary mailbox source: {1} SatchmoClusterIp: {2} SatchmoDGroup", new object[]
						{
							text,
							aduser.PrimaryMailboxSource(),
							aduser.SatchmoClusterIp(),
							aduser.SatchmoDGroup()
						}));
					}
					logAction("Current state: Primary mailbox exists in EXO.");
					logAction("Action requested: RemovePrimary: Remove primary consumer mailbox mserv records pointing to EXO. ");
					aduser[ADUserSchema.PrimaryMailboxSource] = PrimaryMailboxSourceType.Hotmail;
					aduser.Database = null;
					aduser.Alias = null;
					session.Save(aduser);
				}
				else
				{
					if (!flag2)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "You must either set: RemoveExoSecondary or both RemoveExoPrimary and SwitchToSecondary as true", new object[0]));
					}
					if (aduser.PrimaryMailboxSource() != PrimaryMailboxSourceType.Hotmail || aduser.Database == null)
					{
						throw new InvalidOperationException(string.Format(CultureInfo.CurrentUICulture, "Cannot proceed with RemoveSecondary operation. Invalid state detected. PUID: '{0}' Current primary mailbox source: {1} Database: {2}", new object[]
						{
							text,
							aduser.PrimaryMailboxSource(),
							aduser.Database
						}));
					}
					logAction("Current state: An existing primary mailbox in Hotmail and secondary mailbox in EXO.");
					logAction("Action requested: RemoveSecondary: Remove Mserv records pointing to existing secondary consumer mailbox in EXO. ");
					aduser.Database = null;
					aduser.Alias = null;
					session.Save(aduser);
				}
			}
			finally
			{
				((IAggregateSession)session).MbxReadMode = mbxReadMode;
				((IAggregateSession)session).BackendWriteMode = backendWriteMode;
			}
		}

		public static bool IsKnownException(Exception exception)
		{
			return exception is TaskInvalidOperationException || exception is TaskArgumentException || exception is ManagementObjectNotFoundException || exception is ManagementObjectAlreadyExistsException || exception is ADDriverStoreAccessPermanentException || exception is ADDriverStoreAccessTransientException;
		}

		public static ADUser ReadUser(IRecipientSession session, ulong puid, bool mservOnly)
		{
			ADUser result;
			try
			{
				((IAggregateSession)session).MbxReadMode = (mservOnly ? MbxReadMode.NoMbxRead : MbxReadMode.OnlyIfLocatorDataAvailable);
				((IAggregateSession)session).BackendWriteMode = BackendWriteMode.NoWrites;
				result = session.FindADUserByExternalDirectoryObjectId(ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid).ToString());
			}
			catch (ADDriverStoreAccessPermanentException ex)
			{
				if (!(ex.InnerException is MapiExceptionMdbOffline) && !(ex.InnerException is MapiExceptionUserInformationNotFound))
				{
					throw;
				}
				((IAggregateSession)session).MbxReadMode = MbxReadMode.NoMbxRead;
				((IAggregateSession)session).BackendWriteMode = BackendWriteMode.NoWrites;
				result = session.FindADUserByExternalDirectoryObjectId(ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid).ToString());
			}
			return result;
		}

		private static bool PopulateStoreOnlyProperties(ADRawEntry user, ulong puid, PropertyBag propertyValuesBag, bool setMbxPropertyDefinition = false)
		{
			return ConsumerMailboxHelper.PopulateConsumerMailboxProperties(user, puid, ConsumerMailboxHelper.StoreProperties, propertyValuesBag, setMbxPropertyDefinition);
		}

		private static bool PopulateAliasAndOtherRecordProperties(ADUser user, ulong puid, PropertyBag propertyValuesBag)
		{
			return ConsumerMailboxHelper.PopulateConsumerMailboxProperties(user, puid, ConsumerMailboxHelper.MServAliasAndOtherRecordProperties, propertyValuesBag, false);
		}

		private static bool PopulateMservPuidRecordProperties(ADUser user, ulong puid, bool makePrimary, bool makeSecondary, PropertyBag propertiesToSet)
		{
			bool result = false;
			user.SetId(ConsumerIdentityHelper.GetADObjectIdFromPuid(puid));
			if (propertiesToSet.IsModified(ADMailboxRecipientSchema.Database))
			{
				user.Database = (ADObjectId)propertiesToSet[ADMailboxRecipientSchema.Database];
				result = true;
			}
			if (makePrimary)
			{
				user[ADUserSchema.PrimaryMailboxSource] = PrimaryMailboxSourceType.Exo;
				result = true;
			}
			else if (makeSecondary)
			{
				user[ADUserSchema.PrimaryMailboxSource] = PrimaryMailboxSourceType.Hotmail;
				result = true;
			}
			return result;
		}

		private static bool PopulateConsumerMailboxProperties(ADRawEntry user, ulong puid, IEnumerable<ADPropertyDefinition> propertiesToSet, PropertyBag propertyValuesBag, bool setMbxPropertyDefinition = false)
		{
			bool result = false;
			if (!setMbxPropertyDefinition)
			{
				user.SetId(ConsumerIdentityHelper.GetADObjectIdFromPuid(puid));
			}
			foreach (ADPropertyDefinition adpropertyDefinition in propertiesToSet)
			{
				if (propertyValuesBag.IsModified(adpropertyDefinition))
				{
					if (adpropertyDefinition.IsMultivalued)
					{
						MultiValuedPropertyBase multiValuedPropertyBase = setMbxPropertyDefinition ? ((MultiValuedPropertyBase)user[adpropertyDefinition.MbxPropertyDefinition]) : ((MultiValuedPropertyBase)user[adpropertyDefinition]);
						MultiValuedPropertyBase multiValuedPropertyBase2 = (MultiValuedPropertyBase)propertyValuesBag[adpropertyDefinition];
						foreach (object item in multiValuedPropertyBase2.Added)
						{
							multiValuedPropertyBase.Add(item);
						}
						foreach (object item2 in multiValuedPropertyBase2.Removed)
						{
							multiValuedPropertyBase.Remove(item2);
						}
					}
					else if (setMbxPropertyDefinition)
					{
						user[adpropertyDefinition.MbxPropertyDefinition] = propertyValuesBag[adpropertyDefinition];
					}
					else
					{
						user[adpropertyDefinition] = propertyValuesBag[adpropertyDefinition];
					}
					result = true;
				}
			}
			return result;
		}

		private static void SaveChangesInRepairMode(IRecipientSession session, ulong puid, IEnumerable<ADPropertyDefinition> propertiesToSet, PropertyBag propertyValuesBag, Action<string> logAction, Action<string> warningLogAction)
		{
			foreach (ADPropertyDefinition adpropertyDefinition in propertiesToSet)
			{
				if (propertyValuesBag.IsModified(adpropertyDefinition))
				{
					if (adpropertyDefinition.IsMultivalued)
					{
						MultiValuedPropertyBase multiValuedPropertyBase = (MultiValuedPropertyBase)propertyValuesBag[adpropertyDefinition];
						foreach (object addedValue in multiValuedPropertyBase.Added)
						{
							ConsumerMailboxHelper.TrySaveChanges(session, puid, adpropertyDefinition, addedValue, null, logAction, warningLogAction);
						}
						foreach (object removedValue in multiValuedPropertyBase.Removed)
						{
							ConsumerMailboxHelper.TrySaveChanges(session, puid, adpropertyDefinition, null, removedValue, logAction, warningLogAction);
						}
					}
					else
					{
						ConsumerMailboxHelper.TrySaveChanges(session, puid, adpropertyDefinition, propertyValuesBag[adpropertyDefinition], null, logAction, warningLogAction);
					}
				}
			}
		}

		private static bool TrySaveChanges(IRecipientSession session, ulong puid, ADPropertyDefinition prop, object addedValue, object removedValue, Action<string> logAction, Action<string> warningLogAction)
		{
			if (addedValue != null && removedValue != null)
			{
				throw new InvalidOperationException("You must set a value to addedValue or removedValue, not both.");
			}
			object obj = addedValue;
			bool result;
			try
			{
				ADUser aduser = new ADUser();
				aduser.SetId(ConsumerIdentityHelper.GetADObjectIdFromPuid(puid));
				if (prop.IsMultivalued)
				{
					if (addedValue != null)
					{
						((MultiValuedPropertyBase)aduser[prop]).Add(addedValue);
					}
					else if (removedValue != null)
					{
						((MultiValuedPropertyBase)aduser[prop]).Remove(removedValue);
						obj = removedValue;
					}
				}
				else
				{
					aduser[prop] = addedValue;
				}
				session.Save(aduser);
				result = true;
			}
			catch (Exception ex)
			{
				logAction(string.Format("Error writing value. PUID: {0} Property: {1} Value: {2} Error: {3}", new object[]
				{
					ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid),
					prop.Name,
					obj ?? "<NULL>",
					ex.ToString()
				}));
				warningLogAction(string.Format("Error writing value. PUID: {0} Property: {1} Value: {2} Error: {3}", new object[]
				{
					ConsumerIdentityHelper.GetExchangeGuidFromPuid(puid),
					prop.Name,
					obj ?? "<NULL>",
					ex.Message
				}));
				result = false;
			}
			return result;
		}

		public const string MakeExoPrimaryProperty = "MakeExoPrimary";

		public const string MakeExoSecondaryProperty = "MakeExoSecondary";

		public const string RemoveExoPrimaryProperty = "RemoveExoPrimary";

		public const string RemoveExoSecondaryProperty = "RemoveExoSecondary";

		public const string SkipMigrationProperty = "SkipMigration";

		public const string SwitchToSecondaryProperty = "SwitchToSecondary";

		public const string WindowsLiveIDProperty = "WindowsLiveID";

		public const string PUIDProperty = "PUID";

		public const string DatabaseProperty = "Database";

		public static readonly IEnumerable<ADPropertyDefinition> MServAliasAndOtherRecordProperties = new List<ADPropertyDefinition>(new ADPropertyDefinition[]
		{
			ADRecipientSchema.EmailAddresses,
			ADUserSchema.FblEnabled
		}).AsReadOnly();

		public static readonly IEnumerable<ADPropertyDefinition> StoreProperties = new List<ADPropertyDefinition>(new ADPropertyDefinition[]
		{
			ADRecipientSchema.Description,
			ADUserSchema.Gender,
			ADUserSchema.Occupation,
			ADUserSchema.Region,
			ADUserSchema.Timezone,
			ADUserSchema.Birthdate,
			ADUserSchema.BirthdayPrecision,
			ADUserSchema.NameVersion,
			ADUserSchema.AlternateSupportEmailAddresses,
			ADUserSchema.PostalCode,
			ADUserSchema.OptInUser,
			ADUserSchema.MigrationDryRun,
			ADUserSchema.IsMigratedConsumerMailbox,
			ADUserSchema.FirstName,
			ADUserSchema.LastName,
			ADRecipientSchema.UsageLocation,
			ADRecipientSchema.EmailAddresses,
			ADUserSchema.LocaleID
		}).AsReadOnly();
	}
}
