using System;

namespace System.Threading.Tasks
{
	internal interface ITaskCompletionAction
	{
		void Invoke(Task completingTask);
	}
}
