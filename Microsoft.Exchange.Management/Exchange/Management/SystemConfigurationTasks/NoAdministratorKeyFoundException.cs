using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoAdministratorKeyFoundException : FederationException
	{
		public NoAdministratorKeyFoundException(string trustName) : base(Strings.ErrorNoAdministratorKeyFound(trustName))
		{
			this.trustName = trustName;
		}

		public NoAdministratorKeyFoundException(string trustName, Exception innerException) : base(Strings.ErrorNoAdministratorKeyFound(trustName), innerException)
		{
			this.trustName = trustName;
		}

		protected NoAdministratorKeyFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.trustName = (string)info.GetValue("trustName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("trustName", this.trustName);
		}

		public string TrustName
		{
			get
			{
				return this.trustName;
			}
		}

		private readonly string trustName;
	}
}
