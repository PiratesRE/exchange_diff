using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class SetOrgPersonObjectTask<TIdentity, TPublicObject, TDataObject> : SetRecipientObjectTask<TIdentity, TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : OrgPersonPresentationObject, new() where TDataObject : ADRecipient, IADOrgPerson, new()
	{
		[Parameter(Mandatory = false)]
		public UserContactIdParameter Manager
		{
			get
			{
				return (UserContactIdParameter)base.Fields[OrgPersonPresentationObjectSchema.Manager];
			}
			set
			{
				base.Fields[OrgPersonPresentationObjectSchema.Manager] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CreateDTMFMap
		{
			get
			{
				return (bool)(base.Fields["CreateDTMFMap"] ?? false);
			}
			set
			{
				base.Fields["CreateDTMFMap"] = value;
			}
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			OrgPersonPresentationObject orgPersonPresentationObject = (OrgPersonPresentationObject)this.GetDynamicParameters();
			if (base.Fields.IsModified(OrgPersonPresentationObjectSchema.Manager))
			{
				if (this.Manager != null)
				{
					this.manager = (ADRecipient)base.GetDataObject<ADRecipient>(this.Manager, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorUserOrContactNotFound(this.Manager.ToString())), new LocalizedString?(Strings.ErrorUserOrContactNotUnique(this.Manager.ToString())));
					orgPersonPresentationObject.Manager = (ADObjectId)this.manager.Identity;
					return;
				}
				orgPersonPresentationObject.Manager = null;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			IADOrgPerson iadorgPerson = (IADOrgPerson)base.PrepareDataObject();
			ADRecipient adrecipient = iadorgPerson as ADRecipient;
			if (!base.Fields.IsModified(OrgPersonPresentationObjectSchema.Manager) && adrecipient.IsModified(ADOrgPersonSchema.Manager) && iadorgPerson.Manager != null)
			{
				this.manager = (ADRecipient)base.GetDataObject<ADRecipient>(new UserContactIdParameter(iadorgPerson.Manager), base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorUserOrContactNotFound(iadorgPerson.Manager.ToString())), new LocalizedString?(Strings.ErrorUserOrContactNotUnique(iadorgPerson.Manager.ToString())));
			}
			if (this.CreateDTMFMap)
			{
				iadorgPerson.PopulateDtmfMap(true);
			}
			TaskLogger.LogExit();
			return adrecipient;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			TDataObject dataObject = this.DataObject;
			if (dataObject.IsModified(ADOrgPersonSchema.UMCallingLineIds))
			{
				this.ValidateUMCallingLineIds();
			}
			if (this.manager != null)
			{
				RecipientTaskHelper.CheckRecipientInSameOrganizationWithDataObject(this.DataObject, this.manager, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
		}

		private void ValidateUMCallingLineIds()
		{
			TDataObject dataObject = this.DataObject;
			foreach (string text in dataObject.UMCallingLineIds)
			{
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADOrgPersonSchema.UMCallingLineIds, text);
				string format = "tel:{0}";
				string text2 = string.Format(format, text.StartsWith("+", StringComparison.OrdinalIgnoreCase) ? text : ("+" + text));
				QueryFilter queryFilter2 = new TextFilter(ADOrgPersonSchema.RtcSipLine, text2 + ";", MatchOptions.Prefix, MatchFlags.IgnoreCase);
				QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, ADOrgPersonSchema.RtcSipLine, text2);
				QueryFilter queryFilter4 = new OrFilter(new QueryFilter[]
				{
					queryFilter2,
					queryFilter3
				});
				ADRecipient[] array = base.TenantGlobalCatalogSession.Find(null, QueryScope.SubTree, new OrFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter4
				}), null, 2);
				if (array != null)
				{
					foreach (ADRecipient adrecipient in array)
					{
						Guid guid = adrecipient.Guid;
						TDataObject dataObject2 = this.DataObject;
						if (guid != dataObject2.Guid)
						{
							Exception exception = new ArgumentException(Strings.CallingLineIdNotUnique(text, adrecipient.Identity.ToString()), "UMCallingLineIds");
							ErrorCategory category = ErrorCategory.InvalidArgument;
							TDataObject dataObject3 = this.DataObject;
							base.WriteError(exception, category, dataObject3.Identity);
						}
					}
				}
			}
		}

		private const string createDTMFMap = "CreateDTMFMap";

		private ADRecipient manager;
	}
}
