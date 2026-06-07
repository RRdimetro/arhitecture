using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using ProductApp.Data;
using ProductApp.Models;

namespace ProductApp
{
    public partial class ProductForm : Form
    {
        private DataGridView dgvProducts;
        private TextBox txtName, txtPrice;
        private ComboBox cmbCategory;
        private Button btnAdd, btnEdit, btnDelete;

        public ProductForm()
        {
            InitializeComponent();
            LoadProducts();
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.dgvProducts = new DataGridView();
            this.txtName = new TextBox();
            this.txtPrice = new TextBox();
            this.cmbCategory = new ComboBox();
            this.btnAdd = new Button();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            ((System.ComponentModel.ISupportInitialize)this.dgvProducts).BeginInit();
            this.SuspendLayout();

            this.dgvProducts.Location = new System.Drawing.Point(12, 80);
            this.dgvProducts.Size = new System.Drawing.Size(650, 300);
            this.dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProducts.ReadOnly = true;

            this.txtName.Location = new System.Drawing.Point(12, 12);
            this.txtName.Size = new System.Drawing.Size(200, 23);
            this.txtName.PlaceholderText = "Название товара";

            this.txtPrice.Location = new System.Drawing.Point(220, 12);
            this.txtPrice.Size = new System.Drawing.Size(150, 23);
            this.txtPrice.PlaceholderText = "Цена";

            this.cmbCategory.Location = new System.Drawing.Point(380, 12);
            this.cmbCategory.Size = new System.Drawing.Size(200, 23);
            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            this.btnAdd.Location = new System.Drawing.Point(12, 390);
            this.btnAdd.Size = new System.Drawing.Size(100, 30);
            this.btnAdd.Text = "Добавить";
            this.btnAdd.Click += BtnAdd_Click;

            this.btnEdit.Location = new System.Drawing.Point(270, 390);
            this.btnEdit.Size = new System.Drawing.Size(100, 30);
            this.btnEdit.Text = "Редактировать";
            this.btnEdit.Click += BtnEdit_Click;

            this.btnDelete.Location = new System.Drawing.Point(530, 390);
            this.btnDelete.Size = new System.Drawing.Size(100, 30);
            this.btnDelete.Text = "Удалить";
            this.btnDelete.Click += BtnDelete_Click;

            this.ClientSize = new System.Drawing.Size(680, 440);
            this.Controls.Add(this.dgvProducts);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtPrice);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnDelete);
            this.Text = "Товары";
            this.StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)this.dgvProducts).EndInit();
            this.ResumeLayout(false);
        }

        private void LoadProducts()
        {
            using var context = new AppDbContext();
            var products = context.Products
                .Include(p => p.Category)
                .Select(p => new { p.Id, p.Name, Категория = p.Category!.Name, p.Price })
                .OrderBy(p => p.Name)
                .ToList();
            dgvProducts.DataSource = products;
        }

        private void LoadCategories()
        {
            using var context = new AppDbContext();
            cmbCategory.DataSource = context.Categories.OrderBy(c => c.Name).ToList();
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название!");
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену (>=0)!");
                return;
            }
            using var context = new AppDbContext();
            context.Products.Add(new Product
            {
                Name = txtName.Text,
                Price = price,
                CategoryId = (int)cmbCategory.SelectedValue
            });
            context.SaveChanges();
            LoadProducts();
            txtName.Clear();
            txtPrice.Clear();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0) return;
            int id = (int)dgvProducts.SelectedRows[0].Cells["Id"].Value;
            if (string.IsNullOrWhiteSpace(txtName.Text)) return;
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену (>=0)!");
                return;
            }
            using var context = new AppDbContext();
            var product = context.Products.Find(id);
            if (product != null)
            {
                product.Name = txtName.Text;
                product.Price = price;
                product.CategoryId = (int)cmbCategory.SelectedValue;
                context.SaveChanges();
                LoadProducts();
                txtName.Clear();
                txtPrice.Clear();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0) return;
            int id = (int)dgvProducts.SelectedRows[0].Cells["Id"].Value;
            if (MessageBox.Show("Удалить?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using var context = new AppDbContext();
                var product = context.Products.Find(id);
                if (product != null)
                {
                    context.Products.Remove(product);
                    context.SaveChanges();
                    LoadProducts();
                }
            }
        }
    }
}