using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileCheckRequiredGenerationCorruptException : FileCheckException
	{
		public FileCheckRequiredGenerationCorruptException(string file, long min, long max) : base(ReplayStrings.FileCheckRequiredGenerationCorrupt(file, min, max))
		{
			this.file = file;
			this.min = min;
			this.max = max;
		}

		public FileCheckRequiredGenerationCorruptException(string file, long min, long max, Exception innerException) : base(ReplayStrings.FileCheckRequiredGenerationCorrupt(file, min, max), innerException)
		{
			this.file = file;
			this.min = min;
			this.max = max;
		}

		protected FileCheckRequiredGenerationCorruptException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.file = (string)info.GetValue("file", typeof(string));
			this.min = (long)info.GetValue("min", typeof(long));
			this.max = (long)info.GetValue("max", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("file", this.file);
			info.AddValue("min", this.min);
			info.AddValue("max", this.max);
		}

		public string File
		{
			get
			{
				return this.file;
			}
		}

		public long Min
		{
			get
			{
				return this.min;
			}
		}

		public long Max
		{
			get
			{
				return this.max;
			}
		}

		private readonly string file;

		private readonly long min;

		private readonly long max;
	}
}
