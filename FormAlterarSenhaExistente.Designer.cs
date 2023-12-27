namespace FormEntrada
{
    partial class FormAlterarSenhaExistente
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAlterarSenhaExistente));
            this.txtAlterarsenhaBanco = new System.Windows.Forms.TextBox();
            this.BtnAlterarBanco = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtAlterarsenhaBanco
            // 
            this.txtAlterarsenhaBanco.Location = new System.Drawing.Point(30, 59);
            this.txtAlterarsenhaBanco.Name = "txtAlterarsenhaBanco";
            this.txtAlterarsenhaBanco.Size = new System.Drawing.Size(215, 20);
            this.txtAlterarsenhaBanco.TabIndex = 0;
            // 
            // BtnAlterarBanco
            // 
            this.BtnAlterarBanco.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BtnAlterarBanco.Location = new System.Drawing.Point(61, 117);
            this.BtnAlterarBanco.Name = "BtnAlterarBanco";
            this.BtnAlterarBanco.Size = new System.Drawing.Size(148, 37);
            this.BtnAlterarBanco.TabIndex = 1;
            this.BtnAlterarBanco.Text = "ALTERAR";
            this.BtnAlterarBanco.UseVisualStyleBackColor = false;
            this.BtnAlterarBanco.Click += new System.EventHandler(this.BtnAlterarBanco_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Modern No. 20", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(80, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "Insira a senha";
            // 
            // FormAlterarSenhaExistente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 250);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnAlterarBanco);
            this.Controls.Add(this.txtAlterarsenhaBanco);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormAlterarSenhaExistente";
            this.Text = "Alterar Senha";
            this.Load += new System.EventHandler(this.FormAlterarSenhaExistente_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAlterarsenhaBanco;
        private System.Windows.Forms.Button BtnAlterarBanco;
        private System.Windows.Forms.Label label1;
    }
}