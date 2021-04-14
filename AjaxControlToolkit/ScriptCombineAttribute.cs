using System;

namespace AjaxControlToolkit
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
	public sealed class ScriptCombineAttribute : Attribute
	{
		public string ExcludeScripts
		{
			get
			{
				return this.excludeScripts;
			}
			set
			{
				this.excludeScripts = value;
			}
		}

		public string IncludeScripts
		{
			get
			{
				return this.includeScripts;
			}
			set
			{
				this.includeScripts = value;
			}
		}

		private string excludeScripts;

		private string includeScripts;
	}
}
