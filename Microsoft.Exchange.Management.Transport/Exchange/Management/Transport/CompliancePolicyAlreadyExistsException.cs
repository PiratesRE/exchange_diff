using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CompliancePolicyAlreadyExistsException : LocalizedException
	{
		public CompliancePolicyAlreadyExistsException(string policyName) : base(Strings.CompliancePolicyAlreadyExists(policyName))
		{
			this.policyName = policyName;
		}

		public CompliancePolicyAlreadyExistsException(string policyName, Exception innerException) : base(Strings.CompliancePolicyAlreadyExists(policyName), innerException)
		{
			this.policyName = policyName;
		}

		protected CompliancePolicyAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
