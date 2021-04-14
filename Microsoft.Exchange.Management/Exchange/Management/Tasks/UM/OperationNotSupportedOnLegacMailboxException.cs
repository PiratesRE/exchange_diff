using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OperationNotSupportedOnLegacMailboxException : LocalizedException
	{
		public OperationNotSupportedOnLegacMailboxException(string use) : base(Strings.OperationNotSupportedOnLegacMailboxException(use))
		{
			this.use = use;
		}

		public OperationNotSupportedOnLegacMailboxException(string use, Exception innerException) : base(Strings.OperationNotSupportedOnLegacMailboxException(use), innerException)
		{
			this.use = use;
		}

		protected OperationNotSupportedOnLegacMailboxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.use = (string)info.GetValue("use", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("use", this.use);
		}

		public string Use
		{
			get
			{
				return this.use;
			}
		}

		private readonly string use;
	}
}
