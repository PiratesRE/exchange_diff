using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.Clutter
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ClutterOverrideManager : IClutterOverrideManager, ICollection<SmtpAddress>, IEnumerable<SmtpAddress>, IEnumerable, IDisposable
	{
		internal ClutterOverrideManager(StoreSession session)
		{
			this.session = session;
			this.Load();
		}

		public int Count
		{
			get
			{
				return this.neverClutterSenders.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(SmtpAddress smtpAddress)
		{
			ClutterUtilities.ValidateSmtpAddress(smtpAddress);
			int num = this.neverClutterSenders.FindIndex((SmtpAddress sender) => sender.Equals(smtpAddress));
			if (num >= 0)
			{
				this.neverClutterSenders.RemoveAt(num);
			}
			this.neverClutterSenders.Add(smtpAddress);
			this.isDirty = true;
			if (this.neverClutterSenders.Count > 1024)
			{
				this.neverClutterSenders.RemoveAt(0);
			}
		}

		public bool Remove(SmtpAddress smtpAddress)
		{
			ClutterUtilities.ValidateSmtpAddress(smtpAddress);
			int num = this.neverClutterSenders.FindIndex((SmtpAddress sender) => sender.Equals(smtpAddress));
			if (num >= 0)
			{
				this.neverClutterSenders.RemoveAt(num);
				this.isDirty = true;
				return true;
			}
			return false;
		}

		public bool Contains(SmtpAddress smtpAddress)
		{
			ClutterUtilities.ValidateSmtpAddress(smtpAddress);
			return this.neverClutterSenders.Contains(smtpAddress);
		}

		public void Clear()
		{
			if (this.neverClutterSenders.Any<SmtpAddress>())
			{
				this.neverClutterSenders.Clear();
				this.isDirty = true;
			}
		}

		public void CopyTo(SmtpAddress[] array, int arrayIndex)
		{
			this.neverClutterSenders.CopyTo(array, arrayIndex);
		}

		public IEnumerator<SmtpAddress> GetEnumerator()
		{
			return this.neverClutterSenders.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Dispose()
		{
			if (this.neverClutterSenders != null)
			{
				this.neverClutterSenders.Clear();
				this.neverClutterSenders = null;
			}
			this.neverClutterRule = null;
		}

		public void Save()
		{
			if (this.isDirty)
			{
				this.neverClutterRule = ClutterOverrideManager.PrepareRule(this.neverClutterRule, this.neverClutterSenders, this.markNeverClutterTag);
				this.neverClutterRule = ClutterOverrideManager.SaveNeverClutterRule(this, this.session, this.neverClutterRule);
				this.isDirty = false;
			}
		}

		internal static Rule PrepareRule(Rule rule, IEnumerable<SmtpAddress> neverClutterSenders, PropTag neverClutterTag)
		{
			if (rule == null)
			{
				rule = new Rule();
				if (neverClutterSenders.Any<SmtpAddress>())
				{
					rule.Operation = RuleOperation.Create;
				}
				else
				{
					rule.Operation = RuleOperation.NoOp;
				}
			}
			else if (neverClutterSenders.Any<SmtpAddress>())
			{
				rule.Operation = RuleOperation.Update;
			}
			else
			{
				rule.Operation = RuleOperation.Delete;
			}
			rule.ExecutionSequence = 0;
			rule.Level = 0;
			rule.StateFlags = RuleStateFlags.Enabled;
			rule.UserFlags = 0U;
			rule.ExtraProperties = null;
			rule.IsExtended = true;
			rule.Name = "Never Clutter Rule";
			rule.Provider = "NeverClutterOverrideRule";
			Rule.ProviderData providerData;
			providerData.Version = 1U;
			providerData.RuleSearchKey = 0U;
			providerData.TimeStamp = ExDateTime.UtcNow.ToFileTimeUtc();
			rule.ProviderData = providerData.ToByteArray();
			rule.Actions = ClutterOverrideManager.BuildRuleActions(neverClutterTag);
			rule.Condition = ClutterOverrideManager.BuildRuleCondition(neverClutterSenders);
			return rule;
		}

		internal static RuleAction[] BuildRuleActions(PropTag neverClutterTag)
		{
			return new RuleAction[]
			{
				new RuleAction.Tag(new PropValue(neverClutterTag, true))
			};
		}

		internal static Restriction BuildRuleCondition(IEnumerable<SmtpAddress> neverClutterSenders)
		{
			return Restriction.Or((from address in neverClutterSenders
			select ClutterOverrideManager.BuildSenderCondition(address)).ToArray<Restriction>());
		}

		internal static Restriction BuildSenderCondition(SmtpAddress smtpAddress)
		{
			return Restriction.Content(PropTag.SenderSmtpAddress, smtpAddress.ToString(), ContentFlags.IgnoreCase);
		}

		internal static Rule SaveNeverClutterRule(object mapiThis, StoreSession session, Rule rule)
		{
			Rule result;
			using (Folder folder = Folder.Bind(session, session.GetDefaultFolderId(DefaultFolderType.Inbox)))
			{
				bool flag = false;
				try
				{
					if (session != null)
					{
						session.BeginMapiCall();
						session.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					folder.MapiFolder.SaveRuleBatch(new Rule[]
					{
						rule
					});
					if (rule.Operation == RuleOperation.Create)
					{
						Rule[] rules = folder.MapiFolder.GetRules(new PropTag[0]);
						foreach (Rule rule2 in rules)
						{
							if (rule2.IsExtended && rule2.Name == "Never Clutter Rule")
							{
								return rule2;
							}
						}
					}
					result = rule;
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex, session, mapiThis, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("ClutterOverrideManager::SaveNeverClutterRule", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex2, session, mapiThis, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("ClutterOverrideManager::SaveNeverClutterRule", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (session != null)
						{
							session.EndMapiCall();
							if (flag)
							{
								session.EndServerHealthCall();
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
			return result;
		}

		internal static Rule LoadNeverClutterRule(object mapiThis, StoreSession session)
		{
			Rule result;
			using (Folder folder = Folder.Bind(session, session.GetDefaultFolderId(DefaultFolderType.Inbox)))
			{
				try
				{
					Rule[] array = null;
					bool flag = false;
					try
					{
						if (session != null)
						{
							session.BeginMapiCall();
							session.BeginServerHealthCall();
							flag = true;
						}
						if (StorageGlobals.MapiTestHookBeforeCall != null)
						{
							StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
						}
						array = folder.MapiFolder.GetRules(new PropTag[0]);
					}
					catch (MapiPermanentException ex)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex, session, mapiThis, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("ClutterOverrideManager::LoadNeverClutterRule", new object[0]),
							ex
						});
					}
					catch (MapiRetryableException ex2)
					{
						throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex2, session, mapiThis, "{0}. MapiException = {1}.", new object[]
						{
							string.Format("ClutterOverrideManager::LoadNeverClutterRule", new object[0]),
							ex2
						});
					}
					finally
					{
						try
						{
							if (session != null)
							{
								session.EndMapiCall();
								if (flag)
								{
									session.EndServerHealthCall();
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
						if (rule.IsExtended && rule.Name == "Never Clutter Rule")
						{
							return rule;
						}
					}
				}
				catch (StoragePermanentException)
				{
				}
				result = null;
			}
			return result;
		}

		internal static List<SmtpAddress> LoadNeverClutterList(Rule rule)
		{
			List<SmtpAddress> list = new List<SmtpAddress>();
			if (rule != null)
			{
				Restriction.OrRestriction orRestriction = rule.Condition as Restriction.OrRestriction;
				if (orRestriction != null)
				{
					foreach (Restriction restriction in orRestriction.Restrictions)
					{
						Restriction.ContentRestriction contentRestriction = restriction as Restriction.ContentRestriction;
						if (contentRestriction != null && contentRestriction.PropTag == PropTag.SenderSmtpAddress)
						{
							string @string = contentRestriction.PropValue.GetString();
							SmtpAddress item = new SmtpAddress(@string);
							if (item.IsValidAddress)
							{
								list.Add(item);
							}
						}
					}
				}
			}
			return list;
		}

		internal static PropTag GetNeverClutterTag(object mapiThis, StoreSession session)
		{
			PropertyDefinition inferenceNeverClutterOverrideApplied = ItemSchema.InferenceNeverClutterOverrideApplied;
			NamedProp namedProp = new NamedProp(WellKnownPropertySet.Inference, inferenceNeverClutterOverrideApplied.Name);
			NamedProp namedProp2 = WellKnownNamedProperties.Find(namedProp);
			NamedProp namedProp3 = namedProp2 ?? namedProp;
			NamedProp[] np = new NamedProp[]
			{
				namedProp3
			};
			PropTag[] array = null;
			bool flag = false;
			try
			{
				if (session != null)
				{
					session.BeginMapiCall();
					session.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				array = session.Mailbox.MapiStore.GetIDsFromNames(true, np);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex, session, mapiThis, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("ClutterOverrideManager::GetNeverClutterTag", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiRulesError, ex2, session, mapiThis, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("ClutterOverrideManager::GetNeverClutterTag", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (session != null)
					{
						session.EndMapiCall();
						if (flag)
						{
							session.EndServerHealthCall();
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
			return (array.Length > 0) ? (array[0] | (PropTag)11U) : PropTag.Null;
		}

		private void Load()
		{
			this.neverClutterRule = ClutterOverrideManager.LoadNeverClutterRule(this, this.session);
			this.neverClutterSenders = ClutterOverrideManager.LoadNeverClutterList(this.neverClutterRule);
			this.markNeverClutterTag = ClutterOverrideManager.GetNeverClutterTag(this, this.session);
			if (this.neverClutterRule != null && (this.neverClutterRule.StateFlags.HasFlag(RuleStateFlags.Error) || !this.neverClutterRule.StateFlags.HasFlag(RuleStateFlags.Enabled)))
			{
				this.isDirty = true;
				return;
			}
			this.isDirty = false;
		}

		internal const string NeverClutterName = "Never Clutter Rule";

		internal const string NeverClutterRuleProvider = "NeverClutterOverrideRule";

		internal const int MaxSize = 1024;

		private readonly StoreSession session;

		private PropTag markNeverClutterTag;

		private Rule neverClutterRule;

		private List<SmtpAddress> neverClutterSenders;

		private bool isDirty;
	}
}
