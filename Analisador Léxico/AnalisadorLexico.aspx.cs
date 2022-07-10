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
            var itens = Pattern.AnalisadorLexico(TextBox1.Text);
            var itensSintaticamente = Pattern.AnalisadorSintatico(itens);
            ListPadroes.DataSource = itensSintaticamente.ToList();
            ListPadroes.DataBind();
        }
    }
}