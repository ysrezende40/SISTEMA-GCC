using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using Color = System.Drawing.Color;

namespace FormEntrada
{
    public partial class FormInventario : Form
    {

        private List<string> Imagens = new List<string>();
        private int indiceAtual = 0;
        private Timer timer = new Timer();
        string diretorioImagens = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Imagens");





        public FormInventario()
        {
            InitializeComponent();

            MaximizeBox = false;
            this.FormClosing += new FormClosingEventHandler(FecharForm_FormClosing);

            //Define o progress seu estilo tamanho e localização.
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Visible = false; // Começa oculto
            progressBar1.Size = new Size(300, 20);
            progressBar1.Location = new Point(250, 320);


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; //licença para usar a bliblioteca do excel

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
                    pictureBox1.Image = System.Drawing.Image.FromFile(caminhoDaImagemAtual);
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

        private void FormInventario_Load(object sender, EventArgs e)
        {
            


            if (Directory.Exists(diretorioImagens))
            {

                string[] arquivosDeImagem = Directory.GetFiles(diretorioImagens, "*.gif");

                Imagens.AddRange(arquivosDeImagem);


            }
            else
            {
                MessageBox.Show("O diretório de imagens não foi encontrado.");
            }


            this.FormBorderStyle = FormBorderStyle.FixedSingle;


        }


        //$                       //$
        private async void btnBuscar_ClickAsync(object sender, EventArgs e)
        {
            if(txtNomeDaEmpresa.Text == "")
            {
                lblMensagem.Location = new Point(150, 350);
                lblMensagem.ForeColor = Color.Red;
                lblMensagem.Font = new Font(lblMensagem.Font.FontFamily, 14, FontStyle.Bold);
                lblMensagem.Text = "Por favor, preencha o nome da empresa";
            }
            else
            {
                //mesma coisa para a label 
                lblMensagem.Text = "Aguarde enquanto os dados são lidos...";
                lblMensagem.Visible = true;
                lblMensagem.ForeColor = ColorTranslator.FromHtml("#1A5D1A");
                lblMensagem.Location = new Point(250, 350);
                lblMensagem.AutoSize = false;
                lblMensagem.Size = new Size(300, 50);
                lblMensagem.Refresh();

                progressBar1.Visible = true;

                Conexao conexao = new Conexao(lblMensagem); //passa a label para alteração na classe conexão

                //$ AQUI E NECESSARIO COLOCAR AWAIT ANTES DE CHAMARMOS A FUNÇÃO ASSICRONA QUE É inserirdados
                await conexao.inserirdados(txtNomeDaEmpresa.Text);

                lblMensagem.Text = "Aguarde enquanto os dados são Exportados Para o Excel...";

                conexao.RealizarConsulta(txtNomeDaEmpresa.Text);


                //faz o progress desaparecer
                progressBar1.Visible = false;
            }
            
        }
        
        class Conexao
        {
            public string username { get; private set; } = Environment.UserName.ToLower();  //pega o nome do usario da maquina

            public string ConnectionString { get; private set; } = Properties.Settings.Default.CS; //armazenei a string de conexão nas propriedades do projeto
            private OracleConnection conexaoOracle;
            private Label lblMensagem;
            bool semdados = false;

            public Conexao(Label Mensagem)
            {
                lblMensagem = Mensagem;
            }
            

            public void ExportarParaExcel(OracleDataReader reader, string nomerec)
            {
                string nomeDaEmpresa = nomerec;
                try
                {
                    using (var package = new ExcelPackage())
                    {
                        DateTime DATAPLANILHA = DateTime.Now;
                        string dataFormatada = DATAPLANILHA.ToString("dd-MM-yyyy");
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Planilha "+ dataFormatada);

                        int totalColumns = reader.FieldCount;
                        int row = 1;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                           //formata o cabeçalho da planilha
                            worksheet.Cells[row, i + 1].Value = reader.GetName(i);
                            worksheet.Cells[row, i + 1].Style.Font.Bold = true;  // Fonte em negrito
                            worksheet.Cells[row, i + 1].Style.Font.Color.SetColor(Color.Black);
                            worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                            worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[row, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Cells[row, i + 1].Style.Font.Italic = true;

                            worksheet.Cells[row, i + 1].Style.Border.Top.Style = ExcelBorderStyle.Thick;
                            worksheet.Cells[row, i + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                            worksheet.Cells[row, i + 1].Style.Border.Left.Style = ExcelBorderStyle.Thick;
                            worksheet.Cells[row, i + 1].Style.Border.Right.Style = ExcelBorderStyle.Thick;
                            worksheet.Column(i + 1).AutoFit();
                           
                        }
                        
                        for (int i = 1; i <= totalColumns; i++)
                        {
                            string columnName = reader.GetName(i - 1);

                            //definindo tamanho para colonas especificas
                            if (columnName == "AREA" || columnName == "TC")
                            {
                                worksheet.Column(i).Width = 8;
                            }
                            if(columnName == "TERMO")
                            {
                                worksheet.Column(i).Width = 10;
                            }
                            if (columnName == "MASTER" || columnName =="HOUSE")
                            {
                                worksheet.Column(i).Width = 15;
                            }
                            if (columnName == "NOME")
                            {
                                worksheet.Column(i).Width = 50;
                            }
                            if (columnName == "NR_PROC")
                            {
                                worksheet.Column(i).Width = 22;
                            }
                            if (columnName == "DT_CHEGADA" || columnName == "DT_RECEBIMENTO")
                            {
                                worksheet.Column(i).Width = 19;
                            }

                        }

                        // Crie uma lista para armazenar os dados
                        List<string[]> data = new List<string[]>();

                        while (reader.Read())
                        {
                            string[] rowData = new string[reader.FieldCount];
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                string cellValue = reader[i].ToString();

                                if (columnName == "DT_RECEBIMENTO")
                                {
                                    if (DateTime.TryParse(cellValue, out DateTime dtValue))
                                    {
                                        cellValue = dtValue.ToString("dd-MM-yyyy HH:mm:ss"); //altera o formato da data
                                    }
                                   
                                }
                                else if (columnName == "DT_CHEGADA")
                                {
                                    if (DateTime.TryParseExact(cellValue, "MM-dd-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtValue))
                                    {
                                        cellValue = dtValue.ToString("dd-MM-yyyy HH:mm:ss");
                                    }
                                  
                                }

                                if (columnName == "AREA")
                                {
                                    // Remove os prefixos "W" e "AWS" da coluna "AREA"
                                    cellValue = cellValue.Replace("-W", "").Replace("-AWS", "").Replace("-MS", "").Replace("MS", "");
                                }

                                rowData[i] = cellValue;

                            }

                            //adiciona o vetor rowData a lista data;
                            data.Add(rowData);
                        }

                        // Ordene os dados com base na coluna escolhida que nesse caso eu escolhi a 0
                        data = data.OrderBy(rowData => rowData[0]).ToList();


                        // Escreva os dados ordenados na planilha
                        row++;
                        foreach (var rowData in data)
                        {
                            for (int i = 0; i < rowData.Length; i++)
                            {
                                worksheet.Cells[row, i + 1].Value = rowData[i];
                            }
                            row++;
                        }

                        Console.WriteLine($"Número de registros lidos do banco de dados: {data.Count}");

                        Console.WriteLine($"Número de registros exportados para o Excel: {data.Count}");

                        //formata as celulas
                        for (int i = 1; i <= totalColumns; i++)
                        {
                            for (int j = 2; j <= data.Count + 1; j++)
                            {
                                var dataCellStyle = worksheet.Cells[j, i].Style;
                                dataCellStyle.Font.Color.SetColor(Color.Black);
                                dataCellStyle.Fill.PatternType = ExcelFillStyle.Solid;
                                dataCellStyle.Fill.BackgroundColor.SetColor(Color.White);
                                dataCellStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                dataCellStyle.VerticalAlignment = ExcelVerticalAlignment.Center;
                                dataCellStyle.Border.Top.Style = ExcelBorderStyle.Thin;
                                dataCellStyle.Border.Bottom.Style = ExcelBorderStyle.Thin;
                                dataCellStyle.Border.Left.Style = ExcelBorderStyle.Thin;
                                dataCellStyle.Border.Right.Style = ExcelBorderStyle.Thin;

                            }
                        }


                        string nomePersonalizado = "Planilha_Inventário";
                        string extension = ".xlsx";

                        string tempFilePath = Path.Combine(Path.GetTempPath(), $"{nomePersonalizado}_{nomeDaEmpresa}_{dataFormatada}{extension}");

                        var tempFile = new FileInfo(tempFilePath);
                        package.SaveAs(tempFile);
                        Process.Start(tempFilePath); //inicia a planilha
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocorreu um erro ao exportar para o Excel e abrir o arquivo: " + ex.Message);
                }
            }



            //$
            public async Task inserirdados(string SqlCriterio)
            {

                await Task.Run(() => // AQUI COLOCAMOS A CONSULTA EM UMA FUNÇÃO ASSICRONA PARA ASSIM NÃO TRAVAR A INTERFACE DO USARIO, PARA REALIZAR ISSO PRECISAMOS ALTERAR DIVERSOS PONTOS DO NOSSO CODIGO 
                //QUE SERÃO MARCADOS COM UM $ PARA INDENTIFICAÇÃO 
                {
                    conexaoOracle = new OracleConnection(ConnectionString);


                    
                    try
                    {
                        conexaoOracle.Open();

                        using (OracleCommand comandoLimparTabela = new OracleCommand())
                        {
                            comandoLimparTabela.Connection = conexaoOracle;
                            comandoLimparTabela.CommandText = "TRUNCATE TABLE tbl_INVENTARIO"; //apaga todos os dados da tabela antes de inserir novos
                            comandoLimparTabela.ExecuteNonQuery();
                        }


                        using (OracleCommand comandoOracle = new OracleCommand())
                        {
                            comandoOracle.Connection = conexaoOracle;
                            comandoOracle.CommandTimeout = 6000;

                            comandoOracle.CommandText = @"
                                        SELECT DISTINCT
                                            SUBSTR(TRIM(V.SAID), 0, 2) AS ""AREA"",
                                            V.AWBID AS ""MASTER"",
                                            V.HAWBID AS ""HOUSE"",
                                            V.TRANSPORTID AS ""TERMO"",
                                            V.DIDTA AS ""NUM_DOC"",
                                            V.SHIPCL AS ""TC"",
                                            SUM(V.GRWGACT) / 1000 AS ""PESO"",
                                            SUM(V.QINV) AS ""VOL"",
                                            V.NAME1 AS ""NOME"",
                                            V.PROCESSNO AS ""NR_PROC"",
                                            I.DTARRIVAL AS ""DT_CHEGADA"",
                                            MIN(I.DTPROCESSED) AS ""DT_RECEBIMENTO""
                                        FROM ADMIN_ESTATISTICA.VW_VINVPARTM V
                                        INNER JOIN ADMIN_ESTATISTICA.VW_VORDERMINBOUND I 
                                            ON (V.AWBID = I.AWBID
                                                AND V.HAWBID = I.HAWBID
                                                AND V.TRANSPORTID = I.TRANSPORTID)
                                        WHERE UPPER(V.NAME1) LIKE '%' || UPPER(:nome_transportador) || '%'
                                            AND V.TRTY = 'TCI'
                                            AND V.INVUSAGE IN ('PICKINV', 'INV')
                                            AND V.SAID <> '0180'
                                            AND V.SAID <> '0188'
                                        GROUP BY 
                                            SUBSTR(TRIM(V.SAID), 0, 2),
                                            V.AWBID,
                                            V.HAWBID,
                                            V.TRANSPORTID, 
                                            V.DIDTA,
                                            V.SHIPCL,
                                            V.NAME1,
                                            V.PROCESSNO,
                                            I.DTARRIVAL,
                                            I.DTPROCESSED
                                        ORDER BY I.DTARRIVAL";



                            comandoOracle.Parameters.Add("nome_transportador", OracleDbType.Varchar2).Value = SqlCriterio;

                            using (OracleDataReader leitorDadosOracle = comandoOracle.ExecuteReader())
                            {
                                if (leitorDadosOracle.HasRows)
                                {

                                    MessageBox.Show("Dados encontrados,por gentileza espere os dados serem exportados para o Excel", "SUCESSO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    MessageBox.Show("Antes de realizar uma nova consulta fecha o arquivo excel da consulta anterior!", "INFORMAÇÃO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    try
                                    {

                                        int linhasInseridasTotal = 0;
                                        while (leitorDadosOracle.Read())
                                        {
                                            string area = leitorDadosOracle["AREA"].ToString();
                                            string master = leitorDadosOracle["MASTER"].ToString();
                                            string house = leitorDadosOracle["HOUSE"].ToString();
                                            string termo = leitorDadosOracle["TERMO"].ToString();
                                            string num_doc = leitorDadosOracle["NUM_DOC"].ToString();
                                            string tc = leitorDadosOracle["TC"].ToString();
                                            string peso = leitorDadosOracle["PESO"].ToString();
                                            string volume = leitorDadosOracle["VOL"].ToString();
                                            string nome = leitorDadosOracle["NOME"].ToString();
                                            string nr_proc = leitorDadosOracle["NR_PROC"].ToString();
                                            string dt_chegada = leitorDadosOracle["DT_CHEGADA"].ToString();
                                            string dt_recebimento = leitorDadosOracle["DT_RECEBIMENTO"].ToString();

                                            using (OracleCommand comandoinserir = new OracleCommand())
                                            {

                                                comandoinserir.Connection = conexaoOracle;
                                                comandoinserir.CommandText = @"
                                                INSERT INTO tbl_INVENTARIO (AREA, MASTER, HOUSE, TERMO, NUM_DOC, TC, PESO, VOL, NOME, NR_PROC, DT_CHEGADA, DT_RECEBIMENTO)
                                                VALUES (:AREA, :MASTER, :HOUSE, :TERMO, :NUM_DOC, :TC, :PESO, :VOL, :NOME, :NR_PROC, :DT_CHEGADA, :DT_RECEBIMENTO)";

                                                comandoinserir.Parameters.Add("AREA", OracleDbType.Varchar2, 4).Value = area;
                                                comandoinserir.Parameters.Add("MASTER", OracleDbType.Varchar2, 20).Value = master;
                                                comandoinserir.Parameters.Add("HOUSE", OracleDbType.Varchar2, 30).Value = house;
                                                comandoinserir.Parameters.Add("TERMO", OracleDbType.Varchar2, 11).Value = termo;
                                                comandoinserir.Parameters.Add("NUM_DOC", OracleDbType.Varchar2, 50).Value = num_doc;
                                                comandoinserir.Parameters.Add("TC", OracleDbType.Varchar2, 4).Value = tc;
                                                comandoinserir.Parameters.Add("PESO", OracleDbType.Decimal).Value = decimal.Parse(peso.Replace(',', '.'), CultureInfo.InvariantCulture);
                                                comandoinserir.Parameters.Add("VOL", OracleDbType.Decimal).Value = decimal.Parse(volume.Replace(',', '.'), CultureInfo.InvariantCulture);
                                                comandoinserir.Parameters.Add("NOME", OracleDbType.Varchar2, 100).Value = nome;
                                                comandoinserir.Parameters.Add("NR_PROC", OracleDbType.Varchar2, 24).Value = nr_proc;
                                                comandoinserir.Parameters.Add("DT_CHEGADA", OracleDbType.Varchar2, 19).Value = dt_chegada;
                                                comandoinserir.Parameters.Add("DT_RECEBIMENTO", OracleDbType.Varchar2, 19).Value = dt_recebimento;

                                                comandoinserir.ExecuteNonQuery();

                                                int linhasInseridas = comandoinserir.ExecuteNonQuery();
                                                linhasInseridasTotal += linhasInseridas;

                                            }

                                        }
                                        Console.WriteLine($"Número total de linhas inseridas: {linhasInseridasTotal}");

                                    }
                                    catch (Exception erro)
                                    {
                                        MessageBox.Show("ocorreu um erro" + erro);
                                        Console.WriteLine("ocorreu um erro" + erro);
                                    }

                                }
                                else
                                {
                                    MessageBox.Show("Não foram encontrados dados para a consulta.","INFORMAÇÃO",MessageBoxButtons.OK,MessageBoxIcon.Information);


                                    semdados = true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ocorreu um erro: " + ex.Message);
                    }

                    finally
                    {
                        conexaoOracle.Close();


                    }

                }
                );


            }
            public void RealizarConsulta(string SqlCriterio)
            {
                conexaoOracle = new OracleConnection(ConnectionString);
                string nome = SqlCriterio;

               

                using (OracleCommand comandoOracle = new OracleCommand())
                {
                    try
                    {
                        conexaoOracle.Open();

                        comandoOracle.Connection = conexaoOracle;
                        comandoOracle.CommandTimeout = 6000;

                        comandoOracle.CommandText = @"
                                SELECT ""AREA"", ""MASTER"", ""HOUSE"", ""TERMO"", ""NUM_DOC"", ""TC"", ""PESO"", ""VOL"", ""NOME"", ""NR_PROC"", ""DT_CHEGADA"", ""DT_RECEBIMENTO""
                                FROM (
                                    SELECT ""AREA"", ""MASTER"", ""HOUSE"", ""TERMO"", ""NUM_DOC"", ""TC"", ""PESO"", ""VOL"", ""NOME"", ""NR_PROC"", ""DT_CHEGADA"", ""DT_RECEBIMENTO"",
                                           ROW_NUMBER() OVER (PARTITION BY ""HOUSE"" ORDER BY ""ROWID"") AS rn
                                    FROM tbl_inventario
                                    WHERE NOME LIKE '%' || UPPER(:nome_transportador) || '%'
                                ) subquery
                                WHERE rn = 1";
                        //a consulta e estruturada para evitar retornar dados repetidos



                        comandoOracle.Parameters.Add("nome_transportador", OracleDbType.Varchar2).Value = SqlCriterio;
                    
                        using (OracleDataReader leitorDadosOracle = comandoOracle.ExecuteReader())
                        {
                            try
                            {
                                if (semdados == false)
                                {
                                    ExportarParaExcel(leitorDadosOracle, nome);
                                    lblMensagem.Text = "EXPORTAÇÃO CONCLUIDA COM SUCESSO";
                                    MessageBox.Show("Exportação concluida com sucesso", "SUCESSO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    lblMensagem.Text = $"A empresa {SqlCriterio} não retornou dados";
                                }
                               
                            }
                            catch (Exception error)
                            {
                                MessageBox.Show("Erro ao exportar para o excel" + error, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Console.WriteLine(error);
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("Erro ao conectar a tabela de inventário " + error, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Console.WriteLine(error);
                    }
                    finally
                    {
                        conexaoOracle.Close();
                    }
                }
            }

        }

        private bool fecharaAplicacao = false;


        private void btnHome_Click(object sender, EventArgs e)
        {
            FormTelaincialSystem abrirForm = new FormTelaincialSystem();
            abrirForm.Show();
            this.Hide();

            fecharaAplicacao = true;
        }

        private void FecharForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (fecharaAplicacao == false)
            {
               
                Application.Exit();
            }
        }

       
    }
}



