using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	[Serializable]
	public class PSRemoteServer : ExchangeDataObject
	{
		internal override ObjectSchema Schema
		{
			get
			{
				return PSRemoteServer.schema;
			}
		}

		public Fqdn RemotePSServer
		{
			get
			{
				return (Fqdn)this[PSRemoteServerSchema.RemotePSServer];
			}
			set
			{
				this[PSRemoteServerSchema.RemotePSServer] = value;
			}
		}

		public string UserAccount { get; set; }

		public string DisplayName { get; set; }

		public bool AutomaticallySelect { get; set; }

		public override ValidationError[] Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			if (!this.AutomaticallySelect && this.RemotePSServer == null)
			{
				list.Add(new PropertyValidationError(Strings.ManuallySelectedServerEmpty, PSRemoteServerSchema.RemotePSServer, this));
			}
			return list.ToArray();
		}

		private static PSRemoteServerSchema schema = ObjectSchema.GetInstance<PSRemoteServerSchema>();
	}
}
