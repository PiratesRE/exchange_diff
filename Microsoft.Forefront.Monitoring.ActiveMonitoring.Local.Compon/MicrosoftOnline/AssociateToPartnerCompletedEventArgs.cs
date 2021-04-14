﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	public class AssociateToPartnerCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal AssociateToPartnerCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		public Contract Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (Contract)this.results[0];
			}
		}

		private object[] results;
	}
}
