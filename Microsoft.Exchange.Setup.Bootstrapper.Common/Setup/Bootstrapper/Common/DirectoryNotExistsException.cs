using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DirectoryNotExistsException : LocalizedException
	{
		public DirectoryNotExistsException(string dirName) : base(Strings.DirectoryNotFound(dirName))
		{
			this.dirName = dirName;
		}

		public DirectoryNotExistsException(string dirName, Exception innerException) : base(Strings.DirectoryNotFound(dirName), innerException)
		{
			this.dirName = dirName;
		}

		protected DirectoryNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dirName = (string)info.GetValue("dirName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dirName", this.dirName);
		}

		public string DirName
		{
			get
			{
				return this.dirName;
			}
		}

		private readonly string dirName;
	}
}
