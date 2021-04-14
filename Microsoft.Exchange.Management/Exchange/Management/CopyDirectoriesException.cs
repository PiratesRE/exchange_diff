using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CopyDirectoriesException : DataSourceOperationException
	{
		public CopyDirectoriesException(string fromFolder, string toFolder, string error) : base(Strings.CopyDirectoriesException(fromFolder, toFolder, error))
		{
			this.fromFolder = fromFolder;
			this.toFolder = toFolder;
			this.error = error;
		}

		public CopyDirectoriesException(string fromFolder, string toFolder, string error, Exception innerException) : base(Strings.CopyDirectoriesException(fromFolder, toFolder, error), innerException)
		{
			this.fromFolder = fromFolder;
			this.toFolder = toFolder;
			this.error = error;
		}

		protected CopyDirectoriesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fromFolder = (string)info.GetValue("fromFolder", typeof(string));
			this.toFolder = (string)info.GetValue("toFolder", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fromFolder", this.fromFolder);
			info.AddValue("toFolder", this.toFolder);
			info.AddValue("error", this.error);
		}

		public string FromFolder
		{
			get
			{
				return this.fromFolder;
			}
		}

		public string ToFolder
		{
			get
			{
				return this.toFolder;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string fromFolder;

		private readonly string toFolder;

		private readonly string error;
	}
}
