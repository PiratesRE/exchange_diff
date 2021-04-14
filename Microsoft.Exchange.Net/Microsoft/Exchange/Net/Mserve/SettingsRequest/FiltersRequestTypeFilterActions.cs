using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Net.Mserve.SettingsRequest
{
	[XmlType(AnonymousType = true, Namespace = "HMSETTINGS:")]
	[GeneratedCode("xsd", "2.0.50727.1318")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class FiltersRequestTypeFilterActions
	{
		public FiltersRequestTypeFilterActionsMoveToFolder MoveToFolder
		{
			get
			{
				return this.moveToFolderField;
			}
			set
			{
				this.moveToFolderField = value;
			}
		}

		private FiltersRequestTypeFilterActionsMoveToFolder moveToFolderField;
	}
}
