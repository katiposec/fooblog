<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="view_item.aspx.cs" Inherits="FooBlog.view_item" %>
<%@ Import Namespace="FooBlog" %>
<%@ Import Namespace="Microsoft.Security.Application" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <asp:DataList ID="merchList" runat="server" DataKeyField="merchid">
        <ItemTemplate>
            <div class="left-align" style="width: 30%">
                <img class="responsive-img materialboxed" style="border-radius: 2px" src='<%# (String.IsNullOrEmpty(Eval("merchimg").ToString()) ? FooStringHelper.MakeImageUrl("merch_default.jpg") : FooStringHelper.MakeImageUrl((string) Eval("merchimg"))) %>' />
            </div>
            <h5><%#(String.IsNullOrEmpty(Eval("merchname").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchname"))) %></h5>
            <p class="grey-text">
                Price: <%#(String.IsNullOrEmpty(Eval("merchprice").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("merchprice"))) %>
            </p>
            <p><%#(String.IsNullOrEmpty(Eval("merchbody").ToString()) ? "" : Sanitizer.GetSafeHtmlFragment((string) Eval("merchbody"))) %></p>
        </ItemTemplate>
    </asp:DataList>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    <br />
    <asp:Panel ID="reviewPanel" runat="server" Visible="False">
        <h5>Reviews</h5>
        <asp:DataList ID="reviewList" runat="server" DataKeyField="reviewid">
            <ItemTemplate>
                <div class="row z-depth-1" style="padding-bottom: 5px; padding-top: 10px;">
                    <div class="col s1">
                        <a href='<%# (String.IsNullOrEmpty(Eval("userid").ToString()) ? "#" : String.Format("view_profile.aspx?id={0}", Eval("userid"))) %>'>
                            <img class="responsive-img" style="border-radius: 2px" src='<%# (String.IsNullOrEmpty(Eval("profileimg").ToString()) ? FooStringHelper.MakeImageUrl("profile_default.jpg") : FooStringHelper.MakeImageUrl((string) Eval("profileimg"))) %>' />
                        </a>
                    </div>
                    <div class="col s11" style="padding-bottom: 5px">
                        <p>
                            <%#(String.IsNullOrEmpty(Eval("reviewbody").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("reviewbody"))) %>
                        </p>
                        <p style="font-size: 12px">
                            <a href='<%# (String.IsNullOrEmpty(Eval("userid").ToString()) ? "#" : String.Format("view_profile.aspx?id={0}", Eval("userid"))) %>'><%#(String.IsNullOrEmpty(Eval("useralias").ToString()) ? "Undefined" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("useralias"))) %></a>
                            <span class="grey-text">&nbsp;@&nbsp;<%# (String.IsNullOrEmpty(Eval("reviewtime").ToString()) ? "" : FooStringHelper.DateTimeToString((DateTime) Eval("reviewtime"))) %></span>
                        </p>
                    </div>
                </div>
            </ItemTemplate>
        </asp:DataList>
        <asp:Label ID="reviewLabel" runat="server"></asp:Label>
        <div class="row">
            <div class="input-field col s12 center-align">
                <asp:TextBox ID="reviewText" MaxLength="1024" runat="server"></asp:TextBox>
                <label for="mainContent_reviewText">Leave a Review</label>
            </div>
        </div>
        <div class="row">
            <div class="input-field col s12 center-align">
                <asp:LinkButton ID="submitButton" runat="server" CausesValidation="False" CssClass="waves-effect waves-light blue lighten-1 btn-large" OnClientClick=" if (!validateReview()) return false; " OnClick="submitButton_Click">Submit</asp:LinkButton>
            </div>
        </div>
        <asp:HiddenField ID="RequestToken" runat="server" />
        <asp:Label ID="reviewErrorLabel" runat="server"></asp:Label>
    </asp:Panel>
</asp:Content>