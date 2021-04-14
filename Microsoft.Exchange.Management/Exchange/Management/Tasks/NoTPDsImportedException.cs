using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoTPDsImportedException : LocalizedException
	{
		public NoTPDsImportedException() : base(Strings.NoTPDsImported)
		{
		}

		public NoTPDsImportedException(Exception innerException) : base(Strings.NoTPDsImported, innerException)
		{
		}

		protected NoTPDsImportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
