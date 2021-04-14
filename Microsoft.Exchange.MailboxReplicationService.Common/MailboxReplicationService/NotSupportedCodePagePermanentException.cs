using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotSupportedCodePagePermanentException : MailboxReplicationPermanentException
	{
		public NotSupportedCodePagePermanentException(int codePage, string server) : base(MrsStrings.NotSupportedCodePageError(codePage, server))
		{
			this.codePage = codePage;
			this.server = server;
		}

		public NotSupportedCodePagePermanentException(int codePage, string server, Exception innerException) : base(MrsStrings.NotSupportedCodePageError(codePage, server), innerException)
		{
			this.codePage = codePage;
			this.server = server;
		}

		protected NotSupportedCodePagePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.codePage = (int)info.GetValue("codePage", typeof(int));
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("codePage", this.codePage);
			info.AddValue("server", this.server);
		}

		public int CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly int codePage;

		private readonly string server;
	}
}
