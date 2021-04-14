using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToFindUMReportDataException : LocalizedException
	{
		public UnableToFindUMReportDataException(string mailboxOwner) : base(Strings.UnableToFindUMReportData(mailboxOwner))
		{
			this.mailboxOwner = mailboxOwner;
		}

		public UnableToFindUMReportDataException(string mailboxOwner, Exception innerException) : base(Strings.UnableToFindUMReportData(mailboxOwner), innerException)
		{
			this.mailboxOwner = mailboxOwner;
		}

		protected UnableToFindUMReportDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxOwner = (string)info.GetValue("mailboxOwner", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxOwner", this.mailboxOwner);
		}

		public string MailboxOwner
		{
			get
			{
				return this.mailboxOwner;
			}
		}

		private readonly string mailboxOwner;
	}
}
