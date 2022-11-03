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
    public partial class frmRequests : Form
    {
        private ConnectingToDatabase db = new ConnectingToDatabase();
        private int request = 0; //variable used to differentiate wheter the registration request has been approved or decline
        private string email = "";
        public frmRequests()
        {
            InitializeComponent();
        }
        private void frmRequests_Load(object sender, EventArgs e)
        {
            db.OpenConnection();
            Refresh(db.conn);
        }
        //Resets display of all the requests that are still "In progress"
        private void Refresh(NpgsqlConnection conn) 
        {
            string querry = "SELECT * FROM app_user_v WHERE request LIKE '%In progress%'";

            NpgsqlDataAdapter adapterRequest = new NpgsqlDataAdapter(querry, conn);
            DataSet dataSetRequest = new DataSet();
            adapterRequest.Fill(dataSetRequest);

            dgvRequests.DataSource = dataSetRequest.Tables[0];

        }
        //Accept button event which calls a method that gets mail from dgv with the request set to 2(accept)
        private void btnAccept_Click(object sender, EventArgs e) 
        {
            request = 2;
            GetMail(request);
        }
        //Decline button event which calls a method that gets mail from dgv with the request set to 3(decline)
        private void btnDecline_Click(object sender, EventArgs e) 
        {
            request = 3;
            GetMail(request);
        }
        //A method which gets an email from the dgv and calls another method for Accepting/Declining request if the user press Yes on the pop up box
        private void GetMail(int request) 
        {
            var rowChosen = dgvRequests.CurrentRow;
            email = rowChosen.Cells[0].Value.ToString();
            if (rowChosen == null || rowChosen.Index < 0)
            {
                MessageBox.Show("Please choose a request to be accepted/declined!", "WARNING!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var confirmDecision = MessageBox.Show("Are you sure that you want to accept the request?",
                                     "Accept the request!",
                                     MessageBoxButtons.YesNo);
                if (confirmDecision == DialogResult.Yes)
                {
                    AcceptDeclineRequest(db.conn, request, email);
                }
                else
                {
                    return;
                }
            }
        }
        //A method which sends a querry to the database and accepts/denies a request
        private void AcceptDeclineRequest(NpgsqlConnection conn, int request, string mail) 
        {
            string querry = "UPDATE \"app_user\" SET request_id = @request WHERE email=@mail";
            NpgsqlCommand command = new NpgsqlCommand(querry, conn);
            command.Parameters.AddWithValue("@request", request);
            command.Parameters.AddWithValue("@mail", mail);
            command.ExecuteNonQuery();
            Refresh(db.conn);
        }        
    }
}
