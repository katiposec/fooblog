using System;
using System.Web.UI;

namespace FooBlog
{
    public partial class rng : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void genButton_Click(object sender, EventArgs e)
        {
            outLabel.Text = FooStringHelper.RandomString(int.Parse(lenBox.Text));
        }

        protected void hashButton_Click(object sender, EventArgs e)
        {
            outLabel.Text = FooCryptHelper.CreateShaHash(inBox.Text);
        }
    }
}