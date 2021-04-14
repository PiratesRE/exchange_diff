using System;

namespace Microsoft.Exchange.Data
{
	internal static class WellKnownGuid
	{
		private const string systemWkGuidString = "{F3301DAB-8876-D111-ADED-00C04FD8D5CD}";

		private const string domainControllersWkGuidString = "{FFB261A3-D2FF-D111-AA4B-00C04FD7D83A}";

		private const string lostAndFoundContainerWkGuidString = "{b75381ab-8876-d111-aded-00c04fd8d5cd}";

		private const string usersWkGuidString = "{15CAD1A9-8876-D111-ADED-00C04FD8D5CD}";

		private const string exsWkGuidString = "{6C01D2A7-F083-4503-8132-789EEB127B84}";

		private const string masWkGuidString = "{CF2E5202-8599-4A98-9232-056FC704CC8B}";

		private const string eoaWkGuidString_E12 = "{3D604B35-D992-4155-AAFD-8C0AE688EA0F}";

		private const string eoaWkGuidString = "{29A962C2-91D6-4AB7-9E06-8728F8F842EA}";

		private const string emaWkGuidString_E12 = "{D998D80B-0839-4210-B77B-F4A555E07FA1}";

		private const string emaWkGuidString = "{1DC472DB-5849-4D0A-B304-FE6981E56297}";

		private const string epaWkGuidString_E12 = "{73000302-5915-4dce-A330-91DD24F58231}";

		private const string epaWkGuidString = "{7B41FA45-7435-4EDC-929B-C4B059699792}";

		private const string eraWkGuidString_E12 = "{660113DC-542F-4574-B9C4-90A52961F8FC}";

		private const string eraWkGuidString = "{D3399E1A-BE5A-4757-B979-FFC0C6E5EA26}";

		private const string e3iWkGuidString = "{3F965B9C-F167-4b4a-936C-B8EFB19C4784}";

		private const string etsWkGuidString = "{586a87ea-6ddb-4cd0-9006-939818f800eb}";

		private const string ewpWkGuidString = "{11d0174c-be7e-4266-afae-e03bc66d381f}";

		private const string efomgWkGuidString = "{a0aad226-2daf-4978-b5a1-d2debd168d8f}";

		private const string eopsWkGuidString = "{E8A7E650-5F72-440e-9D77-E250A6C0E8F9}";

		private const string eahoWkGuidString = "{70bf766e-de1c-4776-b193-758d1306c8fb}";

		private const string configDeletedObjectsWkGuidString = "{80EAE218-4F68-D211-B9AA-00C04F79F805}";

		private const string exchangeInfoPropSetGuidString = "{1f298a89-de98-47b8-b5cd-572Ad53d267e}";

		private const string exchangePersonalInfoPropSetGuidString = "{B1B3A417-EC55-4191-B327-B72E33E38AF2}";

		private const string publicInfoPropSetGuidString = "{e48d0154-bcf8-11d1-8702-00c04fb96050}";

		private const string personalInfoPropSetGuidString = "{77b5b886-944a-11d1-aebd-0000f80367c1}";

		private const string userAccountRestrictionsPropSetGuidString = "{4c164200-20c0-11d0-a768-00aa006e0529}";

		public const string InPlaceHoldIdentityLegalHoldGuidString = "98E9BABD09A04bcf8455A58C2AA74182";

		public const string SendAsExtendedRightGuidString = "ab721a54-1e2f-11d0-9819-00aa0040529b";

		public const string ReceiveAsExtendedRightGuidString = "ab721a56-1e2f-11d0-9819-00aa0040529b";

		public static readonly Guid SystemWkGuid = new Guid("{F3301DAB-8876-D111-ADED-00C04FD8D5CD}");

		public static readonly Guid DomainControllersWkGuid = new Guid("{FFB261A3-D2FF-D111-AA4B-00C04FD7D83A}");

		public static readonly Guid LostAndFoundContainerWkGuid = new Guid("{b75381ab-8876-d111-aded-00c04fd8d5cd}");

		public static readonly Guid UsersWkGuid = new Guid("{15CAD1A9-8876-D111-ADED-00C04FD8D5CD}");

		public static readonly Guid ExSWkGuid = new Guid("{6C01D2A7-F083-4503-8132-789EEB127B84}");

		public static readonly Guid MaSWkGuid = new Guid("{CF2E5202-8599-4A98-9232-056FC704CC8B}");

		public static readonly Guid EoaWkGuid_E12 = new Guid("{3D604B35-D992-4155-AAFD-8C0AE688EA0F}");

		public static readonly Guid EoaWkGuid = new Guid("{29A962C2-91D6-4AB7-9E06-8728F8F842EA}");

