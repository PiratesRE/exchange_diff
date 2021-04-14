using System;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal static class ComplianceTaskCreatorFactory
	{
		public static IComplianceTaskCreator GetInstance(ComplianceBindingType bindingType)
		{
			if (bindingType == ComplianceBindingType.ExchangeBinding)
			{
				return ComplianceTaskCreatorFactory.exchangeBindingTaskCreatorInstance.Value;
			}
			throw new NotImplementedException();
		}

		public static IDisposable SetTestHook(ComplianceBindingType bindingType, IComplianceTaskCreator testHook)
		{
			if (bindingType == ComplianceBindingType.ExchangeBinding)
			{
				return ComplianceTaskCreatorFactory.exchangeBindingTaskCreatorInstance.SetTestHook(testHook);
			}
			throw new NotImplementedException();
		}

		private static IComplianceTaskCreator CreateInstance(string assemblyName, string typeName, params object[] args)
		{
			Assembly assembly = Assembly.Load(assemblyName);
			Type type = assembly.GetType(typeName);
			return (IComplianceTaskCreator)Activator.CreateInstance(type, args);
		}

		private const int MaxUserCountForExchangeBindingTask = 1000;

		private static readonly Hookable<IComplianceTaskCreator> exchangeBindingTaskCreatorInstance = Hookable<IComplianceTaskCreator>.Create(true, ComplianceTaskCreatorFactory.CreateInstance("Microsoft.Exchange.Compliance.TaskCreator", "Microsoft.Exchange.Compliance.TaskCreator.ExchangeBindingTaskCreator", new object[]
		{
			1000
		}));
	}
}
