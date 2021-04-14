using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidIdentityTypeForRemoveCmdletException : LocalizedException
	{
		public InvalidIdentityTypeForRemoveCmdletException(string identity) : base(Strings.InvalidIdentityTypeForRemoveCmdletException(identity))
		{
			this.identity = identity;
		}

		public InvalidIdentityTypeForRemoveCmdletException(string identity, Exception innerException) : base(Strings.InvalidIdentityTypeForRemoveCmdletException(identity), innerException)
		{
			this.identity = identity;
		}

		protected InvalidIdentityTypeForRemoveCmdletException(SerializationInfo info, StreamingContext context) : base(info, context)
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
