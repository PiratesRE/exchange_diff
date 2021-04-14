using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OnlineIsIntegRemoveJobException : LocalizedException
	{
		public OnlineIsIntegRemoveJobException(string database, string job, string failure) : base(Strings.OnlineIsIntegRemoveJobException(database, job, failure))
		{
			this.database = database;
			this.job = job;
			this.failure = failure;
		}

		public OnlineIsIntegRemoveJobException(string database, string job, string failure, Exception innerException) : base(Strings.OnlineIsIntegRemoveJobException(database, job, failure), innerException)
		{
			this.database = database;
			this.job = job;
			this.failure = failure;
		}

		protected OnlineIsIntegRemoveJobException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.job = (string)info.GetValue("job", typeof(string));
			this.failure = (string)info.GetValue("failure", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
			info.AddValue("job", this.job);
			info.AddValue("failure", this.failure);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string Job
		{
			get
			{
				return this.job;
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

		private readonly string job;

		private readonly string failure;
	}
}
