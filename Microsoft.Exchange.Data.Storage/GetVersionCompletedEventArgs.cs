using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using www.outlook.com.highavailability.ServerLocator.v1;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class GetVersionCompletedEventArgs : AsyncCompletedEventArgs
{
	public GetVersionCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
	{
		this.results = results;
	}

	public ServiceVersion Result
	{
		get
		{
			base.RaiseExceptionIfNecessary();
			return (ServiceVersion)this.results[0];
		}
	}

	private object[] results;
}
