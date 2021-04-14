using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ThirdPartyReplicationException : LocalizedException
	{
		public ThirdPartyReplicationException(string error) : base(ThirdPartyReplication.TPRBaseError(error))
		{
			this.error = error;
		}

		public ThirdPartyReplicationException(string error, Exception innerException) : base(ThirdPartyReplication.TPRBaseError(error), innerException)
		{
			this.error = error;
		}

		protected ThirdPartyReplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
