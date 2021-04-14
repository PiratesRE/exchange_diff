using System;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.Rpc;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	public class TestUMConnectivityHelper
	{
		[Serializable]
		public abstract class UMConnectivityResults : UMDiagnosticObject
		{
			public string UmIPAddress
			{
				get
				{
					return this.umIPAddress;
				}
				set
				{
					this.umIPAddress = value;
				}
			}

			public bool EntireOperationSuccess
			{
				get
				{
					return this.entireOperationSuccess;
				}
				set
				{
					this.entireOperationSuccess = value;
				}
			}

			public LocalizedString ReasonForFailure
			{
				get
				{
					return this.reasonForFailure;
				}
				set
				{
					this.reasonForFailure = value;
				}
			}

			public override ObjectId Identity
			{
				get
				{
					return new ConfigObjectId(Guid.NewGuid().ToString());
				}
			}

			protected UMConnectivityResults()
			{
				this.UmIPAddress = string.Empty;
				this.ReasonForFailure = LocalizedString.Empty;
				this.EntireOperationSuccess = false;
			}

			private string umIPAddress;

			private LocalizedString reasonForFailure;

			private bool entireOperationSuccess;
		}

		[Serializable]
		public class LocalUMConnectivityOptionsResults : TestUMConnectivityHelper.UMConnectivityResults
		{
			public string Diagnostics
			{
				get
				{
					return this.diagnostics;
				}
				set
				{
					this.diagnostics = value;
				}
			}

			private string diagnostics = string.Empty;
		}

		[Serializable]
		public class UMConnectivityCallResults : TestUMConnectivityHelper.UMConnectivityResults
		{
			public UMConnectivityCallResults()
			{
				this.currCalls = "0";
			}

			public string CurrCalls
			{
				get
				{
					return this.currCalls;
				}
				set
				{
					this.currCalls = value;
				}
			}

			public double Latency
			{
				get
				{
					return this.latency;
				}
				set
				{
					this.latency = value;
				}
			}

			public bool OutBoundSIPCallSuccess
			{
				get
				{
					return this.outBoundSIPCallSuccess;
				}
				set
				{
					this.outBoundSIPCallSuccess = value;
				}
			}

			private string currCalls;

			private double latency;

			private bool outBoundSIPCallSuccess;
		}

		[Serializable]
		public class LocalUMConnectivityResults : TestUMConnectivityHelper.UMConnectivityCallResults
		{
			public LocalUMConnectivityResults()
			{
				this.totalQueuedMessages = string.Empty;
				this.umserverAcceptingCallAnsweringMessages = true;
			}

			public bool UmserverAcceptingCallAnsweringMessages
			{
				get
				{
					return this.umserverAcceptingCallAnsweringMessages;
				}
				set
				{
					this.umserverAcceptingCallAnsweringMessages = value;
				}
			}

			public string TotalQueuedMessages
			{
				get
				{
					return this.totalQueuedMessages;
				}
				set
				{
					this.totalQueuedMessages = value;
				}
			}

			private string totalQueuedMessages;

			private bool umserverAcceptingCallAnsweringMessages;
		}

		[Serializable]
		public class RemoteUMConnectivityResults : TestUMConnectivityHelper.UMConnectivityCallResults
		{
			public RemoteUMConnectivityResults()
			{
				this.expectedDiagnosticSequence = string.Empty;
				this.receivedDiagnosticSequence = string.Empty;
			}

			public RemoteUMConnectivityResults(TestUMConnectivityHelper.UMConnectivityResults results) : this()
			{
				TestUMConnectivityHelper.UMConnectivityCallResults umconnectivityCallResults = (TestUMConnectivityHelper.UMConnectivityCallResults)results;
				base.CurrCalls = umconnectivityCallResults.CurrCalls;
				base.Latency = umconnectivityCallResults.Latency;
				base.UmIPAddress = umconnectivityCallResults.UmIPAddress;
				base.OutBoundSIPCallSuccess = umconnectivityCallResults.OutBoundSIPCallSuccess;
				base.EntireOperationSuccess = umconnectivityCallResults.EntireOperationSuccess;
				base.ReasonForFailure = umconnectivityCallResults.ReasonForFailure;
			}

			public string ExpectedDigits
			{
				get
				{
					return this.expectedDiagnosticSequence;
				}
				set
				{
					this.expectedDiagnosticSequence = value;
				}
			}

			public string ReceivedDigits
			{
				get
				{
					return this.receivedDiagnosticSequence;
				}
				set
				{
					this.receivedDiagnosticSequence = value;
				}
			}

			private string expectedDiagnosticSequence;

			private string receivedDiagnosticSequence;
		}

		[Serializable]
		public class TUILogonEnumerateResults : TestUMConnectivityHelper.LocalUMConnectivityResults
		{
			public TUILogonEnumerateResults()
			{
				this.mailboxServer = string.Empty;
				this.gotPin = true;
			}

			public string MailboxServerBeingTested
			{
				get
				{
					return this.mailboxServer;
				}
				set
				{
					this.mailboxServer = value;
				}
			}

			public bool SuccessfullyRetrievedPINForValidUMUser
			{
				get
				{
					return this.gotPin;
				}
				set
				{
					this.gotPin = value;
				}
			}

			private string mailboxServer;

			private bool gotPin;
		}

		[Serializable]
		public class ResetPinResults : UMDiagnosticObject
		{
			public ResetPinResults()
			{
				this.mailboxServer = string.Empty;
			}

			public string MailboxServerBeingTested
			{
				get
				{
					return this.mailboxServer;
				}
				set
				{
					this.mailboxServer = value;
				}
			}

			public double Latency
			{
				get
				{
					return this.latency;
				}
				set
				{
					this.latency = value;
				}
			}

			public bool SuccessfullyResetPINForValidUMUser
			{
				get
				{
					return this.resetPinSuccess;
				}
				set
				{
					this.resetPinSuccess = value;
				}
			}

			public bool SuccessfullyResetPasswordForValidUMUser
			{
				get
				{
					return this.resetPasswdSuccess;
				}
				set
				{
					this.resetPasswdSuccess = value;
				}
			}

			public override ObjectId Identity
			{
				get
				{
					return new ConfigObjectId(Guid.NewGuid().ToString());
				}
			}

			private string mailboxServer;

			private bool resetPasswdSuccess;

			private double latency;

			private bool resetPinSuccess;
		}

		internal class TestMailboxUserDetails
		{
			internal TestMailboxUserDetails(string phone, string pin, string dialPlan, string mbxServer)
			{
				this.phone = phone;
				this.pin = pin;
				this.mailboxServer = mbxServer;
				this.dialplan = dialPlan;
			}

			internal string Phone
			{
				get
				{
					return this.phone;
				}
			}

			internal string DialPlan
			{
				get
				{
					return this.dialplan;
				}
			}

			internal string Pin
			{
				get
				{
					return this.pin;
				}
			}

			internal string MailboxServer
			{
				get
				{
					return this.mailboxServer;
				}
			}

			private readonly string phone;

			private readonly string pin;

			private readonly string mailboxServer;

			private readonly string dialplan;
		}

		internal class UsersForResetPin
		{
			internal UsersForResetPin(ExchangePrincipal exp, NetworkCredential netc, ADUser aduser, string server)
			{
				this.ep = exp;
				this.nc = netc;
				this.user = aduser;
				this.mailboxServer = server;
			}

			internal ExchangePrincipal ExPrincipal
			{
				get
				{
					return this.ep;
				}
			}

			internal NetworkCredential NetCreds
			{
				get
				{
					return this.nc;
				}
			}

			internal ADUser Aduser
			{
				get
				{
					return this.user;
				}
			}

			internal string MailboxServer
			{
				get
				{
					return this.mailboxServer;
				}
			}

			private ExchangePrincipal ep;

			private NetworkCredential nc;

			private ADUser user;

			private readonly string mailboxServer;
		}
	}
}
