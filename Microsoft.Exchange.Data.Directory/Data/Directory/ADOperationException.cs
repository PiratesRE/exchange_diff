using System;
using System.DirectoryServices.Protocols;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADOperationException : DataSourceOperationException
	{
		public ADOperationException(LocalizedString message) : base(message)
		{
		}

		public ADOperationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ADOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		internal PooledLdapConnection Connection
		{
			set
			{
				this.connection = value;
			}
		}

		internal DirectoryRequest ADRequest
		{
			set
			{
				this.request = value;
				SearchRequest searchRequest = value as SearchRequest;
				if (searchRequest != null && searchRequest.Filter != null)
				{
					ExWatson.AddExtraData(searchRequest.Filter.ToString());
				}
			}
		}

		private PooledLdapConnection connection;

		private DirectoryRequest request;
	}
}
