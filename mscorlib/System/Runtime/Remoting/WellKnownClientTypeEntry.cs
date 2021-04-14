using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	public class WellKnownClientTypeEntry : TypeEntry
	{
		public WellKnownClientTypeEntry(string typeName, string assemblyName, string objectUrl)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (objectUrl == null)
			{
				throw new ArgumentNullException("objectUrl");
			}
			base.TypeName = typeName;
			base.AssemblyName = assemblyName;
			this._objectUrl = objectUrl;
		}

		public WellKnownClientTypeEntry(Type type, string objectUrl)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (objectUrl == null)
			{
				throw new ArgumentNullException("objectUrl");
			}
			RuntimeType runtimeType = type as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"));
			}
			base.TypeName = type.FullName;
			base.AssemblyName = runtimeType.GetRuntimeAssembly().GetSimpleName();
			this._objectUrl = objectUrl;
		}

		public string ObjectUrl
		{
			get
			{
				return this._objectUrl;
			}
		}

		public Type ObjectType
		{
			[MethodImpl(MethodImplOptions.NoInlining)]
			get
			{
				StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
				return RuntimeTypeHandle.GetTypeByName(base.TypeName + ", " + base.AssemblyName, ref stackCrawlMark);
			}
		}

		public string ApplicationUrl
		{
			get
			{
				return this._appUrl;
			}
			set
			{
				this._appUrl = value;
			}
		}

		public override string ToString()
		{
			string text = string.Concat(new string[]
			{
				"type='",
				base.TypeName,
				", ",
				base.AssemblyName,
				"'; url=",
				this._objectUrl
			});
			if (this._appUrl != null)
			{
				text = text + "; appUrl=" + this._appUrl;
			}
			return text;
		}

		private string _objectUrl;

		private string _appUrl;
	}
}
