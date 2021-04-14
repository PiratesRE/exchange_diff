using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ObjectResolver
	{
		public ObjectResolver(ObjectPicker objectPicker)
		{
			this.loader = objectPicker.CreateDataTableLoaderForResolver();
			this.loader.RefreshCompleted += this.loader_RefreshCompleted;
		}

		[DefaultValue(false)]
		public bool IsResolving
		{
			get
			{
				return this.isResolving;
			}
			set
			{
				this.isResolving = value;
				this.OnIsResolvingChanged(EventArgs.Empty);
			}
		}

		public DataTable ResolvedObjects
		{
			get
			{
				return this.loader.Table;
			}
		}

		public RefreshableComponent Refresher
		{
			get
			{
				return this.loader;
			}
		}

		public void ResolveObjects(ADObjectId rootId, QueryFilter filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			ResultsLoaderProfile resultsLoaderProfile = this.loader.RefreshArgument as ResultsLoaderProfile;
			if (resultsLoaderProfile != null)
			{
				resultsLoaderProfile.Scope = rootId;
				resultsLoaderProfile.InputValue("RecipientPreviewFilter", filter);
			}
			this.IsResolving = true;
			this.loader.Refresh(NullProgress.Value);
		}

		[DefaultValue(true)]
		public bool PrefillBeforeResolving
		{
			get
			{
				return this.prefillBeforeResolving;
			}
			set
			{
				if (value != this.prefillBeforeResolving)
				{
					this.prefillBeforeResolving = value;
					if (this.prefillBeforeResolving)
					{
						this.loader.Table.Columns.Add("LoadStatusColumn", typeof(ItemLoadStatus));
						return;
					}
					this.loader.Table.Columns.Remove("LoadStatusColumn");
				}
			}
		}

		[DefaultValue(false)]
		public bool FastResolving { get; set; }

		private void UpdateNonResolvedObjects(DataTable table)
		{
			foreach (object obj in table.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				if (!DBNull.Value.Equals(dataRow["LoadStatusColumn"]))
				{
					this.SetColumnValue("LoadStatusColumn", ItemLoadStatus.Failed, dataRow);
				}
			}
		}

		private PreFillADObjectIdFiller CreatePreFiller(ICollection idCollection)
		{
			DataTable dataTable = this.loader.Table.Clone();
			dataTable.BeginLoadData();
			foreach (object obj in idCollection)
			{
				ADObjectId adobjectId = (ADObjectId)obj;
				DataRow row = dataTable.NewRow();
				this.SetColumnValue("Identity", adobjectId, row);
				this.SetColumnValue("Guid", adobjectId.ObjectGuid, row);
				this.SetColumnValue("Name", adobjectId.Name, row);
				this.SetColumnValue("LoadStatusColumn", ItemLoadStatus.Loading, row);
				dataTable.Rows.Add(row);
			}
			dataTable.EndLoadData();
			return new PreFillADObjectIdFiller(dataTable);
		}

		private void SetColumnValue(string columnName, object value, DataRow row)
		{
			if (!string.IsNullOrEmpty(columnName) && row.Table.Columns.Contains(columnName))
			{
				row[columnName] = value;
			}
		}

		internal void ResolveObjectIds(ADPropertyDefinition property, ICollection idCollection)
		{
			if (property == null)
			{
				throw new ArgumentOutOfRangeException("property", "property should not be null.");
			}
			if (idCollection == null)
			{
				throw new ArgumentOutOfRangeException("idCollection", "idCollection should not be null.");
			}
			if (idCollection.Count == 0)
			{
				throw new ArgumentOutOfRangeException("idCollection", "idCollection should not be empty.");
			}
			ISupportResolvingIds supportResolvingIds = this.loader as ISupportResolvingIds;
			if (supportResolvingIds != null)
			{
				supportResolvingIds.IdentitiesToResolve = new ArrayList(idCollection);
				supportResolvingIds.PropertyForResolving = property;
			}
			ResultsLoaderProfile profile = this.loader.RefreshArgument as ResultsLoaderProfile;
			if (profile != null)
			{
				profile.PipelineObjects = new ArrayList(idCollection).ToArray();
				profile.IsResolving = true;
				profile.ResolveProperty = property.Name;
				profile.SearchText = string.Empty;
				profile.Scope = null;
				if (this.PrefillBeforeResolving)
				{
					if (this.CanApplyPrefill(profile))
					{
						if (this.FastResolving)
						{
							profile.ClearFiller();
						}
						AbstractDataTableFiller preFiller = this.CreatePreFiller(idCollection);
						profile.InsertTableFiller(0, preFiller);
						this.ResolveObjectIdsCompleted += delegate(object param0, RunWorkerCompletedEventArgs param1)
						{
							this.UpdateNonResolvedObjects(this.loader.Table);
							profile.RemoveTableFiller(preFiller);
						};
					}
					else
					{
						this.PrefillBeforeResolving = false;
					}
				}
			}
			this.IsResolving = true;
			this.loader.Refresh(NullProgress.Value);
		}

		private bool CanApplyPrefill(ResultsLoaderProfile profile)
		{
			bool flag;
			if (profile.PipelineObjects.All((object obj) => obj is ADObjectId))
			{
				flag = (from filler in profile.TableFillers
				select filler.CommandBuilder).All((ICommandBuilder builder) => builder is ExchangeCommandBuilder && (builder as ExchangeCommandBuilder).resolveForIdentity() && !(builder as ExchangeCommandBuilder).UseFilterToResolveNonId);
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			List<string> supportedPrimaryKeys = new List<string>
			{
				"Identity",
				"Guid",
				"Name"
			};
			bool flag3 = profile.DataTable.PrimaryKey.All((DataColumn column) => supportedPrimaryKeys.Contains(column.ColumnName));
			return flag2 && flag3;
		}

		private void loader_RefreshCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (!this.loader.Refreshing)
			{
				this.IsResolving = false;
				this.OnResolveObjectIdsCompleted(e);
			}
		}

		protected virtual void OnResolveObjectIdsCompleted(RunWorkerCompletedEventArgs e)
		{
			RunWorkerCompletedEventHandler resolveObjectIdsCompleted = this.ResolveObjectIdsCompleted;
			if (resolveObjectIdsCompleted != null)
			{
				resolveObjectIdsCompleted(this, e);
			}
		}

		public event RunWorkerCompletedEventHandler ResolveObjectIdsCompleted;

		protected virtual void OnIsResolvingChanged(EventArgs e)
		{
			EventHandler isResolvingChanged = this.IsResolvingChanged;
			if (isResolvingChanged != null)
			{
				isResolvingChanged(this, e);
			}
		}

		public event EventHandler IsResolvingChanged;

		public const string LoadStatusColumn = "LoadStatusColumn";

		private DataTableLoader loader;

		private bool isResolving;

		private bool prefillBeforeResolving;
	}
}
