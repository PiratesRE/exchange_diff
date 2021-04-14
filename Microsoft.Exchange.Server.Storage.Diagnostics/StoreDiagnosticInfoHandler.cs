using System;
using System.Xml.Linq;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal abstract class StoreDiagnosticInfoHandler : IDiagnosable
	{
		protected StoreDiagnosticInfoHandler(string componentName)
		{
			this.componentName = componentName;
		}

		public string GetDiagnosticComponentName()
		{
			StoreDiagnosticInfoHandler.<>c__DisplayClass1 CS$<>8__locals1 = new StoreDiagnosticInfoHandler.<>c__DisplayClass1();
			CS$<>8__locals1.<>4__this = this;
			ExecutionDiagnostics executionDiagnostics = new ExecutionDiagnostics();
			CS$<>8__locals1.result = null;
			WatsonOnUnhandledException.Guard(executionDiagnostics, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<GetDiagnosticComponentName>b__0)));
			return CS$<>8__locals1.result;
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			StoreDiagnosticInfoHandler.<>c__DisplayClass4 CS$<>8__locals1 = new StoreDiagnosticInfoHandler.<>c__DisplayClass4();
			CS$<>8__locals1.parameters = parameters;
			CS$<>8__locals1.<>4__this = this;
			ExecutionDiagnostics executionDiagnostics = new ExecutionDiagnostics();
			CS$<>8__locals1.result = null;
			WatsonOnUnhandledException.Guard(executionDiagnostics, new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<GetDiagnosticInfo>b__3)));
			return CS$<>8__locals1.result;
		}

		public abstract XElement GetDiagnosticQuery(DiagnosableParameters parameters);

		public void Register()
		{
			ProcessAccessManager.RegisterComponent(this);
		}

		public void Deregister()
		{
			ProcessAccessManager.UnregisterComponent(this);
		}

		private readonly string componentName;
	}
}
