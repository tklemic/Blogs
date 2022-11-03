using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace Blogs
{
    public partial class frmAddComment : Form
    {
        private ConnectingToDatabase db = new ConnectingToDatabase();
        private int postIDReceived;
        private int userIDLoggedInReceived;

        public frmAddComment(int idPost, int idUserLoggedIn)
        {
            InitializeComponent();
            postIDReceived = idPost;
            userIDLoggedInReceived = idUserLoggedIn;
        }
        private void frmAddComment_Load(object sender, EventArgs e)
        {
            db.OpenConnection();
        }
        //Confirm button event which calls a method to confirm an addition of the new comment
        private void btnConfirm_Click(object sender, EventArgs e) 
        {
            ConfirmComment(db.conn);
        }
        //A method which sends a query to db and creates a new comment with today's date
        private void ConfirmComment(NpgsqlConnection conn) 
        {
            if (txtContent.Text != "")
            {
                DateTime dateTime = DateTime.UtcNow.Date;
                string query = "insert into comment(content, app_user_id, post_id, date) " +
                    "VALUES('" + txtContent.Text.ToString() + "','" + userIDLoggedInReceived.ToString() + "','" + postIDReceived.ToString() + "','" + dateTime.ToString("yyyy /MM/dd") + "')";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                MessageBox.Show("Comment successfully added!", "OBAVIJEST!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Missing the content of a comment!", "Upozorenje!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        
    }
}
