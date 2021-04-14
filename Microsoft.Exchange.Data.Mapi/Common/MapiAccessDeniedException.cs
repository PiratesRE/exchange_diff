using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MapiAccessDeniedException : MapiOperationException
	{
		public MapiAccessDeniedException(LocalizedString message) : base(message)
		{
		}

		public MapiAccessDeniedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MapiAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
