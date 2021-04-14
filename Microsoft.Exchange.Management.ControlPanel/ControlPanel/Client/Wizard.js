﻿Type.registerNamespace("ECP");function EcpWizardForm(n){EcpWizardForm.initializeBase(this,[n])}EcpWizardForm.prototype={initialize:function(){WizardFormBase.prototype.initialize.call(this)},get_TotalSteps:function(){return this.$U_5},set_TotalSteps:function(n){if(this.$U_5!==n){this.$U_5=n;this.raisePropertyChanged("TotalSteps")}return n},$U_5:0,get_CurrentStepIndex:function(){return this.$K_5},set_CurrentStepIndex:function(n){if(this.$K_5!==n){this.$K_5=n;this.$V_4(this.$A_5);this.raisePropertyChanged("CurrentStepIndex")}return n},$K_5:0,get_$c_5:function(){return this.$A_5},set_$c_5:function(n){this.$A_5=n;this.$V_4(this.$A_5);return n},$A_5:null};function EcpWizardStep(n){EcpWizardStep.initializeBase(this,[n]);this.addBinding(new Binding("IsCurrentViewModel","IsActive",2))}EcpWizardStep.prototype={$C_5:!1,get_IsActive:function(){return this.$C_5},set_IsActive:function(n){if(this.$C_5!==n){this.$C_5=n;if(this.$C_5){this.show();this.$0_4.set_$c_5(this.$J_4)}else this.hide();this.raisePropertyChanged("IsActive")}return n}};function ShowResultWizardStep(n){this.$$d_$k_6=Function.createDelegate(this,this.$k_6);this.$$d_$b_6=Function.createDelegate(this,this.$b_6);ShowResultWizardStep.initializeBase(this,[n])}ShowResultWizardStep.prototype={$9_6:null,$I_6:null,$D_6:null,$N_6:!1,get_WebServiceStep:function(){return this.$9_6},set_WebServiceStep:function(n){this.$9_6=n;return n},get_SucceededText:function(){return this.$I_6},set_SucceededText:function(n){this.$I_6=n;return n},get_MessageDiv:function(){return this.$D_6},set_MessageDiv:function(n){this.$D_6=n;return n},show:function(){var n=this.$9_6.results;this.$N_6=!!n&&!!n.Output&&n.Output.length>0;if(this.$N_6){EcpUtil.showElement(this.$0_4.$2_4,!1);EcpUtil.showElement(this.$0_4.get_CancelButton(),!1);Sys.UI.DomElement.removeCssClass(this.$0_4.get_CommitButton(),CssClasses.baseFormButtonWithRightBorder);this.$n_6(n);$addHandler(this.$0_4.get_CommitButton(),"click",this.$$d_$b_6);$addHandler(window,"unload",this.$$d_$k_6)}else{EcpUtil.showElement(this.$0_4.get_CommitButton(),!1);EcpUtil.showElement(this.$0_4.$3_4,!1);this.$m_6(n,this.$9_6.error)}WizardStepBase.prototype.show.call(this)},$m_6:function(n,t){this.$D_6.innerHTML=this.getErrorMessage(n,t)},getErrorMessage:function(n,t){var i=EcpUtil.retrieveErrorMsgs(n);t&&(i=i.concat.call(i,ErrorHandling.getWebServiceErrorMsg(t)));return this.$a_6(i)},$f_6:function(n){var t="";t=EcpUtil.isNullOrBlank(this.$I_6)?n.Informations&&n.Informations.length>0?n.Informations[0]:Strings.get_Success():this.$I_6;return t},$n_6:function(n){var t="";t=n&&n.Warnings&&n.Warnings.length>0?this.$a_6(n.Warnings):this.$f_6(n);this.$D_6.innerHTML=t},$a_6:function(n){var t=EcpUtil.combineMsgsToHtml(n);return String.format('<table class="WizardMsgTbl"><tr><td>{0}</td></tr></table>',t)},getNextStep:function(n){EcpUtil.showElement(this.$0_4.$2_4,!0);EcpUtil.showElement(this.$0_4.get_CommitButton(),!0);EcpUtil.showElement(this.$0_4.$3_4,!0);EcpUtil.showElement(this.$0_4.get_CancelButton(),!0);WizardStep.prototype.getNextStep.call(this,n)},$b_6:function(n){this.$N_6&&$removeHandler(this.$0_4.get_CommitButton(),"click",this.$$d_$b_6);DataLossReporting.suspendWarning();window.close()},$k_6:function(n){$removeHandler(window,"unload",this.$$d_$k_6);this.$l_6()},$l_6:function(){PopupWindowManager.onSaveSucceededClose(null,new WebServiceSucceededEventArgs(null,this.$9_6.results,null))}};function SubscriptionResultWizardStep(n){SubscriptionResultWizardStep.initializeBase(this,[n])}SubscriptionResultWizardStep.ImapOrPopLinkClicked=function(){DataLossReporting.suspendWarning()};SubscriptionResultWizardStep.prototype={$L_7:null,$F_7:!1,$E_7:!1,get_FailedHelpLink:function(){return this.$L_7},set_FailedHelpLink:function(n){this.$L_7=n;return n},get_NewPopAccessible:function(){return this.$F_7},set_NewPopAccessible:function(n){this.$F_7=n;return n},get_NewImapAccessible:function(){return this.$E_7},set_NewImapAccessible:function(n){this.$E_7=n;return n},getErrorMessage:function(n,t){var r=!1;if(n&&n.ErrorRecords){for(var u=0;u<n.ErrorRecords.length;u++)if(n.ErrorRecords[u].Type==="Microsoft.Exchange.Management.Aggregation.AutoProvisionFailedException"){r=!0;break}}else n||t||(r=!0);if(r){var i=Strings.AutoProvisionFailedMsg('<a href="'+this.$L_7+'" target="_blank">'+Strings.get_ReadThis()+"</a>");this.$F_7&&this.$E_7?i+="<p />"+Strings.GuideToSubscriptionPages("NewPopSubscription.aspx","NewImapSubscription.aspx","SubscriptionResultWizardStep.ImapOrPopLinkClicked()"):this.$F_7?i+="<p />"+Strings.GuideToSubscriptionPagePopOrImapOnly("NewPopSubscription.aspx","POP","SubscriptionResultWizardStep.ImapOrPopLinkClicked()"):this.$E_7&&(i+="<p />"+Strings.GuideToSubscriptionPagePopOrImapOnly("NewImapSubscription.aspx","IMAP","SubscriptionResultWizardStep.ImapOrPopLinkClicked()"));return String.format('<table class="WizardMsgTbl"><tr><td>{0}</td></tr></table>',i)}else return ShowResultWizardStep.prototype.getErrorMessage.call(this,n,t)}};function WebServiceWizardStep(n){this.$$d_internalWebServiceMethodFailed=Function.createDelegate(this,this.internalWebServiceMethodFailed);this.$$d_internalWebServiceMethodCompleted=Function.createDelegate(this,this.internalWebServiceMethodCompleted);this.$$d_internalWebServiceMethodSucceeded=Function.createDelegate(this,this.internalWebServiceMethodSucceeded);WebServiceWizardStep.initializeBase(this,[n]);this.$Y_6=this.$$d_internalWebServiceMethodSucceeded;this.$W_6=this.$$d_internalWebServiceMethodCompleted;this.$X_6=this.$$d_internalWebServiceMethodFailed}WebServiceWizardStep.prototype={$6_6:null,$4_6:null,$W_6:null,$Y_6:null,$X_6:null,$P_6:null,$O_6:!1,$G_6:!1,$Q_6:!1,results:null,error:null,get_PropertyPage:function(){return this.$6_6},set_PropertyPage:function(n){this.$6_6=n;return n},get_hasOutput:function(){return!!this.results&&!!this.results.Output&&this.results.Output.length>0},get_WebServiceMethod:function(){return this.$4_6},set_WebServiceMethod:function(n){if(this.$4_6){this.$4_6.remove_Succeeded(this.$Y_6);this.$4_6.remove_Completed(this.$W_6);this.$4_6.remove_Failed(this.$X_6)}this.$4_6=n;if(this.$4_6){this.$4_6.add_Succeeded(this.$Y_6);this.$4_6.add_Completed(this.$W_6);this.$4_6.add_Failed(this.$X_6)}return n},get_ProgressDescription:function(){return this.$P_6},set_ProgressDescription:function(n){this.$P_6=n;return n},get_NextOnCancel:function(){return this.$O_6},set_NextOnCancel:function(n){this.$O_6=n;return n},get_NextOnError:function(){return this.$G_6},set_NextOnError:function(n){this.$G_6=n;return n},get_ShowErrors:function(){return this.$Q_6},set_ShowErrors:function(n){this.$Q_6=n;return n},dispose:function(){this.set_WebServiceMethod(null);EcpControl.prototype.dispose.call(this)},getNextStep:function(n){this.results=null;UpdateProgressPopUp.showProgress(this.$P_6);var t;switch(this.$4_6.get_ParameterNames()){case 0:t=[];break;case 3:t=[this.$6_6.get_Bindings()];break;case 4:t=[this.$6_6.get_identityBinding(),new ChangedValuesBinding(this.$6_6.get_Bindings())];break;case 5:case 6:t=[this.$6_6.get_identityBinding()];break;default:throw Error.notImplemented();}this.$4_6.set_Parameters(t);this.$4_6.invoke(n)},internalWebServiceMethodSucceeded:function(n,t){this.results=t.get_results()},internalWebServiceMethodCompleted:function(n,t){var i=t.get_userContext();UpdateProgressPopUp.progressCompleted(null,!0);if(t.get_cancelled()&&!this.$O_6){this.results=null;this.error=null;this.$0_4.get_CancelButton().disabled=this.$0_4.$2_4.disabled=this.$0_4.$3_4.disabled=!1;return}var r=!!this.results&&(!this.results.ErrorRecords||!this.results.ErrorRecords.length);if(this.$G_6||EcpProfile.get_instance().get_EnableWizardNextOnError()||r)i(this.$7_5);else{this.results&&(this.results.Warnings&&this.results.Warnings.length>0||this.results.ErrorRecords&&this.results.ErrorRecords.length>0)&&this.$Q_6&&ErrorHandling.showPowerShellErrors(this.results);i(this.$5_5)}},internalWebServiceMethodFailed:function(n,t){this.error=t.get_error();(this.$G_6||EcpProfile.get_instance().get_EnableWizardNextOnError())&&t.set_handled(!0)}};function WizardFormBase(n){this.$$d_$i_4=Function.createDelegate(this,this.$i_4);WizardFormBase.initializeBase(this,[n])}WizardFormBase.prototype={$R_4:null,$S_4:!1,$2_4:null,$3_4:null,initialize:function(){BaseForm.prototype.initialize.call(this);$addHandler(document.documentElement,"keypress",this.$$d_$i_4)},dispose:function(){$removeHandler(document.documentElement,"keypress",this.$$d_$i_4);BaseForm.prototype.dispose.call(this)},get_BackButton:function(){return this.$2_4},set_BackButton:function(n){this.$2_4=n;return n},get_NextButton:function(){return this.$3_4},set_NextButton:function(n){this.$3_4=n;return n},get_ShowWizardStepLabel:function(){return this.$R_4},set_ShowWizardStepLabel:function(n){this.$R_4=n;return n},get_ShowWizardStepTitle:function(){return this.$S_4},set_ShowWizardStepTitle:function(n){this.$S_4=n;return n},$V_4:function(n){this.$S_4&&EcpUtil.setInnerText(this.$R_4,n)},$i_4:function(n){this.get_defaultButton()&&WizardPropertyUtil.clientSide_FireDefaultButton(n,this.get_defaultButton())},get_defaultButton:function(){return this.$3_4.style.display===""&&!EcpUtil.domElementDisabled(this.$3_4)?this.$3_4:this.get_CommitButton().style.display===""&&!EcpUtil.domElementDisabled(this.get_CommitButton())?this.get_CommitButton():this.$2_4.style.display===""&&!EcpUtil.domElementDisabled(this.$2_4)?this.$2_4:this.get_CancelButton().style.display===""&&!EcpUtil.domElementDisabled(this.get_CancelButton())?this.get_CancelButton():null}};function WizardForm(n){this.$$d_$e_5=Function.createDelegate(this,this.$e_5);this.$$d_$j_5=Function.createDelegate(this,this.$j_5);this.$$d_$h_5=Function.createDelegate(this,this.$h_5);this.$$d_$d_5=Function.createDelegate(this,this.$d_5);this.$8_5=[];this.$H_5={};WizardForm.initializeBase(this,[n]);BaseForm.prototype.set_SetInitialFocus.call(this,!1);this.$B_5=this.$$d_$d_5;FieldValidationAssistant.add_elementValidating(this.$B_5)}WizardForm.prototype={$1_5:null,$T_5:null,$B_5:null,$o_5:0,get_StartsWithStepID:function(){return this.$T_5},set_StartsWithStepID:function(n){this.$T_5=n;return n},get_CurrentStepID:function(){return this.$1_5.$5_5},registerStep:function(n){this.$H_5[n.$5_5]=n;this.$o_5++;if(n.$5_5===this.$T_5){var t=this;window.setTimeout(function(){t.set_currentStep(n)},10)}},getStep:function(n){return this.$H_5[n]},get_currentStep:function(){return this.$1_5},set_currentStep:function(n){if(this.$1_5===n)return n;this.$1_5&&this.$1_5.hide();this.$1_5=n;EcpUtil.showElement(this.$2_4,this.$8_5.length>0);EcpUtil.showElement(this.$3_4,!this.$1_5.isFinalStep());EcpUtil.showElement(this.get_CommitButton(),this.$1_5.isFinalStep());this.set_FvaEnabled(this.$1_5.$M_4);this.$1_5.show();this.$V_4(this.$1_5.$J_4);return n},initialize:function(){WizardFormBase.prototype.initialize.call(this);$addHandler(this.$2_4,"click",this.$$d_$h_5);$addHandler(this.$3_4,"click",this.$$d_$j_5)},dispose:function(){$removeHandler(this.$2_4,"click",this.$$d_$h_5);$removeHandler(this.$3_4,"click",this.$$d_$j_5);FieldValidationAssistant.remove_elementValidating(this.$B_5);this.$B_5=null;WizardFormBase.prototype.dispose.call(this)},$h_5:function(n){this.$8_5.length>0&&this.set_currentStep(this.$8_5.pop())},$j_5:function(n){var t=ControlHelper.validateControls(this.$1_5.get_element(),!0);if(t.length>0||FieldValidationAssistant.get_instance().highlightFieldsInError()){DialogManager.focusControlAfterClose(t.length>0?t[0].get_element():FieldValidationAssistant.get_instance().get_firstTextboxInError());MessageBox.showMessage(Strings.get_FieldsInError(),MessageBoxType.error)}else this.$1_5.isFinalStep()?this.get_CommitButton().click():this.$g_5()},$g_5:function(){this.get_CancelButton().disabled=this.$2_4.disabled=this.$3_4.disabled=!0;this.$1_5.getNextStep(this.$$d_$e_5)},$e_5:function(n){this.get_CancelButton().disabled=!1;this.$3_4.disabled=!1;this.$2_4.disabled=!1;if(this.$1_5.$5_5!==n){this.$8_5.push(this.$1_5);this.set_currentStep(this.$H_5[n])}},$d_5:function(n,t){t.set_validateElement(EcpUtil.isVisible(t.get_element()))}};function WizardStep(n){WizardStep.initializeBase(this,[n])}WizardStep.prototype={$5_5:null,$7_5:null,initialize:function(){EcpControl.prototype.initialize.call(this);this.$0_4.registerStep(this)},get_StepID:function(){return this.$5_5},set_StepID:function(n){this.$5_5=n;return n},get_NextStepID:function(){return this.$7_5},set_NextStepID:function(n){this.$7_5=n;return n},getNextStep:function(n){n(this.$7_5)},isFinalStep:function(){return!this.$7_5}};function WizardStepBase(n){WizardStepBase.initializeBase(this,[n])}WizardStepBase.prototype={$0_4:null,$M_4:!1,$J_4:null,get_FvaEnabled:function(){return this.$M_4},set_FvaEnabled:function(n){this.$M_4=n;return n},get_Title:function(){return this.$J_4},set_Title:function(n){this.$J_4=n;return n},get_Form:function(){return this.$0_4},set_Form:function(n){this.$0_4=n;return n},show:function(){EcpUtil.showElement(this.get_element(),!0);if(!FocusUtil.setFirstFieldFocused(this.get_element(),!0)&&this.$0_4.get_defaultButton()){var n=this;window.setTimeout(function(){EcpUtil.safeFocus(n.$0_4.get_defaultButton())},0)}},hide:function(){EcpUtil.showElement(this.get_element(),!1)}};WizardFormBase.registerClass("WizardFormBase",BaseForm);EcpWizardForm.registerClass("EcpWizardForm",WizardFormBase);WizardStepBase.registerClass("WizardStepBase",ContainerControl);EcpWizardStep.registerClass("EcpWizardStep",WizardStepBase);WizardStep.registerClass("WizardStep",WizardStepBase);ShowResultWizardStep.registerClass("ShowResultWizardStep",WizardStep);SubscriptionResultWizardStep.registerClass("SubscriptionResultWizardStep",ShowResultWizardStep);WebServiceWizardStep.registerClass("WebServiceWizardStep",WizardStep);WizardForm.registerClass("WizardForm",WizardFormBase);ShowResultWizardStep.msgFmt='<table class="WizardMsgTbl"><tr><td>{0}</td></tr></table>'