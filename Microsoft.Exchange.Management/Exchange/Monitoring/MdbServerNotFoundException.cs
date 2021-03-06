using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MdbServerNotFoundException : LocalizedException
	{
		public MdbServerNotFoundException(string database) : base(Strings.MdbServerNotFoundException(database))
		{
			this.database = database;
		}

		public MdbServerNotFoundException(string database, Exception innerException) : base(Strings.MdbServerNotFoundException(database), innerException)
		{
			this.database = database;
		}

		protected MdbServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
