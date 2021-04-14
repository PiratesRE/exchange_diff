using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Search.Core.Common
{
	[Serializable]
	internal class NoTransitionException : InvalidOperationException
	{
		public NoTransitionException(StatefulComponent component, uint state, uint signal) : base(string.Format("The transition for state {0} ({1}) and signal {2} ({3}) is not defined for component {4}", new object[]
		{
			state,
			component.GetStateName(state),
			signal,
			component.GetSignalName(signal),
			component
		}))
		{
			this.componentString = component.ToString();
		}

		protected NoTransitionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.componentString = (string)info.GetValue("ComponentString", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ComponentString", this.componentString);
		}

		private const string ComponentStringLabel = "ComponentString";

		private string componentString;
	}
}
