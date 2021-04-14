using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RulesMapiModifyTable : MapiModifyTable
	{
		internal RulesMapiModifyTable(MapiFolder folder)
		{
			this.folder = folder;
		}

		public override MapiTable GetTable(GetTableFlags flags)
		{
			base.CheckDisposed();
			return this.folder.GetRulesTable();
		}

		public override void ModifyTable(ModifyTableFlags flags, ICollection<RowEntry> rowList)
		{
			base.CheckDisposed();
			List<Rule> list = new List<Rule>();
			List<Rule> list2 = new List<Rule>();
			List<Rule> list3 = new List<Rule>();
			Rule[] array = null;
			foreach (RowEntry rowEntry in rowList)
			{
				switch (rowEntry.RowFlags)
				{
				case RowEntry.RowOp.Add:
				{
					Rule rule3 = Rule.CreateRuleFromProperties(this.folder, null, rowEntry.Values);
					if (rule3.Actions == null)
					{
						throw MapiExceptionHelper.InvalidParameterException("Incomplete rule parameters on RowOp.Add");
					}
					list.Add(rule3);
					continue;
				}
				case RowEntry.RowOp.Modify:
				{
					Rule existingRule = null;
					if (Rule.IsPublicFolderRule(rowEntry.Values))
					{
						try
						{
							long ruleID = rowEntry.Values.First((PropValue propValue) => propValue.PropTag == PropTag.RuleID).GetLong();
							array = (array ?? this.folder.GetRules(new PropTag[0]));
							existingRule = array.First((Rule rule) => rule.ID == ruleID);
						}
						catch (InvalidOperationException)
						{
							throw MapiExceptionHelper.InvalidParameterException("Try to modify nonexisting rule on RowOp.Modify");
						}
					}
					Rule rule3 = Rule.CreateRuleFromProperties(this.folder, existingRule, rowEntry.Values);
					list2.Add(rule3);
					continue;
				}
				case RowEntry.RowOp.Remove:
				{
					Rule rule3 = Rule.CreateRuleFromProperties(this.folder, null, rowEntry.Values);
					if (rule3.ID == 0L)
					{
						throw MapiExceptionHelper.InvalidParameterException("Incomplete rule parameters on RowOp.Remove");
					}
					list3.Add(rule3);
					continue;
				}
				}
				throw MapiExceptionHelper.InvalidParameterException("Invalid RowFlag value.");
			}
			if ((flags & ModifyTableFlags.RowListReplace) == ModifyTableFlags.RowListReplace)
			{
				if (list2.Count != 0 || list3.Count != 0)
				{
					throw MapiExceptionHelper.InvalidParameterException("Cannot modify and/or remove rules when RowListReplace specified");
				}
				array = (array ?? this.folder.GetRules(new PropTag[0]));
				foreach (Rule rule2 in array)
				{
					if (!rule2.IsExtended)
					{
						list3.Add(rule2);
					}
				}
			}
			this.folder.DeleteRules(list3.ToArray());
			this.folder.ModifyRules(list2.ToArray());
			this.folder.AddRules(list.ToArray());
		}

		private readonly MapiFolder folder;
	}
}
