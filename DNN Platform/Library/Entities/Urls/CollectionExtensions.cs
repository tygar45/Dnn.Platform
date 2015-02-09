﻿#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2014
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using DotNetNuke.Collections.Internal;

namespace DotNetNuke.Entities.Urls
{
    public static class CollectionExtensions
    {
        public static void LoadFromXmlFile(this Dictionary<int, List<ParameterReplaceAction>> actions, string fileName, int portalId, bool portalSpecific, ref List<string> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException("messages");
            }
            messages = new List<string>();
            if (File.Exists(fileName))
            {
                var rdr = new XmlTextReader(fileName);
                while (rdr.Read())
                {
                    switch (rdr.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (rdr.Name == "parameterReplace")
                            {
                                //now set up the action
                                string portalIdRaw = rdr.GetAttribute("portalId");
                                int rulePortalId = -1;
                                if (portalIdRaw != null)
                                {
                                    Int32.TryParse(portalIdRaw, out rulePortalId);
                                }
                                //807 : if portal specific then import all regardless of portal id specified
                                if (rulePortalId == portalId || rulePortalId == -1 || portalSpecific)
                                {
                                    int actionCount = 0;
                                    string tabIdRaw = rdr.GetAttribute("tabIds") ?? rdr.GetAttribute("tabId");
                                    string tabNames = rdr.GetAttribute("tabNames");
                                    string name = rdr.GetAttribute("name");
                                    List<int> tabIds = XmlHelpers.TabIdsFromAttributes(tabIdRaw, tabNames, portalId,
                                                                                       ref messages);
                                    foreach (int tabId in tabIds)
                                    {
                                        var action = new ParameterReplaceAction
                                        {
                                            LookFor = rdr.GetAttribute("lookFor"),
                                            ReplaceWith = rdr.GetAttribute("replaceWith"),
                                            PortalId = portalId,
                                            Name = name,
                                            TabId = tabId
                                        };
                                        string changeToSiteRootRaw = rdr.GetAttribute("changeToSiteRoot");
                                        bool changeToSiteRoot;
                                        bool.TryParse(changeToSiteRootRaw, out changeToSiteRoot);
                                        action.ChangeToSiteRoot = changeToSiteRoot;

                                        List<ParameterReplaceAction> tabActionCol;
                                        if (actions.ContainsKey(action.TabId))
                                        {
                                            tabActionCol = actions[action.TabId];
                                        }
                                        else
                                        {
                                            tabActionCol = new List<ParameterReplaceAction>();
                                            actions.Add(action.TabId, tabActionCol);
                                        }
                                        tabActionCol.Add(action);

                                        actionCount++;
                                        messages.Add(name + " replace actions added:" + actionCount.ToString());
                                    }
                                }
                            }
                            break;

                        case XmlNodeType.EndElement:
                            break;
                    }
                }
                rdr.Close();
            }
            else
            {
                messages.Add("File not Found: " + fileName);
            }
        }

        /// <summary>
        /// Returns all the redirect rules for the specified portal
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="fileName"></param>
        /// <param name="portalId"></param>
        /// <param name="portalSpecific">If true, all rules belong to supplied portalId, even if not specified.</param>
        /// <param name="messages"></param>
        /// <remarks>807 : change to allow specificatoin of assumption that all rules belong to the supplied portal</remarks>
        public static void LoadFromXmlFile(this Dictionary<int, List<ParameterRedirectAction>> actions, string fileName, int portalId, bool portalSpecific, ref List<string> messages)
        {
            if (messages == null)
            {
                messages = new List<string>();
            }
            if (File.Exists(fileName))
            {
                var rdr = new XmlTextReader(fileName);
                while (rdr.Read())
                {
                    switch (rdr.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (rdr.Name == "parameterRedirect")
                            {
                                var tabMessages = new List<string>();
                                int actionCount = 0;
                                //now set up the action
                                string portalIdRaw = rdr.GetAttribute("rulePortalId");
                                if (string.IsNullOrEmpty(portalIdRaw))
                                {
                                    portalIdRaw = rdr.GetAttribute("portalId");
                                }
                                int rulePortalId = -1;
                                if (portalIdRaw != null)
                                {
                                    Int32.TryParse(portalIdRaw, out rulePortalId);
                                }
                                if (rulePortalId == portalId || rulePortalId == -1 || portalSpecific)
                                //if portal specific, all rules are assumed to belong to the portal
                                {
                                    string tabIdRaw = rdr.GetAttribute("tabIds");
                                    string tabNames = rdr.GetAttribute("tabNames");
                                    string name = rdr.GetAttribute("name");
                                    string fromSiteRootRaw = rdr.GetAttribute("fromSiteRoot");
                                    string fromDefaultRaw = rdr.GetAttribute("fromDefault");
                                    string changeToSiteRootRaw = rdr.GetAttribute("changeToSiteRoot");
                                    bool fromDefault;
                                    bool fromSiteRoot;
                                    bool changeToSiteRoot;
                                    bool.TryParse(fromDefaultRaw, out fromDefault);
                                    bool.TryParse(fromSiteRootRaw, out fromSiteRoot);
                                    bool.TryParse(changeToSiteRootRaw, out changeToSiteRoot);
                                    List<int> tabIds = XmlHelpers.TabIdsFromAttributes(tabIdRaw, tabNames, portalId,
                                                                                       ref tabMessages);
                                    foreach (int tabId in tabIds)
                                    {
                                        var action = new ParameterRedirectAction
                                        {
                                            PortalId = portalId,
                                            LookFor = rdr.GetAttribute("lookFor"),
                                            RedirectTo = rdr.GetAttribute("redirectTo"),
                                            Name = name,
                                            Action = rdr.GetAttribute("action"),
                                            ChangeToSiteRoot = changeToSiteRoot,
                                            TabId = tabId
                                        };
                                        if (fromDefault)
                                        {
                                            //check for 'fromDefault' attribute
                                            action.ForDefaultPage = true;
                                            action.TabId = -2;
                                        }
                                        else
                                        {
                                            //or support the older convention, which was to include a tabid of -2
                                            action.ForDefaultPage = tabId == -2;
                                        }
                                        if (fromSiteRoot)
                                        {
                                            action.TabId = -3; //site root marker
                                        }
                                        List<ParameterRedirectAction> tabActionCol;
                                        if (actions.ContainsKey(action.TabId))
                                        {
                                            tabActionCol = actions[action.TabId];
                                        }
                                        else
                                        {
                                            tabActionCol = new List<ParameterRedirectAction>();
                                            actions.Add(action.TabId, tabActionCol);
                                        }
                                        tabActionCol.Add(action);
                                        actionCount++;
                                    }
                                    messages.Add(name + " redirect actions added:" + actionCount.ToString());
                                }
                                if (tabMessages.Count > 0)
                                {
                                    messages.AddRange(tabMessages);
                                }
                            }
                            break;

                        case XmlNodeType.EndElement:
                            break;
                    }
                }
                rdr.Close();
            }
        }

