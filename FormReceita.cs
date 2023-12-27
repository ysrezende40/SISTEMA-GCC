using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
    public partial class FormReceita : Form
    {
        private Timer contagem;

        public FormReceita()
        {
            InitializeComponent();
            MaximizeBox = false;
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            contagem = new Timer();
            contagem.Interval = 60000;
            contagem.Tick += Fimdetempo;



            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Visible = false; // Comece oculto
            progressBar1.Size = new Size(300, 20);
            progressBar1.Location = new Point(250, 320);

        }
        public FormReceita(object sender, EventArgs e)
        {
            contagem.Start();
        }
        private void Fimdetempo(object sender, EventArgs e)
        {
            contagem.Stop();
            FormTelaincialSystem abrirForm = new FormTelaincialSystem();
            abrirForm.Show();
            this.Hide();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            contagem.Stop();
            FormTelaincialSystem abrirForm = new FormTelaincialSystem();
            abrirForm.Show();
            this.Hide();

        }

        private async void btnBuscar_ClickAsync(object sender, EventArgs e)
        {
            DateTime dataInicial, dataFinal;

            if (!string.IsNullOrWhiteSpace(txtDtinicial.Text) && !string.IsNullOrWhiteSpace(txtDtfinal.Text))
            {
                bool dataIni = DateTime.TryParseExact(txtDtinicial.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataInicial); //verifica se as datas estão no formato correto
                bool dataFini = DateTime.TryParseExact(txtDtfinal.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataFinal);

                if (dataIni && dataFini)
                {
                    lblMensagem.Text = "Aguarde enquanto os dados são lidos...";
                    lblMensagem.Visible = true;
                    lblMensagem.Font = new Font(lblMensagem.Font.FontFamily, 18, FontStyle.Bold);
                    lblMensagem.ForeColor = ColorTranslator.FromHtml("#F5F7F8");
                    lblMensagem.BackColor = ColorTranslator.FromHtml("#83A2FF");
                    lblMensagem.Location = new Point(150, 350);
                    lblMensagem.AutoSize = false;
                    lblMensagem.Size = new Size(300, 50);

                    Size size = TextRenderer.MeasureText(lblMensagem.Text, lblMensagem.Font);
                    lblMensagem.Size = new Size(size.Width + 10, size.Height + 10); 

                    lblMensagem.Refresh();

                    progressBar1.Visible = true;


                    ConexaoReceita conexao = new ConexaoReceita(lblMensagem);
                    await conexao.RealizarConsulta(txtDtinicial.Text, txtDtfinal.Text);

                    progressBar1.Visible = false;
                    lblMensagem.Size = new Size(500, 80);
                    lblMensagem.Font = new Font(lblMensagem.Font.FontFamily, 18, FontStyle.Bold);
                    lblMensagem.Text = "Exportação concluida com sucesso";
                    txtDtinicial.Clear();
                    txtDtfinal.Clear();
                }
                else
                {
                    lblMensagem.Location = new Point(150, 350);
                    lblMensagem.Font = new Font(lblMensagem.Font.FontFamily, 18, FontStyle.Bold);
                    lblMensagem.Text = "As datas não estão no formato correto.";
                    lblMensagem.ForeColor = Color.Red;
                    lblMensagem.BackColor = Color.White;
                    return;
                }
            }
            else
            {
                lblMensagem.Location = new Point(150, 350);
                lblMensagem.Font = new Font(lblMensagem.Font.FontFamily, 18, FontStyle.Bold);
                lblMensagem.Text = "Por favor, preencha ambas as datas.";
                lblMensagem.ForeColor = Color.Red;
                lblMensagem.BackColor = Color.White;
                return;
            }
        }
        public class ConexaoReceita
        {
            public string username { get; private set; } = Environment.UserName.ToLower();

            public string ConnectionString { get; private set; } = Properties.Settings.Default.CS; //armazenei a string de conexão nas propriedades do projeto
            private OracleConnection conexaoOracle;
            private Label lblMensagem;


            public ConexaoReceita(Label Mensagem)
            {
                lblMensagem = Mensagem;
            }




            public async Task RealizarConsulta(string dtInicial, string dtFinal)
            {
              

                await Task.Run(() =>
                {
                    conexaoOracle = new OracleConnection(ConnectionString);

                    try
                    {
                        conexaoOracle.Open();

                        using (OracleCommand ComandoOracle = new OracleCommand())
                        {
                            ComandoOracle.Connection = conexaoOracle;
                            ComandoOracle.CommandTimeout = 600;

                            string dataInicialFormatada = DateTime.ParseExact(dtInicial, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 00:00:00");//formata as datas para realizar a pesquisa no banco de dados oracle
                            string dataFinalFormatada = DateTime.ParseExact(dtFinal, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd 23:59:59");


                            ComandoOracle.CommandText = @"
                                    SELECT 
                                        TRANSPORTID AS TERMO, 
                                        AWBID MAWB, 
                                        HAWBID AS HAWB, 
                                        SHIPCL AS TRATAMENTO, 
                                        PARTID AS NATUREZA, 
                                        NAME1 AS EMPRESA, 
                                        LINESTOTAL AS VOL_IF, 
                                        LINESFINISHED AS VO_REC, 
                                        ROUND(WEIGHTTOTAL/1000, 2) AS PESO_INF, 
                                        ROUND(WEIGHTFINISHED/1000, 2) AS PESO_REC,
                                        TO_CHAR(TO_DATE(DTPROCESSED, 'YYYY-MM-DD HH24:MI:SS'), 'DD/MM/YYYY HH24:MI:SS') AS DT_RECEBIMENTO
                                    FROM 
                                        ADMIN_ESTATISTICA.VW_VORDERMINBOUND
                                    WHERE
                                        TO_DATE(DTPROCESSED, 'YYYY-MM-DD HH24:MI:SS') BETWEEN TO_DATE(:DATAINICIAL, 'YYYY-MM-DD HH24:MI:SS') AND TO_DATE(:DATAFINAL, 'YYYY-MM-DD HH24:MI:SS')
                                        AND ORDERTY = 'TCI'
                                        AND (NAME1 NOT LIKE 'DESCONHECIDO' OR (NAME1 LIKE 'DESCONHECIDO' AND AWBID LIKE '892%'))
                                    GROUP BY 
                                        TRANSPORTID, 
                                        AWBID, 
                                        HAWBID, 
                                        SHIPCL, 
                                        PARTID, 
                                        NAME1, 
                                        LINESTOTAL, 
                                        LINESFINISHED, 
                                        ROUND(WEIGHTTOTAL/1000, 2), 
                                        ROUND(WEIGHTFINISHED/1000, 2),
                                        TO_CHAR(TO_DATE(DTPROCESSED, 'YYYY-MM-DD HH24:MI:SS'), 'DD/MM/YYYY HH24:MI:SS')";


                            ComandoOracle.Parameters.Add("DATAINICIAL", OracleDbType.Varchar2).Value = dataInicialFormatada;
                            ComandoOracle.Parameters.Add("DATAFINAL", OracleDbType.Varchar2).Value = dataFinalFormatada;


                            using (OracleDataReader leitorDadosOracle = ComandoOracle.ExecuteReader())
                            {
                                try
                                {


                                    MessageBox.Show("Dados encontrados com sucesso, Por gentileza espere os dados serem exportados para o excel", "SUCESSO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    ExportarParaExcel(leitorDadosOracle,dtInicial,dtFinal);
                                }
                                catch (Exception error)
                                {
                                    MessageBox.Show("Erro ao exportar para o excel" + error, "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Console.WriteLine(error);
                                }
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show("Ocorreu um erro: " + error);
                        Console.WriteLine("Ocorreu um erro: " + error);
                    }
                    finally
                    {
                        conexaoOracle.Close();
                    }

                });



            }

            private static string ConverterFormatoData(string data) // função para remover a barra porque na hora de salvar o windows não permite que o caminho seja com barra.
            {
                return data.Replace("/", "-");
            }

            private static void ExportarParaExcel(OracleDataReader reader, string dtInicial,string dtFinal)
            {

                string dataFormatadaINI = ConverterFormatoData(dtInicial);
                string dataFormatadaFINI = ConverterFormatoData(dtFinal);

                try
                {
                    using (var package = new ExcelPackage())
                    {
                        DateTime DATAPLANILHA = DateTime.Now;
                        string dataFormatada = DATAPLANILHA.ToString("dd-MM-yyyy");
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Planilha " + dataFormatada);

                        int totalColumns = reader.FieldCount;
                        int row = 1;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // Formata o cabeçalho da planilha
                            worksheet.Cells[row, i + 1].Value = reader.GetName(i);
                            worksheet.Cells[row, i + 1].Style.Font.Bold = true;  
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

                        int rowDataIndex = 2;

                        for (int i = 1; i <= totalColumns; i++)
                        {
                            worksheet.Column(i + 1).AutoFit();
                            string columnName = reader.GetName(i - 1);

                            if (columnName == "EMPRESA")
                            {
                                worksheet.Column(i).Width = 46; //aqui define o tamanho da coluna
                            }
                            if (columnName == "TERMO" || columnName == "MAWB" || columnName == "HAWB" || columnName == "DT_RECEBIMENTO")
                            {
                                worksheet.Column(i).Width = 18; 
                            }



                        }

                        while (reader.Read())
                        {
                           
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                worksheet.Cells[rowDataIndex, i + 1].Value = reader[i].ToString();
                            }
                            rowDataIndex++;
                        }

                        // Formata as células
                        for (int i = 1; i <= totalColumns; i++)
                        {
                            for (int j = 2; j <= rowDataIndex; j++)
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


                        string caminhoArquivo = $@"C:\Users\ysaac.queiroz\Documents\RELATORIO RECEITA.xlsx";



                        var tempFile = new FileInfo(caminhoArquivo);
                        package.SaveAs(tempFile);
                        MessageBox.Show("Dados exportados com sucesso", "SUCESSO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Process.Start(caminhoArquivo); // Inicia a planilha
                       
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocorreu um erro ao exportar para o Excel e abrir o arquivo: " + ex.Message);
                    MessageBox.Show("erro" + ex);
                }
            }

        }

        private void FormReceita_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
    }
}
