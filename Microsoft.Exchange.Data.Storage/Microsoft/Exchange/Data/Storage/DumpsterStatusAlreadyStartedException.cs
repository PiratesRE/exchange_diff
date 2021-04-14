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
	internal class DumpsterStatusAlreadyStartedException : LocalizedException
	{
		public DumpsterStatusAlreadyStartedException() : base(ServerStrings.DumpsterStatusAlreadyStartedException)
		{
		}

		public DumpsterStatusAlreadyStartedException(Exception innerException) : base(ServerStrings.DumpsterStatusAlreadyStartedException, innerException)
		{
		}

		protected DumpsterStatusAlreadyStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
