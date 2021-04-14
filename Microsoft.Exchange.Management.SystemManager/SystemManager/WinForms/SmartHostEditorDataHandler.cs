using System;
using System.Net;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SmartHostEditorDataHandler : StrongTypeEditorDataHandler<SmartHost>
	{
		public SmartHostEditorDataHandler(bool isUMSmartHost, SmartHostEditor control, bool isCloudOrganizationMode) : base(control, "SmartHost")
		{
			this.isUMSmartHost = isUMSmartHost;
			base.Table.Rows[0]["IsCloudOrganizationMode"] = isCloudOrganizationMode;
			base.Table.Rows[0]["IsIpAddress"] = !isCloudOrganizationMode;
		}

		public SmartHostEditorDataHandler(bool isUMSmartHost, SmartHostEditor control) : this(isUMSmartHost, control, false)
		{
		}

		protected override void UpdateStrongType()
		{
			Hostname hostname = (!DBNull.Value.Equals(base.Table.Rows[0]["Domain"])) ? ((Hostname)base.Table.Rows[0]["Domain"]) : null;
			bool flag = (bool)base.Table.Rows[0]["IsIpAddress"];
			IPAddress ipaddress = (IPAddress)base.Table.Rows[0]["Address"];
			string address = string.Empty;
			if (flag)
			{
				address = ipaddress.ToString();
			}
			else if (hostname != null)
			{
				address = hostname.HostnameString;
			}
			base.StrongType = (this.isUMSmartHost ? UMSmartHost.Parse(address) : SmartHost.Parse(address));
		}

		protected override void UpdateTable()
		{
			SmartHost strongType = base.StrongType;
			if (strongType.IsIPAddress)
			{
				base.Table.Rows[0]["Address"] = strongType.Address;
			}
			else
			{
				base.Table.Rows[0]["Domain"] = strongType.Domain;
			}
			base.Table.Rows[0]["IsIpAddress"] = (strongType.IsIPAddress && false.Equals(base.Table.Rows[0]["IsCloudOrganizationMode"]));
		}

		private bool isUMSmartHost;
	}
}
