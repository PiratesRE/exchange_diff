using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MultipleDefaultFoldersInPolicyException : LocalizedException
	{
		public MultipleDefaultFoldersInPolicyException(string policyName, string userName) : base(Strings.MultipleDefaultFoldersInPolicyException(policyName, userName))
		{
			this.policyName = policyName;
			this.userName = userName;
		}

		public MultipleDefaultFoldersInPolicyException(string policyName, string userName, Exception innerException) : base(Strings.MultipleDefaultFoldersInPolicyException(policyName, userName), innerException)
		{
			this.policyName = policyName;
			this.userName = userName;
		}

		protected MultipleDefaultFoldersInPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.policyName = (string)info.GetValue("policyName", typeof(string));
			this.userName = (string)info.GetValue("userName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("policyName", this.policyName);
			info.AddValue("userName", this.userName);
		}

		public string PolicyName
		{
			get
			{
				return this.policyName;
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		private readonly string policyName;

		private readonly string userName;
	}
}
