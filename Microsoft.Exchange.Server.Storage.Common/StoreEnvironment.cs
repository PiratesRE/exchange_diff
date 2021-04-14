using System;
using System.Net.Sockets;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class StoreEnvironment
	{
		private static StoreEnvironment.Values Instance
		{
			get
			{
				if (StoreEnvironment.values == null)
				{
					StoreEnvironment.values = new StoreEnvironment.Values();
				}
				return StoreEnvironment.values;
			}
		}

		public static bool IsDatacenterEnvironment
		{
			get
			{
				return StoreEnvironment.Instance.IsDatacenterEnvironment;
			}
		}

		public static bool IsDedicatedEnvironment
		{
			get
			{
				return StoreEnvironment.Instance.IsDedicatedEnvironment;
			}
		}

		public static bool IsDogfoodEnvironment
		{
			get
			{
				return StoreEnvironment.Instance.IsDogfoodEnvironment;
			}
		}

		public static bool IsPerformanceEnvironment
		{
			get
			{
				return StoreEnvironment.Instance.IsPerformanceEnvironment;
			}
		}

		public static bool IsSdfEnvironment
		{
			get
			{
				return StoreEnvironment.Instance.IsSdfEnvironment;
			}
		}

		public static bool IsTestEnvironment
		{
			get
			{
				return StoreEnvironment.Instance.IsTestEnvironment;
			}
		}

		public static string MachineName
		{
			get
			{
				return StoreEnvironment.Instance.MachineName;
			}
		}

		internal static void Reset()
		{
			StoreEnvironment.values = null;
		}

		private static bool GetIsDatacenterDedicated()
		{
			bool result = false;
			try
			{
				result = Datacenter.IsDatacenterDedicated(true);
			}
			catch (CannotDetermineExchangeModeException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_CannotDetectEnvironment, new object[]
				{
					"Datacenter",
					ex
				});
			}
			return result;
		}

		private static bool GetExEnvironmentSetting(Func<bool> getter)
		{
			bool result = false;
			try
			{
				result = getter();
			}
			catch (SocketException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				Globals.LogEvent(MSExchangeISEventLogConstants.Tuple_CannotDetectEnvironment, new object[]
				{
					"ExEnvironment",
					ex
				});
			}
			return result;
		}

		private static bool GetIsPerformanceEnvironment()
		{
			int value = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Exchange_Test\\v15", "PerformanceTestEnvironment", 0);
			return value != 0;
		}

		private static StoreEnvironment.Values values;

		private class Values
		{
			public bool IsDatacenterEnvironment
			{
				get
				{
					if (this.isDatacenterEnvironment == null)
					{
						this.isDatacenterEnvironment = new bool?(Datacenter.IsRunningInExchangeDatacenter(false));
					}
					return this.isDatacenterEnvironment.Value;
				}
			}

			public bool IsDedicatedEnvironment
			{
				get
				{
					if (this.isDedicatedEnvironment == null)
					{
						this.isDedicatedEnvironment = new bool?(StoreEnvironment.GetIsDatacenterDedicated());
					}
					return this.isDedicatedEnvironment.Value;
				}
			}

			public bool IsDogfoodEnvironment
			{
				get
				{
					if (this.isDogfoodEnvironment == null)
					{
						this.isDogfoodEnvironment = new bool?(StoreEnvironment.GetExEnvironmentSetting(() => ExEnvironment.IsDogfoodDomain));
					}
					return this.isDogfoodEnvironment.Value;
				}
			}

			public bool IsPerformanceEnvironment
			{
				get
				{
					if (this.isPerformanceTest == null)
					{
						this.isPerformanceTest = new bool?(StoreEnvironment.GetIsPerformanceEnvironment());
					}
					return this.isPerformanceTest.Value;
				}
			}

			public bool IsSdfEnvironment
			{
				get
				{
					if (this.isSdfEnvironment == null)
					{
						this.isSdfEnvironment = new bool?(StoreEnvironment.GetExEnvironmentSetting(() => ExEnvironment.IsSdfDomain));
					}
					return this.isSdfEnvironment.Value;
				}
			}

			public bool IsTestEnvironment
			{
				get
				{
					if (this.isTestEnvironment == null)
					{
						this.isTestEnvironment = new bool?(StoreEnvironment.GetExEnvironmentSetting(() => ExEnvironment.IsTestDomain || ExEnvironment.IsTest));
					}
					return this.isTestEnvironment.Value;
				}
			}

			public string MachineName
			{
				get
				{
					if (this.machineName == null)
					{
						this.machineName = StoreEnvironment.Values.GetMachineName();
					}
					return this.machineName;
				}
			}

			public static string GetMachineName()
			{
				return Environment.MachineName;
			}

			private bool? isDatacenterEnvironment;

			private bool? isDedicatedEnvironment;

			private bool? isDogfoodEnvironment;

			private bool? isPerformanceTest;

			private bool? isSdfEnvironment;

			private bool? isTestEnvironment;

			private string machineName;
		}
	}
}
