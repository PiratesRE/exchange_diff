using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol.EDiscovery;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	[Serializable]
	public class ComplianceSearch : ComplianceJob
	{
		public ComplianceSearch()
		{
			base.JobType = ComplianceJobType.ComplianceSearch;
			this.StartDate = new DateTime?(ComplianceJobConstants.MinComplianceTime);
			this.EndDate = new DateTime?(ComplianceJobConstants.MinComplianceTime);
			this.Language = CultureInfo.InvariantCulture;
			this.SearchType = ComplianceSearch.ComplianceSearchType.EstimateSearch;
			this.LogLevel = ComplianceJobLogLevel.Suppressed;
			this.successfulResults = new List<SearchResult.TargetSearchResult>();
			this.failedResults = new List<SearchResult.TargetSearchResult>();
		}

		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)this[ComplianceSearchSchema.Language];
			}
			internal set
			{
				this[ComplianceSearchSchema.Language] = value;
			}
		}

		public MultiValuedProperty<string> StatusMailRecipients
		{
			get
			{
				return (MultiValuedProperty<string>)this[ComplianceSearchSchema.StatusMailRecipients];
			}
			internal set
			{
				this[ComplianceSearchSchema.StatusMailRecipients] = value;
			}
		}

		public ComplianceJobLogLevel LogLevel
		{
			get
			{
				return (ComplianceJobLogLevel)this[ComplianceSearchSchema.LogLevel];
			}
			set
			{
				this[ComplianceSearchSchema.LogLevel] = value;
			}
		}

		public bool IncludeUnindexedItems
		{
			get
			{
				return (bool)this[ComplianceSearchSchema.IncludeUnindexedItems];
			}
			internal set
			{
				this[ComplianceSearchSchema.IncludeUnindexedItems] = value;
			}
		}

		public string KeywordQuery
		{
			get
			{
				return (string)this[ComplianceSearchSchema.KeywordQuery];
			}
			internal set
			{
				this[ComplianceSearchSchema.KeywordQuery] = value;
			}
		}

		public DateTime? StartDate
		{
			get
			{
				return (DateTime?)this[ComplianceSearchSchema.StartDate];
			}
			internal set
			{
				this[ComplianceSearchSchema.StartDate] = value;
			}
		}

		public DateTime? EndDate
		{
			get
			{
				return (DateTime?)this[ComplianceSearchSchema.EndDate];
			}
			internal set
			{
				if (value != null && value.Value.Hour == 0 && value.Value.Minute == 0 && value.Value.Second == 0)
				{
					this[ComplianceSearchSchema.EndDate] = new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, DateTime.MaxValue.Hour, DateTime.MaxValue.Minute, DateTime.MaxValue.Second);
					return;
				}
				this[ComplianceSearchSchema.EndDate] = value;
			}
		}

		public ComplianceSearch.ComplianceSearchType SearchType
		{
			get
			{
				return (ComplianceSearch.ComplianceSearchType)this[ComplianceSearchSchema.SearchType];
			}
			internal set
			{
				this[ComplianceSearchSchema.SearchType] = value;
			}
		}

		public long ResultNumber { get; private set; }

		public long ResultSize { get; private set; }

		public string SuccessResults
		{
			get
			{
				return this.GetPerBindingResultsString(this.successfulResults);
			}
		}

		public string FailedResults
		{
			get
			{
				return this.GetPerBindingResultsString(this.failedResults);
			}
		}

		public string Errors { get; set; }

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ComplianceSearch.schema;
			}
		}

		internal override byte[] JobData
		{
			get
			{
				ComplianceSearchData complianceSearchData = new ComplianceSearchData
				{
					SearchType = this.SearchType,
					KeywordQuery = this.KeywordQuery,
					StatusMailRecipients = this.StatusMailRecipients.ToArray(),
					LogLevel = this.LogLevel,
					SearchOptions = this.GenerateSearchOptions(),
					SearchConditions = this.GenerateSearchConditions()
				};
				complianceSearchData.Language = this.Language.Name;
				if (string.IsNullOrEmpty(complianceSearchData.Language))
				{
					complianceSearchData.Language = this.Language.TwoLetterISOLanguageName;
				}
				return ComplianceSerializer.Serialize<ComplianceSearchData>(ComplianceSearchData.Description, complianceSearchData);
			}
			set
			{
				ComplianceSearchData complianceSearchData = ComplianceSerializer.DeSerialize<ComplianceSearchData>(ComplianceSearchData.Description, value);
				this.SearchType = complianceSearchData.SearchType;
				this.KeywordQuery = complianceSearchData.KeywordQuery;
				this.StatusMailRecipients = complianceSearchData.StatusMailRecipients;
				this.LogLevel = complianceSearchData.LogLevel;
				if (complianceSearchData.Language.Equals(CultureInfo.InvariantCulture.TwoLetterISOLanguageName))
				{
					this.Language = CultureInfo.InvariantCulture;
				}
				else
				{
					this.Language = new CultureInfo(complianceSearchData.Language);
				}
				this.ParseSearchOptions(complianceSearchData.SearchOptions);
				this.ParseSearchCondtions(complianceSearchData.SearchConditions);
			}
		}

		internal byte[] GetExchangeWorkDefinition()
		{
			SearchWorkDefinition searchWorkDefinition = new SearchWorkDefinition();
			searchWorkDefinition.Parser = SearchWorkDefinition.QueryParser.KQL;
			searchWorkDefinition.DetailCount = 500;
			searchWorkDefinition.Query = this.ToKqlQuery();
			WorkPayload workPayload = new WorkPayload();
			workPayload.WorkDefinition = ComplianceSerializer.Serialize<SearchWorkDefinition>(SearchWorkDefinition.Description, searchWorkDefinition);
			workPayload.WorkDefinitionType = WorkDefinitionType.EDiscovery;
			return ComplianceSerializer.Serialize<WorkPayload>(WorkPayload.Description, workPayload);
		}

		internal override void UpdateJobResults()
		{
			long num = 0L;
			long num2 = 0L;
			foreach (KeyValuePair<ComplianceBindingType, ComplianceBinding> keyValuePair in base.Bindings)
			{
				WorkPayload workPayload;
				FaultDefinition faultDefinition;
				if (keyValuePair.Value.JobResults != null && keyValuePair.Value.JobResults.Length != 0 && ComplianceSerializer.TryDeserialize<WorkPayload>(WorkPayload.Description, keyValuePair.Value.JobResults, out workPayload, out faultDefinition, "UpdateJobResults", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\ObjectModel\\ComplianceSearch.cs", 325))
				{
					switch (workPayload.WorkDefinitionType)
					{
					case WorkDefinitionType.EDiscovery:
					{
						SearchResult searchResult = ComplianceSerializer.DeSerialize<SearchResult>(SearchResult.Description, workPayload.WorkDefinition);
						if (searchResult != null)
						{
							num += searchResult.TotalCount;
							num2 += searchResult.TotalSize;
							if (searchResult.Results != null && searchResult.Results.Count != 0)
							{
								foreach (SearchResult.TargetSearchResult item in searchResult.Results)
								{
									this.successfulResults.Add(item);
								}
							}
						}
						break;
					}
					case WorkDefinitionType.Fault:
						this.UpdateFailureResults(workPayload);
						break;
					}
				}
			}
			this.ResultNumber = num;
			this.ResultSize = num2;
		}

		internal string ToKqlQuery()
		{
			string keywordQuery = this.KeywordQuery;
			string additionalFilter = this.GetAdditionalFilter();
			if (string.IsNullOrEmpty(additionalFilter))
			{
				if (string.IsNullOrEmpty(keywordQuery))
				{
					return "size>=0";
				}
				return keywordQuery;
			}
			else
			{
				if (string.IsNullOrEmpty(keywordQuery))
				{
					return additionalFilter;
				}
				new StringBuilder();
				if (keywordQuery.IndexOf(" OR ", StringComparison.OrdinalIgnoreCase) != -1)
				{
					return string.Format("({0}) {1}", keywordQuery, additionalFilter);
				}
				return string.Format("{0} {1}", keywordQuery, additionalFilter);
			}
		}

		private void UpdateFailureResults(WorkPayload workPayload)
		{
			FaultDefinition results;
			FaultDefinition faultDefinition;
			if (ComplianceSerializer.TryDeserialize<FaultDefinition>(FaultDefinition.Description, workPayload.WorkDefinition, out results, out faultDefinition, "UpdateFailureResults", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\ObjectModel\\ComplianceSearch.cs", 429))
			{
				this.UpdateErrors(results);
			}
		}

		private void UpdateErrors(ResultBase results)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (results != null && results.Faults != null)
			{
				foreach (FaultRecord faultRecord in results.Faults)
				{
					if (faultRecord.Data != null)
					{
						foreach (KeyValuePair<string, string> keyValuePair in faultRecord.Data)
						{
							string key;
							if ((key = keyValuePair.Key) != null && key == "UM")
							{
								stringBuilder.Append(keyValuePair.Value);
							}
						}
					}
				}
			}
			if (stringBuilder.Length > 0)
			{
				this.Errors = stringBuilder.ToString();
			}
		}

		private string GetAdditionalFilter()
		{
			DateTime? startDate = this.StartDate;
			DateTime? endDate = this.EndDate;
			if ((startDate == null || startDate == null) && (endDate == null || endDate == null))
			{
				return null;
			}
			string text = string.Empty;
			if (startDate != null && startDate != null)
			{
				text = string.Format("received>=\"{0}\"", startDate.Value.ToString(this.Language));
			}
			if (endDate != null && endDate != null)
			{
				string text2 = string.Format("received<=\"{0}\"", endDate.Value.ToString(this.Language));
				if (!string.IsNullOrEmpty(text))
				{
					text = string.Format("({0} AND {1})", text, text2);
				}
				else
				{
					text = text2;
				}
			}
			return text;
		}

		private string GetPerBindingResultsString(List<SearchResult.TargetSearchResult> results)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("{");
			bool flag = true;
			foreach (SearchResult.TargetSearchResult targetSearchResult in results)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(",\r\n ");
				}
				stringBuilder.Append(targetSearchResult.ToString());
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		private string GenerateSearchOptions()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.IncludeUnindexedItems)
			{
				stringBuilder.Append("IncludeUnindexedItems");
				stringBuilder.Append(":");
				stringBuilder.Append("true");
			}
			return stringBuilder.ToString();
		}

		private void ParseSearchOptions(string searchOptions)
		{
			if (string.IsNullOrEmpty(searchOptions))
			{
				return;
			}
			string[] array = searchOptions.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			if (searchOptions == null || searchOptions.Length == 0)
			{
				return;
			}
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					':'
				}, StringSplitOptions.RemoveEmptyEntries);
				string a;
				if (array3 != null && array3.Length == 2 && (a = array3[0]) != null && a == "IncludeUnindexedItems")
				{
					this.IncludeUnindexedItems = array3[1].Equals("true");
				}
			}
		}

		private List<byte[]> GenerateSearchConditions()
		{
			List<byte[]> list = new List<byte[]>();
			if (this.StartDate != null)
			{
				ComplianceSearchCondition complianceSearchCondition = new ComplianceSearchCondition(ComplianceSearchCondition.ConditionName.StartDate, this.StartDate.Value.Ticks.ToString());
				list.Add(complianceSearchCondition.ToBlob());
			}
			if (this.EndDate != null)
			{
				ComplianceSearchCondition complianceSearchCondition2 = new ComplianceSearchCondition(ComplianceSearchCondition.ConditionName.EndDate, this.EndDate.Value.Ticks.ToString());
				list.Add(complianceSearchCondition2.ToBlob());
			}
			return list;
		}

		private void ParseSearchCondtions(List<byte[]> conditions)
		{
			if (conditions == null || conditions.Count == 0)
			{
				return;
			}
			foreach (byte[] blob in conditions)
			{
				ComplianceSearchCondition complianceSearchCondition = ComplianceSerializer.DeSerialize<ComplianceSearchCondition>(ComplianceSearchCondition.Description, blob);
				if (complianceSearchCondition != null)
				{
					switch (complianceSearchCondition.Name)
					{
					case ComplianceSearchCondition.ConditionName.StartDate:
					{
						DateTime value;
						if (this.TryParseDateString(complianceSearchCondition.Content, out value))
						{
							this.StartDate = new DateTime?(value);
						}
						break;
					}
					case ComplianceSearchCondition.ConditionName.EndDate:
					{
						DateTime value;
						if (this.TryParseDateString(complianceSearchCondition.Content, out value))
						{
							this.EndDate = new DateTime?(value);
						}
						break;
					}
					}
				}
			}
		}

		private bool TryParseDateString(string ticksStr, out DateTime date)
		{
			date = default(DateTime);
			long ticks;
			if (long.TryParse(ticksStr, out ticks))
			{
				date = new DateTime(ticks, DateTimeKind.Utc);
				return true;
			}
			return false;
		}

		private const string OptionKeyIncludeUnindexedItems = "IncludeUnindexedItems";

		private const string OptionValueTrue = "true";

		private static readonly ComplianceSearchSchema schema = ObjectSchema.GetInstance<ComplianceSearchSchema>();

		private List<SearchResult.TargetSearchResult> successfulResults;

		private List<SearchResult.TargetSearchResult> failedResults;

		public enum ComplianceSearchType : byte
		{
			UnknownType,
			EstimateSearch
		}
	}
}
