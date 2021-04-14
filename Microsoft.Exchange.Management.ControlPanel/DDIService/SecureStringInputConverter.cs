using System;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.DDIService
{
	public class SecureStringInputConverter : IInputConverter, IDDIConverter
	{
		public bool CanConvert(object sourceObject)
		{
			return sourceObject is string;
		}

		public object Convert(object sourceObject)
		{
			string password = sourceObject as string;
			return password.ConvertToSecureString();
		}
	}
}
