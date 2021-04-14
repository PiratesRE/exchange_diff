using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PathIsAlreadyAValidMountPoint : DatabaseVolumeInfoException
	{
		public PathIsAlreadyAValidMountPoint(string path) : base(ReplayStrings.PathIsAlreadyAValidMountPoint(path))
		{
			this.path = path;
		}

		public PathIsAlreadyAValidMountPoint(string path, Exception innerException) : base(ReplayStrings.PathIsAlreadyAValidMountPoint(path), innerException)
		{
			this.path = path;
		}

		protected PathIsAlreadyAValidMountPoint(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.path = (string)info.GetValue("path", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("path", this.path);
		}

		public string Path
		{
			get
			{
				return this.path;
			}
		}

		private readonly string path;
	}
}
