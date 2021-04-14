using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetCerts : ServiceCommand<GetCertsResponse>
	{
		public GetCerts(CallContext callContext, GetCertsRequest request) : base(callContext)
		{
			this.getEncryptionCertsCommand = new GetEncryptionCerts(callContext, request);
		}

		protected override GetCertsResponse InternalExecute()
		{
			GetCertsResponse getCertsResponse = this.getEncryptionCertsCommand.Execute();
			List<string> list = new List<string>();
			foreach (string[] array in getCertsResponse.ValidRecipients)
			{
				if (array != null && array.Length > 0)
				{
					list.AddRange(array);
				}
			}
			getCertsResponse.ValidRecipients = null;
			getCertsResponse.CertsRawData = list.ToArray();
			return getCertsResponse;
		}

		private readonly GetEncryptionCerts getEncryptionCertsCommand;
	}
}
