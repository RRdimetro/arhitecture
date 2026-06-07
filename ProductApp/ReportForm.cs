using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using ProductApp.Data;

namespace ProductApp
{
    public partial class ReportForm : Form
    {
        private DataGridView dgvReport1, dgvReport2, dgvReport3;
        private Label lblTitle1, lblTitle2, lblTitle3;

        public ReportForm()
        {
            InitializeComponent();
            LoadReports();
        }

        private void InitializeComponent()
        {
            this.dgvReport1 = new DataGridView();
            this.dgvReport2 = new DataGridView();
            this.dgvReport3 = new DataGridView();
            this.lblTitle1 = new Label();
            this.lblTitle2 = new Label();
            this.lblTitle3 = new Label();
            ((System.ComponentModel.ISupportInitialize)this.dgvReport1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvReport2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvReport3).BeginInit();
            this.SuspendLayout();

            this.lblTitle1.Location = new System.Drawing.Point(12, 10);
            this.lblTitle1.Size = new System.Drawing.Size(700, 30);
            this.lblTitle1.Text = "Раздел 1: Полный список товаров";
            this.lblTitle1.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            this.dgvReport1.Location = new System.Drawing.Point(12, 40);
            this.dgvReport1.Size = new System.Drawing.Size(750, 180);
            this.dgvReport1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReport1.ReadOnly = true;

            this.lblTitle2.Location = new System.Drawing.Point(12, 230);
            this.lblTitle2.Size = new System.Drawing.Size(700, 30);
            this.lblTitle2.Text = "Раздел 2: Количество товаров по категориям";
            this.lblTitle2.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            this.dgvReport2.Location = new System.Drawing.Point(12, 260);
            this.dgvReport2.Size = new System.Drawing.Size(750, 150);
            this.dgvReport2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReport2.ReadOnly = true;

            this.lblTitle3.Location = new System.Drawing.Point(12, 420);
            this.lblTitle3.Size = new System.Drawing.Size(700, 30);
            this.lblTitle3.Text = "Раздел 3: Средняя цена по категориям";
            this.lblTitle3.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            this.dgvReport3.Location = new System.Drawing.Point(12, 450);
            this.dgvReport3.Size = new System.Drawing.Size(750, 150);
            this.dgvReport3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReport3.ReadOnly = true;

            this.ClientSize = new System.Drawing.Size(780, 630);
            this.Controls.Add(this.lblTitle1);
            this.Controls.Add(this.dgvReport1);
            this.Controls.Add(this.lblTitle2);
            this.Controls.Add(this.dgvReport2);
            this.Controls.Add(this.lblTitle3);
            this.Controls.Add(this.dgvReport3);
            this.Text = "Отчёты";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoScroll = true;
            ((System.ComponentModel.ISupportInitialize)this.dgvReport1).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvReport2).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvReport3).EndInit();
            this.ResumeLayout(false);
        }

        private void LoadReports()
        {
            using var context = new AppDbContext();

            var report1 = context.Products
                .Include(p => p.Category)
                .Select(p => new { Название = p.Name, Категория = p.Category!.Name, Цена = p.Price })
                .OrderBy(p => p.Название)
                .ToList();
            dgvReport1.DataSource = report1;

            var report2 = context.Products
                .GroupBy(p => p.Category!.Name)
                .Select(g => new { Категория = g.Key, Количество = g.Count() })
                .OrderBy(r => r.Категория)
                .ToList();
            dgvReport2.DataSource = report2;

            var report3 = context.Products
                .GroupBy(p => p.Category!.Name)
                .Select(g => new { Категория = g.Key, СредняяЦена = Math.Round(g.Average(p => p.Price), 2) })
                .OrderByDescending(r => r.СредняяЦена)
                .ToList();
            dgvReport3.DataSource = report3;
        }
    }
}