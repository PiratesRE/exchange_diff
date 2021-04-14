using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoveVirtualDirectoryGetApplicationPoolUnknownError : LocalizedException
	{
		public RemoveVirtualDirectoryGetApplicationPoolUnknownError(string path) : base(Strings.RemoveVirtualDirectoryGetApplicationPoolUnknownError(path))
		{
			this.path = path;
		}

		public RemoveVirtualDirectoryGetApplicationPoolUnknownError(string path, Exception innerException) : base(Strings.RemoveVirtualDirectoryGetApplicationPoolUnknownError(path), innerException)
		{
			this.path = path;
		}

		protected RemoveVirtualDirectoryGetApplicationPoolUnknownError(SerializationInfo info, StreamingContext context) : base(info, context)
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
