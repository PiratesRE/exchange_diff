using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidIdentityTypeForClearCmdletException : LocalizedException
	{
		public InvalidIdentityTypeForClearCmdletException(string identity) : base(Strings.InvalidIdentityTypeForClearCmdletException(identity))
		{
			this.identity = identity;
		}

		public InvalidIdentityTypeForClearCmdletException(string identity, Exception innerException) : base(Strings.InvalidIdentityTypeForClearCmdletException(identity), innerException)
		{
			this.identity = identity;
		}

		protected InvalidIdentityTypeForClearCmdletException(SerializationInfo info, StreamingContext context) : base(info, context)
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
