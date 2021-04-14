using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class TooManyFoldersException : MigrationPermanentException
	{
		public TooManyFoldersException() : base(Strings.TooManyFoldersStatus)
		{
		}

		public TooManyFoldersException(Exception innerException) : base(Strings.TooManyFoldersStatus, innerException)
		{
		}

		protected TooManyFoldersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
