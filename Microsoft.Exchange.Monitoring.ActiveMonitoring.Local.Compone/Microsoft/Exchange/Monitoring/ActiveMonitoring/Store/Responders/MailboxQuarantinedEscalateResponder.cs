using System;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Responders
{
	public class MailboxQuarantinedEscalateResponder : EscalateResponder
	{
		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			DateTime dateTime = DateTime.MinValue;
			try
			{
				Guid guid;
				if (Guid.TryParse(base.Definition.TargetExtension, out guid))
				{
					using (RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, Environment.MachineName))
					{
						if (registryKey != null)
						{
							string name = string.Format("{0}\\{1}\\Private-{2}", "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS", Environment.MachineName, guid);
							using (RegistryKey registryKey2 = registryKey.OpenSubKey(name, true))
							{
								if (registryKey2 != null)
								{
									using (RegistryKey registryKey3 = registryKey2.OpenSubKey("QuarantinedMailboxes"))
									{
										if (registryKey3 == null)
										{
											return;
										}
										string[] subKeyNames = registryKey3.GetSubKeyNames();
										if (subKeyNames != null && subKeyNames.Length < 1)
										{
											return;
										}
									}
									double totalSeconds = StoreDiscovery.suppressQuarantineAlertDuration.TotalSeconds;
									double.TryParse(base.Definition.Attributes["SuppressQuarantineAlertDuration"], out totalSeconds);
									object value = registryKey2.GetValue("EscalatedTime");
									if (value != null)
									{
										dateTime = DateTime.FromFileTimeUtc((long)value);
									}
									if (value == null || dateTime.AddSeconds(totalSeconds) < DateTime.UtcNow)
									{
										registryKey2.SetValue("EscalatedTime", DateTime.UtcNow.ToFileTime(), RegistryValueKind.QWord);
										base.DoResponderWork(cancellationToken);
										return;
									}
								}
								base.Result.StateAttribute4 = "Skipping escalation";
								base.Result.StateAttribute5 = string.Format("{0}-{1}", "EscalatedTime", dateTime);
								goto IL_16D;
							}
						}
						base.DoResponderWork(cancellationToken);
						IL_16D:
						goto IL_180;
					}
				}
				base.DoResponderWork(cancellationToken);
				IL_180:;
			}
			catch (IOException)
			{
				base.DoResponderWork(cancellationToken);
			}
			catch (SecurityException)
			{
				base.DoResponderWork(cancellationToken);
			}
			catch (UnauthorizedAccessException)
			{
				base.DoResponderWork(cancellationToken);
			}
		}

		private const string QuarantineBaseRegistryKey = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS";

		private const string QuarantineMailboxesRegistryKey = "QuarantinedMailboxes";

		private const string EscalatedTime = "EscalatedTime";
	}
}
