using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidApplicationUriException : FederationException
	{
		public InvalidApplicationUriException(string uri) : base(Strings.ErrorInvalidApplicationUri(uri))
		{
			this.uri = uri;
		}

		public InvalidApplicationUriException(string uri, Exception innerException) : base(Strings.ErrorInvalidApplicationUri(uri), innerException)
		{
			this.uri = uri;
		}

		protected InvalidApplicationUriException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.uri = (string)info.GetValue("uri", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("uri", this.uri);
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
		}

		private readonly string uri;
	}
}
