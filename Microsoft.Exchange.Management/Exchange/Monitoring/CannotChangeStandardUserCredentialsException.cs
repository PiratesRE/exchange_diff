using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotChangeStandardUserCredentialsException : LocalizedException
	{
		public CannotChangeStandardUserCredentialsException(string username) : base(Strings.CannotChangeStandardUserCredentials(username))
		{
			this.username = username;
		}

		public CannotChangeStandardUserCredentialsException(string username, Exception innerException) : base(Strings.CannotChangeStandardUserCredentials(username), innerException)
		{
			this.username = username;
		}

		protected CannotChangeStandardUserCredentialsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.username = (string)info.GetValue("username", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("username", this.username);
		}

		public string Username
		{
			get
			{
				return this.username;
			}
		}

		private readonly string username;
	}
}
