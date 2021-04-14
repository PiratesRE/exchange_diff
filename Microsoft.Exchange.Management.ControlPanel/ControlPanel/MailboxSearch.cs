using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(MailboxSearch))]
	[DataContract]
	public class MailboxSearch : MailboxSearchRow
	{
		public MailboxSearch(MailboxSearchObject searchObject) : base(searchObject)
		{
		}

		[DataMember]
		public string Caption
		{
			get
			{
				return base.MailboxSearch.Name.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SearchAllMailboxes
		{
			get
			{
				return this.SearchAllMailboxesCalculated.ToJsonString(null);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public bool SearchAllMailboxesCalculated
		{
			get
			{
				return base.MailboxSearch.SourceMailboxes == null || base.MailboxSearch.SourceMailboxes.Count == 0;
			}
		}

		[DataMember]
		public string SearchAllDates
		{
			get
			{
				return (base.MailboxSearch.StartDate == null && base.MailboxSearch.EndDate == null).ToJsonString(null);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IncludeUnsearchableItems
		{
			get
			{
				return base.MailboxSearch.IncludeUnsearchableItems;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SearchStartDate
		{
			get
			{
				return base.MailboxSearch.StartDate.ToUserDateTimeGeneralFormatString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SearchEndDate
		{
			get
			{
				return base.MailboxSearch.EndDate.ToUserDateTimeGeneralFormatString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool SendMeEmailOnComplete
		{
			get
			{
				return base.MailboxSearch.StatusMailRecipients.Contains(RbacPrincipal.Current.ExecutingUserId);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool EnableFullLogging
		{
			get
			{
				return base.MailboxSearch.LogLevel == LoggingLevel.Full;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ResultNumber
		{
			get
			{
				KeywordHit keywordHit = base.MailboxSearch.AllKeywordHits.Find((KeywordHit hit) => hit.Phrase == "652beee2-75f7-4ca0-8a02-0698a3919cb9");
				long num = base.MailboxSearch.EstimateOnly ? base.MailboxSearch.ResultNumberEstimate : base.MailboxSearch.ResultNumber;
				if (!this.IncludeUnsearchableItems || keywordHit == null)
				{
					return num.ToString();
				}
				return string.Format(Strings.MailboxSeachCountIncludeUnsearchable, num, keywordHit.Count);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string EstimatedItems
		{
			get
			{
				if (base.MailboxSearch.SearchStatistics == null || base.MailboxSearch.SearchStatistics.Count == 0)
				{
					return "0";
				}
				return base.MailboxSearch.SearchStatistics[0].EstimatedItems.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string UnsearchableItemsAdded
		{
			get
			{
				if (base.MailboxSearch.SearchStatistics == null || base.MailboxSearch.SearchStatistics.Count == 0)
				{
					return "0";
				}
				return string.Format("+{0}", base.MailboxSearch.SearchStatistics[0].UnsearchableItemsAdded);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DuplicatesRemoved
		{
			get
			{
				if (base.MailboxSearch.SearchStatistics == null || base.MailboxSearch.SearchStatistics.Count == 0)
				{
					return "0";
				}
				return string.Format("-{0}", base.MailboxSearch.SearchStatistics[0].TotalDuplicateItems);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SkippedErrorItems
		{
			get
			{
				if (base.MailboxSearch.SearchStatistics == null || base.MailboxSearch.SearchStatistics.Count == 0)
				{
					return "0";
				}
				return string.Format("-{0}", base.MailboxSearch.SearchStatistics[0].SkippedErrorItems);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string TotalCopiedItems
		{
			get
			{
				if (base.MailboxSearch.SearchStatistics == null || base.MailboxSearch.SearchStatistics.Count == 0)
				{
					return "0";
				}
				return string.Format("={0}", base.MailboxSearch.SearchStatistics[0].TotalItemsCopied);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ResultsLink
		{
			get
			{
				return base.MailboxSearch.ResultsLink;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string TargetMailboxDisplayName
		{
			get
			{
				if (base.MailboxSearch.TargetMailbox == null)
				{
					return Strings.TargetMailboxRemoved;
				}
				RecipientObjectResolverRow recipientObjectResolverRow = RecipientObjectResolver.Instance.ResolveObjects(new ADObjectId[]
				{
					base.MailboxSearch.TargetMailbox
				}).FirstOrDefault<RecipientObjectResolverRow>();
				if (recipientObjectResolverRow == null)
				{
					return string.Empty;
				}
				return recipientObjectResolverRow.DisplayName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string TargetMailboxSmtp
		{
			get
			{
				if (base.MailboxSearch.TargetMailbox == null)
				{
					return Strings.TargetMailboxRemoved;
				}
				RecipientObjectResolverRow recipientObjectResolverRow = RecipientObjectResolver.Instance.ResolveObjects(new ADObjectId[]
				{
					base.MailboxSearch.TargetMailbox
				}).FirstOrDefault<RecipientObjectResolverRow>();
				if (recipientObjectResolverRow == null)
				{
					return this.TargetMailboxDisplayName;
				}
				return recipientObjectResolverRow.PrimarySmtpAddress;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LastRunByDisplayName
		{
			get
			{
				return base.MailboxSearch.LastRunBy ?? string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int PercentComplete
		{
			get
			{
				return base.MailboxSearch.PercentComplete;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> SourceMailboxes
		{
			get
			{
				if (this.SearchAllMailboxesCalculated)
				{
					return null;
				}
				return RecipientObjectResolver.Instance.ResolveObjects(base.MailboxSearch.SourceMailboxes.ToArray());
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public Identity TargetMailbox
		{
			get
			{
				if (base.MailboxSearch.TargetMailbox == null)
				{
					return null;
				}
				RecipientObjectResolverRow recipientObjectResolverRow = RecipientObjectResolver.Instance.ResolveObjects(new ADObjectId[]
				{
					base.MailboxSearch.TargetMailbox
				}).FirstOrDefault<RecipientObjectResolverRow>();
				if (recipientObjectResolverRow == null)
				{
					return base.MailboxSearch.TargetMailbox.ToIdentity();
				}
				return recipientObjectResolverRow.Identity;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SearchQuery
		{
			get
			{
				return base.MailboxSearch.SearchQuery;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Senders
		{
			get
			{
				if (base.MailboxSearch.Senders.Count == 0)
				{
					return string.Empty;
				}
				return string.Join(", ", base.MailboxSearch.Senders.ToArray<string>());
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Recipients
		{
			get
			{
				if (base.MailboxSearch.Recipients.Count == 0)
				{
					return string.Empty;
				}
				return string.Join(", ", base.MailboxSearch.Recipients.ToArray<string>());
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LogLevel
		{
			get
			{
				return base.MailboxSearch.LogLevel.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string EstimateOnly
		{
			get
			{
				return base.MailboxSearch.EstimateOnly.ToJsonString(null);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool ExcludeDuplicateMessages
		{
			get
			{
				return base.MailboxSearch.ExcludeDuplicateMessages;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Errors
		{
			get
			{
				if (base.MailboxSearch.Errors != null && base.MailboxSearch.Errors.Count != 0)
				{
					return base.MailboxSearch.Errors.ToStringArray().StringArrayJoin(", ");
				}
				return Strings.NoErrors;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ErrorsOneLine
		{
			get
			{
				this.SetErrorArrays();
				int num = this.errorOneLinerStrings.Length;
				StringBuilder stringBuilder = new StringBuilder(string.Empty);
				this.totalErrorsFound = 0;
				if (this.errorMailboxes != null && this.errorMailboxes.Length >= num)
				{
					for (int i = 1; i < num; i++)
					{
						if (this.errorMailboxes[i] != null && this.errorMailboxes[i].Count > 0)
						{
							this.totalErrorsFound += this.errorMailboxes[i].Count;
							stringBuilder.AppendLine(this.errorOneLinerStrings[i]);
							if (this.errorMailboxes[i].Count > 1)
							{
								stringBuilder.AppendLine(string.Format("{0} {1}", this.errorMailboxes[i].Count, Strings.EDiscoveryInstances));
							}
							else
							{
								stringBuilder.AppendLine(string.Format("1 {0}", Strings.EDiscoveryInstance));
							}
						}
					}
				}
				if (this.errorUndefined != null && this.errorUndefined.Count > 0)
				{
					stringBuilder.AppendLine(this.errorOneLinerStrings[0]);
					if (this.errorUndefined.Count > 1)
					{
						stringBuilder.AppendLine(string.Format("{0} {1}", this.errorUndefined.Count, Strings.EDiscoveryInstances));
					}
					else
					{
						stringBuilder.AppendLine(string.Format("1 {0}", Strings.EDiscoveryInstance));
					}
				}
				if (this.totalErrorsFound + this.errorUndefined.Count <= 0)
				{
					return Strings.NoErrors;
				}
				return stringBuilder.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ErrorsUndefinedSummary
		{
			get
			{
				this.SetErrorArrays();
				StringBuilder stringBuilder = new StringBuilder(string.Empty);
				if (this.errorUndefined != null && this.errorUndefined.Count > 0)
				{
					stringBuilder.AppendLine(this.errorOneLinerDetailStrings[0]);
					if (this.errorUndefined.Count > 1)
					{
						stringBuilder.AppendLine(string.Format("{0} {1}", this.errorUndefined.Count, Strings.EDiscoveryInstances));
					}
					else
					{
						stringBuilder.AppendLine(string.Format("1 {0}", Strings.EDiscoveryInstance));
					}
					stringBuilder.Append("\n");
					return stringBuilder.ToString();
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ErrorsOneLineDetails
		{
			get
			{
				this.SetErrorArrays();
				int num = this.errorOneLinerDetailStrings.Length;
				StringBuilder stringBuilder = new StringBuilder(string.Empty);
				this.totalErrorsFound = 0;
				if (this.errorMailboxes == null || this.errorItems == null || this.errorDocumentIds == null || this.errorServers == null)
				{
					return Strings.NoErrors;
				}
				for (int i = 1; i < num; i++)
				{
					if (this.errorMailboxes[i] != null && this.errorMailboxes[i].Count > 0)
					{
						this.totalErrorsFound++;
						stringBuilder.AppendLine(this.errorOneLinerDetailStrings[i]);
						for (int j = 0; j < this.errorMailboxes[i].Count; j++)
						{
							if (!string.IsNullOrEmpty(this.errorMailboxes[i][j]))
							{
								stringBuilder.AppendLine(string.Format("{0}\t{1}", Strings.EDiscoveryMailbox, this.errorMailboxes[i][j]));
							}
							if (this.errorItems[i] != null && this.errorItems[i].Count > j && !string.IsNullOrEmpty(this.errorItems[i][j]))
							{
								stringBuilder.AppendLine(string.Format("{0}\t\t{1}", Strings.EDiscoveryItem, this.errorItems[i][j]));
							}
							if (this.errorDocumentIds[i] != null && this.errorDocumentIds[i].Count > j && !string.IsNullOrEmpty(this.errorDocumentIds[i][j]))
							{
								stringBuilder.AppendLine(string.Format("{0}\t{1}", Strings.EDiscoveryDocumentId, this.errorDocumentIds[i][j]));
							}
							if (this.errorServers[i] != null && this.errorServers[i].Count > j && !string.IsNullOrEmpty(this.errorServers[i][j]))
							{
								stringBuilder.AppendLine(string.Format("{0}\t{1}", Strings.EDiscoveryServer, this.errorServers[i][j]));
							}
							stringBuilder.Append("\n");
						}
					}
				}
				if (this.totalErrorsFound <= 0 && (this.errorUndefined == null || this.errorUndefined.Count <= 0))
				{
					return Strings.NoErrors;
				}
				return stringBuilder.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ErrorsUndefinedDetails
		{
			get
			{
				this.SetErrorArrays();
				if (this.errorUndefined != null && this.errorUndefined.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder(string.Empty);
					for (int i = 0; i < this.errorUndefined.Count; i++)
					{
						stringBuilder.AppendLine(this.errorUndefined[i]);
						stringBuilder.Append("\n");
					}
					return stringBuilder.ToString();
				}
				return Strings.NoErrors;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Information
		{
			get
			{
				if (base.MailboxSearch.Information != null && base.MailboxSearch.Information.Count != 0)
				{
					return base.MailboxSearch.Information.ToStringArray().StringArrayJoin(", ");
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<KeywordHitRow> KeywordHits
		{
			get
			{
				return from kwh in base.MailboxSearch.KeywordHits
				select new KeywordHitRow(kwh);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Description
		{
			get
			{
				return base.MailboxSearch.Description;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string InPlaceHoldDescription
		{
			get
			{
				string result = Strings.MailboxSearchInPlaceHoldDescriptionNone;
				if (base.MailboxSearch.InPlaceHoldEnabled)
				{
					if (base.MailboxSearch.ItemHoldPeriod.IsUnlimited || base.MailboxSearch.ItemHoldPeriod.Value.Days == 0)
					{
						result = Strings.MailboxSearchInPlaceHoldDescriptionIndefinitely;
					}
					else if (base.MailboxSearch.ItemHoldPeriod.Value.Days > 1)
					{
						result = string.Format(Strings.MailboxSearchInplaceHoldDescriptionDays, base.MailboxSearch.ItemHoldPeriod.Value.Days);
					}
					else
					{
						result = Strings.MailboxSearchInPlaceHoldDescriptionOneDay;
					}
				}
				return result;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string InPlaceHoldErrors
		{
			get
			{
				if (base.MailboxSearch.InPlaceHoldErrors != null && base.MailboxSearch.InPlaceHoldErrors.Count != 0)
				{
					return base.MailboxSearch.InPlaceHoldErrors.ToStringArray().StringArrayJoin(", ");
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int StatisticsStartIndex
		{
			get
			{
				return base.MailboxSearch.StatisticsStartIndex;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int TotalKeywords
		{
			get
			{
				return base.MailboxSearch.TotalKeywords;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int TotalKnownErrors
		{
			get
			{
				string errorsOneLine = this.ErrorsOneLine;
				return this.totalErrorsFound;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int TotalUndefinedErrors
		{
			get
			{
				this.SetErrorArrays();
				if (this.errorUndefined != null)
				{
					return this.errorUndefined.Count;
				}
				return 0;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private void SetErrorArrays()
		{
			if (this.errorArraysAreSet)
			{
				return;
			}
			int num = this.errorOneLinerDetailStrings.Length;
			if (this.errorMailboxes == null)
			{
				this.errorMailboxes = new List<string>[num];
			}
			if (this.errorItems == null)
			{
				this.errorItems = new List<string>[num];
			}
			if (this.errorDocumentIds == null)
			{
				this.errorDocumentIds = new List<string>[num];
			}
			if (this.errorServers == null)
			{
				this.errorServers = new List<string>[num];
			}
			if (this.errorUndefined == null)
			{
				this.errorUndefined = new List<string>();
			}
			foreach (string text in base.MailboxSearch.Errors)
			{
				int num2 = -1;
				int num3 = 0;
				int num4 = text.IndexOf("EDiscoveryError:E");
				if (num4 >= 0)
				{
					num3 = num4 + 17;
					int num5 = text.IndexOf("::", num3);
					if (num5 <= num3)
					{
						this.errorUndefined.Add(text);
						continue;
					}
					string s = text.Substring(num3, num5 - num3);
					int.TryParse(s, out num2);
				}
				if (num2 < 1 || num2 > num - 1)
				{
					if (text.Contains("[NotFound]") || text.Contains("'NotFound'"))
					{
						num2 = 1;
					}
					else if (text.Contains("The server cannot service this request right now. Try again later") || text.Contains("ErrorServerBusy") || text.Contains("ServerBusyException"))
					{
						num2 = 3;
					}
					else if (text.Contains("Preview search failed due to transient error 'MapiExceptionMultiMailboxSearchFailed: Multi Mailbox Search failed"))
					{
						num2 = 4;
					}
					else if (text.Contains("The mailbox database is temporarily unavailable"))
					{
						num2 = 6;
					}
					else if (text.Contains("The SMTP address has no mailbox associated with it"))
					{
						num2 = 9;
					}
				}
				if (num2 < 1 || num2 > num - 1)
				{
					this.errorUndefined.Add(text);
				}
				else
				{
					num3 = 0;
					string[] array = new string[]
					{
						"Mailbox:",
						"Item:",
						"DocumentId:",
						"Server:"
					};
					this.SetOneErrorArray(text, array[0], ref num3, ref this.errorMailboxes[num2]);
					this.SetOneErrorArray(text, array[1], ref num3, ref this.errorItems[num2]);
					this.SetOneErrorArray(text, array[2], ref num3, ref this.errorDocumentIds[num2]);
					num3 = 0;
					this.SetOneErrorArray(text, array[3], ref num3, ref this.errorServers[num2]);
				}
			}
			this.errorArraysAreSet = true;
		}

		private void SetOneErrorArray(string strError, string search, ref int iRefStart, ref List<string> errorArray)
		{
			if (errorArray == null)
			{
				errorArray = new List<string>();
			}
			int num = iRefStart;
			int num2 = strError.IndexOf(search, num);
			if (num2 < 0)
			{
				errorArray.Add(string.Empty);
				return;
			}
			num = num2 + search.Length;
			int num3 = strError.IndexOf("::", num);
			if (num3 > num)
			{
				int num4 = strError.IndexOf(search, num + search.Length);
				while (num4 > 0 && num4 < num3)
				{
					int num5 = strError.IndexOf(search, num4 + search.Length);
					if (num5 <= 0 || num5 >= num3)
					{
						break;
					}
					num4 = num5;
				}
				if (num4 > 0 && num4 < num3)
				{
					num = num4;
				}
				errorArray.Add(strError.Substring(num, num3 - num));
				iRefStart = num3 + 2;
				return;
			}
			errorArray.Add(string.Empty);
		}

		private List<string>[] errorDocumentIds;

		private List<string>[] errorItems;

		private List<string>[] errorMailboxes;

		private List<string>[] errorServers;

		private List<string> errorUndefined;

		private bool errorArraysAreSet;

		private int totalErrorsFound;

		private string[] errorOneLinerStrings = new string[]
		{
			Strings.EDiscoveryE000OneLiner,
			Strings.EDiscoveryE001OneLiner,
			Strings.EDiscoveryE002OneLiner,
			Strings.EDiscoveryE003OneLiner,
			Strings.EDiscoveryE004OneLiner,
			Strings.EDiscoveryE005OneLiner,
			Strings.EDiscoveryE006OneLiner,
			Strings.EDiscoveryE007OneLiner,
			Strings.EDiscoveryE008OneLiner,
			Strings.EDiscoveryE009OneLiner,
			Strings.EDiscoveryE010OneLiner
		};

		private string[] errorOneLinerDetailStrings = new string[]
		{
			Strings.EDiscoveryE000FullMessage,
			Strings.EDiscoveryE001FullMessage,
			Strings.EDiscoveryE002FullMessage,
			Strings.EDiscoveryE003FullMessage,
			Strings.EDiscoveryE004FullMessage,
			Strings.EDiscoveryE005FullMessage,
			Strings.EDiscoveryE006FullMessage,
			Strings.EDiscoveryE007FullMessage,
			Strings.EDiscoveryE008FullMessage,
			Strings.EDiscoveryE009FullMessage,
			Strings.EDiscoveryE010FullMessage
		};
	}
}
