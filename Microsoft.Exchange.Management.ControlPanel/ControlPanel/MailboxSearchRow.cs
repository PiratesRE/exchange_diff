using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxSearchRow : BaseRow
	{
		public MailboxSearchRow(MailboxSearchObject searchObject) : base(new Identity(searchObject.Identity, searchObject.Name), searchObject)
		{
			this.MailboxSearch = searchObject;
		}

		public MailboxSearchObject MailboxSearch { get; set; }

		[DataMember]
		public string Name
		{
			get
			{
				return this.MailboxSearch.Name;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LastStartTime
		{
			get
			{
				return this.MailboxSearch.LastStartTime.ToUserDateTimeString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Status
		{
			get
			{
				return LocalizedDescriptionAttribute.FromEnum(typeof(SearchState), this.MailboxSearch.Status);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DisplayStatus
		{
			get
			{
				string text = string.Empty;
				text = LocalizedDescriptionAttribute.FromEnum(typeof(SearchState), this.MailboxSearch.Status);
				if (!this.MailboxSearch.Status.Equals(SearchState.InProgress))
				{
					return text;
				}
				return string.Format(Strings.MailboxSearchInProgressStatus, text, this.MailboxSearch.PercentComplete);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsStoppable
		{
			get
			{
				return this.MailboxSearch.Status == SearchState.InProgress || SearchState.EstimateInProgress == this.MailboxSearch.Status;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsStartable
		{
			get
			{
				if (this.MailboxSearch.EstimateOnly)
				{
					return SearchState.EstimateInProgress != this.MailboxSearch.Status && SearchState.EstimateStopping != this.MailboxSearch.Status;
				}
				return this.MailboxSearch.Status != SearchState.InProgress && SearchState.Stopping != this.MailboxSearch.Status;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsFullStatsSearchAllowed
		{
			get
			{
				return !this.IsKeywordStatisticsDisabled && !string.IsNullOrEmpty(this.MailboxSearch.SearchQuery) && !this.MailboxSearch.IncludeKeywordStatistics && this.MailboxSearch.EstimateOnly;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsKeywordStatisticsDisabled
		{
			get
			{
				return this.MailboxSearch.KeywordStatisticsDisabled;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Icon
		{
			get
			{
				return MailboxSearchRow.FromEnum(this.MailboxSearch.Status);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsResumable
		{
			get
			{
				return this.MailboxSearch.Status == SearchState.PartiallySucceeded;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsEstimateOnly
		{
			get
			{
				return this.MailboxSearch.EstimateOnly;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsPreviewable
		{
			get
			{
				return !string.IsNullOrEmpty(this.PreviewResultsLink);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PreviewResultsLink
		{
			get
			{
				return this.MailboxSearch.PreviewResultsLink;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string HoldStatusDescription
		{
			get
			{
				if (RbacPrincipal.Current.IsInRole("LegalHold"))
				{
					return this.MailboxSearch.InPlaceHoldEnabled ? Strings.DiscoveryHoldHoldStatusYes : Strings.DiscoveryHoldHoldStatusNo;
				}
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CreatedByDisplayName
		{
			get
			{
				return DiscoveryHoldPropertiesHelper.GetCreatedByUserDisplayName(this.MailboxSearch.CreatedBy);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string LastModifiedTimeDisplay
		{
			get
			{
				return this.MailboxSearch.LastModifiedTime.ToUserDateTimeString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public DateTime LastModifiedUTCDateTime
		{
			get
			{
				if (this.MailboxSearch.LastModifiedTime == null)
				{
					return DateTime.MinValue;
				}
				return this.MailboxSearch.LastModifiedTime.Value.UniversalTime;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ExportToolUrl
		{
			get
			{
				string newValue = string.Empty;
				if (MailboxSearchRow.LocalServer != null && !string.IsNullOrEmpty(MailboxSearchRow.LocalServer.Fqdn))
				{
					newValue = MailboxSearchRow.LocalServer.Fqdn;
				}
				return ThemeResource.ExportToolPath.Replace("{0}", newValue) + "microsoft.exchange.ediscovery.exporttool.application?name=" + Uri.EscapeDataString(this.Name) + "&ews=";
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		internal static string FromEnum(SearchState status)
		{
			string result = string.Empty;
			switch (status)
			{
			case SearchState.InProgress:
			case SearchState.EstimateInProgress:
				result = CommandSprite.GetCssClass(CommandSprite.SpriteId.MailboxSearchInProgress);
				break;
			case SearchState.Failed:
			case SearchState.EstimateFailed:
				result = CommandSprite.GetCssClass(CommandSprite.SpriteId.MailboxSearchFailed);
				break;
			case SearchState.Stopping:
			case SearchState.EstimateStopping:
				result = CommandSprite.GetCssClass(CommandSprite.SpriteId.MailboxSearchStopping);
				break;
			case SearchState.Stopped:
			case SearchState.EstimateStopped:
				result = CommandSprite.GetCssClass(CommandSprite.SpriteId.MailboxSearchStopped);
				break;
			case SearchState.Succeeded:
			case SearchState.EstimateSucceeded:
				result = CommandSprite.GetCssClass(CommandSprite.SpriteId.MailboxSearchSucceeded);
				break;
			case SearchState.PartiallySucceeded:
			case SearchState.EstimatePartiallySucceeded:
				result = CommandSprite.GetCssClass(CommandSprite.SpriteId.MailboxSearchPartiallySucceeded);
				break;
			}
			return result;
		}

		[DataMember]
		public string ResultSize
		{
			get
			{
				if (this.MailboxSearch.EstimateOnly)
				{
					return this.MailboxSearch.ResultSizeEstimate.ToAppropriateUnitFormatString("{0:0.##}");
				}
				return this.MailboxSearch.ResultSize.ToAppropriateUnitFormatString("{0:0.##}");
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private static Server LocalServer
		{
			get
			{
				if (MailboxSearchRow.localServer == null)
				{
					MailboxSearchRow.localServer = MailboxSearchRow.GetLocalServer();
				}
				return MailboxSearchRow.localServer;
			}
		}

		private static Server GetLocalServer()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 396, "GetLocalServer", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\Reporting\\MailboxSearches.cs");
			topologyConfigurationSession.UseConfigNC = true;
			topologyConfigurationSession.UseGlobalCatalog = true;
			return topologyConfigurationSession.FindLocalServer();
		}

		private static Server localServer;
	}
}
