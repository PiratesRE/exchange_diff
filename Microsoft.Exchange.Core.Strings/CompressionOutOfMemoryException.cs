using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CompressionOutOfMemoryException : LocalizedException
	{
		public CompressionOutOfMemoryException() : base(CoreStrings.CompressionOutOfMemory)
		{
		}

		public CompressionOutOfMemoryException(Exception innerException) : base(CoreStrings.CompressionOutOfMemory, innerException)
		{
		}

		protected CompressionOutOfMemoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
