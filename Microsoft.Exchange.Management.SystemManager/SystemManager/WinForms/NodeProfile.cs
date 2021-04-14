using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Serializable]
	public class NodeProfile
	{
		[DefaultValue(null)]
		[TypeConverter(typeof(OrganizationTypesConverter))]
		public OrganizationType[] OrganizationTypes { get; set; }

		public ResultPaneProfile ResultPaneProfile { get; set; }

		[TypeConverter(typeof(DDIObjectTypeConverter))]
		public Type Type { get; set; }

		public string Name { get; set; }

		public NodeProfileList NodeProfiles
		{
			get
			{
				return this.nodeProfiles;
			}
			set
			{
				this.nodeProfiles = value;
			}
		}

		public ScopeNode[] GetScopeNodes()
		{
			List<ScopeNode> list = new List<ScopeNode>();
			foreach (NodeProfile nodeProfile in this.nodeProfiles)
			{
				if (WinformsHelper.IsCurrentOrganizationAllowed(nodeProfile.OrganizationTypes) && nodeProfile.Type != null)
				{
					bool flag = false;
					ExchangeScopeNode exchangeScopeNode = (ExchangeScopeNode)nodeProfile.Type.GetConstructor(new Type[0]).Invoke(new object[0]);
					ScopeNode[] scopeNodes = nodeProfile.GetScopeNodes();
					if (scopeNodes.Length > 0)
					{
						flag = true;
						exchangeScopeNode.Children.AddRange(scopeNodes);
					}
					if (nodeProfile.ResultPaneProfile != null && nodeProfile.ResultPaneProfile.HasPermission())
					{
						exchangeScopeNode.ViewDescriptions.Add(ExchangeFormView.CreateViewDescription(nodeProfile.ResultPaneProfile.Type));
						flag = true;
					}
					if (scopeNodes.Length > 0 && exchangeScopeNode.ViewDescriptions.Count == 0)
					{
						exchangeScopeNode.ViewDescriptions.Add(ExchangeFormView.CreateViewDescription(typeof(RbacPermissionLockResultPane)));
					}
					if (flag)
					{
						list.Add(exchangeScopeNode);
					}
				}
			}
			return list.ToArray();
		}

		public static bool CanAddAtomResultPane(Type atomResultPaneType)
		{
			FieldInfo field = atomResultPaneType.GetField("SchemaName");
			if (null != field)
			{
				string profileName = (string)field.GetValue(null);
				return NodeProfile.dataProfileLoader.GetProfile(profileName).HasPermission();
			}
			return true;
		}

		private static readonly ObjectPickerProfileLoader dataProfileLoader = new ObjectPickerProfileLoader(1);

		private NodeProfileList nodeProfiles = new NodeProfileList();
	}
}
