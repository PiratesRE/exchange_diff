using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class BaseTask : ITask, ITaskDescriptor
	{
		public BaseTask(IContext context, LocalizedString title, LocalizedString description, TaskType type, params ContextProperty[] dependentProperties)
		{
			Util.ThrowOnNullArgument(context, "context");
			this.context = context;
			this.stateSpecificPropertyBag = this.context.Properties;
			this.title = title;
			this.description = description;
			this.type = type;
			this.result = TaskResult.Undefined;
			this.dependentProperties = Array.AsReadOnly<ContextProperty>(dependentProperties);
		}

		public IPropertyBag Properties
		{
			get
			{
				return this.stateSpecificPropertyBag;
			}
		}

		public TaskResult Result
		{
			get
			{
				return this.result;
			}
			protected set
			{
				this.result = value;
			}
		}

		public LocalizedString TaskTitle
		{
			get
			{
				return this.title;
			}
		}

		public LocalizedString TaskDescription
		{
			get
			{
				return this.description;
			}
		}

		public TaskType TaskType
		{
			get
			{
				return this.type;
			}
		}

		public IEnumerable<ContextProperty> DependentProperties
		{
			get
			{
				return TaskInfo.Get(base.GetType()).Dependencies.Concat(this.dependentProperties);
			}
		}

		protected ILogger Logger
		{
			get
			{
				return this.context.Logger;
			}
		}

		protected IEnvironment Environment
		{
			get
			{
				return this.context.Environment;
			}
		}

		void ITask.Initialize(Action resumeDelegate)
		{
			this.ChangeState(BaseTask.State.Uninitialized, BaseTask.State.Initialized);
			this.resumeDelegate = resumeDelegate;
			this.Logger.TaskStarted(this);
		}

		void ITask.OnCompleted()
		{
			this.Set<ExDateTime>(BaseTask.TaskFinished, ExDateTime.Now);
			this.stateSpecificPropertyBag = this.context.Properties;
			this.ChangeState(BaseTask.State.Executing, BaseTask.State.ResultObtained);
			this.Logger.TaskCompleted(this, this.Result);
		}

		IEnumerator<ITask> ITask.Process()
		{
			this.ChangeState(BaseTask.State.Initialized, BaseTask.State.Executing);
			this.stateSpecificPropertyBag = new BaseTask.DependencyCheckingPropertyBag(this.context.Properties, this.DependentProperties);
			this.Set<ExDateTime>(BaseTask.TaskStarted, ExDateTime.Now);
			return this.Process();
		}

		public IContext CreateDerivedContext()
		{
			return this.context;
		}

		public IContext CreateContextCopy()
		{
			return this.context.CreateDerived();
		}

		public TValue Get<TValue>(ContextProperty<TValue> property)
		{
			return this.Properties.Get(property);
		}

		public void Set<T>(ContextProperty property, T value)
		{
			this.Properties.Set(property, value);
		}

		public virtual BaseTask Copy()
		{
			throw new NotImplementedException("BaseTask.Copy()");
		}

		protected abstract IEnumerator<ITask> Process();

		protected void Resume()
		{
			if (this is AsyncTask)
			{
				this.resumeDelegate();
				return;
			}
			throw new InvalidOperationException("Only AsyncTask derived classes can Resume");
		}

		private void ChangeState(BaseTask.State oldState, BaseTask.State newState)
		{
			if (this.state == oldState)
			{
				this.state = newState;
				return;
			}
			throw new InvalidOperationException(string.Format("Only State={0} tasks can transition into {1}.", oldState, newState));
		}

		public static readonly ContextProperty<ExDateTime> TaskStarted = ContextPropertySchema.TaskStarted.SetOnly();

		public static readonly ContextProperty<ExDateTime> TaskFinished = ContextPropertySchema.TaskFinished.SetOnly();

		public static readonly ContextProperty<Exception> Exception = ContextPropertySchema.Exception.SetOnly();

		public static readonly ContextProperty<string> ErrorDetails = ContextPropertySchema.ErrorDetails.SetOnly();

		protected static readonly ContextProperty<TimeSpan> Timeout = ContextPropertySchema.Timeout.GetOnly();

		private readonly ReadOnlyCollection<ContextProperty> dependentProperties;

		private readonly IContext context;

		private readonly LocalizedString title;

		private readonly LocalizedString description;

		private readonly TaskType type;

		private Action resumeDelegate;

		private BaseTask.State state;

		private IPropertyBag stateSpecificPropertyBag;

		private TaskResult result;

		private enum State
		{
			Uninitialized,
			Initialized,
			Executing,
			ResultObtained
		}

		private class DependencyCheckingPropertyBag : IPropertyBag
		{
			public DependencyCheckingPropertyBag(IPropertyBag inner, IEnumerable<ContextProperty> dependencies)
			{
				Util.ThrowOnNullArgument(inner, "inner");
				this.inner = inner;
				foreach (ContextProperty contextProperty in dependencies)
				{
					ContextProperty.AccessMode accessMode;
					this.propertyAccess.TryGetValue(contextProperty, out accessMode);
					this.propertyAccess[contextProperty] = (accessMode | contextProperty.AllowedAccessMode);
				}
			}

			public bool TryGet(ContextProperty property, out object value)
			{
				this.EnsurePropertyDeclared(property, ContextProperty.AccessMode.Get);
				return this.inner.TryGet(property, out value);
			}

			public void Set(ContextProperty property, object value)
			{
				this.EnsurePropertyDeclared(property, ContextProperty.AccessMode.Set);
				this.inner.Set(property, value);
			}

			private void EnsurePropertyDeclared(ContextProperty requestedProperty, ContextProperty.AccessMode requestedAccess)
			{
				ContextProperty.AccessMode accessMode;
				this.propertyAccess.TryGetValue(requestedProperty, out accessMode);
				if ((accessMode & requestedAccess) != requestedAccess)
				{
					throw new InvalidOperationException(string.Format("A task has declared at most {0} access mode on {1}, but is requesting {2}", accessMode, requestedProperty, requestedAccess));
				}
			}

			private readonly IPropertyBag inner;

			private readonly Dictionary<ContextProperty, ContextProperty.AccessMode> propertyAccess = new Dictionary<ContextProperty, ContextProperty.AccessMode>();
		}
	}
}
