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

namespace FormEntrada
{
    public partial class FormEV : Form
    {
        public DataTable logTable;

        public FormEV()
        {
            InitializeComponent();
            InitializeLogTable();
            dataGridView1.ReadOnly = true;
        }

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            string username = Environment.UserName.ToLower();
            if(username == "cicero.chagas")
            {
                lblMensagem.Visible = true;
                lblMensagem.ForeColor = ColorTranslator.FromHtml("#39A7FF");
                lblMensagem.Location = new Point(250, 20);
                lblMensagem.BackColor = ColorTranslator.FromHtml("#BEFFF7");
                lblMensagem.AutoSize = false;
                lblMensagem.Size = new Size(300, 80);
                lblMensagem.Refresh();
                lblMensagem.Text = "Inserindo dados...";
                conexaoEv conexao = new conexaoEv(lblMensagem, logTable, UpdateDataGridView);
                await conexao.dadosEV();
            }
            else
            {
                MessageBox.Show("Você não tem permissão para isso", "INFORMAÇÃO", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
           
           
        }

        private void InitializeLogTable()
        {
            logTable = new DataTable();
            logTable.Columns.Add("Data da Operação");
            logTable.Columns.Add("Duração da Operação");
            logTable.Columns.Add("Linhas Inseridas");
            logTable.Columns.Add("Horário de Término");
        }

        private void UpdateDataGridView()
        {
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke((MethodInvoker)delegate
                {
                    dataGridView1.DataSource = logTable;
                    dataGridView1.Refresh();
                    ResizeDataGridViewColumns();
                });
            }
            else
            {
                dataGridView1.DataSource = logTable;
                dataGridView1.Refresh();
                ResizeDataGridViewColumns();
            }
        }
        private void ResizeDataGridViewColumns()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[dataGridView1.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            FormTelaincialSystem abrirForm = new FormTelaincialSystem();
            abrirForm.Show();
            this.Hide();
        }
    }

    class conexaoEv
    {
        private Label lblMensagem;
        private DataTable logTable;
        private Action UpdateDataGridView;



        public conexaoEv(Label lblMenrec, DataTable table, Action updateFunction)
        {
            lblMensagem = lblMenrec;
            logTable = table;
            UpdateDataGridView = updateFunction;
        }

        public string ConnectionString { get; private set; } = Properties.Settings.Default.CS;
       

