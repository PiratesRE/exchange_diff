using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeVolumeInfoMultipleDbMountPointsException : DatabaseVolumeInfoException
	{
		public ExchangeVolumeInfoMultipleDbMountPointsException(string volumeName, string dbVolRootPath, string dbMountPoints, int maxDbs) : base(ReplayStrings.ExchangeVolumeInfoMultipleDbMountPointsException(volumeName, dbVolRootPath, dbMountPoints, maxDbs))
		{
			this.volumeName = volumeName;
			this.dbVolRootPath = dbVolRootPath;
			this.dbMountPoints = dbMountPoints;
			this.maxDbs = maxDbs;
		}

		public ExchangeVolumeInfoMultipleDbMountPointsException(string volumeName, string dbVolRootPath, string dbMountPoints, int maxDbs, Exception innerException) : base(ReplayStrings.ExchangeVolumeInfoMultipleDbMountPointsException(volumeName, dbVolRootPath, dbMountPoints, maxDbs), innerException)
		{
			this.volumeName = volumeName;
			this.dbVolRootPath = dbVolRootPath;
			this.dbMountPoints = dbMountPoints;
			this.maxDbs = maxDbs;
		}

		protected ExchangeVolumeInfoMultipleDbMountPointsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.dbVolRootPath = (string)info.GetValue("dbVolRootPath", typeof(string));
			this.dbMountPoints = (string)info.GetValue("dbMountPoints", typeof(string));
			this.maxDbs = (int)info.GetValue("maxDbs", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("dbVolRootPath", this.dbVolRootPath);
			info.AddValue("dbMountPoints", this.dbMountPoints);
			info.AddValue("maxDbs", this.maxDbs);
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		public string DbVolRootPath
		{
			get
			{
				return this.dbVolRootPath;
			}
		}

		public string DbMountPoints
		{
			get
			{
				return this.dbMountPoints;
			}
		}

		public int MaxDbs
		{
			get
			{
				return this.maxDbs;
			}
		}

		private readonly string volumeName;

		private readonly string dbVolRootPath;

		private readonly string dbMountPoints;

		private readonly int maxDbs;
	}
}
