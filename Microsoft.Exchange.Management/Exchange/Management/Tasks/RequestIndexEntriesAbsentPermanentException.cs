using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequestIndexEntriesAbsentPermanentException : MailboxReplicationPermanentException
	{
		public RequestIndexEntriesAbsentPermanentException(string name) : base(Strings.ErrorCouldNotFindIndexEntriesForRequest(name))
		{
			this.name = name;
		}

		public RequestIndexEntriesAbsentPermanentException(string name, Exception innerException) : base(Strings.ErrorCouldNotFindIndexEntriesForRequest(name), innerException)
		{
			this.name = name;
		}

		protected RequestIndexEntriesAbsentPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