        public static void LoadFromXmlFile(this Dictionary<int, SharedList<ParameterRewriteAction>> actions, string fileName, int portalId, bool portalSpecific, ref List<string> messages)
        {
            if (messages == null)
            {
                messages = new List<string>();
            }
            if (File.Exists(fileName))
            {
                var rdr = new XmlTextReader(fileName);
                while (rdr.Read())
                {
                    switch (rdr.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (rdr.Name == "parameterRewrite")
                            {
                                string portalIdRaw = rdr.GetAttribute("portalId");
                                int rulePortalId = -1;
                                int actionCount = 0;
                                if (portalIdRaw != null)
                                {
                                    Int32.TryParse(portalIdRaw, out rulePortalId);
                                }
                                if (rulePortalId == portalId || rulePortalId == -1 || portalId == -1 || portalSpecific)
                                {
                                    //now set up the action
                                    string tabIdRaw = rdr.GetAttribute("tabIds");
                                    string tabNames = rdr.GetAttribute("tabNames");
                                    string name = rdr.GetAttribute("name");
                                    string fromSiteRootRaw = rdr.GetAttribute("fromSiteRoot");
                                    bool fromSiteRoot;
                                    bool.TryParse(fromSiteRootRaw, out fromSiteRoot);
                                    List<int> tabIds = XmlHelpers.TabIdsFromAttributes(tabIdRaw, tabNames, portalId,
                                                                                       ref messages);
                                    foreach (int tabId in tabIds)
                                    {
                                        var action = new ParameterRewriteAction
                                        {
                                            LookFor = rdr.GetAttribute("lookFor"),
                                            RewriteTo = rdr.GetAttribute("rewriteTo"),
                                            Name = name,
                                            TabId = tabId
                                        };
                                        if (fromSiteRoot)
                                        {
                                            action.ForSiteRoot = true;
                                            action.TabId = -3;
                                        }
                                        else
                                        {
                                            //older rule specified tabid -3 meant site root
                                            action.ForSiteRoot = tabId == -3;
                                        }

                                        action.PortalId = portalId;
                                        SharedList<ParameterRewriteAction> tabActionCol;
                                        if (actions.ContainsKey(action.TabId))
                                        {
                                            tabActionCol = actions[action.TabId];
                                        }
                                        else
                                        {
                                            tabActionCol = new SharedList<ParameterRewriteAction>();
                                            actions.Add(action.TabId, tabActionCol);
                                        }
                                        tabActionCol.Add(action);
                                        actionCount++;
                                    }
                                    messages.Add(name + " rewrite actions added:" + actionCount.ToString());
                                }
                            }


                            break;

                        case XmlNodeType.EndElement:
                            break;
                    }
                }
                rdr.Close();
            }
            else
            {
                messages.Add("Filename does not exist:" + fileName);
            }
        }

        public static Dictionary<string, string> CreateDictionaryFromString(string delimitedString, char pairSeparator, char separator)
        {
            var dictionary = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(delimitedString))
            {
                var pairs = delimitedString.Split(pairSeparator);
                foreach (string pair in pairs)
                {
                    if (!String.IsNullOrEmpty(pair))
                    {
                        var chars = pair.Split(separator);
                        dictionary[chars[0]] = chars[1];
                    }
                }
            }

            return dictionary;
        }
    }
}