		public static readonly Guid EmaWkGuid_E12 = new Guid("{D998D80B-0839-4210-B77B-F4A555E07FA1}");

		public static readonly Guid EmaWkGuid = new Guid("{1DC472DB-5849-4D0A-B304-FE6981E56297}");

		public static readonly Guid EpaWkGuid_E12 = new Guid("{73000302-5915-4dce-A330-91DD24F58231}");

		public static readonly Guid EpaWkGuid = new Guid("{7B41FA45-7435-4EDC-929B-C4B059699792}");

		public static readonly Guid EraWkGuid_E12 = new Guid("{660113DC-542F-4574-B9C4-90A52961F8FC}");

		public static readonly Guid EraWkGuid = new Guid("{D3399E1A-BE5A-4757-B979-FFC0C6E5EA26}");

		public static readonly Guid E3iWkGuid = new Guid("{3F965B9C-F167-4b4a-936C-B8EFB19C4784}");

		public static readonly Guid EtsWkGuid = new Guid("{586a87ea-6ddb-4cd0-9006-939818f800eb}");

		public static readonly Guid EwpWkGuid = new Guid("{11d0174c-be7e-4266-afae-e03bc66d381f}");

		public static readonly Guid EfomgWkGuid = new Guid("{a0aad226-2daf-4978-b5a1-d2debd168d8f}");

		public static readonly Guid EopsWkGuid = new Guid("{E8A7E650-5F72-440e-9D77-E250A6C0E8F9}");

		public static readonly Guid EahoWkGuid = new Guid("{70bf766e-de1c-4776-b193-758d1306c8fb}");

		public static readonly Guid ConfigDeletedObjectsWkGuid = new Guid("{80EAE218-4F68-D211-B9AA-00C04F79F805}");

		public static readonly Guid ExchangeInfoPropSetGuid = new Guid("{1f298a89-de98-47b8-b5cd-572Ad53d267e}");

		public static readonly Guid ExchangePersonalInfoPropSetGuid = new Guid("{B1B3A417-EC55-4191-B327-B72E33E38AF2}");

		public static readonly Guid PublicInfoPropSetGuid = new Guid("{e48d0154-bcf8-11d1-8702-00c04fb96050}");

		public static readonly Guid PersonalInfoPropSetGuid = new Guid("{77b5b886-944a-11d1-aebd-0000f80367c1}");

		public static readonly Guid UserAccountRestrictionsPropSetGuid = new Guid("{4c164200-20c0-11d0-a768-00aa006e0529}");

		public static readonly Guid InPlaceHoldIdentityLegalHoldGuid = new Guid("98E9BABD09A04bcf8455A58C2AA74182");

		public static readonly Guid CreatePublicFolderExtendedRightGuid = new Guid("cf0b3dc8-afe6-11d2-aa04-00c04f8eedd8");

		public static readonly Guid CreateTopLevelPublicFolderExtendedRightGuid = new Guid("cf4b9d46-afe6-11d2-aa04-00c04f8eedd8");

		public static readonly Guid RIMMailboxAdminsGroupGuid = new Guid("1e6b6d42-7174-4e4b-8de1-0df23acb1c42");

		public static readonly Guid RIMMailboxUsersGroupGuid = new Guid("62f35d94-58c7-4003-adda-fc207c1562a7");

		public static readonly Guid MailEnablePublicFolderGuid = new Guid("cf899a6a-afe6-11d2-aa04-00c04f8eedd8");

		public static readonly Guid ModifyPublicFolderACLExtendedRightGuid = new Guid("D74A8769-22B9-11d3-AA62-00C04F8EEDD8");

		public static readonly Guid ModifyPublicFolderAdminACLExtendedRightGuid = new Guid("D74A876F-22B9-11d3-AA62-00C04F8EEDD8");

		public static readonly Guid ModifyPublicFolderDeletedItemRetentionExtendedRightGuid = new Guid("cffe6da4-afe6-11d2-aa04-00c04f8eedd8");

		public static readonly Guid ModifyPublicFolderExpiryExtendedRightGuid = new Guid("cfc7978e-afe6-11d2-aa04-00c04f8eedd8");

		public static readonly Guid ModifyPublicFolderQuotasExtendedRightGuid = new Guid("d03a086e-afe6-11d2-aa04-00c04f8eedd8");

		public static readonly Guid ModifyPublicFolderReplicaListExtendedRightGuid = new Guid("d0780592-afe6-11d2-aa04-00c04f8eedd8");

		public static readonly Guid SendAsExtendedRightGuid = new Guid("ab721a54-1e2f-11d0-9819-00aa0040529b");

		public static readonly Guid ReceiveAsExtendedRightGuid = new Guid("ab721a56-1e2f-11d0-9819-00aa0040529b");

