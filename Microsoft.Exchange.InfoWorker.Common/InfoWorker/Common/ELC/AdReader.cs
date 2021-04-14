using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class AdReader
	{
		internal static ADUser GetADUser(MailboxSession session, bool readOnly, out IRecipientSession recipSession)
		{
			recipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(readOnly, ConsistencyMode.FullyConsistent, session.MailboxOwner.MailboxInfo.OrganizationId.ToADSessionSettings(), 43, "GetADUser", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\ELC\\AdReader.cs");
			ADRecipient adrecipient = null;
			string text = session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
			try
			{
				if (session.MailboxOwner.MailboxInfo.MailboxGuid != Guid.Empty)
				{
					adrecipient = recipSession.FindByExchangeGuidIncludingArchive(session.MailboxOwner.MailboxInfo.MailboxGuid);
				}
				else if (SmtpAddress.IsValidSmtpAddress(text))
				{
					SmtpProxyAddress proxyAddress = new SmtpProxyAddress(text, true);
					adrecipient = recipSession.FindByProxyAddress(proxyAddress);
				}
				else
				{
					AdReader.Tracer.TraceDebug<string>(0L, "Mailbox '{0}' does not have a valid smtp address.", text);
				}
			}
			catch (DataValidationException)
			{
				AdReader.Tracer.TraceError<string>(0L, "AD object for '{0}' has a data validation issue.", text);
				return null;
			}
			if (adrecipient == null)
			{
				AdReader.Tracer.TraceDebug<string>(0L, "Mailbox '{0}' does not have a user associated with it.", text);
				return null;
			}
			ADUser aduser = adrecipient as ADUser;
			if (aduser == null)
			{
				AdReader.Tracer.TraceDebug<string>(0L, "'{0}': Is not an ADUser.", text);
				return null;
			}
			return aduser;
		}

		protected static readonly Trace Tracer = ExTraceGlobals.ELCTracer;
	}
}
