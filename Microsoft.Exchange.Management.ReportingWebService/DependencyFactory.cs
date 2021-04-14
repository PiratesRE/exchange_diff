using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Xml;
using Microsoft.Exchange.Management.ReportingWebService.PowerShell;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class DependencyFactory
	{
		public static void RegisterTestCreator<T>(T delegator)
		{
			Type typeFromHandle = typeof(T);
			if (DependencyFactory.creators.ContainsKey(typeFromHandle))
			{
				DependencyFactory.creators[typeFromHandle] = delegator;
				return;
			}
			DependencyFactory.creators.Add(typeFromHandle, delegator);
		}

		public static void UnRegisterTestCreator<T>()
		{
			Type typeFromHandle = typeof(T);
			if (DependencyFactory.creators.ContainsKey(typeFromHandle))
			{
				DependencyFactory.creators[typeFromHandle] = null;
			}
		}

		public static IEntity CreateEntity(string name, TaskInvocationInfo taskInvocationInfo, Dictionary<string, List<string>> reportPropertyCmdletParamsMap, IReportAnnotation annotation)
		{
			CreateEntityDelegate @delegate = DependencyFactory.GetDelegate<CreateEntityDelegate>();
			if (@delegate != null)
			{
				return @delegate(name, taskInvocationInfo, reportPropertyCmdletParamsMap, annotation);
			}
			return new Entity(name, taskInvocationInfo, reportPropertyCmdletParamsMap, annotation);
		}

		public static IReportingDataSource CreateReportingDataSource(IPrincipal principal)
		{
			CreateReportingDataSourceDelegate @delegate = DependencyFactory.GetDelegate<CreateReportingDataSourceDelegate>();
			if (@delegate != null)
			{
				return @delegate();
			}
			return new ReportingDataSource(principal);
		}

		public static IReportAnnotation CreateReportAnnotation(XmlNode annotationNode)
		{
			CreateReportAnnotationDelegate @delegate = DependencyFactory.GetDelegate<CreateReportAnnotationDelegate>();
			if (@delegate != null)
			{
				return @delegate(annotationNode);
			}
			return ReportAnnotation.Load(annotationNode);
		}

		public static IPSCommandWrapper CreatePSCommandWrapper()
		{
			CreatePSCommandWrapperDelegate @delegate = DependencyFactory.GetDelegate<CreatePSCommandWrapperDelegate>();
			if (@delegate != null)
			{
				return @delegate();
			}
			return new PSCommandWrapper();
		}

		public static IPSCommandResolver CreatePSCommandResolver(IEnumerable<string> snapIns)
		{
			CreatePSCommandResolverDelegate @delegate = DependencyFactory.GetDelegate<CreatePSCommandResolverDelegate>();
			if (@delegate != null)
			{
				return @delegate(snapIns);
			}
			return new ExchangeCommandResolver(snapIns);
		}

		private static TDelegate GetDelegate<TDelegate>() where TDelegate : class
		{
			Type typeFromHandle = typeof(TDelegate);
			if (DependencyFactory.creators.ContainsKey(typeFromHandle) && DependencyFactory.creators[typeFromHandle] != null)
			{
				return (TDelegate)((object)DependencyFactory.creators[typeFromHandle]);
			}
			return default(TDelegate);
		}

		private static readonly Dictionary<Type, object> creators = new Dictionary<Type, object>();

		private static readonly Type createEntityDelegateType = typeof(CreateEntityDelegate);

		private static readonly Type createReportingDataSourceDelegateType = typeof(CreateReportingDataSourceDelegate);

		private static readonly Type createReportAnnotationDelegateType = typeof(CreateReportAnnotationDelegate);

		private static readonly Type createPSCommandWrapperDelegateType = typeof(CreatePSCommandWrapperDelegate);

		private static readonly Type createPSCommandResolverDelegateType = typeof(CreatePSCommandResolverDelegate);
	}
}
