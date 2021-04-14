using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class RecipientFilter : ViewDropDownFilter
	{
		protected RecipientFilter()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			base["Properties"] = "PrimarySmtpAddress,DisplayName,ArchiveGuid,AuthenticationType,RecipientType,RecipientTypeDetails,ResourceType,WindowsLiveID,Identity,ExchangeVersion,OrganizationId";
			RecipientTypeDetails[] recipientTypeDetailsParam = this.RecipientTypeDetailsParam;
			if (recipientTypeDetailsParam != null && recipientTypeDetailsParam.Length > 0)
			{
				this.RecipientTypeDetailsList = recipientTypeDetailsParam;
			}
		}

		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Get-Recipient";
			}
		}

		public RecipientTypeDetails[] RecipientTypeDetailsList
		{
			get
			{
				return (RecipientTypeDetails[])base["RecipientTypeDetails"];
			}
			protected set
			{
				base["RecipientTypeDetails"] = value;
			}
		}

		protected abstract RecipientTypeDetails[] RecipientTypeDetailsParam { get; }

		protected override string[] FilterableProperties
		{
			get
			{
				return RecipientFilter.filterableProperties;
			}
		}

		protected override void UpdateFilterProperty()
		{
			base["Filter"] = this.TranslateToAnr();
		}

		private string TranslateToAnr()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.TranslateBasicFilter());
			if (!string.IsNullOrEmpty(base.SearchText))
			{
				string text = base.SearchText.Trim();
				int num = text.Trim().IndexOf(" ");
				if (num > 0)
				{
					string text2 = text.Substring(0, num);
					string text3 = text.Substring(num + 1, text.Length - 1 - num);
					TextFilter textFilter = new TextFilter(OrgPersonPresentationObjectSchema.FirstName, text2, MatchOptions.Prefix, MatchFlags.Default);
					TextFilter textFilter2 = new TextFilter(OrgPersonPresentationObjectSchema.LastName, text3, MatchOptions.Prefix, MatchFlags.Default);
					CompositeFilter compositeFilter = new AndFilter(new QueryFilter[]
					{
						textFilter,
						textFilter2
					});
					QueryFilter queryFilter = new TextFilter(OrgPersonPresentationObjectSchema.FirstName, text3, MatchOptions.Prefix, MatchFlags.Default);
					QueryFilter queryFilter2 = new TextFilter(OrgPersonPresentationObjectSchema.LastName, text2, MatchOptions.Prefix, MatchFlags.Default);
					CompositeFilter compositeFilter2 = new AndFilter(new QueryFilter[]
					{
						queryFilter,
						queryFilter2
					});
					stringBuilder.Append(" -or ");
					stringBuilder.Append(new OrFilter(new QueryFilter[]
					{
						compositeFilter,
						compositeFilter2
					}).GenerateInfixString(FilterLanguage.Monad));
				}
			}
			return stringBuilder.ToString();
		}

		public new const string RbacParameters = "?ResultSize&Filter&RecipientTypeDetails&Properties";

		private static string[] filterableProperties = new string[]
		{
			"DisplayName",
			"Alias",
			OrgPersonPresentationObjectSchema.FirstName.Name,
			OrgPersonPresentationObjectSchema.LastName.Name
		};
	}
}
