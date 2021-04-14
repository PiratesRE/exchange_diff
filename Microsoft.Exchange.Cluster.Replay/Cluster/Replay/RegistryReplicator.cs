using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class RegistryReplicator : IDisposeTrackable, IDisposable
	{
		internal RegistryReplicator(RealRegistry localRegistry, ClusterRegistry clusterRegistry, ManualResetEvent keyChanged)
		{
			this.m_localRegistry = localRegistry;
			this.m_clusterRegistry = clusterRegistry;
			this.m_keyChanged = keyChanged;
			this.m_isCopyEnabled = true;
			this.m_isValid = true;
			this.m_isCopying = false;
			this.m_disposed = false;
			this.m_isMarkedForRemoval = false;
			this.m_disposeTracker = this.GetDisposeTracker();
		}

		internal SafeHandle Handle
		{
			get
			{
				return this.m_localRegistry.Handle;
			}
		}

		internal ManualResetEvent KeyChanged
		{
			get
			{
				return this.m_keyChanged;
			}
		}

		internal bool IsCopyEnabled
		{
			get
			{
				return this.m_isCopyEnabled;
			}
		}

		internal bool IsCopying
		{
			get
			{
				return this.m_isCopying;
			}
		}

		internal bool IsValid
		{
			get
			{
				return this.m_isValid;
			}
		}

		internal bool IsInitialReplication
		{
			get
			{
				return this.m_localRegistry.IsInitialReplication | this.m_clusterRegistry.IsInitialReplication;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RegistryReplicator>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.m_disposeTracker != null)
			{
				this.m_disposeTracker.Suppress();
			}
		}

		internal void SetInitialReplication()
		{
			this.m_localRegistry.SetInitialReplication();
			this.m_clusterRegistry.SetInitialReplication();
		}

		internal void SetValid()
		{
			this.m_isValid = true;
		}

		internal void SetInvalid()
		{
			this.m_isValid = false;
		}

		internal void EnableCopy()
		{
			this.m_isCopyEnabled = true;
		}

		internal void DisableCopy()
		{
			this.m_isCopyEnabled = false;
		}

		internal void SetMarkedForRemoval()
		{
			this.m_isMarkedForRemoval = true;
		}

		internal void ResetMarkedForRemoval()
		{
			this.m_isMarkedForRemoval = false;
		}

		internal bool IsMarkedForRemoval
		{
			get
			{
				return this.m_isMarkedForRemoval;
			}
		}

		internal void InverseCopy()
		{
			this.CopyWorker(this.m_clusterRegistry, this.m_localRegistry);
		}

		internal void Copy(AmClusterHandle handle)
		{
			if (handle != null)
			{
				this.m_clusterRegistry.SetClusterHandle(handle);
			}
			this.CopyWorker(this.m_localRegistry, this.m_clusterRegistry);
		}

		private void CopyWorker(RegistryManipulator source, RegistryManipulator destination)
		{
			string text = null;
			string text2 = null;
			bool flag = true;
			bool flag2 = true;
			this.m_isCopying = true;
			try
			{
				Queue<string> queue = new Queue<string>();
				Queue<string> queue2 = new Queue<string>();
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				queue.Enqueue(string.Empty);
				queue2.Enqueue(string.Empty);
				while (this.m_isValid)
				{
					if (flag)
					{
						list.Clear();
						if (queue.Count > 0)
						{
							text = queue.Dequeue();
							flag = false;
							foreach (string text3 in source.GetSubKeyNames(text))
							{
								string item = text3;
								if (text.CompareTo(string.Empty) != 0)
								{
									item = text + "\\" + text3;
								}
								queue.Enqueue(item);
							}
						}
						else
						{
							text = null;
						}
					}
					if (flag2)
					{
						list2.Clear();
						if (queue2.Count > 0)
						{
							text2 = queue2.Dequeue();
							foreach (string text4 in destination.GetSubKeyNames(text2))
							{
								string item2 = text4;
								if (text2.CompareTo(string.Empty) != 0)
								{
									item2 = text2 + "\\" + text4;
								}
								queue2.Enqueue(item2);
							}
						}
						else
						{
							text2 = null;
						}
					}
					int num;
					if (text == null)
					{
						num = 1;
					}
					else if (text2 == null)
					{
						num = -1;
					}
					else
					{
						num = string.Compare(text, text2, StringComparison.OrdinalIgnoreCase);
					}
					if (num == 0)
					{
						foreach (string item3 in source.GetValueNames(text))
						{
							list.Add(item3);
						}
						list.Sort();
						foreach (string item4 in destination.GetValueNames(text2))
						{
							list2.Add(item4);
						}
						list2.Sort();
						int num2 = 0;
						int num3 = 0;
						while (num3 < list.Count || num2 < list2.Count)
						{
							int num4;
							if (num3 >= list.Count)
							{
								num4 = 1;
							}
							else if (num2 >= list2.Count)
							{
								num4 = -1;
							}
							else
							{
								num4 = string.Compare(list[num3], list2[num2]);
							}
							if (num4 == 0)
							{
								RegistryValue value = source.GetValue(text, list[num3]);
								RegistryValue value2 = destination.GetValue(text2, list2[num2]);
								if (value != null && !value.Equals(value2))
								{
									destination.SetValue(text, value);
								}
								else if (value == null)
								{
									destination.DeleteValue(text2, list2[num2]);
								}
								num3++;
								num2++;
							}
							else if (num4 < 0)
							{
								RegistryValue value3 = source.GetValue(text, list[num3]);
								if (value3 != null)
								{
									destination.SetValue(text, value3);
								}
								num3++;
							}
							else
							{
								destination.DeleteValue(text2, list2[num2]);
								num2++;
							}
						}
						flag = true;
						flag2 = true;
					}
					else if (num < 0 && text != null)
					{
						destination.CreateKey(text);
						flag2 = false;
						text2 = text;
					}
					else
					{
						if (text2 == null)
						{
							return;
						}
						destination.DeleteKey(text2);
						flag2 = true;
					}
				}
				AmTrace.Warning("Skipping a copy notification since the database has changed master. Source root: {0}", new object[]
				{
					source.Root
				});
			}
			finally
			{
				this.m_isCopying = false;
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.m_disposed && disposing)
			{
				this.SetInvalid();
				if (this.m_localRegistry != null)
				{
					this.m_localRegistry.Dispose();
				}
				if (this.m_clusterRegistry != null)
				{
					this.m_clusterRegistry.Dispose();
				}
				if (this.m_keyChanged != null)
				{
					this.m_keyChanged.Close();
				}
				if (this.m_disposeTracker != null)
				{
					this.m_disposeTracker.Dispose();
				}
			}
			this.m_disposed = true;
		}

		private RealRegistry m_localRegistry;

		private ClusterRegistry m_clusterRegistry;

		private ManualResetEvent m_keyChanged;

		private bool m_isCopyEnabled;

		private bool m_isValid;

		private bool m_isCopying;

		private bool m_disposed;

		private bool m_isMarkedForRemoval;

		private DisposeTracker m_disposeTracker;
	}
}
