using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogInspectorCouldNotMoveLogFileException : LogInspectorFailedException
	{
		public LogInspectorCouldNotMoveLogFileException(string oldpath, string newpath, string error) : base(ReplayStrings.LogInspectorCouldNotMoveLogFileException(oldpath, newpath, error))
		{
			this.oldpath = oldpath;
			this.newpath = newpath;
			this.error = error;
		}

		public LogInspectorCouldNotMoveLogFileException(string oldpath, string newpath, string error, Exception innerException) : base(ReplayStrings.LogInspectorCouldNotMoveLogFileException(oldpath, newpath, error), innerException)
		{
			this.oldpath = oldpath;
			this.newpath = newpath;
			this.error = error;
		}

		protected LogInspectorCouldNotMoveLogFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.oldpath = (string)info.GetValue("oldpath", typeof(string));
			this.newpath = (string)info.GetValue("newpath", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("oldpath", this.oldpath);
			info.AddValue("newpath", this.newpath);
			info.AddValue("error", this.error);
		}

		public string Oldpath
		{
			get
			{
				return this.oldpath;
			}
		}

		public string Newpath
		{
			get
			{
				return this.newpath;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string oldpath;

		private readonly string newpath;

		private readonly string error;
	}
}
