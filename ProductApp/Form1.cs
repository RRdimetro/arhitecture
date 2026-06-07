using System;
using System.Windows.Forms;

namespace ProductApp
{
    public partial class Form1 : Form
    {
        private Button btnCategories;
        private Button btnProducts;
        private Button btnReport;

        public Form1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.btnCategories = new Button();
            this.btnProducts = new Button();
            this.btnReport = new Button();
            this.SuspendLayout();

            this.btnCategories.Location = new System.Drawing.Point(50, 40);
            this.btnCategories.Size = new System.Drawing.Size(220, 60);
            this.btnCategories.Text = "Управление категориями";
            this.btnCategories.Click += (s, e) => new CategoryForm().ShowDialog();

            this.btnProducts.Location = new System.Drawing.Point(50, 120);
            this.btnProducts.Size = new System.Drawing.Size(220, 60);
            this.btnProducts.Text = "Управление товарами";
            this.btnProducts.Click += (s, e) => new ProductForm().ShowDialog();

            this.btnReport.Location = new System.Drawing.Point(50, 200);
            this.btnReport.Size = new System.Drawing.Size(220, 60);
            this.btnReport.Text = "Отчёты";
            this.btnReport.Click += (s, e) => new ReportForm().ShowDialog();

            this.ClientSize = new System.Drawing.Size(320, 310);
            this.Controls.Add(this.btnCategories);
            this.Controls.Add(this.btnProducts);
            this.Controls.Add(this.btnReport);
            this.Text = "Главное меню";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
        }
    }
}