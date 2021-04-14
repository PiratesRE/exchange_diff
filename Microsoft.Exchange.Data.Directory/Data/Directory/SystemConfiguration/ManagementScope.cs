using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class ManagementScope : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ManagementScope.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ManagementScope.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ManagementScopeSchema.ExchangeManagementScope2010_SPVersion;
			}
		}

		public ADObjectId RecipientRoot
		{
			get
			{
				return (ADObjectId)this[ManagementScopeSchema.RecipientRoot];
			}
			internal set
			{
				this[ManagementScopeSchema.RecipientRoot] = value;
			}
		}

		internal string Filter
		{
			get
			{
				return (string)this[ManagementScopeSchema.Filter];
			}
			set
			{
				this[ManagementScopeSchema.Filter] = value;
			}
		}

		public string RecipientFilter
		{
			get
			{
				return (string)this[ManagementScopeSchema.RecipientFilter];
			}
			internal set
			{
				this[ManagementScopeSchema.RecipientFilter] = value;
			}
		}

		public string ServerFilter
		{
			get
			{
				return (string)this[ManagementScopeSchema.ServerFilter];
			}
			internal set
			{
				this[ManagementScopeSchema.ServerFilter] = value;
			}
		}

		public string DatabaseFilter
		{
			get
			{
				return (string)this[ManagementScopeSchema.DatabaseFilter];
			}
			internal set
			{
				this[ManagementScopeSchema.DatabaseFilter] = value;
			}
		}

		public string TenantOrganizationFilter
		{
			get
			{
				return (string)this[ManagementScopeSchema.OrganizationFilter];
			}
			internal set
			{
				this[ManagementScopeSchema.OrganizationFilter] = value;
			}
		}

		public ScopeRestrictionType ScopeRestrictionType
		{
			get
			{
				return (ScopeRestrictionType)this[ManagementScopeSchema.ScopeRestrictionType];
			}
			internal set
			{
				this[ManagementScopeSchema.ScopeRestrictionType] = value;
			}
		}

		public bool Exclusive
		{
			get
			{
				return (bool)this[ManagementScopeSchema.Exclusive];
			}
			internal set
			{
				this[ManagementScopeSchema.Exclusive] = value;
			}
		}

		internal QueryFilter QueryFilter
		{
			get
			{
				return this.queryFilter;
			}
			set
			{
				this.queryFilter = value;
			}
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			base.ValidateRead(errors);
			bool flag = ADObjectId.IsNullOrEmpty(this.RecipientRoot);
			switch (this.ScopeRestrictionType)
			{
			case ScopeRestrictionType.RecipientScope:
				break;
			case ScopeRestrictionType.ServerScope:
			case ScopeRestrictionType.PartnerDelegatedTenantScope:
			case ScopeRestrictionType.DatabaseScope:
				if (!flag)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.RootMustBeEmpty(this.ScopeRestrictionType), this.Identity, base.OriginatingServer));
				}
				break;
			default:
				errors.Add(new ObjectValidationError(DirectoryStrings.UnKnownScopeRestrictionType(this.ScopeRestrictionType.ToString(), base.Name), this.Identity, base.OriginatingServer));
				break;
			}
			if (string.IsNullOrEmpty(this.Filter))
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.FilterCannotBeEmpty(this.ScopeRestrictionType), this.Identity, base.OriginatingServer));
			}
			if (this.Exclusive && this.ScopeRestrictionType == ScopeRestrictionType.PartnerDelegatedTenantScope)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ScopeCannotBeExclusive(this.ScopeRestrictionType), this.Identity, base.OriginatingServer));
			}
		}

		internal bool IsScopeSmallerOrEqualThan(ManagementScope scope, out LocalizedString notTrueReason)
		{
			notTrueReason = LocalizedString.Empty;
			if (object.ReferenceEquals(this, scope))
			{
				return true;
			}
			if (scope == null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug(11000L, "IsScopeSmallerOrEqualThan: cannot compare this instance with $NULL");
				return false;
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<ADObjectId, ADObjectId>(11000L, "-->IsScopeSmallerOrEqualThan: this = {0}, other = {1}", base.Id, scope.Id);
			if (this.ScopeRestrictionType == scope.ScopeRestrictionType && ADObjectId.Equals(this.RecipientRoot, scope.RecipientRoot) && string.Equals(this.Filter, scope.Filter, StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug(11000L, "IsScopeSmallerOrEqualThan: both management scopes have exactly the same restrction type, root and filter.");
				return true;
			}
			if (this.ScopeRestrictionType == ScopeRestrictionType.RecipientScope && scope.ScopeRestrictionType == ScopeRestrictionType.RecipientScope && string.Equals(this.Filter, scope.Filter, StringComparison.OrdinalIgnoreCase) && this.RecipientRoot != null && scope.RecipientRoot != null)
			{
				ExTraceGlobals.AccessCheckTracer.TraceDebug<ADObjectId, ADObjectId>(11000L, "IsScopeSmallerOrEqualThan: comparing the Root of two scope objects: this (Root = {0}), other (Root = {1})", this.RecipientRoot, scope.RecipientRoot);
				if (this.RecipientRoot.IsDescendantOf(scope.RecipientRoot))
				{
					return true;
				}
				notTrueReason = DirectoryStrings.OUsNotSmallerOrEqual(this.RecipientRoot.ToString(), scope.RecipientRoot.ToString());
			}
			else
			{
				notTrueReason = DirectoryStrings.CannotCompareScopeObjects(base.Id.ToString(), scope.Id.ToString());
				ExTraceGlobals.AccessCheckTracer.TraceDebug(11000L, "IsScopeSmallerOrEqualThan: non-comparable scope objects. We can only compare scope scope objects of RecipientScope with non-empty root and equivalent filter.");
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(11000L, "<--IsScopeSmallerOrEqualThan: return false");
			return false;
		}

		internal bool IsFullyCoveredByOU(ADObjectId ouIdentity, out LocalizedString notTrueReason)
		{
			notTrueReason = LocalizedString.Empty;
			if (ouIdentity == null)
			{
				throw new ArgumentNullException("ouIdentity");
			}
			if (string.IsNullOrEmpty(ouIdentity.DistinguishedName))
			{
				throw new ArgumentNullException("ouIdentity.DistinguishedName");
			}
			if (ADObjectId.IsNullOrEmpty(this.RecipientRoot))
			{
				notTrueReason = DirectoryStrings.RootCannotBeEmpty(this.ScopeRestrictionType);
				ExTraceGlobals.AccessCheckTracer.TraceError(11001L, "IsFullyCoveredByOU: this instance has invalid empty Root.");
				return false;
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction(11001L, "-->IsFullyCoveredByOU: this = (Id = {0}, Root = {1}, Type = {2}), OU = {3}", new object[]
			{
				base.Id,
				this.RecipientRoot,
				this.ScopeRestrictionType,
				ouIdentity.DistinguishedName
			});
			bool flag = false;
			if (this.ScopeRestrictionType == ScopeRestrictionType.RecipientScope)
			{
				flag = this.RecipientRoot.IsDescendantOf(ouIdentity);
				if (!flag)
				{
					notTrueReason = DirectoryStrings.OUsNotSmallerOrEqual(this.RecipientRoot.ToString(), ouIdentity.ToString());
				}
			}
			else
			{
				notTrueReason = DirectoryStrings.CannotCompareScopeObjectWithOU(base.Id.ToString(), this.ScopeRestrictionType.ToString(), ouIdentity.ToString());
			}
			ExTraceGlobals.AccessCheckTracer.TraceFunction<bool>(11001L, "<--IsScopeSmallerOrEqualThan: return {0}", flag);
			return flag;
		}

		public static readonly ADObjectId RdnScopesContainerToOrganization = new ADObjectId("CN=Scopes,CN=RBAC");

		private static ManagementScopeSchema schema = ObjectSchema.GetInstance<ManagementScopeSchema>();

		private static string mostDerivedClass = "msExchScope";

		private QueryFilter queryFilter;
	}
}
