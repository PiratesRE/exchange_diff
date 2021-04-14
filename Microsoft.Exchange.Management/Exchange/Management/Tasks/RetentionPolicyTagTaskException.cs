using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RetentionPolicyTagTaskException : LocalizedException
	{
		public RetentionPolicyTagTaskException(string reason) : base(Strings.RetentionPolicyTagTaskException(reason))
		{
			this.reason = reason;
		}

		public RetentionPolicyTagTaskException(string reason, Exception innerException) : base(Strings.RetentionPolicyTagTaskException(reason), innerException)
		{
			this.reason = reason;
		}

		protected RetentionPolicyTagTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string reason;
	}
}
