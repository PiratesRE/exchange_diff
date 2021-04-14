using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindSpareVolumeException : DatabaseVolumeInfoException
	{
		public CouldNotFindSpareVolumeException(string databases) : base(ReplayStrings.CouldNotFindSpareVolumeException(databases))
		{
			this.databases = databases;
		}

		public CouldNotFindSpareVolumeException(string databases, Exception innerException) : base(ReplayStrings.CouldNotFindSpareVolumeException(databases), innerException)
		{
			this.databases = databases;
		}

		protected CouldNotFindSpareVolumeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databases = (string)info.GetValue("databases", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databases", this.databases);
		}

		public string Databases
		{
			get
			{
				return this.databases;
			}
		}

		private readonly string databases;
	}
}
