using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess
{
	internal abstract class LocalProbe : MapiProbe
	{
		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			if (probeDefinition == null)
			{
				throw new LocalizedException(Strings.WrongDefinitionType);
			}
			AutoPopulateDefinition autoPopulateDefinition = new AutoPopulateDefinition(this.GetProbeType(), probeDefinition);
			autoPopulateDefinition.ValidateAndAutoFill(propertyBag);
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			bool flag = this.GetProbeType() == ProbeType.Ctp;
			List<PropertyInformation> list = new List<PropertyInformation>
			{
				new PropertyInformation("Identity", Strings.Identity, true),
				new PropertyInformation("Account", Strings.MonitoringAccount, false),
				new PropertyInformation("AccountDisplayName", Strings.AccountDisplayName, false),
				new PropertyInformation("Endpoint", Strings.Endpoint, false),
				new PropertyInformation("SecondaryEndpoint", Strings.SecondaryEndpoint, false),
				new PropertyInformation("ItemTargetExtension", Strings.ExtensionAttributes, false)
			};
			if (flag)
			{
				list.Add(new PropertyInformation("Password", Strings.MonitoringAccountPassword, false));
			}
			return list;
		}

		protected abstract ProbeType GetProbeType();

		public class DeepTest : LocalProbe
		{
			protected override bool ShouldCreateRestrictedCredentials()
			{
				return true;
			}

			protected override ITask CreateTask()
			{
				return new EmsmdbTask(base.Context);
			}

			protected sealed override ProbeType GetProbeType()
			{
				return ProbeType.DeepTest;
			}

			protected override void ProcessTaskException(Exception ex)
			{
				if (MapiProbe.DidProbeFailDueToPassiveMDB(ex))
				{
					base.SetRootCause("Passive");
					return;
				}
				base.ProcessTaskException(ex);
			}
		}

		public class MapiHttpDeepTest : LocalProbe.DeepTest
		{
			protected override ITask CreateTask()
			{
				return new EmsmdbMapiHttpTask(base.Context);
			}
		}

		public class SelfTest : LocalProbe
		{
			protected override bool ShouldCreateRestrictedCredentials()
			{
				return true;
			}

			protected override ITask CreateTask()
			{
				return new CompositeTask(base.Context, new ITask[]
				{
					new VerifyRpcProxyTask(base.Context),
					new DummyRpcTask(base.Context)
				});
			}

			protected sealed override ProbeType GetProbeType()
			{
				return ProbeType.SelfTest;
			}
		}

		public class MapiHttpSelfTest : LocalProbe.SelfTest
		{
			protected override ITask CreateTask()
			{
				return new CompositeTask(base.Context, new ITask[]
				{
					new DummyMapiHttpTask(base.Context)
				});
			}
		}

		public class Ctp : LocalProbe
		{
			protected override ITask CreateTask()
			{
				return new CompositeTask(base.Context, new ITask[]
				{
					new VerifyRpcProxyTask(base.Context),
					new EmsmdbTask(base.Context)
				});
			}

			protected sealed override ProbeType GetProbeType()
			{
				return ProbeType.Ctp;
			}

			protected override void ProcessTaskException(Exception ex)
			{
				if (base.DidProbeFailDueToDatabaseMountedElsewhere(ex))
				{
					base.SetRootCause("Passive");
					throw new AggregateException(new Exception[]
					{
						ex
					});
				}
				base.ProcessTaskException(ex);
			}
		}

		public class MapiHttpCtp : LocalProbe.Ctp
		{
			protected override ITask CreateTask()
			{
				return new CompositeTask(base.Context, new ITask[]
				{
					new EmsmdbMapiHttpTask(base.Context)
				});
			}

			protected override void ProcessTaskException(Exception ex)
			{
				if (base.DidProbeFailDueToInvalidRequestType(ex))
				{
					base.SetRootCause("MapiHttpVersionMismatch");
					return;
				}
				base.ProcessTaskException(ex);
			}
		}
	}
}
