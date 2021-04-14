using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OnlineIsIntegQueryJobException : LocalizedException
	{
		public OnlineIsIntegQueryJobException(string database, string failure) : base(Strings.OnlineIsIntegQueryJobException(database, failure))
		{
			this.database = database;
			this.failure = failure;
		}

		public OnlineIsIntegQueryJobException(string database, string failure, Exception innerException) : base(Strings.OnlineIsIntegQueryJobException(database, failure), innerException)
		{
			this.database = database;
			this.failure = failure;
		}

		protected OnlineIsIntegQueryJobException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.failure = (string)info.GetValue("failure", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
			info.AddValue("failure", this.failure);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string Failure
		{
			get
			{
				return this.failure;
			}
		}

		private readonly string database;

		private readonly string failure;
	}
}
