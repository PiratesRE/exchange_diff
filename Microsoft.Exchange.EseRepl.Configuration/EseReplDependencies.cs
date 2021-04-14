using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Common.Extensions;
using Microsoft.Exchange.Diagnostics.Components.EseRepl;
using Microsoft.Exchange.EseRepl.Common;
using Microsoft.Practices.Unity;

namespace Microsoft.Exchange.EseRepl.Configuration
{
	internal class EseReplDependencies
	{
		public static IUnityContainer Initialize()
		{
			Assert instance = Assert.Instance;
			Assert.Initialize(instance);
			return new UnityContainer().RegisterInstance<IAssert>(instance).RegisterInstance<IRegistryReader>(RegistryReader.Instance).RegisterInstance<IRegistryWriter>(RegistryWriter.Instance).RegisterInstance<ISerialization>(Serialization.Instance).RegisterInstance<ITracer>(0.ToString(), new Tracer(ExTraceGlobals.DagNetChooserTracer)).RegisterInstance<ITracer>(1.ToString(), new Tracer(ExTraceGlobals.DagNetEnvironmentTracer)).RegisterInstance<IEseReplConfig>(EseReplConfig.Instance);
		}
	}
}
