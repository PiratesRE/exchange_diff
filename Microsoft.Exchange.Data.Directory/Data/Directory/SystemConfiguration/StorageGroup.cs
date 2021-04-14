using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class StorageGroup : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return StorageGroup.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return StorageGroup.mostDerivedClass;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!base.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				AsciiCharactersOnlyConstraint asciiCharactersOnlyConstraint = new AsciiCharactersOnlyConstraint();
				PropertyConstraintViolationError propertyConstraintViolationError = asciiCharactersOnlyConstraint.Validate(this.Name, StorageGroupSchema.Name, null);
				if (propertyConstraintViolationError != null)
				{
					errors.Add(propertyConstraintViolationError);
				}
				if (null != this.LogFolderPath)
				{
					propertyConstraintViolationError = asciiCharactersOnlyConstraint.Validate(this.LogFolderPath, StorageGroupSchema.LogFolderPath, null);
					if (propertyConstraintViolationError != null)
					{
						errors.Add(propertyConstraintViolationError);
					}
				}
				if (null != this.SystemFolderPath)
				{
					propertyConstraintViolationError = asciiCharactersOnlyConstraint.Validate(this.SystemFolderPath, StorageGroupSchema.SystemFolderPath, null);
					if (propertyConstraintViolationError != null)
					{
						errors.Add(propertyConstraintViolationError);
					}
				}
			}
			if (base.Id.DomainId != null && base.Id.Depth - base.Id.DomainId.Depth < 8)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorInvalidDNDepth, this.Identity, string.Empty));
			}
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(StorageGroupSchema.EventLogSourceID))
			{
				this[StorageGroupSchema.EventLogSourceID] = "MSExchangeIS";
			}
			base.StampPersistableDefaultValues();
		}

		public NonRootLocalLongFullPath LogFolderPath
		{
			get
			{
				return (NonRootLocalLongFullPath)this[StorageGroupSchema.LogFolderPath];
			}
			internal set
			{
				this[StorageGroupSchema.LogFolderPath] = value;
			}
		}

		public NonRootLocalLongFullPath SystemFolderPath
		{
			get
			{
				return (NonRootLocalLongFullPath)this[StorageGroupSchema.SystemFolderPath];
			}
			internal set
			{
				this[StorageGroupSchema.SystemFolderPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CircularLoggingEnabled
		{
			get
			{
				return (bool)this[StorageGroupSchema.CircularLoggingEnabled];
			}
			set
			{
				this[StorageGroupSchema.CircularLoggingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ZeroDatabasePages
		{
			get
			{
				return (bool)this[StorageGroupSchema.ZeroDatabasePages];
			}
			set
			{
				this[StorageGroupSchema.ZeroDatabasePages] = value;
			}
		}

		public string LogFilePrefix
		{
			get
			{
				return (string)this[StorageGroupSchema.LogFilePrefix];
			}
			internal set
			{
				this[StorageGroupSchema.LogFilePrefix] = value;
			}
		}

		public int LogFileSize
		{
			get
			{
				return (int)this[StorageGroupSchema.LogFileSize];
			}
		}

		public bool RecoveryEnabled
		{
			get
			{
				return (bool)this[StorageGroupSchema.RecoveryEnabled];
			}
		}

		public bool OnlineDefragEnabled
		{
			get
			{
				return (bool)this[StorageGroupSchema.OnlineDefragEnabled];
			}
		}

		public bool IndexCheckingEnabled
		{
			get
			{
				return (bool)this[StorageGroupSchema.IndexCheckingEnabled];
			}
		}

		public string EventLogSourceID
		{
			get
			{
				return (string)this[StorageGroupSchema.EventLogSourceID];
			}
		}

		public int LogCheckpointDepth
		{
			get
			{
				return (int)this[StorageGroupSchema.LogCheckpointDepth];
			}
		}

		public bool CommitDefault
		{
			get
			{
				return (int)this[StorageGroupSchema.CommitDefault] != 0;
			}
		}

		public int DatabaseExtensionSize
		{
			get
			{
				return (int)this[StorageGroupSchema.DatabaseExtensionSize];
			}
		}

		public int PageFragment
		{
			get
			{
				return (int)this[StorageGroupSchema.PageFragment];
			}
		}

		public int PageTempDBMinimum
		{
			get
			{
				return (int)this[StorageGroupSchema.PageTempDBMinimum];
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[StorageGroupSchema.Server];
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[StorageGroupSchema.ServerName];
			}
		}

		public bool Recovery
		{
			get
			{
				return (bool)this[StorageGroupSchema.Recovery];
			}
			internal set
			{
				this[StorageGroupSchema.Recovery] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public new string Name
		{
			get
			{
				return (string)this[StorageGroupSchema.Name];
			}
			set
			{
				this[StorageGroupSchema.Name] = value;
			}
		}

		public CanRunDefaultUpdateState? CanRunDefaultUpdate
		{
			get
			{
				return (CanRunDefaultUpdateState?)this[StorageGroupSchema.CanRunDefaultUpdate];
			}
			internal set
			{
				this[StorageGroupSchema.CanRunDefaultUpdate] = value;
			}
		}

		public CanRunRestoreState? CanRunRestore
		{
			get
			{
				return (CanRunRestoreState?)this[StorageGroupSchema.CanRunRestore];
			}
			internal set
			{
				this[StorageGroupSchema.CanRunRestore] = value;
			}
		}

		internal Database[] GetDatabases()
		{
			return base.Session.Find<Database>(base.Id, QueryScope.SubTree, null, null, 0);
		}

		internal Server GetServer()
		{
			return base.Session.Read<Server>(this.Server);
		}

		internal MailboxDatabase[] GetMailboxDatabases()
		{
			return base.Session.Find<MailboxDatabase>(base.Id, QueryScope.SubTree, null, null, 0);
		}

		internal static object ServerGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				result = ((ADObjectId)propertyBag[ADObjectSchema.Id]).AncestorDN(2);
			}
			catch (InvalidOperationException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Server", ex.Message), StorageGroupSchema.Server, propertyBag[ADObjectSchema.Id]), ex);
			}
			return result;
		}

		internal static object ServerNameGetter(IPropertyBag propertyBag)
		{
			return ((ADObjectId)StorageGroup.ServerGetter(propertyBag)).Name;
		}

		internal static object StorageGroupNameGetter(IPropertyBag propertyBag)
		{
			return propertyBag[ADObjectSchema.RawName];
		}

		internal static void StorageGroupNameSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADObjectSchema.RawName] = value;
		}

		private static StorageGroupSchema schema = ObjectSchema.GetInstance<StorageGroupSchema>();

		private static string mostDerivedClass = "msExchStorageGroup";
	}
}
