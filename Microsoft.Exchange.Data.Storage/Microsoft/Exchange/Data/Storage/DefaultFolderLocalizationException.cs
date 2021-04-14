using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DefaultFolderLocalizationException : CorruptDataException
	{
		public DefaultFolderLocalizationException() : base(ServerStrings.idDefaultFoldersNotLocalizedException)
		{
		}

		public DefaultFolderLocalizationException(Exception innerException) : base(ServerStrings.idDefaultFoldersNotLocalizedException, innerException)
		{
		}

		protected DefaultFolderLocalizationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
