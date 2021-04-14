using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DelayCacheFullException : ADTransientException
	{
		public DelayCacheFullException() : base(DirectoryStrings.DelayCacheFull)
		{
		}

		public DelayCacheFullException(Exception innerException) : base(DirectoryStrings.DelayCacheFull, innerException)
		{
		}

		protected DelayCacheFullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
