<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="register.aspx.cs" Inherits="FooBlog.register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h3>register</h3>
    <h5>basic details</h5>
    <asp:Panel ID="errorPanel" runat="server" Visible="False">
        <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    </asp:Panel>
    <asp:Panel ID="formPanel" runat="server">
        <div class="row">
            <div class="input-field col s6">
                <asp:TextBox ID="aliasText" MaxLength="32" runat="server"></asp:TextBox>
                <label for="mainContent_aliasText">Alias</label>
            </div>
            <div class="input-field col s6">
                <asp:TextBox ID="emailText" MaxLength="64" runat="server"></asp:TextBox>
                <label for="mainContent_emailText">Email</label>
            </div>
        </div>
        <h5>contact</h5>
        <div class="row">
            <div class="input-field col s12">
                <asp:TextBox ID="addressText" MaxLength="128" runat="server"></asp:TextBox>
                <label for="mainContent_addressText">Physical Address</label>
            </div>
        </div>
        <div class="row">
            <div class="input-field col s6">
                <asp:TextBox ID="cityText" MaxLength="32" runat="server"></asp:TextBox>
                <label for="mainContent_cityText">City</label>
            </div>
            <div class="input-field col s6">
                <asp:TextBox ID="countryText" MaxLength="32" runat="server"></asp:TextBox>
                <label for="mainContent_countryText">Country</label>
            </div>
        </div>
        <h5>login</h5>
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
        <h5>confirm</h5>
        <div class="row">
            <div class="col s12 center-align">
                <p style="text-align: center">Are you sure that all of the details that you have entered are correct?</p>
            </div>
        </div>
        <div class="row">
            <div class="input-field col s12 center-align">
                <asp:LinkButton ID="submitButton" runat="server" CausesValidation="False" CssClass="waves-effect waves-light blue lighten-1 btn-large" OnClientClick=" if (!validateRegistration()) return false; " OnClick="submitButton_Click">Submit</asp:LinkButton>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="successPanel" runat="server" Visible="False">
        <asp:Label ID="successLabel" runat="server"></asp:Label>
    </asp:Panel>
    <asp:HiddenField ID="RequestToken" runat="server" />
</asp:Content>