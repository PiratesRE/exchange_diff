using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Core.Common;

namespace Microsoft.Exchange.Search.Core.Pipeline
{
	internal sealed class PipelineComponentList : Disposable
	{
		internal PipelineComponentList(int count, int poisonComponentThreshold)
		{
			this.components = new SortedList<int, PipelineComponentList.PipelineComponentElement>(count);
			this.count = count;
			this.hasStartStopComponent = false;
			this.poisonComponentThreshold = poisonComponentThreshold;
		}

		internal int Count
		{
			get
			{
				return this.count;
			}
		}

		internal bool HasStartStopComponent
		{
			get
			{
				return this.hasStartStopComponent;
			}
		}

		public IPipelineComponent this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				IPipelineComponent component;
				lock (this.locker)
				{
					component = this.components[index].Component;
				}
				return component;
			}
		}

		internal bool TrackPoisonComponent(int index)
		{
			bool result = false;
			lock (this.locker)
			{
				result = (++this.components[index].PoisonCount == this.poisonComponentThreshold);
			}
			return result;
		}

		internal bool IsPoisonComponent(int index)
		{
			bool result = false;
			lock (this.locker)
			{
				result = (this.components[index].PoisonCount >= this.poisonComponentThreshold);
			}
			return result;
		}

		internal int IndexOf(IPipelineComponent component)
		{
			Util.ThrowOnNullArgument(component, "component");
			int num = 0;
			while (num < this.count && component != this[num])
			{
				num++;
			}
			if (num >= this.count)
			{
				return -1;
			}
			return num;
		}

		internal IPipelineComponent Insert(int index, PipelineComponentCreator componentCreator)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (this.components.ContainsKey(index))
			{
				throw new ArgumentException("Creator with index already exists", "index");
			}
			Util.ThrowOnNullArgument(componentCreator, "componentCreator");
			this.components.Add(index, new PipelineComponentList.PipelineComponentElement(componentCreator));
			IPipelineComponent pipelineComponent = this[index];
			if (pipelineComponent is IStartStopPipelineComponent)
			{
				this.hasStartStopComponent = true;
			}
			return pipelineComponent;
		}

		internal void Recreate(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (!this.components.ContainsKey(index))
			{
				throw new ArgumentException("Creator with index doesn't exist", "index");
			}
			IPipelineComponent component = this[index];
			lock (this.locker)
			{
				this.components[index].Create();
			}
			this.TryDisposeComponent(component);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PipelineComponentList>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				for (int i = 0; i < this.Count; i++)
				{
					IPipelineComponent component = this[i];
					this.TryDisposeComponent(component);
				}
				this.components.Clear();
				this.components = null;
			}
		}

		private bool TryDisposeComponent(IPipelineComponent component)
		{
			IDisposable disposable = component as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
				return true;
			}
			return false;
		}

		private readonly int count;

		private readonly int poisonComponentThreshold;

		private SortedList<int, PipelineComponentList.PipelineComponentElement> components;

		private object locker = new object();

		private bool hasStartStopComponent;

		internal class PipelineComponentElement
		{
			public PipelineComponentElement(PipelineComponentCreator componentCreator)
			{
				Util.ThrowOnNullArgument(componentCreator, "componentCreator");
				this.creator = componentCreator;
				this.PoisonCount = 0;
				this.Create();
			}

			public IPipelineComponent Component { get; private set; }

			public int PoisonCount { get; set; }

			public void Create()
			{
				this.Component = this.creator();
			}

			private readonly PipelineComponentCreator creator;
		}
	}
}
