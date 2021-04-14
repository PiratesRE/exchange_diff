using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class UIPresentationProfile
	{
		public UIPresentationProfile() : this(new ResultsColumnProfile[0], new FilterColumnProfile[0])
		{
		}

		public UIPresentationProfile(ResultsColumnProfile[] displayedColumnCollection) : this(displayedColumnCollection, new FilterColumnProfile[0])
		{
		}

		public UIPresentationProfile(ResultsColumnProfile[] displayedColumnCollection, FilterColumnProfile[] filterColumnCollection)
		{
			this.DisplayedColumnCollection = (displayedColumnCollection ?? new ResultsColumnProfile[0]);
			this.FilterColumnCollection = (filterColumnCollection ?? new FilterColumnProfile[0]);
			this.ScopeSupportingLevel = ScopeSupportingLevel.NoScoping;
		}

		public bool AutoGenerateColumns { get; set; }

		public ResultsColumnProfile[] DisplayedColumnCollection { get; private set; }

		public FilterColumnProfile[] FilterColumnCollection { get; private set; }

		internal ObjectSchema FilterObjectSchema { get; set; }

		internal FilterLanguage FilterLanguage { get; set; }

		internal Dictionary<string, FilterablePropertyDescription> FilterableProperties
		{
			get
			{
				if (this.filterableProperties == null)
				{
					this.filterableProperties = new Dictionary<string, FilterablePropertyDescription>();
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					foreach (ResultsColumnProfile resultsColumnProfile in this.DisplayedColumnCollection)
					{
						dictionary[resultsColumnProfile.Name] = resultsColumnProfile.Text;
					}
					foreach (FilterColumnProfile filterColumnProfile in this.FilterColumnCollection)
					{
						FilterablePropertyDescription value = new FilterablePropertyDescription(filterColumnProfile.PropertyDefinition, dictionary[filterColumnProfile.RefDisplayedColumn], filterColumnProfile.Operators, filterColumnProfile.PickerProfile, filterColumnProfile.ValueMember)
						{
							ObjectPickerDisplayMember = filterColumnProfile.DisplayMember,
							ColumnType = filterColumnProfile.ColumnType,
							FormatMode = filterColumnProfile.FormatMode,
							FilterableListSource = filterColumnProfile.FilterableListSource
						};
						this.filterableProperties.Add(filterColumnProfile.Name, value);
					}
				}
				return this.filterableProperties;
			}
		}

		public ExchangeColumnHeader[] CreateColumnHeaders()
		{
			List<ExchangeColumnHeader> list = new List<ExchangeColumnHeader>();
			foreach (ResultsColumnProfile resultsColumnProfile in this.DisplayedColumnCollection)
			{
				list.Add(new ExchangeColumnHeader(resultsColumnProfile.Name, resultsColumnProfile.Text, -2, resultsColumnProfile.IsDefault, resultsColumnProfile.DefaultEmptyText, resultsColumnProfile.CustomFormatter, resultsColumnProfile.FormatString, resultsColumnProfile.FormatProvider)
				{
					IsSortable = (resultsColumnProfile.SortMode != SortMode.NotSupported),
					ToColorFormatter = resultsColumnProfile.ColorFormatter
				});
			}
			return list.ToArray();
		}

		public bool HideIcon { get; set; }

		public string DisplayName { get; set; }

		public string ImageProperty { get; set; }

		public string SortProperty { get; set; }

		public bool UseTreeViewForm { get; set; }

		public string HelpTopic { get; set; }

		public ScopeSupportingLevel ScopeSupportingLevel { get; set; }

		public ExchangeRunspaceConfigurationSettings.SerializationLevel SerializationLevel { get; set; }

		public bool MultiSelect { get; set; }

		private Dictionary<string, FilterablePropertyDescription> filterableProperties;
	}
}
