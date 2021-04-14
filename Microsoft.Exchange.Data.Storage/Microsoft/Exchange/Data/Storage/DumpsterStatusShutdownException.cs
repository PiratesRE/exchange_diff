using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DumpsterStatusShutdownException : LocalizedException
	{
		public DumpsterStatusShutdownException() : base(ServerStrings.DumpsterStatusShutdownException)
		{
		}

		public DumpsterStatusShutdownException(Exception innerException) : base(ServerStrings.DumpsterStatusShutdownException, innerException)
		{
		}

		protected DumpsterStatusShutdownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
