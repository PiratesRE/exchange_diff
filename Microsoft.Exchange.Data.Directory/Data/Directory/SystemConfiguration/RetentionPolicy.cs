using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class RetentionPolicy : MailboxPolicy
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RetentionPolicy.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RetentionPolicy.mostDerivedClass;
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return RetentionPolicy.parentPath;
			}
		}

		internal override bool IsShareable
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid RetentionId
		{
			get
			{
				if ((Guid)this[RetentionPolicySchema.RetentionId] == Guid.Empty)
				{
					return base.Guid;
				}
				return (Guid)this[RetentionPolicySchema.RetentionId];
			}
			set
			{
				this[RetentionPolicySchema.RetentionId] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> RetentionPolicyTagLinks
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[RetentionPolicySchema.RetentionPolicyTagLinks];
			}
			set
			{
				this[RetentionPolicySchema.RetentionPolicyTagLinks] = value;
			}
		}

		internal override bool CheckForAssociatedUsers()
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.DistinguishedName, base.Id.DistinguishedName),
				new ExistsFilter(RetentionPolicySchema.AssociatedUsers)
			});
			base.Session.SessionSettings.IsSharedConfigChecked = true;
			RetentionPolicy[] array = base.Session.Find<RetentionPolicy>(null, QueryScope.SubTree, filter, null, 1);
			return array != null && array.Length > 0;
		}

		public override bool IsDefault
		{
			get
			{
				return (bool)this[RetentionPolicySchema.IsDefault];
			}
			set
			{
				this[RetentionPolicySchema.IsDefault] = value;
			}
		}

		public bool IsDefaultArbitrationMailbox
		{
			get
			{
				return (bool)this[RetentionPolicySchema.IsDefaultArbitrationMailbox];
			}
			set
			{
				this[RetentionPolicySchema.IsDefaultArbitrationMailbox] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return RetentionPolicy.RetentionPolicyVersion;
			}
		}

		internal override void Initialize()
		{
			if (base.ExchangeVersion == RetentionPolicy.E14RetentionPolicyMajorVersion)
			{
				this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, RetentionPolicy.E14RetentionPolicyFullVersion);
			}
		}

		internal override QueryFilter VersioningFilter
		{
			get
			{
				ExchangeObjectVersion e14RetentionPolicyMajorVersion = RetentionPolicy.E14RetentionPolicyMajorVersion;
				ExchangeObjectVersion nextMajorVersion = e14RetentionPolicyMajorVersion.NextMajorVersion;
				return new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADObjectSchema.ExchangeVersion, e14RetentionPolicyMajorVersion),
					new ComparisonFilter(ComparisonOperator.LessThan, ADObjectSchema.ExchangeVersion, nextMajorVersion)
				});
			}
		}

		private static RetentionPolicySchema schema = ObjectSchema.GetInstance<RetentionPolicySchema>();

		private static string mostDerivedClass = "msExchMailboxRecipientTemplate";

		private static ADObjectId parentPath = new ADObjectId("CN=Retention Policies Container");

		internal static readonly ExchangeObjectVersion E14RetentionPolicyMajorVersion = ExchangeObjectVersion.Exchange2010.NextMajorVersion;

		internal static readonly ExchangeObjectVersion E14RetentionPolicyFullVersion = new ExchangeObjectVersion(1, 0, ExchangeObjectVersion.Exchange2010.ExchangeBuild);

		internal static readonly ExchangeObjectVersion RetentionPolicyVersion = RetentionPolicy.E14RetentionPolicyFullVersion;
	}
}
