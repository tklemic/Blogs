using Npgsql;
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

namespace Blogs
{
    public partial class frmLoginRegistration : Form
    {
        private ConnectingToDatabase db = new ConnectingToDatabase();
        public frmLoginRegistration()
        {
            InitializeComponent();
        }
        private void frmLoginRegistration_Load(object sender, EventArgs e)
        {
            db.OpenConnection();
        }
        //A method which sends a querry to database and adds a new app user into app_user
        private void AddUser(NpgsqlConnection conn) 
        {
            string query = "insert into app_user(email, pass, name, surname, date_of_birth, role_id, request_id, status) " +
                "VALUES('" + txtEmail.Text.ToString() + "','" + txtPass.Text.ToString() + "','" + txtName.Text.ToString() + "','" + txtSurname.Text.ToString() + "','" + dTPDateOfBirth.Text.ToString() + "','" +
                 +2 + "','" + 1 + "','" + "Inactive" + "')";

            NpgsqlCommand command = new NpgsqlCommand(query, conn);
            command.ExecuteNonQuery();
        }
        //Login picture box event which calls a method that validates email
        private void pBLogin_Click(object sender, EventArgs e) 
        {
            CheckEmailForLogin(db.conn);
        }
        //Register picture box event which calls a method that validates data entered 
        private void pBRegister_Click(object sender, EventArgs e) 
        {
            CheckDataForRegistration(db.conn);
        }
        //A method called by Register button event which checks for unique email entered, pass requirements and mandatory fields entered
        private void CheckDataForRegistration(NpgsqlConnection conn) 
        {
            string query = "SELECT * FROM \"app_user\" WHERE email= @Mail ";
            NpgsqlCommand command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("@Mail", txtEmail.Text);
            command.ExecuteNonQuery();
            int count = Convert.ToInt32(command.ExecuteScalar());
            if (count > 0) //if it finds a row in app_user table, it means that the email entered is already being used and can't register another app user with the same email
            {
                MessageBox.Show("Email entered is already being used!", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (txtEmail.Text != "" && txtPass.Text != "" && txtName.Text != "" && txtSurname.Text != "")
            {
                if (true)
                {
                    var password = txtPass.Text;

                    var containsNumber = new Regex(@"[0-9]+");
                    var containstCapitalLetter = new Regex(@"[A-Z]+");
                    var containsMin6Char = new Regex(@".{6,}");

                    var passwordMatched = containsNumber.IsMatch(password) && containstCapitalLetter.IsMatch(password) && containsMin6Char.IsMatch(password);
                    if (passwordMatched) //checking if the password matches all the requirements 
                    {
                        MessageBox.Show("First step of the registration successfully done! Please wait until admin approves your registration request before login", "Message!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AddUser(db.conn);
                    }
                    else
                    {
                        MessageBox.Show("Password must contain at least 1 capital letter, 1 number and 6 or more characters in total!", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("You didn't enter all the data required!", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //A method called by Login button event which checks if the user is allowed to enter the app based on request status being approved/declined
        private void CheckEmailForLogin(NpgsqlConnection conn) 
        {
            string query = "SELECT * FROM \"app_user\" WHERE email= @Mail AND pass = @Pass ";
            NpgsqlCommand command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("@Mail", txtEmail.Text);
            command.Parameters.AddWithValue("@Pass", txtPass.Text);
            int status;
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read()) //Checks if the user's request status has been approved before login
            {
                status = Int32.Parse(reader[7].ToString());
                if (status == 1)
                {
                    MessageBox.Show("Your registration status is still being processed. Please wait!", "Message!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    reader.Close();
                }
                else if (status == 2)
                {
                    MessageBox.Show("Login successful!", "Message!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    frmMainMenu frmMainMenu = new frmMainMenu(txtEmail.Text);
                    reader.Close();
                    frmMainMenu.ShowDialog();
                }
                else if (status == 3)
                {
                    MessageBox.Show("Your registration request is denied!", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    reader.Close();
                }
            }
            else
            {
                MessageBox.Show("A user with the email and/or pass entered does not exist!", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                reader.Close();
            }
        }

        
    }
}
