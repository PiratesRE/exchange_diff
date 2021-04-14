using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorPolicyNotFoundException : LocalizedException
	{
		public ErrorPolicyNotFoundException(string policyParameter) : base(Strings.ErrorPolicyNotFound(policyParameter))
		{
			this.policyParameter = policyParameter;
		}

		public ErrorPolicyNotFoundException(string policyParameter, Exception innerException) : base(Strings.ErrorPolicyNotFound(policyParameter), innerException)
		{
			this.policyParameter = policyParameter;
		}

		protected ErrorPolicyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
