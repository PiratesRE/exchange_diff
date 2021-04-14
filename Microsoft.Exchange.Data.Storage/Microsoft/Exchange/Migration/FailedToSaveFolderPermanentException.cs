using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FailedToSaveFolderPermanentException : MigrationPermanentException
	{
		public FailedToSaveFolderPermanentException(LocalizedString message) : base(message)
		{
		}

		public FailedToSaveFolderPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FailedToSaveFolderPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
