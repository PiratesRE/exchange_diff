using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RecoverableItemsAccessDeniedException : StoragePermanentException
	{
		public RecoverableItemsAccessDeniedException(string folder) : base(ServerStrings.RecoverableItemsAccessDeniedException(folder))
		{
			this.folder = folder;
		}

		public RecoverableItemsAccessDeniedException(string folder, Exception innerException) : base(ServerStrings.RecoverableItemsAccessDeniedException(folder), innerException)
		{
			this.folder = folder;
		}

		protected RecoverableItemsAccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.folder = (string)info.GetValue("folder", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("folder", this.folder);
		}

		public string Folder
		{
			get
			{
				return this.folder;
			}
		}

		private readonly string folder;
	}
}
