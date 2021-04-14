using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using www.outlook.com.highavailability.ServerLocator.v1;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[DebuggerStepThrough]
public class GetActiveCopiesForDatabaseAvailabilityGroupCompletedEventArgs : AsyncCompletedEventArgs
{
	public GetActiveCopiesForDatabaseAvailabilityGroupCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
	{
		this.results = results;
	}

	public DatabaseServerInformation[] Result
	{
		get
		{
			base.RaiseExceptionIfNecessary();
			return (DatabaseServerInformation[])this.results[0];
		}
	}

	private object[] results;
}
