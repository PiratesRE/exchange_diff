using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ModeratorPickerFilter : RecipientPickerFilterBase
	{
		protected override RecipientTypeDetails[] RecipientTypeDetailsParam
		{
			get
			{
				return new RecipientTypeDetails[]
				{
					RecipientTypeDetails.UserMailbox,
					RecipientTypeDetails.SharedMailbox,
					RecipientTypeDetails.TeamMailbox,
					RecipientTypeDetails.LinkedMailbox,
					RecipientTypeDetails.LegacyMailbox,
					RecipientTypeDetails.MailUser,
					RecipientTypeDetails.MailContact,
					RecipientTypeDetails.MailForestContact,
					(RecipientTypeDetails)((ulong)int.MinValue)
				};
			}
		}
	}
}
