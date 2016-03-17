<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" CodeBehind="rng.aspx.cs" Inherits="FooBlog.rng" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h4>string ops</h4>
    <h5>random</h5>
    <asp:TextBox ID="lenBox" runat="server"></asp:TextBox><asp:Button ID="genButton" runat="server" Text="Generate"  OnClick="genButton_Click" />
    <h5>hash</h5>
    <asp:TextBox ID="inBox" runat="server"></asp:TextBox><asp:Button ID="hashButton" runat="server" Text="Generate"  OnClick="hashButton_Click" />
    <h5>output</h5>
    <asp:Label ID="outLabel" runat="server"></asp:Label>
</asp:Content>