using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LAB04
{
    public partial class Form3 : Form
    {
        StudentContextDB db;
        public Form3()
        {
            InitializeComponent();
            db = new StudentContextDB();
        }

        private void FillFacultyComboBox(List<Faculty> listFaculty)
        {
            this.comboBox1.DataSource = listFaculty;
            this.comboBox1.DisplayMember = "FacultyName";
            this.comboBox1.ValueMember = "FacultyID";
        }

        private void LoadData(List<Student> listStudents)
        {
            dataGridView1.Rows.Clear();
            foreach (Student student in listStudents)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = student.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = student.FullName;
                dataGridView1.Rows[index].Cells[2].Value = student.Gender;
                dataGridView1.Rows[index].Cells[3].Value = student.AverageScore;
                dataGridView1.Rows[index].Cells[4].Value = student.Faculty.FacultyName;
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            try
            {
                radioButton2.Checked = true;   
                textBox3.Enabled = false;
                List<Student> listStudents = db.Students.ToList();
                List<Faculty> listFaculty = db.Faculties.ToList();
                LoadData(listStudents); 
                FillFacultyComboBox(listFaculty);
                button2.Enabled = false;
                textBox3.Text = "0";
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string maSV = textBox1.Text.ToLower();
                string hoTen = textBox2.Text.ToLower();
                var result = db.Students.Where(sv =>
                    sv.StudentID.ToLower().Contains(maSV) &&
                    sv.FullName.ToLower().Contains(hoTen) 
                ).ToList();
                if (result.Any())
                {
                    LoadData(result);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào thỏa mãn tiêu chí tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                textBox3.Text = result.Count().ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
