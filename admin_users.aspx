<%@ Page Title="" Language="C#" MasterPageFile="~/Template.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="admin_users.aspx.cs" Inherits="FooBlog.admin_users" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <h3>User Management</h3>
    <asp:Label ID="errorLabel" ForeColor="red" runat="server"></asp:Label>
    <asp:GridView ID="userGrid" runat="server" AutoGenerateColumns="False" ShowFooter="True" DataKeyNames="userid"
                  OnRowDeleting="GridView_Delete" OnRowUpdating="GridView_Update" OnRowEditing="GridView_Edit"
                  OnRowCancelingEdit="GridView_Cancel" OnRowCommand="GridView_Command" OnRowDataBound="GridView_RowDataBound" Width="720px">
        <EmptyDataTemplate>No results returned.</EmptyDataTemplate>

        <Columns>
            <asp:BoundField DataField="userid" HeaderText="User ID" InsertVisible="False" ReadOnly="True" SortExpression="username" />
            <asp:TemplateField HeaderText="Name">
                <EditItemTemplate>
                    <asp:TextBox ID="txtUserName" MaxLength="32" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("username").ToString()) ? "Undefined" : Server.HtmlDecode((string) Eval("username"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtUserNameFooter" MaxLength="32" runat="server"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="nameLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("username").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("username"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Alias">
                <EditItemTemplate>
                    <asp:TextBox ID="txtUserAlias" MaxLength="32" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("useralias").ToString()) ? "Undefined" : Server.HtmlDecode((string) Eval("useralias"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtUserAliasFooter" MaxLength="32" runat="server"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="aliasLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("useralias").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("useralias"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Email">
                <EditItemTemplate>
                    <asp:TextBox ID="txtEmail" MaxLength="64" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("email").ToString()) ? "Undefined" : Server.HtmlDecode((string) Eval("email"))) %>'></asp:TextBox>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtEmailFooter" MaxLength="64" runat="server"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="emailLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("email").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("email"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Password">
                <EditItemTemplate>
                </EditItemTemplate>
                <FooterTemplate>
                    <asp:TextBox ID="txtUserPasswordFooter" MaxLength="128" TextMode="Password" runat="server"></asp:TextBox>
                </FooterTemplate>
                <ItemTemplate>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Group">
                <EditItemTemplate>
                    <div class="input-field">
                        <asp:DropDownList ID="groupDropdown" AutoPostBack="False" runat="server" />
                    </div>
                </EditItemTemplate>
                <FooterTemplate>
                    <div class="input-field">
                        <asp:DropDownList ID="groupDropdownFooter" AutoPostBack="False" runat="server" />
                    </div>
                </FooterTemplate>
                <ItemTemplate>
                    <asp:Label ID="groupLabel" runat="server" Text='<%#(String.IsNullOrEmpty(Eval("groupname").ToString()) ? "<i>Undefined</i>" : Microsoft.Security.Application.Encoder.HtmlEncode((string) Eval("groupname"))) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Edit" ShowHeader="False">
                <EditItemTemplate>
                    <asp:LinkButton ID="UpdateButton" CausesValidation="False" CommandName="Update" Text="Update" OnClientClick=" if (!validateUserGridUpdate()) return false; " runat="server"></asp:LinkButton>
                    <asp:LinkButton ID="CancelButton" CausesValidation="False" CommandName="Cancel" Text="Cancel" runat="server"></asp:LinkButton>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="EditButton" CausesValidation="False" CommandName="Edit" Text="Edit" runat="server"></asp:LinkButton>
                </ItemTemplate>
                <FooterTemplate>
                    <asp:LinkButton ID="NewButton" CausesValidation="False" CommandName="AddNew" Text="Add New" OnClientClick=" if (!validateUserGridInsert()) return false; " runat="server"></asp:LinkButton>
                </FooterTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Delete" ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="DeleteButton" CausesValidation="False" CommandName="Delete" Text="Delete" OnClientClick=" if (!window.confirm('Are you sure?')) return false; " runat="server"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:HiddenField ID="RequestToken" runat="server" />
</asp:Content>