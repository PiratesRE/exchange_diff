using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class Win7EmptyVolumeNotEncryptingAfterStartingEncryptionException : BitlockerUtilException
	{
		public Win7EmptyVolumeNotEncryptingAfterStartingEncryptionException(string volume, string bitlockerState) : base(DiskManagementStrings.Win7EmptyVolumeNotEncryptingAfterStartingEncryptionError(volume, bitlockerState))
		{
			this.volume = volume;
			this.bitlockerState = bitlockerState;
		}

		public Win7EmptyVolumeNotEncryptingAfterStartingEncryptionException(string volume, string bitlockerState, Exception innerException) : base(DiskManagementStrings.Win7EmptyVolumeNotEncryptingAfterStartingEncryptionError(volume, bitlockerState), innerException)
		{
			this.volume = volume;
			this.bitlockerState = bitlockerState;
		}

		protected Win7EmptyVolumeNotEncryptingAfterStartingEncryptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volume = (string)info.GetValue("volume", typeof(string));
			this.bitlockerState = (string)info.GetValue("bitlockerState", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volume", this.volume);
			info.AddValue("bitlockerState", this.bitlockerState);
		}

		public string Volume
		{
			get
			{
				return this.volume;
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

		private readonly string bitlockerState;
	}
}
