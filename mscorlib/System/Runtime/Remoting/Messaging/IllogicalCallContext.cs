using System;
using System.Collections;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	internal class IllogicalCallContext
	{
		private Hashtable Datastore
		{
			get
			{
				if (this.m_Datastore == null)
				{
					this.m_Datastore = new Hashtable();
				}
				return this.m_Datastore;
			}
		}

		internal object HostContext
		{
			get
			{
				return this.m_HostContext;
			}
			set
			{
				this.m_HostContext = value;
			}
		}

		internal bool HasUserData
		{
			get
			{
				return this.m_Datastore != null && this.m_Datastore.Count > 0;
			}
		}

		public void FreeNamedDataSlot(string name)
		{
			this.Datastore.Remove(name);
		}

		public object GetData(string name)
		{
			return this.Datastore[name];
		}

		public void SetData(string name, object data)
		{
			this.Datastore[name] = data;
		}

		public IllogicalCallContext CreateCopy()
		{
			IllogicalCallContext illogicalCallContext = new IllogicalCallContext();
			illogicalCallContext.HostContext = this.HostContext;
			if (this.HasUserData)
			{
				IDictionaryEnumerator enumerator = this.m_Datastore.GetEnumerator();
				while (enumerator.MoveNext())
				{
					illogicalCallContext.Datastore[(string)enumerator.Key] = enumerator.Value;
				}
			}
			return illogicalCallContext;
		}

		private Hashtable m_Datastore;

		private object m_HostContext;

		internal struct Reader
		{
			public Reader(IllogicalCallContext ctx)
			{
				this.m_ctx = ctx;
			}

			public bool IsNull
			{
				get
				{
					return this.m_ctx == null;
				}
			}

			[SecurityCritical]
			public object GetData(string name)
			{
				if (!this.IsNull)
				{
					return this.m_ctx.GetData(name);
				}
				return null;
			}

			public object HostContext
			{
				get
				{
					if (!this.IsNull)
					{
						return this.m_ctx.HostContext;
					}
					return null;
				}
			}

			private IllogicalCallContext m_ctx;
		}
	}
}
