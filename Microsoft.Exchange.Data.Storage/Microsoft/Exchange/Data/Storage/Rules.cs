using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Rules : IList<Rule>, ICollection<Rule>, IEnumerable<Rule>, IEnumerable
	{
		public Rules(Folder folder) : this(folder, true, false)
		{
		}

		public Rules(Folder folder, bool includeHidden) : this(folder, true, includeHidden)
		{
		}

		public Rules(Folder folder, bool loadRules, bool includeHidden = false)
		{
			this.folder = folder;
			this.data = new List<Rule>();
			if (loadRules)
			{
				this.ParseRules(includeHidden);
			}
		}

		public int IndexOf(Rule item)
		{
			this.CheckSaved();
			return this.data.IndexOf(item);
		}

		public void Insert(int index, Rule item)
		{
			this.CheckSaved();
			this.data.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			this.CheckSaved();
			this.data.RemoveAt(index);
		}

		public Rule this[int index]
		{
			get
			{
				this.CheckSaved();
				return this.data[index];
			}
			set
			{
				this.CheckSaved();
				this.data[index] = value;
			}
		}

		public void Add(Rule item)
		{
			this.CheckSaved();
			this.data.Add(item);
		}

		public void Clear()
		{
			this.CheckSaved();
			if (this.data.Count > 0)
			{
				this.data.Clear();
			}
		}

		public bool Contains(Rule item)
		{
			this.CheckSaved();
			return this.data.Contains(item);
		}

		public void CopyTo(Rule[] array, int arrayIndex)
		{
			this.CheckSaved();
			this.data.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get
			{
				this.CheckSaved();
				return this.data.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				this.CheckSaved();
				return false;
			}
		}

		public bool Remove(Rule item)
		{
			this.CheckSaved();
			return this.data.Remove(item);
		}

		public IEnumerator<Rule> GetEnumerator()
		{
			this.CheckSaved();
			return this.data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			this.CheckSaved();
			return this.data.GetEnumerator();
		}

		public Rule FindByRuleId(RuleId id)
		{
			this.CheckSaved();
			if (id == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Rules::Bind. {0} should not be null.", "id");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("id", 1));
			}
			int indexByRuleId = this.GetIndexByRuleId(id);
			if (indexByRuleId != -1)
			{
				return this[indexByRuleId];
			}
			throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
		}

		public void Delete(RuleId id)
		{
			this.CheckSaved();
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Rules::Delete.");
			if (id == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Rules::Delete. {0} should not be null.", "id");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("id", 1));
			}
			int indexByRuleId = this.GetIndexByRuleId(id);
			if (indexByRuleId != -1)
			{
				this.data.RemoveAt(indexByRuleId);
				return;
			}
			throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
		}

		public int GetIndexByRuleId(RuleId id)
		{
			this.CheckSaved();
			if (id == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(0L, "Rules::Bind. {0} should not be null.", "id");
				throw new ArgumentNullException(ServerStrings.ExNullParameter("id", 1));
			}
			if (id.RuleIndex < this.data.Count)
			{
				Rule rule = this.data[id.RuleIndex];
				if (rule.Id.Equals(id))
				{
					return id.RuleIndex;
				}
			}
			foreach (Rule rule2 in this)
			{
				if (rule2.Id.Equals(id))
				{
					return rule2.Id.RuleIndex;
				}
			}
			return -1;
		}

		public bool LegacyOutlookRulesCacheExists
		{
			get
			{
				this.CheckSaved();
				ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.ItemClass, "IPM.RuleOrganizer");
				SortBy[] sortColumns = new SortBy[]
				{
					new SortBy(InternalSchema.ItemClass, SortOrder.Ascending)
				};
				bool result;
				using (QueryResult queryResult = this.Folder.ItemQuery(ItemQueryType.Associated, null, sortColumns, new PropertyDefinition[]
				{
					StoreObjectSchema.EntryId,
					FolderSchema.DisplayName
				}))
				{
					result = queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter);
				}
				return result;
			}
		}

		public virtual void Save()
		{
			this.Save(false);
		}

		public virtual void Save(bool includeHidden)
		{
			this.CheckSaved();
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Rules::Save.");
			List<Rule> list = new List<Rule>();
			List<Rule> list2 = new List<Rule>();
			List<Rule> list3 = new List<Rule>();
			List<Rule> list4 = new List<Rule>();
			int num = 10;
			foreach (Rule rule in this)
			{
				if (rule.IsNotSupported)
				{
					ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "Rules::Save. Can't save NotSupported rule {0}", rule.Name);
					this.UpdateUnsupportedRuleSequence(rule, num);
				}
				else
				{
					if (rule.Sequence != num)
					{
						rule.Sequence = num;
					}
					if (rule.IsDirty)
					{
						rule.Save();
					}
				}
				num++;
				if (rule.IsNew)
				{
					list.Add(rule.ServerRule);
				}
				else if (rule.IsDirty)
				{
					list2.Add(rule.ServerRule);
				}
				else
				{
					list3.Add(rule.ServerRule);
				}
			}
			foreach (Rule rule2 in this.serverRules)
			{
				if ((Rules.IsInboxRule(rule2) || includeHidden) && !list2.Contains(rule2) && !list3.Contains(rule2))
				{
					list4.Add(rule2);
					rule2.Operation = RuleOperation.Delete;
				}
			}
			if (list.Count > 0)
			{
				this.RuleSaver(list.ToArray(), new Rules.RulesUpdaterDelegate(this.Folder.MapiFolder.AddRules));
			}
			if (list2.Count > 0)
			{
				this.RuleSaver(list2.ToArray(), new Rules.RulesUpdaterDelegate(this.Folder.MapiFolder.ModifyRules));
			}
			if (list4.Count > 0)
			{
				this.RuleSaver(list4.ToArray(), new Rules.RulesUpdaterDelegate(this.Folder.MapiFolder.DeleteRules));
			}
			this.RemoveOutlookRuleBlob();
			this.Nuke();
		}

		public void SaveBatch()
		{
			this.CheckSaved();
			ExTraceGlobals.StorageTracer.Information((long)this.GetHashCode(), "Rules::SaveBatch.");
			Rule[] array = (from rule in this
			where RuleOperation.NoOp != rule.ServerRule.Operation
			select rule.ServerRule).ToArray<Rule>();
			if (0 < array.Length)
			{
				this.RuleSaver(array, new Rules.RulesUpdaterDelegate(this.folder.MapiFolder.SaveRuleBatch));
			}
			this.RemoveOutlookRuleBlob();
			this.Nuke();
		}

		public void Nuke()
		{
			this.serverRules = null;
			this.data.Clear();
			this.saved = true;
		}

		public Folder Folder
		{
			get
			{
				return this.folder;
			}
		}

		internal Rule[] ServerRules
		{
			get
			{
				return this.serverRules;
			}
		}

		private static bool IsInboxRule(Rule serverRule)
		{
			return (!(serverRule.Provider != "MSFT:TDX Rules") || !(serverRule.Provider != "ExchangeMailboxRules14") || serverRule.Provider.StartsWith("RuleOrganizer", StringComparison.OrdinalIgnoreCase)) && string.Compare(serverRule.Name, "#NET FOLDERS#", StringComparison.CurrentCultureIgnoreCase) != 0;
		}

		private static void RunMapiCode(LocalizedString context, Action action)
		{
			try
			{
				action();
			}
			catch (MapiRetryableException exception)
			{
				throw StorageGlobals.TranslateMapiException(context, exception, null, null, string.Empty, new object[0]);
			}
			catch (MapiPermanentException exception2)
			{
				throw StorageGlobals.TranslateMapiException(context, exception2, null, null, string.Empty, new object[0]);
			}
		}

		private void ParseRules(bool includeHidden)
		{
			MapiFolder mapiFolder = this.Folder.MapiFolder;
			Rules.RunMapiCode(ServerStrings.ErrorLoadingRules, delegate
			{
				this.serverRules = mapiFolder.GetRules(new PropTag[0]);
			});
			int num = 10;
			foreach (Rule rule in this.serverRules)
			{
				if (Rules.IsInboxRule(rule) || includeHidden)
				{
					Rule rule2 = Rule.Create(this.Folder, rule);
					this.data.Add(rule2);
					rule2.Id = new RuleId(this.data.IndexOf(rule2), rule.ID);
					rule2.ClearDirty();
					if (rule2.Sequence != num)
					{
						rule2.Sequence = num;
					}
					num++;
				}
			}
		}

		private void UpdateUnsupportedRuleSequence(Rule rule, int sequence)
		{
			Rules.RunMapiCode(ServerStrings.ErrorSavingRules, delegate
			{
				if (rule.ServerRule.ExecutionSequence != sequence)
				{
					rule.ServerRule.ExecutionSequence = sequence;
					this.Folder.MapiFolder.ModifyRules(new Rule[]
					{
						rule.ServerRule
					});
				}
			});
		}

		private void RuleSaver(Rule[] mapiRules, Rules.RulesUpdaterDelegate rulesUpdater)
		{
			Rules.RunMapiCode(ServerStrings.ErrorSavingRules, delegate
			{
				StoreSession session = this.Folder.Session;
				object <>4__this = this;
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
					try
					{
						rulesUpdater(mapiRules);
					}
					catch (MapiRetryableException ex)
					{
						if (!(ex is MapiExceptionNotEnoughMemory))
						{
							if (!(ex is MapiExceptionRpcOutOfMemory))
							{
								goto IL_C2;
							}
						}
						try
						{
							foreach (Rule rule in mapiRules)
							{
								rulesUpdater(new Rule[]
								{
									rule
								});
							}
							goto IL_C4;
						}
						catch (MapiExceptionNotEnoughMemory innerException)
						{
							throw new RulesTooBigException(ServerStrings.ErrorSavingRules, innerException);
						}
						catch (MapiExceptionRpcOutOfMemory innerException2)
						{
							throw new RulesTooBigException(ServerStrings.ErrorSavingRules, innerException2);
						}
						goto IL_C2;
						IL_C4:
						goto IL_C6;
						IL_C2:
						throw;
					}
					IL_C6:;
				}
				catch (MapiPermanentException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex2, session, <>4__this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Rules::Save.", new object[0]),
						ex2
					});
				}
				catch (MapiRetryableException ex3)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.ErrorSavingRules, ex3, session, <>4__this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Rules::Save.", new object[0]),
						ex3
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
			});
		}

		private void CheckSaved()
		{
			if (this.saved)
			{
				throw new InvalidOperationException("The Rules collection cannot be used after a save.  Use a new Rules collection.");
			}
		}

		private void RemoveOutlookRuleBlob()
		{
			using (QueryResult queryResult = this.Folder.ItemQuery(ItemQueryType.Associated, null, Rules.OutlookRuleBlobSortByArray, Rules.OutlookRuleBlobPropertyDefinitionArray))
			{
				if (queryResult.SeekToCondition(SeekReference.OriginBeginning, Rules.OutlookRuleBlobFilter))
				{
					List<StoreObjectId> list = new List<StoreObjectId>();
					bool flag = true;
					do
					{
						object[][] rows = queryResult.GetRows(10000);
						foreach (object[] array2 in rows)
						{
							if (!StringComparer.OrdinalIgnoreCase.Equals((string)array2[1], "IPM.RuleOrganizer"))
							{
								flag = false;
								break;
							}
							list.Add(PropertyBag.CheckPropertyValue<VersionedId>(ItemSchema.Id, array2[0]).ObjectId);
						}
						flag = (flag && rows.Length > 0);
					}
					while (flag);
					this.Folder.DeleteObjects(DeleteItemFlags.HardDelete, list.ToArray());
				}
			}
		}

		internal const int SequenceOffset = 10;

		private Rule[] serverRules;

		private Folder folder;

		private readonly List<Rule> data;

		private bool saved;

		private static readonly ComparisonFilter OutlookRuleBlobFilter = new ComparisonFilter(ComparisonOperator.Equal, StoreObjectSchema.ItemClass, "IPM.RuleOrganizer");

		private static readonly SortBy[] OutlookRuleBlobSortByArray = new SortBy[]
		{
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
		};

		private static readonly PropertyDefinition[] OutlookRuleBlobPropertyDefinitionArray = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass
		};

		private delegate void RulesUpdaterDelegate(Rule[] mapiRules);
	}
}
