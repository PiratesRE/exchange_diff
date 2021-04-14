using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToUpdateStoreMailboxInformationException : LocalizedException
	{
		public FailedToUpdateStoreMailboxInformationException(string identity) : base(Strings.ErrorFailedToUpdateStoreMailboxInformationException(identity))
		{
			this.identity = identity;
		}

		public FailedToUpdateStoreMailboxInformationException(string identity, Exception innerException) : base(Strings.ErrorFailedToUpdateStoreMailboxInformationException(identity), innerException)
		{
			this.identity = identity;
		}

		protected FailedToUpdateStoreMailboxInformationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string identity;
	}
}
