using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class RoleAssignmentObjectResolverRow : AdObjectResolverRow
	{
		public RoleAssignmentObjectResolverRow(ADRawEntry entry) : base(entry)
		{
		}

		public string Role
		{
			get
			{
				return this.RoleIdentity.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public ADObjectId RoleIdentity
		{
			get
			{
				return (ADObjectId)base.ADRawEntry[ExchangeRoleAssignmentSchema.Role];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public ConfigWriteScopeType ConfigWriteScopeType
		{
			get
			{
				ConfigWriteScopeType? configWriteScopeType = new ConfigWriteScopeType?((ConfigWriteScopeType)base.ADRawEntry[ExchangeRoleAssignmentSchema.ConfigWriteScope]);
				if (configWriteScopeType == null)
				{
					configWriteScopeType = new ConfigWriteScopeType?(ConfigWriteScopeType.None);
				}
				return configWriteScopeType.Value;
			}
		}

		public RecipientWriteScopeType RecipientWriteScopeType
		{
			get
			{
				return (RecipientWriteScopeType)base.ADRawEntry[ExchangeRoleAssignmentSchema.RecipientWriteScope];
			}
		}

		public ADObjectId CustomConfigWriteScope
		{
			get
			{
				return (ADObjectId)base.ADRawEntry[ExchangeRoleAssignmentSchema.CustomConfigWriteScope];
			}
		}

		public ADObjectId CustomRecipientWriteScope
		{
			get
			{
				if (this.RecipientWriteScopeType == RecipientWriteScopeType.OU)
				{
					return null;
				}
				return (ADObjectId)base.ADRawEntry[ExchangeRoleAssignmentSchema.CustomRecipientWriteScope];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public ADObjectId RecipientOrganizationUnitScope
		{
			get
			{
				if (this.RecipientWriteScopeType == RecipientWriteScopeType.OU)
				{
					return (ADObjectId)base.ADRawEntry[ExchangeRoleAssignmentSchema.CustomRecipientWriteScope];
				}
				return null;
			}
		}

		public bool IsDelegating
		{
			get
			{
				RoleAssignmentDelegationType? roleAssignmentDelegationType = new RoleAssignmentDelegationType?((RoleAssignmentDelegationType)base.ADRawEntry[ExchangeRoleAssignmentSchema.RoleAssignmentDelegationType]);
				return roleAssignmentDelegationType != null && roleAssignmentDelegationType != RoleAssignmentDelegationType.Regular;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public new static PropertyDefinition[] Properties = new List<PropertyDefinition>(AdObjectResolverRow.Properties)
		{
			ADRecipientSchema.DisplayName,
			ADRecipientSchema.RecipientTypeDetails,
			ExchangeRoleAssignmentSchema.ConfigWriteScope,
			ExchangeRoleAssignmentSchema.RecipientWriteScope,
			ExchangeRoleAssignmentSchema.Role,
			ExchangeRoleAssignmentSchema.CustomConfigWriteScope,
			ExchangeRoleAssignmentSchema.CustomRecipientWriteScope
		}.ToArray();
	}
}
