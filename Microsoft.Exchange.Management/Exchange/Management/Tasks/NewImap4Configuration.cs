using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[LocDescription(Strings.IDs.NewImap4ConfigurationTask)]
	[Cmdlet("New", "ImapSettings")]
	public sealed class NewImap4Configuration : NewPopImapConfiguration<Imap4AdConfiguration>
	{
		public NewImap4Configuration()
		{
			this.DataObject.Banner = "The Microsoft Exchange IMAP4 service is ready.";
			this.DataObject.UnencryptedOrTLSBindings = new MultiValuedProperty<IPBinding>(new IPBinding[]
			{
				new IPBinding("0.0.0.0:143"),
				new IPBinding("0000:0000:0000:0000:0000:0000:0.0.0.0:143")
			});
			this.DataObject.SSLBindings = new MultiValuedProperty<IPBinding>(new IPBinding[]
			{
				new IPBinding("0.0.0.0:993"),
				new IPBinding("0000:0000:0000:0000:0000:0000:0.0.0.0:993")
			});
			string localComputerFqdn = NativeHelpers.GetLocalComputerFqdn(false);
			if (!string.IsNullOrEmpty(localComputerFqdn))
			{
				this.DataObject.InternalConnectionSettings = new MultiValuedProperty<ProtocolConnectionSettings>(new ProtocolConnectionSettings[]
				{
					new ProtocolConnectionSettings(localComputerFqdn + ":143:TLS"),
					new ProtocolConnectionSettings(localComputerFqdn + ":993:SSL")
				});
			}
			this.DataObject.ProxyTargetPort = 1993;
		}

		protected override string ProtocolName
		{
			get
			{
				return "Imap4";
			}
		}
	}
}
