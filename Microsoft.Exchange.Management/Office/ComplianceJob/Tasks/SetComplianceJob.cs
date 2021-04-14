using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Office.ComplianceJob.Tasks
{
	public abstract class SetComplianceJob<TDataObject> : SetTenantADTaskBase<ComplianceJobIdParameter, TDataObject, TDataObject> where TDataObject : ComplianceJob, new()
	{
		[Parameter(Mandatory = false)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] ExchangeBinding
		{
			get
			{
				return (string[])base.Fields["ExchangeBinding"];
			}
			set
			{
				base.Fields["ExchangeBinding"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] PublicFolderBinding
		{
			get
			{
				return (string[])base.Fields["PublicFolderBinding"];
			}
			set
			{
				base.Fields["PublicFolderBinding"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] SharePointBinding
		{
			get
			{
				return (string[])base.Fields["SharePointBinding"];
			}
			set
			{
				base.Fields["SharePointBinding"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] AddExchangeBinding
		{
			get
			{
				return (string[])base.Fields["AddExchangeBinding"];
			}
			set
			{
				base.Fields["AddExchangeBinding"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] AddPublicFolderBinding
		{
			get
			{
				return (string[])base.Fields["AddPublicFolderBinding"];
			}
			set
			{
				base.Fields["AddPublicFolderBinding"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] AddSharePointBinding
		{
			get
			{
				return (string[])base.Fields["AddSharePointBinding"];
			}
			set
			{
				base.Fields["AddSharePointBinding"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] RemoveExchangeBinding
		{
			get
			{
				return (string[])base.Fields["RemoveExchangeBinding"];
			}
			set
			{
				base.Fields["RemoveExchangeBinding"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] RemovePublicFolderBinding
		{
			get
			{
				return (string[])base.Fields["RemovePublicFolderBinding"];
			}
			set
			{
				base.Fields["RemovePublicFolderBinding"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] RemoveSharePointBinding
		{
			get
			{
				return (string[])base.Fields["RemoveSharePointBinding"];
			}
			set
			{
				base.Fields["RemoveSharePointBinding"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllPublicFolderBindings
		{
			get
			{
				return (bool)(base.Fields["AllPublicFolderBindings"] ?? false);
			}
			set
			{
				base.Fields["AllPublicFolderBindings"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllExchangeBindings
		{
			get
			{
				return (bool)(base.Fields["AllExchangeBindings"] ?? false);
			}
			set
			{
				base.Fields["AllExchangeBindings"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllSharePointBindings
		{
			get
			{
				return (bool)(base.Fields["AllSharePointBindings"] ?? false);
			}
			set
			{
				base.Fields["AllSharePointBindings"] = value;
			}
		}

		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			if (base.ExchangeRunspaceConfig == null)
			{
				base.ThrowTerminatingError(new ComplianceJobTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			return new ComplianceJobProvider(base.ExchangeRunspaceConfig.OrganizationId);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				TDataObject dataObject = this.DataObject;
				if (dataObject.IsChanged(ComplianceJobSchema.DisplayName))
				{
					ComplianceJobProvider complianceJobProvider = (ComplianceJobProvider)base.DataSession;
					TDataObject dataObject2 = this.DataObject;
					if (complianceJobProvider.FindJobsByName<ComplianceSearch>(dataObject2.Name) != null)
					{
						TDataObject dataObject3 = this.DataObject;
						base.WriteError(new ComplianceSearchNameIsNotUniqueException(dataObject3.Name), ErrorCategory.InvalidArgument, this.DataObject);
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			ComplianceJob complianceJob = (ComplianceJob)base.PrepareDataObject();
			bool flag = false;
			string[] array = null;
			if (this.ProcessBindings(complianceJob, ComplianceBindingType.ExchangeBinding, out flag, out array))
			{
				base.Fields["ExchangeBinding"] = array;
				complianceJob.ExchangeBindings = array;
				complianceJob.AllExchangeBindings = flag;
			}
			if (this.ProcessBindings(complianceJob, ComplianceBindingType.PublicFolderBinding, out flag, out array))
			{
				base.Fields["PublicFolderBinding"] = array;
				complianceJob.PublicFolderBindings = array;
				complianceJob.AllPublicFolderBindings = flag;
			}
			if (this.ProcessBindings(complianceJob, ComplianceBindingType.SharePointBinding, out flag, out array))
			{
				base.Fields["SharePointBinding"] = array;
				complianceJob.SharePointBindings = array;
				complianceJob.AllSharePointBindings = flag;
			}
			if ((complianceJob.ExchangeBindings == null || complianceJob.ExchangeBindings.Count == 0) && !complianceJob.AllExchangeBindings && (complianceJob.PublicFolderBindings == null || complianceJob.PublicFolderBindings.Count == 0) && !complianceJob.AllPublicFolderBindings && (complianceJob.SharePointBindings == null || complianceJob.SharePointBindings.Count == 0) && !complianceJob.AllSharePointBindings)
			{
				base.WriteError(new ComplianceJobTaskException(Strings.NoBindingsSet), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields.IsModified("Description"))
			{
				complianceJob.Description = this.Description;
			}
			if (base.Fields.IsModified("Name"))
			{
				complianceJob.Name = this.Name;
			}
			complianceJob.LastModifiedTime = DateTime.UtcNow;
			return complianceJob;
		}

		private bool ProcessBindings(ComplianceJob dataObject, ComplianceBindingType bindingType, out bool processedAllBindings, out string[] processedBindings)
		{
			processedAllBindings = false;
			processedBindings = null;
			string text;
			string text2;
			bool flag;
			string[] array;
			string[] array2;
			string[] array3;
			MultiValuedProperty<string> multiValuedProperty;
			switch (bindingType)
			{
			case ComplianceBindingType.ExchangeBinding:
				text = "ExchangeBinding";
				text2 = "AllExchangeBindings";
				flag = (bool)(base.Fields["AllExchangeBindings"] ?? false);
				array = (string[])base.Fields["ExchangeBinding"];
				array2 = (string[])base.Fields["AddExchangeBinding"];
				array3 = (string[])base.Fields["RemoveExchangeBinding"];
				multiValuedProperty = dataObject.ExchangeBindings;
				processedAllBindings = dataObject.AllExchangeBindings;
				break;
			case ComplianceBindingType.SharePointBinding:
				text = "SharePointBinding";
				text2 = "AllSharePointBindings";
				flag = (bool)(base.Fields["AllSharePointBindings"] ?? false);
				array = (string[])base.Fields["SharePointBinding"];
				array2 = (string[])base.Fields["AddSharePointBinding"];
				array3 = (string[])base.Fields["RemoveSharePointBinding"];
				multiValuedProperty = dataObject.SharePointBindings;
				processedAllBindings = dataObject.AllSharePointBindings;
				break;
			case ComplianceBindingType.PublicFolderBinding:
				text = "PublicFolderBinding";
				text2 = "AllPublicFolderBindings";
				flag = (bool)(base.Fields["AllPublicFolderBindings"] ?? false);
				array = (string[])base.Fields["PublicFolderBinding"];
				array2 = (string[])base.Fields["AddPublicFolderBinding"];
				array3 = (string[])base.Fields["RemovePublicFolderBinding"];
				multiValuedProperty = dataObject.PublicFolderBindings;
				processedAllBindings = dataObject.AllPublicFolderBindings;
				break;
			default:
				return false;
			}
			bool result = false;
			if (base.Fields.IsModified(text))
			{
				result = true;
			}
			else if (multiValuedProperty != null)
			{
				array = multiValuedProperty.ToArray();
			}
			if (array2 != null && array2.Length > 0)
			{
				if (array == null)
				{
					array = array2;
				}
				else
				{
					array = array.Union(array2).ToArray<string>();
				}
				result = true;
			}
			if (array3 != null && array3.Length > 0 && array != null)
			{
				array = array.Except(array3).ToArray<string>();
				result = true;
			}
			if (base.Fields.IsModified(text2))
			{
				processedAllBindings = flag;
				result = true;
			}
			if (flag)
			{
				if (array != null || multiValuedProperty != null)
				{
					this.WriteWarning(Strings.AllSourceMailboxesParameterOverride(text2, text));
				}
				processedBindings = null;
			}
			else
			{
				processedBindings = array;
			}
			return result;
		}

		private const string ParameterExchangeBinding = "ExchangeBinding";

		private const string ParameterPublicFolderBinding = "PublicFolderBinding";

		private const string ParameterSharePointBinding = "SharePointBinding";

		private const string ParameterAddExchangeBinding = "AddExchangeBinding";

		private const string ParameterAddPublicFolderBinding = "AddPublicFolderBinding";

		private const string ParameterAddSharePointBinding = "AddSharePointBinding";

		private const string ParameterRemoveExchangeBinding = "RemoveExchangeBinding";

		private const string ParameterRemovePublicFolderBinding = "RemovePublicFolderBinding";

		private const string ParameterRemoveSharePointBinding = "RemoveSharePointBinding";

		private const string ParameterAllPublicFolderBindings = "AllPublicFolderBindings";

		private const string ParameterAllExchangeBindings = "AllExchangeBindings";

		private const string ParameterAllSharePointBindings = "AllSharePointBindings";

		private const string ParameterForce = "Force";

		private const string ParameterDescription = "Description";

		private const string ParameterName = "Name";
	}
}
