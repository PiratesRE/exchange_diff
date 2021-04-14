using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AuthenticationMethodNotSupportedException : MigrationPermanentException
	{
		public AuthenticationMethodNotSupportedException(string authenticationMethod, string protocol, string validValues) : base(Strings.ErrorAuthenticationMethodNotSupported(authenticationMethod, protocol, validValues))
		{
			this.authenticationMethod = authenticationMethod;
			this.protocol = protocol;
			this.validValues = validValues;
		}

		public AuthenticationMethodNotSupportedException(string authenticationMethod, string protocol, string validValues, Exception innerException) : base(Strings.ErrorAuthenticationMethodNotSupported(authenticationMethod, protocol, validValues), innerException)
		{
			this.authenticationMethod = authenticationMethod;
			this.protocol = protocol;
			this.validValues = validValues;
		}

		protected AuthenticationMethodNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.authenticationMethod = (string)info.GetValue("authenticationMethod", typeof(string));
			this.protocol = (string)info.GetValue("protocol", typeof(string));
			this.validValues = (string)info.GetValue("validValues", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("authenticationMethod", this.authenticationMethod);
			info.AddValue("protocol", this.protocol);
			info.AddValue("validValues", this.validValues);
		}

		public string AuthenticationMethod
		{
			get
			{
				return this.authenticationMethod;
			}
		}

		public string Protocol
		{
			get
			{
				return this.protocol;
			}
		}

		public string ValidValues
		{
			get
			{
				return this.validValues;
			}
		}

		private readonly string authenticationMethod;

		private readonly string protocol;

		private readonly string validValues;
	}
}
