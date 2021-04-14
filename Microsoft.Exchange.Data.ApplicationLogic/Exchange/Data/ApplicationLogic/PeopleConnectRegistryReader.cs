using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PeopleConnectRegistryReader
	{
		private PeopleConnectRegistryReader()
		{
			this.DogfoodInEnterprise = false;
		}

		public static PeopleConnectRegistryReader Read()
		{
			PeopleConnectRegistryReader result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PeopleConnect"))
				{
					if (registryKey == null)
					{
						result = new PeopleConnectRegistryReader();
					}
					else
					{
						result = new PeopleConnectRegistryReader
						{
							DogfoodInEnterprise = Convert.ToBoolean((int)registryKey.GetValue("DogfoodInEnterprise", 0))
						};
					}
				}
			}
			catch (SecurityException e)
			{
				result = PeopleConnectRegistryReader.TraceErrorAndReturnEmptyConfiguration(e);
			}
			catch (IOException e2)
			{
				result = PeopleConnectRegistryReader.TraceErrorAndReturnEmptyConfiguration(e2);
			}
			catch (UnauthorizedAccessException e3)
			{
				result = PeopleConnectRegistryReader.TraceErrorAndReturnEmptyConfiguration(e3);
			}
			return result;
		}

		public bool DogfoodInEnterprise { get; private set; }

		private static PeopleConnectRegistryReader TraceErrorAndReturnEmptyConfiguration(Exception e)
		{
			PeopleConnectRegistryReader.Tracer.TraceError<Exception>(0L, "PeopleConnectRegistryReader.Read: caught exception {0}", e);
			return new PeopleConnectRegistryReader();
		}

		private const string DogfoodInEnterpriseValueName = "DogfoodInEnterprise";

		private const string PeopleConnectKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PeopleConnect";

		private static readonly Trace Tracer = ExTraceGlobals.PeopleConnectConfigurationTracer;
	}
}
