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
    public partial class frmMainMenu : Form
    {
        public string emailLoggedIn;
        private int keyOwner = 0; //varible used to differentiate which button was clicked (New post or edit post)
        private int idPost = 0;
        private int idPostOwner = 0;
        private int idUserLoggedIn = 0;
        private ConnectingToDatabase db = new ConnectingToDatabase();
        public frmMainMenu(string mail)
        {
            InitializeComponent();
            emailLoggedIn = mail;
        }
        private void frmMainMenu_Load(object sender, EventArgs e)
        {
            db.OpenConnection();
            CheckForAdmin(db.conn);
            Refresh(db.conn);
        }
        //A method which checks wether the user who's currently logged in also an admin
        private void CheckForAdmin(NpgsqlConnection conn) 
        {
            string query = "SELECT * FROM \"app_user\" WHERE email= @Mail AND role_id = @Role";
            NpgsqlCommand command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("@Mail", emailLoggedIn);
            command.Parameters.AddWithValue("@Role", 1);
            command.ExecuteNonQuery();
            int count = Convert.ToInt32(command.ExecuteScalar());
            if (count > 0) //if the row/user found is an admin, enable 2 admin options
            {
                btnRequests.Enabled = true;
                btnChangeStatus.Enabled = true;
            }
            else
            {
                btnRequests.Enabled = false;
                btnChangeStatus.Enabled = false;
            }
        }
        //Refresh display of all posts from the owners whose status is "Active"
        private void Refresh(NpgsqlConnection conn) 
        {
            string query = "SELECT id, title, content, date, name, surname, email, app_user_id FROM post_v WHERE status LIKE '%Active%' ORDER BY date DESC";

            NpgsqlDataAdapter adapterPost = new NpgsqlDataAdapter(query, conn);
            DataSet dataSetPost = new DataSet();
            adapterPost.Fill(dataSetPost);

            dgvPosts.DataSource = dataSetPost.Tables[0];
        }
        //Upon selecting each row in dgv, call a method
        private void dgvPosts_SelectionChanged(object sender, EventArgs e) 
        {
            CheckPostOwnerMatchingUserLoggedIn();
            ShowPost();
        }
        //A method to show the full text (content) of the post selected
        private void ShowPost()
        {
            string postText = dgvPosts.CurrentRow.Cells[2].Value.ToString();
            txtPost.Text = postText;
        }
        //A method called upon dgv selection changed which checks if the user who's currently logged in also the owner of the post chosen
        private void CheckPostOwnerMatchingUserLoggedIn() 
        {
            string email = dgvPosts.CurrentRow.Cells[6].Value.ToString();
            if (email == emailLoggedIn)
            {
                btnDelete.Enabled = true;
                btnEditPost.Enabled = true;
            }
            else
            {
                btnDelete.Enabled = false;
                btnEditPost.Enabled = false;
            }
        }
        //Request button event which opens a form called frmRequests
        private void btnRequests_Click(object sender, EventArgs e) 
        {
            frmRequests frmRequests = new frmRequests();
            frmRequests.ShowDialog();
        }
        //Change status button event which opens a form called frmListOfUsers
        private void btnChangeStatus_Click(object sender, EventArgs e) 
        {
            frmListOfUsers frmListOfUsers = new frmListOfUsers();
            frmListOfUsers.ShowDialog();
        }
        //Refresh button event which calls a method for refreshing dgv display
        private void btnRefresh_Click(object sender, EventArgs e) 
        {
            Refresh(db.conn);
        }
        //Create button even which opens a form called frmPost for creating new post
        private void btnCreate_Click(object sender, EventArgs e) 
        {
            idUserLoggedIn = GetUserID(db.conn); //calling a method which gets an id from user who's currently logged in
            keyOwner = 1;
            idPost = 0;
            frmPost frmPost = new frmPost(idUserLoggedIn, keyOwner, idPost);
            frmPost.ShowDialog();
        }
        //A method which sends a query to db to get user's ID, matched with email that was sent here from previous form
        private int GetUserID(NpgsqlConnection conn) 
        {
            string query = "SELECT * FROM \"app_user\" WHERE email= @Mail";
            NpgsqlCommand command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("@Mail", emailLoggedIn);
            NpgsqlDataReader reader = command.ExecuteReader();
            reader.Read();
            int idUser = int.Parse(reader[0].ToString());
            reader.Close();
            return idUser;
        }
        //Delete button event which calls a method for deleting chosen post
        private void btnDelete_Click(object sender, EventArgs e) 
        {
            DeletePost(db.conn);
        }
        //A method which sends a query to db to delete chosen post
        private void DeletePost(NpgsqlConnection conn) 
        {
            string query = "DELETE FROM \"post\" WHERE id=@id";
            NpgsqlCommand command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("@id", int.Parse(dgvPosts.CurrentRow.Cells[0].Value.ToString()));
            command.ExecuteNonQuery();
            MessageBox.Show("Post successfully deleted!", "Message!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Refresh(db.conn);
        }
        //Edit button event which calls a method for editing chosen post
        private void btnEditPost_Click(object sender, EventArgs e) 
        {
            EditPost();
        }
        //A method which opens a form called frmPost for deleting chosen post
        private void EditPost() 
        {
            keyOwner = 2;
            idPost = GetPostID(); //calling a method which gets an id from post chosen on dgv
            idUserLoggedIn = GetUserID(db.conn);
            frmPost frmPost = new frmPost(idUserLoggedIn, keyOwner, idPost);
            frmPost.ShowDialog();
        }
        //Show comments button event which calls a method for showing comments 
        private void btnComments_Click(object sender, EventArgs e) 
        {
            ShowComments();
        }
        //A method which opens a form called frmComments which shows all the comments for a certain post
        private void ShowComments() 
        {
            idPost = GetPostID();
            idPostOwner = GetPostOwnerID(); //calling a method which gets an id from dgv which says who's the post owner
            idUserLoggedIn = GetUserID(db.conn);
            frmComments frmComments = new frmComments(idPost, idPostOwner, idUserLoggedIn);
            frmComments.ShowDialog();
        }
        //A method for getting an id of the post selected
        private int GetPostID() 
        {
            int idPost = int.Parse(dgvPosts.CurrentRow.Cells[0].Value.ToString());
            return idPost;
        }
        //A method for getting an user id who is also an owner of the post selected
        private int GetPostOwnerID() 
        {
            int idPostOwner = int.Parse(dgvPosts.CurrentRow.Cells[7].Value.ToString());
            return idPostOwner;
        }

    }
}
