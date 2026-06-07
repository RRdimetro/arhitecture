using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using ProductApp.Data;
using ProductApp.Models;

namespace ProductApp
{
    public partial class CategoryForm : Form
    {
        private DataGridView dgvCategories;
        private TextBox txtName;
        private Button btnAdd, btnEdit, btnDelete;

        public CategoryForm()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.dgvCategories = new DataGridView();
            this.txtName = new TextBox();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)this.dgvCategories).BeginInit();
            this.SuspendLayout();

            this.dgvCategories.Location = new System.Drawing.Point(12, 50);
            this.dgvCategories.Size = new System.Drawing.Size(450, 300);
            this.dgvCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvCategories.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.txtName.Location = new System.Drawing.Point(12, 12);
            this.txtName.Size = new System.Drawing.Size(300, 23);
            this.txtName.PlaceholderText = "Название категории";

            this.btnAdd.Location = new System.Drawing.Point(12, 360);
            this.btnAdd.Size = new System.Drawing.Size(100, 30);
            this.btnAdd.Text = "Добавить";
            this.btnAdd.Click += BtnAdd_Click;

            this.btnEdit.Location = new System.Drawing.Point(120, 360);
            this.btnEdit.Size = new System.Drawing.Size(100, 30);
            this.btnEdit.Text = "Редактировать";
            this.btnEdit.Click += BtnEdit_Click;

            this.btnDelete.Location = new System.Drawing.Point(230, 360);
            this.btnDelete.Size = new System.Drawing.Size(100, 30);
            this.btnDelete.Text = "Удалить";
            this.btnDelete.Click += BtnDelete_Click;

            this.ClientSize = new System.Drawing.Size(480, 410);
            this.Controls.Add(this.dgvCategories);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Text = "Категории";
            this.StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)this.dgvCategories).EndInit();
            this.ResumeLayout(false);
        }

        private void LoadCategories()
        {
            using var context = new AppDbContext();
            var categories = context.Categories.OrderBy(c => c.Name).ToList();
            dgvCategories.DataSource = categories;
            if (dgvCategories.Columns.Contains("Products"))
                dgvCategories.Columns["Products"].Visible = false;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название!");
                return;
            }
            using var context = new AppDbContext();
            context.Categories.Add(new Category { Name = txtName.Text });
            context.SaveChanges();
            LoadCategories();
            txtName.Clear();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0) return;
            var selected = (Category)dgvCategories.SelectedRows[0].DataBoundItem;
            if (string.IsNullOrWhiteSpace(txtName.Text)) return;
            using var context = new AppDbContext();
            var category = context.Categories.Find(selected.Id);
            if (category != null)
            {
                category.Name = txtName.Text;
                context.SaveChanges();
                LoadCategories();
                txtName.Clear();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0) return;
            var selected = (Category)dgvCategories.SelectedRows[0].DataBoundItem;
            using var context = new AppDbContext();
            var category = context.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == selected.Id);
            if (category != null && category.Products.Any())
            {
                MessageBox.Show("Нельзя удалить - есть товары!");
                return;
            }
            if (MessageBox.Show("Удалить?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                context.Categories.Remove(category!);
                context.SaveChanges();
                LoadCategories();
            }
        }
    }
}