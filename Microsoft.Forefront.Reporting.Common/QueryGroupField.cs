using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Forefront.Reporting.Common
{
	internal class QueryGroupField : QueryField
	{
		internal QueryGroupField(QueryGroupField parent, int startPos, int endPos) : base(parent, startPos, endPos)
		{
			this.FromDate = parent.FromDate;
			this.ToDate = parent.ToDate;
			this.MsgStatus = parent.MsgStatus;
			string grouperName = this.GetGrouperName(startPos, endPos);
			try
			{
				this.GrouperName = (QueryGroupName)Enum.Parse(typeof(QueryGroupName), grouperName);
			}
			catch (FormatException innerException)
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidGrouper, grouperName, startPos, innerException);
			}
			if (this.GrouperName == QueryGroupName.AND && parent.IsFirstAndGroup == null)
			{
				this.IsFirstAndGroup = new bool?(true);
			}
		}

		internal QueryGroupField(string queryString, QueryCompiler compiler) : base(null, 0, queryString.Length)
		{
			base.QueryString = queryString;
			string grouperName = this.GetGrouperName(0, queryString.Length);
			try
			{
				this.GrouperName = (QueryGroupName)Enum.Parse(typeof(QueryGroupName), grouperName);
			}
			catch (FormatException innerException)
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidGrouper, grouperName, 0, innerException);
			}
			base.Compiler = compiler;
			if (this.GrouperName == QueryGroupName.AND)
			{
				this.IsFirstAndGroup = new bool?(true);
			}
		}

		internal QueryGroupName GrouperName { get; set; }

		internal DateTime? FromDate { get; set; }

		internal DateTime? ToDate { get; set; }

		internal StatusFlags? MsgStatus { get; set; }

		internal bool NeedToFillInMsgStatus { get; set; }

		internal bool? IsFirstAndGroup { get; set; }

		internal override string Compile()
		{
			List<QueryPropertyField> list = new List<QueryPropertyField>();
			List<Tuple<int, int>> list2 = new List<Tuple<int, int>>();
			foreach (Tuple<int, int> tuple2 in this.GetInnerFields(base.StartPosition, base.EndPosition))
			{
				if (this.GetGrouperName(tuple2.Item1, tuple2.Item2) == null)
				{
					list.Add(new QueryPropertyField(this, tuple2.Item1, tuple2.Item2));
				}
				else
				{
					list2.Add(tuple2);
				}
			}
			if (list.Count > 100)
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.ToManyPropertiesInGroup, this.GrouperName.ToString(), base.StartPosition);
			}
			List<string> first = (from field in list
			select field.Compile() into str
			where !string.IsNullOrWhiteSpace(str)
			select str).ToList<string>();
			List<QueryGroupField> list3 = (from tuple in list2
			select new QueryGroupField(this, tuple.Item1, tuple.Item2)).ToList<QueryGroupField>();
			if (list3.Count > 100)
			{
				throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.ToManyGroups, this.GrouperName.ToString(), base.StartPosition);
			}
			List<string> second = (from field in list3
			select field.Compile() into str
			where !string.IsNullOrWhiteSpace(str)
			select str).ToList<string>();
			if (this.GrouperName == QueryGroupName.AND)
			{
				base.HasOptionalCriterion = list3.Concat(list).Any((QueryField field) => field.HasOptionalCriterion);
			}
			else
			{
				base.HasOptionalCriterion = list3.Concat(list).All((QueryField field) => field.HasOptionalCriterion);
			}
			if (this.IsFirstAndGroup != null && this.IsFirstAndGroup.Value)
			{
				if (this.FromDate == null)
				{
					throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.MissingRequiredProperty, QueryProperty.FromDate.ToString(), base.StartPosition);
				}
				if (this.ToDate == null)
				{
					throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.MissingRequiredProperty, QueryProperty.ToDate.ToString(), base.StartPosition);
				}
			}
			string text = string.Join(QueryGroupField.supportedGroupers[this.GrouperName], first.Concat(second));
			if (this.NeedToFillInMsgStatus)
			{
				if (this.MsgStatus != null)
				{
					if (text.Contains("%MsgStatus%"))
					{
						text = text.Replace("%MsgStatus%", this.GenMsgStatusCode(this.MsgStatus.Value));
					}
					else
					{
						text = text + QueryGroupField.supportedGroupers[this.GrouperName] + string.Format("OnDemandQueryUtil.MsgStatusMatch(msgStatus {0})", this.GenMsgStatusCode(this.MsgStatus.Value));
					}
				}
				else
				{
					text = text.Replace("%MsgStatus%", string.Empty);
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return string.Empty;
			}
			return string.Format("({0})", text);
		}

		private string GenMsgStatusCode(StatusFlags msgStatus)
		{
			if (msgStatus <= StatusFlags.Send)
			{
				if (msgStatus == StatusFlags.Expand)
				{
					return ", StatusFlags.Expand";
				}
				if (msgStatus == StatusFlags.Send)
				{
					return ", StatusFlags.Send";
				}
			}
			else
			{
				if (msgStatus == StatusFlags.Deliver)
				{
					return ", StatusFlags.Send | StatusFlags.Deliver";
				}
				if (msgStatus == StatusFlags.Fail)
				{
					return ", StatusFlags.Fail";
				}
			}
			throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidValue, "MsgStatus", base.StartPosition);
		}

		private string GetGrouperName(int fieldStart, int fieldEnd)
		{
			int num = base.QueryString.IndexOf('(', fieldStart, fieldEnd - fieldStart);
			if (num > fieldStart)
			{
				return base.QueryString.Substring(fieldStart, num - fieldStart).Trim();
			}
			return null;
		}

		private IEnumerable<Tuple<int, int>> GetInnerFields(int fieldStart, int fieldEnd)
		{
			int openParentheses = 0;
			bool inQuote = false;
			int fromPos = -1;
			for (int pos = fieldStart; pos < fieldEnd; pos++)
			{
				char c = base.QueryString[pos];
				if (c != '"')
				{
					switch (c)
					{
					case '(':
						if (!inQuote)
						{
							openParentheses++;
							if (openParentheses == 1)
							{
								fromPos = pos + 1;
							}
						}
						break;
					case ')':
						if (!inQuote)
						{
							openParentheses--;
							if (openParentheses < 0)
							{
								throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnpairedParenthese, string.Empty, pos);
							}
							if (openParentheses == 0)
							{
								if (pos != fieldEnd - 1)
								{
									throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.UnpairedParenthese, string.Empty, pos);
								}
								if (fromPos < pos)
								{
									yield return Tuple.Create<int, int>(fromPos, pos);
									goto IL_20B;
								}
								throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidProperty, string.Empty, pos);
							}
						}
						break;
					case ',':
						if (!inQuote && openParentheses == 1)
						{
							if (fromPos >= pos)
							{
								throw new InvalidQueryException(InvalidQueryException.InvalidQueryErrorCode.InvalidProperty, string.Empty, pos);
							}
							yield return Tuple.Create<int, int>(fromPos, pos);
							fromPos = pos + 1;
						}
						break;
					}
				}
				else
				{
					inQuote = !inQuote;
				}
			}
			IL_20B:
			yield break;
		}

		private const int NumOfPropertiesInGroupLimit = 100;

		private static Dictionary<QueryGroupName, string> supportedGroupers = new Dictionary<QueryGroupName, string>
		{
			{
				QueryGroupName.AND,
				" && "
			},
			{
				QueryGroupName.OR,
				" || "
			}
		};
	}
}
