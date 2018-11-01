namespace SOSequencing
{
    partial class frmSOSeq
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
            this.dgvSOSeq = new System.Windows.Forms.DataGridView();
            this.btnUP = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCan = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFrom = new System.Windows.Forms.DateTimePicker();
            this.txtTo = new System.Windows.Forms.DateTimePicker();
            this.btnPOP = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAddnew = new System.Windows.Forms.Button();
            this.btnLast = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrvs = new System.Windows.Forms.Button();
            this.btnFirst = new System.Windows.Forms.Button();
            this.txtDocNum = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSOSeq)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvSOSeq
            // 
            this.dgvSOSeq.AllowUserToAddRows = false;
            this.dgvSOSeq.BackgroundColor = System.Drawing.Color.White;
            this.dgvSOSeq.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSOSeq.Location = new System.Drawing.Point(12, 118);
            this.dgvSOSeq.Name = "dgvSOSeq";
            this.dgvSOSeq.RowHeadersWidth = 20;
            this.dgvSOSeq.Size = new System.Drawing.Size(1124, 435);
            this.dgvSOSeq.TabIndex = 0;
            this.dgvSOSeq.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSOSeq_CellContentClick);
            // 
            // btnUP
            // 
            this.btnUP.BackColor = System.Drawing.Color.DarkGray;
            this.btnUP.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnUP.Location = new System.Drawing.Point(1141, 279);
            this.btnUP.Name = "btnUP";
            this.btnUP.Size = new System.Drawing.Size(46, 26);
            this.btnUP.TabIndex = 1;
            this.btnUP.Text = "Up";
            this.btnUP.UseVisualStyleBackColor = false;
            this.btnUP.Click += new System.EventHandler(this.btnUP_Click);
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.Color.DarkGray;
            this.btnDown.ForeColor = System.Drawing.Color.White;
            this.btnDown.Location = new System.Drawing.Point(1141, 323);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(46, 26);
            this.btnDown.TabIndex = 2;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 574);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(87, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCan
            // 
            this.btnCan.Location = new System.Drawing.Point(120, 573);
            this.btnCan.Name = "btnCan";
            this.btnCan.Size = new System.Drawing.Size(86, 23);
            this.btnCan.TabIndex = 4;
            this.btnCan.Text = "Cancel";
            this.btnCan.UseVisualStyleBackColor = true;
            this.btnCan.Click += new System.EventHandler(this.btnCan_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "From Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(312, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "To Date";
            // 
            // txtFrom
            // 
            this.txtFrom.Location = new System.Drawing.Point(83, 70);
            this.txtFrom.Name = "txtFrom";
            this.txtFrom.Size = new System.Drawing.Size(200, 20);
            this.txtFrom.TabIndex = 9;
            // 
            // txtTo
            // 
            this.txtTo.Location = new System.Drawing.Point(364, 68);
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new System.Drawing.Size(200, 20);
            this.txtTo.TabIndex = 10;
            // 
            // btnPOP
            // 
            this.btnPOP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPOP.Location = new System.Drawing.Point(599, 67);
            this.btnPOP.Name = "btnPOP";
            this.btnPOP.Size = new System.Drawing.Size(114, 22);
            this.btnPOP.TabIndex = 11;
            this.btnPOP.Text = "Populate";
            this.btnPOP.UseVisualStyleBackColor = true;
            this.btnPOP.Click += new System.EventHandler(this.btnPOP_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAddnew);
            this.groupBox1.Controls.Add(this.btnLast);
            this.groupBox1.Controls.Add(this.btnNext);
            this.groupBox1.Controls.Add(this.btnPrvs);
            this.groupBox1.Controls.Add(this.btnFirst);
            this.groupBox1.Location = new System.Drawing.Point(-3, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1221, 47);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Navigate";
            // 
            // btnAddnew
            // 
            this.btnAddnew.AccessibleDescription = "Add New";
            this.btnAddnew.AccessibleName = "Add New";
            this.btnAddnew.Location = new System.Drawing.Point(652, 16);
            this.btnAddnew.Name = "btnAddnew";
            this.btnAddnew.Size = new System.Drawing.Size(44, 22);
            this.btnAddnew.TabIndex = 4;
            this.btnAddnew.Text = "+";
            this.btnAddnew.UseVisualStyleBackColor = true;
            this.btnAddnew.Click += new System.EventHandler(this.btnAddnew_Click);
            // 
            // btnLast
            // 
            this.btnLast.Location = new System.Drawing.Point(603, 16);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(45, 22);
            this.btnLast.TabIndex = 3;
            this.btnLast.Text = ">>";
            this.btnLast.UseVisualStyleBackColor = true;
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(554, 16);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(45, 22);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrvs
            // 
            this.btnPrvs.Location = new System.Drawing.Point(503, 16);
            this.btnPrvs.Name = "btnPrvs";
            this.btnPrvs.Size = new System.Drawing.Size(45, 22);
            this.btnPrvs.TabIndex = 1;
            this.btnPrvs.Text = "<";
            this.btnPrvs.UseVisualStyleBackColor = true;
            this.btnPrvs.Click += new System.EventHandler(this.btnPrvs_Click);
            // 
            // btnFirst
            // 
            this.btnFirst.Location = new System.Drawing.Point(450, 16);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(45, 22);
            this.btnFirst.TabIndex = 0;
            this.btnFirst.Text = "<<";
            this.btnFirst.UseVisualStyleBackColor = true;
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // txtDocNum
            // 
            this.txtDocNum.Enabled = false;
            this.txtDocNum.Location = new System.Drawing.Point(998, 67);
            this.txtDocNum.Name = "txtDocNum";
            this.txtDocNum.Size = new System.Drawing.Size(100, 20);
            this.txtDocNum.TabIndex = 13;
            this.txtDocNum.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(935, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Doc Num";
            // 
            // frmSOSeq
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1191, 607);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDocNum);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnPOP);
            this.Controls.Add(this.txtTo);
            this.Controls.Add(this.txtFrom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCan);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUP);
            this.Controls.Add(this.dgvSOSeq);
            this.MaximizeBox = false;
            this.Name = "frmSOSeq";
            this.Text = "Sale Order Sequencing";
            this.Load += new System.EventHandler(this.SOSeq_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSOSeq)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSOSeq;
        private System.Windows.Forms.Button btnUP;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker txtFrom;
        private System.Windows.Forms.DateTimePicker txtTo;
        private System.Windows.Forms.Button btnPOP;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnLast;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrvs;
        private System.Windows.Forms.Button btnFirst;
        private System.Windows.Forms.TextBox txtDocNum;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnAddnew;
    }
}

