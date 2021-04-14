using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ConstraintErrorException : LocalizedException
	{
		public ConstraintErrorException(DataMoveReplicationConstraintParameter desired, string database) : base(Strings.ConstraintError(desired, database))
		{
			this.desired = desired;
			this.database = database;
		}

		public ConstraintErrorException(DataMoveReplicationConstraintParameter desired, string database, Exception innerException) : base(Strings.ConstraintError(desired, database), innerException)
		{
			this.desired = desired;
			this.database = database;
		}

		protected ConstraintErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.desired = (DataMoveReplicationConstraintParameter)info.GetValue("desired", typeof(DataMoveReplicationConstraintParameter));
			this.database = (string)info.GetValue("database", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("desired", this.desired);
			info.AddValue("database", this.database);
		}

		public DataMoveReplicationConstraintParameter Desired
		{
			get
			{
				return this.desired;
			}
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		private readonly DataMoveReplicationConstraintParameter desired;

		private readonly string database;
	}
}
