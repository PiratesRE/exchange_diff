using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UsedOnlyEncryptionOnNonWin8ServerException : BitlockerUtilException
	{
		public UsedOnlyEncryptionOnNonWin8ServerException(string volumeID) : base(DiskManagementStrings.UsedOnlyEncryptionOnNonWin8ServerError(volumeID))
		{
			this.volumeID = volumeID;
		}

		public UsedOnlyEncryptionOnNonWin8ServerException(string volumeID, Exception innerException) : base(DiskManagementStrings.UsedOnlyEncryptionOnNonWin8ServerError(volumeID), innerException)
		{
			this.volumeID = volumeID;
		}

		protected UsedOnlyEncryptionOnNonWin8ServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeID = (string)info.GetValue("volumeID", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeID", this.volumeID);
		}

		public string VolumeID
		{
			get
			{
				return this.volumeID;
			}
		}

		private readonly string volumeID;
	}
}
