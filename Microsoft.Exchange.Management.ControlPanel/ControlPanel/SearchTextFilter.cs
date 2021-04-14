using System;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class SearchTextFilter : ResultSizeFilter
	{
		[DataMember]
		public string SearchText
		{
			get
			{
				return this.searchText;
			}
			set
			{
				this.searchText = value;
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			this.UpdateFilterProperty();
		}

		protected string TranslateBasicFilter()
		{
			string text = (this.FilterType == SearchTextFilterType.StartsWith) ? "{0} -like '{1}*'" : ((this.FilterType == SearchTextFilterType.Contains) ? "{0} -like '*{1}*'" : "{0} -eq '{1}'");
			string text2 = null;
			if (!string.IsNullOrEmpty(this.searchText))
			{
				text2 = this.searchText;
				text2 = text2.Replace("'", "''");
				text2 = text2.TrimEnd(new char[]
				{
					'*'
				});
				if (this.FilterType == SearchTextFilterType.Contains)
				{
					text2 = text2.TrimStart(new char[]
					{
						'*'
					});
				}
				if (text2.Length > 0)
				{
					StringBuilder stringBuilder = new StringBuilder((text2.Length + text.Length + this.FilterableProperties[0].Length) * this.FilterableProperties.Length + 10);
					stringBuilder.Append(string.Format(text, this.FilterableProperties[0], text2));
					for (int i = 1; i < this.FilterableProperties.Length; i++)
					{
						stringBuilder.Append(" -or ");
						stringBuilder.Append(string.Format(text, this.FilterableProperties[i], text2));
					}
					text2 = stringBuilder.ToString();
				}
			}
			return text2;
		}

		protected virtual void UpdateFilterProperty()
		{
			base["Filter"] = this.TranslateBasicFilter();
		}

		protected abstract string[] FilterableProperties { get; }

		protected virtual SearchTextFilterType FilterType
		{
			get
			{
				return SearchTextFilterType.StartsWith;
			}
		}

		protected const string StartsWithFilter = "{0} -like '{1}*'";

		protected const string ContainsFilter = "{0} -like '*{1}*'";

		protected const string EqualsFilter = "{0} -eq '{1}'";

		public new const string RbacParameters = "?ResultSize&Filter";

		private string searchText;
	}
}
