using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace FormEntrada
{
    public partial class FormAlterarSenhaExistente : Form
    {

        private string connectionString = Properties.Settings.Default.CSLG;
        string nomeDoUsuario = "";

        public FormAlterarSenhaExistente()
        {
            InitializeComponent();
            MaximizeBox = false;
            txtAlterarsenhaBanco.PasswordChar = '*';
        }
        public FormAlterarSenhaExistente(string nomerecebido)
        {
            InitializeComponent();
            nomeDoUsuario = nomerecebido;
            txtAlterarsenhaBanco.PasswordChar = '*';
        }

        private void BtnAlterarBanco_Click(object sender, EventArgs e)
        {
            if (txtAlterarsenhaBanco.Text == "")
            {
                MessageBox.Show("Por gentileza insira uma senha válida","INFORMAÇÃO",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            else
            {
                string senhaInformada = txtAlterarsenhaBanco.Text;

                if (validandoSenha(senhaInformada))
                {
                    string senhaInformadaEncriprotagrafada = encripritadorDeSenha(senhaInformada);
                    if (alterarNoBanco(nomeDoUsuario, senhaInformadaEncriprotagrafada) > 0)
                    {
                        MessageBox.Show("Senha alterada com sucesso", "SUCESSO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Form1 abrirForm = new Form1();
                        abrirForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Ocorreu um erro ao alterar a senha. Tente novamente.", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

            }
        }
        private bool validandoSenha(string senhaCriterios)
        {
            if (senhaCriterios.Length < 6)
            {
                MessageBox.Show("A senha deve conter pelo menos 6 caracteres.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtAlterarsenhaBanco.Clear();
              

                return false;
            }

            bool hasLetter = false;
            bool hasNumber = false;
            bool hasSpecialChar = false;

            foreach (char c in senhaCriterios) // aqui verifica se a senha contem letra percorrendo a senha com foreach e usando hasLetter que significa tem letra.
            {
                if (char.IsLetter(c))
                {
                    hasLetter = true;
                }
                else if (char.IsDigit(c))
                {
                    hasNumber = true;
                }
                else if (!char.IsLetterOrDigit(c))
                {
                    hasSpecialChar = true;
                }
            }

            if (!hasLetter || !hasNumber || !hasSpecialChar)
            {
                MessageBox.Show("A senha deve conter letras,números e pelo menos um caractere especial .", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtAlterarsenhaBanco.Clear();
                return false;
            }

            return true;
        }
        private string encripritadorDeSenha(string input) //cripritografa a senha
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        int alterarNoBanco(string nomeUsuario, string senhaAlterada)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // aqui obtem o ID do usuário pelo nome de usuário
                string sqlObterId = "SELECT ID FROM users WHERE USERS = @Nome";
                SqlCommand commandObterId = new SqlCommand(sqlObterId, connection);
                commandObterId.Parameters.AddWithValue("@Nome", nomeUsuario);

                
                object result = commandObterId.ExecuteScalar();

                if (result != null)
                {
                    //converte o resultado pois a coluna e inteiro no banco de dados.
                    int idUsuario = Convert.ToInt32(result);

                    // Atualiza a senha do usuário com o ID obtido
                    string sqlAtualizarSenha = "UPDATE users SET SENHA = @Senha WHERE ID = @Id";
                    SqlCommand commandAtualizarSenha = new SqlCommand(sqlAtualizarSenha, connection);
                    commandAtualizarSenha.Parameters.AddWithValue("@Senha", senhaAlterada);
                    commandAtualizarSenha.Parameters.AddWithValue("@Id", idUsuario);

                    int rowsAffected = commandAtualizarSenha.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        
                        return idUsuario;
                    }
                    else
                    {
                       
                        return -1;
                    }
                }
                else
                {
                    
                    return -1;
                }
            }
        }

        private void FormAlterarSenhaExistente_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
    }
}
