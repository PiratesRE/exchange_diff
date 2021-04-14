using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class AACountersInstance : PerformanceCounterInstance
	{
		internal AACountersInstance(string instanceName, AACountersInstance autoUpdateTotalInstance) : base(instanceName, "MSExchangeUMAutoAttendant")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalCalls = new ExPerformanceCounter(base.CategoryName, "Total Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalCalls);
				this.BusinessHoursCalls = new ExPerformanceCounter(base.CategoryName, "Business Hours Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BusinessHoursCalls);
				this.OutOfHoursCalls = new ExPerformanceCounter(base.CategoryName, "Out of Hours Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutOfHoursCalls);
				this.DisconnectedWithoutInput = new ExPerformanceCounter(base.CategoryName, "Disconnected Without Input", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DisconnectedWithoutInput);
				this.TransferredCount = new ExPerformanceCounter(base.CategoryName, "Transferred Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TransferredCount);
				this.DirectoryAccessed = new ExPerformanceCounter(base.CategoryName, "Directory Accessed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessed);
				this.DirectoryAccessedByExtension = new ExPerformanceCounter(base.CategoryName, "Directory Accessed by Extension", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedByExtension);
				this.DirectoryAccessedByDialByName = new ExPerformanceCounter(base.CategoryName, "Directory Accessed by Dial by Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedByDialByName);
				this.DirectoryAccessedSuccessfullyByDialByName = new ExPerformanceCounter(base.CategoryName, "Directory Accessed Successfully by Dial by Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedSuccessfullyByDialByName);
				this.DirectoryAccessedBySpokenName = new ExPerformanceCounter(base.CategoryName, "Directory Accessed by Spoken Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedBySpokenName);
				this.DirectoryAccessedSuccessfullyBySpokenName = new ExPerformanceCounter(base.CategoryName, "Directory Accessed Successfully by Spoken Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedSuccessfullyBySpokenName);
				this.OperatorTransfers = new ExPerformanceCounter(base.CategoryName, "Operator Transfers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OperatorTransfers);
				this.CustomMenuOptions = new ExPerformanceCounter(base.CategoryName, "Custom Menu Options", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CustomMenuOptions);
				this.MenuOption1 = new ExPerformanceCounter(base.CategoryName, "Menu Option 1 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption1);
				this.MenuOption2 = new ExPerformanceCounter(base.CategoryName, "Menu Option 2 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption2);
				this.MenuOption3 = new ExPerformanceCounter(base.CategoryName, "Menu Option 3 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption3);
				this.MenuOption4 = new ExPerformanceCounter(base.CategoryName, "Menu Option 4 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption4);
				this.MenuOption5 = new ExPerformanceCounter(base.CategoryName, "Menu Option 5 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption5);
				this.MenuOption6 = new ExPerformanceCounter(base.CategoryName, "Menu Option 6 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption6);
				this.MenuOption7 = new ExPerformanceCounter(base.CategoryName, "Menu Option 7 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption7);
				this.MenuOption8 = new ExPerformanceCounter(base.CategoryName, "Menu Option 8 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption8);
				this.MenuOption9 = new ExPerformanceCounter(base.CategoryName, "Menu Option 9 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption9);
				this.MenuOptionTimeout = new ExPerformanceCounter(base.CategoryName, "Menu Option Timed Out", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOptionTimeout);
				this.AverageCallTime = new ExPerformanceCounter(base.CategoryName, "Average Call Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageCallTime);
				this.AverageRecentCallTime = new ExPerformanceCounter(base.CategoryName, "Average Recent Call Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageRecentCallTime);
				this.CallsDisconnectedOnIrrecoverableExternalError = new ExPerformanceCounter(base.CategoryName, "Calls Disconnected by UM on Irrecoverable External Error", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CallsDisconnectedOnIrrecoverableExternalError);
				this.SpeechCalls = new ExPerformanceCounter(base.CategoryName, "Calls with Speech Input", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SpeechCalls);
				this.AmbiguousNameTransfers = new ExPerformanceCounter(base.CategoryName, "Ambiguous Name Transfers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AmbiguousNameTransfers);
				this.DisallowedTransfers = new ExPerformanceCounter(base.CategoryName, "Disallowed Transfers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DisallowedTransfers);
				this.NameSpoken = new ExPerformanceCounter(base.CategoryName, "Calls with Spoken Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NameSpoken);
				this.TransfersToSendMessage = new ExPerformanceCounter(base.CategoryName, "Calls with Sent Message", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TransfersToSendMessage);
				this.TransfersToDtmfFallbackAutoAttendant = new ExPerformanceCounter(base.CategoryName, "Calls with DTMF fallback", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TransfersToDtmfFallbackAutoAttendant);
				this.SentToAutoAttendant = new ExPerformanceCounter(base.CategoryName, "Sent to Auto Attendant", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SentToAutoAttendant);
				this.OperatorTransfersInitiatedByUser = new ExPerformanceCounter(base.CategoryName, "Operator Transfers Requested by User", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OperatorTransfersInitiatedByUser);
				this.PercentageSuccessfulCalls = new ExPerformanceCounter(base.CategoryName, "% Successful Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageSuccessfulCalls);
				this.PercentageSuccessfulCalls_Base = new ExPerformanceCounter(base.CategoryName, "Base counter for % Successful Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageSuccessfulCalls_Base);
				this.OperatorTransfersInitiatedByUserFromOpeningMenu = new ExPerformanceCounter(base.CategoryName, "Operator Transfers Requested by User from Opening Menu", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OperatorTransfersInitiatedByUserFromOpeningMenu);
				long num = this.TotalCalls.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		internal AACountersInstance(string instanceName) : base(instanceName, "MSExchangeUMAutoAttendant")
		{
			bool flag = false;
			List<ExPerformanceCounter> list = new List<ExPerformanceCounter>();
			try
			{
				this.TotalCalls = new ExPerformanceCounter(base.CategoryName, "Total Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TotalCalls);
				this.BusinessHoursCalls = new ExPerformanceCounter(base.CategoryName, "Business Hours Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.BusinessHoursCalls);
				this.OutOfHoursCalls = new ExPerformanceCounter(base.CategoryName, "Out of Hours Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OutOfHoursCalls);
				this.DisconnectedWithoutInput = new ExPerformanceCounter(base.CategoryName, "Disconnected Without Input", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DisconnectedWithoutInput);
				this.TransferredCount = new ExPerformanceCounter(base.CategoryName, "Transferred Count", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TransferredCount);
				this.DirectoryAccessed = new ExPerformanceCounter(base.CategoryName, "Directory Accessed", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessed);
				this.DirectoryAccessedByExtension = new ExPerformanceCounter(base.CategoryName, "Directory Accessed by Extension", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedByExtension);
				this.DirectoryAccessedByDialByName = new ExPerformanceCounter(base.CategoryName, "Directory Accessed by Dial by Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedByDialByName);
				this.DirectoryAccessedSuccessfullyByDialByName = new ExPerformanceCounter(base.CategoryName, "Directory Accessed Successfully by Dial by Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedSuccessfullyByDialByName);
				this.DirectoryAccessedBySpokenName = new ExPerformanceCounter(base.CategoryName, "Directory Accessed by Spoken Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedBySpokenName);
				this.DirectoryAccessedSuccessfullyBySpokenName = new ExPerformanceCounter(base.CategoryName, "Directory Accessed Successfully by Spoken Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DirectoryAccessedSuccessfullyBySpokenName);
				this.OperatorTransfers = new ExPerformanceCounter(base.CategoryName, "Operator Transfers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OperatorTransfers);
				this.CustomMenuOptions = new ExPerformanceCounter(base.CategoryName, "Custom Menu Options", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CustomMenuOptions);
				this.MenuOption1 = new ExPerformanceCounter(base.CategoryName, "Menu Option 1 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption1);
				this.MenuOption2 = new ExPerformanceCounter(base.CategoryName, "Menu Option 2 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption2);
				this.MenuOption3 = new ExPerformanceCounter(base.CategoryName, "Menu Option 3 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption3);
				this.MenuOption4 = new ExPerformanceCounter(base.CategoryName, "Menu Option 4 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption4);
				this.MenuOption5 = new ExPerformanceCounter(base.CategoryName, "Menu Option 5 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption5);
				this.MenuOption6 = new ExPerformanceCounter(base.CategoryName, "Menu Option 6 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption6);
				this.MenuOption7 = new ExPerformanceCounter(base.CategoryName, "Menu Option 7 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption7);
				this.MenuOption8 = new ExPerformanceCounter(base.CategoryName, "Menu Option 8 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption8);
				this.MenuOption9 = new ExPerformanceCounter(base.CategoryName, "Menu Option 9 Used", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOption9);
				this.MenuOptionTimeout = new ExPerformanceCounter(base.CategoryName, "Menu Option Timed Out", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.MenuOptionTimeout);
				this.AverageCallTime = new ExPerformanceCounter(base.CategoryName, "Average Call Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageCallTime);
				this.AverageRecentCallTime = new ExPerformanceCounter(base.CategoryName, "Average Recent Call Time", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AverageRecentCallTime);
				this.CallsDisconnectedOnIrrecoverableExternalError = new ExPerformanceCounter(base.CategoryName, "Calls Disconnected by UM on Irrecoverable External Error", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.CallsDisconnectedOnIrrecoverableExternalError);
				this.SpeechCalls = new ExPerformanceCounter(base.CategoryName, "Calls with Speech Input", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SpeechCalls);
				this.AmbiguousNameTransfers = new ExPerformanceCounter(base.CategoryName, "Ambiguous Name Transfers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.AmbiguousNameTransfers);
				this.DisallowedTransfers = new ExPerformanceCounter(base.CategoryName, "Disallowed Transfers", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.DisallowedTransfers);
				this.NameSpoken = new ExPerformanceCounter(base.CategoryName, "Calls with Spoken Name", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.NameSpoken);
				this.TransfersToSendMessage = new ExPerformanceCounter(base.CategoryName, "Calls with Sent Message", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TransfersToSendMessage);
				this.TransfersToDtmfFallbackAutoAttendant = new ExPerformanceCounter(base.CategoryName, "Calls with DTMF fallback", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.TransfersToDtmfFallbackAutoAttendant);
				this.SentToAutoAttendant = new ExPerformanceCounter(base.CategoryName, "Sent to Auto Attendant", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.SentToAutoAttendant);
				this.OperatorTransfersInitiatedByUser = new ExPerformanceCounter(base.CategoryName, "Operator Transfers Requested by User", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OperatorTransfersInitiatedByUser);
				this.PercentageSuccessfulCalls = new ExPerformanceCounter(base.CategoryName, "% Successful Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageSuccessfulCalls);
				this.PercentageSuccessfulCalls_Base = new ExPerformanceCounter(base.CategoryName, "Base counter for % Successful Calls", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.PercentageSuccessfulCalls_Base);
				this.OperatorTransfersInitiatedByUserFromOpeningMenu = new ExPerformanceCounter(base.CategoryName, "Operator Transfers Requested by User from Opening Menu", instanceName, null, new ExPerformanceCounter[0]);
				list.Add(this.OperatorTransfersInitiatedByUserFromOpeningMenu);
				long num = this.TotalCalls.RawValue;
				num += 1L;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					foreach (ExPerformanceCounter exPerformanceCounter in list)
					{
						exPerformanceCounter.Close();
					}
				}
			}
			this.counters = list.ToArray();
		}

		public override void GetPerfCounterDiagnosticsInfo(XElement topElement)
		{
			XElement xelement = null;
			foreach (ExPerformanceCounter exPerformanceCounter in this.counters)
			{
				try
				{
					if (xelement == null)
					{
						xelement = new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.InstanceName));
						topElement.Add(xelement);
					}
					xelement.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					topElement.Add(content);
				}
			}
		}

		public readonly ExPerformanceCounter TotalCalls;

		public readonly ExPerformanceCounter BusinessHoursCalls;

		public readonly ExPerformanceCounter OutOfHoursCalls;

		public readonly ExPerformanceCounter DisconnectedWithoutInput;

		public readonly ExPerformanceCounter TransferredCount;

		public readonly ExPerformanceCounter DirectoryAccessed;

		public readonly ExPerformanceCounter DirectoryAccessedByExtension;

		public readonly ExPerformanceCounter DirectoryAccessedByDialByName;

		public readonly ExPerformanceCounter DirectoryAccessedSuccessfullyByDialByName;

		public readonly ExPerformanceCounter DirectoryAccessedBySpokenName;

		public readonly ExPerformanceCounter DirectoryAccessedSuccessfullyBySpokenName;

		public readonly ExPerformanceCounter OperatorTransfers;

		public readonly ExPerformanceCounter CustomMenuOptions;

		public readonly ExPerformanceCounter MenuOption1;

		public readonly ExPerformanceCounter MenuOption2;

		public readonly ExPerformanceCounter MenuOption3;

		public readonly ExPerformanceCounter MenuOption4;

		public readonly ExPerformanceCounter MenuOption5;

		public readonly ExPerformanceCounter MenuOption6;

		public readonly ExPerformanceCounter MenuOption7;

		public readonly ExPerformanceCounter MenuOption8;

		public readonly ExPerformanceCounter MenuOption9;

		public readonly ExPerformanceCounter MenuOptionTimeout;

		public readonly ExPerformanceCounter AverageCallTime;

		public readonly ExPerformanceCounter AverageRecentCallTime;

		public readonly ExPerformanceCounter CallsDisconnectedOnIrrecoverableExternalError;

		public readonly ExPerformanceCounter SpeechCalls;

		public readonly ExPerformanceCounter AmbiguousNameTransfers;

		public readonly ExPerformanceCounter DisallowedTransfers;

		public readonly ExPerformanceCounter NameSpoken;

		public readonly ExPerformanceCounter TransfersToSendMessage;

		public readonly ExPerformanceCounter TransfersToDtmfFallbackAutoAttendant;

		public readonly ExPerformanceCounter SentToAutoAttendant;

		public readonly ExPerformanceCounter OperatorTransfersInitiatedByUser;

		public readonly ExPerformanceCounter PercentageSuccessfulCalls;

		public readonly ExPerformanceCounter PercentageSuccessfulCalls_Base;

		public readonly ExPerformanceCounter OperatorTransfersInitiatedByUserFromOpeningMenu;
	}
}
