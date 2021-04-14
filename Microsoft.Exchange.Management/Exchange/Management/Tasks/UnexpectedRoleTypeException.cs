using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnexpectedRoleTypeException : LocalizedException
	{
		public UnexpectedRoleTypeException(string roleDN, RoleType invalid, RoleType expected) : base(Strings.UnexpectedRoleTypeException(roleDN, invalid, expected))
		{
			this.roleDN = roleDN;
			this.invalid = invalid;
			this.expected = expected;
		}

		public UnexpectedRoleTypeException(string roleDN, RoleType invalid, RoleType expected, Exception innerException) : base(Strings.UnexpectedRoleTypeException(roleDN, invalid, expected), innerException)
		{
			this.roleDN = roleDN;
			this.invalid = invalid;
			this.expected = expected;
		}

		protected UnexpectedRoleTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.roleDN = (string)info.GetValue("roleDN", typeof(string));
			this.invalid = (RoleType)info.GetValue("invalid", typeof(RoleType));
			this.expected = (RoleType)info.GetValue("expected", typeof(RoleType));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("roleDN", this.roleDN);
			info.AddValue("invalid", this.invalid);
			info.AddValue("expected", this.expected);
		}

		public string RoleDN
		{
			get
			{
				return this.roleDN;
			}
		}

		public RoleType Invalid
		{
			get
			{
				return this.invalid;
			}
		}

		public RoleType Expected
		{
			get
			{
				return this.expected;
			}
		}

		private readonly string roleDN;

		private readonly RoleType invalid;

		private readonly RoleType expected;
	}
}
