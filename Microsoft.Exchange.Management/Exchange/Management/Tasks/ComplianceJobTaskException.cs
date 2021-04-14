using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ComplianceJobTaskException : LocalizedException
	{
		public ComplianceJobTaskException(string failure) : base(Strings.ComplianceJobTaskException(failure))
		{
			this.failure = failure;
		}

		public ComplianceJobTaskException(string failure, Exception innerException) : base(Strings.ComplianceJobTaskException(failure), innerException)
		{
			this.failure = failure;
		}

		protected ComplianceJobTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.failure = (string)info.GetValue("failure", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("failure", this.failure);
		}

		public string Failure
		{
			get
			{
				return this.failure;
			}
		}

		private readonly string failure;
	}
}
