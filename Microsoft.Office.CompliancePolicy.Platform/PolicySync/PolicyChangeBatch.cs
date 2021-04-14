using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[KnownType(typeof(AssociationConfiguration))]
	[KnownType(typeof(TenantCookie))]
	[DataContract]
	[KnownType(typeof(TenantCookieCollection))]
	[KnownType(typeof(RuleConfiguration))]
	[KnownType(typeof(ScopeConfiguration))]
	[KnownType(typeof(BindingConfiguration))]
	[KnownType(typeof(PolicyConfiguration))]
	public sealed class PolicyChangeBatch
	{
		[DataMember]
		public IEnumerable<PolicyConfigurationBase> Changes { get; set; }

		[DataMember]
		public TenantCookieCollection NewCookies { get; set; }
	}
}
