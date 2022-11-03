using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blogs
{
    public partial class frmListOfUsers : Form
    {
        private ConnectingToDatabase db = new ConnectingToDatabase();
        private string userStatus = "";
        private int idUser = 0;
        public frmListOfUsers()
        {
            InitializeComponent();
        }
        private void frmListOfUsers_Load(object sender, EventArgs e)
        {
            db.OpenConnection();
            Refresh(db.conn);
        }
        //Reset list of users
        private void Refresh(NpgsqlConnection conn) 
        {
            string query = "SELECT * FROM app_user ORDER BY 1";

            NpgsqlDataAdapter adapterUser = new NpgsqlDataAdapter(query, conn);
            DataSet dataSetUser = new DataSet();
            adapterUser.Fill(dataSetUser);

            dgvUsers.DataSource = dataSetUser.Tables[0];

        }
        //Change status button event which calls a method to change status of a certain user
        private void btnChangeStatus_Click(object sender, EventArgs e) 
        {
            var rowChosen = dgvUsers.CurrentRow;
            idUser = int.Parse(rowChosen.Cells[0].Value.ToString());
            if (rowChosen == null || rowChosen.Index < 0)
            {
                MessageBox.Show("Please choose a user to whom you want to change status!", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (rowChosen.Cells[8].Value.ToString() == "Active") //If the user has Active status, make it Inactive
            {
                userStatus = "Inactive";
                ChangeStatus(db.conn, userStatus, idUser);
            }
            else if (rowChosen.Cells[8].Value.ToString() == "Inactive") //If the user has Inactive status, make it Active
            {
                userStatus = "Active";
                ChangeStatus(db.conn, userStatus, idUser);
            }
        }
        //Method which sends a querry to db and changes user's status
        private void ChangeStatus(NpgsqlConnection conn, string userStatus, int idUser) 
        {
            string query = "UPDATE \"app_user\" SET status = @status WHERE id=@id";
            NpgsqlCommand command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("@status", userStatus);
            command.Parameters.AddWithValue("@id", idUser);
            command.ExecuteNonQuery();
            MessageBox.Show("User status successfully changed!", "OBAVIJEST!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Refresh(db.conn);
        }
        //Exit button event which exists current form and closes a connection to db
        private void btnExit_Click(object sender, EventArgs e) 
        {
            db.CloseConnection();
            Close();
        }

        
    }
}