        public async Task dadosEV()
        {
            int linhasInseridas = 0;




            DateTime inicioOperacao = DateTime.Now;
            await Task.Run(() =>
            {
               
                try
                {
                   

                    using (OracleConnection conexaoOracle = new OracleConnection(ConnectionString))
                    {
                        try
                        {
                            conexaoOracle.Open();

                            string query = @"
                                INSERT INTO EV_DOC_LIB (
                                    NR_DOC,
                                    CD_DECLARACAO,
                                    DT_DOCUMENTO,
                                    DT_ATRACACAO,
                                    DT_REGISTRO,
                                    DT_DESEMBARACO,
                                    DT_RECEPCAO,
                                    TP_DOCUMENTO,
                                    NUM_DOCUMENTO,
                                    TIPO_DOC,
                                    TP_MANIFESTO,
                                    QTD_ADICOES,
                                    RECINTO,
                                    NR_IMPORTADOR,
                                    NOME_IMPORTADOR,
                                    TP_IMPORTADOR,
                                    PAIS_ORIGEM,
                                    NOME_PAIS_ORIGEM,
                                    PB_CARGA,
                                    PL_CARGA,
                                    FRETE_USD,
                                    SEGURO_USD,
                                    VAL_DOCUMENTO
                                )
                                SELECT DISTINCT
                                    doc.num_documento                  AS ""NR_DOC"",
                                    imp.cd_tipo_declaracao             AS ""CD_DECLARACAO"",
                                    doc.dat_documento                  AS ""DT_DOCUMENTO"",
                                    doc.dat_atracacao                  AS ""DT_ATRACACAO"",
                                    imp.dt_registro_di                 AS ""DT_REGISTRO"",
                                    dli.d_data_desembaraco             AS ""DT_DESEMBARACO"",
                                    imp.dt_recepcao                    AS ""DT_RECEPCAO"",
                                    doc.tip_documento                  AS ""TP_DOCUMENTO"",
                                    docb.num_documento                 AS ""NUM_DOCUMENTO"",
                                    docb.tip_documento                 AS ""TIPO_DOC"",
                                    imp.cd_tipo_manifesto              AS ""TP_MANIFESTO"",
                                    imp.qtd_adicao                     AS ""QTD_ADICOES"",
                                    imp.s_cod_recinto_alfandegado      AS ""RECINTO"",
                                    imp.nr_importador                  AS ""NR_IMPORTADOR"",
                                    prc.nom_razao_social               AS ""NOME_IMPORTADOR"",
                                    prc.idc_pessoa                     AS ""TP_IMPORTADOR"",
                                    imp.cd_pais_proc_carga             AS ""PAIS_ORIGEM"",
                                    pais.nom_pais                      AS ""NOME_PAIS_ORIGEM"",
                                    imp.pb_carga                       AS ""PB_CARGA"",
                                    imp.pl_carga                       AS ""PL_CARGA"",
                                    imp.vl_tot_frete_dolar             AS ""FRETE_USD"",
                                    imp.vl_total_seg_dolar             AS ""SEGURO_USD"",
                                    doc.val_documento                  AS ""VAL_DOCUMENTO""
                                FROM sfwevvcp.sfw_documento@EVSYS_PRKP02 doc,
                                     sfwevvcp.sfw_doc_importacao@EVSYS_PRKP02 imp,
                                     sfwevvcp.sfw_parceiro@EVSYS_PRKP02 prc,
                                     sfwevvcp.zp_documento_liberacao@EVSYS_PRKP02 dli,
                                     sfwevvcp.sfw_documento_baixa@EVSYS_PRKP02 db,
                                     sfwevvcp.sfw_documento@EVSYS_PRKP02 docb,
                                     sfwevvcp.sfw_pais@EVSYS_PRKP02 pais
                                WHERE doc.ide_documento = imp.ide_documento
                                  AND doc.ide_parceiro = prc.ide_parceiro
                                  AND imp.cd_pais_proc_carga = pais.cod_pais
                                  AND doc.ide_documento = dli.n_ide_documento
                                  AND doc.ide_documento = db.ide_documento
                                  AND db.ide_documento_baixa = docb.ide_documento
                                  AND doc.tip_documento = '10 - DI'
                                  AND doc.dat_atracacao >= ADD_MONTHS(TRUNC(SYSDATE), -12)";



                            using (OracleCommand comandoEv = new OracleCommand(query, conexaoOracle))
                            {
                                
                                


                                DateTime terminoOperacao = DateTime.Now;
                                TimeSpan duracaoOperacao = terminoOperacao - inicioOperacao;

                               
                                
                                    try
                                    {
                                       
                                        int linhasAfetadas = comandoEv.ExecuteNonQuery();

                                        if (linhasAfetadas > 0)
                                        {
                                            linhasInseridas++;
                                            AtualizarLabel("começou o processo");


                                            AtualizarLabel("dados inseridos com sucesso");
                                            MessageBox.Show("dados inseridos com sucesso");


                                            RegistrarLog($"Operação realizada no dia: {DateTime.Now} segundos");
                                            RegistrarLog($"Operação realizada em: {duracaoOperacao.TotalSeconds} segundos");
                                            RegistrarLog($"Linhas inseridas: {linhasAfetadas}");
                                            RegistrarLog($"Horário de término: {terminoOperacao}");
                                            RegistrarLog("-----------------------------------------------------------------");



                                                    DataRow newRow = logTable.NewRow();
                                                    newRow["Data da Operação"] = DateTime.Now.ToString();
                                                    newRow["Duração da Operação"] = duracaoOperacao;
                                                    newRow["Linhas Inseridas"] = linhasAfetadas;
                                                    newRow["Horário de Término"] = terminoOperacao;
                                                    logTable.Rows.Add(newRow);


                                                    

                                    }
                                    }
                                    catch (OracleException ex)
                                    {
                                       
                                        Console.WriteLine($"Erro durante a inserção: {ex.Message}");
                                       
                                    }

                                   
                                

                            }
                        }
                        catch (OracleException ex)
                        {
                            Console.WriteLine($"Erro Oracle: {ex.Message}");
                        }
                        finally
                        {
                            conexaoOracle.Close();
                        }
                    }
                }
                catch(Exception erro)
                {
                    MessageBox.Show("erro com a conexão" + erro);
                }
               


            });



            UpdateDataGridView();

        }
        public void AtualizarLabel(string mensagem)
        {
            if (lblMensagem.InvokeRequired)
            {
                lblMensagem.Invoke((MethodInvoker)delegate
                {
                    lblMensagem.Text = mensagem;
                });
            }
            else
            {
                lblMensagem.Text = mensagem;
            }
        }

        private void RegistrarLog(string mensagem)
        {
            string path = "G:\\Coord_Estatistica_de_Carga\\001_Desenvolvimentos\\Ysaac\\RegistrosEv\\registrosEV";


            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
                {
                    file.WriteLine(mensagem);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao registrar log: " + ex.Message);
            }
        }


        


    }
}
