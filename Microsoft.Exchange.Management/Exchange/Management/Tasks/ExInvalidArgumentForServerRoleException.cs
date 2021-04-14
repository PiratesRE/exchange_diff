using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExInvalidArgumentForServerRoleException : LocalizedException
	{
		public ExInvalidArgumentForServerRoleException(string property, string role) : base(Strings.ExInvalidArgumentForServerRoleException(property, role))
		{
			this.property = property;
			this.role = role;
		}

		public ExInvalidArgumentForServerRoleException(string property, string role, Exception innerException) : base(Strings.ExInvalidArgumentForServerRoleException(property, role), innerException)
		{
			this.property = property;
			this.role = role;
		}

		protected ExInvalidArgumentForServerRoleException(SerializationInfo info, StreamingContext context) : base(info, context)
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
