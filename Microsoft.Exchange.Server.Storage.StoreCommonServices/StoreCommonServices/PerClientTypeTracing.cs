using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class PerClientTypeTracing
	{
		private static IPerIdentityTracing<ClientType> Instance
		{
			get
			{
				return PerClientTypeTracing.hookableInstance.Value;
			}
		}

		public static bool IsConfigured
		{
			get
			{
				return PerClientTypeTracing.Instance.IsConfigured;
			}
		}

		public static bool IsTurnedOn
		{
			get
			{
				return PerClientTypeTracing.Instance.IsTurnedOn;
			}
		}

		public static bool IsEnabled(ClientType clientType)
		{
			return PerClientTypeTracing.Instance.IsEnabled(clientType);
		}

		public static void TurnOn()
		{
			PerClientTypeTracing.Instance.TurnOn();
		}

		public static void TurnOff()
		{
			PerClientTypeTracing.Instance.TurnOff();
		}

		internal static IDisposable SetTestHook(IPerIdentityTracing<ClientType> testHook)
		{
			return PerClientTypeTracing.hookableInstance.SetTestHook(testHook);
		}

		private static Hookable<IPerIdentityTracing<ClientType>> hookableInstance = Hookable<IPerIdentityTracing<ClientType>>.Create(true, new PerClientTypeTracing.PerClientTypeTracingImpl());

		private class PerClientTypeTracingImpl : ExCustomTracingAdaptor<ClientType>, IPerIdentityTracing<ClientType>
		{
			public bool IsConfigured
			{
				get
				{
					return ExTraceConfiguration.Instance.PerThreadTracingConfigured;
				}
			}

			public bool IsTurnedOn
			{
				get
				{
					return BaseTrace.CurrentThreadSettings.IsEnabled;
				}
			}

			public bool IsEnabled(ClientType clientType)
			{
				return base.IsTracingEnabled(clientType);
			}

			public void TurnOn()
			{
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}

			public void TurnOff()
			{
				BaseTrace.CurrentThreadSettings.DisableTracing();
			}

			protected override HashSet<ClientType> LoadEnabledIdentities(ExTraceConfiguration currentInstance)
			{
				HashSet<ClientType> hashSet = new HashSet<ClientType>();
				List<string> list;
				if (currentInstance.CustomParameters.TryGetValue("ClientType", out list))
				{
					for (int i = 0; i < list.Count; i++)
					{
						ClientType item;
						if (Enum.TryParse<ClientType>(list[i], true, out item))
						{
							hashSet.Add(item);
						}
					}
				}
				return hashSet;
			}
		}
	}
}
