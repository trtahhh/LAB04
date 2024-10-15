using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LAB04
{
    public partial class Form2 : Form
    {
        StudentContextDB db;
        public Form2()
        {
            InitializeComponent();
            db = new StudentContextDB();
        }

        private void ReturnToDefault()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                List<Faculty> listFaculty = db.Faculties.ToList();
                LoadData(listFaculty);
                textBox4.Enabled = false;
                CountProfessor();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void LoadData(List<Faculty> listFaculty)
        {
            dataGridView1.Rows.Clear();
            try
            {
                foreach (var item in listFaculty) 
                {
                    int index = dataGridView1.Rows.Add();
                    dataGridView1.Rows[index].Cells[0].Value = item.FacultyID;
                    dataGridView1.Rows[index].Cells[1].Value = item.FacultyName;
                    dataGridView1.Rows[index].Cells[2].Value = item.TotalProfessor;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int totalProfessor = int.Parse(textBox3.Text);
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }
                if (!Regex.IsMatch(textBox1.Text, @"^[a-zA-Z0-9]+$"))
                {
                    MessageBox.Show("Mã khoa không hợp lệ. Vui lòng nhập lại.");
                    return;
                }
                if (textBox2.Text.Length < 3 || textBox2.Text.Length > 100)
                {
                    MessageBox.Show("Tên khoa phải có độ dài từ 3 đến 100 ký tự.");
                    return;
                }
                if (!Regex.IsMatch(textBox2.Text, @"^[\p{L}\s]+$"))
                {
                    MessageBox.Show("Tên khoa không hợp lệ. Tên chỉ được chứa chữ cái và khoảng trắng.");
                    return;
                }
                if (totalProfessor < 0 || totalProfessor > 15)
                {
                    MessageBox.Show("Tổng số giáo sư không hợp lệ");
                    return;
                }
                var newFacult = new Faculty
                {
                    FacultyID = int.Parse(textBox1.Text),
                    FacultyName = textBox2.Text,
                    TotalProfessor = int.Parse(textBox3.Text)
                };
                db.Faculties.Add(newFacult);
                db.SaveChanges();
                LoadData(db.Faculties.ToList());
                CountProfessor();
                ReturnToDefault();
                MessageBox.Show("Thêm khoa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CountProfessor()
        {
            int totalProfessor = 0;
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[2].Value != null)
                {
                    if (int.TryParse(row.Cells[2].Value.ToString(), out int professor))
                    {
                        totalProfessor += professor;
                    }
                }
            }
            textBox4.Text = totalProfessor.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                textBox1.Text = selectedRow.Cells[0].Value.ToString();
                textBox2.Text = selectedRow.Cells[1].Value.ToString();  
                textBox3.Text = selectedRow.Cells[2].Value.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                List<Faculty> listFaculty = db.Faculties.ToList();
                var faculty = listFaculty.FirstOrDefault(p => p.FacultyID == int.Parse(textBox1.Text));
                if (faculty != null)
                {
                    if(listFaculty.Any(p => p.FacultyID == int.Parse(textBox1.Text) && p.FacultyID != faculty.FacultyID))
                    {
                        MessageBox.Show("Mã số khoa đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                
                faculty.FacultyID = int.Parse(textBox1.Text);   
                faculty.FacultyName = textBox1.Text;
                faculty.TotalProfessor = int.Parse(textBox3.Text);
                db.SaveChanges();
                LoadData(db.Faculties.ToList());
                CountProfessor();
                ReturnToDefault();
                MessageBox.Show("Chỉnh sửa thông tin khoa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi khi nhập dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                List<Faculty> listFaculty = db.Faculties.ToList();
                var faculty = listFaculty.FirstOrDefault(p => p.FacultyID == int.Parse(textBox1.Text));
                if (faculty != null)
                {
                    var noti = MessageBox.Show("Bạn có muốn xóa khoa này?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (noti == DialogResult.Yes)
                    {
                        db.Faculties.Remove(faculty);
                        db.SaveChanges();
                        LoadData(db.Faculties.ToList());
                        CountProfessor();
                        ReturnToDefault();
                        MessageBox.Show("Khoa đã được xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Khoa không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch( Exception ex )
            {
                MessageBox.Show($"Lỗi khi nhập dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
