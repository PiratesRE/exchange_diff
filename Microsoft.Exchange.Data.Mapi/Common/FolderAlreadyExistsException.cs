using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FolderAlreadyExistsException : MapiObjectAlreadyExistsException
	{
		public FolderAlreadyExistsException(string folder) : base(Strings.FolderAlreadyExistsExceptionError(folder))
		{
			this.folder = folder;
		}

		public FolderAlreadyExistsException(string folder, Exception innerException) : base(Strings.FolderAlreadyExistsExceptionError(folder), innerException)
		{
			this.folder = folder;
		}

		protected FolderAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
