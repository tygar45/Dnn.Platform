﻿using System;
using System.Web.UI;
using DotNetNuke.Web.DDRMenu.Localisation;
using DotNetNuke.Web.DDRMenu.DNNCommon;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI;

namespace DotNetNuke.Web.DDRMenu
{
    public partial class MenuView : ModuleBase
    {
        private MenuBase _menu;

        protected override void OnPreRender(EventArgs e)
        {
            using (new DNNContext(this))
            {
                try
                {
                    base.OnPreRender(e);

                    var menuStyle = GetStringSetting("MenuStyle");
                    if (String.IsNullOrEmpty(menuStyle))
                    {
                        _menu = null;
                        return;
                    }

                    var menuSettings = new Settings
                    {
                        MenuStyle = GetStringSetting("MenuStyle"),
                        NodeXmlPath = GetStringSetting("NodeXmlPath"),
                        NodeSelector = GetStringSetting("NodeSelector"),
                        IncludeContext = GetBoolSetting("IncludeContext"),
                        IncludeHidden = GetBoolSetting("IncludeHidden"),
                        IncludeNodes = GetStringSetting("IncludeNodes"),
                        ExcludeNodes = GetStringSetting("ExcludeNodes"),
                        NodeManipulator = GetStringSetting("NodeManipulator"),
                        TemplateArguments =
                                               DDRMenu.Settings.TemplateArgumentsFromSettingString(GetStringSetting("TemplateArguments")),
                        ClientOptions =
                                               DDRMenu.Settings.ClientOptionsFromSettingString(GetStringSetting("ClientOptions"))
                    };

                    MenuNode rootNode = null;
                    if (String.IsNullOrEmpty(menuSettings.NodeXmlPath))
                    {
                        rootNode =
                            new MenuNode(
                                Localiser.LocaliseDNNNodeCollection(
                                    Navigation.GetNavigationNodes(
                                        ClientID,
                                        Navigation.ToolTipSource.None,
                                        -1,
                                        -1,
                                        DNNAbstract.GetNavNodeOptions(menuSettings.IncludeHidden))));
                    }

                    _menu = MenuBase.Instantiate(menuStyle);
                    _menu.RootNode = rootNode;
                    _menu.ApplySettings(menuSettings);

                    _menu.PreRender();
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
        }

        protected override void Render(HtmlTextWriter htmlWriter)
        {
            using (new DNNContext(this))
            {
                try
                {
                    base.Render(htmlWriter);
                    if (_menu == null)
                    {
                        htmlWriter.WriteEncodedText("Please specify menu style in settings.");
                    }
                    else
                    {
                        _menu.Render(htmlWriter);
                    }
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }
            }
        }
    }
}