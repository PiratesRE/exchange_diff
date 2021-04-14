using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADTopologyEndpointNotFoundException : ADTransientException
	{
		public ADTopologyEndpointNotFoundException(string url) : base(DirectoryStrings.ADTopologyEndpointNotFoundException(url))
		{
			this.url = url;
		}

		public ADTopologyEndpointNotFoundException(string url, Exception innerException) : base(DirectoryStrings.ADTopologyEndpointNotFoundException(url), innerException)
		{
			this.url = url;
		}

		protected ADTopologyEndpointNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.url = (string)info.GetValue("url", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("url", this.url);
		}

		public string Url
		{
			get
			{
				return this.url;
			}
		}

		private readonly string url;
	}
}
