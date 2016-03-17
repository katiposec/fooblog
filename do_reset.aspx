<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="do_reset.aspx.cs" Inherits="FooBlog.do_reset" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h4>reset password</h4>
    <asp:Panel ID="errorPanel" runat="server" Visible="False">
        <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="formPanel" runat="server" Visible="False">
        <div class="row">
            <div class="input-field col s6">
                <asp:TextBox ID="passText" TextMode="Password" MaxLength="128" runat="server"></asp:TextBox>
                <label for="mainContent_passText">New Password</label>
            </div>
            <div class="input-field col s6">
                <asp:TextBox ID="confirmText" TextMode="Password" MaxLength="128" runat="server"></asp:TextBox>
                <label for="mainContent_confirmText">Confirm Password</label>
            </div>
        </div>
        <div class="row">
            <div class="input-field col s12 center-align">
                <asp:LinkButton ID="submitButton" runat="server" CausesValidation="False" CssClass="waves-effect waves-light blue lighten-1 btn-large" OnClick="submitButton_Click">Submit</asp:LinkButton>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="successPanel" runat="server" Visible="False">
        <asp:Label ID="successLabel" runat="server"></asp:Label>
    </asp:Panel>
    <asp:HiddenField ID="RequestToken" runat="server" />
</asp:Content>