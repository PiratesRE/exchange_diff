using System;
using System.Collections;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting.Providers
{
	internal class SmtpCheckerProviderImpl : ISmtpCheckerProvider
	{
		internal SmtpCheckerProviderImpl() : this(new Func<string, object>(SmtpCheckerProviderImpl.CreateTargetObject))
		{
		}

		internal SmtpCheckerProviderImpl(Func<string, object> activator)
		{
			this.createTargetFunction = activator;
		}

		public IEnumerable GetMxRecords(Fqdn domain, IConfigDataProvider configDataProvider)
		{
			return this.GetData("Microsoft.Exchange.Hygiene.Reporting.SMTPVerificationTests.VerifyMxRecord", configDataProvider, new object[]
			{
				domain
			});
		}

		public IEnumerable GetOutboundConnectors(Fqdn domain, IConfigDataProvider configDataProvider)
		{
			return this.GetData("Microsoft.Exchange.Hygiene.Reporting.SMTPVerificationTests.VerifyOutboundConnector", configDataProvider, new object[]
			{
				domain
			});
		}

		public IEnumerable GetServiceDeliveries(SmtpAddress recipient, IConfigDataProvider configDataProvider)
		{
			return this.GetData("Microsoft.Exchange.Hygiene.Reporting.SMTPVerificationTests.VerifyServiceDelivery", configDataProvider, new object[]
			{
				recipient
			});
		}

		private static object CreateTargetObject(string targetInstanceTypeName)
		{
			return Activator.CreateInstance("Microsoft.Exchange.Hygiene.Reporting.SMTPVerificationTests", targetInstanceTypeName).Unwrap();
		}

		private IEnumerable GetData(string smtpDataTypeName, IConfigDataProvider configDataProvider, params object[] parameters)
		{
			object obj = this.createTargetFunction(smtpDataTypeName);
			Type type = obj.GetType();
			PropertyInfo property = type.GetProperty("ConfigSession");
			if (property != null)
			{
				property.SetValue(obj, configDataProvider);
			}
			MethodInfo method = type.GetMethod("Execute");
			return (IEnumerable)Schema.Utilities.Invoke(method, obj, parameters);
		}

		private readonly Func<string, object> createTargetFunction;
	}
}
