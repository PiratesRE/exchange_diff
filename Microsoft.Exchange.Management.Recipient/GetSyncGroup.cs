using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "SyncGroup", DefaultParameterSetName = "Identity")]
	public sealed class GetSyncGroup : GetRecipientBase<NonMailEnabledGroupIdParameter, ADGroup>
	{
		[Parameter(Mandatory = false)]
		public new long UsnForReconciliationSearch
		{
			get
			{
				return base.UsnForReconciliationSearch;
			}
			set
			{
				base.UsnForReconciliationSearch = value;
			}
		}

		protected override PropertyDefinition[] SortProperties
		{
			get
			{
				return GetSyncGroup.SortPropertiesArray;
			}
		}

		protected override RecipientType[] RecipientTypes
		{
			get
			{
				return NonMailEnabledGroupIdParameter.AllowedRecipientTypes;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return NonMailEnabledGroupIdParameter.AllowedRecipientTypeDetails;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncGroup.FromDataObject((ADGroup)dataObject);
		}

		internal override ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<SyncGroupSchema>();
			}
		}

		private static readonly PropertyDefinition[] SortPropertiesArray = new PropertyDefinition[]
		{
			ADObjectSchema.Name,
			ADRecipientSchema.DisplayName
		};
	}
}
