using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidIdentityException : LocalizedException
	{
		public InvalidIdentityException(string identity, string format) : base(Strings.InvalidIdentity(identity, format))
		{
			this.identity = identity;
			this.format = format;
		}

		public InvalidIdentityException(string identity, string format, Exception innerException) : base(Strings.InvalidIdentity(identity, format), innerException)
		{
			this.identity = identity;
			this.format = format;
		}

		protected InvalidIdentityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
			this.format = (string)info.GetValue("format", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
			info.AddValue("format", this.format);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string Format
		{
			get
			{
				return this.format;
			}
		}

		private readonly string identity;

		private readonly string format;
	}
}
