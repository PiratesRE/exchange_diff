using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdminGroupNotFoundException : LocalizedException
	{
		public AdminGroupNotFoundException(string adminGroup) : base(Strings.AdminGroupNotFoundException(adminGroup))
		{
			this.adminGroup = adminGroup;
		}

		public AdminGroupNotFoundException(string adminGroup, Exception innerException) : base(Strings.AdminGroupNotFoundException(adminGroup), innerException)
		{
			this.adminGroup = adminGroup;
		}

		protected AdminGroupNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.adminGroup = (string)info.GetValue("adminGroup", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("adminGroup", this.adminGroup);
		}

		public string AdminGroup
		{
			get
			{
				return this.adminGroup;
			}
		}

		private readonly string adminGroup;
	}
}
