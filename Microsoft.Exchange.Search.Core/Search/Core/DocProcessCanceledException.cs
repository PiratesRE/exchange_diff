using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Search.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DocProcessCanceledException : OperationFailedException
	{
		public DocProcessCanceledException() : base(Strings.DocProcessCanceled)
		{
		}

		public DocProcessCanceledException(Exception innerException) : base(Strings.DocProcessCanceled, innerException)
		{
		}

		protected DocProcessCanceledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
