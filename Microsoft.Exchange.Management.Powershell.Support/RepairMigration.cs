using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Migration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Migration.Rpc;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Cmdlet("Repair", "Migration", DefaultParameterSetName = "UpdateMigrationUser")]
	public sealed class RepairMigration : MigrationOrganizationTaskBase
	{
		[Parameter(Mandatory = true, ParameterSetName = "EnableConfigFeatures")]
		[Parameter(Mandatory = true, ParameterSetName = "AddOrgUpgradeConstraint")]
		public SwitchParameter Add
		{
			get
			{
				return (SwitchParameter)(base.Fields["Add"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Add"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationReport")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationStoreObject")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationCacheEntry")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationBatch")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationUser")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationFolder")]
		public SwitchParameter Update
		{
			get
			{
				return (SwitchParameter)(base.Fields["Update"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Update"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationFolder")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationBatch")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationUser")]
		[Parameter(Mandatory = true, ParameterSetName = "ExpireOrgUpgradeConstraint")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationReport")]
		[Parameter(Mandatory = true, ParameterSetName = "DisableConfigFeatures")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationStoreObject")]
		public SwitchParameter Remove
		{
			get
			{
				return (SwitchParameter)(base.Fields["Remove"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Remove"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RevertMigrationBatch")]
		[Parameter(Mandatory = true, ParameterSetName = "RevertMigrationUser")]
		public SwitchParameter Revert
		{
			get
			{
				return (SwitchParameter)(base.Fields["Revert"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Revert"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "SyncMigrationUser")]
		public SwitchParameter SyncSubscription
		{
			get
			{
				return (SwitchParameter)(base.Fields["SyncSubscription"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SyncSubscription"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ResumeMigrationUser")]
		public SwitchParameter ResumeSubscription
		{
			get
			{
				return (SwitchParameter)(base.Fields["ResumeSubscription"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ResumeSubscription"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "FlushMigrationUser")]
		public SwitchParameter FlushSubscription
		{
			get
			{
				return (SwitchParameter)(base.Fields["FlushSubscription"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["FlushSubscription"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DisableSubscription")]
		public SwitchParameter DisableSubscription
		{
			get
			{
				return (SwitchParameter)(base.Fields["DisableSubscription"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DisableSubscription"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DeleteSubscription")]
		public SwitchParameter DeleteSubscription
		{
			get
			{
				return (SwitchParameter)(base.Fields["DeleteSubscription"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DeleteSubscription"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateSubscriptionStatus")]
		public SwitchParameter UpdateSyncSubscription
		{
			get
			{
				return (SwitchParameter)(base.Fields["UpdateSyncSubscription"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UpdateSyncSubscription"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoveSubscription")]
		public SwitchParameter RemoveSyncSubscription
		{
			get
			{
				return (SwitchParameter)(base.Fields["RemoveSyncSubscription"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RemoveSyncSubscription"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationCacheEntry")]
		public SwitchParameter CacheEntry
		{
			get
			{
				return (SwitchParameter)(base.Fields["CacheEntry"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CacheEntry"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationUser")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationBatch")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationReport")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationStoreObject")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationBatch")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationUser")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationReport")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationStoreObject")]
		[ValidateNotNullOrEmpty]
		public string Property
		{
			get
			{
				return (string)base.Fields["Property"];
			}
			set
			{
				base.Fields["Property"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationUser")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationBatch")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationReport")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationStoreObject")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationBatch")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationUser")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationReport")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationStoreObject")]
		[ValidateNotNullOrEmpty]
		public string ExtendedProperty
		{
			get
			{
				return (string)base.Fields["ExtendedProperty"];
			}
			set
			{
				base.Fields["ExtendedProperty"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationBatch")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationUser")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateSubscriptionStatus")]
		[ValidateNotNullOrEmpty]
		public string Status
		{
			get
			{
				return (string)base.Fields["Status"];
			}
			set
			{
				base.Fields["Status"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RemoveSubscription")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationBatch")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationUser")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationReport")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateSubscriptionStatus")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationStoreObject")]
		public string PropertyType
		{
			get
			{
				return (string)base.Fields["PropertyType"];
			}
			set
			{
				base.Fields["PropertyType"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationBatch")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationStoreObject")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationUser")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationFolder")]
		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationReport")]
		public object PropertyValue
		{
			get
			{
				return base.Fields["Value"];
			}
			set
			{
				base.Fields["Value"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpdateMigrationStoreObject")]
		[Parameter(Mandatory = false, ParameterSetName = "RemoveMigrationStoreObject")]
		public string Attachment
		{
			get
			{
				return (string)base.Fields["Attachment"];
			}
			set
			{
				base.Fields["Attachment"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "RevertMigrationBatch")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationBatch")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationBatch")]
		public MigrationBatchIdParameter BatchId
		{
			get
			{
				return (MigrationBatchIdParameter)base.Fields["BatchId"];
			}
			set
			{
				base.Fields["BatchId"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationStoreObject")]
		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationStoreObject")]
		public string StoreObjectId
		{
			get
			{
				return (string)base.Fields["StoreObjectId"];
			}
			set
			{
				base.Fields["StoreObjectId"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RevertMigrationUser")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationUser")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationUser")]
		[Parameter(Mandatory = true, ParameterSetName = "DisableSubscription")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveSubscription")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateSubscriptionStatus")]
		[Parameter(Mandatory = true, ParameterSetName = "FlushMigrationUser")]
		[Parameter(Mandatory = true, ParameterSetName = "ResumeMigrationUser")]
		[Parameter(Mandatory = true, ParameterSetName = "SyncMigrationUser")]
		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "DeleteSubscription")]
		public MigrationUserIdParameter UserId
		{
			get
			{
				return (MigrationUserIdParameter)base.Fields["UserId"];
			}
			set
			{
				base.Fields["UserId"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "RemoveSubscription")]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateSubscriptionStatus")]
		[ValidateNotNull]
		[ValidateNotEmptyGuid]
		public Guid? SubscriptionId
		{
			get
			{
				return (Guid?)base.Fields["SubscriptionId"];
			}
			set
			{
				base.Fields["SubscriptionId"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationFolder")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationFolder")]
		[ValidateNotNull]
		public MigrationFolderName? FolderId
		{
			get
			{
				return (MigrationFolderName?)base.Fields["FolderId"];
			}
			set
			{
				base.Fields["FolderId"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "UpdateMigrationReport")]
		[Parameter(Mandatory = true, ParameterSetName = "RemoveMigrationReport")]
		public MigrationReportId ReportId
		{
			get
			{
				return (MigrationReportId)base.Fields["ReportId"];
			}
			set
			{
				base.Fields["ReportId"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExpireOrgUpgradeConstraint")]
		public DateTime? ConstraintExpiryDate
		{
			get
			{
				return (DateTime?)base.Fields["ConstraintExpiryDate"];
			}
			set
			{
				base.Fields["ConstraintExpiryDate"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "AddOrgUpgradeConstraint")]
		[Parameter(Mandatory = true, ParameterSetName = "ExpireOrgUpgradeConstraint")]
		public SwitchParameter UpgradeConstraint
		{
			get
			{
				return (SwitchParameter)(base.Fields["UpgradeConstraint"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UpgradeConstraint"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "EnableConfigFeatures")]
		[Parameter(Mandatory = true, ParameterSetName = "DisableConfigFeatures")]
		public MigrationFeature? Feature
		{
			get
			{
				return (MigrationFeature?)base.Fields["Feature"];
			}
			set
			{
				base.Fields["Feature"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRepairMigration;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!string.IsNullOrEmpty(this.Status))
			{
				if (!string.IsNullOrEmpty(this.Property) || !string.IsNullOrEmpty(this.ExtendedProperty) || !string.IsNullOrEmpty(this.PropertyType))
				{
					this.WriteError(Strings.ValidateRepairUpdateStatus);
					return;
				}
			}
			else if (!string.IsNullOrEmpty(this.Property) || !string.IsNullOrEmpty(this.ExtendedProperty))
			{
				if (this.Update)
				{
					if (!base.Fields.IsChanged("Value"))
					{
						this.WriteInternalError("Please pass in a value when updating a property");
						return;
					}
					if (!string.IsNullOrEmpty(this.ExtendedProperty) && string.IsNullOrEmpty(this.PropertyType))
					{
						this.WriteInternalError(string.Format("Please pass in a type when updating an extended property '{0}'", this.ExtendedProperty));
						return;
					}
					if (string.IsNullOrEmpty(this.ExtendedProperty) && !string.IsNullOrEmpty(this.PropertyType))
					{
						this.WriteInternalError("Type is not needed when setting a regular property.  It's pulled form the property definition");
						return;
					}
				}
				else if (this.Remove)
				{
					if (!base.Fields.IsChanged("Value"))
					{
						this.WriteInternalError("the Value isn't needed when removing a property");
						return;
					}
					if (!string.IsNullOrEmpty(this.PropertyType))
					{
						this.WriteInternalError("the Type isn't needed when removing a property");
						return;
					}
				}
			}
			else if (this.Update && !this.CacheEntry && string.IsNullOrEmpty(this.Attachment))
			{
				this.WriteError(Strings.ValidateRepairUpdateMissingStatus);
			}
		}

		private bool ShouldRepairContinue(LocalizedString message)
		{
			return this.Force || base.ShouldContinue(message);
		}

		protected override void InternalProcessRecord()
		{
			Action updateOperation = delegate()
			{
				if (this.SubscriptionId != null)
				{
					this.InternalProcessSubscription(this.SubscriptionId.Value);
					return;
				}
				if (this.CacheEntry)
				{
					if (!this.ShouldRepairContinue(Strings.ConfirmRepairUpdateCacheEntry(base.Organization.ToString())))
					{
						return;
					}
					this.UpdateCacheEntry();
					return;
				}
				else
				{
					if (this.BatchId != null)
					{
						MigrationJob migrationJob = this.GetMigrationJob(null);
						if (migrationJob == null)
						{
							this.WriteError(Strings.MigrationJobNotFound(this.BatchId.ToString()));
						}
						this.InternalProcessJob(migrationJob);
						return;
					}
					if (this.UserId != null)
					{
						if (this.Remove && string.IsNullOrEmpty(this.Property) && string.IsNullOrEmpty(this.ExtendedProperty))
						{
							if (!this.ShouldRepairContinue(Strings.ConfirmRepairRemoveUsers(this.UserId.ToString())))
							{
								return;
							}
							using (List<MigrationJobItem>.Enumerator enumerator = this.GetMigrationJobItems().GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									MigrationJobItem migrationJobItem = enumerator.Current;
									this.WriteWarning(Strings.WarnRepairRemovingUser(migrationJobItem.ToString()));
									migrationJobItem.Delete(base.DataProvider);
								}
								return;
							}
						}
						MigrationJobItem migrationJobItem2 = this.GetMigrationJobItem();
						this.InternalProcessJobItem(migrationJobItem2);
						return;
					}
					if (this.FolderId != null)
					{
						this.InternalProcessFolder(this.FolderId.Value);
						return;
					}
					if (this.ReportId != null)
					{
						if (!this.ShouldRepairContinue(Strings.ConfirmRepairRemoveReport(this.ReportId.ToString())))
						{
							return;
						}
						this.RemoveReportItem(this.ReportId);
						return;
					}
					else
					{
						if (this.StoreObjectId != null)
						{
							this.InternalProcessStoreObject();
							return;
						}
						if (this.UpgradeConstraint)
						{
							this.UpdateOrganizationUpgradeConstraint(this.Add);
							return;
						}
						if (this.Feature != null)
						{
							if (this.Add)
							{
								this.EnableMigrationFeatures(this.Feature.Value);
								return;
							}
							if (this.Remove)
							{
								this.DisableMigrationFeatures(this.Feature.Value);
							}
						}
					}
					return;
				}
			};
			try
			{
				MigrationHelper.RunUpdateOperation(updateOperation);
			}
			catch (LocalizedException exception)
			{
				this.WriteError(exception);
			}
		}

		private bool TryUpdateMigrationReport(MigrationPersistableBase migrationObject, string msg)
		{
			MigrationJob migrationJob = migrationObject as MigrationJob;
			if (migrationJob == null)
			{
				return false;
			}
			migrationJob.ReportData.Append(Strings.MigrationReportRepaired(base.ExecutingUserIdentityName, msg));
			base.BatchProvider.MailboxProvider.FlushReport(migrationJob.ReportData);
			return true;
		}

		private bool TryGetPropertyDefinition(out PropertyDefinition propertyDefinition)
		{
			propertyDefinition = null;
			if (string.IsNullOrEmpty(this.Property))
			{
				return false;
			}
			Type typeFromHandle = typeof(MigrationBatchMessageSchema);
			FieldInfo field = typeFromHandle.GetField(this.Property, BindingFlags.Static | BindingFlags.Public);
			if (field == null)
			{
				this.WriteInternalError(string.Format("couldn't find property {0} in MigrationBatchMessageSchema", this.Property));
			}
			try
			{
				propertyDefinition = (PropertyDefinition)field.GetValue(null);
				return true;
			}
			catch (NotSupportedException arg)
			{
				this.WriteInternalError(string.Format("couldn't retrieve property value {0} ex:{1}", this.Property, arg));
			}
			catch (FieldAccessException arg2)
			{
				this.WriteInternalError(string.Format("couldn't retrieve property value {0} ex:{1}", this.Property, arg2));
			}
			catch (ArgumentException arg3)
			{
				this.WriteInternalError(string.Format("couldn't retrieve property value {0} ex:{1}", this.Property, arg3));
			}
			return false;
		}

		private Type GetPropertyType()
		{
			if (string.IsNullOrEmpty(this.PropertyType))
			{
				return null;
			}
			Type result = null;
			try
			{
				result = Type.GetType(this.PropertyType, false, false);
			}
			catch (ArgumentException arg)
			{
				this.WriteInternalError(string.Format("couldn't extract type from {0} {1}", this.PropertyType, arg));
			}
			catch (TargetInvocationException arg2)
			{
				this.WriteInternalError(string.Format("couldn't extract type from {0} {1}", this.PropertyType, arg2));
			}
			catch (TypeLoadException arg3)
			{
				this.WriteInternalError(string.Format("couldn't extract type from {0} {1}", this.PropertyType, arg3));
			}
			return result;
		}

		private object GetConvertedValue(Type propertyType)
		{
			MigrationUtil.ThrowOnNullArgument(propertyType, "propertyType");
			object result = null;
			try
			{
				if (this.PropertyValue != null && this.PropertyValue.GetType() == propertyType)
				{
					return this.PropertyValue;
				}
				if (propertyType == typeof(byte[]) && this.PropertyValue is string)
				{
					try
					{
						base.WriteVerbose(new LocalizedString("attempting to convert from base64 string."));
						result = Convert.FromBase64String((string)this.PropertyValue);
						goto IL_161;
					}
					catch (FormatException arg)
					{
						base.WriteVerbose(new LocalizedString("attempting to convert from an ascii string:" + arg));
						ASCIIEncoding asciiencoding = new ASCIIEncoding();
						result = asciiencoding.GetBytes((string)this.PropertyValue);
						goto IL_161;
					}
				}
				if (propertyType == typeof(ExDateTime) && this.PropertyValue is string)
				{
					result = ExDateTime.Parse((string)this.PropertyValue);
				}
				else
				{
					if (propertyType == typeof(string[]))
					{
						object[] array = (object[])this.PropertyValue;
						string[] array2 = new string[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							array2[i] = (string)array[i];
						}
						return array2;
					}
					TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
					if (converter == null)
					{
						this.WriteInternalError(string.Format("no converter for {0}", propertyType));
					}
					result = converter.ConvertFrom(this.PropertyValue);
				}
				IL_161:;
			}
			catch (ArgumentException arg2)
			{
				this.WriteInternalError(string.Format("couldn't convert value {0} to {1} ex:{2}", this.PropertyValue, propertyType, arg2));
			}
			catch (FormatException arg3)
			{
				this.WriteInternalError(string.Format("couldn't convert value {0} to {1} ex:{2}", this.PropertyValue, propertyType, arg3));
			}
			catch (NotSupportedException arg4)
			{
				this.WriteInternalError(string.Format("couldn't convert value {0} to {1} ex:{2}", this.PropertyValue, propertyType, arg4));
			}
			return result;
		}

		private void InternalProcessStoreObject()
		{
			StoreObjectId storeObjectId = Microsoft.Exchange.Data.Storage.StoreObjectId.Deserialize(this.StoreObjectId);
			PropertyDefinition[] array = MigrationStoreObject.IdPropertyDefinition;
			PropertyDefinition propertyDefinition = null;
			if (!string.IsNullOrEmpty(this.Property))
			{
				if (!this.TryGetPropertyDefinition(out propertyDefinition))
				{
					return;
				}
				array = MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
				{
					new PropertyDefinition[]
					{
						propertyDefinition
					},
					MigrationStoreObject.IdPropertyDefinition
				});
			}
			if (Folder.IsFolderId(storeObjectId))
			{
				using (Folder folder = Folder.Bind(base.DataProvider.MailboxSession, storeObjectId, array))
				{
					using (MigrationFolder migrationFolder = new MigrationFolder(folder))
					{
						this.InternalProcessStoreObject(migrationFolder, propertyDefinition, null);
					}
					return;
				}
			}
			using (MigrationMessageItem migrationMessageItem = new MigrationMessageItem(base.DataProvider, storeObjectId, array))
			{
				this.InternalProcessStoreObject(migrationMessageItem, propertyDefinition, null);
			}
		}

		private bool InternalProcessPersistable(MigrationPersistableBase migrationObject)
		{
			PropertyDefinition migrationPersistableDictionary;
			if (!this.TryGetPropertyDefinition(out migrationPersistableDictionary))
			{
				if (string.IsNullOrEmpty(this.ExtendedProperty))
				{
					return false;
				}
				migrationPersistableDictionary = MigrationBatchMessageSchema.MigrationPersistableDictionary;
			}
			PropertyDefinition[] properties = new PropertyDefinition[]
			{
				migrationPersistableDictionary
			};
			using (IMigrationStoreObject migrationStoreObject = migrationObject.FindStoreObject(base.DataProvider, properties))
			{
				this.InternalProcessStoreObject(migrationStoreObject, migrationPersistableDictionary, migrationObject);
			}
			return true;
		}

		private void InternalProcessStoreObject(IMigrationStoreObject storeObject, PropertyDefinition propertyDefinition, MigrationPersistableBase migrationObject)
		{
			storeObject.OpenAsReadWrite();
			string text = (migrationObject != null) ? migrationObject.ToString() : storeObject.Id.ToBase64String();
			bool flag = false;
			if (this.Remove)
			{
				if (!string.IsNullOrEmpty(this.Attachment))
				{
					IMigrationMessageItem migrationMessageItem = storeObject as IMigrationMessageItem;
					if (migrationMessageItem == null)
					{
						this.WriteInternalError("can only remove attachments for messages");
					}
					IMigrationAttachment attachment = migrationMessageItem.GetAttachment(this.Attachment, PropertyOpenMode.ReadOnly);
					using (StreamReader streamReader = new StreamReader(attachment.Stream))
					{
						string value = streamReader.ReadToEnd();
						if (this.ShouldRepairContinue(Strings.ConfirmRepairRemoveProperty(string.Format("Attachment={0}", this.Attachment), value, text)))
						{
							migrationMessageItem.DeleteAttachment(this.Attachment);
							flag = true;
						}
						goto IL_4CF;
					}
				}
				if (!string.IsNullOrEmpty(this.ExtendedProperty))
				{
					PersistableDictionary dictionaryProperty = MigrationHelper.GetDictionaryProperty(storeObject, propertyDefinition, true);
					if (!dictionaryProperty.Contains(this.ExtendedProperty))
					{
						this.WriteInternalError(string.Format("extended property {0} not found in {1}", this.ExtendedProperty, text));
					}
					else if (this.ShouldRepairContinue(Strings.ConfirmRepairRemoveProperty(this.ExtendedProperty.ToString(), dictionaryProperty[this.ExtendedProperty].ToString(), text)))
					{
						object arg = dictionaryProperty[this.ExtendedProperty];
						dictionaryProperty.Remove(this.ExtendedProperty);
						MigrationHelper.SetDictionaryProperty(storeObject, propertyDefinition, dictionaryProperty);
						flag = true;
						string msg = string.Format("removed extended property {0} with value {1} from property {2}", this.ExtendedProperty, arg, propertyDefinition);
						this.TryUpdateMigrationReport(migrationObject, msg);
					}
				}
				else if (!string.IsNullOrEmpty(this.Property))
				{
					object valueOrDefault = storeObject.GetValueOrDefault<object>(propertyDefinition, null);
					if (valueOrDefault == null)
					{
						this.WriteInternalError(string.Format("property {0} not found in {1}", this.Property, text));
					}
					else if (this.ShouldRepairContinue(Strings.ConfirmRepairRemoveProperty(this.Property.ToString(), valueOrDefault.ToString(), text)))
					{
						storeObject.Delete(propertyDefinition);
						flag = true;
						this.TryUpdateMigrationReport(migrationObject, string.Format("removed property {0} with value {1}", this.Property, valueOrDefault));
					}
				}
				else if (this.ShouldRepairContinue(Strings.ConfirmRepairRemoveStoreObject(storeObject.Id.ToString())))
				{
					base.DataProvider.RemoveMessage(storeObject.Id);
				}
			}
			else if (this.Update)
			{
				if (!string.IsNullOrEmpty(this.Attachment))
				{
					IMigrationMessageItem migrationMessageItem2 = storeObject as IMigrationMessageItem;
					if (migrationMessageItem2 == null)
					{
						this.WriteInternalError("can only remove attachments for messages");
					}
					IMigrationAttachment migrationAttachment;
					if (migrationMessageItem2.TryGetAttachment(this.Attachment, PropertyOpenMode.ReadOnly, out migrationAttachment))
					{
						this.WriteInternalError(string.Format("need to remove existing attachment {0} before updating/creating", this.Attachment));
					}
					string text2 = (string)this.GetConvertedValue(typeof(string));
					if (this.ShouldRepairContinue(Strings.ConfirmRepairUpdateProperty(string.Format("Attachment={0}", this.Attachment), null, text2, text)))
					{
						IMigrationAttachment migrationAttachment2;
						migrationAttachment = (migrationAttachment2 = migrationMessageItem2.CreateAttachment(this.Attachment));
						try
						{
							using (StreamWriter streamWriter = new StreamWriter(migrationAttachment.Stream))
							{
								streamWriter.Write(text2);
								streamWriter.Flush();
							}
							migrationAttachment.Save(null);
						}
						finally
						{
							if (migrationAttachment2 != null)
							{
								migrationAttachment2.Dispose();
							}
						}
						flag = true;
					}
				}
				else if (!string.IsNullOrEmpty(this.ExtendedProperty))
				{
					Type propertyType = this.GetPropertyType();
					if (propertyType == null)
					{
						this.WriteInternalError(string.Format("couldn't find type {0} for updating property", this.PropertyType));
					}
					object convertedValue = this.GetConvertedValue(propertyType);
					if (convertedValue == null)
					{
						this.WriteInternalError(string.Format("couldn't convert value {0} to type {1}", this.PropertyValue, propertyType));
					}
					PersistableDictionary dictionaryProperty2 = MigrationHelper.GetDictionaryProperty(storeObject, propertyDefinition, false);
					object obj = null;
					if (dictionaryProperty2.Contains(this.ExtendedProperty))
					{
						obj = dictionaryProperty2[this.ExtendedProperty];
					}
					if (this.ShouldRepairContinue(Strings.ConfirmRepairUpdateProperty(this.ExtendedProperty, (obj == null) ? string.Empty : obj.ToString(), (convertedValue == null) ? string.Empty : convertedValue.ToString(), text)))
					{
						dictionaryProperty2[this.ExtendedProperty] = convertedValue;
						MigrationHelper.SetDictionaryProperty(storeObject, propertyDefinition, dictionaryProperty2);
						flag = true;
						string msg2 = string.Format("setting extended property {0} with value {1} to value {2} from property {3}", new object[]
						{
							this.ExtendedProperty,
							obj,
							convertedValue,
							propertyDefinition
						});
						this.TryUpdateMigrationReport(migrationObject, msg2);
					}
				}
				else
				{
					object convertedValue2 = this.GetConvertedValue(propertyDefinition.Type);
					if (convertedValue2 == null)
					{
						this.WriteInternalError(string.Format("couldn't convert value {0} to type {1}", this.PropertyValue, propertyDefinition.Type));
					}
					object valueOrDefault2 = storeObject.GetValueOrDefault<object>(propertyDefinition, null);
					if (this.ShouldRepairContinue(Strings.ConfirmRepairUpdateProperty(this.Property.ToString(), (valueOrDefault2 == null) ? string.Empty : valueOrDefault2.ToString(), (convertedValue2 == null) ? string.Empty : convertedValue2.ToString(), text)))
					{
						storeObject[propertyDefinition] = convertedValue2;
						flag = true;
						this.TryUpdateMigrationReport(migrationObject, string.Format("setting property {0} with value {1} to value {2}", propertyDefinition, valueOrDefault2, convertedValue2));
					}
				}
			}
			IL_4CF:
			if (flag)
			{
				base.WriteVerbose(new LocalizedString("saving store object..."));
				storeObject.Save(SaveMode.FailOnAnyConflict);
			}
		}

		private void InternalProcessFolder(MigrationFolderName folder)
		{
			if (!this.Remove || !string.IsNullOrEmpty(this.ExtendedProperty) || !string.IsNullOrEmpty(this.Property))
			{
				if (folder == MigrationFolderName.SyncMigration)
				{
					this.InternalProcessPersistable(base.BatchProvider.MigrationSession);
				}
				return;
			}
			if (!this.ShouldRepairContinue(Strings.ConfirmRepairRemoveFolder(folder.ToString())))
			{
				return;
			}
			MigrationFolder.RemoveFolder(base.DataProvider.MailboxSession, folder);
		}

		private void InternalProcessJob(MigrationJob job)
		{
			if (this.InternalProcessPersistable(job))
			{
				return;
			}
			if (this.Update)
			{
				if (!this.ShouldRepairContinue(Strings.ConfirmRepairBatch("Update", job.ToString())))
				{
					return;
				}
				this.SetJobStatus(job);
				return;
			}
			else
			{
				if (!this.Remove)
				{
					if (this.Revert)
					{
						if (!this.ShouldRepairContinue(Strings.ConfirmRepairBatch("Revert", job.ToString())))
						{
							return;
						}
						this.RevertJob(job, MigrationJobStatus.Failed);
					}
					return;
				}
				if (!this.ShouldRepairContinue(Strings.ConfirmRepairBatch("Remove", job.ToString())))
				{
					return;
				}
				job.Delete(base.DataProvider, true);
				return;
			}
		}

		private void InternalProcessJobItem(MigrationJobItem jobItem)
		{
			if (this.InternalProcessPersistable(jobItem))
			{
				return;
			}
			if (this.Update)
			{
				if (!this.ShouldRepairContinue(Strings.ConfirmRepairUser("Update", jobItem.ToString())))
				{
					return;
				}
				this.SetJobItemStatus(jobItem);
				return;
			}
			else if (this.Remove)
			{
				if (!this.ShouldRepairContinue(Strings.ConfirmRepairUser("Remove", jobItem.ToString())))
				{
					return;
				}
				jobItem.Delete(base.DataProvider);
				return;
			}
			else
			{
				if (!this.Revert)
				{
					if (this.SyncSubscription)
					{
						if (!this.ShouldRepairContinue(Strings.ConfirmRepairUser("SyncSubscription", jobItem.ToString())))
						{
							return;
						}
						if (jobItem.IsPAW)
						{
							return;
						}
						using (ILegacySubscriptionHandler legacySubscriptionHandler = this.GetLegacySubscriptionHandler(jobItem))
						{
							legacySubscriptionHandler.CreateUnderlyingSubscriptions(jobItem);
							return;
						}
					}
					if (this.ResumeSubscription)
					{
						if ((jobItem.MigrationType == MigrationType.IMAP && !this.ShouldRepairContinue(Strings.ConfirmRepairResumeIMAPSubscription(jobItem.ToString()))) || !this.ShouldRepairContinue(Strings.ConfirmRepairUser("ResumeSubscription", jobItem.ToString())))
						{
							return;
						}
						if (jobItem.IsPAW)
						{
							return;
						}
						using (ILegacySubscriptionHandler legacySubscriptionHandler2 = this.GetLegacySubscriptionHandler(jobItem))
						{
							MigrationUserStatus startedStatus = (jobItem.InitialSyncDuration != null) ? MigrationUserStatus.IncrementalSyncing : MigrationUserStatus.Syncing;
							legacySubscriptionHandler2.ResumeUnderlyingSubscriptions(startedStatus, jobItem);
							return;
						}
					}
					if (this.FlushSubscription)
					{
						if (!this.ShouldRepairContinue(Strings.ConfirmRepairUser("FlushSubscription", jobItem.ToString())))
						{
							return;
						}
						if (jobItem.IsPAW)
						{
							return;
						}
						using (ILegacySubscriptionHandler legacySubscriptionHandler3 = this.GetLegacySubscriptionHandler(jobItem))
						{
							legacySubscriptionHandler3.SyncToUnderlyingSubscriptions(jobItem);
							return;
						}
					}
					if (this.DisableSubscription)
					{
						if (!this.ShouldRepairContinue(Strings.ConfirmRepairUser("DisableSubscription", jobItem.ToString())))
						{
							return;
						}
						if (jobItem.IsPAW)
						{
							return;
						}
						using (ILegacySubscriptionHandler legacySubscriptionHandler4 = this.GetLegacySubscriptionHandler(jobItem))
						{
							legacySubscriptionHandler4.DisableSubscriptions(jobItem);
							return;
						}
					}
					if (this.DeleteSubscription)
					{
						if (!this.ShouldRepairContinue(Strings.ConfirmRepairUser("DeleteSubscription", jobItem.ToString())))
						{
							return;
						}
						if (jobItem.IsPAW)
						{
							return;
						}
						using (ILegacySubscriptionHandler legacySubscriptionHandler5 = this.GetLegacySubscriptionHandler(jobItem))
						{
							legacySubscriptionHandler5.DeleteUnderlyingSubscriptions(jobItem);
						}
					}
					return;
				}
				if (!this.ShouldRepairContinue(Strings.ConfirmRepairUser("Revert", jobItem.ToString())))
				{
					return;
				}
				this.RevertJobItem(jobItem, MigrationUserStatus.Corrupted);
				return;
			}
		}

		private void InternalProcessSubscription(Guid subscriptionId)
		{
			if (this.Update)
			{
				UpdateSyncSubscriptionAction updateSyncSubscriptionAction;
				if (!Enum.TryParse<UpdateSyncSubscriptionAction>(this.Status, false, out updateSyncSubscriptionAction))
				{
					this.WriteInternalError(string.Format("couldn't convert status {0} to type {1}", this.Status, updateSyncSubscriptionAction.GetType()));
				}
				this.UpdateTransportSyncSubscription(subscriptionId, updateSyncSubscriptionAction);
				return;
			}
			if (this.RemoveSyncSubscription)
			{
				this.UpdateTransportSyncSubscription(subscriptionId, UpdateSyncSubscriptionAction.Delete);
			}
		}

		private void WriteError(LocalizedString errorString)
		{
			this.WriteError(new MigrationPermanentException(errorString, "error running repair migration"));
		}

		private void WriteInternalError(string errorString)
		{
			this.WriteError(new MigrationPermanentException(new LocalizedString(errorString), "error running repair migration"));
		}

		private MigrationJob GetMigrationJob(Guid? id)
		{
			MigrationJob migrationJob = null;
			if (id != null)
			{
				base.WriteVerbose(new LocalizedString(string.Format("looking for a job based on guid {0}", id.Value)));
				migrationJob = MigrationJob.GetUniqueByJobId(base.DataProvider, id.Value);
			}
			else if (this.BatchId != null && this.BatchId.MigrationBatchId != null && this.BatchId.MigrationBatchId.Name != MigrationBatchId.Any.ToString())
			{
				base.WriteVerbose(new LocalizedString(string.Format("searching for job with name {0}", this.BatchId)));
				migrationJob = MigrationJob.GetUniqueByBatchId(base.DataProvider, this.BatchId.MigrationBatchId);
			}
			if (migrationJob == null)
			{
				string value = "No batch id, so not scoped to job.  If there's more than one job, please run get-migration(Batch|Job) to get job names";
				base.WriteVerbose(new LocalizedString(value));
				List<MigrationJob> list = new List<MigrationJob>(MigrationJob.Get(base.DataProvider, base.BatchProvider.MigrationSession.Config));
				if (list.Count == 1)
				{
					base.WriteVerbose(new LocalizedString(string.Format("Found a single job {0}", migrationJob)));
					migrationJob = list[0];
				}
			}
			if (migrationJob == null)
			{
				base.WriteVerbose(new LocalizedString("no job was found.."));
			}
			return migrationJob;
		}

		private void RevertJob(MigrationJob job, MigrationJobStatus status)
		{
			if (job.IsPAW)
			{
				this.WriteError(Strings.ValidateRepairInvalidRevertJobType(job.ToString()));
			}
			if (job.Status != status)
			{
				this.WriteError(Strings.ValidateRepairInvalidRevert(job.ToString(), job.Status.ToString()));
			}
			MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(job.StatusData);
			if (migrationStatusData.RevertStatus())
			{
				base.WriteVerbose(new LocalizedString(string.Format("Reverting MigrationJob {0}", job)));
				job.SetStatusData(base.DataProvider, migrationStatusData);
				if (job.PoisonCount > 0)
				{
					base.WriteVerbose(new LocalizedString(string.Format("Clearing poison count of {0}", job.PoisonCount)));
					job.UpdatePoisonCount(base.DataProvider, 0);
				}
				this.TryUpdateMigrationReport(job, "Corruption reverted");
				return;
			}
			this.WriteError(Strings.ErrorRepairReverting(job.ToString()));
		}

		private void SetJobStatus(MigrationJob job)
		{
			if (job.IsPAW)
			{
				MigrationState migrationState;
				if (!Enum.TryParse<MigrationState>(this.Status, false, out migrationState))
				{
					this.WriteError(Strings.ErrorRepairConvertStatus(this.Status.ToString(), migrationState.GetType().ToString()));
				}
				base.WriteVerbose(new LocalizedString(string.Format("MigrationJob before update {0}", job)));
				job.SetStatus(base.DataProvider, MigrationJobStatus.SyncStarting, migrationState, null, null, null, null, null, null, null, true, null, null);
				base.WriteVerbose(new LocalizedString(string.Format("MigrationJob after update {0}", job)));
			}
			else
			{
				MigrationJobStatus migrationJobStatus;
				if (!Enum.TryParse<MigrationJobStatus>(this.Status, false, out migrationJobStatus))
				{
					this.WriteError(Strings.ErrorRepairConvertStatus(this.Status.ToString(), migrationJobStatus.GetType().ToString()));
				}
				MigrationStatusData<MigrationJobStatus> migrationStatusData = new MigrationStatusData<MigrationJobStatus>(job.StatusData);
				base.WriteVerbose(new LocalizedString(string.Format("MigrationJob before update {0}", job)));
				migrationStatusData.UpdateStatus(migrationJobStatus, null);
				job.SetStatusData(base.DataProvider, migrationStatusData);
				base.WriteVerbose(new LocalizedString(string.Format("MigrationJob after update {0}", job)));
			}
			this.TryUpdateMigrationReport(job, "Status updated");
		}

		private MigrationJobItem GetMigrationJobItem()
		{
			List<MigrationJobItem> migrationJobItems = this.GetMigrationJobItems();
			if (migrationJobItems.Count <= 0)
			{
				this.WriteError(Strings.MigrationUserNotFound(this.UserId.ToString()));
			}
			else if (migrationJobItems.Count > 1)
			{
				this.WriteError(Strings.ValidateRepairMultipleUsers(this.UserId.ToString(), migrationJobItems.Count));
			}
			return migrationJobItems[0];
		}

		private List<MigrationJobItem> GetMigrationJobItems()
		{
			if (this.UserId == null || this.UserId.MigrationUserId == null || (string.IsNullOrEmpty(this.UserId.MigrationUserId.Id) && this.UserId.MigrationUserId.JobItemGuid == Guid.Empty))
			{
				this.WriteError(Strings.ValidateRepairInvalidUser);
				return null;
			}
			List<MigrationJobItem> list = null;
			if (this.UserId.MigrationUserId.JobItemGuid != Guid.Empty)
			{
				MigrationJobItem byGuid = MigrationJobItem.GetByGuid(base.DataProvider, this.UserId.MigrationUserId.JobItemGuid);
				if (byGuid == null)
				{
					using (IMigrationDataProvider providerForFolder = base.DataProvider.GetProviderForFolder(MigrationFolderName.CorruptedItems))
					{
						byGuid = MigrationJobItem.GetByGuid(providerForFolder, this.UserId.MigrationUserId.JobItemGuid);
					}
				}
				list = new List<MigrationJobItem>(1);
				if (byGuid != null)
				{
					list.Add(byGuid);
				}
				return list;
			}
			list = new List<MigrationJobItem>(MigrationJobItem.GetByIdentifier(base.DataProvider, null, this.UserId.MigrationUserId.Id, null));
			if (list.Count <= 0)
			{
				using (IMigrationDataProvider providerForFolder2 = base.DataProvider.GetProviderForFolder(MigrationFolderName.CorruptedItems))
				{
					list = new List<MigrationJobItem>(MigrationJobItem.GetByIdentifier(providerForFolder2, null, this.UserId.MigrationUserId.Id, null));
				}
			}
			return list;
		}

		private void RevertJobItem(MigrationJobItem jobItem, MigrationUserStatus status)
		{
			if (jobItem.IsPAW)
			{
				this.WriteError(Strings.ValidateRepairInvalidRevertJobType(jobItem.ToString()));
			}
			if (jobItem.Status != status)
			{
				this.WriteError(Strings.ValidateRepairInvalidRevert(jobItem.ToString(), jobItem.Status.ToString()));
			}
			MigrationStatusData<MigrationUserStatus> migrationStatusData = new MigrationStatusData<MigrationUserStatus>(jobItem.StatusData);
			if (migrationStatusData.RevertStatus())
			{
				base.WriteVerbose(new LocalizedString(string.Format("Reverting MigrationUser {0}", jobItem.Identifier)));
				jobItem.SetStatusData(base.DataProvider, migrationStatusData);
				return;
			}
			this.WriteError(Strings.ErrorRepairReverting(jobItem.ToString()));
		}

		private void SetJobItemStatus(MigrationJobItem jobItem)
		{
			if (jobItem.IsPAW)
			{
				MigrationState migrationState;
				if (!Enum.TryParse<MigrationState>(this.Status, false, out migrationState))
				{
					this.WriteError(Strings.ErrorRepairConvertStatus(this.Status.ToString(), migrationState.GetType().ToString()));
				}
				base.WriteVerbose(new LocalizedString(string.Format("MigrationJobItem before update {0}", jobItem)));
				if (migrationState == MigrationState.Failed || migrationState == MigrationState.Corrupted)
				{
					jobItem.SetStatus(base.DataProvider, (migrationState == MigrationState.Failed) ? MigrationUserStatus.Failed : MigrationUserStatus.Corrupted, migrationState, null, null, null, null, null, null, false, MigrationHelperBase.CreatePermanentExceptionWithInternalData<MigrationUnknownException>("repair migration: setting error"));
				}
				else
				{
					MigrationUserStatus status;
					switch (migrationState)
					{
					case MigrationState.Active:
					case MigrationState.Waiting:
						status = MigrationUserStatus.Syncing;
						break;
					case MigrationState.Completed:
						status = MigrationUserStatus.Completed;
						break;
					case MigrationState.Stopped:
						status = MigrationUserStatus.Stopped;
						break;
					default:
						throw new NotSupportedException("Unexpected state " + migrationState);
					}
					jobItem.SetStatus(base.DataProvider, status, migrationState, null, null, null, null, null, null, false, null);
				}
				base.WriteVerbose(new LocalizedString(string.Format("MigrationJobItem after update {0}", jobItem)));
				return;
			}
			MigrationUserStatus migrationUserStatus;
			if (!Enum.TryParse<MigrationUserStatus>(this.Status, false, out migrationUserStatus))
			{
				this.WriteError(Strings.ErrorRepairConvertStatus(this.Status.ToString(), migrationUserStatus.GetType().ToString()));
			}
			base.WriteVerbose(new LocalizedString(string.Format("setting MigrationJob {0} status from {1} to {2}", jobItem, jobItem.Status, migrationUserStatus)));
			MigrationStatusData<MigrationUserStatus> migrationStatusData = new MigrationStatusData<MigrationUserStatus>(jobItem.StatusData);
			if (MigrationJobItem.IsFailedStatus(migrationUserStatus))
			{
				migrationStatusData.UpdateStatus(migrationUserStatus, MigrationHelperBase.CreatePermanentExceptionWithInternalData<MigrationUnknownException>("repair migration: setting error"), "repair migrationsetting error", null);
			}
			else
			{
				migrationStatusData.UpdateStatus(migrationUserStatus, null);
			}
			jobItem.SetStatusData(base.DataProvider, migrationStatusData);
		}

		private ILegacySubscriptionHandler GetLegacySubscriptionHandler(MigrationJobItem jobItem)
		{
			MigrationJob migrationJob = this.GetMigrationJob(new Guid?(jobItem.MigrationJobId));
			if (migrationJob == null)
			{
				this.WriteError(Strings.MigrationJobNotFound(jobItem.MigrationJobId.ToString()));
			}
			ILegacySubscriptionHandler legacySubscriptionHandler = LegacySubscriptionHandlerBase.CreateSubscriptionHandler(base.DataProvider, migrationJob);
			if (legacySubscriptionHandler == null)
			{
				this.WriteError(Strings.ValidateRepairMissingSubscriptionHandler(migrationJob.ToString()));
			}
			return legacySubscriptionHandler;
		}

		private void UpdateTransportSyncSubscription(Guid subscriptionId, UpdateSyncSubscriptionAction status)
		{
			if (this.UserId == null || this.UserId.MigrationUserId == null || string.IsNullOrEmpty(this.UserId.MigrationUserId.Id))
			{
				this.WriteError(Strings.ValidateRepairInvalidUser);
			}
			MailboxData mailboxData = null;
			try
			{
				mailboxData = base.DataProvider.ADProvider.GetMailboxDataFromSmtpAddress(this.UserId.MigrationUserId.Id, false, true);
			}
			catch (MigrationRecipientNotFoundException ex)
			{
				this.WriteError(Strings.ValidateRepairMissingSubscription(this.UserId.ToString(), ex.ToString()));
			}
			AggregationSubscriptionType aggregationSubscriptionType = AggregationSubscriptionType.IMAP;
			if (!string.IsNullOrEmpty(this.PropertyType) && !Enum.TryParse<AggregationSubscriptionType>(this.PropertyType, false, out aggregationSubscriptionType))
			{
				this.WriteInternalError(string.Format("couldn't convert type {0} to {1}", this.PropertyType, aggregationSubscriptionType.GetType()));
			}
			UpdateSyncSubscriptionArgs args = new UpdateSyncSubscriptionArgs(base.DataProvider.OrganizationId.OrganizationalUnit, mailboxData.MailboxLegacyDN, subscriptionId, aggregationSubscriptionType, status);
			if (!this.ShouldRepairContinue(Strings.ConfirmRepairSubscription(status.ToString(), aggregationSubscriptionType.ToString(), mailboxData.ToString())))
			{
				return;
			}
			MigrationServiceRpcStub migrationServiceRpcStub = new MigrationServiceRpcStub(mailboxData.MailboxServer);
			migrationServiceRpcStub.UpdateSyncSubscription(args);
			base.WriteVerbose(new LocalizedString(string.Format("SUCCESS updating {0} subscriptionId {1}, action {2} for {3}", new object[]
			{
				aggregationSubscriptionType.ToString(),
				subscriptionId,
				status,
				this.UserId.MigrationUserId.Id
			})));
		}

		private void RemoveReportItem(MigrationReportId reportId)
		{
			using (IMigrationDataProvider providerForFolder = base.DataProvider.GetProviderForFolder(MigrationFolderName.SyncMigrationReports))
			{
				MigrationReportItem migrationReportItem = MigrationReportItem.Get(providerForFolder, reportId);
				if (migrationReportItem == null)
				{
					this.WriteError(Strings.ValidateRepairMissingReport(reportId.ToString()));
				}
				migrationReportItem.Delete(providerForFolder);
				base.WriteVerbose(new LocalizedString(string.Format("removing report {0} with message {1}", migrationReportItem, reportId)));
			}
		}

		private void UpdateCacheEntry()
		{
			MigrationObjectTaskBase<MigrationBatchIdParameter>.RegisterMigrationBatch(this, base.DataProvider.MailboxSession, base.CurrentOrganizationId, true, true);
		}

		private void EnableMigrationFeatures(MigrationFeature feature)
		{
			MigrationSession migrationSession = MigrationSession.Get(base.DataProvider);
			IMigrationConfig config = migrationSession.Config;
			config.EnableFeatures(base.DataProvider, feature);
		}

		private void DisableMigrationFeatures(MigrationFeature feature)
		{
			MigrationSession migrationSession = MigrationSession.Get(base.DataProvider);
			IMigrationConfig config = migrationSession.Config;
			config.DisableFeatures(base.DataProvider, feature, this.Force);
		}

		private void UpdateOrganizationUpgradeConstraint(bool addConstraint)
		{
			IUpgradeConstraintAdapter upgradeConstraintAdapter = new OrganizationUpgradeConstraintAdapter();
			if (addConstraint)
			{
				upgradeConstraintAdapter.AddUpgradeConstraint(base.DataProvider, MigrationSession.Get(base.DataProvider));
				return;
			}
			upgradeConstraintAdapter.MarkUpgradeConstraintForExpiry(base.DataProvider, this.ConstraintExpiryDate);
		}

		private const string ParameterSetUpdateMigrationBatch = "UpdateMigrationBatch";

		private const string ParameterSetUpdateMigrationUser = "UpdateMigrationUser";

		private const string ParameterSetUpdateMigrationFolder = "UpdateMigrationFolder";

		private const string ParameterSetUpdateMigrationReport = "UpdateMigrationReport";

		private const string ParameterSetUpdateSubscriptionStatus = "UpdateSubscriptionStatus";

		private const string ParameterSetUpdateMigrationCacheEntry = "UpdateMigrationCacheEntry";

		private const string ParameterSetUpdateMigrationStoreObject = "UpdateMigrationStoreObject";

		private const string ParameterSetRemoveMigrationBatch = "RemoveMigrationBatch";

		private const string ParameterSetRemoveMigrationUser = "RemoveMigrationUser";

		private const string ParameterSetRemoveSubscription = "RemoveSubscription";

		private const string ParameterSetRemoveMigrationFolder = "RemoveMigrationFolder";

		private const string ParameterSetRemoveMigrationReport = "RemoveMigrationReport";

		private const string ParameterSetRemoveMigrationStoreObject = "RemoveMigrationStoreObject";

		private const string ParameterSetRevertMigrationBatch = "RevertMigrationBatch";

		private const string ParameterSetRevertMigrationUser = "RevertMigrationUser";

		private const string ParameterSetSyncMigrationUser = "SyncMigrationUser";

		private const string ParameterSetResumeMigrationUser = "ResumeMigrationUser";

		private const string ParameterSetFlushMigrationUser = "FlushMigrationUser";

		private const string ParameterSetDeleteSubscription = "DeleteSubscription";

		private const string ParameterSetDisableSubscription = "DisableSubscription";

		private const string ParameterSetEnableConfigFeatures = "EnableConfigFeatures";

		private const string ParameterSetDisableConfigFeatures = "DisableConfigFeatures";

		private const string ParameterSetExpireConstraint = "ExpireOrgUpgradeConstraint";

		private const string ParameterSetAddUpgradeConstraint = "AddOrgUpgradeConstraint";

		private const string ParameterPropertyValue = "Value";
	}
}
