using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorPolicyNotUniqueException : LocalizedException
	{
		public ErrorPolicyNotUniqueException(string policyParameter) : base(Strings.ErrorPolicyNotUnique(policyParameter))
		{
			this.policyParameter = policyParameter;
		}

		public ErrorPolicyNotUniqueException(string policyParameter, Exception innerException) : base(Strings.ErrorPolicyNotUnique(policyParameter), innerException)
		{
			this.policyParameter = policyParameter;
		}

		protected ErrorPolicyNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.policyParameter = (string)info.GetValue("policyParameter", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("policyParameter", this.policyParameter);
		}

		public string PolicyParameter
		{
			get
			{
				return this.policyParameter;
			}
		}

		private readonly string policyParameter;
	}
}
