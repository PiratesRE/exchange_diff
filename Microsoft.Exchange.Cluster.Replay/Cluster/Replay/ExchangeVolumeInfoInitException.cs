using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeVolumeInfoInitException : DatabaseVolumeInfoException
	{
		public ExchangeVolumeInfoInitException(string volumeName, string errMsg) : base(ReplayStrings.ExchangeVolumeInfoInitException(volumeName, errMsg))
		{
			this.volumeName = volumeName;
			this.errMsg = errMsg;
		}

		public ExchangeVolumeInfoInitException(string volumeName, string errMsg, Exception innerException) : base(ReplayStrings.ExchangeVolumeInfoInitException(volumeName, errMsg), innerException)
		{
			this.volumeName = volumeName;
			this.errMsg = errMsg;
		}

		protected ExchangeVolumeInfoInitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("errMsg", this.errMsg);
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string volumeName;

		private readonly string errMsg;
	}
}
