using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OutputDirectoryNotExist : LocalizedException
	{
		public OutputDirectoryNotExist(string directory) : base(Strings.OutputDirectoryNotExist(directory))
		{
			this.directory = directory;
		}

		public OutputDirectoryNotExist(string directory, Exception innerException) : base(Strings.OutputDirectoryNotExist(directory), innerException)
		{
			this.directory = directory;
		}

		protected OutputDirectoryNotExist(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.directory = (string)info.GetValue("directory", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("directory", this.directory);
		}

		public string Directory
		{
			get
			{
				return this.directory;
			}
		}

		private readonly string directory;
	}
}
