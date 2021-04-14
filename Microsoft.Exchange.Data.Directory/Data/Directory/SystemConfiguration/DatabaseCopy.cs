using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DatabaseCopy : ADConfigurationObject, IActivationPreferenceSettable<DatabaseCopy>, IComparable<DatabaseCopy>, IProvisioningCacheInvalidation
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return DatabaseCopy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DatabaseCopy.mostDerivedClass;
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			if (string.IsNullOrEmpty(this.HostServerName))
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorHostServerNotSet, DatabaseCopySchema.HostServerName, this));
			}
			if (!this.IsHostServerValidForObjectValidation)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorHostServerNotSet, DatabaseCopySchema.HostServer, this));
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!this.IsHostServerValidForObjectValidation)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorHostServerNotSet, DatabaseCopySchema.HostServer, this));
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public string DatabaseName
		{
			get
			{
				return (string)this[DatabaseCopySchema.DatabaseName];
			}
		}

		internal new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
			set
			{
				this[ADObjectSchema.Name] = value;
			}
		}

		public string HostServerName
		{
			get
			{
				return (string)this[DatabaseCopySchema.HostServerName];
			}
		}

		internal int ActivationPreferenceInternal
		{
			get
			{
				return (int)this[DatabaseCopySchema.ActivationPreference];
			}
			set
			{
				this[DatabaseCopySchema.ActivationPreference] = value;
			}
		}

		public int ActivationPreference
		{
			get
			{
				return this.activationPreference;
			}
			internal set
			{
				this.activationPreference = value;
			}
		}

		internal ADObjectId HostServer
		{
			get
			{
				return (ADObjectId)this[DatabaseCopySchema.HostServer];
			}
			set
			{
				this[DatabaseCopySchema.HostServer] = value;
				this.HostServerUnlinked = (value == null);
			}
		}

		internal bool IsValidDatabaseCopy(bool allowInvalid)
		{
			return allowInvalid || this.IsValid;
		}

		internal bool IsValidForRead
		{
			get
			{
				return base.ValidateRead().Length == 0;
			}
		}

		internal bool IsHostServerPresent
		{
			get
			{
				return this.HostServer != null && !this.HostServer.IsDeleted;
			}
		}

		internal bool IsHostServerValid
		{
			get
			{
				return this.IsHostServerPresent || this.HostServerUnlinked;
			}
		}

		private bool IsHostServerValidForObjectValidation
		{
			get
			{
				return this.IsHostServerPresent || this.InvalidHostServerAllowed;
			}
		}

		public string ParentObjectClass
		{
			get
			{
				return (string)this[DatabaseCopySchema.ParentObjectClass];
			}
			internal set
			{
				this[DatabaseCopySchema.ParentObjectClass] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ReplayLagTime
		{
			get
			{
				return (EnhancedTimeSpan)this[DatabaseCopySchema.ReplayLag];
			}
			set
			{
				this[DatabaseCopySchema.ReplayLag] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TruncationLagTime
		{
			get
			{
				return (EnhancedTimeSpan)this[DatabaseCopySchema.TruncationLag];
			}
			set
			{
				this[DatabaseCopySchema.TruncationLag] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseCopyAutoActivationPolicyType DatabaseCopyAutoActivationPolicy
		{
			get
			{
				return (DatabaseCopyAutoActivationPolicyType)this[DatabaseCopySchema.DatabaseCopyAutoActivationPolicy];
			}
			set
			{
				this[DatabaseCopySchema.DatabaseCopyAutoActivationPolicy] = value;
			}
		}

		internal DatabaseCopyAutoDagFlags AutoDagFlags
		{
			get
			{
				return (DatabaseCopyAutoDagFlags)this[DatabaseCopySchema.AutoDagFlags];
			}
			set
			{
				this[DatabaseCopySchema.AutoDagFlags] = value;
			}
		}

		internal bool InvalidHostServerAllowed
		{
			get
			{
				return (bool)this[DatabaseCopySchema.InvalidHostServerAllowed];
			}
			set
			{
				this[DatabaseCopySchema.InvalidHostServerAllowed] = value;
			}
		}

		internal bool HostServerUnlinked
		{
			get
			{
				return (bool)this[DatabaseCopySchema.HostServerUnlinked];
			}
			private set
			{
				this[DatabaseCopySchema.HostServerUnlinked] = value;
			}
		}

		internal TDatabase GetDatabase<TDatabase>() where TDatabase : IConfigurable, new()
		{
			return (TDatabase)((object)base.Session.Read<TDatabase>(((ADObjectId)this.Identity).Parent));
		}

		internal DatabaseCopy[] GetAllDatabaseCopies()
		{
			Database database = this.GetDatabase<Database>();
			DatabaseCopy[] result = null;
			if (database != null)
			{
				result = database.GetDatabaseCopies();
			}
			return result;
		}

		public override string ToString()
		{
			return this.Identity.ToString();
		}

		internal static object DatabaseNameGetter(IPropertyBag propertyBag)
		{
			ADObjectId parent = ((ADObjectId)propertyBag[ADObjectSchema.Id]).Parent;
			if (parent == null)
			{
				return DatabaseCopySchema.DatabaseName.DefaultValue;
			}
			return parent.Name;
		}

		internal static object HostServerNameGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[DatabaseCopySchema.HostServer];
			string result;
			if (adobjectId == null)
			{
				result = (string)propertyBag[ADObjectSchema.Name];
			}
			else
			{
				result = adobjectId.Name;
			}
			return result;
		}

		public int CompareTo(DatabaseCopy other)
		{
			if (other == null)
			{
				return -1;
			}
			return this.ActivationPreferenceInternal.CompareTo(other.ActivationPreferenceInternal);
		}

		int IActivationPreferenceSettable<DatabaseCopy>.ActualValue
		{
			get
			{
				return this.ActivationPreferenceInternal;
			}
			set
			{
				this.ActivationPreferenceInternal = value;
			}
		}

		int IActivationPreferenceSettable<DatabaseCopy>.DesiredValue
		{
			get
			{
				return this.ActivationPreference;
			}
		}

		bool IActivationPreferenceSettable<DatabaseCopy>.Matches(DatabaseCopy other)
		{
			return base.Guid == other.Guid;
		}

		bool IActivationPreferenceSettable<DatabaseCopy>.InvalidHostServerAllowed
		{
			set
			{
				this.InvalidHostServerAllowed = value;
			}
		}

		internal bool ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			orgId = null;
			keys = null;
			if (base.ObjectState == ObjectState.New || base.ObjectState == ObjectState.Deleted || (base.ObjectState == ObjectState.Changed && (base.IsChanged(DatabaseCopySchema.HostServer) || base.IsChanged(DatabaseCopySchema.HostServerName))))
			{
				keys = new Guid[]
				{
					CannedProvisioningCacheKeys.ProvisioningEnabledDatabasesOnLocalSite
				};
				return true;
			}
			return false;
		}

		bool IProvisioningCacheInvalidation.ShouldInvalidProvisioningCache(out OrganizationId orgId, out Guid[] keys)
		{
			return this.ShouldInvalidProvisioningCache(out orgId, out keys);
		}

		internal const string MaxTimeSpanStr = "14.00:00:00";

		internal const string DefaultReplayLagTimeStr = "00:00:00";

		internal const string DefaultTruncationLagTimeStr = "00:00:00";

		private static DatabaseCopySchema schema = ObjectSchema.GetInstance<DatabaseCopySchema>();

		private static string mostDerivedClass = "msExchMDBCopy";

		private int activationPreference;
	}
}
