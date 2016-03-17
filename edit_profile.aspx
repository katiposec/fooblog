<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="edit_profile.aspx.cs" Inherits="FooBlog.edit_profile" %>
<%@ Import Namespace="FooBlog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h3>Profile</h3>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    <asp:DetailsView ID="userView" DataKeyNames="userid" AllowPaging="False" AutoGenerateRows="False" runat="server" OnModeChanging="UserView_ModeChanging" OnItemUpdating="UserView_ItemUpdating" Width="720px">
        <Fields>
            <asp:BoundField DataField="userid" HeaderText="User ID" InsertVisible="False" ReadOnly="True" SortExpression="userid" />
            <asp:TemplateField HeaderText="Alias">
                <EditItemTemplate>
                    <asp:TextBox ID="txtUserAlias" MaxLength="64" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("useralias").ToString()) ? "" : Server.HtmlDecode((string) Eval("useralias"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="aliasLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("useralias").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("useralias"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Image">
                <EditItemTemplate>
                    <div class="file-field input-field">
                        <div class="waves-effect waves-light blue lighten-1 btn">
                            <span>Select</span>
                            <asp:FileUpload ID="imageUploadForm" runat="server" />
                        </div>
                        <div class="file-path-wrapper">
                            <input class="file-path validate" type="text">
                        </div>
                    </div>
                    <small>File must be an image less than 2MB.</small>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Image ID="profileImg" ImageUrl='<%# (String.IsNullOrEmpty(Eval("profileimg").ToString()) ? FooStringHelper.MakeImageUrl("profile_default.jpg") : FooStringHelper.MakeImageUrl((string) Eval("profileimg"))) %>' Width="150px" runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Email">
                <EditItemTemplate>
                    <asp:TextBox ID="txtUserEmail" MaxLength="64" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("email").ToString()) ? "" : Server.HtmlDecode((string) Eval("email"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="emailLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("email").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("email"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Address">
                <EditItemTemplate>
                    <asp:TextBox ID="txtUserAddress" MaxLength="128" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("address").ToString()) ? "" : Server.HtmlDecode((string) Eval("address"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="addressLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("address").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("address"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="City">
                <EditItemTemplate>
                    <asp:TextBox ID="txtUserCity" MaxLength="32" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("city").ToString()) ? "" : Server.HtmlDecode((string) Eval("city"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="cityLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("city").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("city"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Country">
                <EditItemTemplate>
                    <asp:TextBox ID="txtUserCountry" MaxLength="64" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("country").ToString()) ? "" : Server.HtmlDecode((string) Eval("country"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="countryLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("country").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("country"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Bio">
                <EditItemTemplate>
                    <asp:TextBox ID="txtUserBody" MaxLength="1024" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("profilebody").ToString()) ? "" : Server.HtmlDecode((string) Eval("profilebody"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="bodyLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("profilebody").ToString()) ? "" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("profilebody"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Editing">
                <EditItemTemplate>
                    <asp:LinkButton ID="updateButton" runat="server" CommandName="update" OnClientClick=" if (!validateUserView()) return false; ">Update</asp:LinkButton>
                    <asp:LinkButton ID="cancelButton" runat="server" CommandName="cancel">Cancel</asp:LinkButton>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="editButton" runat="server" CommandName="edit">Edit</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
        <EmptyDataTemplate>
            No results returned.
        </EmptyDataTemplate>
    </asp:DetailsView>
    <br />
    <h4>Reset Password</h4>
    <div class="row">
        <div class="input-field col s6">
            <input id="passText" type="password" />
            <label for="passText">New Password</label>
        </div>
        <div class="input-field col s6">
            <input id="passText_confirm" type="password" />
            <label for="passText_confirm">Confirm Password</label>
        </div>
    </div>
    <div class="row">
        <div class="input-field col s12 center-align">
            <a id="resetButton" class="waves-effect waves-light blue lighten-1 btn-large" onclick=" resetPassword(); ">Reset</a>
        </div>
    </div>
    <asp:HiddenField ID="RequestToken" runat="server" />
</asp:Content>