using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AjaxControlToolkit
{
	public class CombinableScripts
	{
		public ScriptsEntries Scripts { get; set; }

		public string Alias { get; set; }

		public int Rank { get; set; }

		public bool HasScriptResources
		{
			get
			{
				foreach (ScriptEntry scriptEntry in this.Scripts)
				{
					if (!scriptEntry.SkipScriptResources && scriptEntry.HasScriptResources)
					{
						return true;
					}
				}
				return false;
			}
		}

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] DependsOn { get; set; }

		public ScriptReference ToScriptReference()
		{
			return new ScriptReference(this.Scripts[0].Name, this.Scripts[0].Assembly);
		}
	}
}
