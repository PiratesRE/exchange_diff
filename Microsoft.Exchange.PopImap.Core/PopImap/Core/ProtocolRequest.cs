using System;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PopImap.Core
{
	internal abstract class ProtocolRequest
	{
		protected ProtocolRequest(ResponseFactory factory, string arguments)
		{
			this.factory = factory;
			this.arguments = arguments;
			this.parseResult = ParseResult.notYetParsed;
			this.perfCounterTotal = ProtocolBaseServices.VirtualServer.InvalidCommands;
			this.perfCounterFailures = ProtocolBaseServices.VirtualServer.InvalidCommands;
		}

		public virtual ExPerformanceCounter PerfCounterTotal
		{
			get
			{
				return this.perfCounterTotal;
			}
			set
			{
				this.perfCounterTotal = value;
			}
		}

		public virtual ExPerformanceCounter PerfCounterFailures
		{
			get
			{
				return this.perfCounterFailures;
			}
			set
			{
				this.perfCounterFailures = value;
			}
		}

		public virtual bool NeedToDelayStoreAction
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsComplete
		{
			get
			{
				return true;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public virtual bool NeedsStoreConnection
		{
			get
			{
				return false;
			}
		}

		public virtual ParseResult ParseResult
		{
			get
			{
				return this.parseResult;
			}
			set
			{
				this.parseResult = value;
			}
		}

		public ResponseFactory ResponseFactory
		{
			get
			{
				return this.factory;
			}
		}

		public ProtocolSession Session
		{
			get
			{
				return this.factory.Session;
			}
		}

		public MailboxSession Store
		{
			get
			{
				return this.factory.Store;
			}
		}

		public string Arguments
		{
			get
			{
				return this.arguments;
			}
			set
			{
				this.arguments = value;
			}
		}

		public virtual bool VerifyState()
		{
			return true;
		}

		public virtual void ParseArguments()
		{
			if (this.arguments != null && this.arguments.Length > 0)
			{
				this.parseResult = ParseResult.invalidNumberOfArguments;
				return;
			}
			this.parseResult = ParseResult.success;
		}

		public abstract ProtocolResponse Process();

		public override string ToString()
		{
			return this.factory.Session.ToString() + " " + base.ToString();
		}

		public virtual TimeSpan GetBudgetActionTimeout()
		{
			return Budget.GetMaxActionTime(CostType.CAS);
		}

		internal Decoder GetPasswordDecoder(int codePage)
		{
			Decoder decoder = Encoding.GetEncoding(codePage).GetDecoder();
			if (decoder == null)
			{
				decoder = Encoding.GetEncoding(20127).GetDecoder();
			}
			return decoder;
		}

		private ResponseFactory factory;

		private string arguments;

		private ExPerformanceCounter perfCounterTotal;

		private ExPerformanceCounter perfCounterFailures;

		private ParseResult parseResult;
	}
}
