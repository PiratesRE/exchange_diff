using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ThrottlingPolicyCorrupted : ADExternalException
	{
		public ThrottlingPolicyCorrupted(string policyId) : base(DirectoryStrings.ThrottlingPolicyCorrupted(policyId))
		{
			this.policyId = policyId;
		}

		public ThrottlingPolicyCorrupted(string policyId, Exception innerException) : base(DirectoryStrings.ThrottlingPolicyCorrupted(policyId), innerException)
		{
			this.policyId = policyId;
		}

		protected ThrottlingPolicyCorrupted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.policyId = (string)info.GetValue("policyId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("policyId", this.policyId);
		}

		public string PolicyId
		{
			get
			{
				return this.policyId;
			}
		}

		private readonly string policyId;
	}
}
