using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ManagementRoleResolveRow : AdObjectResolverRow
	{
		public ManagementRoleResolveRow(ADRawEntry aDRawEntry) : base(aDRawEntry)
		{
		}

		public string Name
		{
			get
			{
				return this.DisplayName;
			}
		}

		public string Description
		{
			get
			{
				return (string)base.ADRawEntry[ExchangeRoleSchema.Description];
			}
		}

		public ScopeType ImplicitRecipientWriteScopeType
		{
			get
			{
				return (ScopeType)base.ADRawEntry[ExchangeRoleSchema.ImplicitRecipientWriteScope];
			}
		}

		public ScopeType ImplicitConfigWriteScopeType
		{
			get
			{
				return (ScopeType)base.ADRawEntry[ExchangeRoleSchema.ImplicitConfigWriteScope];
			}
		}

		public new static PropertyDefinition[] Properties = new List<PropertyDefinition>(AdObjectResolverRow.Properties)
		{
			ADObjectSchema.Name,
			ExchangeRoleSchema.Description,
			ExchangeRoleSchema.ImplicitConfigWriteScope,
			ExchangeRoleSchema.ImplicitRecipientWriteScope
		}.ToArray();
	}
}
