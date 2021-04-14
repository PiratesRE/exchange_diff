using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Fast
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DatabasePathDoesNotExist : ComponentFailedPermanentException
	{
		public DatabasePathDoesNotExist(string databasePath) : base(Strings.DatabasePathDoesNotExist(databasePath))
		{
			this.databasePath = databasePath;
		}

		public DatabasePathDoesNotExist(string databasePath, Exception innerException) : base(Strings.DatabasePathDoesNotExist(databasePath), innerException)
		{
			this.databasePath = databasePath;
		}

		protected DatabasePathDoesNotExist(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databasePath = (string)info.GetValue("databasePath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databasePath", this.databasePath);
		}

		public string DatabasePath
		{
			get
			{
				return this.databasePath;
			}
		}

		private readonly string databasePath;
	}
}
