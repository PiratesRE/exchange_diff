using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class CalendarParticipant : IDisposable
	{
		protected CalendarParticipant(UserObject userObject, ExDateTime validateFrom, ExDateTime validateUntil)
		{
			if (userObject == null)
			{
				throw new ArgumentNullException("userObject");
			}
			this.UserObject = userObject;
			this.ValidateFrom = validateFrom;
			this.ValidateUntil = validateUntil;
			this.ItemList = new Dictionary<GlobalObjectId, CalendarInstanceContext>();
			this.inquiredMasterGoids = new List<GlobalObjectId>();
		}

		internal UserObject UserObject { get; private set; }

		internal ExDateTime ValidateFrom { get; private set; }

		internal ExDateTime ValidateUntil { get; private set; }

		internal Dictionary<GlobalObjectId, CalendarInstanceContext> ItemList { get; private set; }

		internal static CalendarParticipant Create(UserObject userObject, ExDateTime validateFrom, ExDateTime validateUntil, SessionManager sessionManager)
		{
			ExchangePrincipal exchangePrincipal = userObject.ExchangePrincipal;
			if (exchangePrincipal == null)
			{
				return new CalendarExternalParticipant(userObject, validateFrom, validateUntil);
			}
			try
			{
				if (exchangePrincipal.MailboxInfo.Location.ServerVersion < Globals.E15Version)
				{
					Globals.ConsistencyChecksTracer.TraceDebug(0L, "Remote mailbox is on a pre-E15 server, using XSO to access it.");
					return new CalendarLocalParticipant(userObject, validateFrom, validateUntil, sessionManager);
				}
				bool flag = StringComparer.OrdinalIgnoreCase.Equals(LocalServerCache.LocalServerFqdn, exchangePrincipal.MailboxInfo.Location.ServerFqdn);
				if (flag && !Configuration.CalendarRepairForceEwsUsage)
				{
					Globals.ConsistencyChecksTracer.TraceDebug(0L, "Local server is a best fit to service the remote mailbox.");
					return new CalendarLocalParticipant(userObject, validateFrom, validateUntil, sessionManager);
				}
				if (Configuration.CalendarRepairOppositeMailboxEwsEnabled)
				{
					Uri backEndWebServicesUrl = CalendarParticipant.GetBackEndWebServicesUrl(exchangePrincipal.MailboxInfo);
					if (backEndWebServicesUrl != null)
					{
						Globals.ConsistencyChecksTracer.TraceDebug<Uri>(0L, "Using {0} to access the remote mailbox.", backEndWebServicesUrl);
						if (CalendarItemBase.IsTenantToBeFixed(sessionManager.PrimarySession.Session))
						{
							return new CalendarRemoteParticipant2(userObject, validateFrom, validateUntil, sessionManager.PrimarySession.Session, backEndWebServicesUrl);
						}
						return new CalendarRemoteParticipant(userObject, validateFrom, validateUntil, sessionManager.PrimarySession.Session, backEndWebServicesUrl);
					}
				}
				else
				{
					Globals.ConsistencyChecksTracer.TraceDebug<ExchangePrincipal>(0L, "Unable to access the remote mailbox {0}.", exchangePrincipal);
				}
			}
			catch (WrongServerException exception)
			{
				CalendarParticipant.HandleException(exception, exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			return null;
		}

		internal static bool InternalShouldProcessMailbox(ExchangePrincipal principal)
		{
			return principal.RecipientType == RecipientType.UserMailbox;
		}

		private static Uri GetBackEndWebServicesUrl(IMailboxInfo mailbox)
		{
			Uri result = null;
			Exception ex = null;
			try
			{
				result = BackEndLocator.GetBackEndWebServicesUrl(mailbox);
			}
			catch (BackEndLocatorException ex2)
			{
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				ex = ex3;
			}
			catch (DataSourceOperationException ex4)
			{
				ex = ex4;
			}
			catch (DataValidationException ex5)
			{
				ex = ex5;
			}
			finally
			{
				if (ex != null)
				{
					Globals.ConsistencyChecksTracer.TraceError<IMailboxInfo, Exception>(0L, "Unable to find a server to process the request for {0} - {1}", mailbox, ex);
				}
			}
			return result;
		}

		private static void HandleException(Exception exception, string mailboxSmtpAddress)
		{
			Globals.ConsistencyChecksTracer.TraceError<Exception, string>(0L, "{0}: Could not access remote server to open mailbox {1}.", exception, mailboxSmtpAddress);
		}

		public virtual void Dispose()
		{
			if (this.ItemList != null)
			{
				foreach (CalendarInstanceContext calendarInstanceContext in this.ItemList.Values)
				{
					calendarInstanceContext.Dispose();
				}
				this.ItemList.Clear();
				this.ItemList = null;
			}
		}

		internal abstract void ValidateMeetings(ref Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent, Action<long> onItemRepaired);

		internal void ValidateInstance(CalendarInstanceContext instanceContext, Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent, Action<long> onItemRepaired)
		{
			GlobalObjectId item = new GlobalObjectId(instanceContext.ValidationContext.BaseItem.CleanGlobalObjectId);
			if (!this.inquiredMasterGoids.Contains(item))
			{
				try
				{
					instanceContext.Validate(organizerRumsSent, this.inquiredMasterGoids, onItemRepaired);
					return;
				}
				catch (AttachmentExceededException arg)
				{
					Globals.ConsistencyChecksTracer.TraceError<AttachmentExceededException>((long)this.GetHashCode(), "{0}: Unable to save RUM", arg);
					return;
				}
			}
			Globals.ConsistencyChecksTracer.TraceDebug<GlobalObjectId>((long)this.GetHashCode(), "Skipping meeting validation because its master instance already sent an inquiry message. Meeting GOID: {0}", instanceContext.ValidationContext.BaseItem.GlobalObjectId);
		}

		private List<GlobalObjectId> inquiredMasterGoids;
	}
}
