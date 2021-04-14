using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UMMbxPolicyNotFoundException : LocalizedException
	{
		public UMMbxPolicyNotFoundException(string policy, string user) : base(Strings.UMMbxPolicyNotFound(policy, user))
		{
			this.policy = policy;
			this.user = user;
		}

		public UMMbxPolicyNotFoundException(string policy, string user, Exception innerException) : base(Strings.UMMbxPolicyNotFound(policy, user), innerException)
		{
			this.policy = policy;
			this.user = user;
		}

		protected UMMbxPolicyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.policy = (string)info.GetValue("policy", typeof(string));
			this.user = (string)info.GetValue("user", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("policy", this.policy);
			info.AddValue("user", this.user);
		}

		public string Policy
		{
			get
			{
				return this.policy;
			}
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		private readonly string policy;

		private readonly string user;
	}
}
