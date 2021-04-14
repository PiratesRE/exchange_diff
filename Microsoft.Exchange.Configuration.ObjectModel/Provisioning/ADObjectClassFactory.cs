using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Provisioning
{
	public static class ADObjectClassFactory
	{
		public static TResult GetObjectInstance<TResult>() where TResult : ADObject, new()
		{
			return Activator.CreateInstance<TResult>();
		}
	}
}
