using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AutoReseedNoCopiesException : AutoReseedException
	{
		public AutoReseedNoCopiesException(string databaseName) : base(ReplayStrings.AutoReseedNoCopiesException(databaseName))
		{
			this.databaseName = databaseName;
		}

		public AutoReseedNoCopiesException(string databaseName, Exception innerException) : base(ReplayStrings.AutoReseedNoCopiesException(databaseName), innerException)
		{
			this.databaseName = databaseName;
		}

		protected AutoReseedNoCopiesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		private readonly string databaseName;
	}
}
