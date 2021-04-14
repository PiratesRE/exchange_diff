using System;
using System.Globalization;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriver;
using Microsoft.Exchange.Transport;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	internal class PoisonHandler<TPoisonHandlerContext> : ITransportComponent
	{
		public PoisonHandler(string registrySuffix, TimeSpan poisonEntryExpiryWindow, int maxPoisonEntries)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("registrySuffix", registrySuffix);
			ArgumentValidator.ThrowIfZeroOrNegative("maxPoisonEntries", maxPoisonEntries);
			this.storeDriverPoisonInfo = new SynchronizedDictionary<string, CrashProperties>();
			this.storeDriverPoisonMsgLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\StoreDriver\\" + registrySuffix;
			this.poisonEntryExpiryWindow = poisonEntryExpiryWindow;
			this.maxPoisonEntries = maxPoisonEntries;
		}

		public static TPoisonHandlerContext Context
		{
			internal get
			{
				return PoisonHandler<TPoisonHandlerContext>.context;
			}
			set
			{
				PoisonMessage.Context = null;
				PoisonHandler<TPoisonHandlerContext>.context = value;
			}
		}

		public virtual int PoisonThreshold
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.PoisonThreshold;
			}
		}

		private bool Enabled
		{
			get
			{
				return this.loaded && Components.Configuration.LocalServer.TransportServer.PoisonMessageDetectionEnabled;
			}
		}

		public virtual void SavePoisonContext()
		{
			if (!this.Enabled)
			{
				return;
			}
			if (PoisonHandler<TPoisonHandlerContext>.Context == null)
			{
				TraceHelper.GeneralTracer.TracePass(TraceHelper.MessageProbeActivityId, 0L, "No poison context information stored on the crashing thread. Exiting...");
				return;
			}
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(this.storeDriverPoisonMsgLocation))
			{
				TPoisonHandlerContext tpoisonHandlerContext = PoisonHandler<TPoisonHandlerContext>.Context;
				string text = tpoisonHandlerContext.ToString();
				int num = 1;
				if (registryKey.GetValue(text) != null)
				{
					if (registryKey.GetValueKind(text) != RegistryValueKind.MultiString)
					{
						registryKey.DeleteValue(text, false);
					}
					else
					{
						string[] array = (string[])registryKey.GetValue(text);
						if (array == null || array.Length != 2)
						{
							registryKey.DeleteValue(text, false);
						}
						else if (!int.TryParse(array[0], out num))
						{
							registryKey.DeleteValue(text, false);
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					this.DeleteOldestPoisonEntryIfNecessary(registryKey);
				}
				CrashProperties crashProperties = new CrashProperties((double)num, DateTime.UtcNow);
				registryKey.SetValue(text, new string[]
				{
					Convert.ToString(crashProperties.CrashCount),
					crashProperties.LastCrashTime.ToString("u")
				}, RegistryValueKind.MultiString);
				this.storeDriverPoisonInfo[text] = crashProperties;
			}
		}

		public bool VerifyPoisonMessage(string poisonId, out int crashCount)
		{
			if (string.IsNullOrEmpty(poisonId))
			{
				throw new ArgumentNullException("poisonId");
			}
			if (!this.Enabled || this.storeDriverPoisonInfo.Count == 0)
			{
				crashCount = 0;
				return false;
			}
			CrashProperties crashProperties = null;
			if (this.storeDriverPoisonInfo.TryGetValue(poisonId, out crashProperties))
			{
				if (this.IsExpired(crashProperties))
				{
					this.MarkPoisonMessageHandled(poisonId);
				}
				else if (this.IsMessagePoison(crashProperties))
				{
					crashCount = (int)crashProperties.CrashCount;
					return true;
				}
			}
			crashCount = 0;
			return false;
		}

		public void MarkPoisonMessageHandledIfExists(string poisonId)
		{
			if (this.storeDriverPoisonInfo.ContainsKey(poisonId))
			{
				this.MarkPoisonMessageHandled(poisonId);
			}
		}

		public virtual void MarkPoisonMessageHandled(string poisonId)
		{
			if (string.IsNullOrEmpty(poisonId))
			{
				throw new ArgumentNullException("poisonId");
			}
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(this.storeDriverPoisonMsgLocation, true))
				{
					if (registryKey != null)
					{
						registryKey.DeleteValue(poisonId, false);
					}
				}
			}
			finally
			{
				this.storeDriverPoisonInfo.Remove(poisonId);
			}
		}

		public virtual void Load()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(this.storeDriverPoisonMsgLocation))
			{
				foreach (string text in registryKey.GetValueNames())
				{
					bool flag = registryKey.GetValueKind(text) != RegistryValueKind.MultiString;
					string[] array = (string[])registryKey.GetValue(text);
					if (array.Length != 2)
					{
						flag = true;
					}
					int value;
					DateTime lastCrashTime;
					if (flag)
					{
						TraceHelper.GeneralTracer.TraceFail<string, RegistryKey>(TraceHelper.MessageProbeActivityId, 0L, "Invalid value {0} in {1} registry key. Deleting it.", text, registryKey);
						registryKey.DeleteValue(text, false);
					}
					else if (!int.TryParse(array[0], out value) || !DateTime.TryParseExact(array[1], "u", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.RoundtripKind, out lastCrashTime))
					{
						TraceHelper.GeneralTracer.TraceFail<string, RegistryKey>(TraceHelper.MessageProbeActivityId, 0L, "Invalid value {0} in {1} registry key. Deleting it.", text, registryKey);
						registryKey.DeleteValue(text, false);
					}
					else
					{
						CrashProperties crashProperties = new CrashProperties(Convert.ToDouble(value), lastCrashTime);
						if (!this.IsExpired(crashProperties))
						{
							this.storeDriverPoisonInfo[text] = crashProperties;
						}
						else
						{
							registryKey.DeleteValue(text, false);
						}
					}
				}
			}
			this.loaded = true;
		}

		public void Unload()
		{
			this.storeDriverPoisonInfo.Clear();
			this.loaded = false;
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		protected virtual bool IsMessagePoison(CrashProperties crashProperties)
		{
			return crashProperties.CrashCount >= (double)this.PoisonThreshold;
		}

		protected virtual bool IsExpired(CrashProperties crashProperties)
		{
			if (crashProperties == null)
			{
				return false;
			}
			try
			{
				if (DateTime.UtcNow > crashProperties.LastCrashTime + this.poisonEntryExpiryWindow)
				{
					return true;
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				return true;
			}
			return false;
		}

		private void DeleteOldestPoisonEntryIfNecessary(RegistryKey poisonRegistryEntryKey)
		{
			if (poisonRegistryEntryKey.ValueCount < this.maxPoisonEntries)
			{
				return;
			}
			string oldestPoisonEntryValueName = StoreDriverUtils.GetOldestPoisonEntryValueName(this.storeDriverPoisonInfo);
			if (!string.IsNullOrEmpty(oldestPoisonEntryValueName))
			{
				this.storeDriverPoisonInfo.Remove(oldestPoisonEntryValueName);
				poisonRegistryEntryKey.DeleteValue(oldestPoisonEntryValueName, false);
			}
		}

		protected const string LastCrashTimeFormat = "u";

		private const string PoisonMsgLocationBase = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\PoisonMessage\\StoreDriver\\";

		[ThreadStatic]
		private static TPoisonHandlerContext context;

		private readonly string storeDriverPoisonMsgLocation;

		private readonly TimeSpan poisonEntryExpiryWindow;

		private readonly int maxPoisonEntries;

		private bool loaded;

		private SynchronizedDictionary<string, CrashProperties> storeDriverPoisonInfo;
	}
}
