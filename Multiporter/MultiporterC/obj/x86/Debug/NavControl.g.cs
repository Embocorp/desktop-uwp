﻿#pragma checksum "C:\Users\Iggy\Documents\embi\desktop-uwp\Multiporter\MultiporterC\NavControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "81CEA4F3EBE8D8A6AFA48B5BB8296FD7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MultiporterC
{
    partial class NavControl : 
        global::Windows.UI.Xaml.Controls.UserControl, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.MenuButtonSettings = (global::Windows.UI.Xaml.Controls.Button)(target);
                }
                break;
            case 2:
                {
                    this.MenuButtonAccount = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 36 "..\..\..\NavControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.MenuButtonAccount).Click += this.MenuButtonAccount_Click;
                    #line default
                }
                break;
            case 3:
                {
                    this.MenuButtonDevices = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 30 "..\..\..\NavControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.MenuButtonDevices).Click += this.DevNav_Click;
                    #line default
                }
                break;
            case 4:
                {
                    this.MenuButtonExperiment = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 25 "..\..\..\NavControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.MenuButtonExperiment).Click += this.ExpNav_Click;
                    #line default
                }
                break;
            case 5:
                {
                    this.MenuButtonHome = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 20 "..\..\..\NavControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.MenuButtonHome).Click += this.HomeNav_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.HamburgerButton = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 15 "..\..\..\NavControl.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.HamburgerButton).Click += this.HamburgerButton_Click;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

