using System;
using System.Runtime.Remoting.Messaging;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation
{
	internal class DotNetCallContext : IContextPlugin
	{
		public static IContextPlugin Singleton
		{
			get
			{
				return DotNetCallContext.singleton;
			}
		}

		public Guid? LocalId
		{
			get
			{
				return (Guid?)CallContext.LogicalGetData("MSExchangeLocalId");
			}
			set
			{
				if (value != null)
				{
					CallContext.LogicalSetData("MSExchangeLocalId", value);
					return;
				}
				CallContext.FreeNamedDataSlot("MSExchangeLocalId");
			}
		}

		public bool IsContextPresent
		{
			get
			{
				return true;
			}
		}

		public void SetId()
		{
			CallContext.LogicalSetData("SingleContextIdKey", Environment.CurrentManagedThreadId);
		}

		public bool CheckId()
		{
			object obj = CallContext.LogicalGetData("SingleContextIdKey");
			return obj != null && (int)obj == Environment.CurrentManagedThreadId;
		}

		public void Clear()
		{
			CallContext.FreeNamedDataSlot("MSExchangeLocalId");
			CallContext.FreeNamedDataSlot("SingleContextIdKey");
		}

		private static IContextPlugin singleton = new DotNetCallContext();
	}
}
