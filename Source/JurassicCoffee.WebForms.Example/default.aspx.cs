using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace JurassicCoffee.WebForms.Example
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);

            foreach (var control in this.Page.Controls.Cast<Control>())
            {
               
                foreach (var control2 in control.Controls.Cast<Control>())
                    writer.Write((object)control2);
            }
        }
    }
}