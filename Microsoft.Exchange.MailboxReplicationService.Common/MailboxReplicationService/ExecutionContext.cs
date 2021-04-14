using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ExecutionContext
	{
		private ExecutionContext(DataContext[] contexts)
		{
			this.contexts = new List<DataContext>(contexts.Length);
			foreach (DataContext dataContext in contexts)
			{
				if (dataContext != null)
				{
					this.contexts.Add(dataContext);
				}
			}
		}

		public static ExecutionContext Create(params DataContext[] contexts)
		{
			return new ExecutionContext(contexts);
		}

		public static string GetDataContext(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				ExecutionContext.DataContextHolder contextHolder = ExecutionContext.GetContextHolder(ex, false);
				if (contextHolder != null && contextHolder.Contexts != null && contextHolder.Contexts.Count > 0)
				{
					foreach (string text in contextHolder.Contexts)
					{
						if (stringBuilder.Length == 0)
						{
							stringBuilder.Append(text);
						}
						else
						{
							stringBuilder.AppendFormat(Environment.NewLine + "{0}", text);
						}
					}
					return stringBuilder.ToString();
				}
			}
			return stringBuilder.ToString();
		}

		public static ExceptionSide? GetExceptionSide(Exception exception)
		{
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				ExecutionContext.DataContextHolder contextHolder = ExecutionContext.GetContextHolder(ex, false);
				if (contextHolder != null && contextHolder.Side != null)
				{
					return contextHolder.Side;
				}
			}
			return null;
		}

		public static OperationType GetOperationType(Exception exception)
		{
			for (Exception ex = exception; ex != null; ex = ex.InnerException)
			{
				ExecutionContext.DataContextHolder contextHolder = ExecutionContext.GetContextHolder(ex, false);
				if (contextHolder != null && contextHolder.OperationType != OperationType.None)
				{
					return contextHolder.OperationType;
				}
			}
			return OperationType.None;
		}

		public static void SetExceptionSide(Exception exception, ExceptionSide? side)
		{
			ExecutionContext.DataContextHolder contextHolder = ExecutionContext.GetContextHolder(exception, true);
			if (contextHolder != null)
			{
				contextHolder.Side = side;
			}
		}

		public static void StampCurrentDataContext(Exception exception)
		{
			ExecutionContext.DataContextHolder contextHolder = ExecutionContext.GetContextHolder(exception, true);
			if (contextHolder != null && ExecutionContext.currentContexts != null)
			{
				foreach (DataContext dataContext in ExecutionContext.currentContexts)
				{
					contextHolder.Contexts.Add(dataContext.ToString());
					if (contextHolder.Side == null)
					{
						OperationSideDataContext operationSideDataContext = dataContext as OperationSideDataContext;
						if (operationSideDataContext != null)
						{
							contextHolder.Side = new ExceptionSide?(operationSideDataContext.Side);
						}
					}
					if (contextHolder.OperationType == OperationType.None)
					{
						OperationDataContext operationDataContext = dataContext as OperationDataContext;
						if (operationDataContext != null && operationDataContext.OperationType != OperationType.None)
						{
							contextHolder.OperationType = operationDataContext.OperationType;
						}
					}
				}
			}
		}

		public void Execute(Action operation)
		{
			if (ExecutionContext.currentContexts == null)
			{
				ExecutionContext.currentContexts = new Stack<DataContext>();
			}
			try
			{
				for (int i = this.contexts.Count - 1; i >= 0; i--)
				{
					ExecutionContext.currentContexts.Push(this.contexts[i]);
				}
				ExecutionContext.currentContexts.Push(SeparatorDataContext.Separator);
				operation();
			}
			catch (Exception exception)
			{
				if (ExecutionContext.GetContextHolder(exception, false) == null)
				{
					ExecutionContext.StampCurrentDataContext(exception);
				}
				throw;
			}
			finally
			{
				ExecutionContext.currentContexts.Pop();
				for (int j = 0; j < this.contexts.Count; j++)
				{
					ExecutionContext.currentContexts.Pop();
				}
			}
		}

		private static ExecutionContext.DataContextHolder GetContextHolder(Exception exception, bool createIfMissing)
		{
			if (exception == null || exception.Data == null)
			{
				return null;
			}
			ExecutionContext.DataContextHolder dataContextHolder;
			if (exception.Data.Contains("Microsoft.Exchange.MailboxReplicationService.DataContext"))
			{
				dataContextHolder = (ExecutionContext.DataContextHolder)exception.Data["Microsoft.Exchange.MailboxReplicationService.DataContext"];
			}
			else if (createIfMissing)
			{
				dataContextHolder = new ExecutionContext.DataContextHolder();
				exception.Data["Microsoft.Exchange.MailboxReplicationService.DataContext"] = dataContextHolder;
			}
			else
			{
				dataContextHolder = null;
			}
			return dataContextHolder;
		}

		private const string ExceptionDataKey = "Microsoft.Exchange.MailboxReplicationService.DataContext";

		[ThreadStatic]
		private static Stack<DataContext> currentContexts;

		private List<DataContext> contexts;

		[Serializable]
		private class DataContextHolder
		{
			public DataContextHolder()
			{
				this.contexts = new List<string>();
			}

			protected DataContextHolder(SerializationInfo info, StreamingContext context)
			{
			}

			public List<string> Contexts
			{
				get
				{
					return this.contexts;
				}
			}

			public ExceptionSide? Side { get; set; }

			public OperationType OperationType { get; set; }

			private List<string> contexts;
		}
	}
}
