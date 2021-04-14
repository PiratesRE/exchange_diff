using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Markup;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	[DictionaryKeyProperty("Name")]
	public abstract class Activity
	{
		public Activity() : this(string.Empty)
		{
		}

		public Activity(string name)
		{
			this.Name = name;
			this.ProgressPercent = 0;
		}

		protected Activity(Activity activity) : this(activity.Name)
		{
			this.PreAction = activity.PreAction;
			this.PostAction = activity.PostAction;
			this.ErrorBehavior = activity.ErrorBehavior;
		}

		public virtual Activity Clone()
		{
			throw new InvalidOperationException("Activity is not allowed to clone!");
		}

		public string Name { get; set; }

		[DDIValidCodeBehindMethod]
		public string PreAction { get; set; }

		[DDIValidCodeBehindMethod]
		public string PostAction { get; set; }

		[DefaultValue(ErrorBehavior.Stop)]
		public ErrorBehavior ErrorBehavior { get; set; }

		internal IIsInRole RbacChecker
		{
			get
			{
				return this.rbacChecker ?? RbacCheckerWrapper.RbacChecker;
			}
			set
			{
				this.rbacChecker = value;
			}
		}

		internal IEacHttpContext EacHttpContext
		{
			get
			{
				return this.httpContext ?? Microsoft.Exchange.Management.DDIService.EacHttpContext.Instance;
			}
			set
			{
				this.httpContext = value;
			}
		}

		internal IPSCommandWrapperFactory PowershellFactory
		{
			get
			{
				return this.powershellFactory ?? PSCommandWrapperFactory.Instance;
			}
			set
			{
				this.powershellFactory = value;
			}
		}

		public virtual List<DataColumn> GetExtendedColumns()
		{
			return new List<DataColumn>();
		}

		public RunResult RunCore(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			this.ProgressPercent = 0;
			this.DoPreRun(input, dataTable, store, codeBehind);
			RunResult result = this.Run(input, dataTable, store, codeBehind, updateTableDelegate);
			this.DoPostRun(input, dataTable, store, codeBehind);
			this.ProgressPercent = 100;
			return result;
		}

		public abstract RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate);

		public virtual bool IsRunnable(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			return !AsyncServiceManager.IsCurrentWorkCancelled() && this.HasPermission(input, dataTable, store, null);
		}

		public virtual PowerShellResults[] GetStatusReport(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			return new PowerShellResults[0];
		}

		internal virtual bool HasPermission(DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			return true;
		}

		internal virtual bool HasOutputVariable(string variable)
		{
			return false;
		}

		internal virtual IEnumerable<Activity> Find(Func<Activity, bool> predicate)
		{
			List<Activity> source = new List<Activity>
			{
				this
			};
			return source.Where(predicate);
		}

		internal virtual bool? FindAndCheckPermission(Func<Activity, bool> predicate, DataRow input, DataTable dataTable, DataObjectStore store, Variable updatingVariable)
		{
			bool? result = null;
			if (predicate(this))
			{
				result = new bool?(this.HasPermission(input, dataTable, store, updatingVariable));
			}
			return result;
		}

		internal void DoPostRun(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
			if (null != codeBehind && !string.IsNullOrEmpty(this.PostAction))
			{
				DDIHelper.Trace("PostAction: " + this.PostAction);
				MethodInfo method = codeBehind.GetMethod(this.PostAction, new Type[]
				{
					typeof(DataRow),
					typeof(DataTable),
					typeof(DataObjectStore)
				});
				if (method != null)
				{
					method.Invoke(null, new object[]
					{
						input,
						dataTable,
						store
					});
				}
				else
				{
					method = codeBehind.GetMethod(this.PostAction, new Type[]
					{
						typeof(DataRow),
						typeof(DataTable),
						typeof(DataObjectStore),
						typeof(PowerShellResults[])
					});
					if (!(method != null))
					{
						throw new NotImplementedException("The specified method " + this.PostAction + " should implement one of the two signatures: (DataRow, DataTable, DataObjectStore) or (DataRow, DataTable, DataObjectStore, PowerShellResults[]) .");
					}
					method.Invoke(null, new object[]
					{
						input,
						dataTable,
						store,
						this.GetStatusReport(input, dataTable, store)
					});
				}
			}
			this.DoPostRunCore(input, dataTable, store, codeBehind);
		}

		internal void DoPreRun(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
			if (null != codeBehind && !string.IsNullOrEmpty(this.PreAction))
			{
				DDIHelper.Trace("PreAction: " + this.PreAction);
				codeBehind.GetMethod(this.PreAction).Invoke(null, new object[]
				{
					input,
					dataTable,
					store
				});
			}
			this.DoPreRunCore(input, dataTable, store, codeBehind);
		}

		protected virtual void DoPostRunCore(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
		}

		protected virtual void DoPreRunCore(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
		}

		internal virtual int ProgressPercent { get; set; }

		internal virtual void SetResultSize(int resultSize)
		{
		}

		private IEacHttpContext httpContext;

		private IIsInRole rbacChecker;

		private IPSCommandWrapperFactory powershellFactory;
	}
}