		public static readonly Guid StoreCreateNamedPropertiesExtendedRightGuid = new Guid("D74A8766-22B9-11d3-AA62-00C04F8EEDD8");

		public static readonly Guid StoreTransportAccessExtendedRightGuid = new Guid("9fbec2a2-f761-11d9-963d-00065bbd3175");

		public static readonly Guid StoreConstrainedDelegationExtendedRightGuid = new Guid("9fbec2a1-f761-11d9-963d-00065bbd3175");

		public static readonly Guid StoreReadAccessExtendedRightGuid = new Guid("9fbec2a3-f761-11d9-963d-00065bbd3175");

		public static readonly Guid StoreReadWriteAccessExtendedRightGuid = new Guid("9fbec2a4-f761-11d9-963d-00065bbd3175");

		public static readonly Guid StoreAdminExtendedRightGuid = new Guid("D74A8762-22B9-11d3-AA62-00C04F8EEDD8");

		public static readonly Guid StoreVisibleExtendedRightGuid = new Guid("D74A875E-22B9-11d3-AA62-00C04F8EEDD8");

		public static readonly Guid ChangePasswordExtendedRightGuid = new Guid("ab721a53-1e2f-11d0-9819-00aa0040529b");

		public static readonly Guid ResetPasswordOnNextLogonExtendedRightGuid = new Guid("00299570-246d-11d0-a768-00aa006e0529");

		public static readonly Guid RecipientUpdateExtendedRightGuid = new Guid("165AB2CC-D1B3-4717-9B90-C657E7E57F4D");

		public static readonly Guid DownloadOABExtendedRightGuid = new Guid("BD919C7C-2D79-4950-BC9C-E16FD99285E8");

		public static readonly Guid EpiImpersonationRightGuid = new Guid("8DB0795C-DF3A-4aca-A97D-100162998DFA");

		public static readonly Guid TokenSerializationRightGuid = new Guid("06386F89-BEFB-4e48-BAA1-559FD9221F78");

		public static readonly Guid EpiMayImpersonateRightGuid = new Guid("bc39105d-9baa-477c-a34a-997cc25e3d60");

		public static readonly Guid DsReplicationSynchronize = new Guid("1131f6ab-9c07-11d1-f79f-00c04fc2dcd2");

		public static readonly Guid DsReplicationGetChanges = new Guid("1131f6aa-9c07-11d1-f79f-00c04fc2dcd2");

		public static readonly Guid OpenAddressBookRight = new Guid("a1990816-4298-11d1-ade2-00c04fd8d5cd");

		public static readonly Guid RgUmManagementWkGuid = new Guid("B7DF0CE8-9756-4993-81C8-98B4DBC5A0C6");

		public static readonly Guid RgHelpDeskWkGuid = new Guid("BEC6DDB3-3B2A-4BE8-97EB-2DCE9477E389");

		public static readonly Guid RgRecordsManagementWkGuid = new Guid("C932A4BE-1D4E-4E25-AF99-B40573360D5B");

		public static readonly Guid RgDiscoveryManagementWkGuid = new Guid("2EDE7FC6-3983-4467-90FB-AFDCA3DFDC95");

		public static readonly Guid RgServerManagementWkGuid = new Guid("75E7B84D-B64E-43c1-9565-612E69A80A4F");

		public static readonly Guid RgDelegatedSetupWkGuid = new Guid("261928D1-F5D1-445b-866D-1D6B5BD87A09");

		public static readonly Guid RgHygieneManagementWkGuid = new Guid("F409B703-F351-43BF-88E3-3495369B6771");

		public static readonly Guid RgManagementForestOperatorWkGuid = new Guid("3178BCE1-9934-4fdc-AB62-5FB6A502B820");

		public static readonly Guid RgManagementForestTier1SupportWkGuid = new Guid("4B472D3E-1000-4ab7-A9CA-DE5ABDC317D1");

		public static readonly Guid RgViewOnlyManagementForestOperatorWkGuid = new Guid("971DDF37-4865-40ec-890A-BF2EC8172E04");

		public static readonly Guid RgManagementForestMonitoringWkGuid = new Guid("644B8D91-0D9D-421a-8070-7002FD503842");

		public static readonly Guid RgDataCenterManagementWkGuid = new Guid("A60E029A-FAF6-46dc-9FB6-0383FF786F36");

		public static readonly Guid RgViewOnlyLocalServerAccessWkGuid = new Guid("BC6291B7-E8D3-44b6-B010-1A42C340B20A");

		public static readonly Guid RgDestructiveAccessWkGuid = new Guid("4A96E860-01A2-4314-B2E2-D537B54F11C1");

		public static readonly Guid RgElevatedPermissionsWkGuid = new Guid("8D08A813-05C4-4fcd-8081-BACEF3F9FD6F");

