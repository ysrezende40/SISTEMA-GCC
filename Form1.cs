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
using System.IO;

namespace FormEntrada
{
    public partial class Form1 : Form
    {
        private string connectionString = Properties.Settings.Default.CSLG;
       
        public Form1() //o form sempre precisa se iniciar vazio e depois se quiser passar dados você precisa criar um novo public form1 com os paramentros para receber os dados
        {
            InitializeComponent();
            MaximizeBox = false;
            txtSenha.PasswordChar = '*'; //esconde oque o usario digita

            this.FormClosing += Form1_FormClosing; //evento de fechar a aplicação, final do codigo


           

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle; //evita o redimensionamento da tela. 


            string caminhoDaImagem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Imagens", "Logotipo_GCC.png");


            if (System.IO.File.Exists(caminhoDaImagem))
            {

                ImgLogo.Image = System.Drawing.Image.FromFile(caminhoDaImagem);
            }
            else
            {
                MessageBox.Show("A imagem não foi encontrada.");
            }

        }

        /*public Form1(string ex) 
        {
            InitializeComponent();
         


        }*/

        private void btnRedAlterar_Click(object sender, EventArgs e)
        {
            AlterarUsuario chamarForm = new AlterarUsuario();
            chamarForm.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string loginDigitado = txtLogin.Text;
            string senhaDigitada = txtSenha.Text;

            string senhaCriptografadaDoBanco = ConsultaSenhaBanco(loginDigitado); // Consultar o banco de dados para obter a senha criptografada


            string senhaDigitadaCriptografada = encripritadorDeSenha(senhaDigitada); // Criptografar a senha digitada pelo usuário para comparar com a senha do banco

            if (senhaDigitadaCriptografada == senhaCriptografadaDoBanco)
            {

                txtLogin.BackColor = Color.Green;
                txtSenha.BackColor = Color.Green;

                MessageBox.Show("Login feito com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);



                FormTelaincialSystem abrirForm = new FormTelaincialSystem();
                abrirForm.Show();
                this.Hide();


            }
            else
            {
                 txtLogin.BackColor = Color.Red;
                txtSenha.BackColor = Color.Red;
                MessageBox.Show("Credenciais não reconhecidas.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
               
            }
        }


        private string ConsultaSenhaBanco(string username)
        {
            string encryptedPassword = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT SENHA FROM users WHERE USERS = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    encryptedPassword = command.ExecuteScalar()?.ToString();
                }
            }

            return encryptedPassword;
        }

        private string encripritadorDeSenha(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
                Application.Exit();
            
        }

       
    }


}


