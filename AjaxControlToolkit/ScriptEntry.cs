using System;
using System.Reflection;
using System.Threading;
using System.Web.UI;

namespace AjaxControlToolkit
{
	public class ScriptEntry
	{
		public ScriptEntry() : this(null, null, false)
		{
		}

		public ScriptEntry(string assembly, string name, bool skipScriptResources)
		{
			this.Assembly = assembly;
			this.Name = name;
			this.SkipScriptResources = skipScriptResources;
		}

		public ScriptEntry(ScriptReference scriptReference, bool skipScriptResources) : this(scriptReference.Assembly, scriptReference.Name, skipScriptResources)
		{
		}

		public ScriptEntry(ScriptReference scriptReference) : this(scriptReference.Assembly, scriptReference.Name, false)
		{
		}

		public string Assembly { get; set; }

		public string Name { get; set; }

		public bool SkipScriptResources { get; set; }

		public bool HasScriptResources
		{
			get
			{
				foreach (ScriptResourceAttribute scriptResourceAttribute in this.LoadAssembly().GetCustomAttributes(typeof(ScriptResourceAttribute), false))
				{
					if (scriptResourceAttribute.ScriptName == this.Name)
					{
						return true;
					}
				}
				return false;
			}
		}

		public Assembly LoadAssembly()
		{
			if (null == this.loadedAssembly)
			{
				Assembly value = System.Reflection.Assembly.Load(this.Assembly);
				Interlocked.CompareExchange<Assembly>(ref this.loadedAssembly, value, null);
			}
			return this.loadedAssembly;
		}

		public override bool Equals(object obj)
		{
			ScriptEntry scriptEntry = (ScriptEntry)obj;
			return scriptEntry.Assembly == this.Assembly && scriptEntry.Name == this.Name;
		}

		public override int GetHashCode()
		{
			return this.Assembly.GetHashCode() ^ this.Name.GetHashCode();
		}

		private Assembly loadedAssembly;
	}
}
