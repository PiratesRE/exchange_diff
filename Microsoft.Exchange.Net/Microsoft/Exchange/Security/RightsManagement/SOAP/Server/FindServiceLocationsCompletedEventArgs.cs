﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.Server
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	public class FindServiceLocationsCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal FindServiceLocationsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public ServiceLocationResponse[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (ServiceLocationResponse[])this.results[0];
			}
		}

		private object[] results;
	}
}
