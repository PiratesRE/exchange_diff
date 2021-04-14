using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidVersionException : LocalizedException
	{
		public InvalidVersionException(string version, string format) : base(Strings.InvalidVersion(version, format))
		{
			this.version = version;
			this.format = format;
		}

		public InvalidVersionException(string version, string format, Exception innerException) : base(Strings.InvalidVersion(version, format), innerException)
		{
			this.version = version;
			this.format = format;
		}

		protected InvalidVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.version = (string)info.GetValue("version", typeof(string));
			this.format = (string)info.GetValue("format", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("version", this.version);
			info.AddValue("format", this.format);
		}

		public string Version
		{
			get
			{
				return this.version;
			}
		}

		public string Format
		{
			get
			{
				return this.format;
			}
		}

		private readonly string version;

		private readonly string format;
	}
}
