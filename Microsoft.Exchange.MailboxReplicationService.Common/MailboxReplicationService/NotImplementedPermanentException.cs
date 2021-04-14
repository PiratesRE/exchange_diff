using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotImplementedPermanentException : MailboxReplicationPermanentException
	{
		public NotImplementedPermanentException(string methodName) : base(MrsStrings.NotImplemented(methodName))
		{
			this.methodName = methodName;
		}

		public NotImplementedPermanentException(string methodName, Exception innerException) : base(MrsStrings.NotImplemented(methodName), innerException)
		{
			this.methodName = methodName;
		}

		protected NotImplementedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.methodName = (string)info.GetValue("methodName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("methodName", this.methodName);
		}

		public string MethodName
		{
			get
			{
				return this.methodName;
			}
		}

		private readonly string methodName;
	}
}
