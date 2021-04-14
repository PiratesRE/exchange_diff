using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.AirSync.Wbxml;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal abstract class ChangeTrackingFilter : IChangeTrackingFilter
	{
		internal ChangeTrackingFilter(ChangeTrackingNode[] changeTrackingNodes, bool fillInMissingHashes)
		{
			this.changeTrackingNodes = new Dictionary<string, int>(changeTrackingNodes.Length);
			int num = 0;
			foreach (ChangeTrackingNode changeTrackingNode in changeTrackingNodes)
			{
				if (num == 0 && changeTrackingNode != ChangeTrackingNode.AllNodes)
				{
					throw new ArgumentException("The AllNodes node must be the first node in the list of changetrack nodes!");
				}
				this.changeTrackingNodes.Add(changeTrackingNode.QualifiedName, num++);
			}
			this.fillInMissingHashes = fillInMissingHashes;
		}

		public int?[] UpdateChangeTrackingInformation(XmlNode xmlItemRoot, int?[] oldChangeTrackingInformation)
		{
			this.seenNodes = new XmlNode[this.changeTrackingNodes.Values.Count];
			this.newChangeTrackingInformation = new int?[this.changeTrackingNodes.Values.Count];
			this.newChangeTrackingInformation[0] = this.ComputeHash(xmlItemRoot, true);
			if (this.fillInMissingHashes && oldChangeTrackingInformation != null)
			{
				AirSyncDiagnostics.Assert(this.newChangeTrackingInformation.Length >= oldChangeTrackingInformation.Length, "newChangeTrackingInformation.Length = {0}, oldChangeTrackingInformation.Length = {1}", new object[]
				{
					this.newChangeTrackingInformation.Length,
					oldChangeTrackingInformation.Length
				});
				int num = 0;
				while (num < this.changeTrackingNodes.Values.Count && num < oldChangeTrackingInformation.Length)
				{
					if (this.newChangeTrackingInformation[num] == null)
					{
						this.newChangeTrackingInformation[num] = oldChangeTrackingInformation[num];
					}
					num++;
				}
			}
			return this.newChangeTrackingInformation;
		}

		public int?[] Filter(XmlNode xmlItemRoot, int?[] oldChangeTrackingInformation)
		{
			bool flag = false;
			this.UpdateChangeTrackingInformation(xmlItemRoot, oldChangeTrackingInformation);
			if (oldChangeTrackingInformation == null)
			{
				AirSyncDiagnostics.TraceInfo(ExTraceGlobals.RequestsTracer, this, "ChangeTrackingFilter.Filter returning Xml intact with no previous info!");
				flag = true;
			}
			else
			{
				for (int i = 0; i < this.changeTrackingNodes.Count; i++)
				{
					if (i == 0)
					{
						if (this.newChangeTrackingInformation[i] != oldChangeTrackingInformation[i])
						{
							AirSyncDiagnostics.TraceInfo<int?, int?>(ExTraceGlobals.RequestsTracer, this, "ChangeTrackingFilter.Filter() detected AllNodes hash changed, old {0} new {1} !", oldChangeTrackingInformation[i], this.newChangeTrackingInformation[i]);
							flag = true;
							break;
						}
						xmlItemRoot.RemoveAll();
					}
					else if (i >= oldChangeTrackingInformation.Length || this.newChangeTrackingInformation[i] != oldChangeTrackingInformation[i])
					{
						AirSyncDiagnostics.TraceInfo<int, int?, int?>(ExTraceGlobals.RequestsTracer, this, "ChangeTrackingFilter.Filter() detected other node index {0} hash changed, old {1} new {2} !", i, (i >= oldChangeTrackingInformation.Length) ? new int?(-1) : oldChangeTrackingInformation[i], this.newChangeTrackingInformation[i]);
						xmlItemRoot.AppendChild(this.seenNodes[i]);
						flag = true;
					}
				}
			}
			if (!flag)
			{
				AirSyncDiagnostics.TraceInfo<int?[], int?[]>(ExTraceGlobals.RequestsTracer, this, "ChangeTrackingFilter detected a non-change to item. Old {0} New {1}!", oldChangeTrackingInformation, this.newChangeTrackingInformation);
				throw new ChangeTrackingItemRejectedException();
			}
			return this.newChangeTrackingInformation;
		}

		internal static bool IsEqual(int?[] changeTrackingInformationThis, int?[] changeTrackingInformationThat)
		{
			if (changeTrackingInformationThis == null && changeTrackingInformationThat == null)
			{
				throw new ArgumentNullException("changeTrackingInformationThis/That");
			}
			if (changeTrackingInformationThis == null || changeTrackingInformationThat == null)
			{
				return false;
			}
			if (changeTrackingInformationThis.Length != changeTrackingInformationThat.Length)
			{
				return false;
			}
			for (int i = 0; i < changeTrackingInformationThis.Length; i++)
			{
				if (changeTrackingInformationThis[i] != changeTrackingInformationThat[i])
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsContainer(XmlNode parent)
		{
			foreach (object obj in parent)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					return true;
				}
			}
			return false;
		}

		private int? ComputeHash(XmlNode rootNode, bool shouldChangeTrack)
		{
			int? result = null;
			List<int> list = new List<int>(50);
			list.Add(ChangeTrackingNode.GetQualifiedName(rootNode).GetHashCode());
			foreach (object obj in rootNode.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				string qualifiedName = ChangeTrackingNode.GetQualifiedName(xmlNode);
				if (shouldChangeTrack && this.changeTrackingNodes.ContainsKey(qualifiedName))
				{
					int num = this.changeTrackingNodes[qualifiedName];
					this.seenNodes[num] = xmlNode;
					if (ChangeTrackingFilter.IsContainer(xmlNode))
					{
						AirSyncDiagnostics.TraceInfo<string>(ExTraceGlobals.RequestsTracer, this, "ChangeTrackingFilter.ComputeHash() Recursively computing hash for change tracked container {0}", qualifiedName);
						this.newChangeTrackingInformation[num] = this.ComputeHash(xmlNode, shouldChangeTrack);
						AirSyncDiagnostics.TraceInfo<string, int?>(ExTraceGlobals.RequestsTracer, this, "ChangeTrackingFilter.ComputeHash() Returned hash for change tracked container {0} = {1}", qualifiedName, this.newChangeTrackingInformation[num]);
					}
					else
					{
						int value = ChangeTrackingNode.GetQualifiedName(xmlNode).GetHashCode() ^ this.GetHashCode(xmlNode);
						this.newChangeTrackingInformation[num] = new int?(value);
						AirSyncDiagnostics.TraceInfo<string, int?>(ExTraceGlobals.RequestsTracer, this, "ChangeTrackingFilter.ComputeHash() Calculated change tracked node hash {0} {1}", qualifiedName, this.newChangeTrackingInformation[num]);
					}
				}
				else if (ChangeTrackingFilter.IsContainer(xmlNode))
				{
					int? arg = this.ComputeHash(xmlNode, false);
					if (arg != null)
					{
						list.Add(arg.Value);
						AirSyncDiagnostics.TraceInfo<string, string, int?>(ExTraceGlobals.RequestsTracer, this, "ChangeTrackingFilter.ComputeHash() Returned container node hash {0}{1} = {2}", xmlNode.NamespaceURI, xmlNode.Name, arg);
					}
				}
				else
				{
					int item = ChangeTrackingNode.GetQualifiedName(xmlNode).GetHashCode() ^ this.GetHashCode(xmlNode);
					list.Add(item);
					AirSyncDiagnostics.TraceInfo<string, string, int>(ExTraceGlobals.RequestsTracer, this, "ChangeTrackingFilter.ComputeHash() Calculated node hash {0}{1} = {2}", xmlNode.NamespaceURI, xmlNode.Name, item.GetHashCode());
				}
			}
			if (list.Count > 1)
			{
				list.Sort();
				StringBuilder stringBuilder = new StringBuilder(list.Count * 10);
				foreach (int num2 in list)
				{
					stringBuilder.Append(num2.ToString(CultureInfo.InvariantCulture));
				}
				result = new int?(stringBuilder.ToString().GetHashCode());
			}
			return result;
		}

		private int GetHashCode(XmlNode xmlNode)
		{
			AirSyncBlobXmlNode airSyncBlobXmlNode = xmlNode as AirSyncBlobXmlNode;
			if (airSyncBlobXmlNode != null && airSyncBlobXmlNode.Stream != null)
			{
				return airSyncBlobXmlNode.GetHashCode();
			}
			return xmlNode.InnerText.GetHashCode();
		}

		private const int AllNodesIndex = 0;

		private IDictionary<string, int> changeTrackingNodes;

		private bool fillInMissingHashes;

		private int?[] newChangeTrackingInformation;

		private XmlNode[] seenNodes;
	}
}
