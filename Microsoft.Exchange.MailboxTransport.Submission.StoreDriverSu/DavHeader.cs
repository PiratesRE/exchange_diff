using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverSubmission;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Protocols.Smtp;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal sealed class DavHeader
	{
		public DavHeader(string headers)
		{
			this.headers = headers;
			this.rcptToList = new List<RcptToCommand>();
		}

		public static bool CopySenderAndRecipientsFromHeaders(string headers, TransportMailItem mailItem)
		{
			if (string.IsNullOrEmpty(headers))
			{
				TraceHelper.MapiStoreDriverSubmissionTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "DAV header is not present.");
				return false;
			}
			try
			{
				DavHeader davHeader = new DavHeader(headers);
				davHeader.Parse();
				davHeader.CopyTo(mailItem);
				return true;
			}
			catch (FormatException arg)
			{
				TraceHelper.MapiStoreDriverSubmissionTracer.TracePass<FormatException>(TraceHelper.MessageProbeActivityId, 0L, "Parsing Error: {0}", arg);
			}
			return false;
		}

		public void Parse()
		{
			byte[] nextLine = this.GetNextLine();
			if (nextLine == null)
			{
				throw new FormatException("No Sender");
			}
			this.mailFrom = new MailFromCommand(nextLine);
			for (byte[] nextLine2 = this.GetNextLine(); nextLine2 != null; nextLine2 = this.GetNextLine())
			{
				this.rcptToList.Add(new RcptToCommand(nextLine2));
			}
			if (this.rcptToList.Count == 0)
			{
				throw new FormatException("No Recipients");
			}
		}

		private byte[] GetNextLine()
		{
			int num = this.headers.IndexOf("\r\n", this.current, StringComparison.Ordinal);
			if (num == -1)
			{
				return null;
			}
			byte[] result = Util.AsciiStringToBytes(this.headers, this.current, num - this.current);
			this.current = num + "\r\n".Length;
			return result;
		}

		private void CopyTo(TransportMailItem mailItem)
		{
			mailItem.From = this.mailFrom.Address;
			mailItem.Auth = this.mailFrom.Auth;
			mailItem.EnvId = this.mailFrom.EnvId;
			mailItem.DsnFormat = this.mailFrom.Ret;
			mailItem.BodyType = this.mailFrom.BodyType;
			TraceHelper.MapiStoreDriverSubmissionTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "Add sender {0} from Dav Header, Auth:{1}, EnvId:{2}, DsnFormat:{3}, BodyType: {4}", new object[]
			{
				this.mailFrom.Address,
				this.mailFrom.Auth,
				this.mailFrom.EnvId,
				this.mailFrom.Ret,
				this.mailFrom.BodyType
			});
			foreach (RcptToCommand rcptToCommand in this.rcptToList)
			{
				MailRecipient mailRecipient = mailItem.Recipients.Add((string)rcptToCommand.Address);
				mailRecipient.ORcpt = rcptToCommand.ORcpt;
				mailRecipient.DsnRequested = rcptToCommand.Notify;
				TraceHelper.MapiStoreDriverSubmissionTracer.TracePass<string, string, DsnRequestedFlags>(TraceHelper.MessageProbeActivityId, 0L, "Add Recipient {0} from Dav Header, ORcpt:{1}, Notify:{2}", (string)rcptToCommand.Address, rcptToCommand.ORcpt, rcptToCommand.Notify);
			}
		}

		private const string CRLF = "\r\n";

		private static readonly Trace diag = ExTraceGlobals.MapiStoreDriverSubmissionTracer;

		private readonly string headers;

		private int current;

		private MailFromCommand mailFrom;

		private List<RcptToCommand> rcptToList;
	}
}
