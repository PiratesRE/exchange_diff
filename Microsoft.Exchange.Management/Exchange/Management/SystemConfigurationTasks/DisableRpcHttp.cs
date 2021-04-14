using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Disable", "OutlookAnywhere", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableRpcHttp : RemoveExchangeVirtualDirectory<ADRpcHttpVirtualDirectory>
	{
		[Parameter(ParameterSetName = "Server", ValueFromPipeline = true)]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableRpcHttp(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (this.Server != null)
			{
				Server server = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
				if (base.HasErrors)
				{
					return;
				}
				VirtualDirectoryIdParameter id = VirtualDirectoryIdParameter.Parse(this.Server + "\\*");
				IEnumerable<ADRpcHttpVirtualDirectory> dataObjects = base.GetDataObjects<ADRpcHttpVirtualDirectory>(id, base.DataSession, server.Identity);
				IEnumerator<ADRpcHttpVirtualDirectory> enumerator = dataObjects.GetEnumerator();
				if (!enumerator.MoveNext())
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpNotEnabled(server.Fqdn), string.Empty), ErrorCategory.InvalidArgument, this.Server);
					return;
				}
				this.Identity = new VirtualDirectoryIdParameter(enumerator.Current);
				if (enumerator.MoveNext())
				{
					base.WriteError(new ArgumentException(Strings.ErrorRpcHttpNotUnique(server.Fqdn), string.Empty), ErrorCategory.InvalidArgument, this.Server);
					return;
				}
			}
			base.InternalValidate();
		}

		protected override void DeleteFromMetabase()
		{
		}
	}
}
