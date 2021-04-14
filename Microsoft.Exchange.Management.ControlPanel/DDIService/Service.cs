using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Markup;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.DDIService
{
	[ContentProperty("Workflows")]
	public class Service : ICloneable
	{
		static Service()
		{
			ExpressionParser.RemovePredefinedTypes(typeof(Microsoft.Exchange.ManagementGUI.Resources.Strings));
			ExpressionParser.EnrolPredefinedTypes(typeof(Microsoft.Exchange.Management.ControlPanel.Strings));
			ExpressionParser.EnrolPredefinedTypes(typeof(DDIHelper));
			ExpressionParser.EnrolPredefinedTypes(typeof(DDIUtil));
		}

		public object Clone()
		{
			Service service = new Service();
			service.Name = this.Name;
			service.LambdaExpressionHelper = this.LambdaExpressionHelper;
			service.Class = this.Class;
			service.Variables = new Collection<Variable>((from c in this.Variables
			select c.Clone() as Variable).ToList<Variable>());
			service.DataObjects = new Collection<DataObject>((from c in this.DataObjects
			select c.Clone() as DataObject).ToList<DataObject>());
			service.Workflows = new Collection<Workflow>((from c in this.Workflows
			select c.Clone()).ToList<Workflow>());
			service.Resources = new Dictionary<string, object>(this.Resources);
			return service;
		}

		public Service()
		{
			this.Resources = new Dictionary<string, object>();
		}

		public string Name { get; set; }

		public Type[] LambdaExpressionHelper
		{
			get
			{
				return this.lambdaExpressionHelper;
			}
			set
			{
				if (value != this.lambdaExpressionHelper)
				{
					this.lambdaExpressionHelper = value;
				}
			}
		}

		[TypeConverter(typeof(DDIObjectTypeConverter))]
		public Type Class
		{
			get
			{
				return this.classType;
			}
			set
			{
				if (this.classType != value)
				{
					this.classType = value;
				}
			}
		}

		[DDIMandatoryValue]
		[DDINoDuplication(PropertyName = "Name")]
		public Collection<Variable> Variables
		{
			get
			{
				return this.variables;
			}
			set
			{
				this.variables = value;
			}
		}

		[DDINoDuplication(PropertyName = "Name")]
		public Collection<DataObject> DataObjects
		{
			get
			{
				return this.dataObjects;
			}
			set
			{
				this.dataObjects = value;
			}
		}

		[DDIMandatoryValue]
		[DDINoDuplication(PropertyName = "Name")]
		public Collection<Workflow> Workflows
		{
			get
			{
				return this.workflows;
			}
			set
			{
				this.workflows = value;
			}
		}

		internal List<Type> PredefinedTypes
		{
			get
			{
				if (this.predefinedTypes == null)
				{
					this.predefinedTypes = new List<Type>();
					if (this.LambdaExpressionHelper != null)
					{
						this.predefinedTypes.AddRange(this.LambdaExpressionHelper);
					}
					if (null != this.Class)
					{
						this.predefinedTypes.Add(this.Class);
					}
				}
				return this.predefinedTypes;
			}
		}

		public virtual void Initialize()
		{
			this.Variables.Add(new Variable
			{
				Name = "ShouldContinue",
				Type = typeof(bool)
			});
			this.Variables.Add(new Variable
			{
				Name = "LastErrorContext",
				Type = typeof(ErrorRecordContext)
			});
			List<Workflow> list = new List<Workflow>();
			IEnumerable<Workflow> source = from c in this.workflows
			where c is GetObjectForListWorkflow
			select c;
			Workflow workflow = source.FirstOrDefault<Workflow>();
			IEnumerable<Workflow> source2 = from c in this.workflows
			where c is GetObjectWorkflow
			select c;
			Workflow workflow2 = source2.FirstOrDefault<Workflow>();
			bool flag = source2.Count<Workflow>() > 1;
			list.AddRange(from c in this.workflows
			where c is ICallGetAfterExecuteWorkflow && !((ICallGetAfterExecuteWorkflow)c).IgnoreGetObject
			select c);
			if (workflow2 != null)
			{
				foreach (Workflow workflow3 in list)
				{
					List<Activity> list2 = new List<Activity>(workflow3.Activities);
					if (flag)
					{
						list2.AddRange(workflow.Activities);
					}
					else
					{
						list2.AddRange(workflow2.Activities);
					}
					workflow3.Activities = new Collection<Activity>(list2);
				}
			}
		}

		public List<DataColumn> ExtendedColumns
		{
			get
			{
				List<DataColumn> list = new List<DataColumn>();
				foreach (Workflow workflow in this.workflows)
				{
					foreach (Activity activity in workflow.Activities)
					{
						list.AddRange(activity.GetExtendedColumns());
					}
				}
				return list;
			}
		}

		public IDictionary<string, object> Resources { get; private set; }

		private Type[] lambdaExpressionHelper;

		private Collection<Variable> variables = new Collection<Variable>();

		private Collection<DataObject> dataObjects = new Collection<DataObject>();

		private Collection<Workflow> workflows = new Collection<Workflow>();

		private List<Type> predefinedTypes;

		private Type classType;
	}
}
