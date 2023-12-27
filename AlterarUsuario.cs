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
    public partial class AlterarUsuario : Form
    {
        

        private string connectionString = Properties.Settings.Default.CSLG;

        private SqlConnection connection;

        public AlterarUsuario()
        {
            InitializeComponent();
            MaximizeBox = false;
            txtSenhaNovo.PasswordChar = '*';
            connection = new SqlConnection(connectionString);

        }
        private void AlterarUsuario_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private bool validandoSenha(string senhaCriterios)
        {
            if (senhaCriterios.Length < 6)
            {
                MessageBox.Show("A senha deve conter pelo menos 6 caracteres.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtSenhaNovo.Clear();
                txtLoginNovo.Clear();

                return false;
            }

            bool hasLetter = false;
            bool hasNumber = false;
            bool hasSpecialChar = false;

            foreach (char c in senhaCriterios)     // aqui verifica se a senha contem letra percorrendo a senha com foreach e usando hasLetter que significa tem letra.
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
                MessageBox.Show("A senha deve conter letras, números e pelo menos um caractere especial.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtSenhaNovo.Clear();
                txtLoginNovo.Clear();

                return false;
            }

            return true;
        }

        private bool VerificandoAjaExistencia(string usuario)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM users WHERE USERS = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", usuario);
                    int userCount = Convert.ToInt32(command.ExecuteScalar());

                    return userCount > 0;
                }
            }
        }



        private void btnAlterar_Click(object sender, EventArgs e)
        {
            string novoUsuario = txtLoginNovo.Text;
            string novaSenha = txtSenhaNovo.Text;

            if (!validandoSenha(novaSenha))
            {
                return;
            }

            if (VerificandoAjaExistencia(novoUsuario))
            {
                MessageBox.Show("O usuário já existe no banco de dados.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var resposta = MessageBox.Show("Deseja mudar a senha?.", "Erro", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(resposta == DialogResult.Yes)
                {
                    FormAlterarSenhaExistente abrirForm = new FormAlterarSenhaExistente(novoUsuario);
                    abrirForm.Show();
                    this.Hide();

                }
                txtSenhaNovo.Clear();
                txtLoginNovo.Clear();
                
                return;
            }
            

               string senhaCriptografada = encripritadorDeSenha(novaSenha);
           
                string username = Environment.UserName.ToLower(); // verifica se a pessoa logada no computador faz parte do nosso setor

            if (username == "ysaac.queiroz" || username == "larissa.silva" || username == "cicero.chagas" || username == "laurice.nogueira")
            {
                InserirNovoUsuarioNoBanco(novoUsuario, senhaCriptografada);
                MessageBox.Show("Usuário e senha inseridos com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtSenhaNovo.Clear();
                txtLoginNovo.Clear();
            }
            else
            {
                MessageBox.Show("Você não tem permissão para fazer isso", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


            
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

        private void InserirNovoUsuarioNoBanco(string usuario, string senhaCriptografada)
        {
            try
            {
                connection.Open();
                string query = "INSERT INTO users (USERS, SENHA) VALUES (@Usuario, @Senha)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Usuario", usuario);
                command.Parameters.AddWithValue("@Senha", senhaCriptografada);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inserir usuário: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

       
        private void BtnHome_Click(object sender, EventArgs e)
        {
            Form1 ChamarFormPrincipal = new Form1();
            ChamarFormPrincipal.Show();
            this.Hide();
        }


    }
}


