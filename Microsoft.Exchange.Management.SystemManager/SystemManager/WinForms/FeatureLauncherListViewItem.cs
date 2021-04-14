using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Serializable]
	public class FeatureLauncherListViewItem : ListViewItem
	{
		protected FeatureLauncherListViewItem()
		{
			base.SubItems.Add(new ListViewItem.ListViewSubItem(this, LocalizedDescriptionAttribute.FromEnum(typeof(FeatureStatus), this.Status)));
			base.UseItemStyleForSubItems = false;
		}

		public FeatureLauncherListViewItem(string featureName, string statusPropertyName, Icon icon) : this()
		{
			base.Text = featureName;
			base.Name = featureName;
			this.statusPropertyName = statusPropertyName;
			this.icon = icon;
		}

		public string FeatureName
		{
			get
			{
				return base.Name;
			}
		}

		public string StatusPropertyName
		{
			get
			{
				return this.statusPropertyName;
			}
		}

		public Icon Icon
		{
			get
			{
				return this.icon;
			}
		}

		[DefaultValue(FeatureStatus.None)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FeatureStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				if (value != this.status)
				{
					this.status = value;
					this.StatusText = LocalizedDescriptionAttribute.FromEnum(typeof(FeatureStatus), value);
					this.OnStatusChanged(EventArgs.Empty);
				}
			}
		}

		internal void FireStatusChanged()
		{
			this.OnStatusChanged(EventArgs.Empty);
		}

		public event EventHandler StatusChanged;

		protected virtual void OnStatusChanged(EventArgs e)
		{
			if (this.StatusChanged != null)
			{
				this.StatusChanged(this, e);
			}
		}

		[DefaultValue(DataSourceUpdateMode.OnPropertyChanged)]
		public DataSourceUpdateMode DataSourceUpdateMode
		{
			get
			{
				return this.dataSourceUpdateMode;
			}
			set
			{
				this.dataSourceUpdateMode = value;
			}
		}

		private string StatusText
		{
			get
			{
				return base.SubItems[1].Text;
			}
			set
			{
				base.SubItems[1].Text = value;
			}
		}

		[DefaultValue("")]
		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				if (value != this.Description)
				{
					this.description = value;
				}
			}
		}

		[DefaultValue(null)]
		public Type PropertyPageControl
		{
			get
			{
				return this.propertyPageControl;
			}
			set
			{
				if (!(null == value) && !typeof(ExchangePropertyPageControl).IsAssignableFrom(value))
				{
					throw new ArgumentOutOfRangeException("value", "The PropertyPageControl must derive from ExchangePropertyPageControl");
				}
				if (value != this.PropertyPageControl)
				{
					this.propertyPageControl = value;
					return;
				}
			}
		}

		public bool CanChangeStatus
		{
			get
			{
				return this.canChangeStatus;
			}
			set
			{
				this.canChangeStatus = value;
			}
		}

		public bool EnablePropertiesButtonOnFeatureStatus
		{
			get
			{
				return this.enablePropertiesButtonOnFeatureStatus;
			}
			set
			{
				this.enablePropertiesButtonOnFeatureStatus = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(false)]
		public bool BulkEditing
		{
			get
			{
				return this.bulkEditing;
			}
			set
			{
				if (this.bulkEditing != value)
				{
					this.bulkEditing = value;
					if (this.BulkEditing)
					{
						this.StatusText = Strings.BulkEditingDefaultValueForFeatureItem;
						return;
					}
					this.StatusText = LocalizedDescriptionAttribute.FromEnum(typeof(FeatureStatus), this.Status);
				}
			}
		}

		public string UniqueName
		{
			get
			{
				if (string.IsNullOrEmpty(this.uniqueName))
				{
					this.uniqueName = (string.IsNullOrEmpty(this.StatusPropertyName) ? Guid.NewGuid().ToString() : this.StatusPropertyName);
				}
				return this.uniqueName;
			}
		}

		public string StatusBindingName
		{
			get
			{
				return string.Format("{0}_{1}", this.UniqueName, "Status");
			}
		}

		public bool IsBanned { get; set; }

		public bool IsLocked { get; set; }

		public string BannedMessage { get; set; }

		private FeatureStatus status;

		private string description = string.Empty;

		private Type propertyPageControl;

		private string statusPropertyName;

		private Icon icon;

		private bool canChangeStatus = true;

		private bool enablePropertiesButtonOnFeatureStatus;

		private DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;

		private bool bulkEditing;

		private string uniqueName;
	}
}
