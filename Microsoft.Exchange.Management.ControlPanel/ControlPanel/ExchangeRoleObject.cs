using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ExchangeRoleObject : AdObjectResolverRow
	{
		public ExchangeRoleObject(ADRawEntry entry) : base(entry)
		{
		}

		public string MailboxPlanIndex
		{
			get
			{
				return (string)base.ADRawEntry[ExchangeRoleSchema.MailboxPlanIndex];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public bool IsEndUserRole
		{
			get
			{
				return (bool)base.ADRawEntry[ExchangeRoleSchema.IsEndUserRole];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public bool IsRootRole
		{
			get
			{
				return (bool)base.ADRawEntry[ExchangeRoleSchema.IsRootRole];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public RoleType RoleType
		{
			get
			{
				return (RoleType)base.ADRawEntry[ExchangeRoleSchema.RoleType];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public new static PropertyDefinition[] Properties = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			ExchangeRoleSchema.IsEndUserRole,
			ExchangeRoleSchema.IsRootRole,
			ExchangeRoleSchema.MailboxPlanIndex,
			ExchangeRoleSchema.RoleType
		};
	}
}
