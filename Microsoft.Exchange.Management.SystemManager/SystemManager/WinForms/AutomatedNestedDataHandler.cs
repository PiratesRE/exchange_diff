using System;
using System.Data;
using System.Reflection;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class AutomatedNestedDataHandler : AutomatedDataHandlerBase
	{
		internal DataContext ParentContext
		{
			get
			{
				return this.parentContext;
			}
		}

		public static AutomatedNestedDataHandler CreateDataHandlerWithParentSchema(DataContext parentContext)
		{
			return new AutomatedNestedDataHandler(parentContext, (parentContext.DataHandler as AutomatedDataHandlerBase).Assembly, (parentContext.DataHandler as AutomatedDataHandlerBase).SchemaName);
		}

		public AutomatedNestedDataHandler(DataContext parentContext, Assembly assembly, string schema) : base(assembly, schema)
		{
			this.parentContext = parentContext;
			base.EnableBulkEdit = (parentContext.DataHandler as AutomatedDataHandlerBase).EnableBulkEdit;
		}

		internal override void OnReadData(CommandInteractionHandler interactionHandler, string pageName)
		{
			DataTable table = (DataTable)this.parentContext.DataHandler.DataSource;
			base.Table.Merge(table);
			base.DataObjectStore = ((AutomatedDataHandlerBase)this.parentContext.DataHandler).DataObjectStore.Clone();
			base.RefreshDataObjectStore();
			base.DataSource = base.Table;
		}

		internal override void OnSaveData(CommandInteractionHandler interactionHandler)
		{
			AutomatedDataHandlerBase automatedDataHandlerBase = this.parentContext.DataHandler as AutomatedDataHandlerBase;
			automatedDataHandlerBase.DataObjectStore = base.DataObjectStore;
			automatedDataHandlerBase.RefreshDataObjectStoreWithNewTable();
			this.parentContext.IsDirty = true;
		}

		private AutomatedDataHandler GetRootDataHandler(DataContext context)
		{
			if (context.DataHandler is AutomatedDataHandler)
			{
				return context.DataHandler as AutomatedDataHandler;
			}
			return this.GetRootDataHandler((context.DataHandler as AutomatedNestedDataHandler).ParentContext);
		}

		internal override bool HasViewPermissionForPage(string pageName)
		{
			return this.GetRootDataHandler(this.ParentContext).HasViewPermissionForPage(pageName);
		}

		internal override bool HasPermissionForProperty(string propertyName, bool canUpdate)
		{
			return this.GetRootDataHandler(this.ParentContext).HasPermissionForProperty(propertyName, canUpdate);
		}

		private DataContext parentContext;
	}
}
