using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using www.outlook.com.highavailability.ServerLocator.v1;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class GetServerForDatabaseCompletedEventArgs : AsyncCompletedEventArgs
{
	public GetServerForDatabaseCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
	{
		this.results = results;
	}

	public DatabaseServerInformation Result
	{
		get
		{
			base.RaiseExceptionIfNecessary();
			return (DatabaseServerInformation)this.results[0];
		}
	}

	private object[] results;
}
