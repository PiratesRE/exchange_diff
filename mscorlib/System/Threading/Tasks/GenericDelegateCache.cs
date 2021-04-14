using System;

namespace System.Threading.Tasks
{
	internal static class GenericDelegateCache<TAntecedentResult, TResult>
	{
		internal static Func<Task<Task>, object, TResult> CWAnyFuncDelegate = delegate(Task<Task> wrappedWinner, object state)
		{
			Func<Task<TAntecedentResult>, TResult> func = (Func<Task<TAntecedentResult>, TResult>)state;
			Task<TAntecedentResult> arg = (Task<TAntecedentResult>)wrappedWinner.Result;
			return func(arg);
		};

		internal static Func<Task<Task>, object, TResult> CWAnyActionDelegate = delegate(Task<Task> wrappedWinner, object state)
		{
			Action<Task<TAntecedentResult>> action = (Action<Task<TAntecedentResult>>)state;
			Task<TAntecedentResult> obj = (Task<TAntecedentResult>)wrappedWinner.Result;
			action(obj);
			return default(TResult);
		};

		internal static Func<Task<Task<TAntecedentResult>[]>, object, TResult> CWAllFuncDelegate = delegate(Task<Task<TAntecedentResult>[]> wrappedAntecedents, object state)
		{
			wrappedAntecedents.NotifyDebuggerOfWaitCompletionIfNecessary();
			Func<Task<TAntecedentResult>[], TResult> func = (Func<Task<TAntecedentResult>[], TResult>)state;
			return func(wrappedAntecedents.Result);
		};

		internal static Func<Task<Task<TAntecedentResult>[]>, object, TResult> CWAllActionDelegate = delegate(Task<Task<TAntecedentResult>[]> wrappedAntecedents, object state)
		{
			wrappedAntecedents.NotifyDebuggerOfWaitCompletionIfNecessary();
			Action<Task<TAntecedentResult>[]> action = (Action<Task<TAntecedentResult>[]>)state;
			action(wrappedAntecedents.Result);
			return default(TResult);
		};
	}
}
