using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class LoadBalanceDiagnosableBase<TArgument, TResult> : DataContractDiagnosableBase<TArgument> where TArgument : LoadBalanceDiagnosableArgumentBase, new()
	{
		protected LoadBalanceDiagnosableBase(ILogger logger)
		{
			this.currentLogger = logger;
		}

		private protected DiagnosticLogger Logger { protected get; private set; }

		protected override object GetDiagnosticResult()
		{
			this.Logger = new DiagnosticLogger(this.currentLogger);
			TResult tresult = this.ProcessDiagnostic();
			TArgument arguments = base.Arguments;
			if (arguments.TraceEnabled)
			{
				return new TraceDecoratedResult
				{
					Logs = this.Logger.Logs,
					Result = tresult
				};
			}
			return tresult;
		}

		protected override XmlObjectSerializer CreateSerializer(Type type)
		{
			return new DataContractSerializer(type, type.Name, type.Namespace ?? string.Empty, Array<Type>.Empty, int.MaxValue, false, true, new LoadBalanceDataContractSurrogate(), this.CreateDataContractResolver());
		}

		protected override DataContractResolver CreateDataContractResolver()
		{
			return new LoadBalanceDataContractResolver();
		}

		protected abstract TResult ProcessDiagnostic();

		private readonly ILogger currentLogger;
	}
}
