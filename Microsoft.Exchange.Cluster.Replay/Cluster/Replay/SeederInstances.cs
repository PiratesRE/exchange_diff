using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SeederInstances : SafeInstanceTable<SeederInstanceContainer>
	{
		internal override void RemoveInstance(SeederInstanceContainer instance)
		{
			bool flag = false;
			this.m_rwLock.AcquireWriterLock(-1);
			try
			{
				if (this.m_instances.ContainsKey(instance.Identity))
				{
					flag = true;
					this.m_instances.Remove(instance.Identity);
				}
			}
			finally
			{
				this.m_rwLock.ReleaseWriterLock();
			}
			if (flag)
			{
				instance.WaitUntilStopped();
			}
		}
	}
}
