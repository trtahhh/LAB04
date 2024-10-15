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
    public partial class Form1 : Form
    {
        StudentContextDB db;
        public Form1()
        {
            InitializeComponent();
            db = new StudentContextDB();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var noti = MessageBox.Show("Bạn có muốn thoát chương trình?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (noti == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void FillFacultyComboBox(List<Faculty> listFaculty)
        {
            this.comboBox1.DataSource = listFaculty;
            this.comboBox1.DisplayMember = "FacultyName";
            this.comboBox1.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dataGridView1.Rows.Clear();
            foreach (Student student in listStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = student.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = student.FullName;
                dataGridView1.Rows[index].Cells[2].Value = student.Gender;
                dataGridView1.Rows[index].Cells[3].Value = student.AverageScore;
                dataGridView1.Rows[index].Cells[4].Value = student.Faculty.FacultyName;
            }
        }

        private void CountGender()
        {
            int countMale = db.Students.Count(s => s.Gender == "Male");
            int countFemale = db.Students.Count(s => s.Gender == "Female");
            textBox4.Text = countMale.ToString();
            textBox5.Text = countFemale.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                List<Student> listStudents = db.Students.ToList();
                List<Faculty> listFaculties = db.Faculties.ToList();
                FillFacultyComboBox(listFaculties);
                BindGrid(listStudents);
                radioButton2.Checked = true;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                CountGender();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }
                if (textBox1.Text.Length != 10)
                {
                    MessageBox.Show("Mã số sinh viên phải có đúng 10 ký tự.");
                    return;
                }
                if (!Regex.IsMatch(textBox1.Text, @"^[a-zA-Z0-9]+$"))
                {
                    MessageBox.Show("Mã số sinh viên không hợp lệ. Vui lòng nhập lại.");
                    return;
                }
                if (textBox2.Text.Length < 3 || textBox2.Text.Length > 100)
                {
                    MessageBox.Show("Tên sinh viên phải có độ dài từ 3 đến 100 ký tự.");
                    return;
                }
                if (!Regex.IsMatch(textBox2.Text, @"^[\p{L}\s]+$"))
                {
                    MessageBox.Show("Tên sinh viên không hợp lệ. Tên chỉ được chứa chữ cái và khoảng trắng.");
                    return;
                }
                if (!decimal.TryParse(textBox3.Text, out decimal AverageScore))
                {
                    MessageBox.Show("Điểm trung bình sinh viên không hợp lệ. Vui lòng nhập số thập phân.");
                    return;
                }
                if (AverageScore < 0 || AverageScore > 10)
                {
                    MessageBox.Show("Điểm trung bình sinh viên phải nằm trong khoảng từ 0 đến 10.");
                    return;
                }
                List<Student> listStudent = db.Students.ToList();
                if (listStudent.Any(s => s.StudentID == textBox1.Text)) 
                {
                    MessageBox.Show("Mã số sinh viên đã tồn tại. Vui lòng nhập mã khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var newStudent = new Student
                {
                    StudentID = textBox1.Text,
                    FullName = textBox2.Text,
                    Gender = radioButton1.Checked ? "Male" : "Female",
                    AverageScore = float.Parse(textBox3.Text),
                    FacultyID = int.Parse(comboBox1.SelectedValue.ToString()),
                };
                db.Students.Add(newStudent);
                db.SaveChanges();
                BindGrid(db.Students.ToList());
                CountGender();
                returnToDefault();
                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];   
                textBox1.Text = selectedRow.Cells[0].Value.ToString();
                textBox2.Text = selectedRow.Cells[1].Value.ToString();  
                string gender = selectedRow.Cells[2].Value.ToString();
                if (gender == "Male")
                {
                    radioButton1.Checked = true;
                }
                else 
                {
                    radioButton2.Checked = true;
                }
                textBox3.Text = selectedRow.Cells[3].Value.ToString();
                comboBox1.Text = selectedRow.Cells[4].Value.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                List<Student> listStudents = db.Students.ToList();
                var student = listStudents.FirstOrDefault(s => s.StudentID == textBox1.Text);
                if (student != null)
                {
                    if (listStudents.Any(s => s.StudentID == textBox1.Text && s.StudentID != student.StudentID))
                    {
                        MessageBox.Show("Mã số sinh viên đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    student.FullName = textBox1.Text;
                    student.Gender = radioButton1.Checked ? "Male" : "Female";
                    student.FacultyID = int.Parse(comboBox1.SelectedValue.ToString());
                    student.AverageScore = float.Parse(textBox3.Text);
                    db.SaveChanges();
                    BindGrid(db.Students.ToList());
                    CountGender();
                    MessageBox.Show("Cập nhật thông tin sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    returnToDefault();
                }
                else
                {
                    MessageBox.Show("Mã số sinh viên không tìm thấy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                List<Student> listStudents = db.Students.ToList();
                var student = listStudents.FirstOrDefault(s => s.StudentID == textBox1.Text);
                if(student != null)
                {
                    db.Students.Remove(student);
                    db.SaveChanges();
                    BindGrid(db.Students.ToList());
                    CountGender();
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    returnToDefault();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void returnToDefault()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            radioButton2.Checked = true;
            comboBox1.SelectedItem = "Công nghệ thông tin";
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            Hide();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
            Hide();
        }
    }
}
