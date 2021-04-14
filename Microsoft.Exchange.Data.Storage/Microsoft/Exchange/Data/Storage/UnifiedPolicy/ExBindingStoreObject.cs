using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExBindingStoreObject : EwsStoreObject
	{
		public string Name
		{
			get
			{
				return (string)this[ExBindingStoreObjectSchema.Name];
			}
			set
			{
				this[ExBindingStoreObjectSchema.Name] = value;
			}
		}

		public Guid MasterIdentity
		{
			get
			{
				return Guid.Parse((string)this[ExBindingStoreObjectSchema.MasterIdentity]);
			}
			set
			{
				this[ExBindingStoreObjectSchema.MasterIdentity] = value.ToString();
			}
		}

		public Guid PolicyId
		{
			get
			{
				return Guid.Parse(base.AlternativeId);
			}
			set
			{
				base.AlternativeId = value.ToString();
			}
		}

		public Workload Workload
		{
			get
			{
				return (Workload)this[ExBindingStoreObjectSchema.Workload];
			}
			set
			{
				this[ExBindingStoreObjectSchema.Workload] = value;
			}
		}

		public Guid PolicyVersion
		{
			get
			{
				return (Guid)this[ExBindingStoreObjectSchema.PolicyVersion];
			}
			set
			{
				this[ExBindingStoreObjectSchema.PolicyVersion] = value;
			}
		}

		public DateTime? WhenCreated
		{
			get
			{
				return (DateTime?)this[ExBindingStoreObjectSchema.WhenCreated];
			}
		}

		public DateTime? WhenChanged
		{
			get
			{
				return (DateTime?)this[ExBindingStoreObjectSchema.WhenChanged];
			}
		}

		internal MultiValuedProperty<string> RawAppliedScopes
		{
			get
			{
				return (MultiValuedProperty<string>)this[ExBindingStoreObjectSchema.RawAppliedScopes];
			}
			set
			{
				this[ExBindingStoreObjectSchema.RawAppliedScopes] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ExBindingStoreObject.schema;
			}
		}

		public void FromBindingStorage(BindingStorage bindingStorage, ExPolicyConfigProvider policyConfigProvider)
		{
			ArgumentValidator.ThrowIfNull("bindingStorage", bindingStorage);
			ArgumentValidator.ThrowIfNull("policyConfigProvider", policyConfigProvider);
			this.Name = bindingStorage.Name;
			this.MasterIdentity = bindingStorage.MasterIdentity;
			this.PolicyId = bindingStorage.PolicyId;
			this.Workload = bindingStorage.Workload;
			this.PolicyVersion = bindingStorage.PolicyVersion;
			this.RawAppliedScopes.Clear();
			foreach (ScopeStorage scopeStorage in bindingStorage.AppliedScopes)
			{
				string item = ExBindingStoreObject.ScopeStorageToString(scopeStorage);
				this.RawAppliedScopes.Add(item);
			}
		}

		public BindingStorage ToBindingStorage(ExPolicyConfigProvider policyConfigProvider)
		{
			ArgumentValidator.ThrowIfNull("policyConfigProvider", policyConfigProvider);
			BindingStorage bindingStorage = new BindingStorage();
			bindingStorage.SetId(PolicyStorage.PoliciesContainer.GetChildId(this.Name));
			bindingStorage.Name = this.Name;
			bindingStorage.MasterIdentity = this.MasterIdentity;
			bindingStorage.PolicyId = this.PolicyId;
			bindingStorage.Workload = this.Workload;
			bindingStorage.PolicyVersion = this.PolicyVersion;
			foreach (string scopeStorageString in this.RawAppliedScopes)
			{
				ScopeStorage scopeStorage = ExBindingStoreObject.ScopeStorageFromString(scopeStorageString, policyConfigProvider);
				if (scopeStorage != null)
				{
					bindingStorage.AppliedScopes.Add(scopeStorage);
				}
			}
			if (this.WhenChanged != null)
			{
				bindingStorage.propertyBag.SetField(ADObjectSchema.WhenChangedRaw, ADValueConvertor.ConvertValueToString(this.WhenChanged.Value, null));
			}
			if (this.WhenCreated != null)
			{
				bindingStorage.propertyBag.SetField(ADObjectSchema.WhenCreatedRaw, ADValueConvertor.ConvertValueToString(this.WhenCreated.Value, null));
			}
			bindingStorage.ResetChangeTracking(true);
			bindingStorage.RawObject = this;
			return bindingStorage;
		}

		private static string ScopeStorageToString(ScopeStorage scopeStorage)
		{
			return string.Format("{0},{1},{2},{3},{4}", new object[]
			{
				scopeStorage.MasterIdentity,
				scopeStorage.Mode,
				scopeStorage.PolicyVersion,
				scopeStorage.Name.Replace(',', ' '),
				scopeStorage.Scope
			});
		}

		private static ScopeStorage ScopeStorageFromString(string scopeStorageString, ExPolicyConfigProvider policyConfigProvider)
		{
			if (!string.IsNullOrEmpty(scopeStorageString))
			{
				Guid empty = Guid.Empty;
				Mode mode = Mode.Enforce;
				Guid empty2 = Guid.Empty;
				string[] array = scopeStorageString.Split(new char[]
				{
					','
				}, 5);
				if (array.Length == 5 && Guid.TryParse(array[0], out empty) && Enum.TryParse<Mode>(array[1], out mode) && Guid.TryParse(array[2], out empty2) && !string.IsNullOrEmpty(array[3]) && !string.IsNullOrEmpty(array[4]))
				{
					ScopeStorage scopeStorage = new ScopeStorage();
					scopeStorage.SetId(PolicyStorage.PoliciesContainer.GetChildId(empty.ToString()));
					scopeStorage.Name = array[3];
					scopeStorage.MasterIdentity = empty;
					scopeStorage.PolicyVersion = empty2;
					scopeStorage.Mode = mode;
					scopeStorage.Scope = array[4];
					scopeStorage.ResetChangeTracking(true);
					return scopeStorage;
				}
			}
			policyConfigProvider.LogOneEntry(ExecutionLog.EventType.CriticalError, "Convert string to scope storage", new FormatException(string.Format("'{0}' is not valid for scope storage.", scopeStorageString)));
			return null;
		}

		private static readonly ExBindingStoreObjectSchema schema = new ExBindingStoreObjectSchema();
	}
}
