using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Sqm;

namespace Microsoft.Exchange.Configuration.SQM
{
	internal sealed class CmdletSqmSession : SqmSession
	{
		private CmdletSqmSession() : base(SqmAppID.Configuration, SqmSession.Scope.AppDomain)
		{
			base.Open();
			CmdletSqmSession.SetConsoleCtrlHandler(CmdletSqmSession.breakHandler, true);
			AppDomain.CurrentDomain.ProcessExit += CmdletSqmSession.CloseSessionEventHandler;
			AppDomain.CurrentDomain.DomainUnload += delegate(object param0, EventArgs param1)
			{
				CmdletSqmSession.CloseSessionEventHandler(null, EventArgs.Empty);
				AppDomain.CurrentDomain.ProcessExit -= CmdletSqmSession.CloseSessionEventHandler;
			};
		}

		public static CmdletSqmSession Instance
		{
			get
			{
				if (CmdletSqmSession.instance == null)
				{
					CmdletSqmSession.instance = new CmdletSqmSession();
				}
				return CmdletSqmSession.instance;
			}
		}

		public static bool BreakHandler(int dwCtrlType)
		{
			switch (dwCtrlType)
			{
			case 1:
			case 2:
			case 5:
			case 6:
				if (CmdletSqmSession.Instance != null)
				{
					CmdletSqmSession.Instance.Close();
				}
				break;
			}
			return false;
		}

		public void WriteSQMSession(string cmdletName, string[] paraNames, string context, uint iterationNumber, uint runtime, SqmErrorRecord[] errors)
		{
			lock (this.mutex)
			{
				Guid guid = Guid.NewGuid();
				base.AddToStreamDataPoint(SqmDataID.CMDLET_INFRA_CMDLETINFO, new object[]
				{
					cmdletName,
					iterationNumber,
					runtime,
					context,
					(uint)guid.GetHashCode()
				});
				foreach (string text in paraNames)
				{
					base.AddToStreamDataPoint(SqmDataID.CMDLET_INFRA_PARAMETER_NAME, new object[]
					{
						text,
						(uint)guid.GetHashCode()
					});
				}
				foreach (SqmErrorRecord sqmErrorRecord in errors)
				{
					base.AddToStreamDataPoint(SqmDataID.CMDLET_INFRA_ERROR_NAME, new object[]
					{
						sqmErrorRecord.ExceptionType,
						(uint)guid.GetHashCode(),
						sqmErrorRecord.ErrorId,
						context
					});
				}
			}
		}

		private static void CloseSessionEventHandler(object sender, EventArgs e)
		{
			CmdletSqmSession.Instance.Close();
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern bool SetConsoleCtrlHandler(CmdletSqmSession.ConsoleCtrlDelegate HandlerRoutine, bool Add);

		public const int SqmErrorNumberPerCmdlet = 6;

		private const int CTRL_C_EVENT = 0;

		private const int CTRL_BREAK_EVENT = 1;

		private const int CTRL_CLOSE_EVENT = 2;

		private const int CTRL_LOGOFF_EVENT = 5;

		private const int CTRL_SHUTDOWN_EVENT = 6;

		private static CmdletSqmSession.ConsoleCtrlDelegate breakHandler = new CmdletSqmSession.ConsoleCtrlDelegate(CmdletSqmSession.BreakHandler);

		private static CmdletSqmSession instance = null;

		private object mutex = new object();

		public delegate bool ConsoleCtrlDelegate(int dwCtrlType);
	}
}
