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
    public partial class frmComments : Form
    {
        private ConnectingToDatabase db = new ConnectingToDatabase();
        int postIDReceived;
        int idPostOwnerIDReceived;
        int userIDLoggedInReceived;
        public frmComments(int idPost, int idPostOwner, int idUserLoggedIn)
        {
            InitializeComponent();
            postIDReceived = idPost;
            idPostOwnerIDReceived = idPostOwner;
            userIDLoggedInReceived = idUserLoggedIn;
            CheckOwnerOfPost(db.conn);
        }
        private void frmComments_Load(object sender, EventArgs e)
        {
            db.OpenConnection();
            Refresh(db.conn);
        }
        //A method which enables/disables Delete comment button
        private void CheckOwnerOfPost(NpgsqlConnection conn) 
        {
            if (userIDLoggedInReceived == idPostOwnerIDReceived)
            {
                btnDelete.Enabled = true; //Enables the button if the ID of user logged in matches the ID of a user who owns a post (and comments attached to that post)
            }
            else
            {
                btnDelete.Enabled = false;
            }
        }
        //Reset display of all the comments on a certain post
        private void Refresh(NpgsqlConnection conn) 
        {
            string query = "SELECT content, name, surname, date FROM comment_v WHERE \"post_id\" = '" + postIDReceived + "' ORDER BY date DESC";

            NpgsqlDataAdapter adapterComment = new NpgsqlDataAdapter(query, conn);
            DataSet dataSetComment = new DataSet();
            adapterComment.Fill(dataSetComment);

            dgvComments.DataSource = dataSetComment.Tables[0];
        }
        //Add comment button event which opens a form called frmAddComment 
        private void btnAdd_Click(object sender, EventArgs e) 
        {
            frmAddComment frmAddComment = new frmAddComment(postIDReceived, userIDLoggedInReceived);
            frmAddComment.ShowDialog();
        }
        //Delete comment button event which calles a method for deleting a comment chosen on dgv
        private void btnDelete_Click(object sender, EventArgs e) 
        {
            DeleteComment(db.conn);
        }
        //A method which sends a query to db and deletes a comment
        private void DeleteComment(NpgsqlConnection conn) 
        {
            string query = "DELETE FROM \"comment\" WHERE id=@id";
            NpgsqlCommand command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("@id", int.Parse(dgvComments.CurrentRow.Cells[0].Value.ToString()));
            command.ExecuteNonQuery();
            MessageBox.Show("Comment successfully deleted!", "Message!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Refresh(db.conn);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh(db.conn);
        }
                
    }
}
