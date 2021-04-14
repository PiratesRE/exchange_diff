using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class JunkEmailRule
	{
		private JunkEmailRule(MailboxSession session, ADUser currentUser, ICollection<string> userProxyAddressCollection, ICollection<string> acceptedDomains)
		{
			this.session = session;
			this.isEnabled = true;
			this.isNew = false;
			this.safeListsOnly = false;
			this.safeListsOnlyDirty = false;
			this.isContactsTrusted = false;
			this.currentUser = currentUser;
			this.userProxyAddressCollection = userProxyAddressCollection;
			this.acceptedDomains = acceptedDomains;
			this.lastContactsUpdate = ExDateTime.MinValue;
			this.trustedSenderEmail = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.userProxyAddressCollection);
			this.trustedSenderDomain = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.acceptedDomains);
			this.trustedRecipientEmail = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.userProxyAddressCollection);
			this.trustedRecipientDomain = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.acceptedDomains);
			this.blockedSenderEmail = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.BlockedList, this.userProxyAddressCollection);
			this.blockedSenderDomain = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.BlockedList, this.acceptedDomains);
			this.trustedContactsEmail = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.userProxyAddressCollection);
			this.GetRuleFromServer();
		}

		internal static JunkEmailRule Create(MailboxSession session)
		{
			return JunkEmailRule.Create(session, false);
		}

		internal static JunkEmailRule Create(MailboxSession session, bool filterJunkEmailRule)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			ADUser aduser;
			IRecipientSession recipSession;
			JunkEmailRule.GetCurrentUserAndRecipSession(session, out aduser, out recipSession);
			ICollection<string> collection = null;
			ICollection<string> collection2 = null;
			if (filterJunkEmailRule)
			{
				collection = JunkEmailRule.GetUserProxyAddressCollection(aduser);
				collection2 = JunkEmailRule.GetAcceptedDomainsCollection(recipSession);
			}
			return new JunkEmailRule(session, aduser, collection, collection2);
		}

		public bool IsContactsFolderTrusted
		{
			get
			{
				return this.isContactsTrusted;
			}
		}

		public bool TrustedListsOnly
		{
			get
			{
				return this.safeListsOnly;
			}
			set
			{
				this.safeListsOnly = value;
				this.safeListsOnlyDirty = true;
				Restriction.ExistRestriction existRestriction = (Restriction.ExistRestriction)this.resTrustedSendersOnly.Restrictions[0];
				if (this.safeListsOnly)
				{
					existRestriction.Tag = PropTag.SenderAddrType;
					this.resTrustedSendersOnly.Restrictions[1] = Restriction.Content(PropTag.SenderAddrType, "SMTP", ContentFlags.IgnoreCase);
					return;
				}
				existRestriction.Tag = PropTag.SpamConfidenceLevel;
				this.resTrustedSendersOnly.Restrictions[1] = Restriction.GT(PropTag.SpamConfidenceLevel, -1);
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				this.isEnabled = value;
			}
		}

		public bool IsContactsCacheOutOfDate
		{
			get
			{
				return this.lastContactsUpdate.IncrementDays(7) <= ExDateTime.UtcNow;
			}
		}

		public System.Collections.ObjectModel.ReadOnlyCollection<string> TrustedContactsEmailCollection
		{
			get
			{
				return new System.Collections.ObjectModel.ReadOnlyCollection<string>(this.trustedContactsEmail);
			}
		}

		public JunkEmailCollection TrustedSenderEmailCollection
		{
			get
			{
				return this.trustedSenderEmail;
			}
		}

		public JunkEmailCollection TrustedSenderDomainCollection
		{
			get
			{
				return this.trustedSenderDomain;
			}
		}

		public JunkEmailCollection TrustedRecipientEmailCollection
		{
			get
			{
				return this.trustedRecipientEmail;
			}
		}

		public JunkEmailCollection TrustedRecipientDomainCollection
		{
			get
			{
				return this.trustedRecipientDomain;
			}
		}

		public JunkEmailCollection BlockedSenderEmailCollection
		{
			get
			{
				return this.blockedSenderEmail;
			}
		}

		public JunkEmailCollection BlockedSenderDomainCollection
		{
			get
			{
				return this.blockedSenderDomain;
			}
		}

		internal JunkEmailCollection.ValidationProblem AddTrustedContact(string email, IRecipientSession recipSession)
		{
			bool flag = false;
			if (!this.trustedContactsEmail.Contains(email))
			{
				try
				{
					if (string.IsNullOrEmpty(email) || !SmtpAddress.IsValidSmtpAddress(email))
					{
						ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), (string.IsNullOrEmpty(email) ? "Empty string" : email) + " is an invalid SMTP address");
						return JunkEmailCollection.ValidationProblem.FormatError;
					}
					SmtpProxyAddress proxyAddress = new SmtpProxyAddress(email, true);
					flag = recipSession.IsRecipientInOrg(proxyAddress);
				}
				catch (NonUniqueRecipientException ex)
				{
					ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), ex.Message);
					return JunkEmailCollection.ValidationProblem.Duplicate;
				}
				if (!flag)
				{
					JunkEmailCollection.ValidationProblem validationProblem = this.trustedContactsEmail.TryAdd(email);
					if (validationProblem != JunkEmailCollection.ValidationProblem.NoError)
					{
						ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), email + " could not be added to Trusted Contacts");
					}
					return validationProblem;
				}
				return JunkEmailCollection.ValidationProblem.NoError;
			}
			return JunkEmailCollection.ValidationProblem.NoError;
		}

		public void SynchronizeContactsCache()
		{
			this.trustedContactsEmail.Clear();
			try
			{
				IRecipientSession adrecipientSession = this.session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
				using (ContactsFolder contactsFolder = ContactsFolder.Bind(this.session, DefaultFolderType.Contacts))
				{
					using (QueryResult queryResult = contactsFolder.ItemQuery(ItemQueryType.None, null, null, JunkEmailRule.contactProps))
					{
						string text;
						JunkEmailCollection.ValidationProblem validationProblem;
						for (;;)
						{
							object[][] rows = queryResult.GetRows(100);
							if (rows.Length == 0)
							{
								goto IL_D7;
							}
							foreach (object[] array2 in rows)
							{
								for (int j = 0; j < array2.Length; j += 2)
								{
									string strA;
									if (array2[j] != null && array2[j + 1] != null && (text = (array2[j] as string)) != null && (strA = (array2[j + 1] as string)) != null && string.Compare(strA, "SMTP", StringComparison.CurrentCultureIgnoreCase) == 0)
									{
										validationProblem = this.AddTrustedContact(text, adrecipientSession);
										if (validationProblem == JunkEmailCollection.ValidationProblem.TooManyEntries)
										{
											goto Block_12;
										}
									}
								}
							}
						}
						Block_12:
						throw new JunkEmailValidationException(text, validationProblem);
						IL_D7:;
					}
				}
				this.isContactsTrusted = true;
				this.lastContactsUpdate = ExDateTime.UtcNow;
			}
			catch (DataSourceOperationException ex)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, this, "JunkEmailRule.SynchronizeContactsCache. Failed due to directory exception {0}.", new object[]
				{
					ex
				});
			}
			catch (DataSourceTransientException ex2)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, this, "JunkEmailRule.SynchronizeContactsCache. Failed due to directory exception {0}.", new object[]
				{
					ex2
				});
			}
		}

		public void ClearContactsCache()
		{
			this.trustedContactsEmail.Clear();
			this.isContactsTrusted = false;
		}

		public void Save()
		{
			StoreObjectId defaultFolderId = this.session.GetDefaultFolderId(DefaultFolderType.JunkEmail);
			if (defaultFolderId == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ExUnableToOpenOrCreateDefaultFolder(ClientStrings.JunkEmail));
			}
			if (this.isNew)
			{
				this.junkRule = new Rule();
			}
			RuleStateFlags ruleStateFlags = (RuleStateFlags)0;
			if (this.isEnabled)
			{
				ruleStateFlags |= RuleStateFlags.Enabled;
			}
			ruleStateFlags |= RuleStateFlags.ExitAfterExecution;
			ruleStateFlags |= RuleStateFlags.SkipIfSCLIsSafe;
			this.junkRule.StateFlags = ruleStateFlags;
			this.junkRule.IsExtended = true;
			this.junkRule.ExecutionSequence = 0;
			this.junkRule.UserFlags = 0U;
			this.junkRule.Level = 0;
			this.junkRule.Name = "Junk E-mail Rule";
			this.junkRule.Provider = "JunkEmailRule";
			Rule.ProviderData providerData;
			providerData.Version = 1U;
			providerData.RuleSearchKey = 0U;
			providerData.TimeStamp = ExDateTime.UtcNow.ToFileTimeUtc();
			this.junkRule.ProviderData = providerData.ToByteArray();
			this.LoadRestrictionsFromList(this.resBlockedSenders, this.blockedSenderEmail, ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.userProxyAddressCollection);
			this.LoadRestrictionsFromList(this.resTrustedSendersEmails, this.trustedSenderEmail, ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.userProxyAddressCollection);
			this.LoadRestrictionsFromList(this.resTrustedRecipientsEmails, this.trustedRecipientEmail, ContentFlags.IgnoreCase, PropTag.EmailAddress, this.userProxyAddressCollection);
			this.LoadRestrictionsFromList(this.resBlockedDomains, this.blockedSenderDomain, ContentFlags.SubString | ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.acceptedDomains);
			this.LoadRestrictionsFromList(this.resTrustedSenderDomains, this.trustedSenderDomain, ContentFlags.SubString | ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.acceptedDomains);
			this.LoadRestrictionsFromList(this.resTrustedRecipientDomains, this.trustedRecipientDomain, ContentFlags.SubString | ContentFlags.IgnoreCase, PropTag.EmailAddress, this.acceptedDomains);
			if (this.IsContactsFolderTrusted)
			{
				this.LoadRestrictionsFromList(this.resTrustedContactsEmails, this.trustedContactsEmail, ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.userProxyAddressCollection);
			}
			else
			{
				this.resTrustedContactsEmails.Restrictions = Array<Restriction>.Empty;
			}
			this.junkRule.Condition = this.resJunk;
			NamedProp namedProp = new NamedProp(WellKnownPropertySet.PublicStrings, "http://schemas.microsoft.com/exchange/junkemailmovestamp");
			NamedProp namedProp2 = WellKnownNamedProperties.Find(namedProp);
			if (namedProp2 != null)
			{
				namedProp = namedProp2;
			}
			PropTag propTag = PropTag.Null;
			NamedProp[] np = new NamedProp[]
			{
				namedProp
			};
			PropTag[] array = null;
			StoreSession storeSession = this.session;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				array = this.session.Mailbox.MapiStore.GetIDsFromNames(true, np);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("JunkEmailRule::Save.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("JunkEmailRule::Save.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			if (array.Length > 0)
			{
				propTag = (array[0] | (PropTag)20U);
			}
			long num = (long)((ulong)this.GetJunkMoveStamp());
			this.junkRule.Actions = new RuleAction[]
			{
				new RuleAction.InMailboxMove(defaultFolderId.ProviderLevelItemId),
				new RuleAction.Tag(new PropValue(propTag, num))
			};
			ExDateTime exDateTime = ExTimeZone.UtcTimeZone.ConvertDateTime(this.lastContactsUpdate);
			List<PropValue> list = new List<PropValue>();
			list.Add(new PropValue(PropTag.ReportTime, (DateTime)exDateTime));
			list.Add(new PropValue((PropTag)1627389955U, this.isContactsTrusted ? 1 : 0));
			if (this.safeListsOnlyDirty)
			{
				if (this.safeListsOnly)
				{
					list.Add(new PropValue((PropTag)1627455491U, int.MinValue));
				}
				else
				{
					list.Add(new PropValue((PropTag)1627455491U, this.currentMailboxJunkThresholdSetting));
				}
			}
			this.junkRule.ExtraProperties = list.ToArray();
			using (Folder folder = Folder.Bind(this.session, this.session.GetDefaultFolderId(DefaultFolderType.Inbox)))
			{
				StoreSession storeSession2 = this.session;
				bool flag2 = false;
				try
				{
					if (storeSession2 != null)
					{
						storeSession2.BeginMapiCall();
						storeSession2.BeginServerHealthCall();
						flag2 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					if (this.isNew)
					{
						folder.MapiFolder.AddRules(new Rule[]
						{
							this.junkRule
						});
						Rule[] rules = folder.MapiFolder.GetRules(new PropTag[0]);
						foreach (Rule rule in rules)
						{
							if (rule.IsExtended && rule.Name == "Junk E-mail Rule")
							{
								this.junkRule = rule;
								break;
							}
						}
						this.isNew = false;
					}
					else
					{
						folder.MapiFolder.ModifyRules(new Rule[]
						{
							this.junkRule
						});
					}
				}
				catch (MapiPermanentException ex3)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex3, storeSession2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("JunkEmailRule::Save.", new object[0]),
						ex3
					});
				}
				catch (MapiRetryableException ex4)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex4, storeSession2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("JunkEmailRule::Save.", new object[0]),
						ex4
					});
				}
				finally
				{
					try
					{
						if (storeSession2 != null)
						{
							storeSession2.EndMapiCall();
							if (flag2)
							{
								storeSession2.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
			}
		}

		public void Clear()
		{
			if (this.junkRule != null)
			{
				using (Folder folder = Folder.Bind(this.session, this.session.GetDefaultFolderId(DefaultFolderType.Inbox)))
				{
					StoreSession storeSession = this.session;
					bool flag = false;
					try
					{
						if (storeSession != null)
						{
							storeSession.BeginMapiCall();
							storeSession.BeginServerHealthCall();
							flag = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						folder.MapiFolder.DeleteRules(new Rule[]
						{
							this.junkRule
						});
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("JunkEmailRule::Clear.", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("JunkEmailRule::Clear.", new object[0]),
							ex2
						});
					}
					finally
					{
						try
						{
							if (storeSession != null)
							{
								storeSession.EndMapiCall();
								if (flag)
								{
									storeSession.EndServerHealthCall();
								}
							}
						}
						finally
						{
							if (StorageGlobals.MapiTestHookAfterCall != null)
							{
								StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
							}
						}
					}
				}
			}
			this.junkRule = null;
			this.isNew = true;
			this.isEnabled = true;
			this.safeListsOnly = false;
			this.isContactsTrusted = false;
			this.lastContactsUpdate = ExDateTime.MinValue;
			this.resJunk = this.BuildJunkEmailRestrictionTemplate();
			this.SetRestrictionReferences();
		}

		private static bool LoadListFromRestrictions(Restriction.OrRestriction resOr, JunkEmailCollection strings, ICollection<string> collectionToExclude)
		{
			bool flag = false;
			if (resOr.Restrictions == null)
			{
				return !flag;
			}
			strings.Validating = false;
			foreach (Restriction restriction in resOr.Restrictions)
			{
				Restriction.ContentRestriction contentRestriction = restriction as Restriction.ContentRestriction;
				if (contentRestriction != null)
				{
					string text = (string)contentRestriction.PropValue.Value;
					if (JunkEmailRule.CheckIfShouldExclude(text, collectionToExclude))
					{
						flag = true;
					}
					else
					{
						strings.Add(text);
					}
				}
			}
			strings.Validating = true;
			return !flag;
		}

		private static bool IsValidJunkRestriction(Restriction res)
		{
			bool result = true;
			if (res is Restriction.AndRestriction)
			{
				Restriction.AndRestriction andRestriction = res as Restriction.AndRestriction;
				if (andRestriction.Restrictions.Length > 0 && andRestriction.Restrictions[0] is Restriction.OrRestriction)
				{
					Restriction.OrRestriction orRestriction = andRestriction.Restrictions[0] as Restriction.OrRestriction;
					if (orRestriction.Restrictions.Length <= 0 || !(orRestriction.Restrictions[0] is Restriction.OrRestriction))
					{
						result = false;
					}
					if (orRestriction.Restrictions.Length > 1 && orRestriction.Restrictions[1] is Restriction.AndRestriction)
					{
						Restriction.AndRestriction andRestriction2 = orRestriction.Restrictions[1] as Restriction.AndRestriction;
						if (andRestriction2.Restrictions.Length > 0 && andRestriction2.Restrictions[0] is Restriction.OrRestriction)
						{
							Restriction.OrRestriction orRestriction2 = andRestriction2.Restrictions[0] as Restriction.OrRestriction;
							if (orRestriction2.Restrictions.Length > 0 && orRestriction2.Restrictions[0] is Restriction.AndRestriction)
							{
								Restriction.AndRestriction andRestriction3 = orRestriction2.Restrictions[0] as Restriction.AndRestriction;
								if (andRestriction3.Restrictions.Length <= 1 || !(andRestriction3.Restrictions[0] is Restriction.ExistRestriction) || (!(andRestriction3.Restrictions[1] is Restriction.PropertyRestriction) && !(andRestriction3.Restrictions[1] is Restriction.ContentRestriction)))
								{
									result = false;
								}
							}
							else
							{
								result = false;
							}
							if (orRestriction2.Restrictions.Length <= 1 || !(orRestriction2.Restrictions[1] is Restriction.OrRestriction))
							{
								result = false;
							}
						}
						else
						{
							result = false;
						}
						if (andRestriction2.Restrictions.Length > 1 && andRestriction2.Restrictions[1] is Restriction.NotRestriction)
						{
							Restriction.NotRestriction notRestriction = andRestriction2.Restrictions[1] as Restriction.NotRestriction;
							if (notRestriction.Restriction is Restriction.OrRestriction)
							{
								Restriction.OrRestriction orRestriction3 = notRestriction.Restriction as Restriction.OrRestriction;
								if (orRestriction3.Restrictions.Length <= 0 || !(orRestriction3.Restrictions[0] is Restriction.OrRestriction))
								{
									result = false;
								}
								if (orRestriction3.Restrictions.Length > 1 && orRestriction3.Restrictions[1] is Restriction.RecipientRestriction)
								{
									Restriction.RecipientRestriction recipientRestriction = orRestriction3.Restrictions[1] as Restriction.RecipientRestriction;
									if (!(recipientRestriction.Restriction is Restriction.OrRestriction))
									{
										result = false;
									}
								}
								else
								{
									result = false;
								}
							}
							else
							{
								result = false;
							}
						}
						else
						{
							result = false;
						}
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = false;
				}
				if (andRestriction.Restrictions.Length > 1 && andRestriction.Restrictions[1] is Restriction.NotRestriction)
				{
					Restriction.NotRestriction notRestriction2 = andRestriction.Restrictions[1] as Restriction.NotRestriction;
					if (notRestriction2.Restriction is Restriction.OrRestriction)
					{
						Restriction.OrRestriction orRestriction4 = notRestriction2.Restriction as Restriction.OrRestriction;
						if (orRestriction4.Restrictions.Length <= 0 || !(orRestriction4.Restrictions[0] is Restriction.OrRestriction))
						{
							result = false;
						}
						if (orRestriction4.Restrictions.Length > 1 && orRestriction4.Restrictions[1] is Restriction.RecipientRestriction)
						{
							Restriction.RecipientRestriction recipientRestriction2 = orRestriction4.Restrictions[1] as Restriction.RecipientRestriction;
							if (!(recipientRestriction2.Restriction is Restriction.OrRestriction))
							{
								result = false;
							}
						}
						else
						{
							result = false;
						}
						if (orRestriction4.Restrictions.Length <= 2 || !(orRestriction4.Restrictions[2] is Restriction.OrRestriction))
						{
							result = false;
						}
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static bool CheckIfShouldExclude(string pivotString, ICollection<string> collectionToExclude)
		{
			return collectionToExclude != null && !string.IsNullOrEmpty(pivotString) && collectionToExclude.Contains(pivotString);
		}

		private void GetRuleFromServer()
		{
			using (Folder folder = Folder.Bind(this.session, this.session.GetDefaultFolderId(DefaultFolderType.Inbox)))
			{
				Rule[] array = null;
				StoreSession storeSession = this.session;
				bool flag = false;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					array = folder.MapiFolder.GetRules(JunkEmailRule.extraProps);
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("JunkEmailRule::GetRuleFromServer.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("JunkEmailRule::GetRuleFromServer.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag)
							{
								storeSession.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				foreach (Rule rule in array)
				{
					if (rule.IsExtended && rule.Name == "Junk E-mail Rule")
					{
						this.junkRule = rule;
						break;
					}
				}
			}
			if (this.junkRule == null)
			{
				this.isNew = true;
				this.resJunk = this.BuildJunkEmailRestrictionTemplate();
				this.SetRestrictionReferences();
				return;
			}
			this.isEnabled = ((this.junkRule.StateFlags & RuleStateFlags.Enabled) > (RuleStateFlags)0);
			foreach (PropValue propValue in this.junkRule.ExtraProperties)
			{
				PropTag propTag = propValue.PropTag;
				if (propTag != PropTag.ReportTime)
				{
					if (propTag != (PropTag)1627389955U)
					{
						if (propTag == (PropTag)1627455491U)
						{
							int @int = propValue.GetInt();
							this.safeListsOnly = (@int == int.MinValue);
							this.currentMailboxJunkThresholdSetting = @int;
						}
					}
					else
					{
						this.isContactsTrusted = (propValue.GetInt() > 0);
					}
				}
				else
				{
					ExDateTime exDateTime = new ExDateTime(this.session.ExTimeZone, propValue.GetDateTime());
					this.lastContactsUpdate = exDateTime;
				}
			}
			if (JunkEmailRule.IsValidJunkRestriction(this.junkRule.Condition))
			{
				this.resJunk = (this.junkRule.Condition as Restriction.AndRestriction);
			}
			else
			{
				this.resJunk = this.FixJunkRestriction(this.junkRule.Condition);
				if (this.safeListsOnly)
				{
					this.TrustedListsOnly = true;
				}
			}
			this.SetRestrictionReferences();
			this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(this.resBlockedSenders, this.blockedSenderEmail, this.userProxyAddressCollection) && this.allRestrictionsLoaded);
			this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(this.resBlockedDomains, this.blockedSenderDomain, this.acceptedDomains) && this.allRestrictionsLoaded);
			this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(this.resTrustedSenderDomains, this.trustedSenderDomain, this.acceptedDomains) && this.allRestrictionsLoaded);
			this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(this.resTrustedRecipientDomains, this.trustedRecipientDomain, this.acceptedDomains) && this.allRestrictionsLoaded);
			this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(this.resTrustedSendersEmails, this.trustedSenderEmail, this.userProxyAddressCollection) && this.allRestrictionsLoaded);
			this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(this.resTrustedRecipientsEmails, this.trustedRecipientEmail, this.userProxyAddressCollection) && this.allRestrictionsLoaded);
			if (this.IsContactsFolderTrusted)
			{
				this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(this.resTrustedContactsEmails, this.trustedContactsEmail, this.userProxyAddressCollection) && this.allRestrictionsLoaded);
			}
			Restriction.ExistRestriction existRestriction = (Restriction.ExistRestriction)this.resTrustedSendersOnly.Restrictions[0];
			if (this.safeListsOnly)
			{
				if (existRestriction.Tag != PropTag.SenderAddrType)
				{
					this.safeListsOnly = false;
					this.safeListsOnlyDirty = true;
					return;
				}
			}
			else if (existRestriction.Tag == PropTag.SenderAddrType)
			{
				this.safeListsOnly = true;
				this.safeListsOnlyDirty = true;
			}
		}

		private void LoadRestrictionsFromList(Restriction.OrRestriction resOr, JunkEmailCollection strings, ContentFlags flags, PropTag tag, ICollection<string> collectionToExclude)
		{
			if (strings.Count > 0)
			{
				List<Restriction> list = new List<Restriction>();
				for (int i = 0; i < strings.Count; i++)
				{
					if (!JunkEmailRule.CheckIfShouldExclude(strings[i], collectionToExclude))
					{
						list.Add(Restriction.Content(tag, strings[i], flags));
					}
				}
				resOr.Restrictions = list.ToArray();
				return;
			}
			resOr.Restrictions = Array<Restriction>.Empty;
		}

		private uint GetJunkMoveStamp()
		{
			uint num = 0U;
			uint result;
			using (Folder folder = Folder.Bind(this.session, DefaultFolderType.Inbox, new PropertyDefinition[]
			{
				InternalSchema.AdditionalRenEntryIds
			}))
			{
				byte[][] valueOrDefault = folder.GetValueOrDefault<byte[][]>(InternalSchema.AdditionalRenEntryIds, Array<byte[]>.Empty);
				if (valueOrDefault.Length >= 6 && valueOrDefault[5] != null && valueOrDefault[5].Length == 4)
				{
					try
					{
						num = BitConverter.ToUInt32(valueOrDefault[5], 0);
						return num;
					}
					catch (ArgumentException)
					{
						ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Corrupt Junk move stamp.  Attempting to recreate.");
					}
				}
				int num2 = Math.Max(6, valueOrDefault.Length);
				byte[][] array = new byte[num2][];
				valueOrDefault.CopyTo(array, 0);
				StoreObjectId defaultFolderId = this.session.GetDefaultFolderId(DefaultFolderType.JunkEmail);
				if (defaultFolderId == null)
				{
					throw new ObjectNotFoundException(ServerStrings.ExUnableToOpenOrCreateDefaultFolder(ClientStrings.JunkEmail));
				}
				num = ComputeCRC.Compute(0U, defaultFolderId.ProviderLevelItemId);
				array[5] = BitConverter.GetBytes(num);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == null)
					{
						array[i] = JunkEmailRule.emptyArray;
					}
				}
				using (Folder folder2 = Folder.Bind(this.session, DefaultFolderType.Configuration, new PropertyDefinition[]
				{
					InternalSchema.AdditionalRenEntryIds
				}))
				{
					folder2[InternalSchema.AdditionalRenEntryIds] = array;
					folder2.Save();
				}
				folder[InternalSchema.AdditionalRenEntryIds] = array;
				folder.Save();
				result = num;
			}
			return result;
		}

		private Restriction.AndRestriction BuildJunkEmailRestrictionTemplate()
		{
			return (Restriction.AndRestriction)Restriction.And(new Restriction[]
			{
				Restriction.Or(new Restriction[]
				{
					Restriction.Or(Array<Restriction>.Empty),
					Restriction.And(new Restriction[]
					{
						Restriction.Or(new Restriction[]
						{
							Restriction.And(new Restriction[]
							{
								Restriction.Exist(PropTag.SpamConfidenceLevel),
								Restriction.GT(PropTag.SpamConfidenceLevel, -1)
							}),
							Restriction.Or(Array<Restriction>.Empty)
						}),
						Restriction.Not(Restriction.Or(new Restriction[]
						{
							Restriction.Or(Array<Restriction>.Empty),
							new Restriction.RecipientRestriction(Restriction.Or(Array<Restriction>.Empty))
						}))
					})
				}),
				Restriction.Not(Restriction.Or(new Restriction[]
				{
					Restriction.Or(Array<Restriction>.Empty),
					new Restriction.RecipientRestriction(Restriction.Or(Array<Restriction>.Empty)),
					Restriction.Or(Array<Restriction>.Empty)
				}))
			});
		}

		private Restriction.AndRestriction FixJunkRestriction(Restriction resInput)
		{
			Restriction.AndRestriction andRestriction = resInput as Restriction.AndRestriction;
			Restriction.AndRestriction andRestriction2 = this.BuildJunkEmailRestrictionTemplate();
			JunkEmailCollection strings = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.userProxyAddressCollection);
			JunkEmailCollection strings2 = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.acceptedDomains);
			JunkEmailCollection strings3 = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.userProxyAddressCollection);
			JunkEmailCollection strings4 = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.acceptedDomains);
			JunkEmailCollection strings5 = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.BlockedList, this.userProxyAddressCollection);
			JunkEmailCollection strings6 = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.BlockedList, this.acceptedDomains);
			JunkEmailCollection junkEmailCollection = null;
			try
			{
				Restriction.OrRestriction resOr = (andRestriction.Restrictions[0] as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction;
				this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(resOr, strings5, this.userProxyAddressCollection) && this.allRestrictionsLoaded);
			}
			catch (NullReferenceException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse blocked senders");
			}
			catch (IndexOutOfRangeException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse blocked senders");
			}
			try
			{
				Restriction.OrRestriction resOr2 = (((andRestriction.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.OrRestriction;
				this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(resOr2, strings6, this.acceptedDomains) && this.allRestrictionsLoaded);
			}
			catch (NullReferenceException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse blocked sender domains");
			}
			catch (IndexOutOfRangeException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse blocked sender domains");
			}
			try
			{
				Restriction.OrRestriction resOr3 = ((((andRestriction.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction;
				this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(resOr3, strings2, this.acceptedDomains) && this.allRestrictionsLoaded);
			}
			catch (NullReferenceException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted sender domains");
			}
			catch (IndexOutOfRangeException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted sender domains");
			}
			try
			{
				Restriction.OrRestriction resOr4 = (((((andRestriction.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[1] as Restriction.RecipientRestriction).Restriction as Restriction.OrRestriction;
				this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(resOr4, strings4, this.acceptedDomains) && this.allRestrictionsLoaded);
			}
			catch (NullReferenceException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted recipient domains");
			}
			catch (IndexOutOfRangeException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted recipient domains");
			}
			try
			{
				Restriction.OrRestriction resOr5 = ((andRestriction.Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction;
				this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(resOr5, strings, this.userProxyAddressCollection) && this.allRestrictionsLoaded);
			}
			catch (NullReferenceException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted senders");
			}
			catch (IndexOutOfRangeException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted senders");
			}
			try
			{
				Restriction.OrRestriction resOr6 = (((andRestriction.Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[1] as Restriction.RecipientRestriction).Restriction as Restriction.OrRestriction;
				this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(resOr6, strings3, this.userProxyAddressCollection) && this.allRestrictionsLoaded);
			}
			catch (NullReferenceException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted recipients");
			}
			catch (IndexOutOfRangeException)
			{
				ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted recipients");
			}
			if (this.IsContactsFolderTrusted)
			{
				junkEmailCollection = JunkEmailCollection.Create(this, JunkEmailCollection.ListType.TrustedList, this.userProxyAddressCollection);
				try
				{
					Restriction.OrRestriction resOr7 = ((andRestriction.Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[2] as Restriction.OrRestriction;
					this.allRestrictionsLoaded = (JunkEmailRule.LoadListFromRestrictions(resOr7, junkEmailCollection, this.userProxyAddressCollection) && this.allRestrictionsLoaded);
				}
				catch (NullReferenceException)
				{
					ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted contacts");
					this.trustedContactsEmail.Clear();
					this.isContactsTrusted = false;
				}
				catch (IndexOutOfRangeException)
				{
					ExTraceGlobals.StorageTracer.TraceError((long)this.GetHashCode(), "Can't parse trusted contacts");
					this.trustedContactsEmail.Clear();
					this.isContactsTrusted = false;
				}
			}
			Restriction.OrRestriction resOr8 = (andRestriction2.Restrictions[0] as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction;
			Restriction.OrRestriction resOr9 = (((andRestriction2.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.OrRestriction;
			Restriction.OrRestriction resOr10 = ((((andRestriction2.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction;
			Restriction.OrRestriction resOr11 = (((((andRestriction2.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[1] as Restriction.RecipientRestriction).Restriction as Restriction.OrRestriction;
			Restriction.OrRestriction resOr12 = ((andRestriction2.Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction;
			Restriction.OrRestriction resOr13 = (((andRestriction2.Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[1] as Restriction.RecipientRestriction).Restriction as Restriction.OrRestriction;
			Restriction.OrRestriction orRestriction = ((andRestriction2.Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[2] as Restriction.OrRestriction;
			this.LoadRestrictionsFromList(resOr8, strings5, ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.userProxyAddressCollection);
			this.LoadRestrictionsFromList(resOr12, strings, ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.userProxyAddressCollection);
			this.LoadRestrictionsFromList(resOr13, strings3, ContentFlags.IgnoreCase, PropTag.EmailAddress, this.userProxyAddressCollection);
			this.LoadRestrictionsFromList(resOr9, strings6, ContentFlags.SubString | ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.acceptedDomains);
			this.LoadRestrictionsFromList(resOr10, strings2, ContentFlags.SubString | ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.acceptedDomains);
			this.LoadRestrictionsFromList(resOr11, strings4, ContentFlags.SubString | ContentFlags.IgnoreCase, PropTag.EmailAddress, this.acceptedDomains);
			if (this.IsContactsFolderTrusted)
			{
				if (junkEmailCollection.Count > 0)
				{
					this.LoadRestrictionsFromList(orRestriction, junkEmailCollection, ContentFlags.IgnoreCase, PropTag.SenderEmailAddress, this.userProxyAddressCollection);
				}
				else
				{
					orRestriction.Restrictions = Array<Restriction>.Empty;
				}
			}
			else
			{
				orRestriction.Restrictions = Array<Restriction>.Empty;
			}
			return andRestriction2;
		}

		private void SetRestrictionReferences()
		{
			this.resBlockedSenders = ((this.resJunk.Restrictions[0] as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction);
			this.resBlockedDomains = ((((this.resJunk.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.OrRestriction);
			this.resTrustedSenderDomains = (((((this.resJunk.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction);
			this.resTrustedRecipientDomains = ((((((this.resJunk.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[1] as Restriction.RecipientRestriction).Restriction as Restriction.OrRestriction);
			this.resTrustedSendersEmails = (((this.resJunk.Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[0] as Restriction.OrRestriction);
			this.resTrustedRecipientsEmails = ((((this.resJunk.Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[1] as Restriction.RecipientRestriction).Restriction as Restriction.OrRestriction);
			this.resTrustedContactsEmails = (((this.resJunk.Restrictions[1] as Restriction.NotRestriction).Restriction as Restriction.OrRestriction).Restrictions[2] as Restriction.OrRestriction);
			this.resTrustedSendersOnly = ((((this.resJunk.Restrictions[0] as Restriction.OrRestriction).Restrictions[1] as Restriction.AndRestriction).Restrictions[0] as Restriction.OrRestriction).Restrictions[0] as Restriction.AndRestriction);
		}

		public bool AllRestrictionsLoaded
		{
			get
			{
				return this.allRestrictionsLoaded;
			}
		}

		public static ICollection<string> GetUserProxyAddressCollection(ADUser currentUser)
		{
			HashSet<string> hashSet = null;
			if (currentUser != null)
			{
				hashSet = new HashSet<string>(currentUser.EmailAddresses.Count, StringComparer.OrdinalIgnoreCase);
				foreach (ProxyAddress proxyAddress in currentUser.EmailAddresses)
				{
					if (proxyAddress.Prefix == ProxyAddressPrefix.Smtp && !string.IsNullOrEmpty(proxyAddress.AddressString))
					{
						hashSet.TryAdd(proxyAddress.AddressString);
					}
				}
			}
			return hashSet;
		}

		internal int MaxNumberOfTrustedEntries
		{
			get
			{
				if (!this.maxValuesInitialized)
				{
					this.InitializeMaxValues();
				}
				return this.maxNumberOfTrustedEntries;
			}
		}

		internal int MaxNumberOfBlockedEntries
		{
			get
			{
				if (!this.maxValuesInitialized)
				{
					this.InitializeMaxValues();
				}
				return this.maxNumberOfBlockedEntries;
			}
		}

		internal ADUser CurrentUser
		{
			get
			{
				return this.currentUser;
			}
		}

		public static ICollection<string> GetAcceptedDomainsCollection(IRecipientSession recipSession)
		{
			HashSet<string> hashSet = null;
			if (recipSession != null)
			{
				hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(recipSession.DomainController, recipSession.ReadOnly, ConsistencyMode.IgnoreInvalid, recipSession.NetworkCredential, recipSession.SessionSettings, 1750, "GetAcceptedDomainsCollection", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\obj\\amd64\\Rules\\JunkEmailRule.cs");
				QueryFilter filter = new NotFilter(new BitMaskOrFilter(AcceptedDomainSchema.AcceptedDomainFlags, 1UL));
				OrganizationId currentOrganizationId = tenantOrTopologyConfigurationSession.SessionSettings.CurrentOrganizationId;
				ADObjectId rootId = (OrganizationId.ForestWideOrgId.Equals(currentOrganizationId) || currentOrganizationId == null) ? tenantOrTopologyConfigurationSession.GetOrgContainerId() : currentOrganizationId.ConfigurationUnit;
				ADPagedReader<AcceptedDomain> adpagedReader = tenantOrTopologyConfigurationSession.FindPaged<AcceptedDomain>(rootId, QueryScope.SubTree, filter, null, 0);
				using (IEnumerator<AcceptedDomain> enumerator = adpagedReader.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.DomainName.SmtpDomain != null)
						{
							hashSet.TryAdd(enumerator.Current.DomainName.SmtpDomain.Domain);
							hashSet.TryAdd(JunkEmailRule.SymbolAt + enumerator.Current.DomainName.SmtpDomain.Domain);
						}
					}
				}
			}
			return hashSet;
		}

		private void InitializeMaxValues()
		{
			if (this.CurrentUser != null)
			{
				this.maxNumberOfBlockedEntries = (this.CurrentUser.MaxBlockedSenders ?? 65535);
				this.maxNumberOfTrustedEntries = (this.CurrentUser.MaxSafeSenders ?? 1024);
			}
			else
			{
				this.maxNumberOfBlockedEntries = 65535;
				this.maxNumberOfTrustedEntries = 1024;
			}
			this.maxValuesInitialized = true;
		}

		public static void GetCurrentUserAndRecipSession(MailboxSession session, out ADUser user, out IRecipientSession recipSession)
		{
			IRecipientSession tenantOrRootOrgRecipientSession;
			ADRecipient adrecipient;
			try
			{
				tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, session.GetADSessionSettings(), 1822, "GetCurrentUserAndRecipSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\obj\\amd64\\Rules\\JunkEmailRule.cs");
				adrecipient = tenantOrRootOrgRecipientSession.FindByLegacyExchangeDN(session.MailboxOwnerLegacyDN);
			}
			catch (DataSourceOperationException ex)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "JunkEmailRule.TryInitializeCurrentUser. Failed due to directory exception {0}.", new object[]
				{
					ex
				});
			}
			catch (DataSourceTransientException ex2)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "JunkEmailRule.TryInitializeCurrentUser. Failed due to directory exception {0}.", new object[]
				{
					ex2
				});
			}
			catch (DataValidationException ex3)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex3, null, "JunkEmailRule.TryInitializeCurrentUser. Failed due to directory exception {0}.", new object[]
				{
					ex3
				});
			}
			user = (adrecipient as ADUser);
			recipSession = tenantOrRootOrgRecipientSession;
		}

		internal const string JunkEmailName = "Junk E-mail Rule";

		private const string JunkEmailRuleProvider = "JunkEmailRule";

		private const string TagJunkMoveStampName = "http://schemas.microsoft.com/exchange/junkemailmovestamp";

		private const PropTag TagJunkLastContactUpdate = PropTag.ReportTime;

		private const PropTag TagJunkIncludeContacts = (PropTag)1627389955U;

		private const PropTag TagJunkThreshold = (PropTag)1627455491U;

		private const int SpamFilteringNone = -1;

		private const int SpamFilteringLow = 6;

		private const int SpamFilteringMedium = 5;

		private const int SpamFilteringHigh = 3;

		private const int SpamFilteringTrustedOnly = -2147483648;

		private const int DefaultMaxNumberOfTrustedEntries = 1024;

		private const int DefaultMaxNumberOfBlockedEntries = 65535;

		private static readonly byte[] emptyArray = Array<byte>.Empty;

		private ICollection<string> userProxyAddressCollection;

		private ICollection<string> acceptedDomains;

		private bool allRestrictionsLoaded = true;

		private Restriction.AndRestriction resJunk;

		private Restriction.OrRestriction resBlockedSenders;

		private Restriction.OrRestriction resBlockedDomains;

		private Restriction.OrRestriction resTrustedSenderDomains;

		private Restriction.OrRestriction resTrustedRecipientDomains;

		private Restriction.OrRestriction resTrustedSendersEmails;

		private Restriction.OrRestriction resTrustedRecipientsEmails;

		private Restriction.OrRestriction resTrustedContactsEmails;

		private Restriction.AndRestriction resTrustedSendersOnly;

		private JunkEmailCollection trustedSenderEmail;

		private JunkEmailCollection trustedSenderDomain;

		private JunkEmailCollection trustedRecipientEmail;

		private JunkEmailCollection trustedRecipientDomain;

		private JunkEmailCollection blockedSenderEmail;

		private JunkEmailCollection blockedSenderDomain;

		private JunkEmailCollection trustedContactsEmail;

		private readonly MailboxSession session;

		private bool isEnabled;

		private bool isNew;

		private bool isContactsTrusted;

		private bool safeListsOnly;

		private bool safeListsOnlyDirty;

		private ExDateTime lastContactsUpdate;

		private Rule junkRule;

		private int currentMailboxJunkThresholdSetting = 6;

		private int maxNumberOfTrustedEntries;

		private int maxNumberOfBlockedEntries;

		private bool maxValuesInitialized;

		private ADUser currentUser;

		private static PropTag[] extraProps = new PropTag[]
		{
			PropTag.ReportTime,
			(PropTag)1627389955U,
			(PropTag)1627455491U
		};

		private static PropertyDefinition[] contactProps = new PropertyDefinition[]
		{
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email1AddrType,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email2AddrType,
			ContactSchema.Email3EmailAddress,
			ContactSchema.Email3AddrType
		};

		private static readonly string SymbolAt = "@";

		[Flags]
		internal enum JunkEmailStatus
		{
			None = 0,
			IsPresent = 1,
			IsEnabled = 2,
			IsError = 4
		}

		private enum SpecialFolderEid
		{
			IdxEidSpecialFolderOne,
			IdxEidSpecialFolderTwo,
			IdxEidSpecialFolderThree,
			IdxEidSpecialFolderFour,
			IdxEidJunkEmailFolder,
			IdxJunkEmailMoveStamp,
			MaxEidSpecialFolders
		}
	}
}
