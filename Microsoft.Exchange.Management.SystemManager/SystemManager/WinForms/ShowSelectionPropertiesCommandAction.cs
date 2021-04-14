using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ShowSelectionPropertiesCommandAction : ResultsCommandAction
	{
		public ShowSelectionPropertiesCommandAction()
		{
		}

		public Assembly Assembly { get; set; }

		public string Schema { get; set; }

		public override bool HasPermission()
		{
			if (!string.IsNullOrEmpty(this.Schema) && null != this.Assembly)
			{
				PageConfigurableProfile pageConfigurableProfile = AutomatedDataHandlerBase.BuildProfile(this.Assembly, this.Schema) as PageConfigurableProfile;
				return pageConfigurableProfile.HasPermission();
			}
			return base.HasPermission();
		}

		protected override void OnExecute()
		{
			base.OnExecute();
			bool flag = base.ResultPane.SelectedObjects.Count > 1;
			string objectName = flag ? this.GetBulkSelectionPropertySheetDisplayName() : this.GetSingleSelectionPropertySheetDisplayName();
			string dialogNamePrefix = flag ? "BulkEditingPropertySheet_" : "PropertySheet_";
			this.ShowPropertySheetDailog(Strings.SingleSelectionProperties(objectName), dialogNamePrefix, flag);
		}

		protected virtual bool SharePropertySheetDialog
		{
			get
			{
				return true;
			}
		}

		private Dictionary<SelectedObjects, Guid> SelectedObjectsDictionary
		{
			get
			{
				if (!this.SharePropertySheetDialog)
				{
					return this.PrivateSelectedObjectsDictionary;
				}
				return ShowSelectionPropertiesCommandAction.SharedSelectedObjectsDictionary;
			}
		}

		protected virtual string GetSingleSelectionPropertySheetDisplayName()
		{
			return base.ResultPane.SelectedName;
		}

		protected virtual string GetBulkSelectionPropertySheetDisplayName()
		{
			if (!string.IsNullOrEmpty(base.ResultPane.SelectedObjectDetailsType))
			{
				return base.ResultPane.SelectedObjectDetailsType;
			}
			return base.ResultPane.SelectedName;
		}

		private void ShowPropertySheetDailog(string propertySheetName, string dialogNamePrefix, bool bulkEditing)
		{
			SelectedObjects selectedComponents = new SelectedObjects(base.ResultPane.SelectedObjects);
			Guid value = Guid.Empty;
			if (this.SelectedObjectsDictionary.ContainsKey(selectedComponents))
			{
				value = this.SelectedObjectsDictionary[selectedComponents];
			}
			else
			{
				value = Guid.NewGuid();
				this.SelectedObjectsDictionary[selectedComponents] = value;
			}
			string text = dialogNamePrefix + value.ToString();
			if (!ExchangeForm.ActivateSingleInstanceForm(text))
			{
				ExchangePropertyPageControl[] array = bulkEditing ? this.OnGetBulkSelectionPropertyPageControls() : this.OnGetSingleSelectionPropertyPageControls();
				if (!bulkEditing)
				{
					List<ExchangePropertyPageControl> list = new List<ExchangePropertyPageControl>();
					foreach (ExchangePropertyPageControl exchangePropertyPageControl in array)
					{
						if (exchangePropertyPageControl.HasPermission())
						{
							list.Add(exchangePropertyPageControl);
						}
					}
					array = list.ToArray();
				}
				this.ApplyOptionsOnPage(array, bulkEditing);
				PropertySheetDialog propertySheetDialog = new PropertySheetDialog(propertySheetName, array);
				propertySheetDialog.Name = text;
				propertySheetDialog.HelpTopic = base.ResultPane.SelectionHelpTopic + "Property";
				propertySheetDialog.Closed += delegate(object param0, EventArgs param1)
				{
					if (this.SelectedObjectsDictionary.ContainsKey(selectedComponents))
					{
						this.SelectedObjectsDictionary.Remove(selectedComponents);
					}
				};
				propertySheetDialog.ShowModeless(base.ResultPane, null);
			}
		}

		private void ApplyOptionsOnPage(ExchangePropertyPageControl[] pages, bool bulkEditing)
		{
			if (pages != null && pages.Length > 0)
			{
				ArrayList arrayList = new ArrayList();
				DataContextFlags dataContextFlags = new DataContextFlags();
				dataContextFlags.SelectedObjectsCount = base.ResultPane.SelectedObjects.Count;
				dataContextFlags.SelectedObjectDetailsType = base.ResultPane.SelectedObjectDetailsType;
				for (int i = 0; i < pages.Length; i++)
				{
					DataContext context = pages[i].Context;
					if (context != null && !arrayList.Contains(context))
					{
						arrayList.Add(context);
						dataContextFlags.Pages.Add(pages[i]);
						context.DataSaved += delegate(object param0, EventArgs param1)
						{
							this.RefreshResultsThreadSafely(context);
						};
						if (context.RefreshOnSave == null)
						{
							context.RefreshOnSave = base.GetDefaultRefreshObject();
						}
					}
				}
				AutomatedDataHandler automatedDataHandler = pages[0].DataHandler as AutomatedDataHandler;
				if (automatedDataHandler != null)
				{
					automatedDataHandler.ReaderExecutionContextFactory = new MonadCommandExecutionContextForPropertyPageFactory();
					automatedDataHandler.SaverExecutionContextFactory = new MonadCommandExecutionContextForPropertyPageFactory();
				}
			}
		}

		protected virtual ExchangePropertyPageControl[] OnGetBulkSelectionPropertyPageControls()
		{
			return new ExchangePropertyPageControl[0];
		}

		protected virtual ExchangePropertyPageControl[] OnGetSingleSelectionPropertyPageControls()
		{
			return new ExchangePropertyPageControl[0];
		}

		private static Dictionary<SelectedObjects, Guid> SharedSelectedObjectsDictionary = new Dictionary<SelectedObjects, Guid>();

		private Dictionary<SelectedObjects, Guid> PrivateSelectedObjectsDictionary = new Dictionary<SelectedObjects, Guid>();
	}
}
