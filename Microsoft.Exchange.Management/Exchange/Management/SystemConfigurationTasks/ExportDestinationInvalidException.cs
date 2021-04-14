using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExportDestinationInvalidException : LocalizedException
	{
		public ExportDestinationInvalidException(string path) : base(Strings.ExportDestinationInvalid(path))
		{
			this.path = path;
		}

		public ExportDestinationInvalidException(string path, Exception innerException) : base(Strings.ExportDestinationInvalid(path), innerException)
		{
			this.path = path;
		}

		protected ExportDestinationInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.path = (string)info.GetValue("path", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("path", this.path);
		}

		public string Path
		{
			get
			{
				return this.path;
			}
		}

		private readonly string path;
	}
}
