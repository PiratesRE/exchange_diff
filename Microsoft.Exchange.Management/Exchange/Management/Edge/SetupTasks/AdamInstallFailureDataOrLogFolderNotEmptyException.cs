using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AdamInstallFailureDataOrLogFolderNotEmptyException : LocalizedException
	{
		public AdamInstallFailureDataOrLogFolderNotEmptyException() : base(Strings.AdamInstallFailureDataOrLogFolderNotEmpty)
		{
		}

		public AdamInstallFailureDataOrLogFolderNotEmptyException(Exception innerException) : base(Strings.AdamInstallFailureDataOrLogFolderNotEmpty, innerException)
		{
		}

		protected AdamInstallFailureDataOrLogFolderNotEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
