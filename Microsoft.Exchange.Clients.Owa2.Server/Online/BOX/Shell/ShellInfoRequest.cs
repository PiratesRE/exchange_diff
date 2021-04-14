using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Online.BOX.Shell
{
	[DataContract(Name = "ShellInfoRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class ShellInfoRequest : NavBarInfoRequest
	{
		[DataMember]
		public bool ExcludeMSAjax
		{
			get
			{
				return this.ExcludeMSAjaxField;
			}
			set
			{
				this.ExcludeMSAjaxField = value;
			}
		}

		[DataMember]
		public ShellBaseFlight? ShellBaseFlight
		{
			get
			{
				return this.ShellBaseFlightField;
			}
			set
			{
				this.ShellBaseFlightField = value;
			}
		}

		[DataMember]
		public string TenantId
		{
			get
			{
				return this.TenantIdField;
			}
			set
			{
				this.TenantIdField = value;
			}
		}

		[DataMember]
		public string UserThemeId
		{
			get
			{
				return this.UserThemeIdField;
			}
			set
			{
				this.UserThemeIdField = value;
			}
		}

		private bool ExcludeMSAjaxField;

		private ShellBaseFlight? ShellBaseFlightField;

		private string TenantIdField;

		private string UserThemeIdField;
	}
}
