using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NestedFoldersNotAllowedException : LocalizedException
	{
		public NestedFoldersNotAllowedException() : base(Strings.NestedFoldersNotAllowedException)
		{
		}

		public NestedFoldersNotAllowedException(Exception innerException) : base(Strings.NestedFoldersNotAllowedException, innerException)
		{
		}

		protected NestedFoldersNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
