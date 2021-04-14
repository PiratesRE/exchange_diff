using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DefaultTaskNodeCreationException : StorageTransientException
	{
		public DefaultTaskNodeCreationException(string folderType) : base(ServerStrings.idUnableToAddDefaultTaskFolderToDefaultTaskGroup(folderType))
		{
			this.folderType = folderType;
		}

		public DefaultTaskNodeCreationException(string folderType, Exception innerException) : base(ServerStrings.idUnableToAddDefaultTaskFolderToDefaultTaskGroup(folderType), innerException)
		{
			this.folderType = folderType;
		}

		protected DefaultTaskNodeCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folderType = (string)info.GetValue("folderType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folderType", this.folderType);
		}

		public string FolderType
		{
			get
			{
				return this.folderType;
			}
		}

		private readonly string folderType;
	}
}
