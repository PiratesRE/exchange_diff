using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PublicFolderNotFoundException : MapiObjectNotFoundException
	{
		public PublicFolderNotFoundException(string folder) : base(Strings.PublicFolderNotFoundExceptionError(folder))
		{
			this.folder = folder;
		}

		public PublicFolderNotFoundException(string folder, Exception innerException) : base(Strings.PublicFolderNotFoundExceptionError(folder), innerException)
		{
			this.folder = folder;
		}

		protected PublicFolderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
