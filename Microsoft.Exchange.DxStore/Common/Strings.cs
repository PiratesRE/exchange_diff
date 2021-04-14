using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.DxStore.Common
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(2536273634U, "DxStoreInstanceStaleStore");
		}

		public static LocalizedString DxStoreServerException(string errMsg)
		{
			return new LocalizedString("DxStoreServerException", Strings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString DxStoreClientTransientException(string errMsg)
		{
			return new LocalizedString("DxStoreClientTransientException", Strings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString DxStoreInstanceServerException(string errMsg2)
		{
			return new LocalizedString("DxStoreInstanceServerException", Strings.ResourceManager, new object[]
			{
				errMsg2
			});
		}

		public static LocalizedString DxStoreServerTransientException(string errMsg)
		{
			return new LocalizedString("DxStoreServerTransientException", Strings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString DxStoreClientException(string errMsg)
		{
			return new LocalizedString("DxStoreClientException", Strings.ResourceManager, new object[]
			{
				errMsg
			});
		}

		public static LocalizedString DxStoreAccessClientTransientException(string errMsg1)
		{
			return new LocalizedString("DxStoreAccessClientTransientException", Strings.ResourceManager, new object[]
			{
				errMsg1
			});
		}

		public static LocalizedString DxStoreCommandConstraintFailed(string phase)
		{
			return new LocalizedString("DxStoreCommandConstraintFailed", Strings.ResourceManager, new object[]
			{
				phase
			});
		}

		public static LocalizedString DxStoreInstanceNotReady(string currentState)
		{
			return new LocalizedString("DxStoreInstanceNotReady", Strings.ResourceManager, new object[]
			{
				currentState
			});
		}

		public static LocalizedString DxStoreAccessServerTransientException(string errMsg1)
		{
			return new LocalizedString("DxStoreAccessServerTransientException", Strings.ResourceManager, new object[]
			{
				errMsg1
			});
		}

		public static LocalizedString DxStoreAccessClientException(string errMsg2)
		{
			return new LocalizedString("DxStoreAccessClientException", Strings.ResourceManager, new object[]
			{
				errMsg2
			});
		}

		public static LocalizedString DxStoreManagerServerTransientException(string errMsg5)
		{
			return new LocalizedString("DxStoreManagerServerTransientException", Strings.ResourceManager, new object[]
			{
				errMsg5
			});
		}

		public static LocalizedString DxStoreInstanceClientException(string errMsg2)
		{
			return new LocalizedString("DxStoreInstanceClientException", Strings.ResourceManager, new object[]
			{
				errMsg2
			});
		}

		public static LocalizedString DxStoreBindingNotSupportedException(string bindingStr)
		{
			return new LocalizedString("DxStoreBindingNotSupportedException", Strings.ResourceManager, new object[]
			{
				bindingStr
			});
		}

		public static LocalizedString DxStoreManagerClientException(string errMsg4)
		{
			return new LocalizedString("DxStoreManagerClientException", Strings.ResourceManager, new object[]
			{
				errMsg4
			});
		}

		public static LocalizedString DxStoreManagerGroupNotFoundException(string groupName)
		{
			return new LocalizedString("DxStoreManagerGroupNotFoundException", Strings.ResourceManager, new object[]
			{
				groupName
			});
		}

		public static LocalizedString DxStoreInstanceComponentNotInitialized(string component)
		{
			return new LocalizedString("DxStoreInstanceComponentNotInitialized", Strings.ResourceManager, new object[]
			{
				component
			});
		}

		public static LocalizedString SerializeError(string err)
		{
			return new LocalizedString("SerializeError", Strings.ResourceManager, new object[]
			{
				err
			});
		}

		public static LocalizedString DxStoreInstanceStaleStore
		{
			get
			{
				return new LocalizedString("DxStoreInstanceStaleStore", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DxStoreInstanceKeyNotFound(string keyName)
		{
			return new LocalizedString("DxStoreInstanceKeyNotFound", Strings.ResourceManager, new object[]
			{
				keyName
			});
		}

		public static LocalizedString DxStoreManagerServerException(string errMsg4)
		{
			return new LocalizedString("DxStoreManagerServerException", Strings.ResourceManager, new object[]
			{
				errMsg4
			});
		}

		public static LocalizedString DxStoreInstanceServerTransientException(string errMsg3)
		{
			return new LocalizedString("DxStoreInstanceServerTransientException", Strings.ResourceManager, new object[]
			{
				errMsg3
			});
		}

		public static LocalizedString DxStoreManagerClientTransientException(string errMsg5)
		{
			return new LocalizedString("DxStoreManagerClientTransientException", Strings.ResourceManager, new object[]
			{
				errMsg5
			});
		}

		public static LocalizedString DxStoreInstanceClientTransientException(string errMsg3)
		{
			return new LocalizedString("DxStoreInstanceClientTransientException", Strings.ResourceManager, new object[]
			{
				errMsg3
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.DxStore.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			DxStoreInstanceStaleStore = 2536273634U
		}

		private enum ParamIDs
		{
			DxStoreServerException,
			DxStoreClientTransientException,
			DxStoreInstanceServerException,
			DxStoreServerTransientException,
			DxStoreClientException,
			DxStoreAccessClientTransientException,
			DxStoreCommandConstraintFailed,
			DxStoreInstanceNotReady,
			DxStoreAccessServerTransientException,
			DxStoreAccessClientException,
			DxStoreManagerServerTransientException,
			DxStoreInstanceClientException,
			DxStoreBindingNotSupportedException,
			DxStoreManagerClientException,
			DxStoreManagerGroupNotFoundException,
			DxStoreInstanceComponentNotInitialized,
			SerializeError,
			DxStoreInstanceKeyNotFound,
			DxStoreManagerServerException,
			DxStoreInstanceServerTransientException,
			DxStoreManagerClientTransientException,
			DxStoreInstanceClientTransientException
		}
	}
}
