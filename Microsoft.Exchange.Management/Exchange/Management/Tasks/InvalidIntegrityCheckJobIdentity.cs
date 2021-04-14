using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidIntegrityCheckJobIdentity : LocalizedException
	{
		public InvalidIntegrityCheckJobIdentity(string identity) : base(Strings.InvalidIntegrityCheckJobIdentity(identity))
		{
			this.identity = identity;
		}

		public InvalidIntegrityCheckJobIdentity(string identity, Exception innerException) : base(Strings.InvalidIntegrityCheckJobIdentity(identity), innerException)
		{
			this.identity = identity;
		}

		protected InvalidIntegrityCheckJobIdentity(SerializationInfo info, StreamingContext context) : base(info, context)
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
