using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AnchorService
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AnchorLogContext
	{
		private AnchorLogContext()
		{
			this.summarizables = new SortedDictionary<string, ISummarizable>();
		}

		public static AnchorLogContext Current
		{
			get
			{
				AnchorLogContext result;
				if ((result = AnchorLogContext.context) == null)
				{
					result = (AnchorLogContext.context = new AnchorLogContext());
				}
				return result;
			}
		}

		public string Source
		{
			get
			{
				if (string.IsNullOrEmpty(this.source))
				{
					return "Default";
				}
				return this.source;
			}
			set
			{
				this.source = value;
				this.isDirty = true;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
			set
			{
				this.organizationId = value;
				this.summarizables.Clear();
				this.isDirty = true;
			}
		}

		public static void Clear()
		{
			AnchorLogContext.context = null;
		}

		public void SetSummarizable(ISummarizable summarizable)
		{
			AnchorUtil.ThrowOnNullArgument(summarizable, "summarizable");
			this.summarizables[summarizable.SummaryName] = summarizable;
			this.isDirty = true;
		}

		public void ClearSummarizable(ISummarizable summarizable)
		{
			AnchorUtil.ThrowOnNullArgument(summarizable, "summarizable");
			this.summarizables.Remove(summarizable.SummaryName);
			this.isDirty = true;
		}

		public override string ToString()
		{
			if (!this.isDirty)
			{
				return this.cachedSummary;
			}
			this.isDirty = false;
			StringBuilder stringBuilder = new StringBuilder();
			if (this.OrganizationId != null)
			{
				stringBuilder.Append(this.OrganizationId.ToString());
				stringBuilder.Append(";");
			}
			if (this.summarizables.Count > 0)
			{
				stringBuilder.Append(string.Join(";", this.SummarizablesToString()));
			}
			this.cachedSummary = stringBuilder.ToString();
			return this.cachedSummary;
		}

		public override int GetHashCode()
		{
			int num = 0;
			foreach (ISummarizable summarizable in this.summarizables.Values)
			{
				num += summarizable.GetHashCode();
			}
			return num;
		}

		private IEnumerable<string> SummarizablesToString()
		{
			foreach (string key in this.summarizables.Keys)
			{
				ISummarizable summarizable = this.summarizables[key];
				yield return summarizable.SummaryName + '=' + string.Join(",", summarizable.SummaryTokens);
			}
			yield break;
		}

		internal const string SeparatorLevelTwo = ",";

		internal const string SeparatorLevelOne = ";";

		private const string DefaultSource = "Default";

		[ThreadStatic]
		private static AnchorLogContext context;

		private string source;

		private OrganizationId organizationId;

		private SortedDictionary<string, ISummarizable> summarizables;

		private string cachedSummary = string.Empty;

		private bool isDirty = true;
	}
}
