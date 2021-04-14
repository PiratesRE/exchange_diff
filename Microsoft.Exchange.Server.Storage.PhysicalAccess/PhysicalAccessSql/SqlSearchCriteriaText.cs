using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessSql
{
	public class SqlSearchCriteriaText : SearchCriteriaText, ISqlSearchCriteria
	{
		public SqlSearchCriteriaText(Column lhs, SearchCriteriaText.SearchTextFullness fullnessFlags, SearchCriteriaText.SearchTextFuzzyLevel fuzzynessFlags, Column rhs) : base(lhs, fullnessFlags, fuzzynessFlags, rhs)
		{
		}

		public void AppendQueryText(CultureInfo culture, SqlQueryModel model, SqlCommand command)
		{
			SearchCriteriaText.SearchTextFullness searchTextFullness = base.FullnessFlags & ~(SearchCriteriaText.SearchTextFullness.PrefixOnAnyWord | SearchCriteriaText.SearchTextFullness.PhraseMatch);
			bool flag = false;
			if (base.Lhs.IsNullable || base.Rhs.IsNullable)
			{
				command.Append("(");
				if (searchTextFullness == SearchCriteriaText.SearchTextFullness.FullString)
				{
					if (base.Lhs.IsNullable && base.Rhs.IsNullable)
					{
						command.Append("(");
						((ISqlColumn)base.Lhs).AppendQueryText(model, command);
						command.Append(" IS NULL AND ");
						((ISqlColumn)base.Rhs).AppendQueryText(model, command);
						command.Append(" IS NULL) OR (");
						((ISqlColumn)base.Lhs).AppendQueryText(model, command);
						command.Append(" IS NOT NULL AND ");
						((ISqlColumn)base.Rhs).AppendQueryText(model, command);
						command.Append(" IS NOT NULL AND ");
						flag = true;
					}
					else if (base.Rhs.IsNullable)
					{
						((ISqlColumn)base.Rhs).AppendQueryText(model, command);
						command.Append(" IS NOT NULL AND ");
					}
					else
					{
						((ISqlColumn)base.Lhs).AppendQueryText(model, command);
						command.Append(" IS NOT NULL AND ");
					}
				}
				else if (base.Lhs.IsNullable && base.Rhs.IsNullable)
				{
					((ISqlColumn)base.Rhs).AppendQueryText(model, command);
					command.Append(" IS NULL OR (");
					((ISqlColumn)base.Lhs).AppendQueryText(model, command);
					command.Append(" IS NOT NULL AND ");
					flag = true;
				}
				else if (base.Rhs.IsNullable)
				{
					((ISqlColumn)base.Rhs).AppendQueryText(model, command);
					command.Append(" IS NULL OR ");
				}
				else
				{
					((ISqlColumn)base.Lhs).AppendQueryText(model, command);
					command.Append(" IS NOT NULL AND ");
				}
			}
			if (searchTextFullness == SearchCriteriaText.SearchTextFullness.FullString)
			{
				if (base.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0)
				{
					command.Append("UPPER(");
				}
				((ISqlColumn)base.Lhs).AppendQueryText(model, command);
				if (base.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0)
				{
					command.Append(")");
				}
				command.Append(" = ");
				if (base.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0)
				{
					command.Append("UPPER(");
				}
				((ISqlColumn)base.Rhs).AppendQueryText(model, command);
				SqlCollationHelper.AppendCollation(base.Lhs, culture, command);
				if (base.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0)
				{
					command.Append(")");
				}
			}
			else
			{
				string parameterValue = null;
				if (searchTextFullness == SearchCriteriaText.SearchTextFullness.Prefix)
				{
					parameterValue = (string)((ConstantColumn)base.Rhs).Value + "%";
				}
				else if ((ushort)(searchTextFullness & SearchCriteriaText.SearchTextFullness.SubString) == 1)
				{
					parameterValue = "%" + (string)((ConstantColumn)base.Rhs).Value + "%";
				}
				if (base.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0)
				{
					command.Append("UPPER(");
				}
				((ISqlColumn)base.Lhs).AppendQueryText(model, command);
				if (base.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0)
				{
					command.Append(")");
				}
				command.Append(" LIKE ");
				if (base.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0)
				{
					command.Append("UPPER(");
				}
				command.AppendParameter(parameterValue);
				SqlCollationHelper.AppendCollation(base.Lhs, culture, command);
				if (base.FuzzynessFlags != (SearchCriteriaText.SearchTextFuzzyLevel)0)
				{
					command.Append(")");
				}
			}
			if (base.Lhs.IsNullable || base.Rhs.IsNullable)
			{
				if (flag)
				{
					command.Append(")");
				}
				command.Append(")");
			}
		}
	}
}
