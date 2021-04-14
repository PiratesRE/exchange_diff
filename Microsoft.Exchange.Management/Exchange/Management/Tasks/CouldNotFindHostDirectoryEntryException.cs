using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindHostDirectoryEntryException : LocalizedException
	{
		public CouldNotFindHostDirectoryEntryException(string host) : base(Strings.CouldNotFindHostDirectoryEntryException(host))
		{
			this.host = host;
		}

		public CouldNotFindHostDirectoryEntryException(string host, Exception innerException) : base(Strings.CouldNotFindHostDirectoryEntryException(host), innerException)
		{
			this.host = host;
		}

		protected CouldNotFindHostDirectoryEntryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.host = (string)info.GetValue("host", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("host", this.host);
		}

		public string Host
		{
			get
			{
				return this.host;
			}
		}

		private readonly string host;
	}
}
