using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class PreviewPickerFilter : RecipientPickerFilterBase
	{
		[DataMember]
		public SupportRecipientFilterObject CustomizedFilters { get; set; }

		[DataMember]
		public string PreviewType { get; set; }

		public bool HasCondition { get; set; }

		protected override RecipientTypeDetails[] RecipientTypeDetailsParam
		{
			get
			{
				return new RecipientTypeDetails[]
				{
					RecipientTypeDetails.EquipmentMailbox,
					RecipientTypeDetails.DynamicDistributionGroup,
					RecipientTypeDetails.LegacyMailbox,
					RecipientTypeDetails.LinkedMailbox,
					RecipientTypeDetails.MailContact,
					RecipientTypeDetails.MailForestContact,
					RecipientTypeDetails.MailNonUniversalGroup,
					RecipientTypeDetails.MailUniversalDistributionGroup,
					RecipientTypeDetails.MailUniversalSecurityGroup,
					RecipientTypeDetails.MailUser,
					RecipientTypeDetails.PublicFolder,
					(RecipientTypeDetails)((ulong)int.MinValue),
					RecipientTypeDetails.RemoteRoomMailbox,
					RecipientTypeDetails.RemoteEquipmentMailbox,
					RecipientTypeDetails.RemoteSharedMailbox,
					RecipientTypeDetails.RemoteTeamMailbox,
					RecipientTypeDetails.RoomMailbox,
					RecipientTypeDetails.SharedMailbox,
					RecipientTypeDetails.TeamMailbox,
					RecipientTypeDetails.UserMailbox
				};
			}
		}

		protected override void UpdateFilterProperty()
		{
			string text = null;
			if (this.CustomizedFilters != null)
			{
				if (this.CustomizedFilters.DataObject != null && !string.IsNullOrEmpty(this.CustomizedFilters.DataObject.LdapRecipientFilter))
				{
					text = this.CustomizedFilters.DataObject.LdapRecipientFilter;
					this.HasCondition = true;
				}
				else if (!string.IsNullOrEmpty(this.CustomizedFilters.LdapRecipientFilter))
				{
					text = this.CustomizedFilters.LdapRecipientFilter;
					this.HasCondition = true;
				}
				if (this.CustomizedFilters.RecipientContainer != null)
				{
					base["OrganizationalUnit"] = this.CustomizedFilters.RecipientContainer;
					this.HasCondition = true;
				}
			}
			if (this.HasCondition)
			{
				bool flag = this.PreviewType != null && this.PreviewType.Equals("al", StringComparison.InvariantCultureIgnoreCase);
				if (text != null)
				{
					if (flag)
					{
						text = string.Format("(&{0}{1})", PreviewPickerFilter.ldapHiddenFromALFilter, text);
					}
					base["RecipientPreviewFilter"] = text;
					return;
				}
				if (flag)
				{
					base["Filter"] = PreviewPickerFilter.recipientHiddenFromALFilter;
				}
			}
		}

		public new const string RbacParameters = "?ResultSize&Filter&RecipientTypeDetails&Properties&RecipientPreviewFilter&OrganizationalUnit";

		private const string PreviewType_AL = "al";

		private static ComparisonFilter hiddenFromALFilter = new ComparisonFilter(ComparisonOperator.NotEqual, ADRecipientSchema.HiddenFromAddressListsValue, true);

		private static string ldapHiddenFromALFilter = LdapFilterBuilder.LdapFilterFromQueryFilter(PreviewPickerFilter.hiddenFromALFilter);

		private static string recipientHiddenFromALFilter = PreviewPickerFilter.hiddenFromALFilter.GenerateInfixString(FilterLanguage.Monad);
	}
}
