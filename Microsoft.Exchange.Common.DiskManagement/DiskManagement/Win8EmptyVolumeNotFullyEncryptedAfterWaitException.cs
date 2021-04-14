using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class Win8EmptyVolumeNotFullyEncryptedAfterWaitException : BitlockerUtilException
	{
		public Win8EmptyVolumeNotFullyEncryptedAfterWaitException(string volume, int milliseconds, string bitlockerState) : base(DiskManagementStrings.Win8EmptyVolumeNotFullyEncryptedAfterWaitError(volume, milliseconds, bitlockerState))
		{
			this.volume = volume;
			this.milliseconds = milliseconds;
			this.bitlockerState = bitlockerState;
		}

		public Win8EmptyVolumeNotFullyEncryptedAfterWaitException(string volume, int milliseconds, string bitlockerState, Exception innerException) : base(DiskManagementStrings.Win8EmptyVolumeNotFullyEncryptedAfterWaitError(volume, milliseconds, bitlockerState), innerException)
		{
			this.volume = volume;
			this.milliseconds = milliseconds;
			this.bitlockerState = bitlockerState;
		}

		protected Win8EmptyVolumeNotFullyEncryptedAfterWaitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volume = (string)info.GetValue("volume", typeof(string));
			this.milliseconds = (int)info.GetValue("milliseconds", typeof(int));
			this.bitlockerState = (string)info.GetValue("bitlockerState", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volume", this.volume);
			info.AddValue("milliseconds", this.milliseconds);
			info.AddValue("bitlockerState", this.bitlockerState);
		}

		public string Volume
		{
			get
			{
				return this.volume;
			}
		}

		public int Milliseconds
		{
			get
			{
				return this.milliseconds;
			}
		}

		public string BitlockerState
		{
			get
			{
				return this.bitlockerState;
			}
		}

		private readonly string volume;

		private readonly int milliseconds;

		private readonly string bitlockerState;
	}
}
