using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRemoveAssociatedThrottlingPolicyException : LocalizedException
	{
		public CannotRemoveAssociatedThrottlingPolicyException(string policyName) : base(Strings.ErrorCannotRemoveAssociatedThrottlingPolicy(policyName))
		{
			this.policyName = policyName;
		}

		public CannotRemoveAssociatedThrottlingPolicyException(string policyName, Exception innerException) : base(Strings.ErrorCannotRemoveAssociatedThrottlingPolicy(policyName), innerException)
		{
			this.policyName = policyName;
		}

		protected CannotRemoveAssociatedThrottlingPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.policyName = (string)info.GetValue("policyName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("policyName", this.policyName);
		}

		public string PolicyName
		{
			get
			{
				return this.policyName;
			}
		}

		private readonly string policyName;
	}
}
