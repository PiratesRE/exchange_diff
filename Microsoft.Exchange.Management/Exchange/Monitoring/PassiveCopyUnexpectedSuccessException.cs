using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PassiveCopyUnexpectedSuccessException : LocalizedException
	{
		public PassiveCopyUnexpectedSuccessException(string database) : base(Strings.messagePassiveCopyUnexpectedSuccessException(database))
		{
			this.database = database;
		}

		public PassiveCopyUnexpectedSuccessException(string database, Exception innerException) : base(Strings.messagePassiveCopyUnexpectedSuccessException(database), innerException)
		{
			this.database = database;
		}

		protected PassiveCopyUnexpectedSuccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		private readonly string database;
	}
}
