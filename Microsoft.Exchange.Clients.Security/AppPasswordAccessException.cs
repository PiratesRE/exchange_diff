using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Clients.Security
{
	public class AppPasswordAccessException : LiveClientException
	{
		public AppPasswordAccessException() : base(new LocalizedString(null), null)
		{
		}

		public Strings.IDs ErrorMessageStringId
		{
			get
			{
				return -1220450835;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return Strings.AppPasswordAccessErrorMessage;
			}
		}
	}
}
