using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class PolicyAndIdentityParameterUsedTogetherException : LocalizedException
	{
		public PolicyAndIdentityParameterUsedTogetherException(string policy, string identity) : base(Strings.PolicyAndIdentityParameterUsedTogether(policy, identity))
		{
			this.policy = policy;
			this.identity = identity;
		}

		public PolicyAndIdentityParameterUsedTogetherException(string policy, string identity, Exception innerException) : base(Strings.PolicyAndIdentityParameterUsedTogether(policy, identity), innerException)
		{
			this.policy = policy;
			this.identity = identity;
		}

		protected PolicyAndIdentityParameterUsedTogetherException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.policy = (string)info.GetValue("policy", typeof(string));
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("policy", this.policy);
			info.AddValue("identity", this.identity);
		}

		public string Policy
		{
			get
			{
				return this.policy;
			}
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string policy;

		private readonly string identity;
	}
}
