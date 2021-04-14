using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckEDBMissingException : FileCheckException
	{
		public FileCheckEDBMissingException(string edbFileName) : base(ReplayStrings.FileCheckEDBMissing(edbFileName))
		{
			this.edbFileName = edbFileName;
		}

		public FileCheckEDBMissingException(string edbFileName, Exception innerException) : base(ReplayStrings.FileCheckEDBMissing(edbFileName), innerException)
		{
			this.edbFileName = edbFileName;
		}

		protected FileCheckEDBMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.edbFileName = (string)info.GetValue("edbFileName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("edbFileName", this.edbFileName);
		}

		public string EdbFileName
		{
			get
			{
				return this.edbFileName;
			}
		}

		private readonly string edbFileName;
	}
}
