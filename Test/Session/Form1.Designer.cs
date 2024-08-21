namespace TestSession
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            dataGridView1 = new DataGridView();
            button2 = new Button();
            button1 = new Button();
            button3 = new Button();
            button4 = new Button();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(150, 25);
            label1.Name = "label1";
            label1.Size = new Size(44, 17);
            label1.TabIndex = 7;
            label1.Text = "未加载";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(29, 196);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(708, 181);
            dataGridView1.TabIndex = 6;
            // 
            // button2
            // 
            button2.Location = new Point(29, 81);
            button2.Name = "button2";
            button2.Size = new Size(121, 24);
            button2.TabIndex = 5;
            button2.Text = "初始化Session";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(29, 21);
            button1.Name = "button1";
            button1.Size = new Size(106, 24);
            button1.TabIndex = 4;
            button1.Text = "加载驱动和数据源";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button3
            // 
            button3.Location = new Point(27, 142);
            button3.Name = "button3";
            button3.Size = new Size(82, 24);
            button3.TabIndex = 8;
            button3.Text = "查询数据1";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(138, 142);
            button4.Name = "button4";
            button4.Size = new Size(82, 24);
            button4.TabIndex = 9;
            button4.Text = "查询数据2";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(167, 85);
            label2.Name = "label2";
            label2.Size = new Size(44, 17);
            label2.TabIndex = 10;
            label2.Text = "未加载";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 420);
            Controls.Add(label2);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(label1);
            Controls.Add(dataGridView1);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private DataGridView dataGridView1;
        private Button button2;
        private Button button1;
        private Button button3;
        private Button button4;
        private Label label2;
    }
}