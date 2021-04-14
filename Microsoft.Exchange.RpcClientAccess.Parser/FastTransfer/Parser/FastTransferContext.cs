using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[DebuggerNonUserCode]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class FastTransferContext<TDataInterface> : BaseObject where TDataInterface : class, IFastTransferDataInterface
	{
		protected FastTransferContext(IResourceTracker resourceTracker, IPropertyFilterFactory propertyFilterFactory, bool isMovingMailbox)
		{
			this.resourceTracker = resourceTracker;
			this.propertyFilterFactory = propertyFilterFactory;
			this.isMovingMailbox = isMovingMailbox;
		}

		public bool IsMovingMailbox
		{
			get
			{
				return this.isMovingMailbox;
			}
		}

		public FastTransferState State
		{
			get
			{
				FastTransferState? fastTransferState = this.state;
				if (fastTransferState == null)
				{
					return FastTransferState.Partial;
				}
				return fastTransferState.GetValueOrDefault();
			}
			protected set
			{
				this.state = new FastTransferState?(value);
			}
		}

		public IPropertyFilterFactory PropertyFilterFactory
		{
			get
			{
				return this.propertyFilterFactory;
			}
		}

		public IResourceTracker ResourceTracker
		{
			get
			{
				return this.resourceTracker;
			}
		}

		internal FastTransferStateMachineFactory StateMachineFactory
		{
			get
			{
				base.CheckDisposed();
				return this.stateMachineFactoryInstance;
			}
		}

		internal TDataInterface DataInterface
		{
			get
			{
				return this.currentDataInterface;
			}
		}

		private FastTransferStateMachine? Top
		{
			get
			{
				if (this.fastTransferStack.Count <= 0)
				{
					return null;
				}
				return new FastTransferStateMachine?(this.fastTransferStack.Peek());
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("({0})", this.State);
			foreach (FastTransferStateMachine fastTransferStateMachine in this.fastTransferStack)
			{
				stringBuilder.Append(" <= ");
				stringBuilder.Append(fastTransferStateMachine);
			}
			return stringBuilder.ToString();
		}

		protected static FastTransferStateMachine CreateStateMachine<TContext>(TContext context, IFastTransferProcessor<TContext> fastTransferObject) where TContext : FastTransferContext<TDataInterface>
		{
			FastTransferStateMachine result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				disposeGuard.Add<IFastTransferProcessor<TContext>>(fastTransferObject);
				IEnumerator<FastTransferStateMachine?> enumerator = fastTransferObject.Process(context);
				disposeGuard.Add<IEnumerator<FastTransferStateMachine?>>(enumerator);
				FastTransferStateMachine fastTransferStateMachine = new FastTransferStateMachine(fastTransferObject, enumerator);
				disposeGuard.Success();
				result = fastTransferStateMachine;
			}
			return result;
		}

		protected void PushInitial(FastTransferStateMachine stateMachine)
		{
			if (this.state != null || this.fastTransferStack.Count > 0)
			{
				throw new InvalidOperationException("The fast transfer context has alread been primed");
			}
			this.Push(stateMachine);
		}

		protected virtual void Process(TDataInterface dataInterface)
		{
			if (this.state == FastTransferState.Error)
			{
				if (this.unexpectedExceptionDispatchInfo != null)
				{
					this.unexpectedExceptionDispatchInfo.Throw();
				}
				return;
			}
			if (this.state == null && this.Top == null)
			{
				throw new InvalidOperationException("Context has not yet been primed with a FastTransferObject");
			}
			TDataInterface tdataInterface = Interlocked.Exchange<TDataInterface>(ref this.currentDataInterface, dataInterface);
			FastTransferStateMachine? fastTransferStateMachine = null;
			try
			{
				this.state = new FastTransferState?(FastTransferState.Error);
				FastTransferStateMachine? fastTransferStateMachine2 = this.Top;
				while (this.CanContinue() && fastTransferStateMachine2 != null)
				{
					try
					{
						fastTransferStateMachine = fastTransferStateMachine2.Value.Step();
					}
					catch (RopExecutionException ex)
					{
						throw new RopExecutionException(string.Format("State machine stalled. Stack: {0}", this), ex.ErrorCode, ex);
					}
					if (fastTransferStateMachine != null)
					{
						if (!fastTransferStateMachine.Value.Equals(fastTransferStateMachine2.Value))
						{
							this.Push(fastTransferStateMachine.Value);
						}
						fastTransferStateMachine2 = fastTransferStateMachine;
						fastTransferStateMachine = null;
					}
					else
					{
						this.Pop(fastTransferStateMachine2.Value);
						fastTransferStateMachine2 = this.Top;
					}
					TDataInterface dataInterface2 = this.DataInterface;
					dataInterface2.NotifyCanSplitBuffers();
				}
				this.state = new FastTransferState?((fastTransferStateMachine2 == null) ? FastTransferState.Done : FastTransferState.Partial);
			}
			catch (Exception source)
			{
				this.unexpectedExceptionDispatchInfo = ExceptionDispatchInfo.Capture(source);
				throw;
			}
			finally
			{
				if (fastTransferStateMachine != null)
				{
					fastTransferStateMachine.Value.Dispose();
				}
				TDataInterface tdataInterface2 = Interlocked.Exchange<TDataInterface>(ref this.currentDataInterface, default(TDataInterface));
			}
		}

		protected abstract bool CanContinue();

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FastTransferContext<TDataInterface>>(this);
		}

		protected override void InternalDispose()
		{
			foreach (FastTransferStateMachine fastTransferStateMachine in this.fastTransferStack)
			{
				fastTransferStateMachine.Dispose();
			}
			base.InternalDispose();
		}

		private void Push(FastTransferStateMachine stateMachine)
		{
			this.fastTransferStack.Push(stateMachine);
		}

		private void Pop(FastTransferStateMachine doneStateMachine)
		{
			using (this.fastTransferStack.Pop())
			{
			}
		}

		private const FastTransferState DefaultState = FastTransferState.Partial;

		private readonly Stack<FastTransferStateMachine> fastTransferStack = new Stack<FastTransferStateMachine>();

		private readonly FastTransferStateMachineFactory stateMachineFactoryInstance = new FastTransferStateMachineFactory();

		private readonly IPropertyFilterFactory propertyFilterFactory;

		private readonly IResourceTracker resourceTracker;

		private readonly bool isMovingMailbox;

		private TDataInterface currentDataInterface;

		private FastTransferState? state;

		private ExceptionDispatchInfo unexpectedExceptionDispatchInfo;
	}
}
