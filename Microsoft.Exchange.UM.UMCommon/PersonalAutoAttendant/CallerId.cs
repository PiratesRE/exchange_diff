using System;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal abstract class CallerId<T> : CallerIdBase
	{
		internal CallerId(CallerIdTypeEnum type, T data) : base(type)
		{
			this.data = data;
		}

		protected T GetData()
		{
			return this.data;
		}

		private T data;
	}
}
