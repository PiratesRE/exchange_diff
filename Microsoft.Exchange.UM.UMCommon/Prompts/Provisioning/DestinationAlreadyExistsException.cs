using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DestinationAlreadyExistsException : PublishingException
	{
		public DestinationAlreadyExistsException(string path) : base(Strings.DestinationAlreadyExists(path))
		{
			this.path = path;
		}

		public DestinationAlreadyExistsException(string path, Exception innerException) : base(Strings.DestinationAlreadyExists(path), innerException)
		{
			this.path = path;
		}

		protected DestinationAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
