using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NonMatchingLegacyDNPermanentException : MailboxReplicationPermanentException
	{
		public NonMatchingLegacyDNPermanentException(string sourceMailboxLegDN, string targetMailbox, string parameterName) : base(Strings.ErrorNonMatchingLegacyDNs(sourceMailboxLegDN, targetMailbox, parameterName))
		{
			this.sourceMailboxLegDN = sourceMailboxLegDN;
			this.targetMailbox = targetMailbox;
			this.parameterName = parameterName;
		}

		public NonMatchingLegacyDNPermanentException(string sourceMailboxLegDN, string targetMailbox, string parameterName, Exception innerException) : base(Strings.ErrorNonMatchingLegacyDNs(sourceMailboxLegDN, targetMailbox, parameterName), innerException)
		{
			this.sourceMailboxLegDN = sourceMailboxLegDN;
			this.targetMailbox = targetMailbox;
			this.parameterName = parameterName;
		}

		protected NonMatchingLegacyDNPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sourceMailboxLegDN = (string)info.GetValue("sourceMailboxLegDN", typeof(string));
			this.targetMailbox = (string)info.GetValue("targetMailbox", typeof(string));
			this.parameterName = (string)info.GetValue("parameterName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sourceMailboxLegDN", this.sourceMailboxLegDN);
			info.AddValue("targetMailbox", this.targetMailbox);
			info.AddValue("parameterName", this.parameterName);
		}

		public string SourceMailboxLegDN
		{
			get
			{
				return this.sourceMailboxLegDN;
			}
		}

		public string TargetMailbox
		{
			get
			{
				return this.targetMailbox;
			}
		}

		public string ParameterName
		{
			get
			{
				return this.parameterName;
			}
		}

		private readonly string sourceMailboxLegDN;

		private readonly string targetMailbox;

		private readonly string parameterName;
	}
}
