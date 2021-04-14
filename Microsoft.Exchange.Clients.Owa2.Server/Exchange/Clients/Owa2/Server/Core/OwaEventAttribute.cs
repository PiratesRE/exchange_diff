using System;
using System.Collections;
using System.Reflection;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class OwaEventAttribute : Attribute
	{
		public OwaEventAttribute(string name, bool isInternal, bool allowAnonymousAccess)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.isInternal = isInternal;
			this.AllowAnonymousAccess = allowAnonymousAccess;
			this.paramInfoTable = new Hashtable();
		}

		public OwaEventAttribute(string name, bool isInternal) : this(name, isInternal, false)
		{
		}

		public OwaEventAttribute(string name) : this(name, false)
		{
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		internal bool IsAsync
		{
			get
			{
				return this.isAsync;
			}
			set
			{
				this.isAsync = value;
			}
		}

		internal MethodInfo MethodInfo
		{
			get
			{
				return this.methodInfo;
			}
			set
			{
				this.methodInfo = value;
			}
		}

		internal MethodInfo BeginMethodInfo
		{
			get
			{
				return this.beginMethodInfo;
			}
			set
			{
				this.beginMethodInfo = value;
			}
		}

		internal MethodInfo EndMethodInfo
		{
			get
			{
				return this.endMethodInfo;
			}
			set
			{
				this.endMethodInfo = value;
			}
		}

		internal ulong RequiredMask
		{
			get
			{
				return this.requiredMask;
			}
			set
			{
				this.requiredMask = value;
			}
		}

		internal OwaEventVerb AllowedVerbs
		{
			get
			{
				return this.verbs;
			}
			set
			{
				this.verbs = value;
			}
		}

		internal bool IsInternal
		{
			get
			{
				return this.isInternal;
			}
			set
			{
				this.isInternal = value;
			}
		}

		internal bool AllowAnonymousAccess { get; private set; }

		internal Hashtable ParamInfoTable
		{
			get
			{
				return this.paramInfoTable;
			}
		}

		internal void AddParameterInfo(OwaEventParameterAttribute paramInfo)
		{
			this.paramInfoTable.Add(paramInfo.Name, paramInfo);
		}

		internal OwaEventParameterAttribute FindParameterInfo(string name)
		{
			return (OwaEventParameterAttribute)this.paramInfoTable[name];
		}

		private string name;

		private Hashtable paramInfoTable;

		private MethodInfo methodInfo;

		private MethodInfo beginMethodInfo;

		private MethodInfo endMethodInfo;

		private ulong requiredMask;

		private OwaEventVerb verbs = OwaEventVerb.Post;

		private bool isAsync;

		private bool isInternal;
	}
}
