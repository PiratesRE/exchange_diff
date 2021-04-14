using System;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal class ObjectNotInitializedException : ApplicationException
	{
		internal ObjectNotInitializedException(Type type) : base(string.Format("An instance of {0} is used before initialization.", type))
		{
		}
	}
}
