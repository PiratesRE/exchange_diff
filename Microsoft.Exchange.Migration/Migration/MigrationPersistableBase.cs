using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MigrationPersistableBase : IMigrationPersistable, IMigrationSerializable
	{
		protected MigrationPersistableBase()
		{
			this.Version = -1L;
			this.ExtendedProperties = new PersistableDictionary();
			this.reportData = new Lazy<ReportData>(new Func<ReportData>(this.CreateDiagnosticReport), LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public long Version { get; protected set; }

		public StoreObjectId StoreObjectId { get; protected set; }

		public ExDateTime CreationTime { get; protected set; }

		public PersistableDictionary ExtendedProperties { get; protected set; }

		public virtual PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return MigrationPersistableBase.MigrationBaseDefinitions;
			}
		}

		public virtual PropertyDefinition[] InitializationPropertyDefinitions
		{
			get
			{
				return MigrationPersistableBase.VersionPropertyDefinitions;
			}
		}

		public virtual long CurrentSupportedVersion
		{
			get
			{
				return this.MinimumSupportedVersion;
			}
		}

		public abstract long MinimumSupportedVersion { get; }

		public abstract long MaximumSupportedVersion { get; }

		public virtual long MinimumSupportedPersistableVersion
		{
			get
			{
				return this.MinimumSupportedVersion;
			}
		}

		public ReportData ReportData
		{
			get
			{
				return this.reportData.Value;
			}
		}

		protected bool IsPersisted
		{
			get
			{
				return this.Version != -1L;
			}
		}

		protected virtual Guid ReportGuid
		{
			get
			{
				return Guid.Empty;
			}
		}

		protected OrganizationId OrganizationId { get; set; }

		public virtual bool TryLoad(IMigrationDataProvider dataProvider, StoreObjectId id)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.ThrowOnNullArgument(id, "id");
			bool success = true;
			MigrationUtil.RunTimedOperation(delegate()
			{
				using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(dataProvider, id, this.InitializationPropertyDefinitions))
				{
					this.OrganizationId = dataProvider.OrganizationId;
					if (!this.InitializeFromMessageItem(migrationStoreObject))
					{
						success = false;
						return;
					}
					migrationStoreObject.Load(this.PropertyDefinitions);
					if (!this.ReadFromMessageItem(migrationStoreObject))
					{
						success = false;
						return;
					}
					this.LoadLinkedStoredObjects(migrationStoreObject, dataProvider);
				}
				this.CheckVersion();
			}, this);
			return success;
		}

		public virtual bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			if (this.Version < this.MinimumSupportedPersistableVersion)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "not reading extended properties for version:" + this.Version, new object[0]);
				return false;
			}
			this.ExtendedProperties = MigrationHelper.GetDictionaryProperty(message, MigrationBatchMessageSchema.MigrationPersistableDictionary, true);
			this.ReadStoreObjectIdProperties(message);
			return true;
		}

		public virtual void CreateInStore(IMigrationDataProvider dataProvider, Action<IMigrationStoreObject> streamAction)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			MigrationUtil.AssertOrThrow(this.StoreObjectId == null, "Object should not have been created already.", new object[0]);
			using (IMigrationStoreObject migrationStoreObject = this.CreateStoreObject(dataProvider))
			{
				this.InitializeVersion();
				if (streamAction != null)
				{
					streamAction(migrationStoreObject);
				}
				this.WriteToMessageItem(migrationStoreObject, false);
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
				migrationStoreObject.Load(this.InitializationPropertyDefinitions);
				this.ReadStoreObjectIdProperties(migrationStoreObject);
			}
		}

		public virtual void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			this.CheckVersion();
			message[MigrationBatchMessageSchema.MigrationVersion] = this.Version;
			if (this.Version < this.MinimumSupportedPersistableVersion)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "not writing extended properties for version:" + this.Version, new object[0]);
				return;
			}
			this.WriteExtendedPropertiesToMessageItem(message);
		}

		public virtual XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			return this.GetDiagnosticInfo(dataProvider, argument, new XElement("PersistableBase"));
		}

		public abstract IMigrationStoreObject FindStoreObject(IMigrationDataProvider dataProvider, StoreObjectId id, PropertyDefinition[] properties);

		public IMigrationStoreObject FindStoreObject(IMigrationDataProvider dataProvider, PropertyDefinition[] properties)
		{
			ExAssert.RetailAssert(this.StoreObjectId != null, "Need to persist the objects before trying to retrieve their storage object.");
			if (properties == null)
			{
				properties = this.PropertyDefinitions;
			}
			return this.FindStoreObject(dataProvider, this.StoreObjectId, properties);
		}

		public IMigrationStoreObject FindStoreObject(IMigrationDataProvider dataProvider)
		{
			return this.FindStoreObject(dataProvider, null);
		}

		protected virtual void LoadLinkedStoredObjects(IMigrationStoreObject item, IMigrationDataProvider dataProvider)
		{
		}

		protected XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument, XElement parent)
		{
			parent.Add(new XElement("version", this.Version));
			parent.Add(new XElement("created", (this.StoreObjectId != null) ? this.CreationTime.ToString() : null));
			parent.Add(new XElement("reportGuid", this.ExtendedProperties.Get<Guid>("report", Guid.Empty)));
			if (this.Version >= this.MinimumSupportedPersistableVersion)
			{
				IOrderedEnumerable<DictionaryEntry> orderedEnumerable = from DictionaryEntry item in this.ExtendedProperties
				orderby item.Key
				select item;
				foreach (DictionaryEntry dictionaryEntry in orderedEnumerable)
				{
					IEnumerable<object> enumerable = dictionaryEntry.Value as IEnumerable<object>;
					if (enumerable != null)
					{
						XElement xelement = new XElement((string)dictionaryEntry.Key);
						foreach (object obj in enumerable)
						{
							xelement.Add(new XElement("Element", MigrationPersistableBase.GetDiagnosticObject(obj)));
						}
						parent.Add(xelement);
					}
					else if (dictionaryEntry.Value != null)
					{
						parent.Add(new XElement((string)dictionaryEntry.Key, MigrationPersistableBase.GetDiagnosticObject(dictionaryEntry.Value)));
					}
					else
					{
						parent.Add(new XElement((string)dictionaryEntry.Key));
					}
				}
			}
			if (dataProvider != null && argument.HasArgument("storage"))
			{
				parent.Add(this.GetStorageDiagnosticInfo(dataProvider, argument));
			}
			return parent;
		}

		protected virtual XElement GetStorageDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			ExAssert.RetailAssert(this.StoreObjectId != null, "Need to persist the objects before trying to retrieve their diagnostics");
			XElement diagnosticInfo;
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(dataProvider, this.StoreObjectId, this.PropertyDefinitions))
			{
				diagnosticInfo = migrationStoreObject.GetDiagnosticInfo(this.PropertyDefinitions, argument);
			}
			return diagnosticInfo;
		}

		protected abstract IMigrationStoreObject CreateStoreObject(IMigrationDataProvider dataProvider);

		protected void InitializeVersion()
		{
			if (this.CurrentSupportedVersion == -1L)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "not writing version. we're not allowed", new object[0]);
				return;
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "setting version to {0}", new object[]
			{
				this.CurrentSupportedVersion
			});
			this.Version = this.CurrentSupportedVersion;
		}

		protected virtual bool InitializeFromMessageItem(IMigrationStoreObject message)
		{
			long valueOrDefault = message.GetValueOrDefault<long>(MigrationBatchMessageSchema.MigrationVersion, -1L);
			if (valueOrDefault == -1L)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "object doesn't exist", new object[0]);
				return false;
			}
			this.Version = valueOrDefault;
			return true;
		}

		protected void SetVersion(IMigrationDataProvider dataProvider, long version)
		{
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(dataProvider))
			{
				this.CheckVersion(version);
				migrationStoreObject.OpenAsReadWrite();
				migrationStoreObject[MigrationBatchMessageSchema.MigrationVersion] = version;
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
				this.Version = version;
			}
		}

		protected void CheckVersion()
		{
			this.CheckVersion(this.Version);
		}

		protected void CheckVersion(long version)
		{
			if (version < this.MinimumSupportedVersion || version > this.MaximumSupportedVersion)
			{
				throw new MigrationVersionMismatchException(version, this.MaximumSupportedVersion);
			}
		}

		protected virtual void SaveExtendedProperties(IMigrationDataProvider provider)
		{
			MigrationUtil.AssertOrThrow(this.StoreObjectId != null, "Object should have been created before trying to save properties.", new object[0]);
			using (IMigrationStoreObject migrationStoreObject = this.FindStoreObject(provider, MigrationPersistableBase.MigrationBaseDefinitions))
			{
				migrationStoreObject.OpenAsReadWrite();
				this.WriteExtendedPropertiesToMessageItem(migrationStoreObject);
				migrationStoreObject.Save(SaveMode.NoConflictResolution);
			}
			if (this.ReportGuid != Guid.Empty)
			{
				provider.FlushReport(this.ReportData);
			}
		}

		protected virtual void WriteExtendedPropertiesToMessageItem(IMigrationStoreObject message)
		{
			MigrationHelper.SetDictionaryProperty(message, MigrationBatchMessageSchema.MigrationPersistableDictionary, this.ExtendedProperties);
		}

		protected void ReadStoreObjectIdProperties(IMigrationStoreObject storeObject)
		{
			this.StoreObjectId = storeObject.Id;
			this.CreationTime = storeObject.CreationTime;
		}

		private static object GetDiagnosticObject(object obj)
		{
			byte[] array = obj as byte[];
			if (array != null)
			{
				try
				{
					obj = new Guid(array);
				}
				catch (ArgumentException)
				{
				}
			}
			return obj;
		}

		private ReportData CreateDiagnosticReport()
		{
			if (!(this.ReportGuid != Guid.Empty))
			{
				return null;
			}
			return new ReportData(this.ReportGuid, ReportVersion.ReportE14R6Compression);
		}

		protected const long InvalidVersion = -1L;

		protected static readonly PropertyDefinition[] VersionPropertyDefinitions = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationVersion
		};

		protected static readonly PropertyDefinition[] MigrationBaseDefinitions = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationPersistableDictionary
		};

		private readonly Lazy<ReportData> reportData;
	}
}
