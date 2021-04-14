using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OldRmsOnlineImportAfterRmsOnlineForwardSync : LocalizedException
	{
		public OldRmsOnlineImportAfterRmsOnlineForwardSync() : base(Strings.OldRmsOnlineImportAfterRmsOnlineForwardSync)
		{
		}

		public OldRmsOnlineImportAfterRmsOnlineForwardSync(Exception innerException) : base(Strings.OldRmsOnlineImportAfterRmsOnlineForwardSync, innerException)
		{
		}

		protected OldRmsOnlineImportAfterRmsOnlineForwardSync(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
