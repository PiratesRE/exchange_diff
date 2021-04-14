using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ThemeSelectionInfoType
	{
		[DataMember]
		public Theme[] Themes
		{
			get
			{
				return this.themes;
			}
			set
			{
				this.themes = value;
			}
		}

		[DataMember]
		public string[] CssPaths
		{
			get
			{
				return this.cssPaths;
			}
			set
			{
				this.cssPaths = value;
			}
		}

		[DataMember]
		public string ThemePath
		{
			get
			{
				return this.themePath;
			}
			set
			{
				this.themePath = value;
			}
		}

		private Theme[] themes;

		private string[] cssPaths;

		private string themePath;
	}
}