		public static readonly Guid RgViewOnlyWkGuid = new Guid("C4D9726F-9CCF-47c6-9B1E-2E4971FF6486");

		public static readonly Guid RgOperationsWkGuid = new Guid("1DC6B9B0-3580-44c3-AD04-C89172C40958");

		public static readonly Guid RgServiceAccountsWkGuid = new Guid("69910741-AF5A-4f25-904F-AE00A6C7582A");

		public static readonly Guid RgComplianceManagementWkGuid = new Guid("9B440AB3-B4A9-4520-8C4B-B22F33C52766");

		public static readonly Guid RgViewOnlyPIIWkGuid = new Guid("AAA4C161-1F7B-455B-820C-5D2EDB1A7D47");

		public static readonly Guid RgCapacityDestructiveAccessWkGuid = new Guid("7E3685CD-8BEF-4F41-AC45-0D0B14EDD008");

		public static readonly Guid RgCapacityServerAdminsWkGuid = new Guid("370a9745-22bd-4e9c-80fa-162363e48148");

		public static readonly Guid RgCustomerChangeAccessWkGuid = new Guid("1649a084-f94c-4393-9cce-4530a491d1fd");

		public static readonly Guid RgCustomerDataAccessWkGuid = new Guid("b83f41a6-cd6e-41d0-bd13-284361b48ce9");

		public static readonly Guid RgAccessToCustomerDataDCOnlyWkGuid = new Guid("4CE08A5A-224F-416E-B77C-B34A7AE56E6B");

		public static readonly Guid RgDatacenterOperationsDCOnlyWkGuid = new Guid("BD34316F-FB28-433A-A218-52816234E5F3");

		public static readonly Guid RgCustomerDestructiveAccessWkGuid = new Guid("93889eb0-ee19-4229-b5a8-ff0732a3b07a");

		public static readonly Guid RgCustomerPIIAccessWkGuid = new Guid("e6abdf42-cdac-4c7b-97ad-4b3b6466796b");

		public static readonly Guid RgManagementAdminAccessWkGuid = new Guid("ac2562f4-866e-4049-8ae8-42649ec73917");

		public static readonly Guid RgManagementCACoreAdminWkGuid = new Guid("5bf65a76-317f-409f-a2d7-a0c1dc2b2569");

		public static readonly Guid RgManagementChangeAccessWkGuid = new Guid("a81ca1e9-d1e5-4cf3-a69c-4d89c63deb22");

		public static readonly Guid RgManagementDestructiveAccessWkGuid = new Guid("fe45aa5a-b32e-45d3-837c-dd0b00cc4df2");

		public static readonly Guid RgCapacityFrontendServerAdminsWkGuid = new Guid("3528e4d2-4faf-4c75-b374-50fc3bfbb8fd");

		public static readonly Guid RgManagementServerAdminsWkGuid = new Guid("aee383e0-711c-4625-851b-4a4c0d0c117e");

		public static readonly Guid RgCapacityDCAdminsWkGuid = new Guid("d7e9161c-495e-4c2f-91ba-e044bda511db");

		public static readonly Guid RgNetworkingAdminAccessWkGuid = new Guid("6bacb10c-31fe-4837-903a-f5d227cf95ec");

		public static readonly Guid RgNetworkingChangeAccessWkGuid = new Guid("d5eb34ed-1a14-4e69-b454-e7bb867104e2");

		public static readonly Guid RgCommunicationManagersWkGuid = new Guid("f26700d2-55ca-4b3d-9779-002078475902");

		public static readonly Guid RgMailboxManagementWkGuid = new Guid("acf98416-bb5d-45d7-9a23-3bec7fe981f5");

		public static readonly Guid RgFfoAntiSpamAdminsWkGuid = new Guid("36A79B6E-0872-4D33-92C8-B813BBCD3C9A");

		public static readonly Guid RgDedicatedSupportAccessWkGuid = new Guid("12abd11b-d764-44d3-b862-9c5ab6e90088");

		public static readonly Guid RgAppLockerExemptionWkGuid = new Guid("e24a1b87-6a64-46c4-b9c8-91f878f9d31c");

		public static readonly Guid RgECSAdminServerAccessWkGuid = new Guid("64092ddc-75d1-4a6d-980d-30e79e3cf251");

		public static readonly Guid RgECSPIIAccessServerAccessWkGuid = new Guid("164a26b9-2b54-4ce7-bb2a-d033897e7da8");

		public static readonly Guid RgECSAdminWkGuid = new Guid("c36ca20f-3d1d-40b5-a7a9-c598298e083d");

		public static readonly Guid RgECSPIIAccessWkGuid = new Guid("dbf37be1-038c-4535-b74e-7bbd2f453a7c");
	}
}
