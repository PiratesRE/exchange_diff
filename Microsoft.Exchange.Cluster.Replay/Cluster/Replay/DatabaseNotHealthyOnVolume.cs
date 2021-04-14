using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseNotHealthyOnVolume : DatabaseVolumeInfoException
	{
		public DatabaseNotHealthyOnVolume(string databaseName, string volumeName) : base(ReplayStrings.DatabaseNotHealthyOnVolume(databaseName, volumeName))
		{
			this.databaseName = databaseName;
			this.volumeName = volumeName;
		}

		public DatabaseNotHealthyOnVolume(string databaseName, string volumeName, Exception innerException) : base(ReplayStrings.DatabaseNotHealthyOnVolume(databaseName, volumeName), innerException)
		{
			this.databaseName = databaseName;
			this.volumeName = volumeName;
		}

		protected DatabaseNotHealthyOnVolume(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseName", this.databaseName);
			info.AddValue("volumeName", this.volumeName);
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		private readonly string databaseName;

		private readonly string volumeName;
	}
}
