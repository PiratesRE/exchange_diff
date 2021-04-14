using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class FeatureLauncherBulkEditorAdapter : BulkEditorAdapter
	{
		public FeatureLauncherBulkEditorAdapter(FeatureLauncherPropertyControl featureLauncherControl) : base(featureLauncherControl)
		{
			((IFeatureLauncherBulkEditSupport)featureLauncherControl).FeatureItemUpdated += this.OnFeatureItemUpdated;
		}

		private void OnFeatureItemUpdated(object sender, EventArgs e)
		{
			this.UpdateFeatureItem(null);
		}

		protected override IList<string> InnerGetManagedProperties()
		{
			IList<string> list = base.InnerGetManagedProperties();
			FeatureLauncherPropertyControl featureLauncherPropertyControl = base.HostControl as FeatureLauncherPropertyControl;
			foreach (object obj in featureLauncherPropertyControl.FeatureListView.Items)
			{
				FeatureLauncherListViewItem featureLauncherListViewItem = (FeatureLauncherListViewItem)obj;
				list.Add(featureLauncherListViewItem.StatusBindingName);
			}
			return list;
		}

		protected override void OnStateChanged(BulkEditorAdapter sender, BulkEditorStateEventArgs e)
		{
			base.OnStateChanged(sender, e);
			Control hostControl = base.HostControl;
			FeatureLauncherListViewItem featureLauncherListViewItem = this.FindItemByPropertyName(e.PropertyName);
			this.UpdateFeatureItem(featureLauncherListViewItem);
			if (featureLauncherListViewItem != null && base[e.PropertyName] == null)
			{
				featureLauncherListViewItem.FireStatusChanged();
			}
		}

		internal void UpdateFeatureItem(FeatureLauncherListViewItem featureItem)
		{
			FeatureLauncherPropertyControl featureLauncherPropertyControl = base.HostControl as FeatureLauncherPropertyControl;
			featureItem = (featureItem ?? (featureLauncherPropertyControl.FeatureListView.FirstSelectedItem as FeatureLauncherListViewItem));
			if (featureItem != null)
			{
				if (base[featureItem.StatusBindingName] == 3)
				{
					featureItem.IsLocked = true;
					if (object.ReferenceEquals(featureItem, featureLauncherPropertyControl.FeatureListView.FirstSelectedItem))
					{
						featureLauncherPropertyControl.PropertiesButton.Enabled = false;
						featureLauncherPropertyControl.EnableButton.Enabled = false;
						featureLauncherPropertyControl.DisableButton.Enabled = false;
					}
					featureLauncherPropertyControl.FeatureListView.DrawLockedIcon = true;
					featureLauncherPropertyControl.FeatureListView.Invalidate();
					return;
				}
				if (featureLauncherPropertyControl.EnablingButtonsVisible)
				{
					featureItem.BulkEditing = (base[featureItem.StatusBindingName] != 0);
					if (featureItem.BulkEditing && object.ReferenceEquals(featureItem, featureLauncherPropertyControl.FeatureListView.FirstSelectedItem))
					{
						featureLauncherPropertyControl.PropertiesButton.Enabled = (featureLauncherPropertyControl.PropertiesButton.Enabled && base[featureItem.StatusBindingName] != 2 && !featureItem.EnablePropertiesButtonOnFeatureStatus);
						bool flag = base[featureItem.StatusBindingName] != 2;
						flag = (flag && (featureLauncherPropertyControl.EnableButton.Enabled || featureLauncherPropertyControl.DisableButton.Enabled));
						featureLauncherPropertyControl.EnableButton.Enabled = flag;
						featureLauncherPropertyControl.DisableButton.Enabled = flag;
					}
				}
			}
		}

		private FeatureLauncherListViewItem FindItemByPropertyName(string propertyName)
		{
			FeatureLauncherListViewItem result = null;
			FeatureLauncherPropertyControl featureLauncherPropertyControl = base.HostControl as FeatureLauncherPropertyControl;
			foreach (object obj in featureLauncherPropertyControl.FeatureListView.Items)
			{
				FeatureLauncherListViewItem featureLauncherListViewItem = (FeatureLauncherListViewItem)obj;
				if (featureLauncherListViewItem.StatusBindingName.Equals(propertyName))
				{
					result = featureLauncherListViewItem;
					break;
				}
			}
			return result;
		}
	}
}
