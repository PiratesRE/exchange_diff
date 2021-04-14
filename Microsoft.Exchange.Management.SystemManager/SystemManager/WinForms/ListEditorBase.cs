using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.Commands;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class ListEditorBase<TObject> : DataListControl
	{
		protected Dictionary<TObject, string> ChangedObjects
		{
			get
			{
				return this.changedObjects;
			}
		}

		public ListEditorBase()
		{
			this.InitializeComponent();
			base.DataListView.VirtualMode = true;
			this.addObjectCommand = new Command();
			this.addObjectCommand.Name = "addObject";
			this.addObjectCommand.Description = Strings.AddObjectDescription;
			this.addObjectCommand.Execute += this.addObjectCommand_Execute;
			this.addObjectCommand.Icon = Icons.Add;
			base.DataListView.RemoveCommand.Executing += this.RemoveCommand_Executing;
			base.DataListView.DataSource = null;
			this.addObjectToolStripItem = new CommandToolStripButton(this.addObjectCommand);
			this.addObjectToolStripItem.Visible = false;
			this.removeObjectToolStripItem = new CommandToolStripButton(base.DataListView.RemoveCommand);
			this.removeObjectToolStripItem.Visible = false;
			base.ToolStripItems.Add(this.addObjectToolStripItem);
			base.ToolStripItems.Add(this.removeObjectToolStripItem);
			this.deleteContextmenu = new CommandMenuItem(base.DataListView.RemoveCommand, base.Components);
			this.deleteContextmenu.Visible = false;
			base.DataListView.ContextMenu.MenuItems.Add(this.deleteContextmenu);
			base.DataListView.AutoGenerateColumns = false;
			base.DataListView.AllowRemove = true;
			base.DataListView.IconLibrary = ObjectPicker.ObjectClassIconLibrary;
			this.AddButtonText = Strings.AddObject;
			this.RemoveButtonText = Strings.ListEditRemove;
		}

		private void RemoveCommand_Executing(object sender, CancelEventArgs e)
		{
			this.ExtractChangedObjects(base.DataListView.SelectedObjects);
			this.RemoveFromIdentityList(this.changedObjects.Keys);
		}

		private void addObjectCommand_Execute(object sender, EventArgs e)
		{
			ObjectPicker objectPicker = this.ObjectPickerForEdit ?? this.ObjectPicker;
			if (objectPicker.ShowDialog() == DialogResult.OK && !this.IsResolving)
			{
				if (base.DataListView.DataSource is DataTable)
				{
					DataTable dataTable = (DataTable)base.DataListView.DataSource;
					dataTable.Merge(objectPicker.SelectedObjects);
				}
				else
				{
					base.DataListView.DataSource = objectPicker.SelectedObjects.Copy();
				}
				this.ExtractChangedObjects(objectPicker.SelectedObjects.Rows);
				this.AddToIdentityList(this.changedObjects.Keys);
			}
		}

		private void ExtractChangedObjects(ICollection changedRowList)
		{
			this.changedObjects.Clear();
			foreach (object obj in changedRowList)
			{
				if (obj is DataRow || obj is DataRowView)
				{
					DataRow dataRow = (obj is DataRow) ? ((DataRow)obj) : ((DataRowView)obj).Row;
					this.InsertChangedObject(dataRow);
				}
			}
		}

		protected abstract void InsertChangedObject(DataRow dataRow);

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string AddButtonText
		{
			get
			{
				return this.addObjectCommand.Text;
			}
			set
			{
				this.addObjectCommand.Text = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string RemoveButtonText
		{
			get
			{
				return base.DataListView.RemoveCommand.Text;
			}
			set
			{
				base.DataListView.RemoveCommand.Text = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		[Browsable(false)]
		public ObjectPicker ObjectPicker
		{
			get
			{
				return this.objectPicker;
			}
			set
			{
				if (this.objectPicker != value)
				{
					this.objectPicker = value;
					if (this.objectPicker != null)
					{
						this.objectPicker.AllowMultiSelect = true;
					}
					this.addObjectToolStripItem.Enabled = (this.objectPicker != null);
					if (this.objectResolver != null)
					{
						this.objectResolver.ResolveObjectIdsCompleted -= this.objectResolver_ResolveObjectIdsCompleted;
						this.objectResolver.IsResolvingChanged -= this.objectResolver_IsResolvingChanged;
					}
					this.objectResolver = new ObjectResolver(this.objectPicker)
					{
						PrefillBeforeResolving = true
					};
					this.objectResolver.ResolveObjectIdsCompleted += this.objectResolver_ResolveObjectIdsCompleted;
					this.objectResolver.IsResolvingChanged += this.objectResolver_IsResolvingChanged;
					base.DataListView.DataSourceRefresher = this.objectResolver.Refresher;
					base.DataListView.IdentityProperty = ((this.ObjectPicker != null) ? this.ObjectPicker.IdentityProperty : null);
					base.DataListView.ImagePropertyName = ((this.ObjectPicker != null) ? this.ObjectPicker.ImageProperty : null);
					base.DataListView.DataSource = this.objectResolver.ResolvedObjects;
					if (string.IsNullOrEmpty(base.DataListView.SortProperty) && this.ObjectPicker != null)
					{
						base.DataListView.SortProperty = this.ObjectPicker.DefaultSortProperty;
					}
				}
			}
		}

		private void objectResolver_IsResolvingChanged(object sender, EventArgs e)
		{
			this.UpdateToolStripButtonStatus();
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[DefaultValue(null)]
		public ObjectPicker ObjectPickerForEdit
		{
			get
			{
				return this.secondPickerForEdit;
			}
			set
			{
				this.secondPickerForEdit = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DefaultValue(false)]
		public bool Editable
		{
			get
			{
				return this.isEditable;
			}
			set
			{
				if (value != this.isEditable)
				{
					this.isEditable = value;
					this.UpdateToolStripButtonStatus();
				}
			}
		}

		[DefaultValue(false)]
		public bool ReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				if (value != this.isReadOnly)
				{
					this.isReadOnly = value;
					this.UpdateToolStripButtonStatus();
				}
			}
		}

		[DefaultValue(true)]
		public bool ImageShown
		{
			get
			{
				return this.imageShown;
			}
			set
			{
				if (value != this.imageShown)
				{
					this.imageShown = value;
					base.DataListView.IconLibrary = (value ? ObjectPicker.ObjectClassIconLibrary : null);
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal ADPropertyDefinition ObjectFilterProperty
		{
			get
			{
				return this.objectFilterProperty;
			}
			set
			{
				this.objectFilterProperty = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		[Browsable(false)]
		public object ObjectFilterTarget
		{
			get
			{
				return this.objectFilterTarget;
			}
			set
			{
				if (this.ObjectFilterTarget != value)
				{
					this.objectFilterTarget = value;
					this.OnObjectFilterTargetChanged();
					this.ResolveObjectList();
				}
			}
		}

		private void OnObjectFilterTargetChanged()
		{
			EventHandler eventHandler = (EventHandler)base.Events[ListEditorBase<TObject>.EventObjectFilterTargetChanged];
			if (eventHandler != null)
			{
				eventHandler(this, EventArgs.Empty);
			}
		}

		public event EventHandler ObjectFilterTargetChanged
		{
			add
			{
				base.Events.AddHandler(ListEditorBase<TObject>.EventObjectFilterTargetChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(ListEditorBase<TObject>.EventObjectFilterTargetChanged, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public MultiValuedProperty<TObject> IdentityList
		{
			get
			{
				return this.identityMvpList;
			}
			set
			{
				this.identityMvpList = value;
			}
		}

		protected virtual void AddToIdentityList(ICollection identities)
		{
			this.UpdateIdentityList(identities, false);
		}

		protected virtual void RemoveFromIdentityList(ICollection identities)
		{
			this.UpdateIdentityList(identities, true);
		}

		private void UpdateIdentityList(ICollection identities, bool remove)
		{
			if (identities != null && identities.Count > 0)
			{
				if (this.IdentityList == null)
				{
					this.IdentityList = new MultiValuedProperty<TObject>();
				}
				base.NotifyExposedPropertyIsModified();
				foreach (object obj in identities)
				{
					TObject item = (TObject)((object)obj);
					try
					{
						if (remove)
						{
							this.IdentityList.Remove(item);
						}
						else
						{
							this.IdentityList.Add(item);
						}
					}
					catch (InvalidOperationException)
					{
					}
				}
				this.OnIdentityListChanged(new IdentityListChangedEventArgs<TObject>(remove ? 1 : 0, this.ChangedObjects));
			}
		}

		protected virtual void OnIdentityListChanged(IdentityListChangedEventArgs<TObject> e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ListEditorBase<TObject>.EventIdentityListChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IdentityListChanged
		{
			add
			{
				base.Events.AddHandler(ListEditorBase<TObject>.EventIdentityListChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(ListEditorBase<TObject>.EventIdentityListChanged, value);
			}
		}

		private void ResolveObjectList()
		{
			if (this.ObjectFilterTarget != null && DBNull.Value != this.ObjectFilterTarget && !this.IsResolving)
			{
				ICollection collection = null;
				if (this.ObjectFilterTarget is TObject)
				{
					collection = new TObject[]
					{
						(TObject)((object)this.ObjectFilterTarget)
					};
				}
				else if (this.ObjectFilterTarget is MultiValuedProperty<TObject>)
				{
					MultiValuedProperty<TObject> multiValuedProperty = (MultiValuedProperty<TObject>)this.ObjectFilterTarget;
					if (multiValuedProperty.Count > 0)
					{
						collection = multiValuedProperty;
					}
				}
				if (collection != null)
				{
					if (base.DataListView.AvailableColumns.Count > 1)
					{
						base.DataListView.StatusPropertyName = "LoadStatusColumn";
					}
					else if (!this.FastResolving)
					{
						this.objectResolver.PrefillBeforeResolving = false;
					}
					this.objectResolver.ResolvedObjects.Rows.Clear();
					this.objectResolver.ResolveObjectIds(this.ObjectFilterProperty, collection);
					base.DataListView.DataSource = this.objectResolver.ResolvedObjects;
					return;
				}
				base.DataListView.DataSource = null;
			}
		}

		public bool PrefillBeforeResolving
		{
			get
			{
				return this.objectResolver.PrefillBeforeResolving;
			}
			set
			{
				this.objectResolver.PrefillBeforeResolving = value;
			}
		}

		public bool FastResolving
		{
			get
			{
				return this.objectResolver.FastResolving;
			}
			set
			{
				this.objectResolver.FastResolving = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool IsResolving
		{
			get
			{
				return this.objectResolver != null && this.objectResolver.IsResolving;
			}
		}

		public void LoadObjects(ADObjectId containerId, QueryFilter filter)
		{
			if (!this.IsResolving)
			{
				this.objectResolver.ResolvedObjects.Rows.Clear();
				this.objectResolver.ResolveObjects(containerId, filter);
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			this.UpdateToolStripButtonStatus();
		}

		private void UpdateToolStripButtonStatus()
		{
			if (base.IsHandleCreated)
			{
				base.DataListView.AllowRemove = (this.Editable && !this.ReadOnly && !this.IsResolving);
				if (this.addObjectToolStripItem != null)
				{
					this.addObjectToolStripItem.Visible = this.Editable;
					this.addObjectToolStripItem.Enabled = (!this.ReadOnly && !this.IsResolving);
				}
				if (this.removeObjectToolStripItem != null)
				{
					this.removeObjectToolStripItem.Visible = this.Editable;
					this.removeObjectToolStripItem.Enabled = (!this.ReadOnly && !this.IsResolving && base.DataListView.RemoveCommand.Enabled);
				}
				if (this.deleteContextmenu != null)
				{
					this.deleteContextmenu.Visible = this.Editable;
					this.deleteContextmenu.Enabled = this.removeObjectToolStripItem.Enabled;
				}
			}
		}

		private void objectResolver_ResolveObjectIdsCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				base.ShowError(Strings.ErrorNotAllObjectsLoaded(e.Error.Message));
			}
			this.ExtractChangedObjects(this.objectResolver.ResolvedObjects.Rows);
			this.TrackResolvedObjects(this.ChangedObjects.Keys);
			base.DataListView.BackupItemsStates();
			base.DataListView.RestoreItemsStates(false);
			this.OnResolveCompleted(EventArgs.Empty);
		}

		protected virtual void TrackResolvedObjects(ICollection identities)
		{
		}

		protected virtual void OnResolveCompleted(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[ListEditorBase<TObject>.EventResolveCompleted];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler ResolveCompleted
		{
			add
			{
				base.Events.AddHandler(ListEditorBase<TObject>.EventResolveCompleted, value);
			}
			remove
			{
				base.Events.RemoveHandler(ListEditorBase<TObject>.EventResolveCompleted, value);
			}
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new SizeF(8f, 16f);
			base.Name = "ListEditorBase";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.objectResolver != null)
			{
				this.objectResolver.ResolveObjectIdsCompleted -= this.objectResolver_ResolveObjectIdsCompleted;
				this.objectResolver.IsResolvingChanged -= this.objectResolver_IsResolvingChanged;
				this.objectResolver.Refresher.CancelRefresh();
			}
			base.Dispose(disposing);
		}

		internal DataTable ResolvedObjects
		{
			get
			{
				return this.objectResolver.ResolvedObjects;
			}
		}

		public override Dictionary<string, HashSet<Control>> ExposedPropertyRelatedControls
		{
			get
			{
				Dictionary<string, HashSet<Control>> exposedPropertyRelatedControls = base.ExposedPropertyRelatedControls;
				if (!exposedPropertyRelatedControls.ContainsKey("ObjectFilterTarget"))
				{
					exposedPropertyRelatedControls.Add("ObjectFilterTarget", base.GetChildControls());
				}
				return exposedPropertyRelatedControls;
			}
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "IdentityList";
			}
		}

		private Command addObjectCommand;

		private ObjectPicker objectPicker;

		private ObjectPicker secondPickerForEdit;

		private ObjectResolver objectResolver;

		private CommandToolStripButton addObjectToolStripItem;

		private CommandToolStripButton removeObjectToolStripItem;

		private CommandMenuItem deleteContextmenu;

		private Dictionary<TObject, string> changedObjects = new Dictionary<TObject, string>();

		private bool isEditable;

		private bool isReadOnly;

		private bool imageShown = true;

		private ADPropertyDefinition objectFilterProperty = ADObjectSchema.Id;

		private object objectFilterTarget;

		private static readonly object EventObjectFilterTargetChanged = new object();

		private MultiValuedProperty<TObject> identityMvpList = new MultiValuedProperty<TObject>();

		private static readonly object EventIdentityListChanged = new object();

		private static readonly object EventResolveCompleted = new object();
	}
}
