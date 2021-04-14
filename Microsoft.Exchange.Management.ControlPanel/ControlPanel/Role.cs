using System;
using System.ComponentModel;
using System.Web;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class Role : ValuePair
	{
		[DefaultValue("")]
		public override object Value
		{
			get
			{
				return this.IsInRoles;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		[DefaultValue("")]
		public string Rbac { get; set; }

		public bool IsInRoles
		{
			get
			{
				return LoginUtil.IsInRoles(HttpContext.Current.User, this.Rbac.Split(new char[]
				{
					','
				}));
			}
		}
	}
}
