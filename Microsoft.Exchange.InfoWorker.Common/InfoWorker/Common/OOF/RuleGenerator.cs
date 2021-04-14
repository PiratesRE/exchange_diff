using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Mapi;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal sealed class RuleGenerator : IDisposable
	{
		public RuleGenerator(MailboxSession itemStore, UserOofSettings userOofSettings)
		{
			this.itemStore = itemStore;
			this.userOofSettings = userOofSettings;
			this.ruleManager = new RuleManager();
		}

		public void Dispose()
		{
			if (this.inboxFolder != null)
			{
				this.inboxFolder.Dispose();
				this.inboxFolder = null;
			}
			this.isDisposed = true;
		}

		public void OnUserOofSettingsChanges()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("RuleGenerator");
			}
			Rule[] rules = null;
			try
			{
				rules = this.InboxFolder.GetRules(new PropTag[0]);
			}
			catch (MapiExceptionDataIntegrity innerException)
			{
				throw new IWTransientException(Strings.descFailedToGetRules, innerException);
			}
			this.GenerateOofRules(rules, ".Global", null);
			this.ruleManager.CommitChanges(this.itemStore, this.InboxFolder);
		}

		internal static bool IsEmptyString(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return true;
			}
			bool result;
			using (MemoryStream memoryStream = new MemoryStream(message.Length))
			{
				UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
				byte[] bytes = unicodeEncoding.GetBytes(message);
				memoryStream.Write(bytes, 0, bytes.Length);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (MemoryStream memoryStream2 = new MemoryStream(bytes.Length))
				{
					new HtmlToText
					{
						InputEncoding = unicodeEncoding
					}.Convert(memoryStream, memoryStream2);
					memoryStream2.Seek(0L, SeekOrigin.Begin);
					byte[] array = new byte[memoryStream2.Length];
					if (memoryStream2.Length == 0L)
					{
						result = true;
					}
					else
					{
						if (memoryStream2.Length == 2L)
						{
							int num = memoryStream2.Read(array, 0, 2);
							if (num <= 0)
							{
								return true;
							}
							if (array[0] == 10 || array[0] == 13)
							{
								return true;
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		private void UpdateRuleCondition(Rule rule, Restriction condition)
		{
			if (!ConditionComparer.Equals(rule.Condition, condition))
			{
				rule.Condition = condition;
				this.ruleManager.Update(rule);
			}
		}

		private void GenerateOofRules(Rule[] rules, string ruleNameSuffix, string replyMessageAddendum)
		{
			Rule rule = null;
			Rule rule2 = null;
			Rule rule3 = null;
			int num = 8;
			string text = "Microsoft.Exchange.OOF.InternalSenders" + ruleNameSuffix;
			string text2 = "Microsoft.Exchange.OOF.KnownExternalSenders" + ruleNameSuffix;
			string text3 = "Microsoft.Exchange.OOF.AllExternalSenders" + ruleNameSuffix;
			ReplyBody internalReply = this.userOofSettings.InternalReply;
			ReplyBody externalReply = this.userOofSettings.ExternalReply;
			foreach (Rule rule4 in rules)
			{
				if (rule4 != null)
				{
					if (rule4.Provider == "Microsoft Exchange OOF Assistant")
					{
						if (rule4.Name == text)
						{
							if (rule != null)
							{
								this.ruleManager.Remove(rule4);
							}
							else
							{
								rule = rule4;
							}
						}
						else if (rule4.Name == text3)
						{
							if (rule2 != null)
							{
								this.ruleManager.Remove(rule4);
							}
							else
							{
								rule2 = rule4;
							}
						}
						else if (rule4.Name == text2)
						{
							if (rule3 != null)
							{
								this.ruleManager.Remove(rule4);
							}
							else
							{
								rule3 = rule4;
							}
						}
						else
						{
							this.ruleManager.Remove(rule4);
						}
					}
					else if (rule4.Provider == "MSFT:TDX OOF Rules" && rule4.Actions.Length > 0 && rule4.Actions[0] is RuleAction.OOFReply)
					{
						this.ruleManager.Remove(rule4);
					}
				}
			}
			this.HandleSenderGroupRule(rule, this.userOofSettings.OofState != OofState.Disabled, internalReply, text, "IPM.Note.Rules.OofTemplate.Microsoft", this.userOofSettings.SetByLegacyClient ? OofReplyType.Single : OofReplyType.Internal, RuleGenerator.ConditionType.Internal, num, this.userOofSettings.OofState);
			num++;
			this.HandleSenderGroupRule(rule3, this.userOofSettings.OofState != OofState.Disabled && this.userOofSettings.ExternalAudience == ExternalAudience.Known && !this.userOofSettings.SetByLegacyClient, externalReply, text2, "IPM.Note.Rules.ExternalOofTemplate.Microsoft", OofReplyType.External, RuleGenerator.ConditionType.KnownExternal, num, this.userOofSettings.OofState);
			this.HandleSenderGroupRule(rule2, this.userOofSettings.OofState != OofState.Disabled && this.userOofSettings.ExternalAudience == ExternalAudience.All && !this.userOofSettings.SetByLegacyClient, externalReply, text3, "IPM.Note.Rules.ExternalOofTemplate.Microsoft", OofReplyType.External, RuleGenerator.ConditionType.AllExternal, num, this.userOofSettings.OofState);
		}

		private void HandleSenderGroupRule(Rule rule, bool senderGroupEnabled, ReplyBody reply, string ruleName, string messageClass, OofReplyType oofReplyType, RuleGenerator.ConditionType conditionType, int sequenceNumber, OofState mailboxOofState)
		{
			int i = 0;
			while (i < 5)
			{
				i++;
				try
				{
					if (rule == null)
					{
						if (senderGroupEnabled && !RuleGenerator.IsEmptyString(reply.RawMessage))
						{
							this.CreateOofRule(reply, ruleName, messageClass, oofReplyType, conditionType, sequenceNumber);
						}
						else
						{
							RuleGenerator.Tracer.TraceDebug<IExchangePrincipal, string>((long)this.GetHashCode(), "Mailbox:{0}: Sender group '{1}' and rule doesn't exist. Nothing to do.", this.itemStore.MailboxOwner, ruleName);
						}
					}
					else if (senderGroupEnabled && !RuleGenerator.IsEmptyString(reply.RawMessage))
					{
						this.UpdateOofRule(rule, reply, ruleName, messageClass, oofReplyType, conditionType, sequenceNumber, mailboxOofState);
					}
					else
					{
						this.ruleManager.Remove(rule);
					}
					break;
				}
				catch (SaveConflictException ex)
				{
					if (i == 5)
					{
						RuleGenerator.Tracer.TraceError<IExchangePrincipal, int, SaveConflictException>((long)this.GetHashCode(), "Mailbox:{0}: Exception updating item, exception = {1}, retried {2} times, rethrowing", this.itemStore.MailboxOwner, 5, ex);
						throw;
					}
					RuleGenerator.Tracer.TraceError<IExchangePrincipal, SaveConflictException>((long)this.GetHashCode(), "Mailbox:{0}: Exception updating item, exception = {1}, retrying", this.itemStore.MailboxOwner, ex);
				}
				catch (ObjectNotFoundException ex2)
				{
					if (i == 5)
					{
						RuleGenerator.Tracer.TraceError<IExchangePrincipal, int, ObjectNotFoundException>((long)this.GetHashCode(), "Mailbox:{0}: Exception updating item, exception = {1}, retried {2} times, rethrowing", this.itemStore.MailboxOwner, 5, ex2);
						throw;
					}
					RuleGenerator.Tracer.TraceError<IExchangePrincipal, ObjectNotFoundException>((long)this.GetHashCode(), "Mailbox:{0}: Exception updating item, exception = {1}, retrying", this.itemStore.MailboxOwner, ex2);
				}
			}
		}

		private void CreateOofRule(ReplyBody reply, string ruleName, string messageClass, OofReplyType oofReplyType, RuleGenerator.ConditionType conditionType, int sequenceNumber)
		{
			Guid guid = Guid.NewGuid();
			byte[] replyTemplateMessageEntryID = this.CreateOofReplyTemplate(this.itemStore, guid, reply, messageClass, oofReplyType);
			Restriction restriction = this.GetRestriction(conditionType);
			RuleAction[] actions = new RuleAction[]
			{
				new RuleAction.OOFReply(replyTemplateMessageEntryID, guid)
			};
			Rule rule = new Rule();
			rule.Name = ruleName;
			rule.Provider = "Microsoft Exchange OOF Assistant";
			rule.StateFlags = (RuleStateFlags.KeepOOFHistory | RuleStateFlags.OnlyWhenOOFEx);
			rule.ExecutionSequence = sequenceNumber;
			rule.Condition = restriction;
			rule.Actions = actions;
			if (conditionType == RuleGenerator.ConditionType.KnownExternal)
			{
				rule.IsExtended = true;
			}
			this.ruleManager.Add(rule);
			RuleGenerator.TracerPfd.TracePfd<int, string>((long)this.GetHashCode(), "PFD IWO {0} Creating OOF rule {1}", 21399, rule.Name);
		}

		private void UpdateOofRule(Rule rule, ReplyBody reply, string ruleName, string messageClass, OofReplyType oofReplyType, RuleGenerator.ConditionType conditionType, int sequenceNumber, OofState mailboxOofState)
		{
			bool flag = false;
			bool flag2 = false;
			Restriction restriction = this.GetRestriction(conditionType);
			RuleAction.OOFReply ruleAction = (RuleAction.OOFReply)rule.Actions[0];
			ReplyTemplate replyTemplate = null;
			try
			{
				replyTemplate = ReplyTemplate.Find(this.itemStore, ruleAction);
				if (replyTemplate != null)
				{
					RuleGenerator.Tracer.TraceDebug<IExchangePrincipal, string>((long)this.GetHashCode(), "Mailbox:{0}: Updating ReplyTemplate to Body: '{1}'", this.itemStore.MailboxOwner, reply.RawMessage);
					if (this.userOofSettings.SetByLegacyClient)
					{
						flag2 = !replyTemplate.PlainTextBody.Equals(reply.RawMessage, StringComparison.Ordinal);
						replyTemplate.PlainTextBody = reply.RawMessage;
					}
					else
					{
						string value = TextUtil.ConvertHtmlToPlainText(reply.RawMessage);
						flag2 = !replyTemplate.PlainTextBody.Equals(value, StringComparison.Ordinal);
						replyTemplate.CharSet = this.GetDefaultCharsetForCountryCode(reply.LanguageTag);
						replyTemplate.HtmlBody = reply.RawMessage;
					}
					replyTemplate.OofReplyType = oofReplyType;
					replyTemplate.ClassName = messageClass;
					replyTemplate.SaveChanges();
					if (flag2)
					{
						rule.StateFlags |= RuleStateFlags.ClearOOFHistory;
						byte[] propValue = this.itemStore.__ContainedMapiStore.GlobalIdFromId(rule.ID);
						OofHistory.RemoveOofHistoryEntriesWithProperty(this.itemStore, mailboxOofState != OofState.Disabled, OofHistory.PropId.GlobalRuleId, propValue);
						RuleGenerator.Tracer.TraceDebug<IExchangePrincipal, string>((long)this.GetHashCode(), "Mailbox:{0}: Updated reply template for global rule '{1}'", this.itemStore.MailboxOwner, ruleName);
					}
				}
				else
				{
					Guid guid = Guid.NewGuid();
					byte[] replyTemplateMessageEntryID = this.CreateOofReplyTemplate(this.itemStore, guid, reply, messageClass, oofReplyType);
					rule.Actions = new RuleAction[]
					{
						new RuleAction.OOFReply(replyTemplateMessageEntryID, guid)
					};
					flag = true;
					RuleGenerator.Tracer.TraceDebug<IExchangePrincipal, string>((long)this.GetHashCode(), "Mailbox:{0}: Created new reply template and updated global rule '{1}'", this.itemStore.MailboxOwner, ruleName);
				}
				RuleGenerator.TracerPfd.TracePfd<int, string, IExchangePrincipal>((long)this.GetHashCode(), "PFD IWO {0} Updated OOF Rule '{1}' for Mailbox:{2}", 25495, ruleName, this.itemStore.MailboxOwner);
			}
			finally
			{
				if (replyTemplate != null)
				{
					replyTemplate.Dispose();
				}
			}
			if (!ConditionComparer.Equals(rule.Condition, restriction))
			{
				rule.Condition = restriction;
				flag = true;
				RuleGenerator.Tracer.TraceDebug<IExchangePrincipal, string>((long)this.GetHashCode(), "Mailbox:{0}: Updated rule '{1}' with new condition", this.itemStore.MailboxOwner, ruleName);
			}
			if (flag || flag2)
			{
				this.ruleManager.Update(rule);
			}
		}

		private byte[] CreateOofReplyTemplate(MailboxSession itemStore, Guid templateGuid, ReplyBody reply, string messageClass, OofReplyType oofReplyType)
		{
			byte[] entryId;
			using (ReplyTemplate replyTemplate = ReplyTemplate.Create(itemStore, templateGuid, messageClass, oofReplyType))
			{
				RuleGenerator.Tracer.TraceDebug<IExchangePrincipal, string>((long)this.GetHashCode(), "Mailbox:{0}: Creating ReplyTemplate to Body: '{1}'", this.itemStore.MailboxOwner, reply.RawMessage);
				if (this.userOofSettings.SetByLegacyClient)
				{
					replyTemplate.PlainTextBody = reply.RawMessage;
				}
				else
				{
					replyTemplate.CharSet = this.GetDefaultCharsetForCountryCode(reply.LanguageTag);
					replyTemplate.HtmlBody = reply.RawMessage;
				}
				replyTemplate.SaveChanges();
				RuleGenerator.TracerPfd.TracePfd<int, IExchangePrincipal, string>((long)this.GetHashCode(), "PFD IWO {0} Mailbox:{1}: Created ReplyTemplate to Body: '{2}'", 17303, this.itemStore.MailboxOwner, reply.RawMessage);
				entryId = replyTemplate.EntryId;
			}
			return entryId;
		}

		private string GetDefaultCharsetForCountryCode(string countryCode)
		{
			string result = null;
			Culture culture;
			if (countryCode != null && Culture.TryGetCulture(countryCode, out culture))
			{
				result = culture.MimeCharset.Name;
			}
			return result;
		}

		private Restriction GetRestriction(RuleGenerator.ConditionType conditionType)
		{
			Restriction result;
			if (conditionType == RuleGenerator.ConditionType.Internal)
			{
				result = RuleConditionFactory.CreateInternalSendersGroupCondition();
			}
			else if (conditionType == RuleGenerator.ConditionType.KnownExternal)
			{
				result = RuleConditionFactory.CreateKnownExternalSendersGroupCondition(UserContacts.GetEmailAddresses(this.itemStore));
			}
			else
			{
				if (conditionType != RuleGenerator.ConditionType.AllExternal)
				{
					throw new ArgumentException("Unknown ConditionType");
				}
				result = RuleConditionFactory.CreateAllExternalSendersGroupCondition();
			}
			return result;
		}

		private MapiStore MapiStore
		{
			get
			{
				return this.itemStore.__ContainedMapiStore;
			}
		}

		private MapiFolder InboxFolder
		{
			get
			{
				if (this.inboxFolder == null)
				{
					this.inboxFolder = this.MapiStore.GetInboxFolder();
				}
				return this.inboxFolder;
			}
		}

		private const string SendersRuleNameBase = "Microsoft.Exchange.OOF";

		private const string InternalSendersRuleName = "Microsoft.Exchange.OOF.InternalSenders";

		private const string KnownExternalSendersRuleName = "Microsoft.Exchange.OOF.KnownExternalSenders";

		private const string AllExternalSendersRuleName = "Microsoft.Exchange.OOF.AllExternalSenders";

		private const string GlobalRuleNameSuffix = ".Global";

		private const string OofAssistantRuleProviderName = "Microsoft Exchange OOF Assistant";

		private const string LegacyOofReplyRuleProviderName = "MSFT:TDX OOF Rules";

		private MailboxSession itemStore;

		private UserOofSettings userOofSettings;

		private RuleManager ruleManager;

		private bool isDisposed;

		private MapiFolder inboxFolder;

		private static readonly Trace Tracer = ExTraceGlobals.OOFTracer;

		private static readonly Trace TracerPfd = ExTraceGlobals.PFDTracer;

		private enum ConditionType
		{
			Internal,
			KnownExternal,
			AllExternal
		}
	}
}
