using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Migration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MigrationUserNotFoundException : LocalizedException
	{
		public MigrationUserNotFoundException(string userName) : base(Strings.MigrationUserNotFound(userName))
		{
			this.userName = userName;
		}

		public MigrationUserNotFoundException(string userName, Exception innerException) : base(Strings.MigrationUserNotFound(userName), innerException)
		{
			this.userName = userName;
		}

		protected MigrationUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userName = (string)info.GetValue("userName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userName", this.userName);
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		private readonly string userName;
	}
}
