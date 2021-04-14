using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NonUNCFilePathPermanentException : MailboxReplicationPermanentException
	{
		public NonUNCFilePathPermanentException(string pathName) : base(Strings.ErrorFilePathMustBeUNC(pathName))
		{
			this.pathName = pathName;
		}

		public NonUNCFilePathPermanentException(string pathName, Exception innerException) : base(Strings.ErrorFilePathMustBeUNC(pathName), innerException)
		{
			this.pathName = pathName;
		}

		protected NonUNCFilePathPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.pathName = (string)info.GetValue("pathName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("pathName", this.pathName);
		}

		public string PathName
		{
			get
			{
				return this.pathName;
			}
		}

		private readonly string pathName;
	}
}
