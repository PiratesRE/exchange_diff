using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidArgumentForServerRoleException : LocalizedException
	{
		public InvalidArgumentForServerRoleException(string property, string role) : base(Strings.InvalidArgumentForServerRoleException(property, role))
		{
			this.property = property;
			this.role = role;
		}

		public InvalidArgumentForServerRoleException(string property, string role, Exception innerException) : base(Strings.InvalidArgumentForServerRoleException(property, role), innerException)
		{
			this.property = property;
			this.role = role;
		}

		protected InvalidArgumentForServerRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.property = (string)info.GetValue("property", typeof(string));
			this.role = (string)info.GetValue("role", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("property", this.property);
			info.AddValue("role", this.role);
		}

		public string Property
		{
			get
			{
				return this.property;
			}
		}

		public string Role
		{
			get
			{
				return this.role;
			}
		}

		private readonly string property;

		private readonly string role;
	}
}
