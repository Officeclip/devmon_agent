<%@ Page
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="software.aspx.cs"
    Inherits="dev_web_api.software" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style type="text/css">
        td {
            padding-left: 10px;
            white-space: nowrap;
        }
    </style>
    <link rel="stylesheet" href="css/style.min.css">
    <link rel="stylesheet" href="css/spacings.min.css">
    <div style="margin: 20px">
        <div>
            <div style="display: inline">
                Agents:
                <asp:DropDownList ID="ddlAgents" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlAgents_SelectedIndexChanged" />
                <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
            </div>
            <div style="display: inline; margin-left: 20px">
                <asp:Literal ID="litDate" runat="server" />
            </div>
            <div style="display: inline; margin-left: 20px">
                <asp:Button ID="btnBack" runat="server"
                    Text="Back to Monitor"
                    Font-Bold="true"
                    OnClick="btnBack_Click" />
            </div>
        </div>
        <div style="margin-top: 10px">
            <asp:Label ID="lblEmptyData" runat="server"
                Text="Data for this agent is not available"
                ForeColor="Red" />
            <asp:GridView ID="grdSoftware" runat="server" CssClass="body-content" />
        </div>
    </div>
</asp:Content>
