using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Mapi;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal sealed class RuleManager
	{
		public void Add(Rule rule)
		{
			if (this.toAdd == null)
			{
				this.toAdd = new List<Rule>();
			}
			this.toAdd.Add(rule);
		}

		public void Update(Rule rule)
		{
			if (this.toUpdate == null)
			{
				this.toUpdate = new List<Rule>();
			}
			this.toUpdate.Add(rule);
		}

		public void Remove(Rule rule)
		{
			if (this.toRemove == null)
			{
				this.toRemove = new List<Rule>();
			}
			this.toRemove.Add(rule);
		}

		public void CommitChanges(MailboxSession itemStore, MapiFolder folder)
		{
			Exception ex = null;
			try
			{
				if (this.toRemove != null)
				{
					folder.DeleteRules(this.toRemove.ToArray());
					List<byte[]> list = new List<byte[]>();
					foreach (Rule rule in this.toRemove)
					{
						RuleManager.Tracer.TraceDebug<IExchangePrincipal, string>((long)this.GetHashCode(), "Mailbox:{0}: Deleted rule '{1}'", itemStore.MailboxOwner, rule.Name);
						RuleManager.TracerPfd.TracePfd<int, IExchangePrincipal, string>((long)this.GetHashCode(), "PFD IWO {0} Mailbox:{1}: Deleted rule '{2}'", 29591, itemStore.MailboxOwner, rule.Name);
						if (((RuleAction.OOFReply)rule.Actions[0]).ReplyTemplateMessageEntryID != null)
						{
							list.Add(((RuleAction.OOFReply)rule.Actions[0]).ReplyTemplateMessageEntryID);
						}
					}
					if (list.Count > 0)
					{
						folder.DeleteMessages(DeleteMessagesFlags.None, list.ToArray());
						foreach (byte[] bytes in list)
						{
							RuleManager.Tracer.TraceDebug<IExchangePrincipal, ByteArray>((long)this.GetHashCode(), "Mailbox:{0}: Deleted reply template '{1}'", itemStore.MailboxOwner, new ByteArray(bytes));
							RuleManager.TracerPfd.TracePfd<int, IExchangePrincipal, ByteArray>((long)this.GetHashCode(), "PFD IWO {0} Mailbox:{1}: Deleted reply template '{2}'", 19607, itemStore.MailboxOwner, new ByteArray(bytes));
						}
					}
					this.toRemove = null;
				}
				if (this.toUpdate != null)
				{
					folder.ModifyRules(this.toUpdate.ToArray());
					foreach (Rule rule2 in this.toUpdate)
					{
						RuleManager.Tracer.TraceDebug<IExchangePrincipal, string>((long)this.GetHashCode(), "Mailbox:{0}: Updated rule '{1}'", itemStore.MailboxOwner, rule2.Name);
						RuleManager.TracerPfd.TracePfd<int, IExchangePrincipal, string>((long)this.GetHashCode(), "PFD IWO {0} Mailbox:{1}: Updated rule '{2}'", 27799, itemStore.MailboxOwner, rule2.Name);
					}
					this.toUpdate = null;
				}
				if (this.toAdd != null)
				{
					folder.AddRules(this.toAdd.ToArray());
					foreach (Rule rule3 in this.toAdd)
					{
						RuleManager.Tracer.TraceDebug<IExchangePrincipal, string>((long)this.GetHashCode(), "Mailbox:{0}: Created new rule '{1}'", itemStore.MailboxOwner, rule3.Name);
						RuleManager.TracerPfd.TracePfd<int, IExchangePrincipal, string>((long)this.GetHashCode(), "PFD IWO {0} Mailbox:{1}: Created new rule '{2}'", 23703, itemStore.MailboxOwner, rule3.Name);
					}
					this.toAdd = null;
				}
			}
			catch (MapiExceptionInvalidParameter mapiExceptionInvalidParameter)
			{
				ex = mapiExceptionInvalidParameter;
			}
			catch (MapiExceptionNotFound mapiExceptionNotFound)
			{
				ex = mapiExceptionNotFound;
			}
			catch (MapiExceptionQuotaExceeded mapiExceptionQuotaExceeded)
			{
				ex = mapiExceptionQuotaExceeded;
			}
			catch (MapiExceptionShutoffQuotaExceeded mapiExceptionShutoffQuotaExceeded)
			{
				ex = mapiExceptionShutoffQuotaExceeded;
			}
			catch (MapiExceptionNotEnoughMemory mapiExceptionNotEnoughMemory)
			{
				ex = mapiExceptionNotEnoughMemory;
			}
			if (ex is MapiExceptionQuotaExceeded || ex is MapiExceptionShutoffQuotaExceeded || ex is MapiExceptionNotEnoughMemory)
			{
				RuleManager.Tracer.TraceError<IExchangePrincipal, Exception>((long)this.GetHashCode(), "Mailbox:{0}: cannot commit rule changes because exceeded rule quota. Exception {1}", itemStore.MailboxOwner, ex);
				Globals.OOFLogger.LogEvent(InfoWorkerEventLogConstants.Tuple_OOFRulesQuotaExceeded, itemStore.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), new object[]
				{
					itemStore.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()
				});
			}
			if (ex != null)
			{
				RuleManager.Tracer.TraceError<IExchangePrincipal, Exception>((long)this.GetHashCode(), "Mailbox:{0}: An OOF rule could not be saved. Exception: {1}", itemStore.MailboxOwner, ex);
				throw new OofRulesSaveException(ex);
			}
		}

		private List<Rule> toRemove;

		private List<Rule> toAdd;

		private List<Rule> toUpdate;

		private static readonly Trace Tracer = ExTraceGlobals.OOFTracer;

		private static readonly Trace TracerPfd = ExTraceGlobals.PFDTracer;
	}
}
