using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceFileNotFoundException : PublishingException
	{
		public SourceFileNotFoundException(string path) : base(Strings.SourceFileNotFound(path))
		{
			this.path = path;
		}

		public SourceFileNotFoundException(string path, Exception innerException) : base(Strings.SourceFileNotFound(path), innerException)
		{
			this.path = path;
		}

		protected SourceFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
