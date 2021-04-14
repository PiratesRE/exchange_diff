using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogInspectorFailedGeneralException : LogInspectorFailedException
	{
		public LogInspectorFailedGeneralException(string fileName, string specificError) : base(ReplayStrings.LogInspectorFailedGeneral(fileName, specificError))
		{
			this.fileName = fileName;
			this.specificError = specificError;
		}

		public LogInspectorFailedGeneralException(string fileName, string specificError, Exception innerException) : base(ReplayStrings.LogInspectorFailedGeneral(fileName, specificError), innerException)
		{
			this.fileName = fileName;
			this.specificError = specificError;
		}

		protected LogInspectorFailedGeneralException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fileName = (string)info.GetValue("fileName", typeof(string));
			this.specificError = (string)info.GetValue("specificError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fileName", this.fileName);
			info.AddValue("specificError", this.specificError);
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public string SpecificError
		{
			get
			{
				return this.specificError;
			}
		}

		private readonly string fileName;

		private readonly string specificError;
	}
}
