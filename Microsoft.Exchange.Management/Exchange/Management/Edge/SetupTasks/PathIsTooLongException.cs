using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PathIsTooLongException : LocalizedException
	{
		public PathIsTooLongException(string path) : base(Strings.PathTooLong(path))
		{
			this.path = path;
		}

		public PathIsTooLongException(string path, Exception innerException) : base(Strings.PathTooLong(path), innerException)
		{
			this.path = path;
		}

		protected PathIsTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
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
