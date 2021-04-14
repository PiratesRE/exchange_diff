using System;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class ADNotificationRequest
	{
		internal Type Type
		{
			get
			{
				return this.type;
			}
		}

		internal string ObjectClass
		{
			get
			{
				return this.objectClass;
			}
		}

		internal ADObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		internal ADNotificationCallback Callback
		{
			get
			{
				return this.callback;
			}
		}

		internal object Context
		{
			get
			{
				return this.context;
			}
		}

		internal bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		internal void MakeInvalid()
		{
			ExTraceGlobals.ADNotificationsTracer.TraceDebug<int>((long)this.GetHashCode(), "Invalidating request {0}", this.GetHashCode());
			this.isValid = false;
		}

		internal ADNotificationRequest(Type type, string objectClass, ADObjectId rootId, ADNotificationCallback callback, object context)
		{
			if (rootId == null)
			{
				throw new ArgumentNullException("rootId");
			}
			this.type = type;
			this.objectClass = objectClass;
			this.rootId = rootId;
			this.callback = callback;
			this.context = context;
			this.isValid = true;
			this.RefCount = 0;
		}

		private Type type;

		private string objectClass;

		private ADObjectId rootId;

		private ADNotificationCallback callback;

		private object context;

		private bool isValid;

		internal ExDateTime LastCallbackTime;

		internal bool isDeletedContainer;

		internal int RefCount;
	}
}
