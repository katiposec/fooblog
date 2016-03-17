<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" CodeBehind="search.aspx.cs" ValidateRequest="false" Inherits="FooBlog.search" %>
<%@ Import Namespace="FooBlog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h3>Search</h3>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    <asp:Panel runat="server" ID="searchPanel" Visible="False">
        <p class="grey-text">
            <asp:Label runat="server" ID="termLabel"></asp:Label>
        </p>
        <br />
        <h4>Ramblings</h4>
        <asp:DataList ID="postList" runat="server" DataKeyField="postid">
            <ItemTemplate>
                <a href='<%# (String.IsNullOrEmpty(Eval("postid").ToString()) ? "#" : String.Format("view_post.aspx?id={0}", Eval("postid"))) %>'>
                    <h5><%# (String.IsNullOrEmpty(Eval("posttitle").ToString()) ? "Undefined" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("posttitle"))) %></h5>
                </a>
                <p class="grey-text">
                    <%# (String.IsNullOrEmpty(Eval("posttime").ToString()) ? "" : FooStringHelper.DateTimeToString((DateTime) Eval("posttime"))) %><br />
                    Tagged: <a href='<%# (String.IsNullOrEmpty(Eval("catid").ToString()) ? "#" : String.Format("view_category.aspx?id={0}", Eval("catid"))) %>'><%#(String.IsNullOrEmpty(Eval("catname").ToString()) ? "Undefined" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("catname"))) %></a>
                </p>
                <p><%#(String.IsNullOrEmpty(Eval("postbrief").ToString()) ? "" : Server.HtmlDecode((string) Eval("postbrief"))) %></p>
                <br />
                <a href='<%# (String.IsNullOrEmpty(Eval("postid").ToString()) ? "#" : String.Format("view_post.aspx?id={0}", Eval("postid"))) %>'>Read more</a>
            </ItemTemplate>
        </asp:DataList>
        <asp:Label ID="postErrorLabel" ForeColor="red" runat="server"></asp:Label>
        <br />
        <h4>Merchandise</h4>
        <asp:DataList ID="merchList" runat="server" DataKeyField="merchid">
            <ItemTemplate>
                <a href='<%# (String.IsNullOrEmpty(Eval("merchid").ToString()) ? "#" : String.Format("view_item.aspx?id={0}", Eval("merchid"))) %>'>
                    <h5><%#(String.IsNullOrEmpty(Eval("merchname").ToString()) ? "Undefined" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchname"))) %></h5>
                </a>
                <p class="grey-text">
                    Price: <%#(String.IsNullOrEmpty(Eval("merchprice").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchprice"))) %>
                </p>
                <p><%#(String.IsNullOrEmpty(Eval("merchbrief").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchbrief"))) %></p>
                <br />
                <a href='<%# (String.IsNullOrEmpty(Eval("merchid").ToString()) ? "#" : String.Format("view_item.aspx?id={0}", Eval("merchid"))) %>'>Read more</a>
            </ItemTemplate>
        </asp:DataList>
        <asp:Label ID="merchErrorLabel" ForeColor="red" runat="server"></asp:Label>
    </asp:Panel>
</asp:Content>