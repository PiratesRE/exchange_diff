using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeException : BitlockerUtilException
	{
		public UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeException(string volume) : base(DiskManagementStrings.UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeError(volume))
		{
			this.volume = volume;
		}

		public UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeException(string volume, Exception innerException) : base(DiskManagementStrings.UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeError(volume), innerException)
		{
			this.volume = volume;
		}

		protected UsedOnlySpaceEncryptionAttemptOnANonEmptyVolumeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volume = (string)info.GetValue("volume", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volume", this.volume);
		}

		public string Volume
		{
			get
			{
				return this.volume;
			}
		}

		private readonly string volume;
	}
}
