using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MulipleComplianceRulesFoundInPolicyException : LocalizedException
	{
		public MulipleComplianceRulesFoundInPolicyException(string policy) : base(Strings.MulipleComplianceRulesFoundInPolicy(policy))
		{
			this.policy = policy;
		}

		public MulipleComplianceRulesFoundInPolicyException(string policy, Exception innerException) : base(Strings.MulipleComplianceRulesFoundInPolicy(policy), innerException)
		{
			this.policy = policy;
		}

		protected MulipleComplianceRulesFoundInPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.policy = (string)info.GetValue("policy", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("policy", this.policy);
		}

		public string Policy
		{
			get
			{
				return this.policy;
			}
		}

		private readonly string policy;
	}
}
