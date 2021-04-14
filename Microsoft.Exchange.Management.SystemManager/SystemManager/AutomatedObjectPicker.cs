using System;
using System.Data;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	public sealed class AutomatedObjectPicker : ObjectPicker
	{
		public AutomatedObjectPicker()
		{
		}

		public AutomatedObjectPicker(IResultsLoaderConfiguration configurable) : base(configurable)
		{
		}

		public AutomatedObjectPicker(string profileName) : base(profileName)
		{
		}

		public override ExchangeColumnHeader[] CreateColumnHeaders()
		{
			return base.ObjectPickerProfile.CreateColumnHeaders();
		}

		public override DataTable CreateResultsDataTable()
		{
			return base.ObjectPickerProfile.CreateResultsDataTable();
		}

		public void InputValue(string columnName, object value)
		{
			base.ObjectPickerProfile.InputValue(columnName, value);
		}

		public object GetValue(string columnName)
		{
			return base.ObjectPickerProfile.GetValue(columnName);
		}

		public override bool ShowListItemIcon
		{
			get
			{
				return !base.ObjectPickerProfile.HideIcon;
			}
		}

		protected override DataTableLoader CreateDataTableLoader()
		{
			return new DataTableLoader(base.ObjectPickerProfile)
			{
				EnforeViewEntireForest = true
			};
		}

		public override void PerformQuery(object rootId, string searchText)
		{
			(base.DataTableLoader.RefreshArgument as ResultsLoaderProfile).SearchText = searchText;
			(base.DataTableLoader.RefreshArgument as ResultsLoaderProfile).Scope = ((base.ShouldScopingWithinDefaultDomainScope && rootId == null) ? base.ScopeSettings.OrganizationalUnit : rootId);
			(base.DataTableLoader.RefreshArgument as ResultsLoaderProfile).IsResolving = false;
			(base.DataTableLoader.RefreshArgument as ResultsLoaderProfile).PipelineObjects = null;
			base.DataTableLoader.Refresh(NullProgress.Value);
		}

		public override string ObjectClassDisplayName
		{
			get
			{
				return base.ObjectPickerProfile.DisplayName;
			}
		}

		public override string NameProperty
		{
			get
			{
				return base.ObjectPickerProfile.NameProperty;
			}
		}

		public override string IdentityProperty
		{
			get
			{
				return base.ObjectPickerProfile.DistinguishIdentity;
			}
		}

		public void SetTitle(string displayName)
		{
			base.ObjectPickerProfile.DisplayName = displayName;
		}

		public override string ImageProperty
		{
			get
			{
				return base.ObjectPickerProfile.ImageProperty;
			}
		}
	}
}
