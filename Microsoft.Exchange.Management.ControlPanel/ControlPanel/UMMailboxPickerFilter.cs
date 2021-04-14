using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMMailboxPickerFilter : RecipientFilter
	{
		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		[DataMember]
		public Identity DialPlanIdentity { get; set; }

		protected override RecipientTypeDetails[] RecipientTypeDetailsParam
		{
			get
			{
				return UMMailboxPickerFilter.allowedRecipientTypeDetails;
			}
		}

		protected override void UpdateFilterProperty()
		{
			base.UpdateFilterProperty();
			string text = (string)base["Filter"];
			if (!string.IsNullOrEmpty(text))
			{
				text = string.Format("({0}) -and ({1})", text, this.GetUMFilter());
			}
			else
			{
				text = this.GetUMFilter();
			}
			base["Filter"] = text;
		}

		private string GetUMFilter()
		{
			string result = "(UMEnabled -eq 'true')";
			if (this.DialPlanIdentity != null)
			{
				ADObjectId adobjectId;
				if (!ADObjectId.TryParseDnOrGuid(this.DialPlanIdentity.RawIdentity, out adobjectId))
				{
					throw new FaultException(new ArgumentException(this.DialPlanIdentity.RawIdentity, "DialPlanIdentity").Message);
				}
				adobjectId = ADSystemConfigurationObjectIDResolver.Instance.ResolveObject(adobjectId);
				if (adobjectId != null)
				{
					result = string.Format("(UMEnabled -eq 'true') -and (UMRecipientDialPlanId -eq '{0}')", adobjectId.DistinguishedName);
				}
			}
			return result;
		}

		private const string UMEnabledFilter = "(UMEnabled -eq 'true')";

		private const string UMRecipientDialPlanIdFilterFormat = "(UMEnabled -eq 'true') -and (UMRecipientDialPlanId -eq '{0}')";

		private static readonly RecipientTypeDetails[] allowedRecipientTypeDetails = UMMailbox.GetUMRecipientTypeDetails();
	}
}
