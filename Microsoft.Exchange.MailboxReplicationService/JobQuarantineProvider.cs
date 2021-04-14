using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class JobQuarantineProvider : IJobQuarantineProvider
	{
		private JobQuarantineProvider()
		{
		}

		public static IJobQuarantineProvider Instance
		{
			get
			{
				return JobQuarantineProvider.hookableInstance.Value;
			}
		}

		internal static IDisposable SetTestHook(IJobQuarantineProvider testHook)
		{
			return JobQuarantineProvider.hookableInstance.SetTestHook(testHook);
		}

		public void QuarantineJob(Guid requestGuid, Exception ex)
		{
			try
			{
				FailureRec failureRec = FailureRec.Create(ex);
				string subkeyName = string.Format(JobQuarantineProvider.KeyNameFormatQuarantinedJob, requestGuid);
				RegistryWriter.Instance.CreateSubKey(Registry.LocalMachine, subkeyName);
				RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, "FailureType", failureRec.FailureType ?? string.Empty, RegistryValueKind.String);
				RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, "Message", failureRec.Message ?? string.Empty, RegistryValueKind.String);
				RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, "StackTrace", failureRec.StackTrace ?? string.Empty, RegistryValueKind.String);
				string dataContext = failureRec.DataContext ?? string.Empty;
				RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, "DataContext", FailureLog.GetDataContextToPersist(dataContext), RegistryValueKind.String);
				string text = string.Empty;
				if (failureRec.InnerException != null)
				{
					text = failureRec.InnerException.StackTrace;
				}
				RegistryWriter.Instance.SetValue(Registry.LocalMachine, subkeyName, "InnerException", text ?? string.Empty, RegistryValueKind.String);
			}
			catch (ArgumentException)
			{
			}
		}

		public void UnquarantineJob(Guid requestGuid)
		{
			string subkeyName = string.Format(JobQuarantineProvider.KeyNameFormatQuarantinedJob, requestGuid);
			try
			{
				RegistryWriter.Instance.DeleteSubKeyTree(Registry.LocalMachine, subkeyName);
			}
			catch (ArgumentException)
			{
			}
		}

		public IDictionary<Guid, FailureRec> GetQuarantinedJobs()
		{
			IRegistryReader instance = RegistryReader.Instance;
			IRegistryWriter instance2 = RegistryWriter.Instance;
			string[] array = null;
			try
			{
				array = instance.GetSubKeyNames(Registry.LocalMachine, JobQuarantineProvider.KeyNameFormatQuarantinedJobRoot);
			}
			catch (ArgumentException)
			{
			}
			if (array == null)
			{
				return new Dictionary<Guid, FailureRec>();
			}
			Dictionary<Guid, FailureRec> dictionary = new Dictionary<Guid, FailureRec>(array.Length);
			string[] array2 = array;
			int i = 0;
			while (i < array2.Length)
			{
				string text = array2[i];
				Guid key = Guid.Empty;
				string subkeyName = Path.Combine(JobQuarantineProvider.KeyNameFormatQuarantinedJobRoot, text);
				try
				{
					key = new Guid(text);
				}
				catch (FormatException)
				{
					try
					{
						instance2.DeleteSubKeyTree(Registry.LocalMachine, subkeyName);
					}
					catch (ArgumentException)
					{
					}
					goto IL_118;
				}
				goto IL_80;
				IL_118:
				i++;
				continue;
				IL_80:
				string value = instance.GetValue<string>(Registry.LocalMachine, subkeyName, "FailureType", string.Empty);
				string value2 = instance.GetValue<string>(Registry.LocalMachine, subkeyName, "Message", string.Empty);
				string value3 = instance.GetValue<string>(Registry.LocalMachine, subkeyName, "StackTrace", string.Empty);
				string value4 = instance.GetValue<string>(Registry.LocalMachine, subkeyName, "DataContext", string.Empty);
				string value5 = instance.GetValue<string>(Registry.LocalMachine, subkeyName, "InnerException", string.Empty);
				FailureRec value6 = FailureRec.Create(value, value2, value3, value4, value5);
				dictionary.Add(key, value6);
				goto IL_118;
			}
			return dictionary;
		}

		private const string ValueNameStackTrace = "StackTrace";

		private const string ValueNameDataContext = "DataContext";

		private const string ValueNameFailureType = "FailureType";

		private const string ValueNameMessage = "Message";

		private const string ValueNameInnerException = "InnerException";

		private static readonly string KeyNameFormatQuarantinedJobRoot = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxReplicationService\\QuarantinedJobs";

		private static readonly string KeyNameFormatQuarantinedJob = JobQuarantineProvider.KeyNameFormatQuarantinedJobRoot + "\\{0}";

		private static Hookable<IJobQuarantineProvider> hookableInstance = Hookable<IJobQuarantineProvider>.Create(false, new JobQuarantineProvider());
	}
}
