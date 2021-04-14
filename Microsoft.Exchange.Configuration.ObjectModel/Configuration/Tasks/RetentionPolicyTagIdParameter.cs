using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RetentionPolicyTagIdParameter : ADIdParameter
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Dehydrateable;
			}
		}

		public RetentionPolicyTagIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public RetentionPolicyTagIdParameter(string identity) : base(identity)
		{
		}

		public RetentionPolicyTagIdParameter(RetentionPolicyTag policyTag) : base(policyTag.Id)
		{
		}

		public RetentionPolicyTagIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public RetentionPolicyTagIdParameter()
		{
		}

		public static RetentionPolicyTagIdParameter Parse(string rawString)
		{
			return new RetentionPolicyTagIdParameter(rawString);
		}
	}
}
