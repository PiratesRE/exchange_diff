using System;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal static class FaultInjectionHelper
	{
		public static Exception CreateXsoExceptionFromMapiException(string exceptionType, LocalizedString message)
		{
			Assembly assembly = Assembly.GetAssembly(typeof(MapiExceptionMailboxQuarantined));
			Type type = assembly.GetType(exceptionType);
			if (type != null)
			{
				ConstructorInfo constructorInfo = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
				ConstructorInfo constructorInfo2 = constructorInfo;
				object[] array = new object[5];
				array[0] = message.ToString();
				array[1] = 0;
				array[2] = 0;
				LocalizedException exception = (LocalizedException)constructorInfo2.Invoke(array);
				return StorageGlobals.TranslateMapiException(message, exception, null, null, string.Empty, new object[0]);
			}
			return null;
		}
	}
}
