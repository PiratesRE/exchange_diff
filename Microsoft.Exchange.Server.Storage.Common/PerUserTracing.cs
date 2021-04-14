using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class PerUserTracing
	{
		private static IPerIdentityTracing<string> Instance
		{
			get
			{
				return PerUserTracing.hookableInstance.Value;
			}
		}

		public static bool IsConfigured
		{
			get
			{
				return PerUserTracing.Instance.IsConfigured;
			}
		}

		public static bool IsTurnedOn
		{
			get
			{
				return PerUserTracing.Instance.IsTurnedOn;
			}
		}

		public static bool IsEnabledForUser(string userName)
		{
			return PerUserTracing.Instance.IsEnabled(userName);
		}

		public static void TurnOn()
		{
			PerUserTracing.Instance.TurnOn();
		}

		public static void TurnOff()
		{
			PerUserTracing.Instance.TurnOff();
		}

		internal static IDisposable SetTestHook(IPerIdentityTracing<string> testHook)
		{
			return PerUserTracing.hookableInstance.SetTestHook(testHook);
		}

		private static Hookable<IPerIdentityTracing<string>> hookableInstance = Hookable<IPerIdentityTracing<string>>.Create(true, new PerUserTracing.PerUserTracingImpl());

		private class PerUserTracingImpl : IPerIdentityTracing<string>
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

			public bool IsEnabled(string userName)
			{
				return ExUserTracingAdaptor.Instance.IsTracingEnabledUser(userName);
			}

			public void TurnOn()
			{
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}

			public void TurnOff()
			{
				BaseTrace.CurrentThreadSettings.DisableTracing();
			}
		}
	}
}
