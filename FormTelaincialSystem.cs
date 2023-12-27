using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormEntrada
{
    public partial class FormTelaincialSystem : Form
    {
        private Timer NovaContagem;


        private List<string> Imagens = new List<string>();
        private int indiceAtual = 0;
        private Timer timer = new Timer();
        string diretorioImagens = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Imagens");

        public FormTelaincialSystem()
        {
            InitializeComponent();

            MaximizeBox = false;

            this.FormClosing += new FormClosingEventHandler(FecharForm_FormClosing);

            // Configurar o timer
            NovaContagem = new Timer();
            NovaContagem.Interval = 30000; // .Interval define o tempo ate acionar o evento .tick que no caso e 30 segundos
            NovaContagem.Tick += usarioUnitilizando;

            // Associar os eventos de atividade do usuário
            this.MouseMove += UsarioUtilizando;
            this.KeyDown += usarioUnitilizando;


            timer.Interval = 5000;
            timer.Tick += Timer_Tick;


            timer.Start();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {

            indiceAtual = (indiceAtual + 1) % Imagens.Count;
            MostrarImagemAtual();
        }

        private void MostrarImagemAtual()
        {
            if (Imagens.Count > 0)
            {
                string caminhoDaImagemAtual = Imagens[indiceAtual];

                if (File.Exists(caminhoDaImagemAtual))
                {
                    imgVCP.Image = System.Drawing.Image.FromFile(caminhoDaImagemAtual);
                }
                else
                {
                    MessageBox.Show("O arquivo da imagem não foi encontrado.");
                }
            }
            else
            {
                MessageBox.Show("Não há imagens para exibir.");
            }
        }

        private void FormTelaincialSystem_Load(object sender, EventArgs e)
        {
            // Iniciar o timer quando o formulário for carregado
            NovaContagem.Start();



            if (Directory.Exists(diretorioImagens))
            {

                string[] arquivosDeImagem = Directory.GetFiles(diretorioImagens, "*.jpg");

                Imagens.AddRange(arquivosDeImagem);


            }
            else
            {
                MessageBox.Show("O diretório de imagens não foi encontrado.");
            }


            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void UsarioUtilizando(object sender, EventArgs e)
        {
            // Resetar o timer quando houver atividade do usuário
            NovaContagem.Stop();
            NovaContagem.Start();
        }

        private void usarioUnitilizando(object sender, EventArgs e)
        {
            // O timer estourou devido à inatividade, chamar outro formulário
            NovaContagem.Stop(); // Parar o timer

           
            Form1 abrirform = new Form1();
            abrirform.Show();
            this.Hide();
        }

        private bool fecharaAplicacao = false;

        private void btnHome_Click(object sender, EventArgs e)
        {
            Form1 abrirform = new Form1();
            abrirform.Show();
            fecharaAplicacao = true;
            this.Hide();

        
        }
        private void inventárioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NovaContagem.Stop();
            FormInventario abrirForm = new FormInventario();
            abrirForm.Show();
            this.Hide();
        }

        private void FecharForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fecharaAplicacao == false)
            {
                Application.Exit();
            }
           
        }

        private void RECEITAFEDERALToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NovaContagem.Stop();
            FormReceita abrirForm = new FormReceita();
            abrirForm.Show();
            this.Hide();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            imgVCP.SizeMode = PictureBoxSizeMode.StretchImage; // Centraliza a imagem
            imgVCP.Anchor = AnchorStyles.None; 
            imgVCP.Visible = true;
        }

        private void dADOSEVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NovaContagem.Stop();
            FormEV abrirForm = new FormEV();
            abrirForm.Show();
            this.Hide();
        }
    }
}

