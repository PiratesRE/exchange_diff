using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AddDatabaseCopyAllCopiesMustBeInSameDagException : LocalizedException
	{
		public AddDatabaseCopyAllCopiesMustBeInSameDagException(string databaseName, string server1, string dag1, string server2, string dag2) : base(Strings.AddDatabaseCopyAllCopiesMustBeInSameDagException(databaseName, server1, dag1, server2, dag2))
		{
			this.databaseName = databaseName;
			this.server1 = server1;
			this.dag1 = dag1;
			this.server2 = server2;
			this.dag2 = dag2;
		}

		public AddDatabaseCopyAllCopiesMustBeInSameDagException(string databaseName, string server1, string dag1, string server2, string dag2, Exception innerException) : base(Strings.AddDatabaseCopyAllCopiesMustBeInSameDagException(databaseName, server1, dag1, server2, dag2), innerException)
		{
			this.databaseName = databaseName;
			this.server1 = server1;
			this.dag1 = dag1;
			this.server2 = server2;
			this.dag2 = dag2;
		}

		protected AddDatabaseCopyAllCopiesMustBeInSameDagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
			this.server1 = (string)info.GetValue("server1", typeof(string));
			this.dag1 = (string)info.GetValue("dag1", typeof(string));
			this.server2 = (string)info.GetValue("server2", typeof(string));
			this.dag2 = (string)info.GetValue("dag2", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
			info.AddValue("server1", this.server1);
			info.AddValue("dag1", this.dag1);
			info.AddValue("server2", this.server2);
			info.AddValue("dag2", this.dag2);
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public string Server1
		{
			get
			{
				return this.server1;
			}
		}

		public string Dag1
		{
			get
			{
				return this.dag1;
			}
		}

		public string Server2
		{
			get
			{
				return this.server2;
			}
		}

		public string Dag2
		{
			get
			{
				return this.dag2;
			}
		}

		private readonly string databaseName;

		private readonly string server1;

		private readonly string dag1;

		private readonly string server2;

		private readonly string dag2;
	}
}
