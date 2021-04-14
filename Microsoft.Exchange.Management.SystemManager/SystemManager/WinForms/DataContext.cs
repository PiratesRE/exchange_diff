using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class DataContext
	{
		public DataContext(DataHandler dataHandler, bool bypassCorruptObjectVerification)
		{
			if (dataHandler == null)
			{
				throw new ArgumentNullException("dataHandler");
			}
			this.dataHandler = dataHandler;
			this.bypassCorruptObjectVerification = bypassCorruptObjectVerification;
		}

		public DataContext(DataHandler dataHandler) : this(dataHandler, false)
		{
		}

		internal List<ExchangePage> Pages
		{
			get
			{
				return this.pages;
			}
		}

		public DataHandler DataHandler
		{
			get
			{
				return this.dataHandler;
			}
		}

		private EventHandlerList Events
		{
			get
			{
				return this.events;
			}
		}

		[DefaultValue(false)]
		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
			set
			{
				if (this.IsDirty != value)
				{
					this.isDirty = value;
					this.OnIsDirtyChanged(EventArgs.Empty);
				}
			}
		}

		public bool IsSaving
		{
			get
			{
				return this.isAccessingData;
			}
		}

		private void OnIsDirtyChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)this.Events[DataContext.EventIsDirtyChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler IsDirtyChanged
		{
			add
			{
				SynchronizedDelegate.Combine(this.Events, DataContext.EventIsDirtyChanged, value);
			}
			remove
			{
				SynchronizedDelegate.Remove(this.Events, DataContext.EventIsDirtyChanged, value);
			}
		}

		public DataContextFlags Flags
		{
			get
			{
				return this.flags;
			}
			set
			{
				this.flags = value;
			}
		}

		internal object ReadData(CommandInteractionHandler interactionHandler, string pageName)
		{
			if (!this.needToReadData)
			{
				if (!(this.DataHandler is AutomatedDataHandler))
				{
					goto IL_53;
				}
			}
			try
			{
				this.isAccessingData = true;
				this.DataHandler.Read(interactionHandler, pageName);
				this.needToReadData = false;
				if (!this.bypassCorruptObjectVerification)
				{
					this.IsDataSourceCorrupted = this.dataHandler.IsCorrupted;
				}
			}
			finally
			{
				this.isAccessingData = false;
			}
			IL_53:
			return this.DataHandler.DataSource;
		}

		internal void SaveData(CommandInteractionHandler interactionHandler)
		{
			if (this.IsDirty)
			{
				try
				{
					this.isAccessingData = true;
					this.DataHandler.Save(interactionHandler);
					bool flag = this.DataHandler.HasWorkUnits && this.DataHandler.WorkUnits.HasFailures;
					if (!flag)
					{
						this.IsDirty = false;
						this.needToReadData = true;
					}
					if (!flag || this.DataHandler.WorkUnits.IsDataChanged)
					{
						this.OnDataSaved(EventArgs.Empty);
					}
				}
				finally
				{
					this.isAccessingData = false;
				}
			}
		}

		[DefaultValue(null)]
		public IRefreshable RefreshOnSave
		{
			get
			{
				return this.refreshOnSave;
			}
			set
			{
				this.refreshOnSave = value;
			}
		}

		private void OnDataSaved(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)this.Events[DataContext.EventDataSaved];
			if (eventHandler != null)
			{
				eventHandler(this, EventArgs.Empty);
			}
		}

		public event EventHandler DataSaved
		{
			add
			{
				SynchronizedDelegate.Combine(this.Events, DataContext.EventDataSaved, value);
			}
			remove
			{
				SynchronizedDelegate.Remove(this.Events, DataContext.EventDataSaved, value);
			}
		}

		public ValidationError[] Validate()
		{
			return this.DataHandler.Validate();
		}

		public ValidationError[] ValidateOnly(object objectToBeValidated)
		{
			return this.DataHandler.ValidateOnly(objectToBeValidated);
		}

		public void OverrideCorruptedValuesWithDefault()
		{
			this.IsDirty = this.DataHandler.OverrideCorruptedValuesWithDefault();
			this.IsDataSourceCorrupted = false;
		}

		public bool IsDataSourceCorrupted
		{
			get
			{
				return this.isDataSourceCorrupted;
			}
			private set
			{
				this.isDataSourceCorrupted = value;
			}
		}

		internal void AllowNextRead()
		{
			this.needToReadData = true;
		}

		public string ModifiedParametersDescription
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.DataHandler.ModifiedParametersDescription);
				return stringBuilder.ToString();
			}
		}

		public int SelectedObjectsCount
		{
			get
			{
				return this.Flags.SelectedObjectsCount;
			}
		}

		public string SelectedObjectDetailsType
		{
			get
			{
				return this.Flags.SelectedObjectDetailsType;
			}
		}

		private DataHandler dataHandler;

		private bool isDirty;

		private bool isAccessingData;

		private EventHandlerList events = new EventHandlerList();

		private static readonly object EventIsDirtyChanged = new object();

		private static readonly object EventDataSaved = new object();

		private List<ExchangePage> pages = new List<ExchangePage>();

		private DataContextFlags flags = new DataContextFlags();

		private bool isDataSourceCorrupted;

		private bool bypassCorruptObjectVerification;

		private bool needToReadData = true;

		private IRefreshable refreshOnSave;
	}
}
