using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnsupportedAuthMechanismException : LocalizedException
	{
		public UnsupportedAuthMechanismException(string authMechanism) : base(NetException.UnsupportedAuthMechanismException(authMechanism))
		{
			this.authMechanism = authMechanism;
		}

		public UnsupportedAuthMechanismException(string authMechanism, Exception innerException) : base(NetException.UnsupportedAuthMechanismException(authMechanism), innerException)
		{
			this.authMechanism = authMechanism;
		}

		protected UnsupportedAuthMechanismException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.authMechanism = (string)info.GetValue("authMechanism", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("authMechanism", this.authMechanism);
		}

		public string AuthMechanism
		{
			get
			{
				return this.authMechanism;
			}
		}

		private readonly string authMechanism;
	}
}
