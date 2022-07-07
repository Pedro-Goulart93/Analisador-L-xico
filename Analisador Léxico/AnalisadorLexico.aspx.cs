using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Analisador_Léxico
{
    public partial class AnalisadorLexico : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Texto_TextChanged(object sender, EventArgs e)
        {
            var itens = Pattern.Analisador(TextBox1.Text);
            ListPadroes.DataSource = itens.ToList();
            ListPadroes.DataBind();
        }
    }
}