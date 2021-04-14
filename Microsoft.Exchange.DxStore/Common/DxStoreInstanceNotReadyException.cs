using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.DxStore.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DxStoreInstanceNotReadyException : DxStoreInstanceServerException
	{
		public DxStoreInstanceNotReadyException(string currentState) : base(Strings.DxStoreInstanceNotReady(currentState))
		{
			this.currentState = currentState;
		}

		public DxStoreInstanceNotReadyException(string currentState, Exception innerException) : base(Strings.DxStoreInstanceNotReady(currentState), innerException)
		{
			this.currentState = currentState;
		}

		protected DxStoreInstanceNotReadyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.currentState = (string)info.GetValue("currentState", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("currentState", this.currentState);
		}

		public string CurrentState
		{
			get
			{
				return this.currentState;
			}
		}

		private readonly string currentState;
	}
}
