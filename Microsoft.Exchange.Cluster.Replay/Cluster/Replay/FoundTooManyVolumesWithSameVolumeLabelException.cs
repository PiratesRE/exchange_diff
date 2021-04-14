using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FoundTooManyVolumesWithSameVolumeLabelException : DatabaseVolumeInfoException
	{
		public FoundTooManyVolumesWithSameVolumeLabelException(string volumeNames, string volumeLabel) : base(ReplayStrings.FoundTooManyVolumesWithSameVolumeLabelException(volumeNames, volumeLabel))
		{
			this.volumeNames = volumeNames;
			this.volumeLabel = volumeLabel;
		}

		public FoundTooManyVolumesWithSameVolumeLabelException(string volumeNames, string volumeLabel, Exception innerException) : base(ReplayStrings.FoundTooManyVolumesWithSameVolumeLabelException(volumeNames, volumeLabel), innerException)
		{
			this.volumeNames = volumeNames;
			this.volumeLabel = volumeLabel;
		}

		protected FoundTooManyVolumesWithSameVolumeLabelException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeNames = (string)info.GetValue("volumeNames", typeof(string));
			this.volumeLabel = (string)info.GetValue("volumeLabel", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeNames", this.volumeNames);
			info.AddValue("volumeLabel", this.volumeLabel);
		}

		public string VolumeNames
		{
			get
			{
				return this.volumeNames;
			}
		}

		public string VolumeLabel
		{
			get
			{
				return this.volumeLabel;
			}
		}

		private readonly string volumeNames;

		private readonly string volumeLabel;
	}
}
