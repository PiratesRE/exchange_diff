using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToRemoveDatabaseSectionException : LocalizedException
	{
		public FailedToRemoveDatabaseSectionException(string database) : base(Strings.FailedToRemoveDatabaseSection(database))
		{
			this.database = database;
		}

		public FailedToRemoveDatabaseSectionException(string database, Exception innerException) : base(Strings.FailedToRemoveDatabaseSection(database), innerException)
		{
			this.database = database;
		}

		protected FailedToRemoveDatabaseSectionException(SerializationInfo info, StreamingContext context) : base(info, context)
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
