﻿<%@ Page
    Language="C#"
    MasterPageFile="~/Site.Master"
    Title="Home"
    AutoEventWireup="true"
    CodeBehind="default.aspx.cs"
    Inherits="dev_web_api._default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="css/style.min.css">
    <link rel="stylesheet" href="css/countdown.css" />
    <style type="text/css">
        .menu:after {
            content: '\2807';
            font-size: 20px;
            float: right
        }

        .menu { 
            display: none;
        }

        .tblcell:hover .menu {
            display: block;
        }
    </style>
    <script type="text/javascript">
        function popup(mylink, windowname, w, h) {
            if (!window.focus) return true;
            var href;
            if (typeof (mylink) == 'string')
                href = mylink;
            else
                href = mylink.href;
            window.open(href, windowname, "width=" + w + ",height=" + h + ",scrollbars=yes,toolbar=no");
            return false;
        }

    </script>
    <div>
        <div>
            <div id="app" style="float: left"></div>
            <div style="margin: 5px 0 0 20px; float: left">
                <span>Agent Group:</span>
                <asp:DropDownList ID="ddlAgentGroups"
                    runat="server"
                    OnSelectedIndexChanged="ddlAgentGroups_SelectedIndexChanged"
                    AutoPostBack="true">
                </asp:DropDownList>
                <asp:CheckBox ID="chkEmailOpt" runat="server"
                    Style="padding-left: 40px"
                    OnCheckedChanged="chkEmailOpt_CheckedChanged"
                    AutoPostBack="true" />
                <span>Send Email if Agent fails</span> &nbsp&nbsp
                <span style="font-size: small; padding-left: 70px">Last Updated: <%= DateTime.Now %>
                </span>

            </div>
            <div style="clear: both" />

        </div>

        <table id="tblMonitor" class="monitor" runat="server" />

        <div style="margin-top: 50px">
            <span class="heading">Agent Installation:</span>
            <ul>
                <li>Download and Install:
                             
                            <ol>
                                <li>Download 
                                    <a
                                        href="https://github.com/Officeclip/devmon_agent/releases/download/Agent-0.5/AgentSetup.zip"
                                        style="font-weight: bold">AgentSetup.zip</a></li>
                                <li>Unzip AgentSetup.zip</li>
                                <li>Run <b>Setup.exe</b></li>
                                <li>Enter Server Url: <b><%= GetWebUri() %></b></li>
                                <li>Enter Server Key: <b><%= serverGuid %></b></li>
                            </ol>

                </li>


                <li>Uninstall:
                    <ul>
                        <li>Go to the <i>Control Panel > Programs and Features</i>
                            and uninstall the Agent Monitor
                        </li>
                    </ul>

                </li>
            </ul>
        </div>
    </div>

    <script src="js/countdown.js"></script>

</asp:Content>
