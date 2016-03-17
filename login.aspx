<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="login.aspx.cs" Inherits="FooBlog.login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h4>login</h4>
    <asp:Panel ID="errorPanel" runat="server" Visible="False">
        <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="formPanel" runat="server">
        <div class="row">
            <div class="input-field col s6">
                <asp:TextBox ID="usernameText" MaxLength="32" runat="server"></asp:TextBox>
                <label for="mainContent_usernameText">Username</label>
            </div>
            <div class="input-field col s6">
                <asp:TextBox ID="passText" MaxLength="128" TextMode="Password" runat="server"></asp:TextBox>
                <label for="mainContent_passText">Password</label>
            </div>
        </div>
        <div class="row">
            <div class="input-field col s12 center-align">
                <a id="loginButton" class="waves-effect waves-light blue lighten-1 btn-large" onclick="doLogin();">Login</a>
            </div>
        </div>
        <div class="row">
            <div class="col s12 center-align">
                <p style="text-align: center"><a href="reset_pass.aspx">Forgotten your password?</a></p>
            </div>
        </div>
    </asp:Panel>
    <asp:HiddenField ID="RequestToken" runat="server" />
</asp:Content>